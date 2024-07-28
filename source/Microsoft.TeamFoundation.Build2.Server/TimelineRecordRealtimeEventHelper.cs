// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TimelineRecordRealtimeEventHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Server.Events;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TimelineRecordRealtimeEventHelper
  {
    public const int MaxMessageSize = 253952;
    public const int MaxLineLength = 1024;
    private const int TimelineRecordsUpdatedOverhead = 34;

    public static Task SendTimelineRecordsUpdatedEventAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid planId,
      Guid timelineId,
      IEnumerable<TimelineRecord> timelineRecords,
      string clientId = null)
    {
      TimelineRecord timelineRecord = timelineRecords.FirstOrDefault<TimelineRecord>();
      if (timelineRecord == null)
        return Task.CompletedTask;
      IBuildDispatcher service = requestContext.GetService<IBuildDispatcher>();
      if (requestContext.IsFeatureEnabled("Build2.SendStagesUpdated"))
      {
        List<TimelineRecordUpdate> list = timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => r?.RecordType == "Stage")).Select<TimelineRecord, TimelineRecordUpdate>((Func<TimelineRecord, TimelineRecordUpdate>) (r => new TimelineRecordUpdate(r))).ToList<TimelineRecordUpdate>();
        if (list.Count > 0)
          service.SendStagesUpdatedAsync(requestContext, clientId, projectId, buildId, timelineId, list);
      }
      int changeId = timelineRecord.ChangeId;
      return service.SendTimelineRecordsUpdatedAsync(requestContext, clientId, buildId, planId, timelineId, changeId);
    }

    public static void SendTimelineRecordsUpdatedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid planId,
      Guid timelineId,
      IEnumerable<TimelineRecord> timelineRecords,
      string clientId = null)
    {
      TimelineRecord timelineRecord = timelineRecords.FirstOrDefault<TimelineRecord>();
      if (timelineRecord == null)
        return;
      int changeId = timelineRecord.ChangeId;
      IBuildDispatcher service = requestContext.GetService<IBuildDispatcher>();
      service.SendTimelineRecordsUpdated(requestContext, clientId, buildId, planId, timelineId, changeId);
      if (!requestContext.IsFeatureEnabled("Build2.SendStagesUpdated"))
        return;
      List<TimelineRecordUpdate> list = timelineRecords.Where<TimelineRecord>((Func<TimelineRecord, bool>) (r => r?.RecordType == "Stage")).Select<TimelineRecord, TimelineRecordUpdate>((Func<TimelineRecord, TimelineRecordUpdate>) (r => new TimelineRecordUpdate(r))).ToList<TimelineRecordUpdate>();
      if (list.Count <= 0)
        return;
      service.SendStagesUpdated(requestContext, clientId, projectId, buildId, timelineId, list);
    }

    private static int GetStringLength(string value) => !string.IsNullOrEmpty(value) ? value.Length : 0;
  }
}
