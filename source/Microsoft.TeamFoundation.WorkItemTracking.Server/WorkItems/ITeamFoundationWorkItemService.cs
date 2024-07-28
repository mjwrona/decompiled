// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ITeamFoundationWorkItemService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [DefaultServiceImplementation(typeof (TeamFoundationWorkItemService))]
  public interface ITeamFoundationWorkItemService : IVssFrameworkService
  {
    void EvaluateRulesOnFieldValues(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeNameOrId,
      IEnumerable<int> fieldsToEvaluate,
      IDictionary<int, object> fieldValues,
      IDictionary<int, object> fieldUpdates);

    WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      DateTime? asOf,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      Guid? projectId = null,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    WorkItem GetWorkItemById(
      IVssRequestContext requestContext,
      int workItemId,
      int permissionsToCheck,
      Guid projectId,
      out string securityToken,
      DateTime? asOf = null,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      DateTime? asOf,
      Guid? projectId,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      DateTime? asOf,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      int permissionsToCheck,
      bool includeResourceLinks = true,
      bool includeWorkItemLinks = true,
      bool includeHistory = true,
      bool includeTags = true,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail,
      bool includeInRecentActivity = false,
      bool useWorkItemIdentity = false,
      bool includeCommentHistory = false,
      DateTime? revisionsSince = null);

    IEnumerable<WorkItem> GetDeletedWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<int> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      Guid? projectId,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<FieldEntry> fields,
      int permissionsToCheck = 16,
      DateTime? asOf = null,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<string> fields,
      int permissionsToCheck = 16,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool disableProjectionLevelThree = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemFieldData> GetWorkItemFieldValues(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      IEnumerable<FieldEntry> fields,
      int permissionsToCheck = 16,
      int batchSize = 200,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted,
      bool suppressCustomerIntelligence = false,
      bool useWorkItemIdentity = false,
      bool disableProjectionLevelThree = false);

    WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string workItemTypeReferenceName,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemStateOnTransition> GetNextStateOnCheckinWithExceptions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);

    IEnumerable<WorkItemUpdateResult> DeleteWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool skipNotifications = false,
      bool skipTestWorkItems = false);

    IEnumerable<WorkItemUpdateResult> RestoreWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);

    WorkItemUpdateResult UpdateWorkItem(
      IVssRequestContext requestContext,
      WorkItemUpdate workItemUpdate,
      bool bypassRules = false,
      bool includeInRecentActivity = false,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false);

    WorkItemUpdateResult UpdateWorkItem(
      IVssRequestContext requestContext,
      WorkItemUpdate workItemUpdate,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool validateOnly = false,
      bool includeInRecentActivity = false,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItemsRemoteLinkOnly(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      bool suppressQueueRemoteLinkJob = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      bool bypassRules = false,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false,
      bool checkRevisionsLimit = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      bool allOrNothing = false,
      bool validateOnly = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false,
      bool isPermissionCheckRequiredForBypassRules = false,
      bool useWorkItemIdentity = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = true,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IEnumerable<WorkItem> workItem,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = true,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false);

    IEnumerable<WorkItemUpdateResult> UpdateWorkItemsStateOnCheckin(
      IVssRequestContext requestContext,
      IDictionary<int, string> workItemIdToStateMap,
      string commentOnSave = null,
      bool allOrNothing = false,
      bool includeInRecentActivity = false,
      IReadOnlyCollection<int> workItemIdsToIncludeInRecentActivity = null,
      bool suppressNotifications = false);

    bool TryUpdateWorkItemIdToStateMap(
      IVssRequestContext requestContext,
      string description,
      out IDictionary<int, string> workItemIdToStateMap);

    void DestroyWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool skipNotifications = false,
      bool skipTestWorkItems = false);

    WorkItemComment GetWorkItemComment(
      IVssRequestContext requestContext,
      int workItemId,
      int revision);

    WorkItemComment GetWorkItemComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int revision);

    WorkItemComments GetWorkItemComments(
      IVssRequestContext requestContext,
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort);

    WorkItemComments GetWorkItemComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      int fromRevision,
      int count,
      CommentSortOrder sort);

    IEnumerable<ArtifactUriQueryResult> GetWorkItemIdsForArtifactUris(
      IVssRequestContext requestContext,
      IEnumerable<string> artifactUris,
      DateTime? asOfDate = null,
      Guid? filterUnderProjectId = null);

    IEnumerable<string> GetAllowedValues(IVssRequestContext requestContext, int fieldId);

    IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string project);

    IEnumerable<string> GetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool excludeIdentities = false);

    bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted);

    bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      out string securityToken);

    bool CanAccessWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      bool checkProjectReadPermission,
      WorkItemRetrievalMode workItemRetrievalMode = WorkItemRetrievalMode.NonDeleted);

    bool HasWorkItemPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId,
      WorkItemRetrievalMode workItemRetrievalMode,
      int permission,
      out string securityToken);

    void CreateOrUpdateWorkItemReactionsAggregateCount(
      IVssRequestContext requestContext,
      int workItemId,
      SocialEngagementType socialEngagementType,
      int incrementCounterValue);

    IEnumerable<WorkItemReactionsCount> GetSortedWorkItemReactionsCount(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      SocialEngagementType socialEngagementType);

    WorkItemChangesResult GetWorkItemsAndLinksChangedDate(
      IVssRequestContext requestContext,
      int linkType);

    int GetWorkItemUpdateId(IVssRequestContext requestContext, int workItemId);
  }
}
