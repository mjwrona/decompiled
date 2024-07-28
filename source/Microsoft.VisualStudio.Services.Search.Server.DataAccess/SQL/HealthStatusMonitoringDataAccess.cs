// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.HealthStatusMonitoringDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class HealthStatusMonitoringDataAccess : 
    SqlAzureDataAccess,
    IHealthStatusMonitoringDataAccess
  {
    public HealthStatusMonitoringDataAccess()
    {
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusMonitoringDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public void AddHealthStatusMonitoringEntry(
      IVssRequestContext requestContext,
      HealthStatusMonitoringRecord record)
    {
      this.ValidateNotNull<HealthStatusMonitoringRecord>(nameof (record), record);
      using (HealthStatusMonitoringComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusMonitoringComponent>())
        component.Insert(record);
    }

    public virtual IEnumerable<HealthStatusMonitoringRecord> GetHealthStatusJobMonitoringDataRecords(
      IVssRequestContext requestContext,
      int count,
      Guid hostId,
      Guid jobId,
      JobStatus status)
    {
      this.ValidateNotEmptyGuid(nameof (hostId), hostId);
      this.ValidateNotEmptyGuid(nameof (jobId), jobId);
      using (HealthStatusMonitoringComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusMonitoringComponent>())
        return (IEnumerable<HealthStatusMonitoringRecord>) component.GetHealthStatusMonitoringDataRecords(count, hostId, jobId, status);
    }

    public virtual IEnumerable<HealthStatusMonitoringRecord> GetHealthStatusMonitoringRecordsWithStatus(
      IVssRequestContext requestContext,
      int count,
      Guid hostId,
      Guid jobId,
      List<JobStatus> statusList)
    {
      this.ValidateNotEmptyGuid(nameof (hostId), hostId);
      this.ValidateNotEmptyGuid(nameof (jobId), jobId);
      this.ValidateNotNullOrEmptyList<JobStatus>(nameof (statusList), (IList<JobStatus>) statusList);
      using (HealthStatusMonitoringComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusMonitoringComponent>())
        return (IEnumerable<HealthStatusMonitoringRecord>) component.GetHealthStatusRecordsWithStatus(count, hostId, jobId, statusList);
    }

    public virtual void UpdateHealthStatusEntry(
      IVssRequestContext requestContext,
      int id,
      JobStatus jobStatus)
    {
      using (HealthStatusMonitoringComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusMonitoringComponent>())
        component.UpdateStatusInHealthStatusTable(id, jobStatus);
    }
  }
}
