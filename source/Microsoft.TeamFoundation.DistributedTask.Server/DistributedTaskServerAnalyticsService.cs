// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskServerAnalyticsService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class DistributedTaskServerAnalyticsService : 
    IDistributedTaskServerAnalyticsService,
    IVssFrameworkService
  {
    private readonly RegistryQuery s_readAXDataFromReplica = new RegistryQuery("/Service/DistributedTaskServer/Settings/ReadAXDataFromReplica");
    private const string c_layer = "DistributedTaskServerAnalyticsService";

    public void PopulateAgentRequestData(
      IVssRequestContext requestContext,
      List<TaskTimelineRecord> timelineRecords)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      List<AgentRequestData> agentRequestDataList = (List<AgentRequestData>) null;
      using (new MethodScope(requestContext, nameof (DistributedTaskServerAnalyticsService), nameof (PopulateAgentRequestData)))
      {
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          agentRequestDataList = component.GetAgentRequestData(timelineRecords);
      }
      // ISSUE: explicit non-virtual call
      if (agentRequestDataList == null || __nonvirtual (agentRequestDataList.Count) <= 0)
        return;
      Dictionary<string, AgentRequestData> jobToDataMap = new Dictionary<string, AgentRequestData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      agentRequestDataList.ForEach((Action<AgentRequestData>) (ard => jobToDataMap[this.GetJobKey(ard.PlanId, ard.JobId)] = ard));
      timelineRecords.ForEach((Action<TaskTimelineRecord>) (rec =>
      {
        string jobKey = this.GetJobKey(rec.PlanGuidId, rec.TimelineRecordGuid);
        if (!jobToDataMap.ContainsKey(jobKey))
          return;
        AgentRequestData agentRequestData = jobToDataMap[jobKey];
        rec.AgentQueueTime = new DateTime?(agentRequestData.QueueTime);
        rec.AgentStartTime = agentRequestData.StartTime;
      }));
    }

    public List<AgentPoolData> GetAgentPoolsByLastModifiedDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime? fromDate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskServerAnalyticsService), nameof (GetAgentPoolsByLastModifiedDate)))
      {
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.GetAgentPoolsByLastModifiedDate(batchSize, fromDate);
      }
    }

    public List<AgentRequestData> GetAgentRequestDataFromDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime? fromDate)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskServerAnalyticsService), nameof (GetAgentRequestDataFromDate)))
      {
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.GetAgentRequestDataFromDate(batchSize, fromDate);
      }
    }

    public List<TaskAgentPoolSizeData> GetAgentPoolSize(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (new MethodScope(requestContext, nameof (DistributedTaskServerAnalyticsService), nameof (GetAgentPoolSize)))
      {
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>(connectionType: new DatabaseConnectionType?(this.GetDatabaseConnectionType(requestContext))))
          return component.GetAgentPoolSize();
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private string GetJobKey(Guid planId, Guid jobId) => string.Format("{0}-{1}", (object) planId, (object) jobId);

    private DatabaseConnectionType GetDatabaseConnectionType(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in this.s_readAXDataFromReplica, true) ? DatabaseConnectionType.Default : DatabaseConnectionType.IntentReadOnly;
  }
}
