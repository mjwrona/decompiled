// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.HealthStatusDataAccess
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
  internal class HealthStatusDataAccess : SqlAzureDataAccess, IHealthStatusDataAccess
  {
    public HealthStatusDataAccess()
    {
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public void AddHealthStatusEntry(IVssRequestContext requestContext, HealthStatusRecord record)
    {
      this.ValidateNotNull<HealthStatusRecord>(nameof (record), record);
      using (HealthStatusComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusComponent>())
        component.Insert(record);
    }

    public virtual IEnumerable<HealthStatusRecord> GetHealthStatusJobDataRecords(
      IVssRequestContext requestContext,
      int count,
      Guid collectionId,
      string jobName,
      JobStatus status)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      if (string.IsNullOrWhiteSpace(jobName))
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("JobName cannot be null or empty."));
      using (HealthStatusComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusComponent>())
        return component is HealthStatusComponentV2 statusComponentV2 ? (IEnumerable<HealthStatusRecord>) statusComponentV2.GetHealthStatusJobDataRecords(count, collectionId, jobName, status) : (IEnumerable<HealthStatusRecord>) component.GetHealthStatusJobDataRecords(count, collectionId, jobName, status);
    }

    public virtual IEnumerable<HealthStatusRecord> GetHealthStatusRecordsWithStatus(
      IVssRequestContext requestContext,
      int count,
      Guid collectionId,
      string jobName,
      List<JobStatus> statusList)
    {
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      if (string.IsNullOrWhiteSpace(jobName))
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("JobName cannot be null or empty."));
      this.ValidateNotNullOrEmptyList<JobStatus>(nameof (statusList), (IList<JobStatus>) statusList);
      List<HealthStatusRecord> recordsWithStatus = new List<HealthStatusRecord>();
      using (HealthStatusComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusComponent>())
      {
        if (component is HealthStatusComponentV2 statusComponentV2)
          recordsWithStatus = statusComponentV2.GetHealthStatusRecordsWithStatus(count, collectionId, jobName, statusList);
      }
      return (IEnumerable<HealthStatusRecord>) recordsWithStatus;
    }

    public virtual void UpdateHealthStatusEntry(
      IVssRequestContext requestContext,
      int id,
      JobStatus jobStatus)
    {
      using (HealthStatusComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<HealthStatusComponent>())
        component.UpdateStatusInHealthStatusTable(id, jobStatus);
    }
  }
}
