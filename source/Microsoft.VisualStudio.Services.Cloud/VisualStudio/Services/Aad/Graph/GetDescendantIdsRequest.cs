// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetDescendantIdsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetDescendantIdsRequest : AadGraphClientPagedRequest<GetDescendantIdsResponse>
  {
    private const string TopQueryParameterName = "$top";
    private const string SkipTokenQueryParameterName = "$skiptoken";
    private const string TransitiveApiFragmentName = "$links/transitiveMembers";
    private const string SkipTokenDelimiter = "skiptoken=X";
    private const string DirectoryObjectsSegment = "directoryObjects/";
    private const string UrlToken = "url";
    private static readonly Dictionary<string, AadObjectType> GraphObjectTypeToAadObjectType = new Dictionary<string, AadObjectType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Microsoft.DirectoryServices.User",
        AadObjectType.User
      },
      {
        "Microsoft.DirectoryServices.Group",
        AadObjectType.Group
      }
    };

    public Guid ObjectId { get; set; }

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ObjectId, "ObjectId");
      int? maxResults1 = this.MaxResults;
      int num1 = 0;
      if (maxResults1.GetValueOrDefault() <= num1 & maxResults1.HasValue)
      {
        int? maxResults2 = this.MaxResults;
        ref int? local = ref maxResults2;
        throw new ArgumentException("Requested maximum results is less than the minimum allowed page size of 1", local.HasValue ? local.GetValueOrDefault().ToString() : (string) null);
      }
      int? maxResults3 = this.MaxResults;
      int num2 = 999;
      if (maxResults3.GetValueOrDefault() > num2 & maxResults3.HasValue)
      {
        maxResults3 = this.MaxResults;
        throw new ArgumentOutOfRangeException(maxResults3.ToString(), (object) 999.ToString(), "Requested maximum results is greater than the maximum page size threshold");
      }
    }

    internal override GetDescendantIdsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      try
      {
        context.TraceEnter(1035050, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
        Uri requestUri = this.ConstructRequestUri(connection);
        context.TraceConditionally(1035053, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "GetDescendantIds requested URI is : " + requestUri.Serialize<Uri>()));
        try
        {
          PagedResults<GraphObject> pagedResults = this.ExecuteGraphRequest(connection, requestUri);
          context.TraceConditionally(1035055, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "GetDescendantIds AAD Graph result: " + pagedResults.Serialize<PagedResults<GraphObject>>()));
          if (!pagedResults.MixedResults.IsNullOrEmpty<string>() && !pagedResults.Results.IsNullOrEmpty<GraphObject>())
          {
            context.TraceSerializedConditionally(1035062, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", "GetDescendantIds response contains both Mixed Result and Results in Paged Response :", (object) pagedResults);
            throw new AadException("GetDescendantIds response contains both Mixed Result and Results in Paged Response");
          }
          HashSet<Tuple<Guid, AadObjectType>> tupleSet = new HashSet<Tuple<Guid, AadObjectType>>();
          if (!pagedResults.Results.IsNullOrEmpty<GraphObject>())
          {
            foreach (GraphObject result in (IEnumerable<GraphObject>) pagedResults.Results)
            {
              Tuple<Guid, AadObjectType> identifier;
              if (this.TryExtractMemberIdentifier(context, result.TokenDictionary[(object) "url"], out identifier))
                tupleSet.Add(identifier);
            }
          }
          else if (!pagedResults.MixedResults.IsNullOrEmpty<string>())
          {
            foreach (JToken jtoken in (IEnumerable<JToken>) JToken.Parse(pagedResults.MixedResults.FirstOrDefault<string>()))
            {
              Tuple<Guid, AadObjectType> identifier;
              if (this.TryExtractMemberIdentifier(context, jtoken[(object) "url"], out identifier))
                tupleSet.Add(identifier);
            }
          }
          context.TraceSerializedConditionally(1035054, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", "GetDescendantIds resolved identifiers is : {0} with paging token {1}", (object) tupleSet, (object) pagedResults.PageToken);
          GetDescendantIdsResponse descendantIdsResponse = new GetDescendantIdsResponse();
          descendantIdsResponse.Identifiers = (ISet<Tuple<Guid, AadObjectType>>) tupleSet;
          descendantIdsResponse.PagingToken = pagedResults.IsLastPage ? (string) null : pagedResults.PageToken;
          return descendantIdsResponse;
        }
        catch (Exception ex)
        {
          context.TraceSerializedConditionally(1035060, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", "GetDescendantIds threw exception : {0} with request URI : {1}", (object) ex.Message, (object) requestUri);
          throw;
        }
      }
      finally
      {
        context.TraceLeave(1035051, "VisualStudio.Services.Aad", "Graph", nameof (Execute));
      }
    }

    private bool TryExtractMemberIdentifier(
      IVssRequestContext context,
      JToken graphObjectUrl,
      out Tuple<Guid, AadObjectType> identifier)
    {
      identifier = (Tuple<Guid, AadObjectType>) null;
      if (graphObjectUrl == null)
        throw new AadException("GetDescendantIds doesn't contains url");
      context.TraceConditionally(1035052, TraceLevel.Verbose, "VisualStudio.Services.Aad", "Graph", (Func<string>) (() => "Graph object url is : " + graphObjectUrl.Serialize<JToken>()));
      string[] segments = new Uri(graphObjectUrl.Value<string>()).Segments;
      int num = Array.IndexOf<string>(segments, "directoryObjects/");
      if (num <= 0 || segments.Length < num + 2)
      {
        context.TraceSerializedConditionally(1035063, TraceLevel.Error, "VisualStudio.Services.Aad", "Graph", "GetDescendantIds unable to parse url {0}", (object) graphObjectUrl);
        throw new AadException(string.Format("GetDescendantIds unable to parse url {0}", (object) graphObjectUrl));
      }
      string key = ((IEnumerable<string>) segments).ElementAt<string>(num + 2).TrimEnd('/');
      AadObjectType aadObjectType;
      if (!GetDescendantIdsRequest.GraphObjectTypeToAadObjectType.TryGetValue(key, out aadObjectType))
        return false;
      string objectId = ((IEnumerable<string>) segments).ElementAt<string>(num + 1).TrimEnd('/');
      identifier = new Tuple<Guid, AadObjectType>(AadGraphClient.CreateGuid(objectId), aadObjectType);
      return true;
    }

    internal virtual PagedResults<GraphObject> ExecuteGraphRequest(
      GraphConnection connection,
      Uri requestUri)
    {
      return Microsoft.Azure.ActiveDirectory.GraphClient.SerializationHelper.DeserializeJsonResponse<GraphObject>(Encoding.UTF8.GetString(connection.ClientConnection.DownloadData(requestUri, (WebHeaderCollection) null)), requestUri);
    }

    internal virtual Uri ConstructRequestUri(GraphConnection connection)
    {
      List<QueryParameter> queryParameters = new List<QueryParameter>();
      queryParameters.Add(new QueryParameter()
      {
        ParameterName = "$top",
        ParameterValue = this.MaxResults.ToString()
      });
      if (!this.PagingToken.IsNullOrEmpty<char>())
      {
        QueryParameter queryParameter = new QueryParameter()
        {
          ParameterName = "$skiptoken",
          ParameterValue = this.ParseSkipToken(this.PagingToken)
        };
        queryParameters.Add(queryParameter);
      }
      return Utils.GetRequestUri(connection, typeof (DirectoryObject), this.ObjectId.ToString(), (IList<QueryParameter>) queryParameters, "$links/transitiveMembers");
    }

    internal string ParseSkipToken(string resultSetPageToken) => resultSetPageToken.IndexOf("skiptoken=X", StringComparison.Ordinal) >= 0 ? resultSetPageToken.Substring(resultSetPageToken.IndexOf("skiptoken=X", StringComparison.Ordinal) + "skiptoken=X".Length - 1) : throw new ArgumentException("Unable to parse Skip Token value in Paging Token", resultSetPageToken);
  }
}
