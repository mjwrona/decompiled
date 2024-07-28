// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetAncestorIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetAncestorIdsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetAncestorIdsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetAncestorIdsRequest";

    public ICollection<Guid> ObjectIds { get; set; }

    public override string ToString()
    {
      ICollection<Guid> objectIds = this.ObjectIds;
      return "GetAncestorIds{ObjectId=" + AadQueryUtils.ToString(objectIds != null ? objectIds.Select<Guid, string>((Func<Guid, string>) (x => x.ToString())) : (IEnumerable<string>) null) + "}";
    }

    internal override void Validate() => ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");

    internal override MsGraphGetAncestorIdsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750140, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetAncestorIdsRequest), (Func<string>) (() => "Calling Microsoft Graph API for 'GetAncestorIds' with parameters ='" + (this.ObjectIds.Serialize<ICollection<Guid>>() ?? "null") + "'"));
        BatchRequestContent batchRequestContent = new BatchRequestContent();
        List<(Guid, string)> list = this.ObjectIds.Select<Guid, (Guid, string)>((Func<Guid, (Guid, string)>) (oid => MsGraphGetAncestorIdsRequest.AddRequestToBatch(oid, graphServiceClient, batchRequestContent))).ToList<(Guid, string)>();
        BatchResponseContent batchResponses = context.RunSynchronously<BatchResponseContent>((Func<Task<BatchResponseContent>>) (() => ((BaseClient) graphServiceClient).Batch.Request().PostAsync(batchRequestContent)));
        if (batchResponses == null)
          throw new MicrosoftGraphException("Microsoft Graph API Get Deleted Objects call returned null");
        Dictionary<Guid, MsGraphGetAncestorIdResponse> dictionary = new Dictionary<Guid, MsGraphGetAncestorIdResponse>();
        (Guid, string) request;
        foreach ((_, _) in list)
        {
          try
          {
            DirectoryObjectGetMemberObjectsCollectionResponse collectionResponse = context.RunSynchronously<DirectoryObjectGetMemberObjectsCollectionResponse>((Func<Task<DirectoryObjectGetMemberObjectsCollectionResponse>>) (() => batchResponses.GetResponseByIdAsync<DirectoryObjectGetMemberObjectsCollectionResponse>(request.Item2)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            dictionary.Add(request.Item1, new MsGraphGetAncestorIdResponse()
            {
              Ancestors = (ISet<Guid>) ((IEnumerable<string>) collectionResponse.Value).Select<string, Guid>(MsGraphGetAncestorIdsRequest.\u003C\u003EO.\u003C0\u003E__Parse ?? (MsGraphGetAncestorIdsRequest.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, Guid>(Guid.Parse))).ToHashSet<Guid>()
            });
          }
          catch (ServiceException ex)
          {
            dictionary.Add(request.Item1, new MsGraphGetAncestorIdResponse()
            {
              Exception = (Exception) ex
            });
            if (ex.IsResourceNotFoundError())
              context.TraceAlways(44750143, TraceLevel.Info, "VisualStudio.Services.Aad", "Graph", string.Format("Microsoft Graph API returned Not Found for {0} oid. ex: {1}", (object) request.Item1, (object) ((Exception) ex).Message));
            else
              context.TraceException(44750141, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", (Exception) ex);
          }
        }
        return new MsGraphGetAncestorIdsResponse()
        {
          Ancestors = (IDictionary<Guid, MsGraphGetAncestorIdResponse>) dictionary
        };
      }
      finally
      {
        context.Trace(44750142, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetAncestorIdsRequest), "Leaving Microsoft Graph API for 'GetAncestorIds'");
      }
    }

    private static (Guid objectId, string requestId) AddRequestToBatch(
      Guid objectId,
      GraphServiceClient graphServiceClient,
      BatchRequestContent batchRequestContent)
    {
      IDirectoryObjectGetMemberObjectsRequest memberObjectsRequest = graphServiceClient.DirectoryObjects[objectId.ToString()].GetMemberObjects(new bool?(false)).Request((IEnumerable<Option>) null);
      HttpRequestMessage httpRequestMessage = ((IBaseRequest) memberObjectsRequest).GetHttpRequestMessage();
      httpRequestMessage.Method = HttpMethod.Post;
      httpRequestMessage.Content = SerializerExtentions.SerializeAsJsonContent(((BaseClient) graphServiceClient).HttpProvider.Serializer, (object) memberObjectsRequest.RequestBody);
      string str = batchRequestContent.AddBatchRequestStep(httpRequestMessage);
      return (objectId, str);
    }
  }
}
