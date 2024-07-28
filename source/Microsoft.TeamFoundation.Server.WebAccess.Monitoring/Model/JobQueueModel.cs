// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Model.JobQueueModel
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
  public class JobQueueModel
  {
    public JobQueueModel(
      IVssRequestContext requestContext,
      TeamFoundationJobReportingQueuePositions entry)
    {
      this.QueuePosition = entry.QueuePosition;
      this.StateTime = new int?(entry.StateTime);
      this.Priority = entry.Priority;
      this.JobId = entry.JobId;
      this.HostId = entry.HostId;
      this.JobName = requestContext.GetService<TeamFoundationJobReportingService>().GetJobName(requestContext, entry.JobId);
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(requestContext, entry.HostId);
      this.HostName = serviceHostProperties.HostType != TeamFoundationHostType.ProjectCollection ? serviceHostProperties.Name : string.Format("[{0}]{1}", (object) service.QueryServiceHostProperties(requestContext, serviceHostProperties.ParentId).Name, (object) serviceHostProperties.Name);
      switch (this.QueuePosition)
      {
        case 1:
          this.DisplayTime = new TimeSpan(0, 0, this.StateTime.Value).ToString();
          this.QueuePositionName = "In Progress";
          break;
        case 2:
          this.DisplayTime = new TimeSpan(0, 0, this.StateTime.Value).ToString();
          this.QueuePositionName = "Queued";
          break;
        case 3:
          this.DisplayTime = new TimeSpan(0, 0, this.StateTime.Value).ToString();
          this.QueuePositionName = "Scheduled";
          break;
        case 4:
          this.DisplayTime = MonitoringServerResources.PositionDisplayHostOffline;
          this.QueuePositionName = "Host Offline";
          break;
        case 5:
          this.DisplayTime = MonitoringServerResources.PositionDisplayHostDormant;
          this.QueuePositionName = "Host Dormant";
          break;
        default:
          this.DisplayTime = MonitoringServerResources.PositionDisplayUnknown;
          break;
      }
    }

    [DataMember(Name = "queuePosition")]
    public int QueuePosition { get; set; }

    [DataMember(Name = "queuePositionName")]
    public string QueuePositionName { get; set; }

    [DataMember(Name = "stateTime")]
    public int? StateTime { get; set; }

    [DataMember(Name = "priority")]
    public int Priority { get; set; }

    [DataMember(Name = "jobId")]
    public Guid JobId { get; set; }

    [DataMember(Name = "hostId")]
    public Guid HostId { get; set; }

    [DataMember(Name = "jobName")]
    public string JobName { get; set; }

    [DataMember(Name = "hostName")]
    public string HostName { get; set; }

    [DataMember(Name = "displayTime")]
    public string DisplayTime { get; set; }
  }
}
