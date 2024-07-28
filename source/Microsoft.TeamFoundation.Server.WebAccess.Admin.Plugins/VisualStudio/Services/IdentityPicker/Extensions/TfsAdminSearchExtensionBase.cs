// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Extensions.TfsAdminSearchExtensionBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Extensions
{
  public class TfsAdminSearchExtensionBase : IdentityPickerExtension
  {
    internal string projectScopeName;
    internal string collectionScopeName;
    internal Guid extensionId = Guid.Empty;
    internal IList<string> constraintList;
    internal const string ProjectScopeNameKey = "ProjectScopeName";
    internal const string CollectionScopeNameKey = "CollectionScopeName";
    internal const string ConstraintsKey = "Constraints";
    private static readonly Guid s_ExtensionId = Guid.Parse("F12CA7AD-00EE-424F-B6D7-9123A60F424F");
    private static readonly Type s_ObservedOperationType = typeof (Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest);
    internal static string Area = "VSS.IdentityPicker";
    internal static string Layer = "Extensions";
    private static int TfsAdminSearchExtensionBase_EvaluateApplicability_Enter = 10030701;
    private static int TfsAdminSearchExtensionBase_EvaluateApplicability_Exception = 10030708;
    private static int TfsAdminSearchExtensionBase_EvaluateApplicability_Leave = 10030709;
    private static int TfsAdminSearchExtensionBase_Invoke_Enter = 10030711;
    private static int TfsAdminSearchExtensionBase_Invoke_Exception = 10030718;
    private static int TfsAdminSearchExtensionBase_Invoke_Leave = 10030719;
    private static int TfsAdminSearchExtensionBase_ComputeResponse_NullResults_Exception = 10030724;
    private TfsAdminSearchExtensionBase.ResponseMerger d_UnionResponses = TfsAdminSearchExtensionBase.\u003C\u003EO.\u003C0\u003E__UnionSearchResponses ?? (TfsAdminSearchExtensionBase.\u003C\u003EO.\u003C0\u003E__UnionSearchResponses = new TfsAdminSearchExtensionBase.ResponseMerger(TfsAdminSearchExtensionBase.UnionSearchResponses));

    public TfsAdminSearchExtensionBase() => this.ExtensionId = TfsAdminSearchExtensionBase.s_ExtensionId;

    public override Type GetExtendedOperationType() => TfsAdminSearchExtensionBase.s_ObservedOperationType;

    public override bool EvaluateApplicability(IVssRequestContext context, OperationRequest request)
    {
      context.TraceEnter(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_EvaluateApplicability_Enter, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, nameof (EvaluateApplicability));
      try
      {
        if (!request.GetType().Equals(TfsAdminSearchExtensionBase.s_ObservedOperationType) || !context.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection) || !(request is Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest))
          return false;
        if (!(request.ExtensionData is SearchOptions extensionData))
          throw new IdentityPickerValidateException("ValidateSearchRequest could not construct valid SearchOptions from this request");
        this.ParseExtensionData(extensionData);
        this.ValidateSearchRequest(request as Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest);
        return true;
      }
      catch (Exception ex)
      {
        context.TraceException(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_EvaluateApplicability_Exception, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, ex);
        return false;
      }
      finally
      {
        context.TraceLeave(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_EvaluateApplicability_Leave, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, nameof (EvaluateApplicability));
      }
    }

    public override OperationResponse Invoke(
      IVssRequestContext requestContext,
      OperationRequest request)
    {
      requestContext.TraceEnter(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_Invoke_Enter, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, nameof (Invoke));
      try
      {
        if (!(request is Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest searchRequest))
          throw new IdentityPickerExtensionException("TfsAdminSearchExtensionBase could not validate that the request is a SearchRequest");
        if (!(request.ExtensionData is SearchOptions extensionData))
          throw new IdentityPickerValidateException("TfsAdminSearchExtensionBase: ExtensionData is not a SearchRequest");
        if (request.ExtensionData != null)
          extensionData.ParseOptions();
        List<string> list = searchRequest.OperationScopes.Select<string, string>((Func<string, string>) (x => x.TrimToLower())).ToList<string>();
        bool needToSearchImsGroups = searchRequest.IdentityTypes.Select<string, string>((Func<string, string>) (x => x.TrimToLower())).ToList<string>().Contains("Group".TrimToLower()) && list.Contains("IMS".TrimToLower());
        bool needToSearchProjectScope = needToSearchImsGroups && !string.IsNullOrWhiteSpace(this.projectScopeName);
        bool needToSearchCollectionScope = needToSearchImsGroups && !string.IsNullOrWhiteSpace(this.collectionScopeName);
        return (OperationResponse) this.ComputeResponse(requestContext, searchRequest, extensionData, needToSearchImsGroups, needToSearchCollectionScope, needToSearchProjectScope);
      }
      catch (Exception ex)
      {
        if (ex is IdentityPickerException)
        {
          throw;
        }
        else
        {
          requestContext.TraceException(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_Invoke_Exception, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, ex);
          throw new IdentityPickerExtensionException("TfsAdminSearchExtensionBase Invoke", ex);
        }
      }
      finally
      {
        requestContext.TraceLeave(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_Invoke_Leave, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, nameof (Invoke));
      }
    }

    private Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchResponse ComputeResponse(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest searchRequest,
      SearchOptions searchOptions,
      bool needToSearchImsGroups,
      bool needToSearchCollectionScope,
      bool needToSearchProjectScope)
    {
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse cumulativeResponse = (Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse) null;
      string empty = string.Empty;
      if (needToSearchCollectionScope && !string.IsNullOrWhiteSpace(this.collectionScopeName))
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.ProjectCollection);
        string query = searchRequest.Query;
        string queryTypeHint = searchRequest.QueryTypeHint;
        List<string> identityTypes = new List<string>();
        identityTypes.Add("Group".TrimToLower());
        List<string> operationScopes = new List<string>();
        operationScopes.Add("IMS".TrimToLower());
        SearchOptions searchOptions1 = searchOptions;
        IList<string> requestedProperties = searchRequest.RequestedProperties;
        string pagingToken = searchRequest.PagingToken;
        TfsAdminSearchExtensionBase.PrefixTransform transform = new TfsAdminSearchExtensionBase.PrefixTransform(this.CollectionPrefixTransformer);
        cumulativeResponse = this.MergeResponses(this.SearchInternal(requestContext1, query, queryTypeHint, (IList<string>) identityTypes, (IList<string>) operationScopes, searchOptions1, requestedProperties, pagingToken, transform), cumulativeResponse);
      }
      if (needToSearchProjectScope && !string.IsNullOrWhiteSpace(this.projectScopeName))
      {
        IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.ProjectCollection);
        string query = searchRequest.Query;
        string queryTypeHint = searchRequest.QueryTypeHint;
        List<string> identityTypes = new List<string>();
        identityTypes.Add("Group".TrimToLower());
        List<string> operationScopes = new List<string>();
        operationScopes.Add("IMS".TrimToLower());
        SearchOptions searchOptions2 = searchOptions;
        IList<string> requestedProperties = searchRequest.RequestedProperties;
        string pagingToken = searchRequest.PagingToken;
        TfsAdminSearchExtensionBase.PrefixTransform transform = new TfsAdminSearchExtensionBase.PrefixTransform(this.ProjectPrefixTransformer);
        cumulativeResponse = this.MergeResponses(this.SearchInternal(requestContext2, query, queryTypeHint, (IList<string>) identityTypes, (IList<string>) operationScopes, searchOptions2, requestedProperties, pagingToken, transform), cumulativeResponse);
      }
      if (needToSearchImsGroups && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        IVssRequestContext requestContext3 = requestContext.To(TeamFoundationHostType.Application);
        string query = searchRequest.Query;
        string queryTypeHint = searchRequest.QueryTypeHint;
        List<string> identityTypes = new List<string>();
        identityTypes.Add("Group".TrimToLower());
        List<string> operationScopes = new List<string>();
        operationScopes.Add("IMS".TrimToLower());
        SearchOptions searchOptions3 = searchOptions;
        IList<string> requestedProperties = searchRequest.RequestedProperties;
        string pagingToken = searchRequest.PagingToken;
        TfsAdminSearchExtensionBase.PrefixTransform transform = new TfsAdminSearchExtensionBase.PrefixTransform(this.TeamFoundationPrefixTransformer);
        cumulativeResponse = this.MergeResponses(this.SearchInternal(requestContext3, query, queryTypeHint, (IList<string>) identityTypes, (IList<string>) operationScopes, searchOptions3, requestedProperties, pagingToken, transform), cumulativeResponse);
      }
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse response = this.MergeResponses(this.SearchInternal(requestContext.To(TeamFoundationHostType.ProjectCollection), searchRequest.Query, searchRequest.QueryTypeHint, searchRequest.IdentityTypes, searchRequest.OperationScopes, searchOptions, searchRequest.RequestedProperties, searchRequest.PagingToken, (TfsAdminSearchExtensionBase.PrefixTransform) null), cumulativeResponse);
      TfsAdminSearchExtensionHelper.FilterSearchResponse(requestContext, this.constraintList, response);
      if (response == null)
      {
        requestContext.Trace(TfsAdminSearchExtensionBase.TfsAdminSearchExtensionBase_ComputeResponse_NullResults_Exception, TraceLevel.Error, TfsAdminSearchExtensionBase.Area, TfsAdminSearchExtensionBase.Layer, "ComputeResponse returned null");
        throw new IdentityPickerExtensionException("Unable to compute a non-null response for this request");
      }
      return new Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchResponse(response);
    }

    private Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse SearchInternal(
      IVssRequestContext requestContext,
      string query,
      string queryTypeHint,
      IList<string> identityTypes,
      IList<string> operationScopes,
      SearchOptions searchOptions,
      IList<string> requestedProperties,
      string pagingToken,
      TfsAdminSearchExtensionBase.PrefixTransform transform)
    {
      searchOptions.Options["MinResults"] = (object) 20;
      searchOptions.Options["MaxResults"] = (object) 20;
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest request1 = new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest(query, queryTypeHint, identityTypes, operationScopes, searchOptions, requestedProperties, (IList<string>) null, (IList<string>) null, pagingToken);
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse searchResponse1 = IdentityOperation.Search(requestContext, request1);
      IList<QueryTokenResult> results = searchResponse1.Results;
      if (results == null)
        return (Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse) null;
      if (results.Count == 0)
        return searchResponse1;
      List<QueryTokenResult> list1 = results.Where<QueryTokenResult>((Func<QueryTokenResult, bool>) (result => result.OptionalProperties.ContainsKey(QueryTokenResultProperties.PagingToken))).ToList<QueryTokenResult>();
      if (list1.Count == 0)
        return searchResponse1;
      List<QueryTokenResult> list2 = results.Where<QueryTokenResult>((Func<QueryTokenResult, bool>) (result => !result.OptionalProperties.ContainsKey(QueryTokenResultProperties.PagingToken))).ToList<QueryTokenResult>();
      foreach (QueryTokenResult queryTokenResult in list1)
      {
        string queryToken = queryTokenResult.QueryToken;
        string query1 = transform != null ? transform(queryToken) : queryToken;
        SearchRequestPagingToken requestPagingToken = JsonConvert.DeserializeObject<SearchRequestPagingToken>(list1.FirstOrDefault<QueryTokenResult>().PagingToken);
        requestPagingToken.Query = query1;
        string pagingToken1 = JsonConvert.SerializeObject((object) requestPagingToken);
        Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest request2 = new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest(query1, queryTypeHint, identityTypes, operationScopes, searchOptions, requestedProperties, (IList<string>) null, (IList<string>) null, pagingToken1);
        Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse searchResponse2 = IdentityOperation.Search(requestContext, request2);
        searchResponse2.Results.FirstOrDefault<QueryTokenResult>().QueryToken = queryToken;
        list2 = list2.Concat<QueryTokenResult>((IEnumerable<QueryTokenResult>) new List<QueryTokenResult>()
        {
          searchResponse2.Results.FirstOrDefault<QueryTokenResult>()
        }).ToList<QueryTokenResult>();
      }
      return new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse()
      {
        Results = (IList<QueryTokenResult>) list2
      };
    }

    private Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse MergeResponses(
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse newResponse,
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse cumulativeResponse)
    {
      if (newResponse == null)
        return cumulativeResponse;
      return cumulativeResponse != null ? this.d_UnionResponses(cumulativeResponse, newResponse) : newResponse;
    }

    private void ValidateSearchRequest(Microsoft.VisualStudio.Services.IdentityPicker.Operations.SearchRequest request)
    {
      if (request.OperationScopes == null || !request.OperationScopes.Select<string, string>((Func<string, string>) (x => x.TrimToLower())).ToList<string>().Contains("IMS".TrimToLower()) || request.IdentityTypes == null || !request.IdentityTypes.Select<string, string>((Func<string, string>) (x => x.TrimToLower())).ToList<string>().Contains("Group".TrimToLower()) || string.IsNullOrWhiteSpace(this.collectionScopeName) && string.IsNullOrWhiteSpace(this.projectScopeName))
        throw new IdentityPickerInvalidExtensionException("ValidateSearchRequest: This extension does not apply for the current request.");
    }

    private void ParseExtensionData(SearchOptions searchOptions)
    {
      string str1 = (string) null;
      if (searchOptions.Options.ContainsKey("ProjectScopeName"))
        str1 = searchOptions.Options["ProjectScopeName"] as string;
      string str2 = (string) null;
      if (searchOptions.Options.ContainsKey("CollectionScopeName"))
        str2 = searchOptions.Options["CollectionScopeName"] as string;
      Guid result = Guid.Empty;
      if (searchOptions.Options.ContainsKey("ExtensionId"))
        Guid.TryParse(searchOptions.Options["ExtensionId"].ToString(), out result);
      IList<string> stringList = (IList<string>) null;
      object obj = (object) null;
      if (searchOptions.Options.TryGetValue("Constraints", out obj) && obj != null)
        stringList = (IList<string>) ((JToken) obj).ToObject<List<string>>();
      this.projectScopeName = str1;
      this.collectionScopeName = str2;
      this.extensionId = result;
      this.constraintList = stringList;
    }

    private string CollectionPrefixTransformer(string prefix) => "[" + this.collectionScopeName + "]\\" + prefix;

    private string ProjectPrefixTransformer(string prefix) => "[" + this.projectScopeName + "]\\" + prefix;

    private string TeamFoundationPrefixTransformer(string prefix) => "[TEAM FOUNDATION]\\" + prefix;

    private static Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse UnionSearchResponses(
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse x,
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse y)
    {
      if (x == null && y == null)
        return (Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse) null;
      if (y == null)
      {
        IList<QueryTokenResult> results = x.Results;
        return new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse() { Results = results };
      }
      if (x == null)
      {
        IList<QueryTokenResult> results = y.Results;
        return new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse() { Results = results };
      }
      IList<QueryTokenResult> results1 = x.Results;
      IList<QueryTokenResult> results2 = y.Results;
      IList<QueryTokenResult> queryTokenResultList;
      if (results1 == null)
      {
        if (results2 == null)
          return (Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse) null;
        queryTokenResultList = results2;
      }
      else
      {
        if (results2 == null)
          ;
        queryTokenResultList = TfsAdminSearchExtensionBase.MergeResults(results1, results2);
      }
      return new Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse()
      {
        Results = queryTokenResultList
      };
    }

    private static IList<QueryTokenResult> MergeResults(
      IList<QueryTokenResult> xResults,
      IList<QueryTokenResult> yResults)
    {
      IEnumerable<QueryTokenResult> queryTokenResults = xResults.Concat<QueryTokenResult>((IEnumerable<QueryTokenResult>) yResults);
      Dictionary<string, QueryTokenResult> source = new Dictionary<string, QueryTokenResult>();
      foreach (QueryTokenResult queryTokenResult1 in queryTokenResults)
      {
        if (queryTokenResult1 != null)
        {
          IList<Identity> identityList = queryTokenResult1.Identities ?? (IList<Identity>) new List<Identity>();
          IDictionary<string, object> dictionary = queryTokenResult1.OptionalProperties ?? (IDictionary<string, object>) new Dictionary<string, object>();
          QueryTokenResult queryTokenResult2;
          if (source.TryGetValue(queryTokenResult1.QueryToken, out queryTokenResult2))
          {
            if (queryTokenResult2.Identities == null)
            {
              source[queryTokenResult1.QueryToken].Identities = identityList;
              source[queryTokenResult1.QueryToken].OptionalProperties = dictionary;
            }
            else
            {
              IList<Identity> list = (IList<Identity>) queryTokenResult2.Identities.Union<Identity>((IEnumerable<Identity>) identityList).ToList<Identity>();
              source[queryTokenResult1.QueryToken].Identities = list;
              foreach (KeyValuePair<string, object> keyValuePair in dictionary.Concat<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) queryTokenResult2.OptionalProperties))
              {
                if (!dictionary.ContainsKey(keyValuePair.Key))
                  dictionary.Add(new KeyValuePair<string, object>(keyValuePair.Key, keyValuePair.Value));
              }
              source[queryTokenResult1.QueryToken].OptionalProperties = dictionary;
            }
          }
          else
            source[queryTokenResult1.QueryToken] = new QueryTokenResult(queryTokenResult1.QueryToken, identityList, dictionary);
        }
      }
      return (IList<QueryTokenResult>) source.Select<KeyValuePair<string, QueryTokenResult>, QueryTokenResult>((Func<KeyValuePair<string, QueryTokenResult>, QueryTokenResult>) (r => r.Value)).ToList<QueryTokenResult>();
    }

    internal delegate Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse ResponseMerger(
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse x,
      Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchResponse y);

    internal delegate string PrefixTransform(string prefix);
  }
}
