// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDistributedTaskServerAnalyticsService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskServerAnalyticsService))]
  public interface IDistributedTaskServerAnalyticsService : IVssFrameworkService
  {
    void PopulateAgentRequestData(
      IVssRequestContext requestContext,
      List<TaskTimelineRecord> timelineRecords);

    List<AgentPoolData> GetAgentPoolsByLastModifiedDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime? fromDate);

    List<AgentRequestData> GetAgentRequestDataFromDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime? fromDate);

    List<TaskAgentPoolSizeData> GetAgentPoolSize(IVssRequestContext requestContext);
  }
}
