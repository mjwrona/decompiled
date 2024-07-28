// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent31
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent31 : WorkItemComponent30
  {
    protected virtual SqlParameter BindWorkItemCommentUpdates(
      string parameterName,
      IEnumerable<WorkItemCommentUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCommentUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCommentUpdateRecord>) new WorkItemComponent31.WorkItemCommentUpdateRecordBinder(), parameterName, rows);
    }

    protected virtual SqlParameter BindWorkItemMentionUpdates(
      string parameterName,
      IEnumerable<WorkItemMentionRecord> rows)
    {
      return this.BindBasicTvp<WorkItemMentionRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemMentionRecord>) new WorkItemComponent31.WorkItemMentionRecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    public override string RemoveDeletedProcesses(Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_RemoveDeletedProcesses");
      this.BindGuid("@changedBy", changedBy);
      string str = (string) this.ExecuteScalar();
      return str == null ? "No results returned from procedure" : str;
    }

    protected class WorkItemCommentUpdateRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCommentUpdateRecord>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[10]
      {
        new SqlMetaData("TempId", SqlDbType.Int),
        new SqlMetaData("CommentId", SqlDbType.Int),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
        new SqlMetaData("ArtifactKind", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Text", SqlDbType.NVarChar, -1L),
        new SqlMetaData("RenderedText", SqlDbType.NVarChar, -1L),
        new SqlMetaData("Format", SqlDbType.TinyInt),
        new SqlMetaData("IsDeleted", SqlDbType.Bit),
        new SqlMetaData("ChangedBy", SqlDbType.NVarChar, 256L),
        new SqlMetaData("ChangedDate", SqlDbType.DateTime)
      };

      public override string TypeName => "typ_WorkItemCommentUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent31.WorkItemCommentUpdateRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCommentUpdateRecord update)
      {
        record.SetInt32(0, update.TempId);
        record.SetInt32(1, update.CommentId);
        record.SetString(2, update.ArtifactId, BindStringBehavior.Unchanged);
        record.SetGuid(3, update.ArtifactKind);
        record.SetString(4, update.Text, BindStringBehavior.Unchanged);
        record.SetString(5, update.RenderedText, BindStringBehavior.EmptyStringToNull);
        record.SetNullableByte(6, update.Format.HasValue ? new byte?((byte) update.Format.Value) : new byte?());
        record.SetBoolean(7, update.IsDeleted);
        record.SetString(8, update.CreatedOnBehalfOf, BindStringBehavior.Unchanged);
        record.SetNullableDateTime(9, update.CreatedOnBehalfDate);
      }
    }

    protected class WorkItemMentionRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemMentionRecord>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[13]
      {
        new SqlMetaData("SourceId", SqlDbType.NVarChar, 200L),
        new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
        new SqlMetaData("SourceType", SqlDbType.NVarChar, 200L),
        new SqlMetaData("RawText", SqlDbType.NVarChar, 200L),
        new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 200L),
        new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 200L),
        new SqlMetaData("CommentId", SqlDbType.Int),
        new SqlMetaData("TempId", SqlDbType.Int),
        new SqlMetaData("MentionAction", SqlDbType.NVarChar, 200L),
        new SqlMetaData("TargetId", SqlDbType.NVarChar, 200L),
        new SqlMetaData("StorageKey", SqlDbType.UniqueIdentifier),
        new SqlMetaData("TargetDataspaceId", SqlDbType.Int),
        new SqlMetaData("NormalizedSourceId", SqlDbType.NVarChar, 200L)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemCommentMentionTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent31.WorkItemMentionRecordBinder.s_metadata;

      public WorkItemMentionRecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemMentionRecord update)
      {
        record.SetString(0, update.SourceId);
        record.SetInt32(1, this.m_dataspaceResolver(update.SourceProjectGuid));
        record.SetString(2, update.SourceType);
        record.SetString(3, update.RawText);
        record.SetString(4, update.ArtifactId);
        record.SetString(5, update.ArtifactType);
        record.SetInt32(6, update.CommentId);
        record.SetInt32(7, update.TempId);
        record.SetString(8, update.MentionAction);
        record.SetString(9, update.TargetId);
        record.SetNullableGuid(10, update.StorageKey);
        record.SetInt32(11, this.m_dataspaceResolver(update.ProjectGuid));
        record.SetString(12, update.NormalizedSourceId);
      }
    }
  }
}
