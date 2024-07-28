// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ChartDataComponent
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.Azure.Boards.Charts.Exceptions;
using Microsoft.Azure.Boards.Charts.Wiql;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Boards.Charts
{
  internal class ChartDataComponent : IChartDataComponent
  {
    private const int CumulativeFlowSampleInterval = 7;

    public IEnumerable<BurndownChartDataPoint> GetBurndownChartData(
      IVssRequestContext requestContext,
      BurndownChartInputs burndownChartInputs)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      WiqlQuery wiqlQuery1 = new WiqlQuery()
      {
        Select = {
          CoreFieldReferenceNames.Id
        },
        Where = (Condition) new GroupCondition()
        {
          Operator = GroupOperator.And,
          Conditions = {
            (Condition) burndownChartInputs.GetFilterConditions(),
            (Condition) new SingleValueCondition()
            {
              Field = CoreFieldReferenceNames.IterationPath,
              Operator = SingleValueOperator.Under,
              Value = (object) burndownChartInputs.Iteration.IterationPath
            },
            (Condition) new MultiValueCondition()
            {
              Field = CoreFieldReferenceNames.WorkItemType,
              Operator = MultiValueOperator.In,
              Values = new Collection<object>((IList<object>) burndownChartInputs.WorkItemTypes.Cast<object>().ToList<object>())
            },
            (Condition) new MultiValueCondition()
            {
              Field = CoreFieldReferenceNames.State,
              Operator = MultiValueOperator.In,
              Values = new Collection<object>((IList<object>) burndownChartInputs.InProgressStates.Cast<object>().ToList<object>())
            },
            (Condition) new SingleValueCondition()
            {
              Field = burndownChartInputs.RemainingWorkField,
              Operator = SingleValueOperator.GreaterThanOrEqual,
              Value = (object) 0
            }
          }
        }
      };
      string queryText = wiqlQuery1.ToString();
      List<BurndownChartDataPoint> cachedBurndownData = this.GetCachedBurndownData(requestContext, burndownChartInputs, queryText);
      try
      {
        DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, burndownChartInputs.TimeZone).Date;
        List<BurndownChartDataPoint> burndownChartData = new List<BurndownChartDataPoint>();
        int valueFromPath = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Application/Backlogs/Team/*").GetValueFromPath<int>("/Configuration/Application/Backlogs/Team/maxBurnDownWorkItemQueryLimit", 1000);
        int limit = Math.Min(burndownChartInputs.WorkItemCountLimit, valueFromPath);
        DateTime dateTime1 = burndownChartInputs.Iteration.FinishDate.Value;
        ref DateTime local = ref dateTime1;
        DateTime? nullable1 = burndownChartInputs.Iteration.StartDate;
        DateTime dateTime2 = nullable1.Value;
        IReadOnlyCollection<DateTime> list1 = (IReadOnlyCollection<DateTime>) Enumerable.Range(0, local.Subtract(dateTime2).Days + 1).Select<int, DateTime>((System.Func<int, DateTime>) (offset => burndownChartInputs.Iteration.StartDate.Value.AddDays((double) offset))).ToList<DateTime>();
        requestContext.Trace(15191005, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, string.Format("BurndownDays : {0}", (object) list1.Count));
        foreach (DateTime dateTime3 in (IEnumerable<DateTime>) list1)
        {
          DateTime currentDate = DateTime.SpecifyKind(dateTime3, DateTimeKind.Unspecified);
          BurndownChartDataPoint burndownChartDataPoint1 = (BurndownChartDataPoint) null;
          if (cachedBurndownData != null)
            burndownChartDataPoint1 = cachedBurndownData.FirstOrDefault<BurndownChartDataPoint>((System.Func<BurndownChartDataPoint, bool>) (o => o.Date == currentDate));
          if (burndownChartDataPoint1 != null && burndownChartDataPoint1.RemainingWork.HasValue)
          {
            requestContext.Trace(15191006, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, string.Format("BurndownDataPoint For : {0}", (object) currentDate));
            burndownChartData.Add(burndownChartDataPoint1);
          }
          else if (currentDate <= date)
          {
            if (currentDate < date)
            {
              wiqlQuery1.AsOf = new DateTime?(currentDate.GetEndOfDayUtc(burndownChartInputs.TimeZone));
            }
            else
            {
              WiqlQuery wiqlQuery2 = wiqlQuery1;
              nullable1 = new DateTime?();
              DateTime? nullable2 = nullable1;
              wiqlQuery2.AsOf = nullable2;
            }
            requestContext.Trace(15191007, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, string.Format("ExecutingWiql For : {0}, {1}", (object) currentDate, (object) wiqlQuery1.ToString()));
            List<int> list2 = vssRequestContext.GetService<IWorkItemQueryService>().ExecuteQuery(vssRequestContext, wiqlQuery1.ToString(), querySource: QuerySource.Charting).WorkItemIds.ToList<int>();
            if (burndownChartInputs.EnforceLimit && list2.Count > limit)
            {
              requestContext.Trace(15191008, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, string.Format("workItemCountExccded For : {0}/{1}, {2}", (object) list2.Count, (object) limit, (object) wiqlQuery1.ToString()));
              throw new BurndownWorkItemLimitExceededException(limit, list2.Count);
            }
            IEnumerable<int> source1 = (IEnumerable<int>) list2;
            BurndownChartDataPoint burndownChartDataPoint2 = new BurndownChartDataPoint()
            {
              Date = currentDate,
              RemainingWork = new double?(0.0)
            };
            for (int index = 0; index < list2.Count; index += 200)
            {
              GenericDataReader source2 = service.PageWorkItems(vssRequestContext, source1.Take<int>(200), (IEnumerable<string>) new string[2]
              {
                CoreFieldReferenceNames.Id,
                burndownChartInputs.RemainingWorkField
              }, wiqlQuery1.AsOf, WorkItemRetrievalMode.All);
              BurndownChartDataPoint burndownChartDataPoint3 = burndownChartDataPoint2;
              double? remainingWork = burndownChartDataPoint3.RemainingWork;
              double num = source2.Sum<IDataRecord>((System.Func<IDataRecord, double>) (w => (double) (w[burndownChartInputs.RemainingWorkField] ?? (object) 0.0)));
              burndownChartDataPoint3.RemainingWork = remainingWork.HasValue ? new double?(remainingWork.GetValueOrDefault() + num) : new double?();
              if (index + 200 < list2.Count)
                source1 = source1.Skip<int>(200);
            }
            burndownChartData.Add(burndownChartDataPoint2);
          }
          else
            burndownChartData.Add(new BurndownChartDataPoint()
            {
              Date = currentDate,
              RemainingWork = new double?()
            });
        }
        this.SetCachedBurndownData(vssRequestContext, burndownChartInputs, burndownChartData, queryText, date);
        ChartDataComponent.RemoveNonWorkingDaysFromSprint(burndownChartInputs, burndownChartData);
        ChartDataComponent.CalculateBurndownTrendlines(burndownChartInputs, burndownChartData);
        ChartDataComponent.CalculateAvailableCapacity(burndownChartInputs, (IEnumerable<BurndownChartDataPoint>) burndownChartData);
        return (IEnumerable<BurndownChartDataPoint>) burndownChartData;
      }
      catch (LegacyValidationException ex)
      {
        requestContext.TraceException(15191009, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, (Exception) ex);
        return (IEnumerable<BurndownChartDataPoint>) Array.Empty<BurndownChartDataPoint>();
      }
    }

    private static void RemoveNonWorkingDaysFromSprint(
      BurndownChartInputs burndownChartInputs,
      List<BurndownChartDataPoint> allChartDataPoints)
    {
      List<BurndownChartDataPoint> burndownChartDataPointList = new List<BurndownChartDataPoint>();
      List<DateTime> workingDays = burndownChartInputs.GetWorkingDays();
      burndownChartDataPointList.AddRange(allChartDataPoints.Where<BurndownChartDataPoint>((System.Func<BurndownChartDataPoint, bool>) (dataPoint => !workingDays.Contains(dataPoint.Date))));
      foreach (BurndownChartDataPoint burndownChartDataPoint in burndownChartDataPointList)
        allChartDataPoints.Remove(burndownChartDataPoint);
    }

    public IEnumerable<VelocityChartDataPoint> GetVelocityChartData(
      IVssRequestContext requestContext,
      VelocityChartInputs velocityChartInputs)
    {
      try
      {
        WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
        WiqlQuery velocityChartQuery = this.GetVelocityChartQuery(velocityChartInputs);
        IEnumerable<int> source = requestContext.GetService<IWorkItemQueryService>().ExecuteQuery(requestContext, velocityChartQuery.ToString()).WorkItemIds;
        Dictionary<string, VelocityChartDataPoint> resultsSet = new Dictionary<string, VelocityChartDataPoint>();
        int count = 200;
        for (; source.Any<int>(); source = source.Skip<int>(count))
          this.ProcessVelocityChartData((IEnumerable<IDataRecord>) service.PageWorkItems(requestContext, source.Take<int>(count), (IEnumerable<string>) new string[4]
          {
            CoreFieldReferenceNames.Id,
            CoreFieldReferenceNames.IterationPath,
            CoreFieldReferenceNames.State,
            velocityChartInputs.EffortField
          }), velocityChartInputs, resultsSet);
        List<VelocityChartDataPoint> velocityChartData = new List<VelocityChartDataPoint>();
        foreach (string iteration1 in velocityChartInputs.Iterations)
        {
          string iteration = iteration1;
          VelocityChartDataPoint velocityChartDataPoint = resultsSet.Values.Where<VelocityChartDataPoint>((System.Func<VelocityChartDataPoint, bool>) (dp => dp.Iteration == iteration)).FirstOrDefault<VelocityChartDataPoint>();
          if (velocityChartDataPoint == null)
            velocityChartDataPoint = new VelocityChartDataPoint()
            {
              Iteration = iteration,
              CompletedWork = 0.0,
              RemainingWork = 0.0
            };
          velocityChartDataPoint.Iteration = this.GetIterationDisplayName(velocityChartDataPoint.Iteration);
          velocityChartData.Add(velocityChartDataPoint);
        }
        return (IEnumerable<VelocityChartDataPoint>) velocityChartData;
      }
      catch (LegacyValidationException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        return (IEnumerable<VelocityChartDataPoint>) Array.Empty<VelocityChartDataPoint>();
      }
    }

    private WiqlQuery GetVelocityChartQuery(VelocityChartInputs velocityChartInputs) => new WiqlQuery()
    {
      Select = {
        CoreFieldReferenceNames.Id
      },
      Where = (Condition) new GroupCondition()
      {
        Operator = GroupOperator.And,
        Conditions = {
          (Condition) velocityChartInputs.GetFilterConditions(),
          (Condition) new GroupCondition()
          {
            Operator = GroupOperator.Or,
            Conditions = velocityChartInputs.Iterations.ToConditions(CoreFieldReferenceNames.IterationPath, SingleValueOperator.Under)
          },
          (Condition) new MultiValueCondition()
          {
            Field = CoreFieldReferenceNames.WorkItemType,
            Operator = MultiValueOperator.In,
            Values = new Collection<object>((IList<object>) velocityChartInputs.WorkItemTypes.Cast<object>().ToList<object>())
          },
          (Condition) new MultiValueCondition()
          {
            Field = CoreFieldReferenceNames.State,
            Operator = MultiValueOperator.In,
            Values = new Collection<object>((IList<object>) velocityChartInputs.CompletedStates.Union<string>((IEnumerable<string>) velocityChartInputs.InProgressStates).Cast<object>().ToList<object>())
          },
          (Condition) new SingleValueCondition()
          {
            Field = velocityChartInputs.EffortField,
            Operator = SingleValueOperator.GreaterThan,
            Value = (object) 0
          }
        }
      }
    };

    internal void ProcessVelocityChartData(
      IEnumerable<IDataRecord> workItemsData,
      VelocityChartInputs velocityChartInputs,
      Dictionary<string, VelocityChartDataPoint> resultsSet)
    {
      foreach (IDataRecord dataRecord in workItemsData)
      {
        string key = (string) dataRecord[CoreFieldReferenceNames.IterationPath];
        string str = (string) dataRecord[CoreFieldReferenceNames.State];
        VelocityChartDataPoint velocityChartDataPoint;
        if (!resultsSet.TryGetValue(key, out velocityChartDataPoint))
        {
          velocityChartDataPoint = new VelocityChartDataPoint()
          {
            Iteration = key
          };
          resultsSet[key] = velocityChartDataPoint;
        }
        if (velocityChartInputs.InProgressStates.Contains(str))
          velocityChartDataPoint.RemainingWork += Convert.ToDouble(dataRecord[velocityChartInputs.EffortField]);
        else if (velocityChartInputs.CompletedStates.Contains(str))
          velocityChartDataPoint.CompletedWork += Convert.ToDouble(dataRecord[velocityChartInputs.EffortField]);
      }
    }

    public IEnumerable<CumulativeFlowDiagramDataPoint> GetCumulativeFlowDiagramData(
      IVssRequestContext requestContext,
      Guid teamId,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs)
    {
      if (!cumulativeFlowDiagramInputs.UseKanbanColumns)
      {
        IdentityChartCache view = IdentityPropertiesView.CreateView<IdentityChartCache>(requestContext, teamId, TeamConstants.TeamChartCachePropertyName);
        return (IEnumerable<CumulativeFlowDiagramDataPoint>) this.GetCumulativeFlowDiagramDataViaQuery(requestContext, cumulativeFlowDiagramInputs, view);
      }
      IList<TrendDataPoint<string>> trendData = this.GenerateTrendData(requestContext, cumulativeFlowDiagramInputs);
      IDictionary<DateTime, IDictionary<Guid, int>> dailyColumnCounts = this.GenerateDailyColumnCounts(cumulativeFlowDiagramInputs.BoardColumnRevisions, trendData, cumulativeFlowDiagramInputs.TimeZone);
      IList<CumulativeFlowDiagramDataPoint> cumulativeFlowDiagramData = (IList<CumulativeFlowDiagramDataPoint>) new List<CumulativeFlowDiagramDataPoint>();
      foreach (KeyValuePair<DateTime, IDictionary<Guid, int>> keyValuePair in (IEnumerable<KeyValuePair<DateTime, IDictionary<Guid, int>>>) dailyColumnCounts)
      {
        cumulativeFlowDiagramData.Add(new CumulativeFlowDiagramDataPoint()
        {
          Date = keyValuePair.Key,
          Counts = keyValuePair.Value
        });
        if (!cumulativeFlowDiagramInputs.IsCustomStartDate)
        {
          if (!keyValuePair.Value.Values.Any<int>((System.Func<int, bool>) (c => c > 0)))
            break;
        }
      }
      return (IEnumerable<CumulativeFlowDiagramDataPoint>) cumulativeFlowDiagramData;
    }

    public virtual IList<TrendDataPoint<string>> GenerateTrendData(
      IVssRequestContext requestContext,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs)
    {
      IWorkItemTrendService service = requestContext.GetService<IWorkItemTrendService>();
      DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(cumulativeFlowDiagramInputs.StartDate, DateTimeKind.Utc), TimeZoneInfo.Utc, cumulativeFlowDiagramInputs.TimeZone).Date;
      if (!cumulativeFlowDiagramInputs.IsCustomStartDate)
        dateTime = this.GetPreviousFirstWorkingDay(dateTime, cumulativeFlowDiagramInputs.FirstWorkDay, true);
      DateTime endDate1 = cumulativeFlowDiagramInputs.EndDate;
      DateTime endOfDay1 = this.AdjustToEndOfDay(dateTime, cumulativeFlowDiagramInputs.TimeZone);
      DateTime endOfDay2 = this.AdjustToEndOfDay(endDate1, cumulativeFlowDiagramInputs.TimeZone);
      IVssRequestContext requestContext1 = requestContext;
      string extensionFieldName = cumulativeFlowDiagramInputs.BoardColumnExtensionFieldName;
      DateTime beginDate = endOfDay1;
      TimeZoneInfo timeZone = cumulativeFlowDiagramInputs.TimeZone;
      TimeSpan interval = TimeSpan.FromDays(7.0);
      DateTime? endDate2 = new DateTime?(endOfDay2);
      return service.GetTrendData<string>(requestContext1, extensionFieldName, beginDate, timeZone, interval, endDate2);
    }

    private DateTime AdjustToEndOfDay(DateTime dateTime, TimeZoneInfo timeZone)
    {
      if (dateTime.Kind == DateTimeKind.Utc)
        return dateTime.AddDays(1.0).AddMilliseconds(-1.0);
      DateTime dateTime1 = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
      dateTime1 = dateTime1.AddDays(1.0);
      return TimeZoneInfo.ConvertTimeFromUtc(dateTime1.AddMilliseconds(-1.0), timeZone);
    }

    private IDictionary<DateTime, IDictionary<Guid, int>> GenerateDailyColumnCounts(
      IEnumerable<BoardColumnRevision> revisions,
      IList<TrendDataPoint<string>> trendData,
      TimeZoneInfo timeZone)
    {
      Dictionary<DateTime, IDictionary<Guid, int>> dailyColumnCounts = new Dictionary<DateTime, IDictionary<Guid, int>>();
      foreach (TrendDataPoint<string> trendDataPoint in trendData.Reverse<TrendDataPoint<string>>())
      {
        DateTime dateTime = trendDataPoint.DateTime;
        IDictionary<Guid, int> dictionary = (IDictionary<Guid, int>) new Dictionary<Guid, int>();
        IDictionary<string, Guid> columnNameToIdMapping = this.GenerateColumnNameToIdMapping(dateTime, timeZone, revisions, trendDataPoint.Comparer);
        foreach (KeyValuePair<string, int> valueCount in (IEnumerable<KeyValuePair<string, int>>) trendDataPoint.ValueCounts)
        {
          if (columnNameToIdMapping.ContainsKey(valueCount.Key))
            dictionary[columnNameToIdMapping[valueCount.Key]] = valueCount.Value;
        }
        dailyColumnCounts[dateTime] = dictionary;
      }
      return (IDictionary<DateTime, IDictionary<Guid, int>>) dailyColumnCounts;
    }

    private IDictionary<string, Guid> GenerateColumnNameToIdMapping(
      DateTime date,
      TimeZoneInfo timeZone,
      IEnumerable<BoardColumnRevision> revisions,
      IEqualityComparer<string> comparer)
    {
      IDictionary<Guid, Tuple<string, DateTime>> dictionary1 = (IDictionary<Guid, Tuple<string, DateTime>>) new Dictionary<Guid, Tuple<string, DateTime>>();
      IDictionary<string, Tuple<Guid, DateTime>> source = (IDictionary<string, Tuple<Guid, DateTime>>) new Dictionary<string, Tuple<Guid, DateTime>>(comparer);
      if (date.Date < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date)
      {
        foreach (BoardColumnRevision boardColumnRevision in (IEnumerable<BoardColumnRevision>) revisions.Where<BoardColumnRevision>((System.Func<BoardColumnRevision, bool>) (r =>
        {
          if (!r.Deleted)
          {
            DateTime? revisedDate = r.RevisedDate;
            DateTime futureDateTimeValue = SharedVariables.FutureDateTimeValue;
            if ((revisedDate.HasValue ? (revisedDate.HasValue ? (revisedDate.GetValueOrDefault() != futureDateTimeValue ? 1 : 0) : 0) : 1) != 0)
            {
              revisedDate = r.RevisedDate;
              return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(revisedDate.Value, DateTimeKind.Utc), timeZone) > date;
            }
          }
          return false;
        })).OrderBy<BoardColumnRevision, DateTime>((System.Func<BoardColumnRevision, DateTime>) (r => r.RevisedDate.Value)))
        {
          IDictionary<Guid, Tuple<string, DateTime>> dictionary2 = dictionary1;
          Guid? id = boardColumnRevision.Id;
          Guid key1 = id.Value;
          if (!dictionary2.ContainsKey(key1))
          {
            IDictionary<Guid, Tuple<string, DateTime>> dictionary3 = dictionary1;
            id = boardColumnRevision.Id;
            Guid key2 = id.Value;
            Tuple<string, DateTime> tuple = Tuple.Create<string, DateTime>(boardColumnRevision.Name, TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(boardColumnRevision.RevisedDate.Value, DateTimeKind.Utc), timeZone));
            dictionary3[key2] = tuple;
          }
        }
        foreach (BoardColumnRevision boardColumnRevision in revisions.Where<BoardColumnRevision>((System.Func<BoardColumnRevision, bool>) (r => r.Deleted && TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(r.RevisedDate.Value, DateTimeKind.Utc), timeZone) > date)))
        {
          IDictionary<Guid, Tuple<string, DateTime>> dictionary4 = dictionary1;
          Guid? id = boardColumnRevision.Id;
          Guid key3 = id.Value;
          if (!dictionary4.ContainsKey(key3))
          {
            IDictionary<Guid, Tuple<string, DateTime>> dictionary5 = dictionary1;
            id = boardColumnRevision.Id;
            Guid key4 = id.Value;
            Tuple<string, DateTime> tuple = Tuple.Create<string, DateTime>(boardColumnRevision.Name, TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(boardColumnRevision.RevisedDate.Value, DateTimeKind.Utc), timeZone));
            dictionary5[key4] = tuple;
          }
        }
      }
      foreach (BoardColumnRevision boardColumnRevision in revisions.Where<BoardColumnRevision>((System.Func<BoardColumnRevision, bool>) (r =>
      {
        DateTime? revisedDate = r.RevisedDate;
        DateTime futureDateTimeValue = SharedVariables.FutureDateTimeValue;
        if (!revisedDate.HasValue)
          return false;
        return !revisedDate.HasValue || revisedDate.GetValueOrDefault() == futureDateTimeValue;
      })))
      {
        IDictionary<Guid, Tuple<string, DateTime>> dictionary6 = dictionary1;
        Guid? id = boardColumnRevision.Id;
        Guid key5 = id.Value;
        if (!dictionary6.ContainsKey(key5))
        {
          IDictionary<Guid, Tuple<string, DateTime>> dictionary7 = dictionary1;
          id = boardColumnRevision.Id;
          Guid key6 = id.Value;
          Tuple<string, DateTime> tuple = Tuple.Create<string, DateTime>(boardColumnRevision.Name, TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(boardColumnRevision.RevisedDate.Value, DateTimeKind.Utc), timeZone));
          dictionary7[key6] = tuple;
        }
      }
      foreach (KeyValuePair<Guid, Tuple<string, DateTime>> keyValuePair in (IEnumerable<KeyValuePair<Guid, Tuple<string, DateTime>>>) dictionary1)
      {
        if (source.ContainsKey(keyValuePair.Value.Item1))
        {
          DateTime dateTime1 = source[keyValuePair.Value.Item1].Item2;
          DateTime dateTime2 = keyValuePair.Value.Item2;
          if (dateTime2 > date && dateTime2 < dateTime1)
            source[keyValuePair.Value.Item1] = Tuple.Create<Guid, DateTime>(keyValuePair.Key, keyValuePair.Value.Item2);
        }
        else
          source[keyValuePair.Value.Item1] = Tuple.Create<Guid, DateTime>(keyValuePair.Key, keyValuePair.Value.Item2);
      }
      return (IDictionary<string, Guid>) source.ToDictionary<KeyValuePair<string, Tuple<Guid, DateTime>>, string, Guid>((System.Func<KeyValuePair<string, Tuple<Guid, DateTime>>, string>) (k => k.Key), (System.Func<KeyValuePair<string, Tuple<Guid, DateTime>>, Guid>) (v => v.Value.Item1), comparer);
    }

    internal IList<CumulativeFlowDiagramDataPoint> GetCumulativeFlowDiagramDataViaQuery(
      IVssRequestContext requestContext,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs,
      IdentityChartCache chartCache)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      requestContext.GetService<WebAccessWorkItemService>();
      WiqlQuery wiqlQuery = new WiqlQuery()
      {
        Select = {
          CoreFieldReferenceNames.Id,
          CoreFieldReferenceNames.State
        },
        Where = (Condition) new GroupCondition()
        {
          Operator = GroupOperator.And,
          Conditions = {
            (Condition) cumulativeFlowDiagramInputs.GetFilterConditions(),
            (Condition) new MultiValueCondition()
            {
              Field = CoreFieldReferenceNames.WorkItemType,
              Operator = MultiValueOperator.In,
              Values = new Collection<object>((IList<object>) cumulativeFlowDiagramInputs.WorkItemTypes.Cast<object>().ToList<object>())
            },
            (Condition) new MultiValueCondition()
            {
              Field = CoreFieldReferenceNames.State,
              Operator = MultiValueOperator.In,
              Values = new Collection<object>((IList<object>) cumulativeFlowDiagramInputs.WorkItemStates.Cast<object>().ToList<object>())
            }
          }
        }
      };
      List<CumulativeFlowDiagramDataPoint> cachedFlowDiagramData = this.GetCachedFlowDiagramData(cumulativeFlowDiagramInputs, chartCache);
      try
      {
        List<CumulativeFlowDiagramDataPoint> results = new List<CumulativeFlowDiagramDataPoint>();
        bool flag1 = false;
        DateTime currentDate = cumulativeFlowDiagramInputs.EndDate;
        bool flag2 = true;
        for (; currentDate >= cumulativeFlowDiagramInputs.StartDate && !flag1; currentDate = this.GetPreviousFirstWorkingDay(currentDate, cumulativeFlowDiagramInputs.FirstWorkDay))
        {
          currentDate = DateTime.SpecifyKind(currentDate, DateTimeKind.Unspecified);
          CumulativeFlowDiagramDataPoint diagramDataPoint = (CumulativeFlowDiagramDataPoint) null;
          if (cachedFlowDiagramData != null)
            diagramDataPoint = cachedFlowDiagramData.FirstOrDefault<CumulativeFlowDiagramDataPoint>((System.Func<CumulativeFlowDiagramDataPoint, bool>) (o => o.Date == currentDate));
          if (diagramDataPoint == null)
          {
            diagramDataPoint = new CumulativeFlowDiagramDataPoint()
            {
              Date = currentDate,
              StateCounts = ChartDataComponent.MakeZeroedStateCounts((IEnumerable<string>) cumulativeFlowDiagramInputs.WorkItemStates)
            };
            DateTime dateTime = currentDate.GetEndOfDayUtc(cumulativeFlowDiagramInputs.TimeZone);
            if (flag2)
            {
              flag2 = false;
              if (cumulativeFlowDiagramInputs.UseActualEndDateTime)
                dateTime = TimeZoneInfo.ConvertTime(currentDate, cumulativeFlowDiagramInputs.TimeZone, TimeZoneInfo.Utc);
            }
            wiqlQuery.AsOf = new DateTime?(dateTime);
            IWorkItemQueryService service1 = vssRequestContext.GetService<IWorkItemQueryService>();
            ITeamFoundationWorkItemService service2 = vssRequestContext.GetService<ITeamFoundationWorkItemService>();
            IVssRequestContext requestContext1 = vssRequestContext;
            string wiql = wiqlQuery.ToString();
            Guid? projectId = new Guid?();
            List<int> list = service1.ExecuteQuery(requestContext1, wiql, projectId: projectId).WorkItemIds.ToList<int>();
            foreach (IGrouping<string, WorkItemFieldData> source in service2.GetWorkItemFieldValues(vssRequestContext, (IEnumerable<int>) list, (IEnumerable<string>) new string[2]
            {
              "System.Id",
              "System.State"
            }, asOf: new DateTime?(dateTime)).GroupBy<WorkItemFieldData, string>((System.Func<WorkItemFieldData, string>) (qd => qd.State)))
            {
              string key = source.Key;
              if (diagramDataPoint.StateCounts.ContainsKey(key))
                diagramDataPoint.StateCounts[key] = source.Count<WorkItemFieldData>();
            }
          }
          flag1 = !diagramDataPoint.StateCounts.Values.Any<int>((System.Func<int, bool>) (o => o > 0));
          results.Add(diagramDataPoint);
        }
        this.SetCachedFlowDiagramData(vssRequestContext, cumulativeFlowDiagramInputs, chartCache, results);
        return (IList<CumulativeFlowDiagramDataPoint>) results;
      }
      catch (LegacyValidationException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        return (IList<CumulativeFlowDiagramDataPoint>) Array.Empty<CumulativeFlowDiagramDataPoint>();
      }
    }

    private DateTime GetPreviousFirstWorkingDay(
      DateTime currentDate,
      DayOfWeek targetDay,
      bool includeCurrentDate = false)
    {
      if (includeCurrentDate && currentDate.Date.DayOfWeek == targetDay)
        return currentDate.Date;
      do
      {
        currentDate = currentDate.Date.AddDays(-1.0);
      }
      while (currentDate.DayOfWeek != targetDay);
      return currentDate;
    }

    private static Dictionary<string, int> MakeZeroedStateCounts(IEnumerable<string> acceptedStates)
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
      foreach (string acceptedState in acceptedStates)
        dictionary[acceptedState] = 0;
      return dictionary;
    }

    private string MakeFlowDiagramName(CumulativeFlowDiagramInputs diagramInputs) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CumulativeFlowDiagram." + diagramInputs.TimeZone.StandardName);

    private string MakeBurndownChartName(BurndownChartInputs diagramInputs) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "BurndownChart." + diagramInputs.TimeZone.StandardName);

    private string SerializeFilterState(CumulativeFlowDiagramInputs diagramInputs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (FieldValue filterFieldValue in diagramInputs.FilterFieldValues)
      {
        stringBuilder.Append(filterFieldValue.IncludeHierarchy.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        stringBuilder.Append(filterFieldValue.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      return stringBuilder.ToString();
    }

    private void SetCachedFlowDiagramData(
      IVssRequestContext requestContext,
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs,
      IdentityChartCache chartCache,
      List<CumulativeFlowDiagramDataPoint> results)
    {
      try
      {
        if (results.Count <= 1 || chartCache == null)
          return;
        CumulativeFlowChartSeries cumulativeFlowChartSeries = new CumulativeFlowChartSeries();
        cumulativeFlowChartSeries.StateTimeSeries = new Dictionary<string, List<int>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        cumulativeFlowChartSeries.FirstWorkDay = cumulativeFlowDiagramInputs.FirstWorkDay;
        foreach (string workItemState in cumulativeFlowDiagramInputs.WorkItemStates)
          cumulativeFlowChartSeries.StateTimeSeries[workItemState] = new List<int>();
        cumulativeFlowChartSeries.EndDate = results[1].Date;
        for (int index = 1; index < results.Count; ++index)
        {
          CumulativeFlowDiagramDataPoint result = results[index];
          foreach (string workItemState in cumulativeFlowDiagramInputs.WorkItemStates)
            cumulativeFlowChartSeries.StateTimeSeries[workItemState].Add(result.StateCounts[workItemState]);
        }
        cumulativeFlowChartSeries.FilterState = this.SerializeFilterState(cumulativeFlowDiagramInputs);
        string str = cumulativeFlowChartSeries.Serialize();
        string entryName = this.MakeFlowDiagramName(cumulativeFlowDiagramInputs);
        string chartCacheData = chartCache.GetChartCacheData(entryName);
        if (chartCacheData != null && string.Equals(chartCacheData, str, StringComparison.Ordinal))
          return;
        chartCache.SetChartCacheItem(entryName, str);
        chartCache.Update(requestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, nameof (SetCachedFlowDiagramData), ex);
      }
    }

    private List<CumulativeFlowDiagramDataPoint> GetCachedFlowDiagramData(
      CumulativeFlowDiagramInputs cumulativeFlowDiagramInputs,
      IdentityChartCache chartCache)
    {
      List<CumulativeFlowDiagramDataPoint> cachedFlowDiagramData = new List<CumulativeFlowDiagramDataPoint>();
      if (chartCache == null)
        return cachedFlowDiagramData;
      try
      {
        string entryName = this.MakeFlowDiagramName(cumulativeFlowDiagramInputs);
        string chartCacheData = chartCache.GetChartCacheData(entryName);
        if (!string.IsNullOrWhiteSpace(chartCacheData))
        {
          CumulativeFlowChartSeries cumulativeFlowChartSeries = ChartSeriesBase.Deserialize<CumulativeFlowChartSeries>(chartCacheData);
          string x = this.SerializeFilterState(cumulativeFlowDiagramInputs);
          if (cumulativeFlowChartSeries != null)
          {
            if (StringComparer.InvariantCulture.Equals(x, cumulativeFlowChartSeries.FilterState))
            {
              if (cumulativeFlowChartSeries.CacheVersion == 1)
              {
                if (cumulativeFlowChartSeries.FirstWorkDay == cumulativeFlowDiagramInputs.FirstWorkDay)
                {
                  DateTime dateTime = cumulativeFlowChartSeries.EndDate;
                  int count = cumulativeFlowChartSeries.StateTimeSeries.First<KeyValuePair<string, List<int>>>().Value.Count;
                  for (int index = 0; index < count; ++index)
                  {
                    CumulativeFlowDiagramDataPoint diagramDataPoint = new CumulativeFlowDiagramDataPoint()
                    {
                      Date = dateTime,
                      StateCounts = ChartDataComponent.MakeZeroedStateCounts((IEnumerable<string>) cumulativeFlowDiagramInputs.WorkItemStates)
                    };
                    foreach (string workItemState in cumulativeFlowDiagramInputs.WorkItemStates)
                      diagramDataPoint.StateCounts[workItemState] = cumulativeFlowChartSeries.StateTimeSeries[workItemState][index];
                    cachedFlowDiagramData.Add(diagramDataPoint);
                    dateTime = dateTime.AddDays(-7.0);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, nameof (GetCachedFlowDiagramData), ex);
      }
      return cachedFlowDiagramData;
    }

    private void SetCachedBurndownData(
      IVssRequestContext requestContext,
      BurndownChartInputs burndownChartInputs,
      List<BurndownChartDataPoint> results,
      string queryText,
      DateTime today)
    {
      using (requestContext.TraceBlock(15191010, 15191012, "WebAccess", TfsTraceLayers.BusinessLogic, nameof (SetCachedBurndownData)))
      {
        try
        {
          if (burndownChartInputs.IdentityChartCache == null)
            return;
          BurndownChartSeries burndownChartSeries = new BurndownChartSeries();
          DateTime? nullable = new DateTime?();
          burndownChartSeries.RemainingWorkSeries = new List<double?>();
          burndownChartSeries.QueryText = queryText;
          for (int index = results.Count - 1; index >= 0; --index)
          {
            BurndownChartDataPoint result = results[index];
            if (result.Date < today)
            {
              burndownChartSeries.RemainingWorkSeries.Add(result.RemainingWork);
              if (!nullable.HasValue)
                nullable = new DateTime?(result.Date);
            }
          }
          burndownChartSeries.EndDate = nullable.HasValue ? nullable.Value : burndownChartInputs.Iteration.StartDate.Value;
          string str = burndownChartSeries.Serialize();
          string entryName = this.MakeBurndownChartName(burndownChartInputs);
          string chartCacheData = burndownChartInputs.IdentityChartCache.GetChartCacheData(entryName);
          if (chartCacheData != null && string.Equals(chartCacheData, str, StringComparison.Ordinal))
            return;
          burndownChartInputs.IdentityChartCache.SetChartCacheItem(entryName, str);
          burndownChartInputs.IdentityChartCache.Update(requestContext);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15191011, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, ex);
        }
      }
    }

    private List<BurndownChartDataPoint> GetCachedBurndownData(
      IVssRequestContext requestContext,
      BurndownChartInputs burndownChartInputs,
      string queryText)
    {
      List<BurndownChartDataPoint> cachedBurndownData = new List<BurndownChartDataPoint>();
      if (burndownChartInputs.IdentityChartCache == null)
        return cachedBurndownData;
      try
      {
        string entryName = this.MakeBurndownChartName(burndownChartInputs);
        string chartCacheData = burndownChartInputs.IdentityChartCache.GetChartCacheData(entryName);
        requestContext.Trace(15191000, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, string.Format("Looking for cached Data found for {0}", (object) burndownChartInputs.IdentityChartCache.Identity.Id));
        if (!string.IsNullOrWhiteSpace(chartCacheData))
        {
          requestContext.Trace(15191001, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, "Cached Data found for " + entryName);
          BurndownChartSeries burndownChartSeries = ChartSeriesBase.Deserialize<BurndownChartSeries>(chartCacheData);
          if (burndownChartSeries != null && TFStringComparer.WorkItemQueryText.Equals(burndownChartSeries.QueryText, queryText) && burndownChartSeries.CacheVersion == 2)
          {
            DateTime dateTime = burndownChartSeries.EndDate;
            int count = burndownChartSeries.RemainingWorkSeries.Count;
            for (int index = 0; index < count; ++index)
            {
              BurndownChartDataPoint burndownChartDataPoint = new BurndownChartDataPoint()
              {
                Date = dateTime,
                RemainingWork = burndownChartSeries.RemainingWorkSeries[index]
              };
              cachedBurndownData.Add(burndownChartDataPoint);
              dateTime = dateTime.AddDays(-1.0);
            }
          }
          else if (burndownChartSeries != null)
          {
            if (TFStringComparer.WorkItemQueryText.Equals(burndownChartSeries.QueryText, queryText))
            {
              if (burndownChartSeries.CacheVersion == 2)
                goto label_13;
            }
            string message = string.Format("CachedQuery: {0}, InputQuery: {1}, CachedVersion: {2}, CurrentVersion: {3}", (object) burndownChartSeries.QueryText, (object) queryText, (object) burndownChartSeries.CacheVersion, (object) 2);
            requestContext.Trace(15191002, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, message);
          }
        }
        else
          requestContext.Trace(15191003, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, "No cached Data for " + entryName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15191004, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, ex);
      }
label_13:
      return cachedBurndownData;
    }

    internal static void CalculateBurndownTrendlines(
      BurndownChartInputs chartInputs,
      List<BurndownChartDataPoint> results)
    {
      if (results.Count <= 1 || !results[0].RemainingWork.HasValue)
        return;
      IEnumerable<BurndownChartDataPoint> source = results.TakeWhile<BurndownChartDataPoint>((System.Func<BurndownChartDataPoint, bool>) (result => result.RemainingWork.HasValue));
      double num1 = source.Sum<BurndownChartDataPoint>((Func<BurndownChartDataPoint, BurndownChartDataPoint, double>) ((previous, current) =>
      {
        double? remainingWork = current.RemainingWork;
        double num2 = remainingWork.Value;
        remainingWork = previous.RemainingWork;
        double num3 = remainingWork.Value;
        return Math.Max(num2 - num3, 0.0);
      }));
      double num4 = results[0].RemainingWork.Value + num1;
      int index1 = source.Count<BurndownChartDataPoint>() - 1;
      double num5 = (double) (chartInputs.GetWorkingDays().Count - 1);
      double num6 = num4 / num5;
      double num7 = (num4 - results[index1].RemainingWork.Value) / (double) index1;
      results[0].IdealTrend = num4;
      results[0].ActualTrend = num4;
      for (int index2 = 1; index2 < results.Count; ++index2)
      {
        results[index2].IdealTrend = results[index2 - 1].IdealTrend - num6;
        results[index2].ActualTrend = results[index2 - 1].ActualTrend - num7;
      }
    }

    internal static void CalculateAvailableCapacity(
      BurndownChartInputs chartInputs,
      IEnumerable<BurndownChartDataPoint> results)
    {
      if (chartInputs.TeamMemberCapacityCollection == null)
        return;
      double num = 0.0;
      foreach (BurndownChartDataPoint burndownChartDataPoint in results.Reverse<BurndownChartDataPoint>())
      {
        DateTime date = burndownChartDataPoint.Date;
        burndownChartDataPoint.AvailableCapacity = num;
        if (!chartInputs.IsNonWorkingDay(date))
        {
          foreach (TeamMemberCapacity teamMemberCapacity in chartInputs.TeamMemberCapacityCollection)
          {
            if (!BurndownChartInputs.IsDayInRange(teamMemberCapacity.DaysOffDates, date))
              num += (double) teamMemberCapacity.Activities.Sum<Activity>((System.Func<Activity, float>) (a => a.CapacityPerDay));
          }
        }
      }
    }

    private string GetIterationDisplayName(string iterationPath)
    {
      string[] strArray = iterationPath.Split(new char[1]
      {
        '\\'
      }, StringSplitOptions.RemoveEmptyEntries);
      return strArray.Length != 0 ? strArray[strArray.Length - 1] : iterationPath;
    }
  }
}
