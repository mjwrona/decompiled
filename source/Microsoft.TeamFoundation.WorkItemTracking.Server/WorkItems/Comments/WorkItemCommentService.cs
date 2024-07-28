// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemCommentService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemCommentService : IVssFrameworkService
  {
    public static readonly int MaxAllowedPageSize = 200;
    private static readonly Guid WorkItemArtifactKind = WorkItemArtifactKinds.WorkItem;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemComments GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      ISet<int> ids,
      WorkItemCommentExpandOptions expandOptions = WorkItemCommentExpandOptions.None,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      requestContext.TraceEnter(120001, nameof (WorkItemCommentService), "Service");
      try
      {
        ArgumentUtility.CheckBoundsInclusive(ids.Count, 1, WorkItemCommentService.MaxAllowedPageSize, nameof (ids));
        string securityToken = WorkItemCommentService.CheckWorkItemReadPermissions(requestContext, projectId, workItemId);
        CommentsList comments = requestContext.GetService<ICommentService>().GetComments(requestContext, projectId, WorkItemCommentService.WorkItemArtifactKind, workItemId.ToString(), ids, (ExpandOptions) expandOptions, includeDeleted);
        return WorkItemComments.FromCommentsList(requestContext, comments, projectId, 16, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120009, nameof (WorkItemCommentService), "Service");
      }
    }

    public WorkItemComments GetComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int? top = null,
      string continuationToken = null,
      WorkItemCommentExpandOptions expandOptions = WorkItemCommentExpandOptions.None,
      bool includeDeleted = false,
      CommentSortOrder order = CommentSortOrder.Desc)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      requestContext.TraceEnter(120001, nameof (WorkItemCommentService), "Service");
      try
      {
        if (top.HasValue)
          ArgumentUtility.CheckBoundsInclusive(top.Value, 1, WorkItemCommentService.MaxAllowedPageSize, nameof (top));
        string securityToken = WorkItemCommentService.CheckWorkItemReadPermissions(requestContext, projectId, workItemId);
        ICommentService service = requestContext.GetService<ICommentService>();
        if (!expandOptions.HasFlag((Enum) WorkItemCommentExpandOptions.RenderedText) && !requestContext.IsContributedFeatureEnabled("ms.vss-work-web.new-boards-hub-feature"))
          expandOptions |= WorkItemCommentExpandOptions.RenderedText;
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        Guid itemArtifactKind = WorkItemCommentService.WorkItemArtifactKind;
        string artifactId = workItemId.ToString();
        int? top1 = top;
        string continuationToken1 = continuationToken;
        int num1 = (int) expandOptions;
        int num2 = includeDeleted ? 1 : 0;
        int order1 = (int) order;
        int? parentId = new int?();
        CommentsList comments = service.GetComments(requestContext1, projectId1, itemArtifactKind, artifactId, top1, continuationToken1, (ExpandOptions) num1, num2 != 0, (Microsoft.TeamFoundation.Comments.Server.SortOrder) order1, parentId);
        return WorkItemComments.FromCommentsList(requestContext, comments, projectId, 16, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120009, nameof (WorkItemCommentService), "Service");
      }
    }

    public virtual WorkItemComment GetComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      WorkItemCommentExpandOptions expandOptions = WorkItemCommentExpandOptions.None,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(120061, nameof (WorkItemCommentService), "Service");
      try
      {
        string securityToken = WorkItemCommentService.CheckWorkItemReadPermissions(requestContext, projectId, workItemId);
        Comment comment = requestContext.GetService<ICommentService>().GetComment(requestContext, projectId, WorkItemCommentService.WorkItemArtifactKind, workItemId.ToString(), commentId, (ExpandOptions) expandOptions, includeDeleted);
        return WorkItemComment.FromComment(requestContext, comment, projectId, 16, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120069, nameof (WorkItemCommentService), "Service");
      }
    }

    public ICollection<WorkItemCommentVersion> GetCommentVersions(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(120041, nameof (WorkItemCommentService), "Service");
      try
      {
        string securityToken = WorkItemCommentService.CheckWorkItemReadPermissions(requestContext, projectId, workItemId);
        return (ICollection<WorkItemCommentVersion>) requestContext.GetService<ICommentService>().GetCommentVersions(requestContext, projectId, WorkItemCommentService.WorkItemArtifactKind, workItemId.ToString(), commentId).Select<CommentVersion, WorkItemCommentVersion>((Func<CommentVersion, WorkItemCommentVersion>) (cv => WorkItemCommentVersion.FromCommentVersion(requestContext, cv, projectId, 16, securityToken))).ToList<WorkItemCommentVersion>();
      }
      finally
      {
        requestContext.TraceLeave(120049, nameof (WorkItemCommentService), "Service");
      }
    }

    public WorkItemCommentVersion GetCommentVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId,
      int version)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      ArgumentUtility.CheckForNonnegativeInt(version, nameof (version));
      requestContext.TraceEnter(120041, nameof (WorkItemCommentService), "Service");
      try
      {
        string securityToken = WorkItemCommentService.CheckWorkItemReadPermissions(requestContext, projectId, workItemId);
        CommentVersion commentVersion = requestContext.GetService<ICommentService>().GetCommentVersion(requestContext, projectId, WorkItemCommentService.WorkItemArtifactKind, workItemId.ToString(), commentId, version);
        return WorkItemCommentVersion.FromCommentVersion(requestContext, commentVersion, projectId, 16, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120049, nameof (WorkItemCommentService), "Service");
      }
    }

    public WorkItemComment AddComment(
      IVssRequestContext requestContext,
      Guid projectId,
      AddWorkItemComment comment)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<AddWorkItemComment>(comment, nameof (comment));
      requestContext.TraceEnter(120011, nameof (WorkItemCommentService), "Service");
      try
      {
        TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
        string securityToken;
        WorkItem workItemById = this.GetWorkItemById(requestContext, service, comment.WorkItemId, this.ShouldFetchResourceLinks(requestContext, projectId, comment.Text), projectId, out securityToken);
        this.ValidateTextLength(requestContext, comment.WorkItemId, comment.Text);
        if (WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsScanningEnabled(requestContext) && !WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBypassScanningEnabled(requestContext))
          this.ValidateTextContainsSecrets(requestContext, projectId, comment.WorkItemId, workItemById.Revision, comment.Text);
        Guid userId = requestContext.GetUserId();
        DateTime utcNow = DateTime.UtcNow;
        string textToSave;
        string fixedText;
        List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords;
        Comment processedComment = this.GetProcessedComment(requestContext, comment.ArtifactKind, comment.Text, (CommentFormat) comment.Format, projectId, workItemById, out textToSave, out fixedText, out linkUpdateRecords);
        WorkItemCommentUpdateRecord commentUpdateRecord1 = new WorkItemCommentUpdateRecord()
        {
          ArtifactKind = comment.ArtifactKind,
          ArtifactId = comment.WorkItemId.ToString(),
          Format = new WorkItemCommentFormat?((WorkItemCommentFormat) comment.Format),
          Text = textToSave,
          RenderedText = processedComment.RenderedText,
          TempId = -1
        };
        bool flag = WorkItemCommentService.IsDualWriteEnabled(requestContext);
        IEnumerable<WorkItemCommentUpdateRecord> source;
        using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
          source = (IEnumerable<WorkItemCommentUpdateRecord>) component.AddWorkItemComments(userId, utcNow, (IEnumerable<WorkItemCommentUpdateRecord>) new WorkItemCommentUpdateRecord[1]
          {
            commentUpdateRecord1
          }, linkUpdateRecords, (IEnumerable<WorkItemMentionRecord>) new List<WorkItemMentionRecord>(), (flag ? 1 : 0) != 0);
        WorkItemCommentUpdateRecord commentUpdateRecord2 = source.Single<WorkItemCommentUpdateRecord>();
        commentUpdateRecord2.TextWithProcessedUrls = fixedText;
        this.FireWorkItemChangedEvent(requestContext, service, workItemById, commentUpdateRecord2, userId);
        this.FireRecentActivityEvent(requestContext, projectId, comment.WorkItemId, userId, utcNow, workItemById.AreaId);
        return WorkItemComment.FromUpdateRecord(requestContext, commentUpdateRecord2, projectId, 32, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120019, nameof (WorkItemCommentService), "Service");
      }
    }

    public WorkItemComment UpdateComment(
      IVssRequestContext requestContext,
      Guid projectId,
      UpdateWorkItemComment comment)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<UpdateWorkItemComment>(comment, nameof (comment));
      requestContext.TraceEnter(120021, nameof (WorkItemCommentService), "Service");
      try
      {
        TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
        string securityToken;
        WorkItem workItemById = this.GetWorkItemById(requestContext, service, comment.WorkItemId, this.ShouldFetchResourceLinks(requestContext, projectId, comment.Text), projectId, out securityToken);
        this.ValidateTextLength(requestContext, comment.WorkItemId, comment.Text);
        string artifactId = comment.WorkItemId.ToString();
        Guid userId = requestContext.GetUserId();
        DateTime utcNow = DateTime.UtcNow;
        this.CheckCanModifyComment(requestContext, projectId, artifactId, comment.CommentId, userId);
        if (WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsScanningEnabled(requestContext) && !WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBypassScanningEnabled(requestContext))
          this.ValidateTextContainsSecrets(requestContext, projectId, comment.WorkItemId, workItemById.Revision, comment.Text);
        string textToSave;
        string fixedText;
        List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords;
        Comment processedComment = this.GetProcessedComment(requestContext, comment.ArtifactKind, comment.Text, (CommentFormat) comment.Format, projectId, workItemById, out textToSave, out fixedText, out linkUpdateRecords);
        WorkItemCommentUpdateRecord record = new WorkItemCommentUpdateRecord()
        {
          CommentId = comment.CommentId,
          ArtifactKind = comment.ArtifactKind,
          ArtifactId = artifactId,
          Format = new WorkItemCommentFormat?((WorkItemCommentFormat) comment.Format),
          Text = textToSave,
          RenderedText = processedComment.RenderedText,
          ModifiedBy = userId,
          ModifiedDate = utcNow
        };
        WorkItemCommentUpdateRecord commentUpdateRecord = this.ExecuteUpdate(requestContext, record, linkUpdateRecords);
        commentUpdateRecord.TextWithProcessedUrls = fixedText;
        this.FireWorkItemChangedEvent(requestContext, service, workItemById, commentUpdateRecord, userId);
        this.FireRecentActivityEvent(requestContext, projectId, comment.WorkItemId, userId, utcNow, workItemById.AreaId);
        return WorkItemComment.FromUpdateRecord(requestContext, commentUpdateRecord, projectId, 32, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120029, nameof (WorkItemCommentService), "Service");
      }
    }

    public WorkItemComment DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int commentId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNonnegativeInt(workItemId, nameof (workItemId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      requestContext.TraceEnter(120031, nameof (WorkItemCommentService), "Service");
      try
      {
        TeamFoundationWorkItemService service = requestContext.GetService<TeamFoundationWorkItemService>();
        string securityToken;
        WorkItem workItemById = this.GetWorkItemById(requestContext, service, workItemId, false, projectId, out securityToken);
        string artifactId = workItemId.ToString();
        Guid userId = requestContext.GetUserId();
        this.CheckCanModifyComment(requestContext, projectId, artifactId, commentId, userId);
        DateTime utcNow = DateTime.UtcNow;
        WorkItemCommentUpdateRecord record = new WorkItemCommentUpdateRecord()
        {
          CommentId = commentId,
          ArtifactKind = WorkItemCommentService.WorkItemArtifactKind,
          ArtifactId = artifactId,
          ModifiedBy = userId,
          ModifiedDate = utcNow,
          IsDeleted = true
        };
        WorkItemCommentUpdateRecord commentUpdateRecord = this.ExecuteUpdate(requestContext, record, new List<WorkItemResourceLinkUpdateRecord>());
        this.FireWorkItemChangedEvent(requestContext, service, workItemById, commentUpdateRecord, userId);
        this.FireRecentActivityEvent(requestContext, projectId, workItemId, userId, utcNow, workItemById.AreaId);
        return WorkItemComment.FromUpdateRecord(requestContext, commentUpdateRecord, projectId, 32, securityToken);
      }
      finally
      {
        requestContext.TraceLeave(120039, nameof (WorkItemCommentService), "Service");
      }
    }

    private Comment GetProcessedComment(
      IVssRequestContext requestContext,
      Guid artifactKind,
      string text,
      CommentFormat format,
      Guid projectId,
      WorkItem workItem,
      out string textToSave,
      out string fixedText,
      out List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords)
    {
      TeamFoundationWorkItemService service1 = requestContext.GetService<TeamFoundationWorkItemService>();
      ICommentService service2 = requestContext.GetService<ICommentService>();
      string str = format != CommentFormat.Markdown ? SafeHtmlWrapper.MakeSafe(text) : SafeHtmlWrapper.MakeSafeWithMdMentions(text);
      int num = 0;
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      int id = workItem.Id;
      string text1 = str;
      Guid projectGuid = projectId;
      WorkItem fieldData = workItem;
      ref int local = ref num;
      (textToSave, fixedText, linkUpdateRecords) = service1.HandleInlineImages(witRequestContext, id, text1, projectGuid, (WorkItemFieldData) fieldData, ref local);
      ProcessComment comment = new ProcessComment(workItem.Id.ToString(), text, format);
      return service2.ProcessComment(requestContext, projectId, artifactKind, comment);
    }

    internal virtual void FireRecentActivityEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      Guid changerId,
      DateTime changedDate,
      int areaId)
    {
      requestContext.GetService<WorkItemRecentActivityService>().TryFireWorkItemRecentActivityEvent(requestContext, WorkItemRecentActivityType.Edited, (IReadOnlyCollection<(int, Guid, int)>) new (int, Guid, int)[1]
      {
        (workItemId, projectId, areaId)
      }, changerId, changedDate);
    }

    internal virtual void FireWorkItemChangedEvent(
      IVssRequestContext requestContext,
      TeamFoundationWorkItemService workItemService,
      WorkItem workItemBeforeUpdate,
      WorkItemCommentUpdateRecord commentRecord,
      Guid modifiedBy)
    {
      bool isNoHistoryEnabledFieldsSupported = WorkItemTrackingFeatureFlags.IsNoHistoryEnabledFieldsSupported(requestContext);
      WorkItemTrackingRequestContext witRequestContext1 = requestContext.WitContext();
      WorkItemUpdate update = new WorkItemUpdate()
      {
        Id = workItemBeforeUpdate.Id,
        Rev = workItemBeforeUpdate.Revision,
        FieldData = (WorkItemFieldData) workItemBeforeUpdate
      };
      WorkItemTypeExtension[] activeExtensions = workItemBeforeUpdate.GetActiveExtensions(requestContext);
      WorkItemUpdateState workItemUpdateState = new WorkItemUpdateState(witRequestContext1, update, isNoHistoryEnabledFieldsSupported)
      {
        FieldData = (WorkItemFieldData) workItemBeforeUpdate,
        CurrentExtensions = activeExtensions,
        OldExtensions = activeExtensions
      };
      if (WorkItemTrackingFeatureFlags.IsUpdateDateInWorkitemUpdateEnabled(requestContext))
        workItemUpdateState.UpdateDate = commentRecord.ModifiedDate;
      foreach (WorkItemTypeExtension currentExtension in workItemUpdateState.CurrentExtensions)
        workItemUpdateState.FieldData.SetFieldValue(witRequestContext1, currentExtension.MarkerField.Field, (object) true);
      workItemUpdateState.WorkItemComment = commentRecord;
      List<WorkItemUpdateState> updateStates1 = new List<WorkItemUpdateState>()
      {
        workItemUpdateState
      };
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap1 = WorkItemCommentService.GetIdentityMap(workItemService, witRequestContext1, updateStates1);
      string displayNameFromId = WorkItemCommentService.GetDistinctDisplayNameFromId(identityMap1, modifiedBy);
      if (displayNameFromId == null)
      {
        requestContext.Trace(120051, TraceLevel.Error, nameof (WorkItemCommentService), "Service", string.Format("Changer Identity with Id:{0} cannot be found in the identitymap", (object) modifiedBy));
      }
      else
      {
        string distinctDisplayName1 = workItemBeforeUpdate.GetFieldValue(requestContext, "System.ChangedBy").ToString();
        if (WorkItemCommentService.GetIdFromDistinctDisplayName(identityMap1, distinctDisplayName1) != modifiedBy)
          workItemBeforeUpdate.SetFieldValue(requestContext, "System.ChangedBy", (object) displayNameFromId);
        string distinctDisplayName2 = workItemBeforeUpdate.GetFieldValue(requestContext, "System.AuthorizedAs").ToString();
        if (WorkItemCommentService.GetIdFromDistinctDisplayName(identityMap1, distinctDisplayName2) != modifiedBy)
          workItemBeforeUpdate.SetFieldValue(requestContext, "System.AuthorizedAs", (object) displayNameFromId);
      }
      workItemBeforeUpdate.SetFieldValue(requestContext, "System.Rev", (object) commentRecord.ArtifactRevision);
      workItemBeforeUpdate.SetFieldValue(requestContext, "System.Watermark", (object) commentRecord.Watermark);
      workItemBeforeUpdate.SetFieldValue(requestContext, "System.ChangedDate", (object) commentRecord.ModifiedDate);
      workItemBeforeUpdate.SetFieldValue(requestContext, "System.AuthorizedDate", (object) commentRecord.ModifiedDate);
      TeamFoundationWorkItemService foundationWorkItemService = workItemService;
      WorkItemTrackingRequestContext witRequestContext2 = witRequestContext1;
      Dictionary<int, WorkItemFieldData> fieldDataMap = new Dictionary<int, WorkItemFieldData>();
      fieldDataMap.Add(workItemBeforeUpdate.Id, (WorkItemFieldData) workItemBeforeUpdate);
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap2 = identityMap1;
      List<WorkItemUpdateState> updateStates2 = updateStates1;
      foundationWorkItemService.TryFireWorkItemChangedEvents(witRequestContext2, (IDictionary<int, WorkItemFieldData>) fieldDataMap, identityMap2, (IEnumerable<WorkItemUpdateState>) updateStates2);
    }

    private static IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentityMap(
      TeamFoundationWorkItemService workItemService,
      WorkItemTrackingRequestContext witRequestContext,
      List<WorkItemUpdateState> updateStates)
    {
      ResolvedIdentityNamesInfo identityNamesInfo = workItemService.GetResolvedIdentityNamesInfo(witRequestContext, (IEnumerable<WorkItemUpdateState>) updateStates, WorkItemUpdateRuleExecutionMode.ValidationOnly);
      return workItemService.ResolveReferencedIdentities(witRequestContext, identityNamesInfo);
    }

    private static Guid GetIdFromDistinctDisplayName(
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      string distinctDisplayName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      return !string.IsNullOrEmpty(distinctDisplayName) && identityMap.TryGetValue(distinctDisplayName, out identity) ? identity.Id : Guid.Empty;
    }

    private static string GetDistinctDisplayNameFromId(
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap,
      Guid tfid)
    {
      if (identityMap != null)
      {
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity> identity in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.Identity.Identity>>) identityMap)
        {
          if (identity.Value.Id == tfid)
            return identity.Key;
        }
      }
      return (string) null;
    }

    private WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      TeamFoundationWorkItemService workItemService,
      int workItemId,
      bool includeResourceLinks,
      Guid projectId,
      out string securityToken)
    {
      TeamFoundationWorkItemService foundationWorkItemService = workItemService;
      IVssRequestContext requestContext1 = requestContext;
      int[] workItemIds = new int[1]{ workItemId };
      DateTime? asOf = new DateTime?();
      ref string local = ref securityToken;
      Guid? nullable = new Guid?(projectId);
      int num = includeResourceLinks ? 1 : 0;
      Guid? projectId1 = nullable;
      DateTime? revisionsSince = new DateTime?();
      return foundationWorkItemService.GetWorkItemsPredicate(requestContext1, (IEnumerable<int>) workItemIds, 512, (Func<IPermissionCheckHelper, int, int, bool>) ((permissionsHelper, areaId, permissionsToCheck) => permissionsHelper.HasWorkItemPermission(areaId, permissionsToCheck, 32)), (Action<int, AccessType>) ((id, accessType) =>
      {
        throw new WorkItemUnauthorizedCommentsAccessException();
      }), asOf, out local, num != 0, false, false, projectId: projectId1, revisionsSince: revisionsSince).SingleOrDefault<WorkItem>();
    }

    private WorkItemCommentUpdateRecord ExecuteUpdate(
      IVssRequestContext requestContext,
      WorkItemCommentUpdateRecord record,
      List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords)
    {
      bool flag = WorkItemCommentService.IsDualWriteEnabled(requestContext);
      IEnumerable<WorkItemCommentUpdateRecord> source;
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        source = (IEnumerable<WorkItemCommentUpdateRecord>) component.UpdateWorkItemComments(record.ModifiedBy, record.ModifiedDate, (IEnumerable<WorkItemCommentUpdateRecord>) new WorkItemCommentUpdateRecord[1]
        {
          record
        }, linkUpdateRecords, (IEnumerable<WorkItemMentionRecord>) new List<WorkItemMentionRecord>(), (flag ? 1 : 0) != 0);
      return source.SingleOrDefault<WorkItemCommentUpdateRecord>() ?? throw new CommentNotFoundException(record.CommentId).Expected(requestContext.ServiceName);
    }

    private static bool IsDualWriteEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsCommentServiceDualWriteEnabled(requestContext) && WorkItemTrackingFeatureFlags.IsCommentServiceLegacyWritesEnabled(requestContext);

    private static string CheckWorkItemReadPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId)
    {
      return WorkItemCommentService.CheckWorkItemPermission(requestContext, projectId, workItemId, 16);
    }

    private static string CheckWorkItemPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int permission)
    {
      string securityToken;
      if (!requestContext.GetService<ITeamFoundationWorkItemService>().HasWorkItemPermission(requestContext, projectId, workItemId, WorkItemRetrievalMode.All, permission, out securityToken))
        throw new WorkItemNotFoundException(workItemId).Expected(requestContext.ServiceName);
      return securityToken;
    }

    private void CheckCanModifyComment(
      IVssRequestContext requestContext,
      Guid projectId,
      string artifactId,
      int commentId,
      Guid userId)
    {
      Comment comment = requestContext.GetService<ICommentService>().GetComment(requestContext, projectId, WorkItemCommentService.WorkItemArtifactKind, artifactId, commentId);
      Guid result;
      if (comment.CreatedBy != userId && (!Guid.TryParse(comment.CreatedOnBehalfOf, out result) || result != userId))
        throw new CommentUpdateException(commentId).Expected(requestContext.ServiceName);
    }

    private void ValidateTextLength(IVssRequestContext requestContext, int workItemId, string text)
    {
      int maxLongTextSize = requestContext.WitContext().ServerSettings.MaxLongTextSize;
      if (maxLongTextSize > 0 && text.Length > maxLongTextSize)
        throw new WorkItemMaxCommentTextSizeExceededException(maxLongTextSize).Expected(requestContext.ServiceName);
    }

    internal void ValidateTextContainsSecrets(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int revisionId,
      string text)
    {
      string exceptionMessage = (string) null;
      try
      {
        if (requestContext == null || !requestContext.IsMicrosoftTenant() || string.IsNullOrEmpty(text))
          return;
        WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
        WorkItemCommentScanData eventArg = new WorkItemCommentScanData(projectId, workItemId, revisionId);
        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(trackingRequestContext.ServerSettings.MaxSecretsScanServiceRequestTimeoutInMilliseconds))
          new WorkItemCommentSecretsScanner(cancellationTokenSource.Token).Process(requestContext, eventArg, text, out exceptionMessage);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(904945, "WorkItemService", "SecretScanWorkItemComments", ex);
      }
      if (WorkItemTrackingFeatureFlags.IsWorkItemTrackingSecretsBlockingEnabled(requestContext) && !string.IsNullOrEmpty(exceptionMessage))
        throw new WorkItemFieldInvalidException(workItemId, exceptionMessage);
    }

    protected virtual bool ShouldFetchResourceLinks(
      IVssRequestContext requestContext,
      Guid projectId,
      string commentText)
    {
      Uri projectAttachmentUri = TeamFoundationWorkItemService.GetProjectAttachmentUri(requestContext, new Guid?(projectId));
      return commentText.IndexOf(projectAttachmentUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase) >= 0;
    }
  }
}
