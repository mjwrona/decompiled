// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeCountHandler
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class CodeCountHandler : AbstractSearchCountHandler
  {
    private const string projectFilterid = "Projects";
    [StaticSafe]
    private static readonly Dictionary<string, string> s_searchfiltertofiltermap = new Dictionary<string, string>()
    {
      {
        "Projects",
        "ProjectFilters"
      }
    };

    public CodeCountHandler(ICountRequestForwarder forwarder)
      : base(forwarder)
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) CodeEntityType.GetInstance());
      this.EntityType = (IEntityType) CodeEntityType.GetInstance();
    }

    public CodeCountHandler()
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) CodeEntityType.GetInstance());
      this.EntityType = (IEntityType) CodeEntityType.GetInstance();
    }

    protected override void InitializeForwarder(IVssRequestContext requestContext)
    {
      if (this.CountRequestForwarder == null)
        this.CountRequestForwarder = (ICountRequestForwarder) new CodeCountRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.IsSecurityChecksEnabled = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableSecurityChecksInQueryPipeline");
    }

    protected override void ValidateExtensionActive(IVssRequestContext requestContext)
    {
      ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier("ms.vss-code-search");
      if (!requestContext.GetService<IInstalledExtensionManager>().IsExtensionActive(requestContext, extensionIdentifier.PublisherName, extensionIdentifier.ExtensionName))
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SearchSharedWebApiResources.ExtensionNotActiveMessage, (object) "Code"));
    }

    protected override void ValidateUserPermission(IVssRequestContext requestContext) => requestContext.GetService<ICodeSecurityChecksService>().ValidateAndSetUserPermissionsForSearchService(requestContext);

    protected override IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      CountRequest query,
      out bool noResultAccessible,
      ProjectInfo projectInfo)
    {
      List<IExpression> source = new List<IExpression>();
      IEnumerable<string> strings1 = (IEnumerable<string>) new List<string>();
      IEnumerable<string> strings2 = (IEnumerable<string>) new List<string>();
      noResultAccessible = false;
      bool isUserAnonymous = (bool) requestContext.Items["isUserAnonymousKey"];
      SearchQuery searchQuery = new SearchQuery();
      searchQuery.SearchText = query.SearchText;
      searchQuery.SearchFilters = query.SearchFilters;
      searchQuery.SkipResults = 0;
      searchQuery.TakeResults = 1000;
      searchQuery.Filters = (IEnumerable<SearchFilter>) new List<SearchFilter>();
      SearchQuery query1 = searchQuery;
      IDictionary<Tuple<string, string>, string> repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
      ICodeSecurityChecksService service = requestContext.GetService<ICodeSecurityChecksService>();
      Stopwatch stopwatch1 = new Stopwatch();
      if (this.IsSecurityChecksEnabled)
      {
        stopwatch1.Start();
        try
        {
          strings1 = this.GetScopedAccessibleGitRepos(requestContext, service, query1, out repoToIdMap);
        }
        finally
        {
          stopwatch1.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GitSecurityChecksTime", "Query Pipeline", (double) stopwatch1.ElapsedMilliseconds);
        }
      }
      bool configValue = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection);
      if (configValue)
      {
        Stopwatch stopwatch2 = new Stopwatch();
        stopwatch2.Start();
        try
        {
          strings2 = this.GetAccessibleProjects(requestContext, service);
        }
        finally
        {
          stopwatch2.Stop();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ProjectSecurityChecksTime", "Query Pipeline", (double) stopwatch2.ElapsedMilliseconds);
        }
      }
      if (strings1 != null && !strings1.Any<string>() && strings2 != null && !strings2.Any<string>())
        service.PopulateUserSecurityChecksDataInRequestContext(requestContext);
      IExpression expression1 = this.BuildTfsScopeExpression(this.IsSecurityChecksEnabled, strings1, isUserAnonymous);
      IExpression expression2 = this.BuildCustomProjectScopeExpression(configValue, strings2);
      source.Add((IExpression) new OrExpression(new IExpression[2]
      {
        expression1,
        expression2
      }));
      DocumentContractType documentContractType = this.IndexMapper.GetDocumentContractType(requestContext);
      if (CodeFileContract.MultiBranchSupportedContracts.Contains(documentContractType))
      {
        SearchFilter searchFilter1;
        if (query1 == null)
        {
          searchFilter1 = (SearchFilter) null;
        }
        else
        {
          IEnumerable<SearchFilter> filters = query1.Filters;
          searchFilter1 = filters != null ? filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (filter => filter.Name.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))) : (SearchFilter) null;
        }
        SearchFilter searchFilter2 = searchFilter1;
        IExpression expression3 = searchFilter2 == null ? (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch.InlineFilterName(), Operator.Equals, "true") : (IExpression) new TermsExpression(CodeFileContract.CodeContractQueryableElement.BranchName.InlineFilterName(), Operator.In, searchFilter2.Values.Select<string, string>((Func<string, string>) (branch => branch.NormalizePath())));
        source.Add(expression3);
      }
      Guid guid = requestContext.GetCollectionID();
      IExpression expression4 = (IExpression) new TermExpression("collectionId", Operator.Equals, guid.ToString().ToLowerInvariant());
      source.Add(expression4);
      if (projectInfo != null)
      {
        string[] terms = new string[1];
        guid = projectInfo.Id;
        terms[0] = guid.ToString().ToLowerInvariant();
        IExpression expression5 = (IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) terms);
        source.Add(expression5);
      }
      if (documentContractType.IsNoPayloadContract())
        source.Add((IExpression) new TermsExpression("isDocumentDeletedInReIndexing", Operator.In, (IEnumerable<string>) new string[1]
        {
          bool.FalseString.ToLowerInvariant()
        }));
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue<string>("/Service/SearchShared/Settings/SoftDeletedProjectIds");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
        source.Add((IExpression) new NotExpression((IExpression) new TermsExpression("projectId", Operator.In, (IEnumerable<string>) ((IEnumerable<string>) currentHostConfigValue.Split(',')).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrEmpty(i))).ToList<string>())));
      return source.Aggregate<IExpression>((System.Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      })));
    }

    private IExpression BuildTfsScopeExpression(
      bool isSecurityChecksEnabled,
      IEnumerable<string> accessibleGitRepos,
      bool isUserAnonymous)
    {
      string type = "repositoryId";
      IExpression expression1 = (IExpression) new EmptyExpression();
      List<string> terms1 = (List<string>) null;
      if (accessibleGitRepos != null)
      {
        terms1 = new List<string>();
        terms1.AddRange(accessibleGitRepos.Select<string, string>((Func<string, string>) (x => x.ToLowerInvariant())));
      }
      if (isSecurityChecksEnabled)
      {
        IExpression expression2 = (IExpression) new AndExpression(new IExpression[2]
        {
          (IExpression) new TermsExpression(type, Operator.In, (IEnumerable<string>) terms1),
          (IExpression) new OrExpression(new IExpression[2]
          {
            (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
            {
              "git"
            }),
            (IExpression) new MissingFieldExpression("vcType")
          })
        });
        if (isUserAnonymous)
          return expression2;
        IExpression expression3 = (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) new List<string>()
        {
          "tfvc"
        });
        return (IExpression) new OrExpression(new IExpression[2]
        {
          expression2,
          expression3
        });
      }
      IExpression expression4 = (IExpression) new MissingFieldExpression("vcType");
      List<string> terms2 = new List<string>(2);
      if (!isUserAnonymous)
        terms2.Add("tfvc");
      terms2.Add("git");
      IExpression expression5 = (IExpression) new TermsExpression("vcType", Operator.In, (IEnumerable<string>) terms2);
      return (IExpression) new OrExpression(new IExpression[2]
      {
        expression4,
        expression5
      });
    }

    private IEnumerable<string> GetAccessibleProjects(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService)
    {
      return securityChecksService.GetUserAccessibleProjects(requestContext);
    }

    internal IEnumerable<string> GetScopedAccessibleGitRepos(
      IVssRequestContext requestContext,
      ICodeSecurityChecksService securityChecksService,
      SearchQuery query,
      out IDictionary<Tuple<string, string>, string> repoToIdMap)
    {
      List<string> stringList = new List<string>();
      repoToIdMap = (IDictionary<Tuple<string, string>, string>) new Dictionary<Tuple<string, string>, string>();
      bool flag = false;
      SearchFilter searchFilter1 = (SearchFilter) null;
      SearchFilter searchFilter2 = (SearchFilter) null;
      ISet<string> stringSet1 = (ISet<string>) null;
      ISet<string> stringSet2 = (ISet<string>) null;
      if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/LimitScopeFilter") && query.Filters != null && query.Filters.Any<SearchFilter>())
      {
        searchFilter1 = query.Filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => "ProjectFilters".Equals(x.Name, StringComparison.OrdinalIgnoreCase)));
        if (searchFilter1?.Values != null && searchFilter1.Values.Any<string>())
        {
          flag = true;
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
      foreach (GitRepositoryData accessibleRepository in securityChecksService.GetUserAccessibleRepositories(requestContext, out allReposAreAccessible))
      {
        string str = accessibleRepository.Id.ToString();
        Tuple<string, string> key = new Tuple<string, string>(accessibleRepository.ProjectName.ToLowerInvariant(), accessibleRepository.Name.ToLowerInvariant());
        if (!repoToIdMap.ContainsKey(key))
          repoToIdMap.Add(key, str);
        if (!allReposAreAccessible)
        {
          if (!flag)
            stringList.Add(str);
          else if (searchFilter2 == null || !searchFilter2.Values.Any<string>())
          {
            if (query.SummarizedHitCountsNeeded || stringSet1.Contains(accessibleRepository.ProjectName))
              stringList.Add(str);
          }
          else if (string.Equals(accessibleRepository.ProjectName, searchFilter1.Values.First<string>(), StringComparison.OrdinalIgnoreCase) && (query.SummarizedHitCountsNeeded || stringSet2.Contains(accessibleRepository.Name)))
            stringList.Add(str);
        }
      }
      return !allReposAreAccessible ? (IEnumerable<string>) stringList : (IEnumerable<string>) null;
    }

    private IExpression BuildCustomProjectScopeExpression(
      bool isCustomProjectSecurityCheckEnabled,
      IEnumerable<string> accessibleProjects)
    {
      IExpression expression1 = (IExpression) new EmptyExpression();
      List<string> terms = (List<string>) null;
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

    private IEnumerable<SearchFilter> GenerateFiltersFromSearchFilters(
      IDictionary<string, IEnumerable<string>> SearchFilters)
    {
      List<SearchFilter> fromSearchFilters = new List<SearchFilter>();
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) SearchFilters)
      {
        string empty = string.Empty;
        bool flag = CodeCountHandler.s_searchfiltertofiltermap.TryGetValue(searchFilter.Key, out empty);
        fromSearchFilters.Add(new SearchFilter()
        {
          Name = flag ? empty : searchFilter.Key,
          Values = searchFilter.Value
        });
      }
      return (IEnumerable<SearchFilter>) fromSearchFilters;
    }

    public override IEnumerable<IndexInfo> ReturnIndexingInfo(
      IVssRequestContext requestContext,
      CountRequest request)
    {
      if (!requestContext.IsProjectScopedResultsCountFeatureEnabled())
        return base.ReturnIndexingInfo(requestContext, request);
      SearchQuery searchQuery1 = new SearchQuery();
      searchQuery1.SearchText = request.SearchText;
      searchQuery1.SearchFilters = request.SearchFilters;
      searchQuery1.SkipResults = 0;
      searchQuery1.TakeResults = 1000;
      searchQuery1.Filters = this.GenerateFiltersFromSearchFilters(request.SearchFilters);
      SearchQuery searchQuery2 = searchQuery1;
      return !requestContext.IsFeatureEnabled("Search.Server.ScopedQuery") ? this.IndexMapper.GetIndexInfo(requestContext) : this.IndexMapper.GetIndexInfo(requestContext, (EntitySearchQuery) searchQuery2);
    }
  }
}
