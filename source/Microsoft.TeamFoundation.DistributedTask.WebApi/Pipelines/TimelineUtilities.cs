// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TimelineUtilities
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineUtilities
  {
    public static void FixRecord(TimelineRecord record)
    {
      if (string.IsNullOrEmpty(record.Identifier))
        return;
      record.RefName = PipelineUtilities.GetName(record.Identifier);
    }

    public static TimelineUtilities.TimelineMapping ParseTimeline(Timeline timeline)
    {
      Dictionary<Guid, TimelineRecord> recordsById = new Dictionary<Guid, TimelineRecord>();
      Dictionary<Guid, IList<TimelineRecord>> recordsByParentId = new Dictionary<Guid, IList<TimelineRecord>>();
      if (timeline != null && timeline.Records != null)
      {
        foreach (TimelineRecord record in timeline.Records)
        {
          recordsById[record.Id] = record;
          if (record.ParentId.HasValue)
          {
            IList<TimelineRecord> timelineRecordList;
            if (!recordsByParentId.TryGetValue(record.ParentId.Value, out timelineRecordList))
            {
              timelineRecordList = (IList<TimelineRecord>) new List<TimelineRecord>();
              recordsByParentId.Add(record.ParentId.Value, timelineRecordList);
            }
            timelineRecordList.Add(record);
          }
          else if (record.RecordType == "Stage")
            TimelineUtilities.FixRecord(record);
        }
      }
      return new TimelineUtilities.TimelineMapping((IDictionary<Guid, TimelineRecord>) recordsById, (IDictionary<Guid, IList<TimelineRecord>>) recordsByParentId);
    }

    public static IList<TimelineRecord> CollectAllChildren(
      Guid rootId,
      IDictionary<Guid, IList<TimelineRecord>> recordsByParent,
      int maxDepth = 2147483647)
    {
      List<TimelineRecord> timelineRecordList1 = new List<TimelineRecord>();
      IList<TimelineRecord> source;
      if (!recordsByParent.TryGetValue(rootId, out source))
        return (IList<TimelineRecord>) timelineRecordList1;
      Queue<Tuple<TimelineRecord, int>> tupleQueue = new Queue<Tuple<TimelineRecord, int>>(source.Select<TimelineRecord, Tuple<TimelineRecord, int>>((Func<TimelineRecord, Tuple<TimelineRecord, int>>) (x => Tuple.Create<TimelineRecord, int>(x, 1))));
      while (tupleQueue.Count > 0)
      {
        Tuple<TimelineRecord, int> tuple = tupleQueue.Dequeue();
        TimelineRecord timelineRecord1 = tuple.Item1;
        int num1 = tuple.Item2;
        timelineRecordList1.Add(timelineRecord1);
        if (num1 < maxDepth)
        {
          int num2 = num1 + 1;
          IList<TimelineRecord> timelineRecordList2;
          if (recordsByParent.TryGetValue(timelineRecord1.Id, out timelineRecordList2))
          {
            foreach (TimelineRecord timelineRecord2 in (IEnumerable<TimelineRecord>) timelineRecordList2)
              tupleQueue.Enqueue(Tuple.Create<TimelineRecord, int>(timelineRecord2, num2));
          }
        }
      }
      return (IList<TimelineRecord>) timelineRecordList1;
    }

    public struct TimelineMapping
    {
      public readonly IDictionary<Guid, TimelineRecord> RecordsById;
      public readonly IDictionary<Guid, IList<TimelineRecord>> RecordsByParentId;

      public TimelineMapping(
        IDictionary<Guid, TimelineRecord> recordsById,
        IDictionary<Guid, IList<TimelineRecord>> recordsByParentId)
      {
        this.RecordsById = recordsById;
        this.RecordsByParentId = recordsByParentId;
      }
    }
  }
}
