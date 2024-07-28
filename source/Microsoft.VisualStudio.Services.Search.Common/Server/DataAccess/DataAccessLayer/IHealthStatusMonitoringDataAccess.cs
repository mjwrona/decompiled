// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IHealthStatusMonitoringDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IHealthStatusMonitoringDataAccess
  {
    void AddHealthStatusMonitoringEntry(
      IVssRequestContext requestContext,
      HealthStatusMonitoringRecord record);

    IEnumerable<HealthStatusMonitoringRecord> GetHealthStatusJobMonitoringDataRecords(
      IVssRequestContext requestContext,
      int count,
      Guid hostId,
      Guid jobId,
      JobStatus status);

    IEnumerable<HealthStatusMonitoringRecord> GetHealthStatusMonitoringRecordsWithStatus(
      IVssRequestContext requestContext,
      int count,
      Guid hostId,
      Guid jobId,
      List<JobStatus> statusList);

    void UpdateHealthStatusEntry(IVssRequestContext requestContext, int id, JobStatus jobStatus);
  }
}
