// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.Monitoring.DashboardsMonitoringRecordBinder
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess.Monitoring
{
  public class DashboardsMonitoringRecordBinder : ObjectBinder<DashboardsMonitoringRecord>
  {
    private SqlColumnBinder PartitionId = new SqlColumnBinder(nameof (PartitionId));
    private SqlColumnBinder MeasurementName = new SqlColumnBinder(nameof (MeasurementName));
    private SqlColumnBinder MeasurementValue = new SqlColumnBinder(nameof (MeasurementValue));

    protected override DashboardsMonitoringRecord Bind() => new DashboardsMonitoringRecord()
    {
      PartitionId = this.PartitionId.GetInt32((IDataReader) this.Reader),
      MeasurementName = this.MeasurementName.GetString((IDataReader) this.Reader, false),
      MeasurementValue = this.MeasurementValue.GetInt32((IDataReader) this.Reader)
    };
  }
}
