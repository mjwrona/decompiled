// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TimelineHelpers
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TimelineHelpers
  {
    public static BuildResult GetTimelineResult(
      IList<TimelineRecord> records,
      string stageName,
      string jobName,
      string configuration)
    {
      if (string.IsNullOrWhiteSpace(stageName) && string.IsNullOrWhiteSpace(jobName) && string.IsNullOrWhiteSpace(configuration))
        throw new ArgumentException("At least one timeline record identifier is required.");
      TimelineRecord stage = (TimelineRecord) null;
      TimelineRecord job = (TimelineRecord) null;
      TimelineRecord timelineRecord = (TimelineRecord) null;
      if (!string.IsNullOrWhiteSpace(stageName))
      {
        stage = records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.RecordType == "Stage")).SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.Name == stageName));
        if (stage == null)
          return BuildResult.None;
      }
      if (!string.IsNullOrWhiteSpace(jobName))
      {
        IEnumerable<TimelineRecord> source = records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.RecordType == "Phase"));
        if (stage != null)
          source = source.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r =>
          {
            Guid? parentId = r.ParentId;
            Guid id = stage.Id;
            if (!parentId.HasValue)
              return false;
            return !parentId.HasValue || parentId.GetValueOrDefault() == id;
          }));
        job = source.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.Name == jobName));
        if (job == null)
          return BuildResult.None;
      }
      if (!string.IsNullOrWhiteSpace(configuration))
      {
        IEnumerable<TimelineRecord> source = records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.RecordType == "Job"));
        if (job != null)
          source = source.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r =>
          {
            Guid? parentId = r.ParentId;
            Guid id = job.Id;
            if (!parentId.HasValue)
              return false;
            return !parentId.HasValue || parentId.GetValueOrDefault() == id;
          }));
        else if (stage != null)
          source = source.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => records.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (p =>
          {
            Guid? parentId = r.ParentId;
            Guid id1 = p.Id;
            if ((parentId.HasValue ? (parentId.HasValue ? (parentId.GetValueOrDefault() == id1 ? 1 : 0) : 1) : 0) == 0)
              return false;
            parentId = p.ParentId;
            Guid id2 = stage.Id;
            if (!parentId.HasValue)
              return false;
            return !parentId.HasValue || parentId.GetValueOrDefault() == id2;
          })) != null));
        timelineRecord = source.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.Name == configuration));
        if (timelineRecord == null)
          return BuildResult.None;
      }
      TaskResult? nullable = (TaskResult?) ((TaskResult?) timelineRecord?.Result ?? (TaskResult?) job?.Result ?? stage?.Result);
      return !nullable.HasValue ? BuildResult.None : nullable.Value.ToBuildResult();
    }
  }
}
