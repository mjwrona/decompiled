// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model.JobHistoryModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model
{
  [DataContract]
  public class JobHistoryModel
  {
    public JobHistoryModel(IVssRequestContext requestContext, TeamFoundationJobReportingHistory job)
    {
      this.HistoryId = job.HistoryId;
      this.JobSource = job.JobSource;
      this.QueueTime = job.QueueTime;
      this.StartTime = job.StartTime;
      this.JobId = job.JobId;
      this.EndTime = job.EndTime;
      this.AgentId = job.AgentId;
      this.Result = (byte) job.Result;
      this.ResultMessage = job.ResultMessage;
      this.QueuedReasons = (int) job.QueuedReasons;
      this.QueuedFlags = job.QueueFlags;
      this.Priority = job.Priority;
      this.QueueDuration = job.QueueDuration.ToString("g");
      this.RunDuration = job.RunDuration.ToString("g");
      this.ResultString = job.ResultString;
      this.QueuedReasonsString = job.QueueReasonsString;
      TeamFoundationJobReportingService service1 = requestContext.GetService<TeamFoundationJobReportingService>();
      this.JobName = service1.GetJobName(requestContext, this.JobId);
      TeamFoundationHostManagementService service2 = requestContext.GetService<TeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties = service2.QueryServiceHostProperties(requestContext, this.JobSource);
      this.HostName = serviceHostProperties.HostType != TeamFoundationHostType.ProjectCollection ? serviceHostProperties.Name : string.Format("[{0}]{1}", (object) service2.QueryServiceHostProperties(requestContext, serviceHostProperties.ParentId).Name, (object) serviceHostProperties.Name);
      TeamFoundationServiceHostProcess agentInformation = service1.GetAgentInformation(requestContext, job.AgentId);
      if (agentInformation == null)
        this.AgentName = MonitoringServerResources.AgentNameNoProcess;
      else
        this.AgentName = string.Format(MonitoringServerResources.AgentNameProcessExists, (object) agentInformation.MachineName, (object) agentInformation.ProcessName);
    }

    [DataMember(Name = "historyId")]
    public long HistoryId { get; set; }

    [DataMember(Name = "jobSource")]
    public Guid JobSource { get; set; }

    [DataMember(Name = "jobId")]
    public Guid JobId { get; set; }

    [DataMember(Name = "queueTime")]
    public DateTime QueueTime { get; set; }

    [DataMember(Name = "startTime")]
    public DateTime StartTime { get; set; }

    [DataMember(Name = "endTime")]
    public DateTime EndTime { get; set; }

    [DataMember(Name = "agentId")]
    public Guid AgentId { get; set; }

    [DataMember(Name = "result")]
    public byte Result { get; set; }

    [DataMember(Name = "resultMessage")]
    public string ResultMessage { get; set; }

    [DataMember(Name = "queuedReasons")]
    public int QueuedReasons { get; set; }

    [DataMember(Name = "queueFlags")]
    public int QueuedFlags { get; set; }

    [DataMember(Name = "priority")]
    public int Priority { get; set; }

    [DataMember(Name = "queueDuration")]
    public string QueueDuration { get; set; }

    [DataMember(Name = "runDuration")]
    public string RunDuration { get; set; }

    [DataMember(Name = "jobName")]
    public string JobName { get; set; }

    [DataMember(Name = "hostName")]
    public string HostName { get; set; }

    [DataMember(Name = "resultString")]
    public string ResultString { get; set; }

    [DataMember(Name = "queuedReasonsString")]
    public string QueuedReasonsString { get; set; }

    [DataMember(Name = "passRates")]
    public string PassRates { get; set; }

    [DataMember(Name = "agentName")]
    public string AgentName { get; set; }
  }
}
