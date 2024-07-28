// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetSoftDeletedObjectsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Aad.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetSoftDeletedObjectsRequest<T> : 
    MicrosoftGraphClientRequest<MsGraphGetSoftDeletedObjectsReponse<T>>
    where T : AadObject
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetSoftDeletedObjectsRequest";

    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate()
    {
      if (typeof (T) != typeof (AadGroup) && typeof (T) != typeof (AadUser))
        throw new InvalidOperationException("Only supported Type are User and Group for get soft deleted objects");
      if (this.ObjectIds.Count<Guid>() > 20)
        throw new ArgumentException(string.Format("Cannot have batch size greater than '{0}'.", (object) 20));
    }

    public override string ToString()
    {
      IEnumerable<Guid> objectIds = this.ObjectIds;
      return "GetSoftDeletedObjects{Objectids=" + AadQueryUtils.ToString(objectIds != null ? objectIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString())) : (IEnumerable<string>) null) + "}";
    }

    internal override MsGraphGetSoftDeletedObjectsReponse<T> Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750130, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetSoftDeletedObjectsRequest<T>), "Entering Microsoft Graph API for Get Deleted Objects");
        BatchRequestContent batchRequestContent = new BatchRequestContent();
        List<(Guid, string)> list = this.ObjectIds.Select<Guid, (Guid, string)>((Func<Guid, (Guid, string)>) (oid => (oid, batchRequestContent.AddBatchRequestStep((IBaseRequest) graphServiceClient.Directory.DeletedItems[oid.ToString()].Request())))).ToList<(Guid, string)>();
        BatchResponseContent batchResponses = context.RunSynchronously<BatchResponseContent>((Func<Task<BatchResponseContent>>) (() => ((BaseClient) graphServiceClient).Batch.Request().PostAsync(batchRequestContent)));
        if (batchResponses == null)
          throw new MicrosoftGraphException("Microsoft Graph API Get Deleted Objects call returned null");
        Dictionary<Guid, GetSoftDeletedObjectResponse<T>> dictionary = new Dictionary<Guid, GetSoftDeletedObjectResponse<T>>();
        (Guid, string) request;
        foreach ((_, _) in (IEnumerable<(Guid, string)>) list)
        {
          try
          {
            DirectoryObject directoryObject = context.RunSynchronously<DirectoryObject>((Func<Task<DirectoryObject>>) (() => batchResponses.GetResponseByIdAsync<DirectoryObject>(request.Item2)));
            GetSoftDeletedObjectResponse<T> deletedObjectResponse = new GetSoftDeletedObjectResponse<T>()
            {
              Object = MicrosoftGraphConverters.ConvertDirectoryObject(directoryObject) as T
            };
            dictionary.Add(request.Item1, deletedObjectResponse);
          }
          catch (ServiceException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
          {
            dictionary.Add(request.Item1, new GetSoftDeletedObjectResponse<T>()
            {
              Object = default (T)
            });
          }
          catch (ServiceException ex)
          {
            context.TraceException(4100005, TraceLevel.Error, "MicrosoftGraphClientRequest", nameof (MsGraphGetSoftDeletedObjectsRequest<T>), (Exception) ex);
            dictionary.Add(request.Item1, new GetSoftDeletedObjectResponse<T>()
            {
              Exception = (Exception) ex
            });
          }
        }
        return new MsGraphGetSoftDeletedObjectsReponse<T>()
        {
          DeletedObjects = (IDictionary<Guid, GetSoftDeletedObjectResponse<T>>) dictionary
        };
      }
      finally
      {
        context.Trace(44750132, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetSoftDeletedObjectsRequest<T>), "Leaving Microsoft Graph API for Get Deleted Objects");
      }
    }
  }
}
