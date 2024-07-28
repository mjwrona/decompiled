// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.RestApiResourceProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public sealed class RestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("search", "EA48A0A1-269C-42D8-B8AD-DDC8FCDCF578");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomTenantLocationId, "search", "customTenant", "{area}/{resource}", VssRestApiVersion.v1_0, routeName: "tenant");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomTenantGetCollectionNamesLocationId, "search", "customTenant", "{area}/{resource}/{tenantName}", VssRestApiVersion.v1_0, routeName: "getTenantAccountNames");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomProjectLocationId, "search", "customProject", "{area}/{resource}", VssRestApiVersion.v1_0, routeName: "project");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomRepositoryRegisterRepositoryLocationId, "search", "customRepository", "{area}/{resource}", VssRestApiVersion.v1_0, routeName: "registerRepository");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomRepositoryGetRepositoriesLocationId, "search", "customRepository", "{area}/{resource}/{projectName}", VssRestApiVersion.v1_0, routeName: "getRepositories");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomRepositoryGetRepositoryLocationId, "search", "customRepository", "{area}/{resource}/{projectName}/{repositoryName}", VssRestApiVersion.v1_0, routeName: "getRepository");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomRepositoryGetRepositoryHealthLocationId, "search", "customRepository", "{area}/health/{resource}/{projectName}/{repositoryName}/{branchName}/{numberOfResults}/{continuationToken}", VssRestApiVersion.v1_0, routeName: "getRepositoryHealth");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomCodeBulkIndexLocationId, "search", "customCode", "{area}/{resource}", VssRestApiVersion.v1_0, routeName: "BulkCodeIndex");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomCodeGetFileContentLocationId, "search", "customCode", "{area}/{resource}/{projectName}/{repositoryName}/{branchName}/{filePath}", VssRestApiVersion.v1_0, routeName: "fileContent");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomCodeFilesMetadataLocationId, "search", "customCode", "{area}/{resource}/{projectName}/{repositoryName}", VssRestApiVersion.v1_0, routeName: "filesMetadata");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CustomCodeGetOperationStatusLocationId, "search", "customCode", "{area}/{resource}/{projectName}/{repositoryName}/{trackingId}", VssRestApiVersion.v1_0, routeName: "operationStatus");
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CodeQueryResultsLocationId, "search", "codeQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.CodeSearchResultsLocationId, "search", "codeSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.CodeSearchPaginatedResultsLocationId, "search", "codeSearchPaginatedResults", "{area}/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.CodeAdvancedQueryResultsLocationId, "search", "codeAdvancedQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.AdvancedCodeSearchResultsLocationId, "search", "advancedCodeSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.TenantCodeQueryResultsLocationId, "search", "tenantCodeQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.TenantWikiQueryResultsLocationId, "search", "tenantWikiQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.ProjectQueryResultsLocationId, "search", "projectQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.WorkItemQueryResultsLocationId, "search", "workItemQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.WorkItemSearchResultsLocationId, "search", "workItemSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.WorkItemInstantSearchResultsLocationId, "search", "workItemInstantSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.EntityCountLocationId, "search", "resultsCount", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.WorkItemFieldsLocationId, "search", "workItemFields", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.UserAccessibleRepositoriesLocationId, "search", "userAccessibleRepositories", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.ExtensionEventPreinstallLocationId, "search", "preinstall", "public/{area}/ExtensionEvents/{resource}", VssRestApiVersion.v2_0);
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.WikiQueryResultsLocationId, "search", "wikiQueryResults", "{area}/{resource}", VssRestApiVersion.v1_0);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.WikiSearchResultsLocationId, "search", "wikiSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.PackageSearchResultsLocationId, "search", "packageSearchResults", "{area}/{resource}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.PackageCountLocationId, "search", "packageResultsCount", "{area}/{resource}", VssRestApiVersion.v5_0);
      routes.MapResourceRoute(TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.BoardSearchResultsLocationId, "search", "boardSearchResults", "{area}/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.RepositoryStatusLocationId, "search", "repositories", "{area}/status/{resource}/{repository}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.CustomRepositoryStatusLocationId, "search", "customRepositories", "{area}/status/{resource}/{repository}", VssRestApiVersion.v5_0);
      routes.MapResourceRoute(TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.TfvcRepositoryStatusLocationId, "search", "tfvc", "{area}/status/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.SettingSearchResultsLocationId, "search", "settingSearchResults", "{area}/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Project, Microsoft.VisualStudio.Services.Search.WebApi.SearchConstants.CustomRepositoryBranchStatusLocationId, "search", "customRepositoryBranch", "{area}/{resource}/{repository}/{branch}", VssRestApiVersion.v5_0, routeName: "getCustomRepositoryBranchStatus");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.SearchConstants.GetLastIndexedChangeIdLocationId, "search", "customRepository", "{area}/{resource}/{projectName}/{repositoryName}/{branchName}/customindexingproperties", VssRestApiVersion.v1_0, routeName: "getLastIndexedChangeId");
    }
  }
}
