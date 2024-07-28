// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WikiCountHandler
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class WikiCountHandler : AbstractSearchCountHandler
  {
    private ISearchSecurityScopeFilterExpressionBuilder m_searchSecurityScopeFilterExpressionBuilder;

    public WikiCountHandler(ICountRequestForwarder forwarder)
      : base(forwarder)
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WikiEntityType.GetInstance());
      this.EntityType = (IEntityType) WikiEntityType.GetInstance();
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new WikiSearchSecurityScopeFilterExpressionBuilder();
    }

    public WikiCountHandler()
    {
      this.IndexMapper = (IIndexMapper) new Microsoft.VisualStudio.Services.Search.Query.IndexMapper((IEntityType) WikiEntityType.GetInstance());
      this.EntityType = (IEntityType) WikiEntityType.GetInstance();
      this.m_searchSecurityScopeFilterExpressionBuilder = (ISearchSecurityScopeFilterExpressionBuilder) new WikiSearchSecurityScopeFilterExpressionBuilder();
    }

    protected override void InitializeForwarder(IVssRequestContext requestContext)
    {
      if (this.CountRequestForwarder == null)
        this.CountRequestForwarder = (ICountRequestForwarder) new WikiCountRequestForwarder(this.IndexMapper.GetESConnectionString(requestContext), requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings"), requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
      this.IsSecurityChecksEnabled = requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableSecurityChecksInQueryPipeline");
    }

    protected override IExpression CreateScopeFiltersExpression(
      IVssRequestContext requestContext,
      CountRequest query,
      out bool noResultAccessible,
      ProjectInfo projectInfo)
    {
      return this.m_searchSecurityScopeFilterExpressionBuilder.GetScopeFilterExpression(requestContext, this.IsSecurityChecksEnabled, out noResultAccessible, projectInfo);
    }

    protected override void ValidateUserPermission(IVssRequestContext requestContext) => requestContext.GetService<IWikiSecurityChecksService>().ValidateAndSetUserPermissionsForSearchService(requestContext);
  }
}
