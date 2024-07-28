// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ScaleUnitDataTransferColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ScaleUnitDataTransferColumns : ObjectBinder<ScaleUnitDataTransfer>
  {
    private SqlColumnBinder sourceStorageAccountColumn = new SqlColumnBinder("SourceStorageAccount");
    private SqlColumnBinder targetStorageAccountColumn = new SqlColumnBinder("TargetStorageAccount");
    private SqlColumnBinder primaryIdColumn = new SqlColumnBinder("PrimaryId");
    private SqlColumnBinder subsetIdColumn = new SqlColumnBinder("SubsetId");
    private SqlColumnBinder itemTypeColumn = new SqlColumnBinder("ItemType");
    private SqlColumnBinder itemIdColumn = new SqlColumnBinder("ItemId");
    private SqlColumnBinder totalEntriesTransferredColumn = new SqlColumnBinder("TotalEntriesTransferred");
    private SqlColumnBinder startedProcessingColumn = new SqlColumnBinder("StartedProcessing");
    private SqlColumnBinder jobThreadColumn = new SqlColumnBinder("JobThread");
    private SqlColumnBinder completedProcessingColumn = new SqlColumnBinder("CompletedProcessing");
    private SqlColumnBinder heartbeatColumn = new SqlColumnBinder("Heartbeat");

    protected override ScaleUnitDataTransfer Bind() => new ScaleUnitDataTransfer()
    {
      SourceStorageAccount = this.sourceStorageAccountColumn.GetString((IDataReader) this.Reader, false),
      TargetStorageAccount = this.targetStorageAccountColumn.GetString((IDataReader) this.Reader, false),
      PrimaryId = this.primaryIdColumn.GetString((IDataReader) this.Reader, false),
      SubsetId = this.subsetIdColumn.GetString((IDataReader) this.Reader, false),
      ItemType = this.itemTypeColumn.GetString((IDataReader) this.Reader, true),
      ItemId = this.itemIdColumn.GetString((IDataReader) this.Reader, true),
      TotalEntriesTransferred = this.totalEntriesTransferredColumn.GetNullableInt64((IDataReader) this.Reader),
      StartedProcessing = this.startedProcessingColumn.GetNullableDateTime((IDataReader) this.Reader),
      JobThread = this.jobThreadColumn.GetNullableGuid((IDataReader) this.Reader),
      CompletedProcessing = this.completedProcessingColumn.GetBoolean((IDataReader) this.Reader),
      Heartbeat = this.heartbeatColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
