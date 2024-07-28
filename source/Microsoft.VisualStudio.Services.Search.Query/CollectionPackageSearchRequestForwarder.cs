// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CollectionPackageSearchRequestForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class CollectionPackageSearchRequestForwarder : PackageSearchRequestForwarder
  {
    public CollectionPackageSearchRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public CollectionPackageSearchRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override PackageSearchResponseContent ForwardSearchRequest(
      IVssRequestContext requestContext,
      PackageSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      bool includeSuggestions,
      out IEnumerable<string> suggestions)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080077, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      PackageSearchResponseContent searchResponse = (PackageSearchResponseContent) null;
      suggestions = (IEnumerable<string>) null;
      try
      {
        this.ValidateInputParameters(searchRequest, scopeFiltersExpression);
        if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          searchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.IndexingNotStarted, out suggestions);
        }
        else
        {
          PackageSearchQueryTagger tagger;
          PackageSearchPlatformRequest searchQueryRequest = this.PreProcessSearchRequestAndFormPlatformRequest(requestContext, searchRequest, indexInfo, scopeFiltersExpression, requestId, out IDictionary<string, IEnumerable<string>> _, out tagger);
          if (tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
            searchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.PrefixWildcardQueryNotSupported, out suggestions);
          else if (searchQueryRequest != null)
          {
            Stopwatch stopwatch = Stopwatch.StartNew();
            PackageSearchPlatformResponse platformSearchResponse = this.SearchPlatform.Search<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest, EntityPluginsFactory.GetEntityType(requestContext, "Package"), this.GetIndexProvider()) as PackageSearchPlatformResponse;
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EPlatformQueryTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
            PackageSearchRequest searchRequest1 = searchRequest;
            searchResponse = PackageSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse, searchRequest1);
            suggestions = (IEnumerable<string>) new List<string>();
            this.SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel((EntitySearchRequest) searchRequest, (EntitySearchResponse) searchResponse);
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080075, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return searchResponse;
    }

    protected override EntityIndexProvider<PackageVersionContract> GetIndexProvider() => (EntityIndexProvider<PackageVersionContract>) new CollectionPackageEntityIndexProvider<PackageVersionContract>();
  }
}
