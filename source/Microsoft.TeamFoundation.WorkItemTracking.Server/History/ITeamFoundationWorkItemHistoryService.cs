// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.ITeamFoundationWorkItemHistoryService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  [DefaultServiceImplementation(typeof (TeamFoundationWorkItemHistoryService))]
  public interface ITeamFoundationWorkItemHistoryService : IVssFrameworkService
  {
    IEnumerable<WorkItemFieldHistoryRecord> GetFieldHistory(
      IVssRequestContext requestContext,
      long timeStamp);

    IEnumerable<WorkItemDestroyHistoryRecord> GetWorkItemDestroyHistory(
      IVssRequestContext requestContext,
      long timeStamp,
      int batchSize = 200);

    IEnumerable<WorkItemTypeCategoryHistoryRecord> GetWorkItemTypeCategoryHistory(
      IVssRequestContext requestContext,
      long timeStamp);

    IEnumerable<WorkItemTypeCategoryMemberHistoryRecord> GetWorkItemTypeCategoryMemberHistory(
      IVssRequestContext requestContext,
      long timeStamp);

    IEnumerable<WorkItemTypeRenameHistoryRecord> GetWorkItemTypeRenameHistory(
      IVssRequestContext requestContext,
      long timeStamp);

    IEnumerable<WorkItemLinkHistoryRecord> GetWorkItemLinkHistory(
      IVssRequestContext requestContext,
      long timeStamp,
      int batchSize = 200,
      DateTime? startDate = null);

    IEnumerable<WorkItemLinkTypeHistoryRecord> GetWorkItemLinkTypeHistory(
      IVssRequestContext requestContext,
      long timeStamp);

    ICollection<WorkItemIdRevisionPair> GetChangedRevisions(
      IVssRequestContext requestContext,
      int watermark,
      int batchSize = 200,
      DateTime? startDate = null);

    ICollection<WorkItemIdRevisionPair> GetChangedRevisions(
      IVssRequestContext requestContext,
      Guid? projectId,
      IEnumerable<string> types,
      int watermark,
      int batchSize = 200,
      DateTime? startDate = null);

    ICollection<WorkItemIdRevisionPair> GetChangedRevisionsPageable(
      IVssRequestContext requestContext,
      Guid? projectId,
      IEnumerable<string> types,
      bool includeLatestOnly,
      bool includeDiscussionChangesOnly,
      bool useLegacyDiscussionChanges,
      WorkItemIdRevisionPair idRevPair,
      int batchSize = 200,
      bool includeDiscussionHistory = false);

    WorkItemIdRevisionPair GetWorkItemWatermarkForDate(
      IVssRequestContext requestContext,
      DateTime date);

    IEnumerable<IDictionary<FieldEntry, object>> GetFieldValuesHistory(
      IVssRequestContext requestContext,
      IEnumerable<string> fieldReferences,
      int watermark,
      int batchSize = 200);

    IEnumerable<IDictionary<FieldEntry, object>> GetFieldValuesHistory(
      IVssRequestContext requestContext,
      IEnumerable<string> fieldReferences,
      IEnumerable<WorkItemIdRevisionPair> idRevPairs);

    IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool includeExtendedProperties = false);

    IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      bool includeExtendedProperties = false);

    IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinksByResourceIds(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<int, int>> workItemIdResourceIdPairs,
      bool includeExtendedProperties = false);

    int? GetWorkItemMinWatermark(IVssRequestContext requestContext, int fieldId);
  }
}
