// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformBatchColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformBatchColumns : ObjectBinder<TransformBatch>
  {
    private SqlColumnBinder BatchIdColumn = new SqlColumnBinder("BatchId");
    private SqlColumnBinder ReadyColumn = new SqlColumnBinder("Ready");
    private SqlColumnBinder FailedColumn = new SqlColumnBinder("Failed");
    private SqlColumnBinder AttemptCountColumn = new SqlColumnBinder("AttemptCount");
    private SqlColumnBinder TriggerTableNameColumn = new SqlColumnBinder("TriggerTableName");
    private SqlColumnBinder TriggerBatchIdStartColumn = new SqlColumnBinder("TriggerBatchIdStart");
    private SqlColumnBinder TriggerBatchIdEndColumn = new SqlColumnBinder("TriggerBatchIdEnd");
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder SprocColumn = new SqlColumnBinder("Sproc");
    private SqlColumnBinder SprocVersionColumn = new SqlColumnBinder("SprocVersion");
    private SqlColumnBinder TransformPriorityColumn = new SqlColumnBinder("TransformPriority");
    private SqlColumnBinder StateColumn = new SqlColumnBinder("State");
    private SqlColumnBinder StateDataColumn = new SqlColumnBinder("StateData");
    private SqlColumnBinder CreateDateTimeColumn = new SqlColumnBinder("CreateDateTime");
    private SqlColumnBinder HeldColumn = new SqlColumnBinder("Held");
    private SqlColumnBinder LowPriorityDeferredColumn = new SqlColumnBinder("LowPriorityDeferred");
    private SqlColumnBinder TargetActiveBlockedColumn = new SqlColumnBinder("TargetActiveBlocked");

    protected override TransformBatch Bind() => new TransformBatch()
    {
      BatchId = this.BatchIdColumn.GetInt64((IDataReader) this.Reader),
      Ready = this.ReadyColumn.GetBoolean((IDataReader) this.Reader),
      Failed = this.FailedColumn.GetBoolean((IDataReader) this.Reader),
      Active = this.FailedColumn.GetBoolean((IDataReader) this.Reader),
      AttemptCount = this.AttemptCountColumn.GetInt32((IDataReader) this.Reader, 0),
      Sproc = this.SprocColumn.GetString((IDataReader) this.Reader, true),
      SprocVersion = this.SprocVersionColumn.GetInt32((IDataReader) this.Reader, 0),
      TransformPriority = this.TransformPriorityColumn.GetInt32((IDataReader) this.Reader),
      TriggerTableName = this.TriggerTableNameColumn.GetString((IDataReader) this.Reader, true),
      TableName = this.TableNameColumn.GetString((IDataReader) this.Reader, true),
      TriggerBatchIdStart = this.TriggerBatchIdStartColumn.GetInt64((IDataReader) this.Reader, -1L),
      TriggerBatchIdEnd = this.TriggerBatchIdEndColumn.GetInt64((IDataReader) this.Reader, -1L),
      State = this.StateColumn.GetString((IDataReader) this.Reader, true),
      StateData = this.StateDataColumn.IsNull((IDataReader) this.Reader) ? new long?() : new long?(this.StateDataColumn.GetInt64((IDataReader) this.Reader)),
      Held = this.HeldColumn.GetBoolean((IDataReader) this.Reader, false),
      CreateDateTime = this.CreateDateTimeColumn.GetDateTime((IDataReader) this.Reader),
      LowPriorityDeferred = this.LowPriorityDeferredColumn.GetBoolean((IDataReader) this.Reader, false, false),
      TargetActiveBlocked = this.TargetActiveBlockedColumn.GetBoolean((IDataReader) this.Reader, false, false)
    };
  }
}
