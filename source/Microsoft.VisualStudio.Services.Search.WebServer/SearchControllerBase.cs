// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SearchControllerBase
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class SearchControllerBase : SearchApiController
  {
    protected IIndexMapper IndexMapper { get; set; }

    protected bool EnableSkipTakeOverride { get; set; }

    protected int SkipResults { get; set; }

    protected int TakeResults { get; set; }

    protected bool EnableSecurityChecksInQueryPipeline { get; set; }

    protected int SkipResultsCap { get; set; }

    protected int TakeResultsCap { get; set; }

    protected int SearchTextLimitCap { get; set; }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.InitializeSettings(this.TfsRequestContext);
    }

    internal virtual void GetSkipResultAndTakeResultValue(IVssRequestContext tfsRequestContext)
    {
      this.SkipResultsCap = tfsRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/MaxSkipResultsCount", 1000);
      this.TakeResultsCap = tfsRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/MaxTakeResultsCount", 1000);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object[])", Justification = "CurrentUICulture is to be used by design.")]
    internal void ProjectFilterValidation(ProjectInfo projectInfo, List<SearchFilter> searchFilters)
    {
      if (projectInfo == null || searchFilters == null)
        return;
      foreach (SearchFilter searchFilter in searchFilters)
      {
        if (string.Equals(searchFilter.Name, "ProjectFilters", StringComparison.OrdinalIgnoreCase) || string.Equals(searchFilter.Name, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Project, StringComparison.OrdinalIgnoreCase))
        {
          int num = searchFilter.Values.Count<string>();
          if (num > 1)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidProjectFiltersCountMessage));
          if (num == 1 && !string.Equals(searchFilter.Values.ElementAt<string>(0), projectInfo.Name, StringComparison.OrdinalIgnoreCase))
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidProjectFilterValueMessage));
        }
      }
    }

    protected virtual SearchOptions GetSearchOptions(IVssRequestContext requestContext)
    {
      SearchOptions searchOptions = SearchOptions.None;
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableFaceting"))
        searchOptions |= SearchOptions.Faceting;
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableHighlighting"))
        searchOptions |= SearchOptions.Highlighting;
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableRanking"))
        searchOptions |= SearchOptions.Ranking;
      return searchOptions;
    }

    protected void InitializeSettings(IVssRequestContext tfsRequestContext)
    {
      this.EnableSkipTakeOverride = tfsRequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/OverrideSkipTake");
      this.SkipResults = tfsRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SkipResults");
      this.TakeResults = tfsRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/TakeResults");
      this.InitializeSearchTextLimitCap(tfsRequestContext);
      this.GetSkipResultAndTakeResultValue(tfsRequestContext);
      this.EnableSecurityChecksInQueryPipeline = tfsRequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableSecurityChecksInQueryPipeline");
    }

    internal virtual void InitializeSearchTextLimitCap(IVssRequestContext requestContext) => this.SearchTextLimitCap = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/MaxSearchTextLength", 1024);

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object)")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.IFormatProvider,System.String,System.Object,System.Object)", Justification = "CurrentUICulture is to be used by design.")]
    protected virtual void ValidateQuery(
      EntitySearchQuery query,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo = null)
    {
      query.ValidateQuery();
      if (query.SkipResults < 0 || query.SkipResults > this.SkipResultsCap)
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.InvalidSkipResultsMessage, (object) this.SkipResultsCap, (object) query.SkipResults));
      if (query.TakeResults < 0 || query.TakeResults > this.TakeResultsCap)
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Microsoft.VisualStudio.Services.Search.Shared.WebApi.SearchSharedWebApiResources.InvalidTakeResultsMessage, (object) this.TakeResultsCap, (object) query.TakeResults));
      if (query.SearchText.Length > this.SearchTextLimitCap)
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.MaxSearchTextLengthExceptionMessageFormat, (object) this.SearchTextLimitCap));
      this.ProjectFilterValidation(projectInfo, query.Filters.ToList<SearchFilter>());
    }

    protected void HandleNullProperties(EntitySearchQuery query)
    {
      if (query.Filters != null)
        return;
      query.Filters = (IEnumerable<SearchFilter>) new List<SearchFilter>();
    }

    protected void SetIndexingStatusInResponse(
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySearchResponse response,
      CollectionIndexingStatus indexingStatus)
    {
      switch (indexingStatus)
      {
        case CollectionIndexingStatus.NotIndexing:
          break;
        case CollectionIndexingStatus.Onboarding:
          response.AddError(new ErrorData()
          {
            ErrorCode = "AccountIsBeingOnboarded",
            ErrorType = ErrorType.Warning
          });
          break;
        case CollectionIndexingStatus.Reindexing:
          response.AddError(new ErrorData()
          {
            ErrorCode = "AccountIsBeingReindexed",
            ErrorType = ErrorType.Warning
          });
          break;
        case CollectionIndexingStatus.BranchIndexing:
          response.AddError(new ErrorData()
          {
            ErrorCode = "BranchesAreBeingIndexed",
            ErrorType = ErrorType.Warning
          });
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (indexingStatus), (object) indexingStatus, (string) null);
      }
    }

    protected void SetIndexingStatusInResponse(
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse response,
      CollectionIndexingStatus indexingStatus)
    {
      switch (indexingStatus)
      {
        case CollectionIndexingStatus.NotIndexing:
          break;
        case CollectionIndexingStatus.Onboarding:
          response.InfoCode = 6;
          break;
        case CollectionIndexingStatus.Reindexing:
          response.InfoCode = 1;
          break;
        case CollectionIndexingStatus.BranchIndexing:
          response.InfoCode = 9;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (indexingStatus), (object) indexingStatus, (string) null);
      }
    }

    protected CollectionIndexingStatus GetIndexingStatus(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      CollectionIndexingStatus indexingStatus = CollectionIndexingStatus.NotIndexing;
      IIndexingStatusService service = requestContext.GetService<IIndexingStatusService>();
      if (service.Supports(entityType))
        indexingStatus = service.GetCollectionIndexingStatus(entityType);
      return indexingStatus;
    }

    protected bool GetIndexingStatusDetails(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string projectName,
      string repoName,
      string branchName)
    {
      IIndexingStatusService service = requestContext.GetService<IIndexingStatusService>();
      return service.Supports(entityType) && service.GetIndexingStatusDetails(requestContext, entityType, projectName, repoName, branchName);
    }

    internal virtual void PublishRequest(EntitySearchQuery request)
    {
    }

    protected virtual void PublishResponse(Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySearchResponse response) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "REST-API", "ErrorCode", (object) response.Errors.Select<ErrorData, string>((Func<ErrorData, string>) (e => e.ErrorCode)));

    protected virtual void PublishResponseInClientTrace(Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse response) => Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("REST-API", "REST-API", "ErrorCode", (object) response.InfoCode);

    protected void PopulateSearchSecuredObjectInResponse(
      IVssRequestContext userRequestContext,
      Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySearchResponse response)
    {
      Guid namespaceId;
      if (!userRequestContext.TryGetItem<Guid>("searchServiceSecurityNamespaceGuidKey", out namespaceId))
        throw new InvalidOperationException("SecurityNamespaceGuid not found.");
      int requiredPermissions;
      if (!userRequestContext.TryGetItem<int>("searchServiceSecurityPermissionKey", out requiredPermissions))
        throw new InvalidOperationException("RequiredPermissions not found");
      string token;
      if (!userRequestContext.TryGetItem<string>("searchServiceSecurityTokenKey", out token) && string.IsNullOrEmpty(token))
        throw new InvalidOperationException("Token not found");
      response.SetSecuredObject(namespaceId, requiredPermissions, token);
    }

    protected void PopulateSearchSecuredObjectInResponse(
      IVssRequestContext userRequestContext,
      SearchSecuredV2Object response)
    {
      Guid namespaceId;
      if (!userRequestContext.TryGetItem<Guid>("searchServiceSecurityNamespaceGuidKey", out namespaceId))
        throw new InvalidOperationException("SecurityNamespaceGuid not found.");
      int requiredPermissions;
      if (!userRequestContext.TryGetItem<int>("searchServiceSecurityPermissionKey", out requiredPermissions))
        throw new InvalidOperationException("RequiredPermissions not found");
      string token;
      if (!userRequestContext.TryGetItem<string>("searchServiceSecurityTokenKey", out token) && string.IsNullOrEmpty(token))
        throw new InvalidOperationException("Token not found");
      response.SetSecuredObject(namespaceId, requiredPermissions, token);
    }

    internal IExpression GetFiltersToIgnoreDeletedDocumentsDuringReindexing() => (IExpression) new TermsExpression("isDocumentDeletedInReIndexing", Operator.In, (IEnumerable<string>) new string[1]
    {
      bool.FalseString.ToLowerInvariant()
    });
  }
}
