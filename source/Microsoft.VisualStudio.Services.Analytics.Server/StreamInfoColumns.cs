// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StreamInfoColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StreamInfoColumns : ObjectBinder<StreamInfo>
  {
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder ProviderShardIdColumn = new SqlColumnBinder("ProviderShardId");
    private SqlColumnBinder StreamIdColumn = new SqlColumnBinder("StreamId");
    private SqlColumnBinder EnabledColumn = new SqlColumnBinder("Enabled");
    private SqlColumnBinder PriorityColumn = new SqlColumnBinder("Priority");
    private SqlColumnBinder CurrentColumn = new SqlColumnBinder("Current");
    private SqlColumnBinder WatermarkColumn = new SqlColumnBinder("Watermark");
    private SqlColumnBinder MaintenanceColumn = new SqlColumnBinder("Maintenance");
    private SqlColumnBinder MaintenanceReasonColumn = new SqlColumnBinder("MaintenanceReason");
    private SqlColumnBinder MaintenceChangedDateColumn = new SqlColumnBinder("MaintenanceChangedTime");
    private SqlColumnBinder DisposedColumn = new SqlColumnBinder("Disposed");
    private SqlColumnBinder CreateTimeColumn = new SqlColumnBinder("CreateTime");
    private SqlColumnBinder LoadedTimeColumn = new SqlColumnBinder("LoadedTime");
    private SqlColumnBinder InitialContentVersionColumn = new SqlColumnBinder("InitialContentVersion");
    private SqlColumnBinder LatestContentVersionColumn = new SqlColumnBinder("LatestContentVersion");
    private SqlColumnBinder KeysOnlyColumn = new SqlColumnBinder("KeysOnly");

    protected override StreamInfo Bind() => new StreamInfo()
    {
      TableName = this.TableNameColumn.GetString((IDataReader) this.Reader, false),
      ProviderShardId = this.ProviderShardIdColumn.GetInt32((IDataReader) this.Reader),
      StreamId = this.StreamIdColumn.GetInt32((IDataReader) this.Reader),
      Enabled = this.EnabledColumn.GetBoolean((IDataReader) this.Reader),
      Priority = this.PriorityColumn.GetInt32((IDataReader) this.Reader),
      Current = this.CurrentColumn.GetBoolean((IDataReader) this.Reader),
      Maintenance = this.MaintenanceColumn.ColumnExists((IDataReader) this.Reader) && this.MaintenanceColumn.GetBoolean((IDataReader) this.Reader),
      MaintenanceReason = this.MaintenanceReasonColumn.ColumnExists((IDataReader) this.Reader) ? this.MaintenanceReasonColumn.GetString((IDataReader) this.Reader, true) : (string) null,
      MaintenceChangedDate = this.MaintenceChangedDateColumn.ColumnExists((IDataReader) this.Reader) ? this.MaintenceChangedDateColumn.GetNullableDateTime((IDataReader) this.Reader) : new DateTime?(),
      Watermark = this.WatermarkColumn.GetString((IDataReader) this.Reader, true),
      Disposed = this.DisposedColumn.ColumnExists((IDataReader) this.Reader) ? this.DisposedColumn.GetBoolean((IDataReader) this.Reader, !this.EnabledColumn.GetBoolean((IDataReader) this.Reader)) : !this.EnabledColumn.GetBoolean((IDataReader) this.Reader),
      CreateTime = this.CreateTimeColumn.ColumnExists((IDataReader) this.Reader) ? this.CreateTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue) : DateTime.MinValue,
      LoadedTime = !this.LoadedTimeColumn.ColumnExists((IDataReader) this.Reader) || this.LoadedTimeColumn.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.LoadedTimeColumn.GetDateTime((IDataReader) this.Reader)),
      InitialContentVersion = !this.InitialContentVersionColumn.ColumnExists((IDataReader) this.Reader) || this.InitialContentVersionColumn.IsNull((IDataReader) this.Reader) ? new int?() : new int?(this.InitialContentVersionColumn.GetInt32((IDataReader) this.Reader)),
      LatestContentVersion = !this.LatestContentVersionColumn.ColumnExists((IDataReader) this.Reader) || this.LatestContentVersionColumn.IsNull((IDataReader) this.Reader) ? new int?() : new int?(this.LatestContentVersionColumn.GetInt32((IDataReader) this.Reader)),
      KeysOnly = this.KeysOnlyColumn.ColumnExists((IDataReader) this.Reader) && this.KeysOnlyColumn.GetBoolean((IDataReader) this.Reader)
    };
  }
}
