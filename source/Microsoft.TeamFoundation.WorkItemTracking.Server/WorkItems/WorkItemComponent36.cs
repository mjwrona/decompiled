// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent36
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent36 : WorkItemComponent35
  {
    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent36.UpdateWorkItemsResultsReader7(bypassRules, isAdmin, updateDataset, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));
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
      this.BindBoolean("@updateMentions", false);
      return (IList<WorkItemCommentUpdateRecord>) this.ExecuteUnknown<IEnumerable<WorkItemCommentUpdateRecord>>((System.Func<IDataReader, IEnumerable<WorkItemCommentUpdateRecord>>) (reader =>
      {
        if (linkUpdateRecords.Any<WorkItemResourceLinkUpdateRecord>())
          reader.NextResult();
        return new WorkItemComponent.WorkItemCommentUpdateRecordObjectBinder().BindAll(reader);
      })).ToList<WorkItemCommentUpdateRecord>();
    }

    internal class UpdateWorkItemsResultsReader7 : WorkItemComponent32.UpdateWorkItemsResultsReader6
    {
      private readonly System.Func<int, Guid> m_dataspaceIdentifierResolver;

      internal UpdateWorkItemsResultsReader7(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset,
        System.Func<int, Guid> GetDataspaceIdentifier)
        : base(bypassRules, isAdmin, updateDataset)
      {
        this.m_dataspaceIdentifierResolver = GetDataspaceIdentifier;
      }
    }
  }
}
