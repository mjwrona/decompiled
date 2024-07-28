// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.SearchRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  public sealed class SearchRequest : AbstractSearchRequest, IOperationRequest
  {
    internal const char QueryTokenSeparator = ';';
    private ConcurrentDictionary<string, QueryTokenResult> queryTokenResultBuilder;
    private SearchRequestPagingToken requestPagingToken;
    private IdentityOperationHelper.ParsedTokens parsedQueryTokens;
    private SearchOptions searchOptions;
    private static IdentityProvider provider = (IdentityProvider) new DirectoryDiscoveryServiceAdapter();
    private const string c_disableSearchByAccountNames = "VisualStudio.Services.IdentityPicker.DisableSearchByAccountNames";

    public string PagingToken { get; set; }

    public SearchRequest(
      string query,
      string queryTypeHint,
      IList<string> identityTypes,
      IList<string> operationScopes,
      SearchOptions searchOptions,
      IList<string> requestedProperties,
      IList<string> filterByAncestorEntityIds,
      IList<string> filterByEntityIds,
      string pagingToken)
      : base(query, queryTypeHint, identityTypes, operationScopes, requestedProperties, filterByAncestorEntityIds, filterByEntityIds)
    {
      this.queryTokenResultBuilder = new ConcurrentDictionary<string, QueryTokenResult>();
      this.searchOptions = searchOptions ?? new SearchOptions();
      this.requestPagingToken = (SearchRequestPagingToken) null;
      if (string.IsNullOrEmpty(pagingToken))
        return;
      try
      {
        this.requestPagingToken = JsonConvert.DeserializeObject<SearchRequestPagingToken>(pagingToken);
      }
      catch (Exception ex)
      {
        this.requestPagingToken = (SearchRequestPagingToken) null;
        throw new IdentityPickerInvalidPagingTokenException("Invalid paging token", ex);
      }
    }

    public override void Validate(IVssRequestContext requestContext)
    {
      base.Validate(requestContext);
      this.requestPagingToken?.Validate(requestContext, this);
      List<string> stringList;
      if (string.IsNullOrEmpty(this.Query))
        stringList = new List<string>();
      else
        stringList = ((IEnumerable<string>) this.Query.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      if (stringList.Count > 100)
        throw new IdentityPickerValidateException("Identity resolution is limited to 100 semicolon separated tokens.");
    }

    public IOperationResponse Process(IVssRequestContext requestContext)
    {
      if (this.requestPagingToken == null)
      {
        try
        {
          IdentityOperationHelper.ParsedTokens identifiers;
          if (!IdentityOperationHelper.TryParseTokens(requestContext, this.Query, this.QueryTypeHint, out identifiers))
            return (IOperationResponse) new SearchResponse()
            {
              Results = (IList<QueryTokenResult>) this.queryTokenResultBuilder.Values.ToList<QueryTokenResult>()
            };
          this.parsedQueryTokens = identifiers;
        }
        catch (Exception ex)
        {
          Tracing.TraceException(requestContext, 125, ex);
          throw;
        }
      }
      else
      {
        if (this.requestPagingToken.PagingTokenType != SearchPagingTokenType.PrefixToken)
          throw new IdentityPickerInvalidPagingTokenException("PagingToken needs to be of the prefix type");
        this.parsedQueryTokens = new IdentityOperationHelper.ParsedTokens();
        this.parsedQueryTokens.Prefixes.Add(this.requestPagingToken.Query);
      }
      this.searchOptions.ParseOptions();
      if (this.requestPagingToken == null)
      {
        if (this.QueryTypeHint == QueryTypeHintEnum.UID)
        {
          this.SearchUniqueIdentifier(requestContext);
        }
        else
        {
          IdentityOperationHelper.CheckRequestByAuthorizedMember(requestContext);
          this.SearchPrefix(requestContext);
        }
        SearchRequest.HandleUnresolvedQueryTokens(this.parsedQueryTokens, (IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder);
        return (IOperationResponse) new SearchResponse()
        {
          Results = (IList<QueryTokenResult>) this.queryTokenResultBuilder.Values.ToList<QueryTokenResult>()
        };
      }
      if (this.parsedQueryTokens.Prefixes != null && this.parsedQueryTokens.Prefixes.Count > 0)
        this.ResolvePrefixes(requestContext, (IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, this.parsedQueryTokens.Prefixes);
      return (IOperationResponse) new SearchResponse()
      {
        Results = (IList<QueryTokenResult>) this.queryTokenResultBuilder.Values.ToList<QueryTokenResult>()
      };
    }

    private void SearchUniqueIdentifier(IVssRequestContext requestContext)
    {
      if (this.parsedQueryTokens.EmailAddresses != null && this.parsedQueryTokens.EmailAddresses.Count > 0)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.IdentityType = this.IdentityType;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.Emails = (IList<string>) this.parsedQueryTokens.EmailAddresses.ToList<string>();
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        foreach (KeyValuePair<string, QueryTokenResult> identity in (IEnumerable<KeyValuePair<string, QueryTokenResult>>) provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest))
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identity.Value);
      }
      if (this.parsedQueryTokens.DomainSamAccountNames != null && this.parsedQueryTokens.DomainSamAccountNames.Count > 0)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.IdentityType = this.IdentityType;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.DomainSamAccountNames = (IList<string>) this.parsedQueryTokens.DomainSamAccountNames.ToList<string>();
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        foreach (KeyValuePair<string, QueryTokenResult> identity in (IEnumerable<KeyValuePair<string, QueryTokenResult>>) provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest))
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identity.Value);
      }
      if (this.parsedQueryTokens.EntityIds != null && this.parsedQueryTokens.EntityIds.Count > 0)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.EntityIds = (IList<string>) this.parsedQueryTokens.EntityIds.ToList<string>();
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        IDictionary<string, QueryTokenResult> identities = provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest);
        foreach (string entityId in this.parsedQueryTokens.EntityIds)
        {
          if (identities.ContainsKey(entityId))
            IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identities[entityId]);
        }
      }
      if (this.parsedQueryTokens.SubjectDescriptors != null && this.parsedQueryTokens.SubjectDescriptors.Count > 0)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.SubjectDescriptors = (IList<SubjectDescriptor>) this.parsedQueryTokens.SubjectDescriptors.ToList<SubjectDescriptor>();
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        IDictionary<string, QueryTokenResult> identities = provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest);
        foreach (SubjectDescriptor subjectDescriptor in this.parsedQueryTokens.SubjectDescriptors)
        {
          if (identities.ContainsKey(subjectDescriptor.ToString()))
            IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identities[subjectDescriptor.ToString()]);
        }
      }
      if (this.parsedQueryTokens.DirectoryIds != null && this.parsedQueryTokens.DirectoryIds.Count > 0)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.DirectoryIds = (IList<Guid>) this.parsedQueryTokens.DirectoryIds.ToList<Guid>();
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        IDictionary<string, QueryTokenResult> identities = provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest);
        foreach (Guid directoryId in this.parsedQueryTokens.DirectoryIds)
        {
          if (identities.ContainsKey(directoryId.ToString()))
            IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identities[directoryId.ToString()]);
        }
      }
      if (this.parsedQueryTokens.AccountNames == null || this.parsedQueryTokens.AccountNames.Count <= 0 || requestContext.IsFeatureEnabled("VisualStudio.Services.IdentityPicker.DisableSearchByAccountNames"))
        return;
      IdentityProvider provider1 = SearchRequest.provider;
      IdentityProviderAdapterDdsGetRequest ipdGetRequest1 = new IdentityProviderAdapterDdsGetRequest();
      ipdGetRequest1.RequestContext = requestContext;
      ipdGetRequest1.OperationScope = this.OperationScope;
      ipdGetRequest1.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
      ipdGetRequest1.Options = SearchRequest.getOptions(this.searchOptions);
      ipdGetRequest1.AccountNames = (IList<string>) this.parsedQueryTokens.AccountNames.ToList<string>();
      ipdGetRequest1.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
      IDictionary<string, QueryTokenResult> identities1 = provider1.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest1);
      foreach (string accountName in this.parsedQueryTokens.AccountNames)
      {
        if (identities1.ContainsKey(accountName))
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, identities1[accountName]);
      }
    }

    private void SearchPrefix(IVssRequestContext requestContext)
    {
      if (this.parsedQueryTokens.Prefixes == null || this.parsedQueryTokens.Prefixes.Count <= 0)
        return;
      if (this.parsedQueryTokens.Count == 1)
      {
        this.ResolvePrefixes(requestContext, (IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, this.parsedQueryTokens.Prefixes);
      }
      else
      {
        foreach (string prefix in this.parsedQueryTokens.Prefixes)
        {
          string str = JsonConvert.SerializeObject((object) new SearchRequestPagingToken(this.IdentityType, this.OperationScope, this.Options, prefix, this.RequestProperties, this.FilterByAncestorEntityIds, this.FilterByEntityIds, SearchPagingTokenType.PrefixToken, string.Empty));
          IdentityProvider.AddOrMergeQueryTokenResult((IDictionary<string, QueryTokenResult>) this.queryTokenResultBuilder, new QueryTokenResult(prefix, (IList<Identity>) new List<Identity>(), (IDictionary<string, object>) new Dictionary<string, object>()
          {
            {
              QueryTokenResultProperties.PagingToken,
              (object) str
            }
          }), true);
        }
      }
    }

    private void ResolvePrefixes(
      IVssRequestContext requestContext,
      IDictionary<string, QueryTokenResult> qtrBuilder,
      HashSet<string> prefixes)
    {
      foreach (string prefix in prefixes)
      {
        IdentityProvider provider = SearchRequest.provider;
        IdentityProviderAdapterDdsGetRequest ipdGetRequest = new IdentityProviderAdapterDdsGetRequest();
        ipdGetRequest.RequestContext = requestContext;
        ipdGetRequest.OperationScope = this.OperationScope;
        ipdGetRequest.IdentityType = this.IdentityType;
        ipdGetRequest.FilterByAncestorEntityIds = (IEnumerable<string>) this.FilterByAncestorEntityIds;
        ipdGetRequest.FilterByEntityIds = (IEnumerable<string>) this.FilterByEntityIds;
        ipdGetRequest.RequestProperties = (IList<string>) this.RequestProperties.ToList<string>();
        ipdGetRequest.Options = SearchRequest.getOptions(this.searchOptions);
        ipdGetRequest.Prefix = prefix;
        ipdGetRequest.DdsPagingToken = this.requestPagingToken != null ? this.requestPagingToken.DdsPagingToken : string.Empty;
        IDictionary<string, QueryTokenResult> identities = provider.GetIdentities((IdentityProviderAdapterGetRequest) ipdGetRequest);
        if (identities.ContainsKey(prefix))
          IdentityProvider.AddOrMergeQueryTokenResult(qtrBuilder, identities[prefix]);
      }
    }

    private static void HandleUnresolvedQueryTokens(
      IdentityOperationHelper.ParsedTokens parsedQueryTokens,
      IDictionary<string, QueryTokenResult> queryTokenResultBuilder)
    {
      SearchRequest.RemoveResolvedTokens(parsedQueryTokens, queryTokenResultBuilder);
      HashSet<string> all = parsedQueryTokens.All;
      if (all.Count <= 0)
        return;
      foreach (string query in all)
        IdentityProvider.AddOrMergeQueryTokenResult(queryTokenResultBuilder, new QueryTokenResult(query, (IList<Identity>) new List<Identity>()));
    }

    private static void RemoveResolvedTokens(
      IdentityOperationHelper.ParsedTokens parsedQueryTokens,
      IDictionary<string, QueryTokenResult> queryTokenResultBuilder)
    {
      IList<HashSet<string>> stringSetList = (IList<HashSet<string>>) new List<HashSet<string>>()
      {
        parsedQueryTokens.Prefixes,
        parsedQueryTokens.EntityIds,
        parsedQueryTokens.EmailAddresses,
        parsedQueryTokens.DomainSamAccountNames
      };
      foreach (KeyValuePair<string, QueryTokenResult> keyValuePair in (IEnumerable<KeyValuePair<string, QueryTokenResult>>) queryTokenResultBuilder)
      {
        foreach (HashSet<string> source in (IEnumerable<HashSet<string>>) stringSetList)
        {
          if (source.Contains<string>(keyValuePair.Value.QueryToken, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            source.Remove(keyValuePair.Value.QueryToken);
        }
        Guid result;
        if (Guid.TryParse(keyValuePair.Value.QueryToken, out result) && parsedQueryTokens.DirectoryIds.Contains(result))
          parsedQueryTokens.DirectoryIds.Remove(result);
      }
    }

    private static Dictionary<string, object> getOptions(SearchOptions searchOptions) => new Dictionary<string, object>()
    {
      {
        "MaxResults",
        (object) searchOptions.maxResults
      },
      {
        "MinResults",
        (object) searchOptions.minResults
      },
      {
        "ScopeId",
        (object) searchOptions.scopeId
      }
    };
  }
}
