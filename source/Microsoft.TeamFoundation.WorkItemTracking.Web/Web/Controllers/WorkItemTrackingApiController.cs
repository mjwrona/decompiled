// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTrackingApiController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.History;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class WorkItemTrackingApiController : TfsProjectApiController
  {
    private ITeamFoundationWorkItemService m_WorkItemService;
    private IWorkItemTypeService m_WorkItemTypeService;
    private IWorkItemQueryService m_QueryService;
    private ITeamFoundationQueryItemService m_QueryItemService;
    private IDataAccessLayer m_DataAccessLayer;
    private ITeamFoundationWorkItemHistoryService m_WarehouseService;
    private WorkItemTrackingRequestContext witRequestContext;
    private WebApiTeam m_Team;

    public override string ActivityLogArea => "WorkItem Tracking";

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.witRequestContext = this.TfsRequestContext.WitContext();
      this.TfsRequestContext.Items["WitApiResourceVersion"] = (object) this.Request.GetApiResourceVersion();
    }

    internal virtual bool ShouldUseIdentityRefForWorkItemFieldValues(
      IVssRequestContext requestContext)
    {
      return this.ControllerSupportsIdentityRefForWorkItemFieldValues();
    }

    internal virtual bool ShouldReturnProjectScopedUrls(IVssRequestContext requestContext) => this.ControllerSupportsProjectScopedUrls();

    protected virtual bool ControllerSupportsIdentityRefForWorkItemFieldValues() => true;

    protected virtual bool ControllerSupportsProjectScopedUrls() => true;

    internal virtual WorkItemTrackingRequestContext WitRequestContext => this.witRequestContext;

    public virtual ITeamFoundationWorkItemService WorkItemService => this.m_WorkItemService ?? (this.m_WorkItemService = this.TfsRequestContext.GetService<ITeamFoundationWorkItemService>());

    public virtual IWorkItemQueryService QueryService => this.m_QueryService ?? (this.m_QueryService = this.TfsRequestContext.GetService<IWorkItemQueryService>());

    public virtual ITeamFoundationQueryItemService QueryItemService => this.m_QueryItemService ?? (this.m_QueryItemService = this.TfsRequestContext.GetService<ITeamFoundationQueryItemService>());

    public virtual IWorkItemTypeService WorkItemTypeService => this.m_WorkItemTypeService ?? (this.m_WorkItemTypeService = this.TfsRequestContext.GetService<IWorkItemTypeService>());

    internal virtual IDataAccessLayer DataAccessLayer => this.m_DataAccessLayer ?? (this.m_DataAccessLayer = (IDataAccessLayer) new DataAccessLayerImpl(this.TfsRequestContext));

    internal virtual ITeamFoundationWorkItemHistoryService WarehouseService => this.m_WarehouseService ?? (this.m_WarehouseService = this.TfsRequestContext.GetService<ITeamFoundationWorkItemHistoryService>());

    internal virtual bool ExcludeUrls => this.Request != null && MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<WorkItemNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTypeNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<HistoryNotUpdatedForRevisionException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemIdRevisionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WitResourceNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingProjectNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<QueryItemNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingTreeNodeNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AttachmentNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ResourceLinkNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemUnauthorizedAccessException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ProjectDoesNotExistWithNameException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTypeCategoryNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<UndeletedQueryItemNotFound>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemLinkEndUnauthorizedAccessException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemLinkAddExtraParentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutGroupDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ProcessWorkItemTypeFieldDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PermissionDeniedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemUnauthorizedAdminPermissionRequiredException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemUnauthorizedAttachmentException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemTrackingQueryUnauthorizedAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemUnauthorizedHistoricalDataAccessException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<InvalidBatchWorkItemUpdateJsonException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SyntaxException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<LegacyQueryItemException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<LegacyValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<KeyNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ConflictingParametersException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTrackingFieldDefinitionNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<VssPropertyValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ResourceLinkTargetUnspecifiedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTrackingTypeTemplateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<PatchOperationFailedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidPatchFieldNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TestPatchOperationFailedException>(HttpStatusCode.PreconditionFailed);
      exceptionMap.AddStatusCode<AttachmentAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemAttachmentIncorrectOffsetException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemAttachmentNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<RuleValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemAttachmentExceedsMaxSizeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemFieldInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemFieldInvalidTreeNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<QueryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidOperationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UndeletedQueryItemParentNotFound>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UndeletedQueryHasSiblingWithSameName>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTrackingTypeTemplateNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTrackingAggregateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotInScopeIdentityException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemRevisionMismatchException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<WorkItemTrackingQueryResultSizeLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemFieldInvalidTreeIdException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemAttachmentAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTagNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemDateInFutureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemDatesNotIncreasingException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FieldNotCustomizableException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemDestroyException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemInvalidUpdateToIsDeletedFieldException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemInvalidRestoreRequestException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProcessFieldAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FieldHasNotBeenDeletedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutPageDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutSectionDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTrackingQueryTimeoutException>(HttpStatusCode.RequestTimeout);
      exceptionMap.AddStatusCode<WorkItemInvalidDestroyRequestException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemPickListItemNameAlreadyInUseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemPickListNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ProcessNotFoundByTypeIdException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<CannotDeletePicklistReferencedByFieldException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTargetProjectDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProjectDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemAreaPathDoesNotMatchTargetProjectException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTargetAreaPathNotProvidedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemIterationPathDoesNotMatchTargetProjectException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTargetIterationPathNotProvidedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTargetTypeDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTargetTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutControlAlreadyExistInGroupException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutGroupAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FormLayoutPageAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProcessPermissionException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemStateNameAlreadyInUseException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemStateCustomizationNotSupportedException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<WorkItemCommentNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemStateDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemStateDefinitionAlreadyExistsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemStateHideInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemStateBlockCompletedCategoryChangesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemStateNoStateInCategoryException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemStateOrderInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTypeTwoStateRestrictionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTypeStateLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTrackingQueryMaxWiqlTextLengthLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<FieldRuleModelValidationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<WorkItemTagLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ClassificationNodeDuplicateNameException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<InvalidDeleteWorkItemCallException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IdentityRefNotAcceptedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ReadRulesPermissionException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<ReadAllowedValuesNotAuthorizedException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<ReadDependentFieldsNotAuthorizedException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<CommentNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<CommentVersionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<CommentUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CommentThreadingNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BehaviorDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<BehaviorReferenceAlreadyExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<CannotReferenceMultipleBehaviorsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CannotReferenceBehaviorFromNonCustomWorkItemTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CannotReferenceAbstractBehaviorException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BehaviorNotReferencedByWorkItemTypeException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<PortfolioBehaviorLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProcessTemplateEmptyNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidWorkItemTemplateIdException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ProcessWorkItemTypeDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddTranslation<SqlException, UnexpectedException>();
      exceptionMap.AddTranslation<SqlTypeException, UnexpectedException>();
      exceptionMap.AddTranslation<AttachmentAlreadyExistsException, RelationAlreadyExistsException>();
      exceptionMap.AddTranslation<LegacyValidationException, VssServiceException>();
      exceptionMap.AddTranslation<WorkItemTrackingTypeTemplateException, VssServiceException>();
      exceptionMap.AddTranslation<PortfolioBehaviorLimitExceededException, PortfolioBehaviorLimitExceededException>();
    }

    protected Guid? GetNullableProjectId() => !(this.ProjectId == Guid.Empty) ? new Guid?(this.ProjectId) : new Guid?();

    public WebApiTeam Team => this.m_Team ?? (this.m_Team = this.ProjectInfo == null ? (WebApiTeam) null : TeamsUtility.GetTeamFromRequest(this.TfsRequestContext, this.ControllerContext, this.ProjectInfo, false));
  }
}
