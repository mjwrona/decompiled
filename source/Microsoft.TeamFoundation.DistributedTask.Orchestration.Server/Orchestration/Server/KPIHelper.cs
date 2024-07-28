// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.KPIHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class KPIHelper
  {
    private const double DTASuccessState = 0.0;
    private const double DTAFailureState = 1.0;
    private const string TaskRecordTypeName = "Task";
    private const string TaskNameKey = "TaskName";
    private const string DtaAreaName = "DTA";
    private const string VstestAreaName = "VSTest";
    private const string DtaTaskstatusMetricName = "DTATaskStatus";
    private const string DtaTaskStatusMetricDescription = "Indicates the status of DTA tasks executed";

    internal static void PublishDTAReliabilityMetric(
      IVssRequestContext requestContext,
      Timeline timelines)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        KpiService service = vssRequestContext.GetService<KpiService>();
        service.EnsureKpiIsRegistered(vssRequestContext, "DTA", "DTATaskStatus", "DTA", "DTATaskStatus", "Indicates the status of DTA tasks executed");
        service.EnsureKpiIsRegistered(vssRequestContext, "VSTest", "DTATaskStatus", "DTA", "DTATaskStatus", "Indicates the status of DTA tasks executed");
        foreach (TimelineRecord timelineRecord in timelines.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (timeline => string.Equals(timeline.RecordType, "Task", StringComparison.OrdinalIgnoreCase))))
        {
          bool flag = false;
          if (timelineRecord.Issues.Any<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Error && q.Data.ContainsKey("TaskName") && "DTA".Equals(q.Data["TaskName"], StringComparison.OrdinalIgnoreCase))))
          {
            flag = true;
            KpiMetric metric = new KpiMetric("DTATaskStatus", 1.0);
            service.Publish(vssRequestContext, "DTA", metric);
          }
          if (timelineRecord.Issues.Any<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Error && q.Data.ContainsKey("TaskName") && "VSTest".Equals(q.Data["TaskName"], StringComparison.OrdinalIgnoreCase))))
          {
            flag = true;
            KpiMetric metric = new KpiMetric("DTATaskStatus", 1.0);
            service.Publish(vssRequestContext, "VSTest", metric);
          }
          if (!flag && timelineRecord.Issues.Any<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Warning && q.Data.ContainsKey("TaskName") && "DTA".Equals(q.Data["TaskName"], StringComparison.OrdinalIgnoreCase))))
          {
            KpiMetric metric = new KpiMetric("DTATaskStatus", 0.0);
            service.Publish(vssRequestContext, "DTA", metric);
          }
          if (!flag && timelineRecord.Issues.Any<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Warning && q.Data.ContainsKey("TaskName") && "VSTest".Equals(q.Data["TaskName"], StringComparison.OrdinalIgnoreCase))))
          {
            KpiMetric metric = new KpiMetric("DTATaskStatus", 0.0);
            service.Publish(vssRequestContext, "VSTest", metric);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (KPIHelper), ex);
      }
    }

    internal static void PublishDTPlanQueued(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTPlanQueued", "Distributed Task Plan Queued", "Tracks when a Distributed Task Plan is created and queued");

    internal static void PublishDTPlanStarted(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTPlanStarted", "Distributed Task Plan Started", "Tracks when a Distributed Task Plan starts");

    internal static void PublishDTPlanCompleted(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTPlanCompleted", "Distributed Task Plan Completed", "Tracks when a Distributed Task Plan completes");

    internal static void PublishDTAgentRequestSent(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTAgentRequestSent", "Distributed Task Agent Reqeust Sent", "Tracks when Distributed Task SDK sends a request for a job agent");

    internal static void PublishDTAgentRequestReceived(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTAgentRequestReceived", "Distributed Task Agent Reqeust Received", "Tracks when Distributed Task Service receives a request for a job agent");

    internal static void PublishDTJobStarted(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTJobStarted", "Distributed Task Job Started", "Tracks when a Distributed Task Job starts");

    internal static void PublishDTJobCompleted(IVssRequestContext requestContext) => KPIHelper.PublishKpiInternal(requestContext, "DTJobCompleted", "Distributed Task Job Completed", "Tracks when Distributed Task Job completes");

    private static void PublishKpiInternal(
      IVssRequestContext requestContext,
      string kpiName,
      string kpiDisplayName,
      string kpiDescription)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IKpiService service = vssRequestContext.GetService<IKpiService>();
        service.EnsureKpiIsRegistered(vssRequestContext, "Microsoft.TeamFoundation.DistributedTask", kpiName, requestContext.ServiceHost.InstanceId.ToString(), kpiDisplayName, kpiDescription);
        KpiMetric metric = new KpiMetric(kpiName, 1.0);
        service.Publish(vssRequestContext, "Microsoft.TeamFoundation.DistributedTask", requestContext.ServiceHost.InstanceId, requestContext.ServiceHost.InstanceId.ToString(), metric);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (KPIHelper), ex);
      }
    }
  }
}
