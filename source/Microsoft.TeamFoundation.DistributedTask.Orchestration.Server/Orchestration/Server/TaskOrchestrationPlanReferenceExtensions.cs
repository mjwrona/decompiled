// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskOrchestrationPlanReferenceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TaskOrchestrationPlanReferenceExtensions
  {
    public static string GetOrchestrationId(this TaskOrchestrationPlanReference plan) => plan.ProcessType == OrchestrationProcessType.Container ? plan.PlanId.ToString("N") : plan.PlanId.ToString("D");

    public static string GetJobOrchestrationId(this TaskOrchestrationPlanReference plan, Guid jobId) => plan.ProcessType == OrchestrationProcessType.Container ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) plan.PlanId, (object) jobId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:D}_{1:D}", (object) plan.PlanId, (object) jobId);

    public static string GetJobOrchestrationId(
      this TaskOrchestrationPlanReference plan,
      TimelineRecord job)
    {
      return string.Format("{0:D}.{1}", (object) plan.PlanId, (object) PipelineUtilities.GetJobInstanceName(job).ToLowerInvariant());
    }

    public static string GetServerTaskOrchestrationId(
      this TaskOrchestrationPlanReference plan,
      TimelineRecord job,
      TimelineRecord task)
    {
      return string.Format("{0:D}.{1}", (object) plan.PlanId, (object) PipelineUtilities.GetTaskInstanceName(job, task).ToLowerInvariant());
    }

    public static string GetServerTaskOrchestrationId(
      this TaskOrchestrationPlanReference plan,
      Guid jobId,
      Guid taskId)
    {
      return plan.ProcessType == OrchestrationProcessType.Container ? ServerTaskExtensions.GetServerTaskOrchestrationId(plan.PlanId, jobId, taskId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:D}_{1:D}_{2:D}", (object) plan.PlanId, (object) jobId, (object) taskId);
    }
  }
}
