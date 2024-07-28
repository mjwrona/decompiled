// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformNextColumns4
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformNextColumns4 : ObjectBinder<TransformResult>
  {
    private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder CompleteColumn = new SqlColumnBinder("Complete");
    private SqlColumnBinder AlreadyActiveColumn = new SqlColumnBinder("AlreadyActive");
    private SqlColumnBinder LowPriorityDeferredColumn = new SqlColumnBinder("LowPriorityDeferred");
    private SqlColumnBinder LowPriorityDeferredReasonColumn = new SqlColumnBinder("LowPriorityDeferredReason");
    private SqlColumnBinder BatchIdColumn = new SqlColumnBinder("BatchId");
    private SqlColumnBinder AttemptCountColumn = new SqlColumnBinder("AttemptCount");
    private SqlColumnBinder SubBatchCountColumn = new SqlColumnBinder("SubBatchCount");
    private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
    private SqlColumnBinder SprocColumn = new SqlColumnBinder("Sproc");
    private SqlColumnBinder SprocVersionColumn = new SqlColumnBinder("SprocVersion");
    private SqlColumnBinder TriggerTableNameColumn = new SqlColumnBinder("TriggerTableName");
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder TriggerProviderShardIdColumn = new SqlColumnBinder("TriggerProviderShardId");
    private SqlColumnBinder TriggerStreamIdColumn = new SqlColumnBinder("TriggerStreamId");
    private SqlColumnBinder TriggerBatchIdStartColumn = new SqlColumnBinder("TriggerBatchIdStart");
    private SqlColumnBinder TriggerBatchIdEndColumn = new SqlColumnBinder("TriggerBatchIdEnd");
    private SqlColumnBinder StateColumn = new SqlColumnBinder("State");
    private SqlColumnBinder StateDataColumn = new SqlColumnBinder("StateData");
    private SqlColumnBinder EndStateColumn = new SqlColumnBinder("EndState");
    private SqlColumnBinder EndStateDataColumn = new SqlColumnBinder("EndStateData");
    private SqlColumnBinder HeldColumn = new SqlColumnBinder("Held");
    private SqlColumnBinder ReadyColumn = new SqlColumnBinder("Ready");
    private SqlColumnBinder FailedColumn = new SqlColumnBinder("Failed");
    private SqlColumnBinder FailedAttemptColumn = new SqlColumnBinder("FailedAttempt");
    private SqlColumnBinder FailedMessageColumn = new SqlColumnBinder("FailedMessage");
    private SqlColumnBinder InsertedCountColumn = new SqlColumnBinder("InsertedCount");
    private SqlColumnBinder UpdatedCountColumn = new SqlColumnBinder("UpdatedCount");
    private SqlColumnBinder DeletedCountColumn = new SqlColumnBinder("DeletedCount");
    private SqlColumnBinder DurationMSColumn = new SqlColumnBinder("DurationMS");
    private SqlColumnBinder TotalInsertedCountColumn = new SqlColumnBinder("TotalInsertedCount");
    private SqlColumnBinder TotalUpdatedCountColumn = new SqlColumnBinder("TotalUpdatedCount");
    private SqlColumnBinder TotalDeletedCountColumn = new SqlColumnBinder("TotalDeletedCount");
    private SqlColumnBinder TotalDurationMSColumn = new SqlColumnBinder("TotalDurationMS");
    private SqlColumnBinder TotalFailedCountColumn = new SqlColumnBinder("TotalFailedCount");
    private SqlColumnBinder ReworkAttemptCountColumn = new SqlColumnBinder("ReworkAttemptCount");
    private SqlColumnBinder TransformPriorityColumn = new SqlColumnBinder("TransformPriority");

    protected override TransformResult Bind() => new TransformResult()
    {
      PartitionId = this.PartitionIdColumn.GetInt32((IDataReader) this.Reader, 0, 0),
      AllProcessingComplete = this.CompleteColumn.GetBoolean((IDataReader) this.Reader),
      AlreadyActive = this.AlreadyActiveColumn.ColumnExists((IDataReader) this.Reader) && this.AlreadyActiveColumn.GetBoolean((IDataReader) this.Reader),
      Deferred = this.LowPriorityDeferredColumn.ColumnExists((IDataReader) this.Reader) && this.LowPriorityDeferredColumn.GetBoolean((IDataReader) this.Reader),
      DeferredReason = this.LowPriorityDeferredReasonColumn.GetString((IDataReader) this.Reader, (string) null),
      StartTime = this.StartDate.GetNullableDateTime((IDataReader) this.Reader, new DateTime?()),
      BatchId = this.BatchIdColumn.GetNullableInt64((IDataReader) this.Reader),
      SubBatchCount = this.SubBatchCountColumn.GetNullableInt32((IDataReader) this.Reader),
      AttemptCount = this.AttemptCountColumn.ColumnExists((IDataReader) this.Reader) ? this.AttemptCountColumn.GetNullableInt32((IDataReader) this.Reader) : new int?(),
      Sproc = this.SprocColumn.GetString((IDataReader) this.Reader, true),
      SprocVersion = this.SprocVersionColumn.GetNullableInt32((IDataReader) this.Reader),
      TriggerTableName = this.TriggerTableNameColumn.GetString((IDataReader) this.Reader, true),
      TableName = this.TableNameColumn.GetString((IDataReader) this.Reader, true),
      TriggerBatchIdStart = this.TriggerBatchIdStartColumn.GetNullableInt64((IDataReader) this.Reader),
      TriggerBatchIdEnd = this.TriggerBatchIdEndColumn.GetNullableInt64((IDataReader) this.Reader),
      State = this.StateColumn.GetString((IDataReader) this.Reader, true),
      StateData = this.StateDataColumn.GetNullableInt64((IDataReader) this.Reader),
      EndState = this.EndStateColumn.GetString((IDataReader) this.Reader, true),
      EndStateData = this.EndStateDataColumn.GetNullableInt64((IDataReader) this.Reader),
      Held = this.HeldColumn.ColumnExists((IDataReader) this.Reader) ? this.HeldColumn.GetNullableBoolean((IDataReader) this.Reader) : new bool?(),
      Ready = this.ReadyColumn.GetNullableBoolean((IDataReader) this.Reader),
      Failed = this.FailedColumn.GetNullableBoolean((IDataReader) this.Reader),
      FailedAttempt = this.FailedAttemptColumn.ColumnExists((IDataReader) this.Reader) ? this.FailedAttemptColumn.GetNullableBoolean((IDataReader) this.Reader) : new bool?(),
      FailedMessage = this.FailedMessageColumn.GetString((IDataReader) this.Reader, true),
      InsertedCount = this.InsertedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      UpdatedCount = this.UpdatedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      DeletedCount = this.DeletedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      DurationMS = this.DurationMSColumn.GetNullableInt32((IDataReader) this.Reader),
      TotalInsertedCount = this.TotalInsertedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      TotalUpdatedCount = this.TotalUpdatedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      TotalDeletedCount = this.TotalDeletedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      TotalDurationMS = this.TotalDurationMSColumn.GetNullableInt32((IDataReader) this.Reader),
      TotalFailedCount = this.TotalFailedCountColumn.GetNullableInt32((IDataReader) this.Reader),
      ReworkAttemptCount = this.ReworkAttemptCountColumn.ColumnExists((IDataReader) this.Reader) ? this.ReworkAttemptCountColumn.GetNullableInt32((IDataReader) this.Reader) : new int?(),
      Priority = this.TransformPriorityColumn.GetNullableInt32((IDataReader) this.Reader)
    };
  }
}
