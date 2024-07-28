// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters.GroupByStorageIdStorageDeletionHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters
{
  public class GroupByStorageIdStorageDeletionHandler : 
    IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult>,
    IHaveInputType<IEnumerable<IStorageDeletionRequest>>,
    IHaveOutputType<NullResult>
  {
    private readonly IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult> delegatedHandler;

    public GroupByStorageIdStorageDeletionHandler(
      IAsyncHandler<IEnumerable<IStorageDeletionRequest>, SimpleResult> delegatedHandler)
    {
      this.delegatedHandler = delegatedHandler;
    }

    public async Task<NullResult> Handle(IEnumerable<IStorageDeletionRequest> requests)
    {
      foreach (IEnumerable<IStorageDeletionRequest> request in requests.GroupBy<IStorageDeletionRequest, Type, IStorageDeletionRequest>((Func<IStorageDeletionRequest, Type>) (r =>
      {
        Type type = r.StorageId?.GetType();
        return (object) type != null ? type : typeof (NullResult);
      }), (Func<IStorageDeletionRequest, IStorageDeletionRequest>) (r => r)))
      {
        SimpleResult simpleResult = await this.delegatedHandler.Handle(request);
      }
      return (NullResult) null;
    }
  }
}
