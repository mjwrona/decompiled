// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent35
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent35 : WorkItemComponent34
  {
    protected override void BindUpdateWorkItemsParameter(
      IVssIdentity userIdentity,
      bool bypassRules,
      bool isAdmin,
      int trendInterval,
      bool dualSave,
      WorkItemUpdateDataset updateDataset,
      int workItemLinksLimit,
      int workItemRemoteLinksLimit)
    {
      base.BindUpdateWorkItemsParameter(userIdentity, bypassRules, isAdmin, trendInterval, dualSave, updateDataset, workItemLinksLimit, workItemRemoteLinksLimit);
      this.BindWorkItemMentionUpdates("@mentionUpdates", (IEnumerable<WorkItemMentionRecord>) new List<WorkItemMentionRecord>());
    }

    public override IList<WorkItemCommentUpdateRecord> AddWorkItemComments(
      Guid addedBy,
      DateTime addedDate,
      IEnumerable<WorkItemCommentUpdateRecord> comments,
      List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords,
      IEnumerable<WorkItemMentionRecord> mentions,
      bool updateLegacyText = false)
    {
      this.PrepareStoredProcedure("prc_AddWorkItemComments");
      this.BindGuid("@authorizedAs", addedBy);
      this.BindDateTime("@authorizedDate", addedDate);
      this.BindWorkItemCommentUpdates("@commentUpdates", comments);
      this.BindWorkItemMentionUpdates("@mentionUpdates", (IEnumerable<WorkItemMentionRecord>) new List<WorkItemMentionRecord>());
      this.BindWorkItemResourceLinkUpdates("@linkUpdates", (IEnumerable<WorkItemResourceLinkUpdateRecord>) linkUpdateRecords);
      this.BindBoolean("@updateLongText", updateLegacyText);
      return (IList<WorkItemCommentUpdateRecord>) this.ExecuteUnknown<IEnumerable<WorkItemCommentUpdateRecord>>((System.Func<IDataReader, IEnumerable<WorkItemCommentUpdateRecord>>) (reader =>
      {
        if (linkUpdateRecords.Any<WorkItemResourceLinkUpdateRecord>())
          reader.NextResult();
        return new WorkItemComponent.WorkItemCommentUpdateRecordObjectBinder().BindAll(reader);
      })).ToList<WorkItemCommentUpdateRecord>();
    }

    public override IList<WorkItemCommentUpdateRecord> UpdateWorkItemComments(
      Guid modifiedBy,
      DateTime modifiedDate,
      IEnumerable<WorkItemCommentUpdateRecord> comments,
      List<WorkItemResourceLinkUpdateRecord> linkUpdateRecords,
      IEnumerable<WorkItemMentionRecord> mentions,
      bool updateLegacyText = false,
      bool updateMentions = false)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemComments");
      this.BindGuid("@changedBy", modifiedBy);
      this.BindDateTime("@changedDate", modifiedDate);
      this.BindWorkItemCommentUpdates("@commentUpdates", comments);
      this.BindWorkItemMentionUpdates("@mentionUpdates", (IEnumerable<WorkItemMentionRecord>) new List<WorkItemMentionRecord>());
      this.BindWorkItemResourceLinkUpdates("@linkUpdates", (IEnumerable<WorkItemResourceLinkUpdateRecord>) linkUpdateRecords);
      this.BindBoolean("@updateLongText", updateLegacyText);
      return (IList<WorkItemCommentUpdateRecord>) this.ExecuteUnknown<IEnumerable<WorkItemCommentUpdateRecord>>((System.Func<IDataReader, IEnumerable<WorkItemCommentUpdateRecord>>) (reader =>
      {
        if (linkUpdateRecords.Any<WorkItemResourceLinkUpdateRecord>())
          reader.NextResult();
        return new WorkItemComponent.WorkItemCommentUpdateRecordObjectBinder().BindAll(reader);
      })).ToList<WorkItemCommentUpdateRecord>();
    }
  }
}
