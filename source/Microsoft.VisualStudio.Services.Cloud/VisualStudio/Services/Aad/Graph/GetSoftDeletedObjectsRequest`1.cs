// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetSoftDeletedObjectsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetSoftDeletedObjectsRequest<T> : 
    AadGraphClientRequest<GetSoftDeletedObjectsResponse<T>>
    where T : AadObject
  {
    public IEnumerable<Guid> ObjectIds { get; set; }

    internal override void Validate()
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.ObjectIds, "ObjectIds");
      int num = this.ObjectIds.Count<Guid>();
      if (num > 5)
        throw new ArgumentException(string.Format("Number of identifiers ({0}) exceeds maximum ({1}).", (object) num, (object) 5));
    }

    internal override GetSoftDeletedObjectsResponse<T> Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(4100001, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
        List<Guid> objectIds = new HashSet<Guid>(this.ObjectIds).ToList<Guid>();
        context.TraceConditionally(4100002, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Getting ancestors of aad entities " + objectIds.Serialize<List<Guid>>()));
        IList<BatchRequestItem> batchRequests = this.InitializeBatchRequests(connection, objectIds);
        IList<BatchResponseItem> batchResponses = connection.ExecuteBatch(batchRequests);
        IDictionary<Guid, GetSoftDeletedObjectResponse<T>> objects = this.PopulateResponse(context, batchResponses, objectIds);
        context.TraceConditionally(4100003, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Result ancestors: " + (objects == null ? "null" : objects.Serialize<IDictionary<Guid, GetSoftDeletedObjectResponse<T>>>())));
        GetSoftDeletedObjectsResponse<T> deletedObjectsResponse = new GetSoftDeletedObjectsResponse<T>();
        deletedObjectsResponse.Objects.AddRange<KeyValuePair<Guid, GetSoftDeletedObjectResponse<T>>, IDictionary<Guid, GetSoftDeletedObjectResponse<T>>>(objects.Select<KeyValuePair<Guid, GetSoftDeletedObjectResponse<T>>, KeyValuePair<Guid, GetSoftDeletedObjectResponse<T>>>((Func<KeyValuePair<Guid, GetSoftDeletedObjectResponse<T>>, KeyValuePair<Guid, GetSoftDeletedObjectResponse<T>>>) (k => k)));
        return deletedObjectsResponse;
      }
      finally
      {
        context.TraceLeave(4100004, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
      }
    }

    private IDictionary<Guid, GetSoftDeletedObjectResponse<T>> PopulateResponse(
      IVssRequestContext context,
      IList<BatchResponseItem> batchResponses,
      List<Guid> objectIds)
    {
      if (objectIds.Count != batchResponses.Count)
        throw new GetSoftDeletedObjectsException(string.Format("Mismatch in Batch response count, expected:{0} actual:{1}", (object) objectIds.Count, (object) batchResponses.Count));
      int num = 0;
      Dictionary<Guid, GetSoftDeletedObjectResponse<T>> dictionary = new Dictionary<Guid, GetSoftDeletedObjectResponse<T>>();
      foreach (BatchResponseItem batchResponse in (IEnumerable<BatchResponseItem>) batchResponses)
      {
        Guid objectId = objectIds[num++];
        if (batchResponse.Exception != null)
        {
          if (batchResponse.Exception.Code == "Request_ResourceNotFound")
          {
            dictionary[objectId] = new GetSoftDeletedObjectResponse<T>()
            {
              Object = default (T)
            };
          }
          else
          {
            context.TraceException(4100005, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", (Exception) batchResponse.Exception);
            dictionary[objectId] = new GetSoftDeletedObjectResponse<T>()
            {
              Exception = (Exception) batchResponse.Exception
            };
          }
        }
        else if (batchResponse.Failed)
        {
          context.Trace(4100006, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", string.Format("{0} BatchResponseItem failed for objectId:{1}", (object) "GetAncestorIdsRequest", (object) objectId));
          dictionary[objectId] = new GetSoftDeletedObjectResponse<T>()
          {
            Exception = (Exception) new GetSoftDeletedObjectsException(string.Format("Failure in batch response item for objectId:{0}", (object) objectId))
          };
        }
        else if (batchResponse.ResultSet.MixedResults.Count != 1)
        {
          context.Trace(4100007, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", string.Format("Got incorrect BatchResponseItem for objectId:{0}", (object) objectId));
          dictionary[objectId] = new GetSoftDeletedObjectResponse<T>()
          {
            Exception = (Exception) new GetSoftDeletedObjectsException(string.Format("Failure in batch response item for objectId:{0}. Did not get single response as expected.", (object) objectId))
          };
        }
        else
          dictionary[objectId] = this.GetObject(batchResponse.ResultSet.MixedResults.First<string>(), objectId);
      }
      return (IDictionary<Guid, GetSoftDeletedObjectResponse<T>>) dictionary;
    }

    private GetSoftDeletedObjectResponse<T> GetObject(string json, Guid objectId)
    {
      object target = new AadJsonConverter().ReadJson((JsonReader) new JsonTextReader((TextReader) new StringReader(json)), this.GetDirectoryObjectType(), (object) json, JsonSerializer.CreateDefault());
      GetSoftDeletedObjectResponse<T> deletedObjectResponse = new GetSoftDeletedObjectResponse<T>();
      T obj = (T) this.Convert(target);
      if (obj.ObjectId != objectId)
        deletedObjectResponse.Exception = (Exception) new GetSoftDeletedObjectsException(string.Format("We did not get expected objectid. Expected:{0}. Actual: {1}.", (object) objectId, (object) obj.ObjectId));
      else
        deletedObjectResponse.Object = obj;
      return deletedObjectResponse;
    }

    private AadObject Convert(object target)
    {
      if (typeof (T) == typeof (AadGroup))
        return (AadObject) AadGraphClient.ConvertGroup((Group) target);
      if (typeof (T) == typeof (AadUser))
        return (AadObject) AadGraphClient.ConvertUser((User) target);
      throw new GetSoftDeletedObjectsException("Unsupported deleted object returned.");
    }

    private IList<BatchRequestItem> InitializeBatchRequests(
      GraphConnection connection,
      List<Guid> objectIds)
    {
      BatchRequestItem[] batchRequestItemArray = new BatchRequestItem[objectIds.Count];
      Type directoryObjectType = this.GetDirectoryObjectType();
      int num = 0;
      foreach (Guid objectId in objectIds)
      {
        Uri requestUrl = this.GetRequestUrl(connection, directoryObjectType, objectId.ToString());
        batchRequestItemArray[num++] = new BatchRequestItem("GET", false, requestUrl, (WebHeaderCollection) null, (string) null);
      }
      return (IList<BatchRequestItem>) batchRequestItemArray;
    }

    private Uri GetRequestUrl(
      GraphConnection connection,
      Type directoryObjectType,
      string objectId)
    {
      Func<Type, string> func = (Func<Type, string>) (objectType =>
      {
        string name = objectType.Name;
        return CultureInfo.CurrentCulture.TextInfo.ToLower(name[0]).ToString() + name.Substring(1) + "s";
      });
      return new Uri(Utils.GetRequestUri(connection, directoryObjectType, objectId).ToString().Replace(func(directoryObjectType), "deletedDirectoryObjects"));
    }

    private Type GetDirectoryObjectType()
    {
      Type type = typeof (T);
      if (type == typeof (AadGroup))
        return typeof (Group);
      if (type == typeof (AadUser))
        return typeof (User);
      throw new InvalidOperationException("Unsupported type used for fetching deleted objects.");
    }
  }
}
