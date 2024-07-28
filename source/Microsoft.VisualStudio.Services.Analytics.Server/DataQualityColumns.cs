// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class DataQualityColumns : ObjectBinder<DataQualityResult>
  {
    private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder RunDateColumn = new SqlColumnBinder("RunDate");
    private SqlColumnBinder StartDateColumn = new SqlColumnBinder("StartDate");
    private SqlColumnBinder EndDateColumn = new SqlColumnBinder("EndDate");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TargetTableColumn = new SqlColumnBinder("TargetTable");
    private SqlColumnBinder ExpectedCountColumn = new SqlColumnBinder("ExpectedCount");
    private SqlColumnBinder ActualCountColumn = new SqlColumnBinder("ActualCount");
    private SqlColumnBinder FailedColumn = new SqlColumnBinder("Failed");

    protected override DataQualityResult Bind() => new DataQualityResult()
    {
      PartitionId = this.PartitionIdColumn.GetInt32((IDataReader) this.Reader),
      RunDate = this.RunDateColumn.GetDateTime((IDataReader) this.Reader),
      StartDate = this.StartDateColumn.GetDateTime((IDataReader) this.Reader),
      EndDate = this.EndDateColumn.GetDateTime((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      TargetTable = this.TargetTableColumn.GetString((IDataReader) this.Reader, false),
      ExpectedValue = this.ExpectedCountColumn.GetInt64((IDataReader) this.Reader),
      ActualValue = this.ActualCountColumn.GetInt64((IDataReader) this.Reader),
      Failed = this.FailedColumn.GetBoolean((IDataReader) this.Reader),
      KpiValue = 0.0
    };
  }
}
