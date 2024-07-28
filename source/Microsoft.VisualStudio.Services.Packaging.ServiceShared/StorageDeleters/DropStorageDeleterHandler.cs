// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.DropStorageDeleterHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Drop.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public class DropStorageDeleterHandler : 
    IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult>,
    IHaveInputType<IEnumerable<IStorageDeletionRequest>>,
    IHaveOutputType<SimpleResult>
  {
    private readonly IDropHttpClient dropClient;
    private readonly ITracerService tracerService;

    public DropStorageDeleterHandler(IDropHttpClient dropClient, ITracerService tracerService)
    {
      this.dropClient = dropClient;
      this.tracerService = tracerService;
    }

    public async Task<SimpleResult> Handle(IEnumerable<IStorageDeletionRequest> requests)
    {
      List<IStorageDeletionRequest<DropStorageId>> list = requests.Select<IStorageDeletionRequest, IStorageDeletionRequest<DropStorageId>>((Func<IStorageDeletionRequest, IStorageDeletionRequest<DropStorageId>>) (r => r as IStorageDeletionRequest<DropStorageId>)).ToList<IStorageDeletionRequest<DropStorageId>>();
      if (list.Any<IStorageDeletionRequest<DropStorageId>>((Func<IStorageDeletionRequest<DropStorageId>, bool>) (r => r == null)))
        return SimpleResult.CouldNotProcess;
      await this.ProcessRequests(list);
      return SimpleResult.Processed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private async Task ProcessRequests(
      List<IStorageDeletionRequest<DropStorageId>> requestList)
    {
      DropStorageDeleterHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ProcessRequests)))
      {
        foreach (IStorageDeletionRequest<DropStorageId> request in requestList)
        {
          try
          {
            DropItem dropItemToBeDeleted = await sendInTheThisObject.dropClient.GetDropAsync(request.StorageId.DropName);
            if (dropItemToBeDeleted == null)
              break;
            if (!(await sendInTheThisObject.dropClient.DeleteDropAsync(dropItemToBeDeleted, false)).IsSuccessStatusCode)
              throw new CannotDeleteDropItemException(Resources.Error_CannotDeleteDropItem((object) dropItemToBeDeleted.Name));
            dropItemToBeDeleted = (DropItem) null;
          }
          catch (AccessCheckException ex) when (sendInTheThisObject.dropClient.IgnoreDeleteAccessDeniedException)
          {
            tracer.TraceInfo(string.Format("{0} access denied exception while attempting to delete drop with id {1}", (object) nameof (DropStorageDeleterHandler), (object) request.StorageId));
            tracer.TraceException((Exception) ex);
          }
          catch (DropNotFoundException ex)
          {
            tracer.TraceInfo(string.Format("{0} could not find the drop with id {1}", (object) nameof (DropStorageDeleterHandler), (object) request.StorageId));
            tracer.TraceException((Exception) ex);
          }
        }
      }
    }
  }
}
