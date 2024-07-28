// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQuality.DataQualityService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.DataQuality
{
  public class DataQualityService : IDataQualityService, IVssFrameworkService
  {
    private static readonly string s_traceLayer = nameof (DataQualityService);
    private static readonly string KpiMetricsArea = "Microsoft.VisualStudio.AnalyticsDataQualityMetrics";
    private const int MinimumTestIntervalSeconds = 900;
    public const int ShortModelReadinessJobIntervalSeconds = 30;
    private const string LatestDataQualityCacheKey = "LatestDataQualityResult";
    private const string ScheduleRegistryPathFolder = "/Service/Analytics/Settings/Quality/LastRunDate/";
    private const string PreviousEndDateRegistryPathFolder = "/Service/Analytics/Settings/Quality/PreviousEndDate/";
    private const string DisabledRegistryPathFolder = "/Service/Analytics/Settings/Quality/Disabled/";
    private const string LatencyExclusionSecondsRegistryPath = "/Service/Analytics/Settings/Quality/LatencyExclusionSeconds";
    private const string LastDataChangedDatePath = "/Service/Analytics/Settings/Quality/LastDataChangedDate";
    private const string RetainHistoryDaysRegistryPath = "/Service/Analytics/Settings/Quality/RetainHistoryDays";
    private static readonly RegistryQuery s_ScheduleQuery = new RegistryQuery("/Service/Analytics/Settings/Quality/LastRunDate/**");
    private static readonly RegistryQuery s_PreviousEndQuery = new RegistryQuery("/Service/Analytics/Settings/Quality/PreviousEndDate/**");
    private static readonly RegistryQuery s_LatencyExclusionSecondsQuery = new RegistryQuery("/Service/Analytics/Settings/Quality/LatencyExclusionSeconds");
    private static readonly RegistryQuery s_LastDataChangedDateQuery = new RegistryQuery("/Service/Analytics/Settings/Quality/LastDataChangedDate");
    private static readonly RegistryQuery s_RetainHistoryDaysQuery = new RegistryQuery("/Service/Analytics/Settings/Quality/RetainHistoryDays");
    internal const int retainHistoryDaysDefault = 180;

    public IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      IVssRequestContext requestContext,
      DataQualityDefinition definition,
      int latencyExclusionSeconds,
      DateTime previousEndDate)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      IKpiService kpiService = (IKpiService) null;
      if (definition.KpiName != null && vssRequestContext.ExecutionEnvironment.IsHostedDeployment)
        kpiService = vssRequestContext.GetService<IKpiService>();
      try
      {
        service.SetValue<string>(requestContext, "/Service/Analytics/Settings/Quality/LastRunDate/" + definition.SprocName, DateTime.UtcNow.ToString());
        IReadOnlyCollection<DataQualityResult> dataQualityResults;
        using (DataQualityComponent component = requestContext.CreateComponent<DataQualityComponent>())
          dataQualityResults = component.CheckDataQuality(definition, latencyExclusionSeconds, previousEndDate);
        if (dataQualityResults != null)
        {
          if (dataQualityResults.Any<DataQualityResult>((Func<DataQualityResult, bool>) (r =>
          {
            DateTime endDate = r.EndDate;
            return true;
          })))
            service.SetValue<string>(requestContext, "/Service/Analytics/Settings/Quality/PreviousEndDate/" + definition.SprocName, dataQualityResults.Max<DataQualityResult, DateTime>((Func<DataQualityResult, DateTime>) (r => r.EndDate)).ToString());
          if (kpiService != null)
          {
            foreach (DataQualityResult dataQualityResult in (IEnumerable<DataQualityResult>) dataQualityResults)
            {
              kpiService.EnsureKpiIsRegistered(vssRequestContext, DataQualityService.KpiMetricsArea, definition.KpiName, dataQualityResult.Scope ?? dataQualityResult.TargetTable, definition.KpiDisplayName, definition.KpiDescription);
              KpiMetric metric = new KpiMetric()
              {
                Name = definition.KpiName,
                Value = dataQualityResult.KpiValue,
                TimeStamp = dataQualityResult.EndDate
              };
              kpiService.Publish(vssRequestContext, DataQualityService.KpiMetricsArea, requestContext.ServiceHost.InstanceId, dataQualityResult.Scope ?? dataQualityResult.TargetTable, metric);
            }
          }
        }
        this.WriteOnPremTelemetry(requestContext, (IEnumerable<DataQualityResult>) dataQualityResults);
        return dataQualityResults;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12014001, "AnalyticsDataQuality", DataQualityService.s_traceLayer, ex);
        throw;
      }
    }

    internal void HandleModelReady(
      IVssRequestContext requestContext,
      Dictionary<string, IEnumerable<DataQualityResult>> latestDQResults)
    {
      if (!this.IsModelReady(requestContext, latestDQResults))
        return;
      requestContext.Trace(12014002, TraceLevel.Info, "AnalyticsDataQuality", DataQualityService.s_traceLayer, "Model is ready. Setting the model ready flag...");
      requestContext.GetService<AnalyticsService>().SetModelReady(requestContext, true);
    }

    internal bool IsModelReady(
      IVssRequestContext requestContext,
      Dictionary<string, IEnumerable<DataQualityResult>> latestDataQualityResults)
    {
      IEnumerable<DataQualityResult> source;
      return !latestDataQualityResults.TryGetValue("ModelReady", out source) || !source.Any<DataQualityResult>((Func<DataQualityResult, bool>) (x => x.Failed));
    }

    public IReadOnlyCollection<DataQualityResult> CheckDataQuality(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityDefinition> definitions = null)
    {
      IAnalyticsService service1 = requestContext.GetService<IAnalyticsService>();
      DateTime utcNow = DateTime.UtcNow;
      DateTime dateTime1 = DateTime.MinValue;
      List<DataQualityResult> dataQualityResultList = new List<DataQualityResult>();
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      DateTime dateTime2 = service2.GetValue<DateTime>(requestContext, in DataQualityService.s_LastDataChangedDateQuery, utcNow);
      Dictionary<string, string> dictionary1 = service2.Read(requestContext, in DataQualityService.s_ScheduleQuery).ToDictionary<RegistryItem, string, string>((Func<RegistryItem, string>) (ri => ri.Path), (Func<RegistryItem, string>) (ri => ri.Value));
      Dictionary<string, string> dictionary2 = service2.Read(requestContext, in DataQualityService.s_PreviousEndQuery).ToDictionary<RegistryItem, string, string>((Func<RegistryItem, string>) (ri => ri.Path), (Func<RegistryItem, string>) (ri => ri.Value));
      int latencyExclusionSeconds = service2.GetValue<int>(requestContext, in DataQualityService.s_LatencyExclusionSecondsQuery, 3600);
      IVssRequestContext requestContext1 = requestContext;
      bool flag = service1.IsModelReady(requestContext1);
      HostProperties hostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(requestContext, requestContext.ServiceHost.InstanceId);
      Dictionary<string, IEnumerable<DataQualityResult>> latestDQResults = new Dictionary<string, IEnumerable<DataQualityResult>>();
      IEnumerable<DataQualityDefinition> source = (IEnumerable<DataQualityDefinition>) ((object) definitions ?? (object) DataQualityDefinitions.All);
      if (flag || AnalyticsFeatureService.SubStatusPreventsStaging(hostProperties.SubStatus))
      {
        source = source.Where<DataQualityDefinition>((Func<DataQualityDefinition, bool>) (x => (x.Criterias & DataQualityCriterias.ModelReady) == DataQualityCriterias.None));
      }
      else
      {
        latestDQResults = this.GetLatestDataQualityResultsByDefinition(requestContext);
        if (!latestDQResults.ContainsKey("ModelReady") || latestDQResults["ModelReady"].All<DataQualityResult>((Func<DataQualityResult, bool>) (x => !x.Failed)))
          source = source.Where<DataQualityDefinition>((Func<DataQualityDefinition, bool>) (x => (x.Criterias & DataQualityCriterias.ModelReady) == DataQualityCriterias.None));
      }
      List<Exception> exceptionList = new List<Exception>();
      foreach (DataQualityDefinition definition in source)
      {
        if (!service2.GetValue<bool>(requestContext, (RegistryQuery) ("/Service/Analytics/Settings/Quality/Disabled/" + definition.SprocName), true, false) && (definition.FeatureFlagToRun == null ? 0 : (!requestContext.IsFeatureEnabled(definition.FeatureFlagToRun) ? 1 : 0)) == 0)
        {
          DateTime result1 = DateTime.MinValue;
          string s1;
          if (dictionary1.TryGetValue("/Service/Analytics/Settings/Quality/LastRunDate/" + definition.SprocName, out s1))
            DateTime.TryParse(s1, out result1);
          if (result1 == DateTime.MinValue && definition.MinimumIntervalSeconds > 900)
          {
            result1 = utcNow - TimeSpan.FromSeconds((double) ((requestContext.ServiceHost.CollectionServiceHost.InstanceId.GetHashCode() % definition.MinimumIntervalSeconds + definition.MinimumIntervalSeconds) % definition.MinimumIntervalSeconds));
            service2.SetValue<string>(requestContext, "/Service/Analytics/Settings/Quality/LastRunDate/" + definition.SprocName, result1.ToString());
          }
          DateTime dateTime3 = result1.AddSeconds((double) definition.MinimumIntervalSeconds);
          if (dateTime3 < utcNow && result1 < dateTime2.AddSeconds((double) latencyExclusionSeconds))
          {
            DateTime result2 = Microsoft.VisualStudio.Services.Analytics.Constants.BeginningOfTimeDateTime;
            string s2;
            if (dictionary2.TryGetValue("/Service/Analytics/Settings/Quality/PreviousEndDate/" + definition.SprocName, out s2))
              DateTime.TryParse(s2, out result2);
            try
            {
              IReadOnlyCollection<DataQualityResult> collection = this.CheckDataQuality(requestContext, definition, latencyExclusionSeconds, result2);
              if (collection != null)
              {
                dataQualityResultList.AddRange((IEnumerable<DataQualityResult>) collection);
                if (!flag)
                {
                  latestDQResults[definition.Name] = (IEnumerable<DataQualityResult>) collection;
                  this.HandleModelReady(requestContext, latestDQResults);
                }
              }
            }
            catch (Exception ex)
            {
              exceptionList.Add(new Exception("Exception while procesing " + definition.Name, ex));
            }
          }
          else if (result1 < dateTime2 && dateTime3 > dateTime1)
            dateTime1 = dateTime3;
        }
      }
      if (dateTime1 > DateTime.MinValue)
      {
        ITeamFoundationJobService service3 = requestContext.GetService<ITeamFoundationJobService>();
        int num = Math.Max((int) (dateTime1 - utcNow).TotalSeconds, 900);
        IVssRequestContext requestContext2 = requestContext;
        Guid[] jobIds = new Guid[1]
        {
          Microsoft.VisualStudio.Services.Analytics.Constants.AnalyticsDataQualityJobId
        };
        int maxDelaySeconds = num;
        service3.QueueDelayedJobs(requestContext2, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
      }
      if (exceptionList.Any<Exception>())
        throw new AggregateException(AnalyticsResources.AGGREGATE_EXCEPTIONS_WITH_2_ARGUMENTS((object) exceptionList.Count, (object) "DataQualityJob"), (IEnumerable<Exception>) exceptionList);
      return (IReadOnlyCollection<DataQualityResult>) dataQualityResultList;
    }

    public IReadOnlyCollection<DataQualityResult> GetCachedLatestDataQualityResults(
      IVssRequestContext requestContext)
    {
      DataQualityCacheService service = requestContext.GetService<DataQualityCacheService>();
      IReadOnlyCollection<DataQualityResult> dataQualityResults;
      if (!service.TryGetValue(requestContext, "LatestDataQualityResult", out dataQualityResults))
      {
        requestContext.Trace(12014003, TraceLevel.Info, DataQualityService.Area, DataQualityService.Layer, "GetCachedLatestDataQualityResults - fetched from database.");
        dataQualityResults = this.GetLatestDataQualityResults(requestContext);
        service.Set(requestContext, "LatestDataQualityResult", dataQualityResults);
      }
      else
        requestContext.Trace(12014003, TraceLevel.Info, DataQualityService.Area, DataQualityService.Layer, "GetCachedLatestDataQualityResults - fetched from cache.");
      return dataQualityResults;
    }

    public void ExpireCachedLatestDataQualityResults(IVssRequestContext requestContext) => requestContext.GetService<DataQualityCacheService>().Remove(requestContext, "LatestDataQualityResult");

    public IReadOnlyCollection<DataQualityResult> GetLatestDataQualityResults(
      IVssRequestContext requestContext)
    {
      using (DataQualityComponent component = requestContext.CreateComponent<DataQualityComponent>())
        return component.GetLatestDataQualityResults();
    }

    public Dictionary<string, IEnumerable<DataQualityResult>> GetLatestDataQualityResultsByDefinition(
      IVssRequestContext requestContext)
    {
      return this.GetLatestDataQualityResults(requestContext).GroupBy<DataQualityResult, string>((Func<DataQualityResult, string>) (x => x.Name)).ToDictionary<IGrouping<string, DataQualityResult>, string, IEnumerable<DataQualityResult>>((Func<IGrouping<string, DataQualityResult>, string>) (x => x.Key), (Func<IGrouping<string, DataQualityResult>, IEnumerable<DataQualityResult>>) (x => x.AsEnumerable<DataQualityResult>()));
    }

    public IReadOnlyCollection<string> GetDataQualityWarnings(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityResult> dataQualityResults)
    {
      List<string> dataQualityWarnings = new List<string>();
      foreach (DataQualityResult dataQualityResult in dataQualityResults)
      {
        DataQualityDefinition qualityDefinition = DataQualityDefinitions.DataQualityNameToDefinitionDict[dataQualityResult.Name];
        if (dataQualityResult.Failed && qualityDefinition.ShouldExposeWarning)
          dataQualityWarnings.Add(qualityDefinition.WarningMessage(dataQualityResult));
      }
      return (IReadOnlyCollection<string>) dataQualityWarnings;
    }

    public void NotifyDataChange(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      DateTime utcNow = DateTime.UtcNow;
      if (service.GetValue<DateTime>(requestContext, in DataQualityService.s_LastDataChangedDateQuery, DateTime.MinValue).AddSeconds((double) (service.GetValue<int>(requestContext, in DataQualityService.s_LatencyExclusionSecondsQuery, 3600) / 2)) < utcNow)
        service.SetValue<DateTime>(requestContext, "/Service/Analytics/Settings/Quality/LastDataChangedDate", utcNow);
      int maxDelaySeconds = 900;
      if (!requestContext.GetService<AnalyticsService>().IsModelReady(requestContext))
        maxDelaySeconds = 30;
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        Microsoft.VisualStudio.Services.Analytics.Constants.AnalyticsDataQualityJobId
      }, maxDelaySeconds);
    }

    public CleanupDataQualityResult CleanupDataQualityResults(IVssRequestContext requestContext)
    {
      int retainHistoryDays = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in DataQualityService.s_RetainHistoryDaysQuery, 180);
      using (DataQualityComponent component = requestContext.CreateComponent<DataQualityComponent>())
        return component.CleanupDataQualityResults(retainHistoryDays);
    }

    private void WriteOnPremTelemetry(
      IVssRequestContext requestContext,
      IEnumerable<DataQualityResult> results)
    {
      IAnalyticsOnPremTelemetryService service = requestContext.GetService<IAnalyticsOnPremTelemetryService>();
      if (results == null)
        return;
      foreach (DataQualityResult result in results)
        service.SetDataQualityResult(requestContext, result);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public static string Area => "AnalyticsDataQuality";

    public static string Layer => "AnalyticsService";

    public void InitializeModelReady(IVssRequestContext requestContext)
    {
      using (DataQualityComponent component = requestContext.CreateComponent<DataQualityComponent>())
        component.InitializeModelReady();
    }
  }
}
