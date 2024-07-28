// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.DashboardMonitoringSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.DataAccess.Monitoring;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  public class DashboardMonitoringSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<DashboardMonitoringSqlResourceComponent>(1)
    }, "DashboardMonitoring");

    public DashboardMonitoringSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public virtual List<DashboardsMonitoringRecord> GetSummaryStats()
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetSummaryStats");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardsMonitoringRecord>((ObjectBinder<DashboardsMonitoringRecord>) new DashboardsMonitoringRecordBinder());
      return resultCollection.GetCurrent<DashboardsMonitoringRecord>().Items;
    }

    public virtual List<DashboardsMonitoringRecord> GetWidgetPopularity()
    {
      this.PrepareStoredProcedure("Dashboards.prc_GetWidgetPopularity");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<DashboardsMonitoringRecord>((ObjectBinder<DashboardsMonitoringRecord>) new DashboardsMonitoringRecordBinder());
      return resultCollection.GetCurrent<DashboardsMonitoringRecord>().Items;
    }
  }
}
