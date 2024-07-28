// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TaskHubExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class TaskHubExtensions
  {
    public static IEnumerable<Issue> GetIssues(
      this TaskHub taskHub,
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      int errorLimit,
      out int errorCount,
      out int warningCount)
    {
      errorCount = 0;
      warningCount = 0;
      if (taskHub == null || plan == null)
        return Enumerable.Empty<Issue>();
      List<Issue> issues = new List<Issue>();
      int num = 0;
      Timeline timeline = taskHub.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true);
      if (timeline != null)
      {
        foreach (TimelineRecord timelineRecord in timeline.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (tr => tr.ParentId.HasValue && tr.ErrorCount.GetValueOrDefault() + tr.WarningCount.GetValueOrDefault() > 0)))
        {
          ref int local1 = ref errorCount;
          int? nullable = timelineRecord.ErrorCount;
          int valueOrDefault1 = nullable.GetValueOrDefault();
          local1 = valueOrDefault1;
          ref int local2 = ref warningCount;
          nullable = timelineRecord.WarningCount;
          int valueOrDefault2 = nullable.GetValueOrDefault();
          local2 = valueOrDefault2;
          if (num < errorLimit)
          {
            foreach (Issue issue in timelineRecord.Issues)
            {
              if (num < errorLimit && issue.Type == IssueType.Error)
              {
                issues.Add(issue);
                ++num;
              }
            }
          }
        }
      }
      return (IEnumerable<Issue>) issues;
    }
  }
}
