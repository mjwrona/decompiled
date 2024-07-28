// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrendService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTrendService : IWorkItemTrendService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<TrendDataPoint<T>> GetTrendData<T>(
      IVssRequestContext requestContext,
      string referenceName,
      DateTime beginDate,
      TimeZoneInfo timeZone,
      TimeSpan interval,
      DateTime? endDate = null)
    {
      FieldEntry fieldEntry = WorkItemTrendService.ValidateField(requestContext, referenceName, false, (string) null, out FieldEntry _);
      if (fieldEntry.SystemType != typeof (T))
        throw new InvalidOperationException();
      beginDate = DateTime.SpecifyKind(beginDate, DateTimeKind.Unspecified);
      if (endDate.HasValue)
      {
        endDate = new DateTime?(DateTime.SpecifyKind(endDate.Value, DateTimeKind.Unspecified));
        if (endDate.Value < beginDate)
          throw new InvalidOperationException();
      }
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      IEqualityComparer<T> equalityComparer = AggregationUtils.GetEqualityComparer<T>(requestContext);
      IList<TrendDataPoint<T>> result = (IList<TrendDataPoint<T>>) new List<TrendDataPoint<T>>();
      using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
      {
        using (ResultCollection trendData = component.GetTrendData(fieldEntry.FieldId, TimeZoneInfo.ConvertTimeToUtc(beginDate, timeZone), endDate.HasValue ? new DateTime?(TimeZoneInfo.ConvertTimeToUtc(endDate.Value, timeZone)) : endDate))
        {
          if (!endDate.HasValue)
          {
            ObjectBinder<DateTime> current = trendData.GetCurrent<DateTime>();
            current.MoveNext();
            endDate = new DateTime?(TimeZoneInfo.ConvertTime(current.Current, timeZone));
          }
          if (beginDate > endDate.Value)
            requestContext.Trace(901800, TraceLevel.Warning, "Services", nameof (WorkItemTrendService), "Begin date is greater than end date. Begin date: {0}, End date: {1}", (object) beginDate, (object) endDate.Value);
          DateTime dateTime = beginDate;
          while (dateTime <= endDate.Value)
          {
            result.Add(new TrendDataPoint<T>(dateTime, equalityComparer));
            if (!(dateTime == endDate.Value))
            {
              dateTime = dateTime.Add(interval);
              if (dateTime > endDate.Value)
                dateTime = endDate.Value;
            }
            else
              break;
          }
          WorkItemTrendService.FillResult<T>(requestContext, trendData, result, converter, timeZone);
          WorkItemTrendService.FillResult<T>(requestContext, trendData, result, converter, timeZone);
        }
      }
      return result;
    }

    private static void FillResult<T>(
      IVssRequestContext requestContext,
      ResultCollection rc,
      IList<TrendDataPoint<T>> result,
      TypeConverter converter,
      TimeZoneInfo timeZone)
    {
      int index1 = 0;
      rc.NextResult();
      ObjectBinder<TrendDataRecord> current1 = rc.GetCurrent<TrendDataRecord>();
label_11:
      while (current1.MoveNext())
      {
        TrendDataRecord current2 = current1.Current;
        T key = (T) converter.ConvertFrom((object) current2.Value);
        DateTime dateTime1 = TimeZoneInfo.ConvertTime(current2.AuthorizedDate, timeZone);
        if (dateTime1 > result[result.Count - 1].DateTime)
        {
          requestContext.Trace(901801, TraceLevel.Warning, "Services", nameof (WorkItemTrendService), "Trend record date is greater than end date. End date: {0}, Authorized date: {1}", (object) result[result.Count - 1].DateTime, (object) dateTime1);
          break;
        }
        while (result[index1].DateTime < dateTime1)
          ++index1;
        DateTime dateTime2 = TimeZoneInfo.ConvertTime(current2.RevisedDate, timeZone);
        int index2 = index1;
        while (true)
        {
          if (index2 < result.Count && result[index2].DateTime < dateTime2)
          {
            if (current2.Count == 0)
              result[index2].ValueCounts.Remove(key);
            else
              result[index2].ValueCounts[key] = current2.Count;
            ++index2;
          }
          else
            goto label_11;
        }
      }
    }

    public void RebuildTrendData(
      IVssRequestContext requestContext,
      string referenceName,
      string markerFieldReferenceName,
      DateTime beginDate,
      bool isBlocking = false)
    {
      FieldEntry markerField;
      FieldEntry fieldEntry = WorkItemTrendService.ValidateField(requestContext, referenceName, true, markerFieldReferenceName, out markerField);
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/TrendDataInterval", true, 900);
      TrendDataRebuildJobData jobData = new TrendDataRebuildJobData()
      {
        FieldReferenceName = referenceName,
        FieldId = fieldEntry.FieldId,
        MarkerFieldReferenceName = markerFieldReferenceName,
        MarkerFieldId = markerField.FieldId,
        BeginDate = beginDate,
        Interval = num
      };
      if (isBlocking)
        this.ExecuteUnderLock(requestContext, jobData.FieldReferenceName, (Action) (() =>
        {
          TrendDataRebuildJobData jobData1 = this.BeginTrendDataRebuild(requestContext, jobData, false);
          if (jobData1 == null)
            return;
          this.EndTrendDataRebuild(requestContext, jobData1);
        }));
      else
        requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.BeginTrendDataRebuildTask), (object) jobData, 0));
    }

    private void ExecuteUnderLock(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      Action action)
    {
      using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, "TREND_DATA_REBUILD_" + fieldReferenceName))
        action();
    }

    private void BeginTrendDataRebuildTask(IVssRequestContext requestContext, object jobDataObject) => this.BeginTrendDataRebuild(requestContext, (TrendDataRebuildJobData) jobDataObject);

    private void BeginTrendDataRebuildInternal(
      IVssRequestContext requestContext,
      ref TrendDataRebuildJobData jobData)
    {
      DateTime endDate = DateTime.MinValue;
      IEnumerable<TrendDataRecord> source = (IEnumerable<TrendDataRecord>) null;
      using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
        source = (IEnumerable<TrendDataRecord>) component.BeginRebuildTrendData(jobData.FieldId, jobData.MarkerFieldId, jobData.Interval, out endDate);
      jobData.EndDate = endDate;
      jobData.Baseline = source.Select<TrendDataRecord, TrendDataBaselineRecord>((Func<TrendDataRecord, TrendDataBaselineRecord>) (x => new TrendDataBaselineRecord()
      {
        Value = x.Value,
        Count = x.Count
      })).ToArray<TrendDataBaselineRecord>();
    }

    private TrendDataRebuildJobData BeginTrendDataRebuild(
      IVssRequestContext requestContext,
      TrendDataRebuildJobData jobData,
      bool queueJob = true)
    {
      IVssRegistryService coreRegistry = requestContext.GetService<IVssRegistryService>();
      Guid jobId = coreRegistry.GetValue<Guid>(requestContext, (RegistryQuery) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/WorkItemTracking/TrendDataRebuild/{0}", (object) jobData.FieldId), false, Guid.Empty);
      ITeamFoundationJobService jobService = requestContext.GetService<ITeamFoundationJobService>();
      if (jobId != Guid.Empty)
      {
        jobService.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          jobId
        }, Enumerable.Empty<TeamFoundationJobDefinition>());
        jobService.StopJob(requestContext, jobId);
      }
      if (queueJob)
        this.ExecuteUnderLock(requestContext, jobData.FieldReferenceName, (Action) (() =>
        {
          this.BeginTrendDataRebuildInternal(requestContext, ref jobData);
          Guid guid = jobService.QueueOneTimeJob(requestContext, TrendDataRebuildJobExtension.JobName, TrendDataRebuildJobExtension.ExtensionName, TeamFoundationSerializationUtility.SerializeToXml((object) jobData), JobPriorityLevel.Normal);
          coreRegistry.SetValue<Guid>(requestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/WorkItemTracking/TrendDataRebuild/{0}", (object) jobData.FieldId), guid);
        }));
      else
        this.BeginTrendDataRebuildInternal(requestContext, ref jobData);
      return jobData;
    }

    internal void EndTrendDataRebuild(
      IVssRequestContext requestContext,
      TrendDataRebuildJobData jobData)
    {
      FieldEntry markerField;
      FieldEntry fieldEntry = WorkItemTrendService.ValidateField(requestContext, jobData.FieldReferenceName, true, jobData.MarkerFieldReferenceName, out markerField);
      IEqualityComparer<string> equalityComparer = AggregationUtils.GetEqualityComparer(requestContext, typeof (string)) as IEqualityComparer<string>;
      Dictionary<string, Tuple<int, DateTime>> dictionary = ((IEnumerable<TrendDataBaselineRecord>) jobData.Baseline).ToDictionary<TrendDataBaselineRecord, string, Tuple<int, DateTime>>((Func<TrendDataBaselineRecord, string>) (x => x.Value), (Func<TrendDataBaselineRecord, Tuple<int, DateTime>>) (x => new Tuple<int, DateTime>(x.Count, jobData.EndDate)), equalityComparer);
      bool flag = false;
      DateTime dateTime;
      for (DateTime endDate = jobData.EndDate; jobData.BeginDate < endDate; endDate = dateTime)
      {
        dateTime = endDate.AddDays(-7.0);
        if (dateTime < jobData.BeginDate)
          dateTime = jobData.BeginDate;
        TrendDataRecord[] array;
        using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
        {
          Dictionary<int, WorkItemTrendService.WorkItemInfo> workItems = (Dictionary<int, WorkItemTrendService.WorkItemInfo>) null;
          using (ResultCollection changedWorkItemData = component.GetChangedWorkItemData(fieldEntry.FieldId, markerField.FieldId, dateTime, endDate))
            workItems = WorkItemTrendService.GetWorkItemChanges(changedWorkItemData, dateTime, endDate);
          array = WorkItemTrendService.GenerateTrendDataRecords(workItems, dictionary, dateTime, endDate, jobData.Interval).ToArray<TrendDataRecord>();
        }
        if (array.Length != 0)
        {
          using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
            component.AddTrendDataRecords(fieldEntry.FieldId, (IList<TrendDataRecord>) array);
        }
        flag = dictionary.Any<KeyValuePair<string, Tuple<int, DateTime>>>((Func<KeyValuePair<string, Tuple<int, DateTime>>, bool>) (x => x.Value.Item1 > 0));
        if (!flag)
          break;
      }
      if (!flag)
        return;
      TrendDataRecord[] array1 = dictionary.Where<KeyValuePair<string, Tuple<int, DateTime>>>((Func<KeyValuePair<string, Tuple<int, DateTime>>, bool>) (x => x.Value.Item2 > jobData.BeginDate)).Select<KeyValuePair<string, Tuple<int, DateTime>>, TrendDataRecord>((Func<KeyValuePair<string, Tuple<int, DateTime>>, TrendDataRecord>) (x =>
      {
        return new TrendDataRecord()
        {
          Value = x.Key,
          Count = x.Value.Item1,
          AuthorizedDate = jobData.BeginDate,
          RevisedDate = x.Value.Item2
        };
      })).ToArray<TrendDataRecord>();
      if (!((IEnumerable<TrendDataRecord>) array1).Any<TrendDataRecord>())
        return;
      using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
        component.AddTrendDataRecords(fieldEntry.FieldId, (IList<TrendDataRecord>) array1);
    }

    internal void SetTrendDataBaseline(
      IVssRequestContext requestContext,
      string referenceName,
      TimeZoneInfo timeZone,
      IList<TrendDataPoint<string>> cachedResults)
    {
      FieldEntry fieldEntry = WorkItemTrendService.ValidateField(requestContext, referenceName, false, (string) null, out FieldEntry _);
      List<TrendDataRecord> records = new List<TrendDataRecord>();
      DateTime? revisedDate = new DateTime?();
      for (int index = 0; index < cachedResults.Count; ++index)
      {
        TrendDataPoint<string> cachedResult = cachedResults[index];
        DateTime utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(cachedResult.DateTime, DateTimeKind.Unspecified), timeZone);
        records.AddRange(WorkItemTrendService.GenerateTrendDataRecords(cachedResult.ValueCounts, utc, revisedDate));
        revisedDate = new DateTime?(utc);
      }
      using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
        component.SetTrendDataBaseline(fieldEntry.FieldId, (IList<TrendDataRecord>) records);
    }

    internal void StampTrendDataBaseline(IVssRequestContext requestContext, string referenceName)
    {
      FieldEntry fieldEntry = WorkItemTrendService.ValidateField(requestContext, referenceName, false, (string) null, out FieldEntry _);
      using (WorkItemTrendDataComponent component = WorkItemTrendDataComponent.CreateComponent(requestContext))
        component.StampTrendDataBaseline(fieldEntry.FieldId);
    }

    private static FieldEntry ValidateField(
      IVssRequestContext requestContext,
      string referenceName,
      bool checkMarkerField,
      string markerFieldReferenceName,
      out FieldEntry markerField)
    {
      markerField = (FieldEntry) null;
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      FieldEntry field = service.GetField(requestContext, referenceName);
      if (!field.IsUsedInTrendData)
        throw new InvalidOperationException();
      if (!checkMarkerField)
        return field;
      markerField = service.GetField(requestContext, markerFieldReferenceName);
      if (!(markerField.SystemType != typeof (bool)))
        return field;
      throw new InvalidOperationException();
    }

    private static Dictionary<int, WorkItemTrendService.WorkItemInfo> GetWorkItemChanges(
      ResultCollection rc,
      DateTime currentBeginDate,
      DateTime endDate)
    {
      Dictionary<int, WorkItemTrendService.WorkItemInfo> workItemChanges = new Dictionary<int, WorkItemTrendService.WorkItemInfo>();
      ObjectBinder<WorkItemDataRecord> current1 = rc.GetCurrent<WorkItemDataRecord>();
      while (current1.MoveNext())
      {
        WorkItemDataRecord current2 = current1.Current;
        WorkItemTrendService.WorkItemInfo workItemInfo;
        if (!workItemChanges.TryGetValue(current2.Id, out workItemInfo))
        {
          workItemInfo = new WorkItemTrendService.WorkItemInfo();
          workItemChanges.Add(current2.Id, workItemInfo);
        }
        workItemInfo.AddMarkerFieldInfo(current2);
      }
      rc.NextResult();
      ObjectBinder<WorkItemDataRecord> current3 = rc.GetCurrent<WorkItemDataRecord>();
      while (current3.MoveNext())
      {
        WorkItemDataRecord current4 = current3.Current;
        WorkItemTrendService.WorkItemInfo workItemInfo;
        if (workItemChanges.TryGetValue(current4.Id, out workItemInfo))
          workItemInfo.AddFieldChanges(current4, currentBeginDate, endDate);
      }
      return workItemChanges;
    }

    private static IEnumerable<TrendDataRecord> GenerateTrendDataRecords(
      Dictionary<int, WorkItemTrendService.WorkItemInfo> workItems,
      Dictionary<string, Tuple<int, DateTime>> baseline,
      DateTime currentBeginDate,
      DateTime endDate,
      int interval)
    {
      while (currentBeginDate < endDate)
      {
        DateTime beginDate = endDate.AddSeconds((double) -interval);
        Dictionary<string, int> dictionary = new Dictionary<string, int>(baseline.Comparer);
        foreach (WorkItemTrendService.WorkItemInfo workItemInfo in workItems.Values)
        {
          foreach (Tuple<DateTime, string, bool> fieldChange in workItemInfo.GetFieldChanges(beginDate))
          {
            int num = 0;
            if (!dictionary.TryGetValue(fieldChange.Item2, out num))
              num = 0;
            dictionary[fieldChange.Item2] = num + (fieldChange.Item3 ? 1 : -1);
          }
        }
        List<TrendDataRecord> trendDataRecordList1 = (List<TrendDataRecord>) null;
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
        {
          if (keyValuePair.Value != 0)
          {
            int num1 = 0;
            Tuple<int, DateTime> tuple;
            if (baseline.TryGetValue(keyValuePair.Key, out tuple))
            {
              num1 = tuple.Item1;
              if (trendDataRecordList1 == null)
                trendDataRecordList1 = new List<TrendDataRecord>();
              List<TrendDataRecord> trendDataRecordList2 = trendDataRecordList1;
              TrendDataRecord trendDataRecord = new TrendDataRecord();
              trendDataRecord.AuthorizedDate = beginDate;
              trendDataRecord.RevisedDate = tuple.Item2;
              trendDataRecord.Value = keyValuePair.Key;
              trendDataRecord.Count = num1;
              trendDataRecordList2.Add(trendDataRecord);
            }
            int num2 = num1 - keyValuePair.Value;
            if (num2 > 0)
              baseline[keyValuePair.Key] = new Tuple<int, DateTime>(num2, beginDate);
            else if (num2 == 0)
              baseline.Remove(keyValuePair.Key);
          }
        }
        endDate = beginDate;
        if (trendDataRecordList1 != null)
        {
          foreach (TrendDataRecord trendDataRecord in trendDataRecordList1)
            yield return trendDataRecord;
        }
      }
    }

    private static IEnumerable<TrendDataRecord> GenerateTrendDataRecords(
      IDictionary<string, int> valueCounts,
      DateTime authorizedDate,
      DateTime? revisedDate)
    {
      foreach (KeyValuePair<string, int> valueCount in (IEnumerable<KeyValuePair<string, int>>) valueCounts)
      {
        TrendDataRecord trendDataRecord = new TrendDataRecord();
        trendDataRecord.AuthorizedDate = authorizedDate;
        trendDataRecord.RevisedDate = revisedDate.HasValue ? revisedDate.Value : SharedVariables.FutureDateTimeValue;
        trendDataRecord.Value = valueCount.Key;
        trendDataRecord.Count = valueCount.Value;
        yield return trendDataRecord;
      }
    }

    private class WorkItemInfo
    {
      private int m_markerFieldChangesIndex;
      private int m_changesIndex;
      private List<DateTime> m_markerFieldChanges = new List<DateTime>();
      private List<Tuple<DateTime, string, bool>> m_changes = new List<Tuple<DateTime, string, bool>>();

      public void AddMarkerFieldInfo(WorkItemDataRecord record)
      {
        if (this.m_markerFieldChanges.Count > 0 && record.RevisedDate == this.m_markerFieldChanges[this.m_markerFieldChanges.Count - 1])
        {
          this.m_markerFieldChanges[this.m_markerFieldChanges.Count - 1] = record.AuthorizedDate;
        }
        else
        {
          this.m_markerFieldChanges.Add(record.RevisedDate);
          this.m_markerFieldChanges.Add(record.AuthorizedDate);
        }
      }

      public void AddFieldChanges(WorkItemDataRecord record, DateTime beginDate, DateTime endDate)
      {
        for (; this.m_markerFieldChanges[this.m_markerFieldChangesIndex] > record.RevisedDate; ++this.m_markerFieldChangesIndex)
        {
          if (this.m_markerFieldChangesIndex == this.m_markerFieldChanges.Count - 1)
            return;
        }
        DateTime minValue = DateTime.MinValue;
        DateTime revisedDate = this.m_markerFieldChangesIndex % 2 != 0 ? record.RevisedDate : this.m_markerFieldChanges[this.m_markerFieldChangesIndex];
        for (int fieldChangesIndex = this.m_markerFieldChangesIndex; fieldChangesIndex < this.m_markerFieldChanges.Count && this.m_markerFieldChanges[fieldChangesIndex] >= record.AuthorizedDate; ++fieldChangesIndex)
        {
          if (fieldChangesIndex % 2 == 0)
          {
            revisedDate = this.m_markerFieldChanges[fieldChangesIndex];
          }
          else
          {
            this.AddFieldChanges(beginDate, endDate, this.m_markerFieldChanges[fieldChangesIndex], revisedDate, record.Value);
            revisedDate = DateTime.MinValue;
          }
        }
        if (!(revisedDate > record.AuthorizedDate))
          return;
        this.AddFieldChanges(beginDate, endDate, record.AuthorizedDate, revisedDate, record.Value);
      }

      public IEnumerable<Tuple<DateTime, string, bool>> GetFieldChanges(DateTime beginDate)
      {
        WorkItemTrendService.WorkItemInfo workItemInfo1 = this;
        while (workItemInfo1.m_changesIndex < workItemInfo1.m_changes.Count && workItemInfo1.m_changes[workItemInfo1.m_changesIndex].Item1 >= beginDate)
        {
          List<Tuple<DateTime, string, bool>> changes = workItemInfo1.m_changes;
          WorkItemTrendService.WorkItemInfo workItemInfo2 = workItemInfo1;
          int changesIndex = workItemInfo1.m_changesIndex;
          int num = changesIndex + 1;
          workItemInfo2.m_changesIndex = num;
          int index = changesIndex;
          yield return changes[index];
        }
      }

      private void AddFieldChanges(
        DateTime beginDate,
        DateTime endDate,
        DateTime authorizedDate,
        DateTime revisedDate,
        string value)
      {
        if (revisedDate >= beginDate && revisedDate < endDate)
          this.m_changes.Add(new Tuple<DateTime, string, bool>(revisedDate, value, false));
        if (!(authorizedDate >= beginDate) || !(authorizedDate < endDate))
          return;
        this.m_changes.Add(new Tuple<DateTime, string, bool>(authorizedDate, value, true));
      }
    }
  }
}
