// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityColumns2
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class DataQualityColumns2 : ObjectBinder<DataQualityResult>
  {
    private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder RunDateColumn = new SqlColumnBinder("RunDate");
    private SqlColumnBinder RunEndDateColumn = new SqlColumnBinder("RunEndDate");
    private SqlColumnBinder StartDateColumn = new SqlColumnBinder("StartDate");
    private SqlColumnBinder EndDateColumn = new SqlColumnBinder("EndDate");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder TargetTableColumn = new SqlColumnBinder("TargetTable");
    private SqlColumnBinder ScopeColumn = new SqlColumnBinder("Scope");
    private SqlColumnBinder ExpectedValueColumn = new SqlColumnBinder("ExpectedValue");
    private SqlColumnBinder ActualValueColumn = new SqlColumnBinder("ActualValue");
    private SqlColumnBinder KpiValueColumn = new SqlColumnBinder("KpiValue");
    private SqlColumnBinder FailedColumn = new SqlColumnBinder("Failed");

    protected override DataQualityResult Bind() => new DataQualityResult()
    {
      PartitionId = this.PartitionIdColumn.GetInt32((IDataReader) this.Reader),
      RunDate = this.RunDateColumn.GetDateTime((IDataReader) this.Reader),
      RunEndDate = this.RunEndDateColumn.GetDateTime((IDataReader) this.Reader),
      StartDate = this.StartDateColumn.GetDateTime((IDataReader) this.Reader),
      EndDate = this.EndDateColumn.GetDateTime((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Scope = this.ScopeColumn.ColumnExists((IDataReader) this.Reader) ? this.ScopeColumn.GetString((IDataReader) this.Reader, true) : (string) null,
      TargetTable = this.TargetTableColumn.GetString((IDataReader) this.Reader, false),
      ExpectedValue = this.ExpectedValueColumn.GetInt64((IDataReader) this.Reader, 0L),
      ActualValue = this.ActualValueColumn.GetInt64((IDataReader) this.Reader, 0L),
      KpiValue = this.KpiValueColumn.GetDouble((IDataReader) this.Reader, 0.0),
      Failed = this.FailedColumn.GetBoolean((IDataReader) this.Reader, false)
    };
  }
}
