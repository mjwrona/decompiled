// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent39
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent39 : WorkItemComponent38
  {
    protected override SqlParameter BindWorkItemMentionUpdates(
      string parameterName,
      IEnumerable<WorkItemMentionRecord> rows)
    {
      return (SqlParameter) null;
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
      this.BindWorkItemResourceLinkUpdates("@linkUpdates", (IEnumerable<WorkItemResourceLinkUpdateRecord>) linkUpdateRecords);
      this.BindBoolean("@updateLongText", updateLegacyText);
      return (IList<WorkItemCommentUpdateRecord>) this.ExecuteUnknown<IEnumerable<WorkItemCommentUpdateRecord>>((System.Func<IDataReader, IEnumerable<WorkItemCommentUpdateRecord>>) (reader =>
      {
        if (linkUpdateRecords.Any<WorkItemResourceLinkUpdateRecord>())
          reader.NextResult();
        return new WorkItemComponent.WorkItemCommentUpdateRecordObjectBinder().BindAll(reader);
      })).ToList<WorkItemCommentUpdateRecord>();
    }

    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent39.UpdateWorkItemsResultsReader8(bypassRules, isAdmin, updateDataset, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));
    }

    internal class UpdateWorkItemsResultsReader8 : WorkItemComponent36.UpdateWorkItemsResultsReader7
    {
      internal UpdateWorkItemsResultsReader8(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset,
        System.Func<int, Guid> GetDataspaceIdentifier)
        : base(bypassRules, isAdmin, updateDataset, GetDataspaceIdentifier)
      {
      }

      protected override void CheckAndReadWorkItemCommentUpdates(IDataReader reader)
      {
        if (this.updateDataset.WorkItemCommentUpdates.Count <= 0 || !reader.NextResult())
          return;
        this.result.WorkItemCommentUpdateResults = (IEnumerable<WorkItemCommentUpdateRecord>) WorkItemTrackingResourceComponent.Bind<WorkItemCommentUpdateRecord>(reader, (System.Func<IDataReader, WorkItemCommentUpdateRecord>) (r => new WorkItemCommentUpdateRecord()
        {
          CommentId = r.GetInt32(0),
          ArtifactId = r.GetString(1),
          ArtifactKind = r.GetGuid(2),
          Version = r.GetInt32(3),
          Text = r.GetString(4),
          RenderedText = r.GetString(5),
          Format = new WorkItemCommentFormat?((WorkItemCommentFormat) r.GetByte(6)),
          CreatedBy = r.GetGuid(7),
          CreatedDate = r.GetDateTime(8),
          CreatedOnBehalfOf = r.GetString(9),
          CreatedOnBehalfDate = new DateTime?(r.GetDateTime(10)),
          ModifiedBy = r.GetGuid(11),
          ModifiedDate = r.GetDateTime(12),
          IsDeleted = r.GetBoolean(13)
        })).ToArray<WorkItemCommentUpdateRecord>();
      }
    }
  }
}
