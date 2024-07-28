// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetAncestorIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetAncestorIdsRequest : AadGraphClientRequest<GetAncestorIdsResponse>
  {
    private const bool SecurityEnabledOnly = false;
    private static readonly string Body = GetAncestorIdsRequest.ConstructRequestBody(false);

    public AadObjectType ObjectType { get; set; }

    public IEnumerable<Guid> ObjectIds { get; set; }

    public int Expand { get; set; }

    public override string ToString() => string.Format("GetAncestorIdsRequest:ObjectIds={0},ObjectType={1},Expand={2}", this.ObjectIds == null ? (object) "null" : (object) this.ObjectIds.Serialize<IEnumerable<Guid>>(), (object) this.ObjectType, (object) this.Expand);

    internal override void Validate()
    {
      if (this.ObjectType != AadObjectType.User && this.ObjectType != AadObjectType.Group && this.ObjectType != AadObjectType.ServicePrincipal)
        throw new ArgumentException("Unsupported object type: " + this.ObjectType.ToString());
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");
      int num = this.ObjectIds.Count<Guid>();
      if (num > 5)
        throw new ArgumentException(string.Format("Number of identifiers ({0}) exceeds maximum ({1}).", (object) num, (object) 5));
      if (this.Expand != -1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override GetAncestorIdsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      return this.ExecuteGetMemberObjectsBatch(context, connection);
    }

    internal GetAncestorIdsResponse ExecuteGetMemberGroupsBatch(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(44741301, "VisualStudio.Services.Aad", "Graph", nameof (ExecuteGetMemberGroupsBatch));
        List<Guid> objectIds = new HashSet<Guid>(this.ObjectIds).ToList<Guid>();
        context.TraceConditionally(44741304, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Getting ancestors of aad entities " + objectIds.Serialize<List<Guid>>()));
        Expression<Action>[] groupsBatchRequests = this.InitializeGetMemberGroupsBatchRequests(connection, (IList<Guid>) objectIds);
        IList<BatchResponseItem> batchResponses = connection.ExecuteBatch(groupsBatchRequests);
        Dictionary<Guid, GetAncestorIdResponse> ancestors = GetAncestorIdsRequest.PopulateAncestorsIdsResponse(context, batchResponses, (IList<Guid>) objectIds);
        context.TraceConditionally(44741308, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Result ancestors: " + (ancestors == null ? "null" : ancestors.Serialize<Dictionary<Guid, GetAncestorIdResponse>>())));
        return new GetAncestorIdsResponse()
        {
          Ancestors = (IDictionary<Guid, GetAncestorIdResponse>) ancestors
        };
      }
      finally
      {
        context.TraceLeave(44741309, "VisualStudio.Services.Aad", "Graph", nameof (ExecuteGetMemberGroupsBatch));
      }
    }

    internal GetAncestorIdsResponse ExecuteGetMemberObjectsBatch(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(44741321, "VisualStudio.Services.Aad", "Graph", nameof (ExecuteGetMemberObjectsBatch));
        List<Guid> objectIds = new HashSet<Guid>(this.ObjectIds).ToList<Guid>();
        context.TraceConditionally(44741324, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Getting ancestors of aad entities " + objectIds.Serialize<List<Guid>>()));
        BatchRequestItem[] objectsBatchRequests = this.InitializeGetMemberObjectsBatchRequests(connection, (IList<Guid>) objectIds);
        IList<BatchResponseItem> batchResponses = connection.ExecuteBatch((IList<BatchRequestItem>) objectsBatchRequests);
        Dictionary<Guid, GetAncestorIdResponse> ancestors = GetAncestorIdsRequest.PopulateAncestorsIdsResponse(context, batchResponses, (IList<Guid>) objectIds);
        context.TraceConditionally(44741328, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Result ancestors: " + (ancestors == null ? "null" : ancestors.Serialize<Dictionary<Guid, GetAncestorIdResponse>>())));
        return new GetAncestorIdsResponse()
        {
          Ancestors = (IDictionary<Guid, GetAncestorIdResponse>) ancestors
        };
      }
      finally
      {
        context.TraceLeave(44741329, "VisualStudio.Services.Aad", "Graph", nameof (ExecuteGetMemberObjectsBatch));
      }
    }

    internal virtual Uri GetRequestUrl(
      GraphConnection connection,
      Type directoryObjectType,
      string objectId)
    {
      return Utils.GetRequestUri(connection, directoryObjectType, objectId, "getMemberObjects");
    }

    private DirectoryObject InitializeDirectoryObject(Guid objectId)
    {
      switch (this.ObjectType)
      {
        case AadObjectType.User:
          User user = new User();
          user.ObjectId = objectId.ToString();
          return (DirectoryObject) user;
        case AadObjectType.Group:
          Group group = new Group();
          group.ObjectId = objectId.ToString();
          return (DirectoryObject) group;
        default:
          throw new AadInternalException("Unexpected object type: " + this.ObjectType.ToString());
      }
    }

    private Type GetDirectoryObjectType()
    {
      switch (this.ObjectType)
      {
        case AadObjectType.User:
          return typeof (User);
        case AadObjectType.Group:
          return typeof (Group);
        case AadObjectType.ServicePrincipal:
          return typeof (ServicePrincipal);
        default:
          throw new AadInternalException("Unexpected object type: " + this.ObjectType.ToString());
      }
    }

    private static Dictionary<Guid, GetAncestorIdResponse> PopulateAncestorsIdsResponse(
      IVssRequestContext context,
      IList<BatchResponseItem> batchResponses,
      IList<Guid> objectIds)
    {
      if (objectIds.Count != batchResponses.Count)
        throw new GetAncestorIdException(string.Format("Mismatch in Batch response count, expected:{0} actual:{1}", (object) objectIds.Count, (object) batchResponses.Count));
      int num = 0;
      Dictionary<Guid, GetAncestorIdResponse> dictionary = new Dictionary<Guid, GetAncestorIdResponse>();
      foreach (BatchResponseItem batchResponse in (IEnumerable<BatchResponseItem>) batchResponses)
      {
        Guid objectId = objectIds[num++];
        if (batchResponse.Exception != null)
        {
          context.TraceException(44741306, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", (Exception) batchResponse.Exception);
          dictionary[objectId] = new GetAncestorIdResponse()
          {
            Exception = (Exception) batchResponse.Exception
          };
        }
        else if (batchResponse.Failed)
        {
          context.Trace(44741307, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", string.Format("{0} BatchResponseItem failed for objectId:{1}", (object) nameof (GetAncestorIdsRequest), (object) objectId));
          dictionary[objectId] = new GetAncestorIdResponse()
          {
            Exception = (Exception) new GetAncestorIdException(string.Format("Failure in batch response item for objectId:{0}", (object) objectId))
          };
        }
        else
          dictionary[objectId] = new GetAncestorIdResponse()
          {
            Ancestors = (ISet<Guid>) new HashSet<Guid>(batchResponse.ResultSet.MixedResults.Select<string, Guid>((Func<string, Guid>) (s => AadGraphClient.CreateGuid(s))))
          };
      }
      return dictionary;
    }

    private Expression<Action>[] InitializeGetMemberGroupsBatchRequests(
      GraphConnection connection,
      IList<Guid> objectIds)
    {
      int num = 0;
      Expression<Action>[] groupsBatchRequests = new Expression<Action>[objectIds.Count];
      foreach (Guid objectId in (IEnumerable<Guid>) objectIds)
      {
        DirectoryObject directoryObject = this.InitializeDirectoryObject(objectId);
        groupsBatchRequests[num++] = (Expression<Action>) (() => connection.GetMemberGroups(directoryObject, false));
      }
      return groupsBatchRequests;
    }

    private BatchRequestItem[] InitializeGetMemberObjectsBatchRequests(
      GraphConnection connection,
      IList<Guid> objectIds)
    {
      BatchRequestItem[] objectsBatchRequests = new BatchRequestItem[objectIds.Count];
      Type directoryObjectType = this.GetDirectoryObjectType();
      int num = 0;
      foreach (Guid objectId in (IEnumerable<Guid>) objectIds)
      {
        Uri requestUrl = this.GetRequestUrl(connection, directoryObjectType, objectId.ToString());
        objectsBatchRequests[num++] = new BatchRequestItem("POST", true, requestUrl, (WebHeaderCollection) null, GetAncestorIdsRequest.Body);
      }
      return objectsBatchRequests;
    }

    private static string ConstructRequestBody(bool securityEnabledOnly) => Microsoft.Azure.ActiveDirectory.GraphClient.SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
    {
      [nameof (securityEnabledOnly)] = securityEnabledOnly.ToString()
    });
  }
}
