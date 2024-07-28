// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent32
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent32 : WorkItemComponent31
  {
    internal override void BindparametersForGetWorkItemsProc(
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool sortLinks,
      int maxLongTextLength,
      int maxRevisionLongTextLength,
      DateTime? asOf,
      DateTime? revisionsSince,
      bool includeComments,
      bool includeCommentHistory)
    {
      this.BindBoolean("@includeComments", includeComments);
      this.BindBoolean("@includeCommentHistory", includeCommentHistory);
      base.BindparametersForGetWorkItemsProc(workItemIds, includeCountFields, includeCustomFields, includeTextFields, includeResourceLinks, includeWorkItemLinks, includeHistory, sortLinks, maxLongTextLength, maxRevisionLongTextLength, asOf, revisionsSince, includeComments, includeCommentHistory);
    }

    protected override WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return (WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>) new WorkItemComponent.WorkItemDatasetBinder8<WorkItemDataset>(bindTitle, bindCountFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), this.RequestContext.WitContext().FieldDictionary);
    }

    protected override WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return (WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValuesBinder8<WorkItemFieldValues>(wideTableFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree, this.RequestContext.WitContext().FieldDictionary);
    }

    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent32.UpdateWorkItemsResultsReader6(bypassRules, isAdmin, updateDataset);
    }

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
      this.BindWorkItemCommentUpdates("@commentUpdates", (IEnumerable<WorkItemCommentUpdateRecord>) updateDataset.WorkItemCommentUpdates);
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

    internal class UpdateWorkItemsResultsReader6 : WorkItemComponent27.UpdateWorkItemsResultsReader5
    {
      internal UpdateWorkItemsResultsReader6(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset)
        : base(bypassRules, isAdmin, updateDataset)
      {
      }

      internal override WorkItemUpdateResultSet Read(IDataReader reader)
      {
        this.result = new WorkItemUpdateResultSet()
        {
          Success = false,
          FailureReason = "Default"
        };
        this.CheckAndReadChangedByInfo(reader);
        if (!this.CheckAndReadPendingSetMembershipChecks(reader))
        {
          this.result.FailureReason = "CheckAndReadPendingSetMembershipChecks failed: " + string.Join(",", this.result.SetMembershipCheckResults.Select<PendingSetMembershipCheckResultRecord, string>((System.Func<PendingSetMembershipCheckResultRecord, string>) (r => string.Format("{0}-{1}-{2}", (object) r.WorkItemId, (object) r.FieldId, (object) r.Status))));
          return this.result;
        }
        if (this.updateDataset.CoreFieldUpdates.Any<WorkItemCoreFieldUpdatesRecord>())
        {
          reader.NextResult();
          if (!this.ReadCoreFieldUpdates(reader))
          {
            this.result.FailureReason = "ReadCoreFieldUpdates failed";
            return this.result;
          }
          if (this.tieCoreFieldUpdatesWithResourceLinkUpdates && !this.CheckAndReadResourceLinkUpdates(reader))
          {
            this.result.FailureReason = "CheckAndReadResourceLinkUpdates core failed";
            return this.result;
          }
        }
        if (!this.tieCoreFieldUpdatesWithResourceLinkUpdates && !this.CheckAndReadResourceLinkUpdates(reader))
        {
          this.result.FailureReason = "CheckAndReadResourceLinkUpdates non-core failed";
          return this.result;
        }
        if (!this.CheckAndReadWorkItemLinkUpdates(reader))
        {
          this.result.FailureReason = "CheckAndReadWorkItemLinkUpdates failed";
          return this.result;
        }
        this.CheckAndReadWorkItemCommentUpdates(reader);
        this.CheckAndReadWatermark(reader);
        this.result.Success = true;
        return this.result;
      }

      protected virtual void CheckAndReadWorkItemCommentUpdates(IDataReader reader)
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
