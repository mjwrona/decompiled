// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeSearchPaginationController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "codeSearchPaginatedResults")]
  [SearchDemandExtension("ms", "vss-code-search")]
  public class CodeSearchPaginationController : SearchControllerBase
  {
    private string m_esConnectionString;
    internal int m_scrollSizeRequestCap;
    internal int m_scrollSizeRequestMin;
    internal IScrollSearchQueryForwarder<ScrollSearchRequest, ScrollSearchResponse> m_codeScrollSearchQueryForwarder;

    public CodeSearchPaginationController() => this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) CodeEntityType.GetInstance());

    internal CodeSearchPaginationController(
      IIndexMapper indexMapper,
      IScrollSearchQueryForwarder<ScrollSearchRequest, ScrollSearchResponse> codeSearchQueryForwarder)
    {
      this.IndexMapper = indexMapper;
      this.m_codeScrollSearchQueryForwarder = codeSearchQueryForwarder;
    }

    [HttpPost]
    [ClientLocationId("852DAC94-E8F7-45A2-9910-927AE35766A2")]
    [ClientTemporarySwaggerExclusion]
    public CodeSearchResponse FetchScrollCodeSearchResults(ScrollSearchRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080036, "REST-API", "REST-API", nameof (FetchScrollCodeSearchResults));
      try
      {
        return this.HandleCodeSearchResults(this.TfsRequestContext, request, this.ProjectInfo);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080037, "REST-API", "REST-API", nameof (FetchScrollCodeSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal CodeSearchResponse HandleCodeSearchResults(
      IVssRequestContext requestContext,
      ScrollSearchRequest request,
      ProjectInfo projectInfo = null)
    {
      try
      {
        if (request == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchWebApiResources.NullQueryMessage);
        if (!requestContext.IsFeatureEnabled("Search.Server.ScrollSearchQuery"))
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(Microsoft.VisualStudio.Services.Search.WebApi.SearchWebApiResources.SearchScrollNotSupportedMessage);
        request.ValidateQuery();
        CodeSearchResponse response = this.HandlePostCodeSearchRequest(this.TfsRequestContext, request, this.EnableSecurityChecksInQueryPipeline, projectInfo);
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) response);
        return response;
      }
      catch (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException ex)
      {
        throw ex.ConvertLegacyExceptionToCorrectException();
      }
    }

    internal CodeSearchResponse HandlePostCodeSearchRequest(
      IVssRequestContext requestContext,
      ScrollSearchRequest query,
      bool isSecurityChecksEnabled,
      ProjectInfo projectInfo = null)
    {
      Stopwatch e2eQueryTimer = Stopwatch.StartNew();
      CodeSearchResponse response = (CodeSearchResponse) null;
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfQueryRequests", "Query Pipeline", 1.0, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("EntityType", "Query Pipeline", (double) CodeEntityType.GetInstance().ID, true);
        if (isSecurityChecksEnabled && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
        if (query == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException("The query is null.");
        if (query.Filters == null)
          query.Filters = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
        if (string.IsNullOrEmpty(query.ScrollId))
          this.ScrollValidateQuery(query, projectInfo);
        this.PublishRequest(query);
        IEnumerable<string> accessibleGitRepos = (IEnumerable<string>) null;
        IEnumerable<string> accessibleProjects = (IEnumerable<string>) null;
        IDictionary<Tuple<string, string>, string> repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
        ICodeSecurityChecksService service = requestContext.GetService<ICodeSecurityChecksService>();
        service.ValidateAndSetUserPermissionsForSearchService(requestContext);
        Stopwatch gitSecurityChecksTimer = new Stopwatch();
        if (isSecurityChecksEnabled)
        {
          gitSecurityChecksTimer.Start();
          try
          {
            accessibleGitRepos = this.GetScopedAccessibleGitRepos(requestContext, service, query, out repoToIdMap);
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
        IExpression filtersExpression = this.GetScopeFiltersExpression(requestContext, service, query, isSecurityChecksEnabled, configValue, accessibleGitRepos, accessibleProjects, documentContractType, projectInfo);
        IEnumerable<IndexInfo> indexInfo = requestContext.IsFeatureEnabled("Search.Server.ScopedQuery") ? this.IndexMapper.GetIndexInfo(requestContext, (EntitySearchQuery) query.ToOldRequestContract()) : this.IndexMapper.GetIndexInfo(requestContext);
        response = (CodeSearchResponse) this.m_codeScrollSearchQueryForwarder.ForwardSearchRequest(requestContext, query, indexInfo, filtersExpression, requestContext.ActivityId.ToString(), documentContractType);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfCodeSearchResultsBeforeTrimming", "Query Pipeline", (double) response.Results.Count<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>());
        double noOfCodeSearchResultsBeforeTrimming = (double) response.Results.Count<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>();
        Stopwatch tfvcSecurityChecksTimer = new Stopwatch();
        Stopwatch tfvcFacetsSecurityChecksTimer = new Stopwatch();
        if (isSecurityChecksEnabled)
        {
          tfvcSecurityChecksTimer.Start();
          response.Results = this.GetUserAccessibleTfvcResults(requestContext, response.Results);
          tfvcSecurityChecksTimer.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("TfvcSecurityChecksTime", "Query Pipeline", (double) tfvcSecurityChecksTimer.ElapsedMilliseconds);
          tfvcFacetsSecurityChecksTimer.Start();
          response.Facets = this.GetUserAccessibleFacets(requestContext, service, response.Facets);
          tfvcFacetsSecurityChecksTimer.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("FacetsSecurityChecksTime", "Query Pipeline", (double) tfvcFacetsSecurityChecksTimer.ElapsedMilliseconds);
        }
        double noOfCodeSearchResultsAfterTrimming = (double) response.Results.Count<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfCodeSearchResultsAfterTrimming", "Query Pipeline", noOfCodeSearchResultsAfterTrimming);
        this.SetIndexingStatusInResponse((Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse) response, this.GetIndexingStatus(requestContext, (IEntityType) CodeEntityType.GetInstance()));
        if (repoToIdMap.Count > 0)
        {
          foreach (Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult result in response.Results)
          {
            if (result.Repository.Type == Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType.Git)
            {
              Tuple<string, string> key = Tuple.Create<string, string>(result.Project.Name.ToLowerInvariant(), result.Repository.Name.ToLowerInvariant());
              string str;
              repoToIdMap.TryGetValue(key, out str);
              result.Repository.Id = str;
            }
          }
        }
        this.UpdateResultWithBranchInfo(requestContext, response);
        this.RemoveLeadingSlashInCustomRepoResults(requestContext, documentContractType, response);
        if (noOfCodeSearchResultsAfterTrimming == 0.0)
          service.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        this.PopulateSearchSecuredObjectInResponse(requestContext, (SearchSecuredV2Object) response);
        this.PublishResponseInClientTrace((Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse) response);
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083058, "REST-API", "REST-API", ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishOnPremiseIndicator("TFS/Search/Query");
      }
      return response;
    }

    internal virtual void GetScrollSizeValidRequest(IVssRequestContext tfsRequestContext)
    {
      this.m_scrollSizeRequestCap = tfsRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/ScrollSize", 3000);
      this.m_scrollSizeRequestMin = tfsRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/CodeSearchTakeResultsLimit", 200);
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeQueryForwarder(this.TfsRequestContext);
      this.GetScrollSizeValidRequest(this.TfsRequestContext);
    }

    private IExpression GetScopeFiltersExpression(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      ScrollSearchRequest query,
      bool isSecurityChecksEnabled,
      bool isCustomProjectSecurityCheckEnabled,
      IEnumerable<string> accessibleGitRepos,
      IEnumerable<string> accessibleProjects,
      DocumentContractType contractType,
      ProjectInfo projectInfo)
    {
      List<IExpression> source = new List<IExpression>();
      IExpression expression1 = this.BuildTfsScopeExpression(requestContext, securityChecksService, isSecurityChecksEnabled, accessibleGitRepos);
      IExpression expression2 = this.BuildCustomProjectScopeExpression(requestContext, securityChecksService, isCustomProjectSecurityCheckEnabled, accessibleProjects);
      source.Add((IExpression) new OrExpression(new IExpression[2]
      {
        expression1,
        expression2
      }));
      if (CodeFileContract.MultiBranchSupportedContracts.Contains(contractType))
      {
        KeyValuePair<string, IEnumerable<string>> keyValuePair = query.Filters.FirstOrDefault<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (filter => filter.Key.Equals("Branch", StringComparison.OrdinalIgnoreCase)));
        IExpression expression3 = keyValuePair.Equals((object) new KeyValuePair<string, IEnumerable<string>>()) ? (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch.InlineFilterName(), Operator.Equals, "true") : (IExpression) new TermsExpression(CodeFileContract.CodeContractQueryableElement.BranchName.InlineFilterName(), Operator.In, keyValuePair.Value.Select<string, string>((Func<string, string>) (branch => branch.NormalizePath())));
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

    private void PublishRequest(ScrollSearchRequest request)
    {
      EntitySearchRequestBase searchRequestBase = (EntitySearchRequestBase) request;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "Query Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["CSSearchFilters"] = (object) string.Join<KeyValuePair<string, IEnumerable<string>>>(" | ", (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) searchRequestBase.Filters)
      });
    }

    private IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> GetUserAccessibleFacets(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> allFacets)
    {
      IEnumerable<FilterCategory> allFacets1 = allFacets.Select<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>, FilterCategory>) (f => new FilterCategory()
      {
        Name = CodeSearchRequestConvertor.FilterKeyMapping[f.Key],
        Filters = f.Value.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (c => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(c.Name, c.Id, c.ResultCount, true)))
      }));
      return CodeSearchResponseConvertor.UpdateFilterCategories(securityChecksService.GetUserAccessibleFacets(requestContext, allFacets1));
    }

    private IEnumerable<string> GetScopedAccessibleGitRepos(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      ScrollSearchRequest query,
      out IDictionary<Tuple<string, string>, string> repoToIdMap)
    {
      List<string> stringList = new List<string>();
      repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
      bool flag = false;
      KeyValuePair<string, IEnumerable<string>> keyValuePair1 = new KeyValuePair<string, IEnumerable<string>>();
      KeyValuePair<string, IEnumerable<string>> keyValuePair2 = new KeyValuePair<string, IEnumerable<string>>();
      ISet<string> stringSet1 = (ISet<string>) null;
      ISet<string> stringSet2 = (ISet<string>) null;
      if (query.Filters != null && query.Filters.Any<KeyValuePair<string, IEnumerable<string>>>())
      {
        keyValuePair1 = query.Filters.FirstOrDefault<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => "ProjectFilters".Equals(x.Key, StringComparison.OrdinalIgnoreCase)));
        if (keyValuePair1.Value != null && keyValuePair1.Value.Any<string>())
        {
          flag = true;
          if (keyValuePair1.Value.Count<string>() == 1)
          {
            keyValuePair2 = query.Filters.FirstOrDefault<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => "RepositoryFilters".Equals(x.Key, StringComparison.OrdinalIgnoreCase)));
            if (keyValuePair2.Value != null && keyValuePair2.Value.Any<string>())
              stringSet2 = (ISet<string>) new HashSet<string>(keyValuePair2.Value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            else
              stringSet1 = (ISet<string>) new HashSet<string>(keyValuePair1.Value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          }
          else
            stringSet1 = (ISet<string>) new HashSet<string>(keyValuePair1.Value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      bool allReposAreAccessible;
      foreach (GitRepositoryData gitRepositoryData in requestContext.IsFeatureEnabled("Search.Server.ScopedQuery") ? securityChecksService.GetUserAccessibleRepositoriesScopedToSearchQuery(requestContext, (EntitySearchQuery) query.ToOldRequestContract(), out allReposAreAccessible) : securityChecksService.GetUserAccessibleRepositories(requestContext, out allReposAreAccessible))
      {
        string str = gitRepositoryData.Id.ToString();
        Tuple<string, string> key = new Tuple<string, string>(gitRepositoryData.ProjectName.ToLowerInvariant(), gitRepositoryData.Name.ToLowerInvariant());
        if (!repoToIdMap.ContainsKey(key))
          repoToIdMap.Add(key, str);
        if (!allReposAreAccessible)
        {
          if (!flag)
            stringList.Add(str);
          else if (keyValuePair2.Equals((object) new KeyValuePair<string, IEnumerable<string>>()))
          {
            if (stringSet1 != null && stringSet1.Contains(gitRepositoryData.ProjectName))
              stringList.Add(str);
          }
          else if (string.Equals(gitRepositoryData.ProjectName, keyValuePair1.Value.First<string>(), StringComparison.OrdinalIgnoreCase) && stringSet2 != null && stringSet2.Contains(gitRepositoryData.Name))
            stringList.Add(str);
        }
      }
      return !allReposAreAccessible ? (IEnumerable<string>) stringList : (IEnumerable<string>) null;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object,System.Object)")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    private void ScrollValidateQuery(ScrollSearchRequest query, ProjectInfo projectInfo)
    {
      if (query.SearchText.Length > this.SearchTextLimitCap)
        throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchWebApiResources.MaxSearchTextLengthExceptionMessageFormat, (object) this.SearchTextLimitCap));
      if (query.ScrollSize > 0)
      {
        if (query.ScrollSize > this.m_scrollSizeRequestCap)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.WebApi.SearchWebApiResources.InvalidScrollMaxSizeMessage, (object) this.m_scrollSizeRequestCap, (object) query.ScrollSize));
        if (query.ScrollSize <= this.m_scrollSizeRequestMin)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.WebApi.SearchWebApiResources.InvalidScrollSizeMinSizeMessage, (object) this.m_scrollSizeRequestMin, (object) query.ScrollSize));
      }
      if (projectInfo != null && query.Filters != null)
      {
        IDictionary<string, IEnumerable<string>> dictionary = (IDictionary<string, IEnumerable<string>>) query.Filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (filter => CodeSearchRequestConvertor.FilterKeyMapping[filter.Key]), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (filter => filter.Value));
        if (dictionary != null)
        {
          List<SearchFilter> searchFilters = new List<SearchFilter>(dictionary.Count);
          foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) dictionary)
            searchFilters.Add(new SearchFilter()
            {
              Name = keyValuePair.Key,
              Values = keyValuePair.Value
            });
          this.ProjectFilterValidation(projectInfo, searchFilters);
        }
      }
      if (query.Filters == null)
        return;
      foreach (KeyValuePair<string, IEnumerable<string>> filter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) query.Filters)
      {
        if (filter.Key.Equals("CodeElementFilters", StringComparison.OrdinalIgnoreCase))
        {
          IEnumerable<string> strings = filter.Value.Except<string>((IEnumerable<string>) CodeSearchFilters.CEFilterIds, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          if (strings.Any<string>())
            throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchWebApiResources.UnsupportedCodeElementFilterMessageFormat, (object) string.Join(",", strings)));
        }
      }
    }

    private void UpdateResultWithBranchInfo(
      IVssRequestContext requestContext,
      CodeSearchResponse response)
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
      CodeSearchResponse response)
    {
      if (!requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/TrimLeadingSlashInCustomRepos", true, true) || !contractType.IsNoPayloadContract())
        return;
      foreach (Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult result in response.Results)
      {
        if (result.Repository.Type == Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType.Custom)
          result.Path = result.Path.TrimStart('/');
      }
    }

    private void TrimResultsWithBranchInfo(
      IVssRequestContext requestContext,
      CodeSearchResponse response)
    {
      if (!this.IndexMapper.GetDocumentContractType(requestContext).IsDedupeFileContract())
        return;
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> facets = response.Facets;
      Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> dictionary = facets != null ? facets.ToDictionary<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>, string>) (filter => CodeSearchRequestConvertor.FilterKeyMapping[filter.Key]), (Func<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (filter => filter.Value)) : (Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
      HashSet<string> requestedBranches = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (dictionary.Any<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>>())
        requestedBranches.AddRange<string, HashSet<string>>(dictionary.First<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>>().Value.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, string>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, string>) (filter => filter.Name)));
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
      Parallel.ForEach<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>(response.Results, (Action<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) (result =>
      {
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version> versionList = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>();
        IEnumerator<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version> enumerator = result.Versions.GetEnumerator();
        if (isDefaultBranchRequested)
        {
          enumerator.MoveNext();
          versionList.Add(enumerator.Current);
        }
        if (isNonDefaultBranchRequested)
        {
          while (enumerator.MoveNext())
          {
            Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version current = enumerator.Current;
            if (requestedBranches.Contains(current.BranchName))
              versionList.Add(current);
          }
        }
        if (versionList.Count <= 0)
          return;
        result.Versions = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Version>) versionList;
      }));
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult> GetUserAccessibleTfvcResults(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult> results)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult> accessibleTfvcResults = new List<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>();
      foreach (Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult result in results)
      {
        if (result.Repository.Type == Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType.Git || result.Repository.Type == Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType.Custom || result.Repository.Type == Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.VersionControlType.Tfvc)
          accessibleTfvcResults.Add(result);
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ReadAccessChecksTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      return (IEnumerable<Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeResult>) accessibleTfvcResults;
    }

    private void UpdateFiltersWithBranchInfo(
      IVssRequestContext requestContext,
      CodeSearchResponse response)
    {
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter> source1 = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>();
      foreach (KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>> facet in (IEnumerable<KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>>) response.Facets)
      {
        List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter> filterList = source1;
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter> source2 = facet.Value;
        Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter filter = source2 != null ? source2.Select<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (i => i)).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>() : (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter) null;
        filterList.Add(filter);
      }
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter[] array = source1.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, bool>) (filter => filter.Name.Equals("Repository", StringComparison.OrdinalIgnoreCase))).ToArray<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>();
      if (!((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) array).Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>())
        return;
      string name1 = ((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) array).First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>().Name;
      if (string.IsNullOrEmpty(name1))
        return;
      string name2 = source1.First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, bool>) (filter => filter.Name.Equals("Project", StringComparison.OrdinalIgnoreCase))).Name;
      IBranchService service = requestContext.GetService<IBranchService>();
      IList<string> branches = service.GetBranches(requestContext, name2, name1);
      string defaultBranch = service.GetDefaultBranch(requestContext, name2, name1);
      if (!branches.Any<string>())
        return;
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter> list = source1.Where<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter, bool>) (filter => filter.Name.Equals("Branch", StringComparison.OrdinalIgnoreCase))).ToList<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>();
      HashSet<string> source3 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (list.Any<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>())
      {
        source3.Add(list.First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>().Name);
        if (source3.Contains<string>("#Default#", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source3.Add(defaultBranch);
        if (source3.Contains<string>("#All#", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          source3.Add(list.First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>().Name);
      }
      else
        source3.Add(defaultBranch);
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter> filters = branches.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (branch => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(branch, branch, 0)));
      if (!response.Facets.ContainsKey("Branch"))
        return;
      response.Facets.Add("Branch", filters);
    }

    private IExpression BuildTfsScopeExpression(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      bool isSecurityChecksEnabled,
      IEnumerable<string> accessibleGitRepos)
    {
      string type = "repositoryId";
      List<string> terms1 = (List<string>) null;
      bool flag = securityChecksService.IsUserAnonymous(requestContext);
      if (accessibleGitRepos != null)
      {
        terms1 = new List<string>();
        terms1.AddRange(accessibleGitRepos.Select<string, string>((Func<string, string>) (x => x.ToLowerInvariant())));
      }
      if (isSecurityChecksEnabled)
      {
        IExpression expression1 = (IExpression) new AndExpression(new IExpression[2]
        {
          (IExpression) new TermsExpression(type, Operator.In, (IEnumerable<string>) terms1),
          (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
          {
            "git"
          })
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
      List<string> terms2 = new List<string>(2);
      if (!flag)
        terms2.Add("tfvc");
      terms2.Add("git");
      return (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) terms2);
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

    private void InitializeQueryForwarder(IVssRequestContext requestContext)
    {
      if (this.m_codeScrollSearchQueryForwarder != null)
        return;
      SearchOptions searchOptions = SearchOptions.None;
      this.m_esConnectionString = this.IndexMapper.GetESConnectionString(requestContext);
      this.m_codeScrollSearchQueryForwarder = (IScrollSearchQueryForwarder<ScrollSearchRequest, ScrollSearchResponse>) new ScrollCodeSearchQueryForwarder(this.m_esConnectionString, requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), searchOptions, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }
  }
}
