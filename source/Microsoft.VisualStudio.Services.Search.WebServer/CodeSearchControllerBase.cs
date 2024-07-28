// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeSearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class CodeSearchControllerBase : SearchControllerBase
  {
    private ISearchQueryForwarder<SearchQuery, CodeQueryResponse> m_codeSearchQueryForwarder;
    private string m_esConnectionString;
    private int m_takeResultsLimit;
    private ISearchQueryForwarder<SearchQuery, CodeQueryResponse> m_codeSearchQueryForwarderABTesting;
    private IExpression m_scopeFiltersExpressionForABTesting;
    private SearchQuery m_searchQueryForABTesting;
    private string m_requestIdForABTesting;
    [StaticSafe]
    private static readonly Random s_random = new Random();

    protected CodeSearchControllerBase() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) CodeEntityType.GetInstance());

    protected CodeSearchControllerBase(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<SearchQuery, CodeQueryResponse> codeSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_codeSearchQueryForwarder = codeSearchQueryForwarder;
    }

    internal CodeQueryResponse HandlePostCodeQueryRequest(
      IVssRequestContext requestContext,
      SearchQuery query,
      bool isSecurityChecksEnabled,
      ProjectInfo projectInfo = null)
    {
      Stopwatch e2eQueryTimer = Stopwatch.StartNew();
      CodeQueryResponse response = (CodeQueryResponse) null;
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) CodeEntityType.GetInstance().ID, true);
        if (isSecurityChecksEnabled && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (query == null)
          throw new InvalidQueryException("The query is null.");
        this.HandleNullProperties((EntitySearchQuery) query);
        this.ValidateQuery((EntitySearchQuery) query, requestContext, projectInfo);
        SearchQuery searchQuery = query?.Clone();
        this.LimitTakeResultsInSearchQuery((EntitySearchQuery) query, requestContext);
        this.PublishRequest((EntitySearchQuery) query);
        if (this.EnableSkipTakeOverride)
        {
          query.SkipResults = this.SkipResults;
          query.TakeResults = this.TakeResults;
        }
        IEnumerable<string> accessibleGitRepos = (IEnumerable<string>) null;
        IEnumerable<string> accessibleProjects = (IEnumerable<string>) null;
        IDictionary<Tuple<string, string>, string> repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
        IDictionary<Tuple<string, string>, string> repoNametoProjectIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
        ICodeSecurityChecksService service = requestContext.GetService<ICodeSecurityChecksService>();
        service.ValidateAndSetUserPermissionsForSearchService(requestContext);
        List<string> disabledrepos = new List<string>();
        Stopwatch gitSecurityChecksTimer = new Stopwatch();
        if (isSecurityChecksEnabled)
        {
          gitSecurityChecksTimer.Start();
          try
          {
            accessibleGitRepos = this.GetScopedAccessibleGitRepos(requestContext, service, query, out repoToIdMap, out repoNametoProjectIdMap, out disabledrepos);
          }
          finally
          {
            gitSecurityChecksTimer.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GitSecurityChecksTime", "Query Pipeline", (double) gitSecurityChecksTimer.ElapsedMilliseconds);
          }
        }
        Stopwatch customProjectSecurityChecksTimer = new Stopwatch();
        bool configValue = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection);
        if (configValue)
        {
          customProjectSecurityChecksTimer.Start();
          try
          {
            accessibleProjects = service.GetUserAccessibleCustomProjects(requestContext);
          }
          finally
          {
            customProjectSecurityChecksTimer.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ProjectSecurityChecksTime", "Query Pipeline", (double) customProjectSecurityChecksTimer.ElapsedMilliseconds);
          }
        }
        DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
        IExpression filtersExpression = this.GetScopeFiltersExpression(requestContext, service, query, isSecurityChecksEnabled, configValue, accessibleGitRepos, accessibleProjects, documentContractType, projectInfo, (IEnumerable<string>) disabledrepos);
        if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting"))
        {
          int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/PercentageQueriesForTestCluster", 100);
          if (CodeSearchControllerBase.s_random.Next(100) < configValueOrDefault)
          {
            this.m_scopeFiltersExpressionForABTesting = filtersExpression;
            this.m_searchQueryForABTesting = query?.Clone();
            this.m_requestIdForABTesting = requestContext.ActivityId.ToString();
            requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTaskCallback(this.ForwardQueryToNewClusterABTestingCallback));
          }
        }
        IEnumerable<IndexInfo> indexInfo = requestContext.IsFeatureEnabled("Search.Server.ScopedQuery") ? this.IndexMapper.GetIndexInfo(requestContext, (EntitySearchQuery) query) : this.IndexMapper.GetIndexInfo(requestContext);
        response = this.m_codeSearchQueryForwarder.ForwardSearchRequest(requestContext, query, indexInfo, filtersExpression, requestContext.ActivityId.ToString(), documentContractType);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfCodeSearchResultsBeforeTrimming", "Query Pipeline", (double) response.Results.Values.Count<CodeResult>());
        double noOfCodeSearchResultsBeforeTrimming = (double) response.Results.Values.Count<CodeResult>();
        Stopwatch tfvcSecurityChecksTimer = new Stopwatch();
        Stopwatch tfvcFacetsSecurityChecksTimer = new Stopwatch();
        if (isSecurityChecksEnabled)
        {
          tfvcSecurityChecksTimer.Start();
          response.Results.Values = this.GetUserAccessibleTfvcResults(requestContext, service, response.Results.Values);
          tfvcSecurityChecksTimer.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("TfvcSecurityChecksTime", "Query Pipeline", (double) tfvcSecurityChecksTimer.ElapsedMilliseconds);
          tfvcFacetsSecurityChecksTimer.Start();
          response.FilterCategories = this.GetUserAccessibleFacets(requestContext, service, response.FilterCategories);
          tfvcFacetsSecurityChecksTimer.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("FacetsSecurityChecksTime", "Query Pipeline", (double) tfvcFacetsSecurityChecksTimer.ElapsedMilliseconds);
        }
        double noOfCodeSearchResultsAfterTrimming = (double) response.Results.Values.Count<CodeResult>();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfCodeSearchResultsAfterTrimming", "Query Pipeline", noOfCodeSearchResultsAfterTrimming);
        CollectionIndexingStatus indexingStatus = this.GetIndexingStatus(requestContext, (IEntityType) CodeEntityType.GetInstance());
        if (this.IsBranchQuery(query.Filters))
        {
          if (this.IsSearchedBranchCurrentlyIndexing(requestContext, query.Filters))
            indexingStatus = CollectionIndexingStatus.BranchIndexing;
          else if (indexingStatus == CollectionIndexingStatus.BranchIndexing)
            indexingStatus = CollectionIndexingStatus.NotIndexing;
        }
        this.SetIndexingStatusInResponse((EntitySearchResponse) response, indexingStatus);
        this.SetOriginalSearchRequestAndErrorCodeInResponse(response, searchQuery);
        if (repoToIdMap.Count > 0)
        {
          foreach (CodeResult codeResult in response.Results.Values)
          {
            if (codeResult.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Git)
            {
              Tuple<string, string> key = Tuple.Create<string, string>(codeResult.Project.ToLowerInvariant(), codeResult.Repository.ToLowerInvariant());
              string str1;
              repoToIdMap.TryGetValue(key, out str1);
              codeResult.RepositoryID = str1;
              string str2;
              repoNametoProjectIdMap.TryGetValue(key, out str2);
              codeResult.ProjectId = str2;
            }
          }
        }
        this.UpdateResultWithBranchInfo(requestContext, response);
        this.UpdateResultsWithSnippetDetails(requestContext, response, documentContractType);
        this.RemoveLeadingSlashInCustomRepoResults(requestContext, documentContractType, response);
        if (noOfCodeSearchResultsAfterTrimming == 0.0)
          service.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        this.PopulateSearchSecuredObjectInResponse(requestContext, (EntitySearchResponse) response);
        this.PublishResponse((EntitySearchResponse) response);
        e2eQueryTimer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EQueryTime", "Query Pipeline", (double) e2eQueryTimer.ElapsedMilliseconds, true);
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("NoOfCodeSearchResultsBeforeTrimming", noOfCodeSearchResultsBeforeTrimming);
          ciData.Add("GitSecurityChecksTime", (double) gitSecurityChecksTimer.ElapsedMilliseconds);
          ciData.Add("ProjectSecurityChecksTime", (double) customProjectSecurityChecksTimer.ElapsedMilliseconds);
          ciData.Add("TfvcSecurityChecksTime", (double) tfvcSecurityChecksTimer.ElapsedMilliseconds);
          ciData.Add("FacetsSecurityChecksTime", (double) tfvcFacetsSecurityChecksTimer.ElapsedMilliseconds);
          ciData.Add("NoOfCodeSearchResultsAfterTrimming", noOfCodeSearchResultsAfterTrimming);
          ciData.Add("E2EQueryTime", (double) e2eQueryTimer.ElapsedMilliseconds);
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFailedQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080002, "REST-API", "REST-API", ex);
        SearchPlatformExceptionLogger.LogSearchPlatformException(ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return response;
    }

    internal string ConvertSearchTextToSubstringSearchText(string searchText)
    {
      foreach (string str1 in Regex.Split(searchText, "[^a-zA-Z0-9]"))
      {
        if (str1.Length >= 5)
        {
          string str2 = str1.Substring(1, str1.Length - 2);
          if (!CodeSearchFilters.SupportedFilterIds.Contains(str1) && !str1.Equals("AND") && !str1.Equals("OR") && !str1.Equals("NOT"))
            return "*" + str2 + "*";
        }
      }
      return string.Empty;
    }

    internal void PercentageBasedConversionToSubstringSearch(IVssRequestContext requestContext)
    {
      if (CodeSearchControllerBase.s_random.Next(100) >= requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/PercentageConversionToSubstringSearch", 50))
        return;
      string substringSearchText = this.ConvertSearchTextToSubstringSearchText(this.m_searchQueryForABTesting.SearchText);
      if (!string.IsNullOrEmpty(substringSearchText))
        this.m_searchQueryForABTesting.SearchText = substringSearchText;
      else
        this.m_searchQueryForABTesting.SearchText = "*amespac*";
    }

    private void ForwardQueryToNewClusterABTestingCallback(
      IVssRequestContext requestContext,
      object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Stopwatch stopwatch = Stopwatch.StartNew();
      requestContext.Items.Add("testQuery", (object) "true");
      try
      {
        IEnumerable<IndexInfo> infoForAbTesting = this.IndexMapper.GetIndexInfoForABTesting(requestContext);
        DocumentContractType documentContractType = requestContext.GetService<IDocumentContractTypeService>().GetSupportedIndexDocumentContractType(requestContext, (IEntityType) CodeEntityType.GetInstance());
        this.PercentageBasedConversionToSubstringSearch(requestContext);
        this.m_codeSearchQueryForwarderABTesting.ForwardSearchRequest(requestContext, this.m_searchQueryForABTesting, infoForAbTesting, this.m_scopeFiltersExpressionForABTesting, this.m_requestIdForABTesting, documentContractType);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080002, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("Failed to fire query to test cluster for AB Testing. Exception [{0}].", (object) ex)));
      }
      finally
      {
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080003, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("ForwardQueryToNewClusterABTestingCallback took time [{0}] milliseconds. ActivityId of original query is [{1}].", (object) stopwatch.ElapsedMilliseconds, (object) this.m_requestIdForABTesting)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal bool IsBranchQuery(IEnumerable<SearchFilter> filters)
    {
      List<string> stringList1 = (List<string>) null;
      List<string> stringList2 = (List<string>) null;
      foreach (SearchFilter filter in filters)
      {
        if (filter.Name == "ProjectFilters")
          stringList1 = filter.Values.ToList<string>();
        if (filter.Name == "RepositoryFilters")
          stringList2 = filter.Values.ToList<string>();
      }
      return stringList1 != null && stringList1.Count == 1 && stringList2 != null && stringList2.Count == 1;
    }

    internal bool IsSearchedBranchCurrentlyIndexing(
      IVssRequestContext requestContext,
      IEnumerable<SearchFilter> filters)
    {
      string projectName = (string) null;
      string repoName = (string) null;
      string branchName = (string) null;
      foreach (SearchFilter filter in filters)
      {
        if (filter.Name == "ProjectFilters")
          projectName = filter.Values.First<string>();
        if (filter.Name == "RepositoryFilters")
          repoName = filter.Values.First<string>();
        if (filter.Name == "BranchFilters")
          branchName = filter.Values.First<string>();
      }
      return string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(repoName) || this.GetIndexingStatusDetails(requestContext, (IEntityType) CodeEntityType.GetInstance(), projectName, repoName, branchName);
    }

    internal IExpression GetScopeFiltersExpression(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      SearchQuery query,
      bool isSecurityChecksEnabled,
      bool isCustomProjectSecurityCheckEnabled,
      IEnumerable<string> accessibleGitRepos,
      IEnumerable<string> accessibleProjects,
      DocumentContractType contractType,
      ProjectInfo projectInfo,
      IEnumerable<string> disabledrepos = null)
    {
      List<IExpression> source = new List<IExpression>();
      IExpression expression1 = this.BuildTfsScopeExpression(requestContext, securityChecksService, isSecurityChecksEnabled, accessibleGitRepos, disabledrepos);
      IExpression expression2 = this.BuildCustomProjectScopeExpression(requestContext, securityChecksService, isCustomProjectSecurityCheckEnabled, accessibleProjects);
      source.Add((IExpression) new OrExpression(new IExpression[2]
      {
        expression1,
        expression2
      }));
      if (CodeFileContract.MultiBranchSupportedContracts.Contains(contractType))
      {
        SearchFilter searchFilter1;
        if (query == null)
        {
          searchFilter1 = (SearchFilter) null;
        }
        else
        {
          IEnumerable<SearchFilter> filters = query.Filters;
          searchFilter1 = filters != null ? filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))) : (SearchFilter) null;
        }
        SearchFilter searchFilter2 = searchFilter1;
        IExpression expression3 = searchFilter2 == null ? (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch.InlineFilterName(), Operator.Equals, "true") : (IExpression) new TermsExpression(CodeFileContract.CodeContractQueryableElement.BranchName.InlineFilterName(), Operator.In, searchFilter2.Values.Select<string, string>((Func<string, string>) (branch => branch.NormalizePath())));
        source.Add(expression3);
      }
      if (contractType.IsNoPayloadContract())
        source.Add(this.GetFiltersToIgnoreDeletedDocumentsDuringReindexing());
      if (projectInfo != null)
      {
        IExpression expression4 = (IExpression) new TermExpression("projectId", Operator.Equals, projectInfo.Id.ToString().ToLowerInvariant());
        source.Add(expression4);
      }
      IExpression expression5 = (IExpression) new TermExpression("collectionId", Operator.Equals, requestContext.GetCollectionID().ToString().ToLowerInvariant());
      source.Add(expression5);
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        source.Add((IExpression) new NotExpression((IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>())));
      return source.Aggregate<IExpression>((System.Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      })));
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeQueryForwarder(this.TfsRequestContext);
    }

    internal void InitializeQueryForwarder(IVssRequestContext requestContext)
    {
      if (this.m_codeSearchQueryForwarder == null)
      {
        SearchOptions searchOptions = this.GetSearchOptions(requestContext);
        this.m_esConnectionString = this.IndexMapper.GetESConnectionString(requestContext);
        this.m_codeSearchQueryForwarder = (ISearchQueryForwarder<SearchQuery, CodeQueryResponse>) new CodeSearchQueryForwarder(this.m_esConnectionString, requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), searchOptions, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      }
      if (!requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting"))
        return;
      SearchOptions searchOptions1 = this.GetSearchOptions(requestContext);
      this.m_codeSearchQueryForwarderABTesting = (ISearchQueryForwarder<SearchQuery, CodeQueryResponse>) new CodeSearchQueryForwarder(requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/ConnectionStringForABTestCluster"), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), searchOptions1, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    internal override void GetSkipResultAndTakeResultValue(IVssRequestContext requestContext)
    {
      this.m_takeResultsLimit = requestContext.GetCurrentHostConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/CodeSearchTakeResultsLimit", 200);
      this.SkipResultsCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/CodeSearchMaxSkipResultsCount", 1000);
      this.TakeResultsCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/CodeSearchMaxTakeResultsCount", 1000);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    protected override void ValidateQuery(
      EntitySearchQuery query,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      base.ValidateQuery(query, requestContext, projectInfo);
      if (query.Filters == null)
        return;
      foreach (SearchFilter filter in query.Filters)
      {
        if (filter.Name.Equals("CodeElementFilters", StringComparison.OrdinalIgnoreCase))
        {
          IEnumerable<string> strings = filter.Values.Except<string>((IEnumerable<string>) CodeSearchFilters.CEFilterIds, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (strings.Any<string>())
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.UnsupportedCodeElementFilterMessageFormat, (object) string.Join(",", strings)));
        }
      }
    }

    private void LimitTakeResultsInSearchQuery(
      EntitySearchQuery query,
      IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("Search.Server.LimitSearchResults"))
        return;
      query.TakeResults = Math.Min(query.TakeResults, this.m_takeResultsLimit);
    }

    private void SetOriginalSearchRequestAndErrorCodeInResponse(
      CodeQueryResponse response,
      SearchQuery searchQuery)
    {
      if (response.Query.TakeResults < searchQuery.TakeResults)
        response.AddError(new ErrorData()
        {
          ErrorCode = "TakeResultValueTrimmedToMaxResultAllowed",
          ErrorType = ErrorType.Warning,
          ErrorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You asked for {0} results in take value but it was trimmed to {1} values as per limit set by the search service", (object) searchQuery.TakeResults, (object) response.Query.TakeResults)
        });
      response.Query = searchQuery;
    }

    internal override void PublishRequest(EntitySearchQuery request)
    {
      SearchQuery searchQuery = request as SearchQuery;
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
      {
        ["CSSkip"] = (object) searchQuery.SkipResults,
        ["CSTake"] = (object) searchQuery.TakeResults
      };
      if (searchQuery.Filters != null)
      {
        properties["CSSearchFilters"] = (object) string.Join<SearchFilter>(" | ", searchQuery.Filters);
        foreach (SearchFilter filter in searchQuery.Filters)
          properties[filter.Name] = (object) string.Join(",", filter.Values);
      }
      properties["CSSearchText"] = searchQuery.SearchText != null ? (object) searchQuery.SearchText : (object) string.Empty;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) properties);
    }

    protected override void PublishResponse(EntitySearchResponse response)
    {
      base.PublishResponse(response);
      CodeQueryResponse codeQueryResponse = response as CodeQueryResponse;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["CSTotalMatches"] = (object) codeQueryResponse.Results.Count,
        ["CSFacets"] = (object) string.Join<FilterCategory>(" | ", codeQueryResponse.FilterCategories)
      });
    }

    internal IEnumerable<string> GetScopedAccessibleGitRepos(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      SearchQuery query,
      out IDictionary<Tuple<string, string>, string> repoToIdMap,
      out IDictionary<Tuple<string, string>, string> repoNametoProjectIdMap,
      out List<string> disabledrepos)
    {
      List<string> stringList = new List<string>();
      repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
      repoNametoProjectIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
      bool flag1 = false;
      SearchFilter searchFilter1 = (SearchFilter) null;
      SearchFilter searchFilter2 = (SearchFilter) null;
      ISet<string> stringSet1 = (ISet<string>) null;
      ISet<string> stringSet2 = (ISet<string>) null;
      disabledrepos = new List<string>();
      if (query.Filters != null && query.Filters.Any<SearchFilter>())
      {
        searchFilter1 = query.Filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => "ProjectFilters".Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (searchFilter1?.Values != null && searchFilter1.Values.Any<string>())
        {
          flag1 = true;
          if (searchFilter1.Values.Count<string>() == 1)
          {
            searchFilter2 = query.Filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => "RepositoryFilters".Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
            if (searchFilter2?.Values != null && searchFilter2.Values.Any<string>())
              stringSet2 = (ISet<string>) new HashSet<string>(searchFilter2.Values, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            else
              stringSet1 = (ISet<string>) new HashSet<string>(searchFilter1.Values, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          }
          else
            stringSet1 = (ISet<string>) new HashSet<string>(searchFilter1.Values, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      bool allReposAreAccessible;
      foreach (GitRepositoryData gitRepositoryData in requestContext.IsFeatureEnabled("Search.Server.ScopedQuery") ? securityChecksService.GetUserAccessibleRepositoriesScopedToSearchQuery(requestContext, (EntitySearchQuery) query, out allReposAreAccessible) : securityChecksService.GetUserAccessibleRepositories(requestContext, out allReposAreAccessible))
      {
        string str = gitRepositoryData.Id.ToString();
        Tuple<string, string> key = new Tuple<string, string>(gitRepositoryData.ProjectName.ToLowerInvariant(), gitRepositoryData.Name.ToLowerInvariant());
        if (!repoToIdMap.ContainsKey(key))
          repoToIdMap.Add(key, str);
        if (!repoNametoProjectIdMap.ContainsKey(key))
          repoNametoProjectIdMap.Add(key, gitRepositoryData.ProjectId.ToString());
        if (!allReposAreAccessible)
        {
          if (!flag1)
          {
            stringList.Add(str);
          }
          else
          {
            int num;
            if (searchFilter2 == null)
            {
              num = 1;
            }
            else
            {
              IEnumerable<string> values = searchFilter2.Values;
              bool? nullable = values != null ? new bool?(values.Any<string>()) : new bool?();
              bool flag2 = true;
              num = !(nullable.GetValueOrDefault() == flag2 & nullable.HasValue) ? 1 : 0;
            }
            if (num != 0)
            {
              if (query.SummarizedHitCountsNeeded || stringSet1 != null && stringSet1.Contains(gitRepositoryData.ProjectName))
                stringList.Add(str);
            }
            else if (string.Equals(gitRepositoryData.ProjectName, searchFilter1.Values.First<string>(), StringComparison.OrdinalIgnoreCase) && (query.SummarizedHitCountsNeeded || stringSet2 != null && stringSet2.Contains(gitRepositoryData.Name)))
              stringList.Add(str);
          }
        }
        if (gitRepositoryData.isdisabled)
          disabledrepos.Add(str);
      }
      return !allReposAreAccessible ? (IEnumerable<string>) stringList : (IEnumerable<string>) null;
    }

    private IExpression BuildCustomProjectScopeExpression(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      bool isCustomProjectSecurityCheckEnabled,
      IEnumerable<string> accessibleProjects)
    {
      IExpression expression1 = (IExpression) new EmptyExpression();
      List<string> terms = (List<string>) null;
      if (securityChecksService.IsUserAnonymous(requestContext))
        return expression1;
      if (accessibleProjects != null)
      {
        terms = new List<string>();
        terms.AddRange(accessibleProjects.Select<string, string>((Func<string, string>) (x => x.ToLowerInvariant())));
      }
      IExpression expression2;
      if (accessibleProjects != null & isCustomProjectSecurityCheckEnabled)
        expression2 = (IExpression) new AndExpression(new IExpression[2]
        {
          (IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) terms),
          (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
          {
            "custom"
          })
        });
      else
        expression2 = (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
        {
          "custom"
        });
      return expression2;
    }

    private IExpression BuildTfsScopeExpression(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      bool isSecurityChecksEnabled,
      IEnumerable<string> accessibleGitRepos,
      IEnumerable<string> disabledrepos)
    {
      string type = "repositoryId";
      List<string> terms1 = (List<string>) null;
      List<string> terms2 = (List<string>) null;
      bool flag = securityChecksService.IsUserAnonymous(requestContext);
      if (accessibleGitRepos != null)
      {
        terms1 = new List<string>();
        terms1.AddRange(accessibleGitRepos.Select<string, string>((Func<string, string>) (x => x.ToLowerInvariant())));
      }
      if (disabledrepos != null)
      {
        terms2 = new List<string>();
        terms2.AddRange(disabledrepos.Select<string, string>((Func<string, string>) (x => x.ToLowerInvariant())));
      }
      if (isSecurityChecksEnabled)
      {
        IExpression expression1 = (IExpression) new AndExpression(new IExpression[3]
        {
          (IExpression) new TermsExpression(type, Operator.In, (IEnumerable<string>) terms1),
          (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
          {
            "git"
          }),
          (IExpression) new NotExpression((IExpression) new TermsExpression(type, Operator.In, (IEnumerable<string>) terms2))
        });
        if (flag)
          return expression1;
        IExpression expression2 = (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
        {
          "tfvc"
        });
        return (IExpression) new OrExpression(new IExpression[2]
        {
          expression1,
          expression2
        });
      }
      List<string> terms3 = new List<string>(2);
      if (!flag)
        terms3.Add("tfvc");
      terms3.Add("git");
      return (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) terms3);
    }

    private IEnumerable<CodeResult> GetUserAccessibleTfvcResults(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      IEnumerable<CodeResult> allResults)
    {
      return securityChecksService.GetUserAccessibleTfvcFiles(requestContext, allResults);
    }

    private IEnumerable<FilterCategory> GetUserAccessibleFacets(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      IEnumerable<FilterCategory> allFacets)
    {
      return securityChecksService.GetUserAccessibleFacets(requestContext, allFacets);
    }

    private void UpdateResultWithBranchInfo(
      IVssRequestContext requestContext,
      CodeQueryResponse response)
    {
      try
      {
        this.UpdateFiltersWithBranchInfo(requestContext, response);
        this.TrimResultsWithBranchInfo(requestContext, response);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080002, "REST-API", "REST-API", ex);
      }
    }

    protected internal void RemoveLeadingSlashInCustomRepoResults(
      IVssRequestContext requestContext,
      DocumentContractType contractType,
      CodeQueryResponse response)
    {
      if (!requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/TrimLeadingSlashInCustomRepos", true, true) || !contractType.IsNoPayloadContract())
        return;
      foreach (CodeResult codeResult in response.Results.Values)
      {
        if (codeResult.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Custom)
          codeResult.Path = codeResult.Path.TrimStart('/');
      }
    }

    private void TrimResultsWithBranchInfo(
      IVssRequestContext requestContext,
      CodeQueryResponse response)
    {
      if (!this.IndexMapper.GetDocumentContractType(requestContext).IsDedupeFileContract())
        return;
      FilterCategory[] array = response.FilterCategories.Where<FilterCategory>((Func<FilterCategory, bool>) (filterCategory => filterCategory.Name.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))).ToArray<FilterCategory>();
      HashSet<string> requestedBranches = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (((IEnumerable<FilterCategory>) array).Any<FilterCategory>())
        requestedBranches.AddRange<string, HashSet<string>>(((IEnumerable<FilterCategory>) array).First<FilterCategory>().Filters.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, bool>) (filter => filter.Selected)).Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, string>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, string>) (filter => filter.Name)));
      else
        requestedBranches.Add("#Default#");
      bool isDefaultBranchRequested = false;
      bool isNonDefaultBranchRequested = false;
      if (requestedBranches.Contains("#Default#"))
      {
        isDefaultBranchRequested = true;
        requestedBranches.Remove("#Default#");
      }
      if (requestedBranches.Any<string>())
        isNonDefaultBranchRequested = true;
      Parallel.ForEach<CodeResult>(response.Results.Values, (Action<CodeResult>) (result =>
      {
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> versionList = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>();
        IEnumerator<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version> enumerator = result.Versions.GetEnumerator();
        if (isDefaultBranchRequested)
        {
          enumerator.MoveNext();
          versionList.Add(enumerator.Current);
        }
        if (isNonDefaultBranchRequested)
        {
          while (enumerator.MoveNext())
          {
            Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version current = enumerator.Current;
            if (requestedBranches.Contains(current.BranchName))
              versionList.Add(current);
          }
        }
        if (versionList.Count <= 0)
          return;
        result.Versions = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Version>) versionList;
        result.Branch = versionList[0].BranchName;
        result.ChangeId = versionList[0].ChangeId;
      }));
    }

    private void UpdateFiltersWithBranchInfo(
      IVssRequestContext requestContext,
      CodeQueryResponse response)
    {
      IList<SearchFilter> source1 = (IList<SearchFilter>) ((object) (response.Query.Filters as IList<SearchFilter>) ?? (object) response.Query.Filters.ToArray<SearchFilter>());
      SearchFilter[] array1 = source1.Where<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("RepositoryFilters", StringComparison.OrdinalIgnoreCase))).ToArray<SearchFilter>();
      if (!((IEnumerable<SearchFilter>) array1).Any<SearchFilter>())
        return;
      SearchFilter searchFilter = ((IEnumerable<SearchFilter>) array1).First<SearchFilter>();
      IList<string> source2 = (IList<string>) ((object) (searchFilter.Values as IList<string>) ?? (object) searchFilter.Values.ToArray<string>());
      if (source2.Count != 1)
        return;
      string projectName = source1.First<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("ProjectFilters", StringComparison.OrdinalIgnoreCase))).Values.First<string>();
      IBranchService service = requestContext.GetService<IBranchService>();
      IList<string> branches = service.GetBranches(requestContext, projectName, source2.First<string>());
      string defaultBranch = service.GetDefaultBranch(requestContext, projectName, source2.First<string>());
      if (!branches.Any<string>())
        return;
      SearchFilter[] array2 = source1.Where<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))).ToArray<SearchFilter>();
      HashSet<string> requestedBranches = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (((IEnumerable<SearchFilter>) array2).Any<SearchFilter>())
      {
        requestedBranches.AddRange<string, HashSet<string>>(((IEnumerable<SearchFilter>) array2).First<SearchFilter>().Values);
        if (requestedBranches.Contains<string>("#Default#", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          requestedBranches.Add(defaultBranch);
        if (requestedBranches.Contains<string>("#All#", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          requestedBranches.AddRange<string, HashSet<string>>((IEnumerable<string>) branches);
      }
      else
        requestedBranches.Add(defaultBranch);
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filters = branches.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (branch => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(branch, branch, 0, requestedBranches.Contains<string>(branch, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))));
      List<FilterCategory> list = response.FilterCategories.ToList<FilterCategory>();
      list.Add(new FilterCategory()
      {
        Name = "BranchFilters",
        Filters = filters
      });
      response.FilterCategories = (IEnumerable<FilterCategory>) list;
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Calling Dispose multiple times for Stream object does not break anything")]
    private void UpdateResultsWithSnippetDetails(
      IVssRequestContext requestContext,
      CodeQueryResponse response,
      DocumentContractType contractType)
    {
      if (!requestContext.IsFeatureEnabled("Search.Server.Code.IncludeSnippetInHits") || contractType.Equals((object) DocumentContractType.DedupeFileContractV5) || contractType.Equals((object) DocumentContractType.SourceNoDedupeFileContractV5) || !requestContext.Items.ContainsKey("includeSnippetInCodeSearchKey"))
        return;
      GitHttpClient redirectedClientIfNeeded = requestContext.GetRedirectedClientIfNeeded<GitHttpClient>();
      IDictionary<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple, List<string>> dictionary = (IDictionary<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple, List<string>>) new Dictionary<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple, List<string>>((IEqualityComparer<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple>) new CodeSearchControllerBase.RepoEqualityComparer());
      foreach (CodeResult codeResult in response.Results.Values)
      {
        if (codeResult.VcType == Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.Code.VersionControlType.Git && codeResult.RepositoryID != null)
        {
          CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple key = new CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple(codeResult.Project, codeResult.RepositoryID);
          if (!dictionary.ContainsKey(key))
            dictionary[key] = new List<string>();
          dictionary[key].Add(codeResult.ContentId);
        }
      }
      IVssRequestContext userState = requestContext.Elevate();
      foreach (CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple key1 in (IEnumerable<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple>) dictionary.Keys)
      {
        CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple key = key1;
        try
        {
          Stream result = redirectedClientIfNeeded.GetBlobsZipAsync((IEnumerable<string>) dictionary[key], key.projectId, key.repositoryId, userState: (object) userState).Result;
          if (result != null)
          {
            using (ZipArchive zipArchive = new ZipArchive(result, ZipArchiveMode.Read))
            {
              foreach (ZipArchiveEntry entry1 in zipArchive.Entries)
              {
                ZipArchiveEntry entry = entry1;
                using (Stream stream = entry.Open())
                {
                  using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                  {
                    string end = streamReader.ReadToEnd();
                    CodeResult codeResult = response.Results.Values.Where<CodeResult>((Func<CodeResult, bool>) (r => r.ContentId == entry.Name && r.Project == key.projectId && r.RepositoryID == key.repositoryId)).FirstOrDefault<CodeResult>();
                    if (codeResult != null)
                      this.UpdateSnippetDetailFromContent(end, codeResult.Hits);
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1083149, "Query Pipeline", "Query", ex.ToString());
        }
      }
    }

    private void UpdateSnippetDetailFromContent(string docContent, IEnumerable<Hit> hits)
    {
      if (string.IsNullOrWhiteSpace(docContent) || hits.Count<Hit>() <= 0)
        return;
      int num1 = 1;
      char ch1 = '\n';
      int num2 = 0;
      int num3 = 0;
      int startIndex = 0;
      char[] charArray = docContent.ToCharArray();
      for (int index1 = 0; index1 < hits.Count<Hit>(); ++index1)
      {
        Hit hit = hits.ElementAt<Hit>(index1);
        if (docContent.Length >= hit.CharOffset + hit.Length && hit.CharOffset >= 0 && hit.Length > 0 && hit.CharOffset >= num2)
        {
          for (int index2 = num2; index2 < docContent.Length; ++index2)
          {
            char ch2 = charArray[index2];
            if (ch1 == '\n')
              num3 = index2;
            if (index2 == hit.CharOffset)
            {
              hit.Line = num1;
              startIndex = num3;
              hit.Column = index2 - startIndex + 1;
            }
            if (charArray[index2] == '\n' || index2 == docContent.Length - 1)
            {
              if (index2 >= hit.CharOffset + hit.Length)
              {
                int num4 = index2 == docContent.Length - 1 ? index2 + 1 : index2;
                if (ch1 == '\r')
                  --num4;
                hit.CodeSnippet = docContent.Substring(startIndex, num4 - startIndex);
                num2 = hit.CharOffset + hit.Length;
                break;
              }
              if (charArray[index2] == '\n')
                ++num1;
            }
            ch1 = ch2;
          }
        }
        else if (hit.CharOffset < num2)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080002, "REST-API", "REST-API", "Unsorted or overlapping offsets: Start offset >= end offset of last match");
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080002, "REST-API", "REST-API", "Invalid offsets or document content length");
      }
    }

    private class ProjectIdAndRepositoryIdTuple
    {
      internal string projectId;
      internal string repositoryId;

      internal ProjectIdAndRepositoryIdTuple(string projectId, string repositoryId)
      {
        this.projectId = projectId;
        this.repositoryId = repositoryId;
      }
    }

    private class RepoEqualityComparer : 
      IEqualityComparer<CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple>
    {
      public bool Equals(
        CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple x,
        CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple y)
      {
        if (x == null && y == null)
          return true;
        return x != null && y != null && x.projectId == y.projectId && x.repositoryId == y.repositoryId;
      }

      public int GetHashCode(
        CodeSearchControllerBase.ProjectIdAndRepositoryIdTuple obj)
      {
        return obj.projectId.GetHashCode() ^ obj.repositoryId.GetHashCode();
      }
    }
  }
}
