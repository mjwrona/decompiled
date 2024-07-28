// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.OnPremTelemetryHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class OnPremTelemetryHelper
  {
    private static readonly Guid s_vsTestTaskId = new Guid("ef087383-ee5e-42c7-9a53-ab56c98420f9");

    public static double GetVsTestAggregateDuration(
      Timeline timeline,
      Guid jobId,
      double averageRunDuration)
    {
      try
      {
        foreach (TimelineRecord timelineRecord in timeline.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x =>
        {
          if (string.Equals(x.RecordType, "Task", StringComparison.OrdinalIgnoreCase))
          {
            Guid? parentId = x.ParentId;
            Guid guid = jobId;
            if ((parentId.HasValue ? (parentId.HasValue ? (parentId.GetValueOrDefault() == guid ? 1 : 0) : 1) : 0) != 0)
              return x.Task != null;
          }
          return false;
        })))
        {
          if (timelineRecord.Task.Id == OnPremTelemetryHelper.s_vsTestTaskId)
          {
            DateTime? nullable = timelineRecord.FinishTime;
            if (nullable.HasValue)
            {
              nullable = timelineRecord.StartTime;
              if (nullable.HasValue)
              {
                nullable = timelineRecord.FinishTime;
                DateTime? startTime = timelineRecord.StartTime;
                double totalMinutes = (nullable.HasValue & startTime.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - startTime.GetValueOrDefault()) : new TimeSpan?()).Value.TotalMinutes;
                if (totalMinutes >= 10.0)
                  averageRunDuration = averageRunDuration == 0.0 ? totalMinutes : (averageRunDuration + totalMinutes) / 2.0;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return averageRunDuration;
    }
  }
}
