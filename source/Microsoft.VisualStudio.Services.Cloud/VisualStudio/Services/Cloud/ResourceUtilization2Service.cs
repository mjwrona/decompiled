// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceUtilization2Service
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ResourceUtilization2Service : 
    VssBaseService,
    IResourceUtilizationService,
    IVssFrameworkService
  {
    private IDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>> m_resourceCache;
    private IReadOnlyDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>> m_localResourceCache;
    private ConcurrentQueue<ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>> m_concurrentCountersToCleanUp;
    private int m_ruleThresholdVersion;
    private int m_deploymentLevelRulesAndThresholdsVersion = -1;
    private RulesAndThresholds m_rulesAndThresholds;
    private ConcurrentStack<ResourceUtilization2Service.IncrementResourceTaskArgs> m_queuedTaskArgs;
    private Random m_randomizer;
    private INotificationRegistration m_stateChangedRegistration;
    private INotificationRegistration m_rulesOrThresholdsChangedRegistration;
    private ILockName m_scheduleServiceTasksLock;
    private ResourceUtilization2Service m_deploymentService;
    private ResourceUtilization2Service.ServiceSettings m_serviceSettings;
    private Guid m_serviceSettingsVersion;
    internal static readonly RegistryQuery s_serviceSettingsRegistryQuery = (RegistryQuery) (ResourceUtilizationConstants.ServiceSettingsRegistryRoot + "*");
    internal const string c_area = "ResourceUtilizationService";
    internal const string c_layer = "Service";
    private const string c_delayTime = "DelayTime";
    private const string c_block = "Block";
    private const string c_concurrencySemaphoreTime = "ConcurrencySemaphoreTime";
    private const string c_fwlinkId = "823950";
    private const int c_windowSeconds = 300;
    private static readonly VssPerformanceCounter m_perfCounter_AverageQueueSize = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageQueueSize", "ResourceUtilizationService");
    private static readonly VssPerformanceCounter m_perfCounter_AverageQueueSizeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageQueueSizeBase", "ResourceUtilizationService");
    private static readonly VssPerformanceCounter m_perfCounter_AverageTaskExecutionTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageTaskExecutionTime", "ResourceUtilizationService");
    private static readonly VssPerformanceCounter m_perfCounter_AverageTaskExecutionTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageTaskExecutionTimeBase", "ResourceUtilizationService");
    private static readonly VssPerformanceCounter m_perfCounter_ServiceCallsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_ServiceCallsPerSec", "ResourceUtilizationService");

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_serviceSettingsVersion = Guid.Empty;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_deploymentService = (ResourceUtilization2Service) null;
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, in ResourceUtilization2Service.s_serviceSettingsRegistryQuery);
        Interlocked.CompareExchange<ResourceUtilization2Service.ServiceSettings>(ref this.m_serviceSettings, new ResourceUtilization2Service.ServiceSettings(requestContext), (ResourceUtilization2Service.ServiceSettings) null);
      }
      else
        this.m_deploymentService = requestContext.To(TeamFoundationHostType.Deployment).GetService<ResourceUtilization2Service>();
      this.m_randomizer = new Random();
      this.RedisCachingService = requestContext.GetService<IRedisCacheService>();
      this.m_queuedTaskArgs = new ConcurrentStack<ResourceUtilization2Service.IncrementResourceTaskArgs>();
      this.m_concurrentCountersToCleanUp = new ConcurrentQueue<ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>();
      this.m_scheduleServiceTasksLock = this.CreateLockName(requestContext, "scheduleServiceTasks");
      this.m_rulesAndThresholds = new RulesAndThresholds(requestContext, -1);
      this.ScheduleServiceTasks(requestContext);
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_stateChangedRegistration = service.CreateRegistration(requestContext, "Default", ResourceUtilizationConstants.ResourceStateChanged, new SqlNotificationCallback(this.OnResourceStateChanged), true, false);
      this.m_rulesOrThresholdsChangedRegistration = service.CreateRegistration(requestContext, "Default", ResourceUtilizationConstants.RulesOrThresholdsChanged, new SqlNotificationCallback(this.OnRulesOrThresholdsChanged), true, false);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_stateChangedRegistration.Unregister(requestContext);
      this.m_rulesOrThresholdsChangedRegistration.Unregister(requestContext);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      service.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.IncrementResourcesTask));
      service.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.CleanupCacheTask));
    }

    public async Task ThrottleRequestAsync(IVssRequestContext requestContext, RUStage ruStage)
    {
      ResourceUtilization2Service utilization2Service = this;
      ThrottleInfo throttleInfo;
      IList<IRequestResourceCounter> requestCounters;
      if (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.BypassTarpitting))
      {
        throttleInfo = (ThrottleInfo) null;
        requestCounters = (IList<IRequestResourceCounter>) null;
      }
      else
      {
        bool flag = false;
        throttleInfo = (ThrottleInfo) null;
        requestCounters = (IList<IRequestResourceCounter>) null;
        if (ruStage == RUStage.RequestReady)
        {
          if (utilization2Service.EnsureRulesAndThresholdsSynced(requestContext))
          {
            Dictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>> temp = new Dictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>>();
            Dictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>> dictionary = new Dictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>();
            foreach (KeyValuePair<Guid, ProcessedRURule> processedRule in utilization2Service.m_rulesAndThresholds.ProcessedRules)
            {
              if (processedRule.Value.Resource is RUResource_Concurrency)
                dictionary[processedRule.Key] = new ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              else
                temp[processedRule.Key] = new ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            }
            utilization2Service.m_localResourceCache = (IReadOnlyDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>) new ReadOnlyDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>((IDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>) dictionary);
            if (utilization2Service.m_resourceCache != null)
            {
              foreach (Guid key in utilization2Service.m_resourceCache.Keys.Where<Guid>((Func<Guid, bool>) (x => !temp.Keys.Contains<Guid>(x))))
                utilization2Service.m_resourceCache.Remove(key);
              // ISSUE: reference to a compiler-generated method
              foreach (Guid key in temp.Keys.Where<Guid>(new Func<Guid, bool>(utilization2Service.\u003CThrottleRequestAsync\u003Eb__2_1)))
                utilization2Service.m_resourceCache.Add(key, new ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
            }
            else
              utilization2Service.m_resourceCache = (IDictionary<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>>) temp;
          }
          utilization2Service.EnsureSettingsSynced(requestContext);
          utilization2Service.CheckRequestThrottleInfo(requestContext, ruStage, ref throttleInfo);
          utilization2Service.IncrementResources(requestContext, ruStage, ref throttleInfo, out requestCounters);
        }
        else
        {
          if (ruStage != RUStage.PostLog)
            throw new VssServiceException("RUStage not supported; this is a code error");
          long requestExecutionTime_ms = Math.Max(1L, requestContext.ExecutionTime() / 1000L);
          ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext)["ExecutionTime"] = (object) requestExecutionTime_ms;
          double totalMilliseconds = requestContext.RequestTimer.DelaySpan.TotalMilliseconds;
          flag = requestContext.RootContext.TryGetItem<ThrottleInfo>(RequestContextItemsKeys.ThrottleInfo2, out throttleInfo);
          utilization2Service.CheckRequestThrottleInfo(requestContext, ruStage, ref throttleInfo);
          utilization2Service.AdjustAndCapPostRequestDelay(requestContext, (int) totalMilliseconds, requestExecutionTime_ms, ref throttleInfo);
        }
        if (throttleInfo != null && !utilization2Service.Settings.DoNotTarpit)
        {
          if (!flag)
            requestContext.RootContext.Items[RequestContextItemsKeys.ThrottleInfo2] = (object) throttleInfo;
          if ((throttleInfo.ThrottleType2 & ResourceState2.Tarpit) > ResourceState2.Normal)
            await utilization2Service.TarpitRequest(requestContext, throttleInfo, ruStage);
        }
        if (ruStage != RUStage.RequestReady)
        {
          throttleInfo = (ThrottleInfo) null;
          requestCounters = (IList<IRequestResourceCounter>) null;
        }
        else if (throttleInfo != null && (throttleInfo.ThrottleType2 & ResourceState2.Block) != ResourceState2.Normal)
        {
          throttleInfo = (ThrottleInfo) null;
          requestCounters = (IList<IRequestResourceCounter>) null;
        }
        else
        {
          SemaphoreWaitStatus[] source = await utilization2Service.AwaitRequestCounters(requestContext, requestCounters);
          if (source.Length == 0)
          {
            throttleInfo = (ThrottleInfo) null;
            requestCounters = (IList<IRequestResourceCounter>) null;
          }
          else
          {
            utilization2Service.AddTimeSpentInConcurrencySemaphore(requestContext, ((IEnumerable<SemaphoreWaitStatus>) source).Max<SemaphoreWaitStatus>((Func<SemaphoreWaitStatus, long>) (s => s.TicksSpentInConcurrencySemaphore)));
            int index = ((IEnumerable<SemaphoreWaitStatus>) source).TakeWhile<SemaphoreWaitStatus>((Func<SemaphoreWaitStatus, bool>) (r => !r.EnterSemaphoreFailed)).Count<SemaphoreWaitStatus>();
            if (index >= requestCounters.Count)
            {
              throttleInfo = (ThrottleInfo) null;
              requestCounters = (IList<IRequestResourceCounter>) null;
            }
            else
            {
              ThrottleInfo other = new ThrottleInfo(requestCounters[index].Rule, requestCounters[index].NamespaceId, requestCounters[index].Key, ResourceState2.Block);
              other.BlockedByConcurrencyTimeout = true;
              other.ShouldNotifyUser = false;
              if (throttleInfo == null)
              {
                requestContext.RootContext.Items[RequestContextItemsKeys.ThrottleInfo2] = (object) other;
                throttleInfo = (ThrottleInfo) null;
                requestCounters = (IList<IRequestResourceCounter>) null;
              }
              else
              {
                throttleInfo.MergeWithNewThrottleInfo(other);
                throttleInfo = (ThrottleInfo) null;
                requestCounters = (IList<IRequestResourceCounter>) null;
              }
            }
          }
        }
      }
    }

    public void ThrottleRequest(IVssRequestContext requestContext)
    {
      if (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.BypassTarpitting))
        return;
      ThrottleInfo throttleInfo;
      bool flag = requestContext.RootContext.TryGetItem<ThrottleInfo>(RequestContextItemsKeys.ThrottleInfo2, out throttleInfo);
      this.CheckRequestThrottleInfo(requestContext, RUStage.EnterMethod, ref throttleInfo);
      this.IncrementResources(requestContext, RUStage.EnterMethod, ref throttleInfo, out IList<IRequestResourceCounter> _);
      if (throttleInfo == null)
        return;
      if (!flag)
        requestContext.RootContext.Items[RequestContextItemsKeys.ThrottleInfo2] = (object) throttleInfo;
      if (!(throttleInfo.Rule.Resource is RUResource_Concurrency) && throttleInfo.Limit != -1L)
        this.AddDistributedResourceHeaders(requestContext, throttleInfo, RUStage.EnterMethod);
      if ((throttleInfo.ThrottleType2 & ResourceState2.Block) <= ResourceState2.Normal || this.Settings.DoNotBlockDistributed)
        return;
      this.BlockRequest(requestContext, throttleInfo);
    }

    public void QueuePreMethodResourcesIncrementData(IVssRequestContext requestContext) => this.QueueResourcesIncrementData(requestContext, RUStage.EnterMethod);

    public void QueuePostMethodResourcesIncrementData(IVssRequestContext requestContext)
    {
      ThrottleInfo throttleInfo;
      if (requestContext.RootContext.TryGetItem<ThrottleInfo>(RequestContextItemsKeys.ThrottleInfo2, out throttleInfo) && (throttleInfo.ThrottleType2 & ResourceState2.Block) != ResourceState2.Normal)
        return;
      this.QueueResourcesIncrementData(requestContext, RUStage.EndRequest);
    }

    public static void DecrementRequestCounters(IVssRequestContext requestContext)
    {
      try
      {
        IList<IRequestResourceCounter> requestResourceCounterList;
        if (!requestContext.RootContext.TryGetItem<IList<IRequestResourceCounter>>(RequestContextItemsKeys.ResourceUtilizationCounters, out requestResourceCounterList))
          return;
        foreach (IRequestResourceCounter requestResourceCounter in (IEnumerable<IRequestResourceCounter>) requestResourceCounterList)
        {
          try
          {
            requestResourceCounter.Decrement();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(522304013, "ResourceUtilizationService", "Service", ex);
          }
        }
        requestContext.RootContext.Items.Remove(RequestContextItemsKeys.ResourceUtilizationCounters);
      }
      finally
      {
        ResourceUtilization2Service.m_perfCounter_ServiceCallsPerSec.Increment();
      }
    }

    public IDictionary<Guid, ProcessedRURule> GetAllRules(IVssRequestContext requestContext)
    {
      this.EnsureRulesAndThresholdsSynced(requestContext);
      return (IDictionary<Guid, ProcessedRURule>) this.m_rulesAndThresholds.ProcessedRules;
    }

    private void IncrementResources(
      IVssRequestContext requestContext,
      RUStage ruStage,
      ref ThrottleInfo throttleInfo,
      out IList<IRequestResourceCounter> requestCounters)
    {
      try
      {
        if (!requestContext.RootContext.TryGetItem<IList<IRequestResourceCounter>>(RequestContextItemsKeys.ResourceUtilizationCounters, out requestCounters) || requestCounters == null)
        {
          requestCounters = (IList<IRequestResourceCounter>) new List<IRequestResourceCounter>();
          requestContext.RootContext.Items[RequestContextItemsKeys.ResourceUtilizationCounters] = (object) requestCounters;
        }
        ThrottleInfo throttleInfo1 = throttleInfo;
        if ((throttleInfo1 != null ? ((throttleInfo1.ThrottleType2 & ResourceState2.Block) > ResourceState2.Normal ? 1 : 0) : 0) != 0)
          return;
        foreach (KeyValuePair<Guid, ProcessedRURule> keyValuePair in this.m_rulesAndThresholds.ProcessedRules.Where<KeyValuePair<Guid, ProcessedRURule>>((Func<KeyValuePair<Guid, ProcessedRURule>, bool>) (p => p.Value.Resource is RUResource_Concurrency)))
        {
          ProcessedRURule rule = keyValuePair.Value;
          if ((rule.WhenToCheckIfGoverned & ruStage) != RUStage.Null && rule.ShouldGovernRequest(requestContext))
          {
            Guid key = keyValuePair.Key;
            ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter> concurrentDictionary;
            if (this.m_localResourceCache != null && this.m_localResourceCache.TryGetValue(key, out concurrentDictionary))
            {
              string entity = rule.GetEntity(requestContext);
              ResourceState2 resourceState2 = ResourceState2.Normal;
              ResourceUtilization2Service.ConcurrentCounter orAdd;
              long usage;
              do
              {
                orAdd = concurrentDictionary.GetOrAdd(entity, (Func<string, ResourceUtilization2Service.ConcurrentCounter>) (k =>
                {
                  long initialCount = -1;
                  if (rule.WhenToCheckIfGoverned == RUStage.RequestReady)
                  {
                    ProcessedRUThreshold forRuleAndEntity = this.GetThresholdForRuleAndEntity(rule, entity);
                    if (forRuleAndEntity != null)
                      initialCount = forRuleAndEntity.Tarpit;
                  }
                  return new ResourceUtilization2Service.ConcurrentCounter(initialCount);
                }));
                usage = orAdd.Increment();
              }
              while (usage == -1L);
              try
              {
                requestCounters.Add((IRequestResourceCounter) new ResourceUtilization2Service.RequestConcurrentCounter(rule, key, entity, orAdd));
                ProcessedRUThreshold forRuleAndEntity = this.GetThresholdForRuleAndEntity(rule, entity);
                if (forRuleAndEntity != null)
                  resourceState2 = forRuleAndEntity.GetState(usage, 0.0, out double _);
              }
              catch
              {
                orAdd.Decrement(false);
                throw;
              }
              if (resourceState2 > ResourceState2.Normal)
              {
                IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
                utilizationEvents[rule.RuleName] = (object) string.Format("{0},0,1,{1}{2}", (object) usage, (object) (int) resourceState2, rule.ShouldOutputEntityToTelemetry ? (object) (" (" + entity + ")") : (object) string.Empty);
                utilizationEvents["ShouldOutputTelemetry"] = (object) true;
              }
              if ((resourceState2 & ResourceState2.Block) > ResourceState2.Normal)
              {
                ThrottleInfo other = new ThrottleInfo(rule, key, entity, resourceState2);
                other.ShouldNotifyUser = this.Settings.SendConcurrencyBlockNotifications;
                if (throttleInfo == null)
                {
                  throttleInfo = other;
                  break;
                }
                if (other == null)
                  break;
                throttleInfo.MergeWithNewThrottleInfo(other);
                break;
              }
            }
          }
        }
      }
      finally
      {
        ResourceUtilization2Service.m_perfCounter_ServiceCallsPerSec.Increment();
      }
    }

    private void CheckRequestThrottleInfo(
      IVssRequestContext requestContext,
      RUStage ruStage,
      ref ThrottleInfo throttleInfo)
    {
      try
      {
        ThrottleInfo throttleInfo1 = throttleInfo;
        if ((throttleInfo1 != null ? ((throttleInfo1.ThrottleType2 & ResourceState2.Block) > ResourceState2.Normal ? 1 : 0) : 0) != 0)
          return;
        foreach (KeyValuePair<Guid, ProcessedRURule> keyValuePair in this.m_rulesAndThresholds.ProcessedRules.Where<KeyValuePair<Guid, ProcessedRURule>>((Func<KeyValuePair<Guid, ProcessedRURule>, bool>) (p => !(p.Value.Resource is RUResource_Concurrency))))
        {
          ProcessedRURule processedRuRule = keyValuePair.Value;
          if (((ruStage == RUStage.RequestReady || ruStage == RUStage.EnterMethod) && processedRuRule.WhenToCheckIfGoverned == ruStage || ruStage == RUStage.PostLog && processedRuRule.Resource.WhenToMeasureResourceConsumption == RUStage.EndRequest) && processedRuRule.ShouldGovernRequest(requestContext))
          {
            if (processedRuRule.Resource is RUResource_DBCPU || processedRuRule.Resource is RUResource_DBCPU_RO)
              requestContext.RootContext.Items[RequestContextItemsKeys.GovernXEvents] = (object) true;
            Guid key = keyValuePair.Key;
            ThrottleInfo throttleInfoForRule = this.GetThrottleInfoForRule(requestContext, key, keyValuePair.Value, ruStage);
            if (throttleInfoForRule != null)
            {
              if (throttleInfo != null)
                throttleInfo.MergeWithNewThrottleInfo(throttleInfoForRule);
              else
                throttleInfo = throttleInfoForRule;
            }
          }
        }
      }
      finally
      {
        ResourceUtilization2Service.m_perfCounter_ServiceCallsPerSec.Increment();
      }
    }

    private void RecordTelemetry(
      IVssRequestContext requestContext,
      ProcessedRURule rule,
      long replenishedUsage,
      double dpImpact,
      double dpFactor,
      ResourceState2 state,
      string entity)
    {
      IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
      utilizationEvents[rule.RuleName] = (object) string.Format("{0},{1},{2},{3}{4}", (object) replenishedUsage, (object) dpImpact.ToString("0.##", (IFormatProvider) CultureInfo.InvariantCulture), (object) dpFactor.ToString("0.###", (IFormatProvider) CultureInfo.InvariantCulture), (object) (int) state, rule.ShouldOutputEntityToTelemetry ? (object) (" (" + entity + ")") : (object) string.Empty);
      utilizationEvents["ShouldOutputTelemetry"] = (object) true;
    }

    private ThrottleInfo GetThrottleInfoForRule(
      IVssRequestContext requestContext,
      Guid ruleId,
      ProcessedRURule rule,
      RUStage ruStage)
    {
      ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult> concurrentDictionary;
      if (this.m_resourceCache != null && this.m_resourceCache.TryGetValue(ruleId, out concurrentDictionary))
      {
        string entity = rule.GetEntity(requestContext);
        ResourceUtilization2Service.RedisResult redisResult;
        concurrentDictionary.TryGetValue(entity, out redisResult);
        if (redisResult != null)
        {
          ProcessedRUThreshold forRuleAndEntity = this.GetThresholdForRuleAndEntity(rule, entity);
          if (forRuleAndEntity != null)
          {
            DateTimeOffset utcNow = (DateTimeOffset) DateTime.UtcNow;
            (long num1, double dpImpact) = redisResult.CalculateReplenishedUsageAndDPImpact(utcNow, forRuleAndEntity.DPFlag, this.Settings.DPImpactDecayTimeConstant, false);
            double dpFactor;
            ResourceState2 state = forRuleAndEntity.GetState(num1, dpImpact, out dpFactor, ruStage == RUStage.PostLog);
            if (state > ResourceState2.Normal || dpImpact > 0.0)
              this.RecordTelemetry(requestContext, rule, num1, dpImpact, dpFactor, state, entity);
            if (state > ResourceState2.DPFlag)
            {
              double conversionFactor = rule.Resource.GetTSTUConversionFactor();
              double throttleThreshold = (double) forRuleAndEntity.GetMinThrottleThreshold();
              long throttlingThreshold = throttleThreshold != -1.0 ? (long) (conversionFactor * dpFactor * throttleThreshold) : -1L;
              long usage = (long) (conversionFactor * (double) num1);
              double seconds;
              double num2;
              if (throttleThreshold != -1.0 && num1 > 0L)
              {
                seconds = Math.Max(0.0, -300.0 * Math.Log(dpFactor * throttleThreshold / (double) num1));
                num2 = 300.0;
              }
              else
              {
                seconds = 0.0;
                num2 = 0.0;
              }
              DateTime reset = redisResult.TimeStamp.UtcDateTime.AddSeconds(num2);
              DateTimeOffset retryAfterTimestamp = utcNow.AddSeconds(seconds);
              ThrottleInfo throttleInfo = new ThrottleInfo(rule, ruleId, (int) (dpImpact + 1.0), entity, state, throttlingThreshold, usage, (DateTimeOffset) reset, retryAfterTimestamp);
              if ((state & ResourceState2.Tarpit) > ResourceState2.Normal)
              {
                double factorAboveThreshold = Math.Max((double) num1 / Math.Max(dpFactor * (double) forRuleAndEntity.Tarpit, 1.0) - 1.0, 0.0);
                throttleInfo.DelayForNextAwait = ruStage == RUStage.RequestReady ? this.GetRequestReadyDelayForNextAwait(requestContext, factorAboveThreshold) : this.GetPostLogDelayForNextAwait(requestContext, factorAboveThreshold);
                if (dpImpact > this.Settings.DPImpactNotificationThreshold && !this.Settings.DoNotTarpit)
                  throttleInfo.ShouldNotifyUser = true;
              }
              if (ruStage == RUStage.RequestReady && (state & (ResourceState2.Flag | ResourceState2.Tarpit)) > ResourceState2.Normal)
                rule.Resource.FlagThrottleInfo(requestContext, ref throttleInfo);
              return throttleInfo;
            }
          }
        }
      }
      return (ThrottleInfo) null;
    }

    private int GetRequestReadyDelayForNextAwait(
      IVssRequestContext requestContext,
      double factorAboveThreshold)
    {
      return Math.Max((int) (this.GetModulationMultiplier(requestContext, RUStage.RequestReady) * (double) this.GetResourceInfoDelay(requestContext, factorAboveThreshold)), 1);
    }

    private int GetPostLogDelayForNextAwait(
      IVssRequestContext requestContext,
      double factorAboveThreshold)
    {
      long num = requestContext.ExecutionTime() / 1000L;
      return (int) (this.GetModulationMultiplier(requestContext, RUStage.PostLog) * this.Settings.PostRequestTarpittingFactor * factorAboveThreshold * (double) num);
    }

    internal int GetResourceInfoDelay(
      IVssRequestContext requestContext,
      double factorAboveThreshold)
    {
      TimeSpan timeSpan;
      int num1;
      if (this.Settings.DelayPowerLawIndex > 0.0)
      {
        double num2 = Math.Pow(factorAboveThreshold, this.Settings.DelayPowerLawIndex);
        timeSpan = this.Settings.InitialDelay;
        double totalMilliseconds = timeSpan.TotalMilliseconds;
        num1 = Convert.ToInt32(Math.Min(num2 * totalMilliseconds * 1000.0, (double) int.MaxValue));
      }
      else
        num1 = Convert.ToInt32(Math.Pow(2.0, 10.0 * Math.Min(factorAboveThreshold, 2.0))) * Convert.ToInt32(this.Settings.InitialDelay.TotalMilliseconds);
      int val1 = num1;
      timeSpan = this.Settings.MaxDelay;
      int int32_1 = Convert.ToInt32(timeSpan.TotalMilliseconds);
      int maxValue = Math.Min(val1, int32_1);
      int num3 = 1 + this.m_randomizer.Next((int) ((1.0 - this.Settings.DelayRandomizationFactor) * (double) maxValue), maxValue);
      timeSpan = this.Settings.DelayTimeToAlwaysAdd;
      int int32_2 = Convert.ToInt32(timeSpan.TotalMilliseconds);
      return num3 + int32_2;
    }

    private static void QueueThrottlingEventForPotentialEmailNotification(
      IVssRequestContext requestContext,
      ThrottleInfo throttleInfo)
    {
      if (!throttleInfo.ShouldNotifyUser || !requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ThrottlingNotification.Email") || (throttleInfo.ThrottleType2 & ResourceState2.Block) > ResourceState2.Normal || requestContext.Status is UnauthorizedRequestException)
        return;
      IThrottlingNotificationService service = requestContext.GetService<IThrottlingNotificationService>();
      if (throttleInfo.UserIdentity == null)
        throttleInfo.UserIdentity = requestContext.GetUserIdentity();
      throttleInfo.IsPublicUser = requestContext.IsRootContextPublicUser();
      IVssRequestContext requestContext1 = requestContext;
      ThrottleInfo throttleInfo1 = throttleInfo;
      service.QueueThrottlingEventForPotentialEmailNotification(requestContext1, throttleInfo1);
    }

    private void QueueResourcesIncrementData(
      IVssRequestContext requestContext,
      RUStage whenToMeasure)
    {
      try
      {
        long executionTimeMilliseconds = Math.Max(1L, requestContext.ExecutionTime() / 1000L);
        IList<ResourceUtilization2Service.IncrementResourceTaskArgs> source = (IList<ResourceUtilization2Service.IncrementResourceTaskArgs>) new List<ResourceUtilization2Service.IncrementResourceTaskArgs>();
        foreach (KeyValuePair<Guid, ProcessedRURule> processedRule in this.m_rulesAndThresholds.ProcessedRules)
        {
          ProcessedRURule processedRuRule = processedRule.Value;
          if (processedRuRule.Resource.WhenToMeasureResourceConsumption == whenToMeasure && processedRuRule.ShouldGovernRequest(requestContext))
          {
            Guid key = processedRule.Key;
            long requestCost = processedRuRule.Resource.GetRequestCost(requestContext);
            string entity = processedRuRule.GetEntity(requestContext);
            if (requestCost > 0L)
              requestContext.TSTUs = Math.Max(requestContext.TSTUs, processedRuRule.Resource.GetTSTUConversionFactor() * (double) requestCost);
            if (requestCost > 0L || processedRuRule.Resource.ShouldQueueZeroIncrement())
            {
              ResourceUtilization2Service.IncrementResourceTaskArgs resourceTaskArgs = new ResourceUtilization2Service.IncrementResourceTaskArgs(key, entity, requestCost, executionTimeMilliseconds);
              source.Add(resourceTaskArgs);
              ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext)[processedRuRule.GetResourceName()] = (object) requestCost.ToString();
            }
          }
        }
        if (source.Count <= 0)
          return;
        this.m_queuedTaskArgs.PushRange(source.ToArray<ResourceUtilization2Service.IncrementResourceTaskArgs>());
      }
      finally
      {
        ResourceUtilization2Service.m_perfCounter_ServiceCallsPerSec.Increment();
      }
    }

    private bool EnsureRulesAndThresholdsSynced(IVssRequestContext requestContext)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (this.m_deploymentService != null)
      {
        this.m_deploymentService.EnsureRulesAndThresholdsSynced(requestContext.To(TeamFoundationHostType.Deployment));
        flag2 = this.m_deploymentLevelRulesAndThresholdsVersion < this.m_deploymentService.m_rulesAndThresholds.Version;
      }
      RulesAndThresholds rulesAndThresholds = (RulesAndThresholds) null;
      int version = this.m_rulesAndThresholds.Version;
      if (this.m_ruleThresholdVersion > version)
      {
        try
        {
          rulesAndThresholds = new RulesAndThresholds(requestContext, this.m_ruleThresholdVersion);
        }
        catch (Exception ex)
        {
          requestContext.Trace(522304007, TraceLevel.Error, "ResourceUtilizationService", "Service", string.Format("Couldn't read RulesAndThresholds; old version={0}; new version={1}; Exception={2}", (object) version, (object) this.m_ruleThresholdVersion, (object) ex.Message));
        }
      }
      else if (flag2)
        rulesAndThresholds = new RulesAndThresholds(this.m_rulesAndThresholds);
      if (rulesAndThresholds != null)
      {
        if (this.m_localResourceCache != null)
        {
          foreach (KeyValuePair<Guid, ProcessedRURule> keyValuePair in this.m_rulesAndThresholds.ProcessedRules.Where<KeyValuePair<Guid, ProcessedRURule>>((Func<KeyValuePair<Guid, ProcessedRURule>, bool>) (r => r.Value.Resource is RUResource_Concurrency && r.Value.WhenToCheckIfGoverned == RUStage.RequestReady)))
            this.m_concurrentCountersToCleanUp.Enqueue(this.m_localResourceCache[keyValuePair.Key]);
        }
        rulesAndThresholds.Compile(requestContext, this.m_deploymentService?.m_rulesAndThresholds, this.Settings);
        this.m_rulesAndThresholds = rulesAndThresholds;
        if (flag2)
          this.m_deploymentLevelRulesAndThresholdsVersion = this.m_deploymentService.m_rulesAndThresholds.Version;
        flag1 = true;
        if (this.m_deploymentService == null)
        {
          using (requestContext.AllowAnonymousOrPublicUserWrites())
          {
            int num = requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
            {
              new TeamFoundationJobReference(ResourceUtilizationConstants.PollSettingsForExpiredThresholdsJobId)
            });
            if (num > 0)
              requestContext.TraceAlways(522304010, TraceLevel.Info, "ResourceUtilizationService", "Service", string.Format("Detected a change in RulesAndThresholds; old version={0}; new version={1}; kicking off ResourceUtilizationPollSettingsForExpiredThresholdsJob returned {2}", (object) version, (object) this.m_ruleThresholdVersion, (object) num));
          }
        }
      }
      return flag1;
    }

    private void OnResourceStateChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ResourceUtilizationInfo resourceUtilizationInfo = TeamFoundationSerializationUtility.Deserialize<ResourceUtilizationInfo>(eventData);
      ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult> concurrentDictionary;
      if (this.m_resourceCache == null || !this.m_resourceCache.TryGetValue(resourceUtilizationInfo.NamespaceId, out concurrentDictionary))
        return;
      concurrentDictionary[resourceUtilizationInfo.Key] = new ResourceUtilization2Service.RedisResult(resourceUtilizationInfo.Result, resourceUtilizationInfo.DPImpact, (DateTimeOffset) resourceUtilizationInfo.TimeStamp, (ResourceState2) resourceUtilizationInfo.State);
    }

    private void OnRulesOrThresholdsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ++this.m_ruleThresholdVersion;
    }

    internal void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_serviceSettings = new ResourceUtilization2Service.ServiceSettings(requestContext);
      this.ScheduleServiceTasks(requestContext);
    }

    private void EnsureSettingsSynced(IVssRequestContext requestContext)
    {
      if (!(this.m_serviceSettingsVersion != this.Settings.Version))
        return;
      this.ScheduleServiceTasks(requestContext);
    }

    private ResourceUtilization2Service.ServiceSettings Settings => this.m_deploymentService != null ? this.m_deploymentService.m_serviceSettings : this.m_serviceSettings;

    private void ScheduleServiceTasks(IVssRequestContext requestContext)
    {
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      bool flag = false;
      try
      {
        flag = requestContext.LockManager.TryGetLock(this.m_scheduleServiceTasksLock, 0);
        if (!flag)
          return;
        ResourceUtilization2Service.ServiceSettings settings = this.Settings;
        if (!(this.m_serviceSettingsVersion != settings.Version))
          return;
        service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.CleanupCacheTask), (object) null, Convert.ToInt32(settings.CleanupInterval.TotalMilliseconds)));
        service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.IncrementResourcesTask), (object) null, Convert.ToInt32(settings.IncrementInterval.TotalMilliseconds)));
        this.m_serviceSettingsVersion = settings.Version;
      }
      finally
      {
        if (flag)
          requestContext.LockManager.ReleaseLock(this.m_scheduleServiceTasksLock);
      }
    }

    private void IncrementResourcesTask(IVssRequestContext requestContext, object taskArgs)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        this.EnsureSettingsSynced(requestContext);
        ConcurrentStack<ResourceUtilization2Service.IncrementResourceTaskArgs> queuedTaskArgs = this.m_queuedTaskArgs;
        this.m_queuedTaskArgs = new ConcurrentStack<ResourceUtilization2Service.IncrementResourceTaskArgs>();
        if (!this.RedisCachingService.IsEnabled(requestContext))
          return;
        this.IncrementResourcesInRedis(requestContext, queuedTaskArgs);
      }
      finally
      {
        stopwatch.Stop();
        VssPerformanceCounter performanceCounter = ResourceUtilization2Service.m_perfCounter_AverageTaskExecutionTime;
        performanceCounter.IncrementTicks(stopwatch);
        performanceCounter = ResourceUtilization2Service.m_perfCounter_AverageTaskExecutionTimeBase;
        performanceCounter.Increment();
      }
    }

    internal void IncrementResourcesInRedis(
      IVssRequestContext requestContext,
      ConcurrentStack<ResourceUtilization2Service.IncrementResourceTaskArgs> taskArgsToProcess)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      ILookup<Guid, KeyDataExecutionTime> lookup = taskArgsToProcess.ToLookup<ResourceUtilization2Service.IncrementResourceTaskArgs, Guid, KeyDataExecutionTime>((Func<ResourceUtilization2Service.IncrementResourceTaskArgs, Guid>) (ta => ta.NamespaceId), (Func<ResourceUtilization2Service.IncrementResourceTaskArgs, KeyDataExecutionTime>) (ta => new KeyDataExecutionTime(ta.Key, ta.Data, ta.ExecutionTimeMilliseconds)));
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = "ResourceUtilizationService",
        KeyExpiry = new TimeSpan?(TimeSpan.FromSeconds(300.0)),
        AllowBatching = new bool?(true)
      };
      int count = taskArgsToProcess.Count;
      ResourceUtilization2Service.m_perfCounter_AverageQueueSize.IncrementBy((long) count);
      ResourceUtilization2Service.m_perfCounter_AverageQueueSizeBase.Increment();
      DateTimeOffset utcNow = (DateTimeOffset) DateTime.UtcNow;
      foreach (IGrouping<Guid, KeyDataExecutionTime> source1 in (IEnumerable<IGrouping<Guid, KeyDataExecutionTime>>) lookup)
      {
        Guid key1 = source1.Key;
        IEnumerable<KeyValuePair<string, WindowItem>> keyValuePairs = source1.ToLookup<KeyDataExecutionTime, string, long>((Func<KeyDataExecutionTime, string>) (x => x.Key), (Func<KeyDataExecutionTime, long>) (x => x.ExecutionTimeMilliseconds > 300000L && x.Data > 0L ? Math.Max(1L, Convert.ToInt64((double) x.Data * (300000.0 / (double) x.ExecutionTimeMilliseconds))) : x.Data)).ToDictionary<IGrouping<string, long>, string, long>((Func<IGrouping<string, long>, string>) (x => x.Key), (Func<IGrouping<string, long>, long>) (x => x.Sum())).Select<KeyValuePair<string, long>, KeyValuePair<string, WindowItem>>((Func<KeyValuePair<string, long>, KeyValuePair<string, WindowItem>>) (x => new KeyValuePair<string, WindowItem>(x.Key, new WindowItem(x.Value))));
        ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult> concurrentDictionary;
        ProcessedRURule rule;
        if (keyValuePairs.Any<KeyValuePair<string, WindowItem>>() && this.m_resourceCache != null && this.m_resourceCache.TryGetValue(key1, out concurrentDictionary) && this.m_rulesAndThresholds.ProcessedRules.TryGetValue(key1, out rule))
        {
          IEnumerable<long> second1 = this.RedisCachingService.GetWindowedDictionaryContainer<string, ResourceUtilization2Service.RedisCacheSecurityToken>(requestContext, key1, settings).IncrementBy(requestContext, keyValuePairs);
          IEnumerable<KeyValuePair<string, long>> source2 = keyValuePairs.Zip<KeyValuePair<string, WindowItem>, long, KeyValuePair<string, long>>(second1, (Func<KeyValuePair<string, WindowItem>, long, KeyValuePair<string, long>>) ((first, second) => new KeyValuePair<string, long>(first.Key, second)));
          DateTime dateTime = utcNow.UtcDateTime.Add(settings.KeyExpiry.Value);
          foreach (KeyValuePair<string, long> keyValuePair in source2.Where<KeyValuePair<string, long>>((Func<KeyValuePair<string, long>, bool>) (x => x.Value > -1L)))
          {
            string key2 = keyValuePair.Key;
            long usage1 = keyValuePair.Value;
            double dpImpact = 0.0;
            double dpFactor = 1.0;
            ResourceState2 state = ResourceState2.Normal;
            ProcessedRUThreshold forRuleAndEntity = this.GetThresholdForRuleAndEntity(rule, key2);
            if (forRuleAndEntity != null)
            {
              ResourceState2 resourceState2 = ResourceState2.Normal;
              ResourceUtilization2Service.RedisResult redisResult;
              if (concurrentDictionary.TryGetValue(key2, out redisResult))
              {
                long usage2 = redisResult.Usage;
                dpImpact = redisResult.CalculateReplenishedUsageAndDPImpact(utcNow, forRuleAndEntity.DPFlag, this.Settings.DPImpactDecayTimeConstant, true).dpImpact;
                resourceState2 = redisResult.State;
              }
              state = forRuleAndEntity.GetState(usage1, dpImpact, out dpFactor);
              if (state > ResourceState2.Normal || state != resourceState2 || dpImpact > 0.0)
                concurrentDictionary[key2] = new ResourceUtilization2Service.RedisResult(usage1, dpImpact, utcNow, state);
              if (resourceState2 != state)
              {
                string eventData = TeamFoundationSerializationUtility.SerializeToString<ResourceUtilizationInfo>(new ResourceUtilizationInfo()
                {
                  Resource = string.Empty,
                  NamespaceId = key1,
                  Key = key2,
                  WindowSeconds = 300,
                  Result = usage1,
                  State = (int) state,
                  DPImpact = dpImpact,
                  TimeStamp = utcNow.UtcDateTime,
                  Expiration = dateTime
                });
                requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, ResourceUtilizationConstants.ResourceStateChanged, eventData);
              }
            }
            TeamFoundationTracingService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>();
            string dataFeed = string.Format("Rule={0}; Entity={1}; Usage={2}; DPImpact={3}; DPFactor={4}; State={5};", (object) rule.RuleName, (object) key2, (object) usage1, (object) dpImpact.ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture), (object) dpFactor.ToString("0.000", (IFormatProvider) CultureInfo.InvariantCulture), (object) (int) state);
            service.TraceResourceUtilization(requestContext, 2, dataFeed);
          }
          count -= source1.Count<KeyDataExecutionTime>();
          if (stopwatch.Elapsed >= this.Settings.MaxIncrementDuration)
          {
            requestContext.Trace(522304011, TraceLevel.Error, "ResourceUtilizationService", "Service", "IncrementResourcesInRedis took {0} milliseconds exceeding the limit of {1} milliseconds, discarding the remaining {2} unprocessed elements", (object) stopwatch.Elapsed.TotalMilliseconds, (object) this.Settings.MaxIncrementDuration, (object) count);
            break;
          }
        }
      }
    }

    private ProcessedRUThreshold GetThresholdForRuleAndEntity(ProcessedRURule rule, string entity)
    {
      ProcessedRUThreshold forRuleAndEntity = (ProcessedRUThreshold) null;
      Dictionary<string, ProcessedRUThreshold> thresholds = rule.Thresholds;
      if (!thresholds.TryGetValue(entity, out forRuleAndEntity))
        thresholds.TryGetValue(string.Empty, out forRuleAndEntity);
      return forRuleAndEntity;
    }

    private void CleanupCacheTask(IVssRequestContext requestContext, object taskArgs)
    {
      this.EnsureSettingsSynced(requestContext);
      if (this.m_resourceCache != null)
      {
        foreach (KeyValuePair<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>> keyValuePair1 in (IEnumerable<KeyValuePair<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult>>>) this.m_resourceCache)
        {
          ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult> concurrentDictionary = keyValuePair1.Value;
          foreach (KeyValuePair<string, ResourceUtilization2Service.RedisResult> keyValuePair2 in concurrentDictionary)
          {
            double currentDpImpact = keyValuePair2.Value.CalculateCurrentDPImpact((DateTimeOffset) DateTime.UtcNow, this.Settings.DPImpactDecayTimeConstant);
            if (currentDpImpact == 0.0)
            {
              bool flag = keyValuePair2.Value.TimeStamp.AddSeconds(300.0) <= (DateTimeOffset) DateTime.UtcNow;
              if (!flag)
                flag = this.GetThresholdForRuleAndEntity(this.m_rulesAndThresholds.ProcessedRules[keyValuePair1.Key], keyValuePair2.Key).GetState(keyValuePair2.Value.Usage, currentDpImpact, out double _) == ResourceState2.Normal;
              if (flag)
                concurrentDictionary.TryRemove(keyValuePair2.Key, out ResourceUtilization2Service.RedisResult _);
            }
          }
        }
      }
      if (this.m_localResourceCache != null)
      {
        foreach (KeyValuePair<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>> keyValuePair3 in (IEnumerable<KeyValuePair<Guid, ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter>>>) this.m_localResourceCache)
        {
          foreach (KeyValuePair<string, ResourceUtilization2Service.ConcurrentCounter> keyValuePair4 in keyValuePair3.Value)
          {
            ResourceUtilization2Service.ConcurrentCounter concurrentCounter;
            if (keyValuePair4.Value.CanCleanup(this.Settings.CleanupInterval) && keyValuePair3.Value.TryRemove(keyValuePair4.Key, out concurrentCounter))
              concurrentCounter.Dispose();
          }
        }
      }
      ConcurrentDictionary<string, ResourceUtilization2Service.ConcurrentCounter> result;
      while (this.m_concurrentCountersToCleanUp.TryDequeue(out result))
      {
        foreach (KeyValuePair<string, ResourceUtilization2Service.ConcurrentCounter> keyValuePair in result)
          keyValuePair.Value.Dispose();
      }
    }

    public static IDictionary<string, object> GetOrCreateResourceUtilizationEvents(
      IVssRequestContext requestContext)
    {
      IDictionary<string, object> utilizationEvents;
      if (!requestContext.RootContext.TryGetItem<IDictionary<string, object>>(RequestContextItemsKeys.ResourceUtilizationEvents, out utilizationEvents))
      {
        utilizationEvents = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.RootContext.Items[RequestContextItemsKeys.ResourceUtilizationEvents] = (object) utilizationEvents;
      }
      return utilizationEvents;
    }

    internal void AdjustAndCapPostRequestDelay(
      IVssRequestContext requestContext,
      int preRequestDelay,
      long requestExecutionTime_ms,
      ref ThrottleInfo throttleInfo)
    {
      if (throttleInfo == null || (throttleInfo.ThrottleType2 & ResourceState2.Tarpit) == ResourceState2.Normal)
        return;
      this.AddDistributedResourceHeaders(requestContext, throttleInfo, RUStage.PostLog);
      this.GetModulationMultiplier(requestContext, RUStage.PostLog);
      throttleInfo.DelayForNextAwait = Math.Max(0, throttleInfo.DelayForNextAwait - preRequestDelay);
      if ((double) throttleInfo.DelayForNextAwait / (double) requestExecutionTime_ms > this.Settings.PostRequestTarpittingRelativeCap)
        throttleInfo.DelayForNextAwait = (int) (this.Settings.PostRequestTarpittingRelativeCap * (double) requestExecutionTime_ms);
      double delayForNextAwait = (double) throttleInfo.DelayForNextAwait;
      TimeSpan tarpittingAbsoluteCap = this.Settings.PostRequestTarpittingAbsoluteCap;
      double totalMilliseconds1 = tarpittingAbsoluteCap.TotalMilliseconds;
      if (delayForNextAwait <= totalMilliseconds1)
        return;
      ThrottleInfo throttleInfo1 = throttleInfo;
      tarpittingAbsoluteCap = this.Settings.PostRequestTarpittingAbsoluteCap;
      int totalMilliseconds2 = (int) tarpittingAbsoluteCap.TotalMilliseconds;
      throttleInfo1.DelayForNextAwait = totalMilliseconds2;
    }

    private double GetModulationMultiplier(IVssRequestContext requestContext, RUStage ruStage)
    {
      IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
      double modulationMultiplier;
      if (ruStage == RUStage.RequestReady)
      {
        switch ((uint) TimeSpan.FromMinutes((double) (requestContext.StartTime().Ticks / 3000000000L)).GetHashCode() % 5U)
        {
          case 0:
          case 1:
            modulationMultiplier = 1.0 - this.Settings.ModulationFactor;
            break;
          case 2:
          case 3:
            modulationMultiplier = 1.0 + this.Settings.ModulationFactor;
            break;
          default:
            modulationMultiplier = 1.0;
            break;
        }
        utilizationEvents["Modulation"] = (object) modulationMultiplier;
      }
      else
      {
        object obj;
        modulationMultiplier = !utilizationEvents.TryGetValue("Modulation", out obj) ? 1.0 : (double) obj;
      }
      return modulationMultiplier;
    }

    private void AddDistributedResourceHeaders(
      IVssRequestContext requestContext,
      ThrottleInfo throttleInfo,
      RUStage ruStage)
    {
      bool flag = this.Settings.DoNotTarpit || this.Settings.DoNotBlockDistributed;
      string resourceHeader = throttleInfo.GetResourceHeader(requestContext);
      Dictionary<string, string> headersToAdd = new Dictionary<string, string>()
      {
        {
          "X-RateLimit-Resource",
          resourceHeader
        },
        {
          "X-RateLimit-Limit",
          throttleInfo.Limit.ToString()
        },
        {
          "X-RateLimit-Remaining",
          Math.Max(throttleInfo.Limit - throttleInfo.Usage, 0L).ToString()
        },
        {
          "X-RateLimit-Reset",
          throttleInfo.Reset.ToUnixTimeSeconds().ToString()
        }
      };
      int val1 = (int) Math.Ceiling((throttleInfo.RetryAfterTimestamp - DateTimeOffset.UtcNow).TotalSeconds);
      if (val1 > 0)
      {
        headersToAdd["Retry-After"] = Math.Max((double) val1, Math.Ceiling(this.Settings.MinRetryAfter.TotalSeconds)).ToString();
      }
      else
      {
        requestContext.TraceAlways(522304012, TraceLevel.Verbose, "ResourceUtilizationService", "Service", string.Format("Omitting RetryAfter for request with state {0}; calculated timestamp {1}", (object) throttleInfo.ThrottleType2, (object) val1));
        if (ruStage == RUStage.PostLog && !flag)
          this.RemoveResponseHeaders(requestContext, (IEnumerable<string>) new string[1]
          {
            "Retry-After"
          });
      }
      if (this.Settings.OutputThrottleInfoToTelemetry)
      {
        IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
        utilizationEvents["TI2_ReportedResource"] = (object) resourceHeader;
        utilizationEvents["TI2_Rule"] = (object) throttleInfo.Rule.RuleName;
        utilizationEvents["TI2_Limit"] = (object) throttleInfo.Limit;
        utilizationEvents["TI2_Usage"] = (object) throttleInfo.Usage;
        IDictionary<string, object> dictionary1 = utilizationEvents;
        DateTimeOffset dateTimeOffset = throttleInfo.Reset;
        // ISSUE: variable of a boxed type
        __Boxed<long> unixTimeSeconds = (ValueType) dateTimeOffset.ToUnixTimeSeconds();
        dictionary1["TI2_Reset"] = (object) unixTimeSeconds;
        if (val1 > 0)
        {
          IDictionary<string, object> dictionary2 = utilizationEvents;
          dateTimeOffset = throttleInfo.RetryAfterTimestamp;
          string str = dateTimeOffset.ToString("s", (IFormatProvider) CultureInfo.InvariantCulture);
          dictionary2["TI2_RetryAfter"] = (object) str;
        }
        else
          utilizationEvents.Remove("TI2_RetryAfter");
      }
      if (flag)
        return;
      this.AddResponseHeaders(requestContext, (IDictionary<string, string>) headersToAdd);
    }

    internal virtual void AddResponseHeaders(
      IVssRequestContext requestContext,
      IDictionary<string, string> headersToAdd)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal) || requestContextInternal.HttpContext == null || requestContextInternal.HttpContext.Response.HeadersWritten)
        return;
      NameValueCollection headers = requestContextInternal.HttpContext.Response.Headers;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) headersToAdd)
        headers.Set(keyValuePair.Key, keyValuePair.Value);
    }

    private void RemoveResponseHeaders(
      IVssRequestContext requestContext,
      IEnumerable<string> headersToRemove)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal) || requestContextInternal.HttpContext == null || requestContextInternal.HttpContext.Response.HeadersWritten)
        return;
      NameValueCollection headers = requestContextInternal.HttpContext.Response.Headers;
      foreach (string name in headersToRemove)
        headers.Remove(name);
    }

    private async Task TarpitRequest(
      IVssRequestContext requestContext,
      ThrottleInfo throttleInfo,
      RUStage ruStage)
    {
      if (throttleInfo.DelayForNextAwait <= 0)
        return;
      if (ruStage == RUStage.PostLog)
        ResourceUtilization2Service.QueueThrottlingEventForPotentialEmailNotification(requestContext, throttleInfo);
      long start = Stopwatch.GetTimestamp();
      try
      {
        await this.Delay(throttleInfo.DelayForNextAwait, requestContext.CancellationToken);
      }
      finally
      {
        long timestamp = Stopwatch.GetTimestamp();
        this.AddTimeSpentDelayedToRequestTimer(requestContext, timestamp - start);
        Dictionary<string, string> headersToAdd = new Dictionary<string, string>()
        {
          {
            "X-RateLimit-Delay",
            requestContext.RequestTimer.DelaySpan.TotalSeconds.ToString("0.000")
          }
        };
        if (throttleInfo.ShouldNotifyUser)
          headersToAdd["X-VSS-GlobalMessage"] = throttleInfo.GetGlobalMessageResponseHeader(requestContext);
        this.AddResponseHeaders(requestContext, (IDictionary<string, string>) headersToAdd);
        IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
        utilizationEvents["DelayTime" + (object) (ruStage == RUStage.PostLog ? 2 : 1)] = (object) throttleInfo.DelayForNextAwait;
        utilizationEvents["ShouldOutputTelemetry"] = (object) true;
      }
    }

    internal virtual void AddTimeSpentDelayedToRequestTimer(
      IVssRequestContext requestContext,
      long timeSpentDelayed)
    {
      requestContext.RequestTimer.RequestTimerInternal().AddTimeSpentDelayed(timeSpentDelayed);
    }

    internal virtual void AddTimeSpentInConcurrencySemaphore(
      IVssRequestContext requestContext,
      long timeSpentInConcurrencySemaphore)
    {
      IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
      requestContext.RequestTimer.RequestTimerInternal().AddTimeSpentInConcurrencySemaphore(timeSpentInConcurrencySemaphore);
      // ISSUE: variable of a boxed type
      __Boxed<long> concurrencySemaphoreTime = (ValueType) requestContext.RequestTimer.ConcurrencySemaphoreTime;
      utilizationEvents["ConcurrencySemaphoreTime"] = (object) concurrencySemaphoreTime;
    }

    internal virtual async Task Delay(int delayForNextAwait, CancellationToken cancellationToken) => await Task.Delay(delayForNextAwait, cancellationToken);

    private Task<SemaphoreWaitStatus[]> AwaitRequestCounters(
      IVssRequestContext requestContext,
      IList<IRequestResourceCounter> requestCounters)
    {
      IList<Task<SemaphoreWaitStatus>> taskList = (IList<Task<SemaphoreWaitStatus>>) new List<Task<SemaphoreWaitStatus>>();
      TimeSpan timeout = this.Settings.BlockOnTimeout ? this.Settings.ConcurrencySemaphoreTimeout : Timeout.InfiniteTimeSpan;
      foreach (IRequestResourceCounter requestCounter in (IEnumerable<IRequestResourceCounter>) requestCounters)
        taskList.Add(requestCounter.WaitAsync(timeout, requestContext.CancellationToken));
      return Task.WhenAll<SemaphoreWaitStatus>((IEnumerable<Task<SemaphoreWaitStatus>>) taskList);
    }

    private void BlockRequest(IVssRequestContext requestContext, ThrottleInfo throttleInfo)
    {
      string resourceHeader = throttleInfo.GetResourceHeader(requestContext);
      Dictionary<string, string> headersToAdd = new Dictionary<string, string>()
      {
        {
          "X-RateLimit-Resource",
          resourceHeader
        }
      };
      if (!(throttleInfo.Rule.Resource is RUResource_Concurrency))
      {
        headersToAdd.Add("X-RateLimit-Limit", throttleInfo.Limit.ToString());
        headersToAdd.Add("X-RateLimit-Remaining", "0");
        this.m_queuedTaskArgs.Push(new ResourceUtilization2Service.IncrementResourceTaskArgs(throttleInfo.Namespace, throttleInfo.Key, 0L, 0L));
      }
      this.AddResponseHeaders(requestContext, (IDictionary<string, string>) headersToAdd);
      IDictionary<string, object> utilizationEvents = ResourceUtilization2Service.GetOrCreateResourceUtilizationEvents(requestContext);
      if (this.Settings.OutputThrottleInfoToTelemetry)
      {
        utilizationEvents["TI2_ReportedResource"] = (object) resourceHeader;
        if (!(throttleInfo.Rule.Resource is RUResource_Concurrency))
        {
          utilizationEvents["TI2_Limit"] = (object) throttleInfo.Limit;
          utilizationEvents["TI2_Remaining"] = (object) "0";
        }
      }
      utilizationEvents["Block"] = (object) 1;
      utilizationEvents["ShouldOutputTelemetry"] = (object) true;
      ResourceUtilization2Service.BlockRequestException(requestContext, throttleInfo);
    }

    private static void BlockRequestException(
      IVssRequestContext requestContext,
      ThrottleInfo throttleInfo)
    {
      ProcessedRURule rule = throttleInfo.Rule;
      string message;
      string messageHtml;
      if (rule.Resource is RUResource_Concurrency)
      {
        message = FrameworkResources.RequestBlockedWithFwlink((object) rule.GetResourceName(), (object) rule.GetFriendlyNamespace(), (object) "823950");
        messageHtml = (string) null;
      }
      else
      {
        int num = (int) Math.Ceiling((throttleInfo.RetryAfterTimestamp - DateTimeOffset.Now).TotalMinutes);
        string str1 = num > 1 ? FrameworkResources.Minutes((object) num) : FrameworkResources.Minute((object) num);
        if (rule.HasIPAddressEntityType)
        {
          string str2 = requestContext.RemoteIPAddress();
          message = HostingResources.RequestBlockedAnonymous((object) str1, (object) str2, (object) "823950");
          messageHtml = HostingResources.RequestBlockedAnonymousHtml((object) str1, (object) str2, (object) "823950");
        }
        else if (rule.HasPublicUserFilter)
        {
          string displayName = requestContext.GetUserIdentity().DisplayName;
          message = HostingResources.RequestBlockedPublicUser((object) str1, (object) displayName, (object) "823950");
          messageHtml = HostingResources.RequestBlockedPublicUserHtml((object) str1, (object) displayName, (object) "823950");
        }
        else
        {
          message = FrameworkResources.RequestBlockedWithFwlink((object) rule.GetResourceName(), (object) rule.GetFriendlyNamespace(), (object) "823950");
          messageHtml = (string) null;
        }
      }
      throw new RequestBlockedException(message, messageHtml);
    }

    public long GetAccumulatedUsage(
      IVssRequestContext requestContext,
      string resourceName,
      Guid namespaceId,
      int windowSeconds,
      string key)
    {
      long accumulatedUsage = 0;
      ConcurrentDictionary<string, ResourceUtilization2Service.RedisResult> concurrentDictionary;
      ResourceUtilization2Service.RedisResult redisResult;
      if (this.m_resourceCache != null && this.m_resourceCache.TryGetValue(namespaceId, out concurrentDictionary) && concurrentDictionary.TryGetValue(key, out redisResult))
        accumulatedUsage = redisResult.Usage;
      return accumulatedUsage;
    }

    public int SetRUMacro(
      IVssRequestContext requestContext,
      string macroName,
      string macroDefinition)
    {
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.SetRUMacro(macroName, macroDefinition);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    public int SetRURule(IVssRequestContext requestContext, string ruleName, string definition)
    {
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.SetRURule(ruleName, definition);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    public int SetRUThreshold(
      IVssRequestContext requestContext,
      string ruleName,
      string entity,
      string flag,
      string tarpit,
      string block,
      string dpMagniture,
      string note,
      string expirationTime)
    {
      long result1;
      long? flag1 = long.TryParse(flag, out result1) ? new long?(result1) : new long?();
      long? tarpit1 = long.TryParse(tarpit, out result1) ? new long?(result1) : new long?();
      long? block1 = long.TryParse(block, out result1) ? new long?(result1) : new long?();
      double result2;
      double? dpMagnitude = double.TryParse(dpMagniture, out result2) ? new double?(result2) : new double?();
      DateTime result3;
      DateTime? expiration = DateTime.TryParse(expirationTime, out result3) ? new DateTime?(result3) : new DateTime?();
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.SetRUThreshold(ruleName, entity, flag1, tarpit1, block1, dpMagnitude, note, expiration);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    public int DeleteRUMacro(IVssRequestContext requestContext, string macroName)
    {
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.DeleteRUMacro(macroName);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    public int DeleteRURule(IVssRequestContext requestContext, string ruleName)
    {
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.DeleteRURule(ruleName);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    public int DeleteRUThreshold(IVssRequestContext requestContext, string ruleName, string entity)
    {
      int num = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
        num = component.DeleteRUThreshold(ruleName, entity);
      this.CallOnRulesOrThresholdsChanged(requestContext);
      return num;
    }

    private void CallOnRulesOrThresholdsChanged(IVssRequestContext requestContext)
    {
      this.OnRulesOrThresholdsChanged(requestContext, ResourceUtilizationConstants.RulesOrThresholdsChanged, string.Empty);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, ResourceUtilizationConstants.RulesOrThresholdsChanged, string.Empty);
    }

    private IRedisCacheService RedisCachingService { get; set; }

    internal abstract class RedisCacheSecurityToken
    {
    }

    internal class RedisResult
    {
      public long Usage { get; private set; }

      public double DPImpact { get; private set; }

      public DateTimeOffset TimeStamp { get; private set; }

      public ResourceState2 State { get; private set; }

      public RedisResult(
        long usage,
        double dpImpact,
        DateTimeOffset timestamp,
        ResourceState2 state)
      {
        this.Usage = usage;
        this.DPImpact = dpImpact;
        this.TimeStamp = timestamp;
        this.State = state;
      }

      public double CalculateCurrentDPImpact(
        DateTimeOffset utcNow,
        double dpImpactDecayTimeConstant)
      {
        bool flag = (this.State & ResourceState2.DPFlag) > ResourceState2.Normal;
        if (!flag && this.DPImpact == 0.0)
          return 0.0;
        double totalSeconds = (utcNow - this.TimeStamp).TotalSeconds;
        double currentDpImpact;
        if (flag)
        {
          currentDpImpact = this.DPImpact + totalSeconds;
        }
        else
        {
          currentDpImpact = dpImpactDecayTimeConstant <= 0.0 ? this.DPImpact - totalSeconds : this.DPImpact * Math.Exp(-totalSeconds / dpImpactDecayTimeConstant);
          if (currentDpImpact < 6.0)
            currentDpImpact = 0.0;
        }
        return currentDpImpact;
      }

      internal (long replenishedUsage, double dpImpact) CalculateReplenishedUsageAndDPImpact(
        DateTimeOffset utcNow,
        long dpFlagThreshold,
        double dpImpactDecayTimeConstant,
        bool exponentialDecay)
      {
        long num1 = !exponentialDecay ? Math.Min((long) Math.Max(Math.Ceiling((double) this.Usage * (300.0 - (utcNow - this.TimeStamp).TotalSeconds) / (double) Math.Max(300, 1)), 0.0), this.Usage) : (long) ((double) this.Usage * Math.Exp(-(utcNow - this.TimeStamp).TotalSeconds / (double) Math.Max(300, 1)));
        double currentDpImpact1;
        if ((this.State & ResourceState2.DPFlag) > ResourceState2.Normal && num1 <= dpFlagThreshold && dpFlagThreshold != -1L)
        {
          double num2 = Math.Max(0.0, (utcNow - this.TimeStamp).TotalSeconds);
          DateTimeOffset dateTimeOffset = this.TimeStamp.AddSeconds(Math.Min(1.0, Math.Max(0.0, (double) (this.Usage - dpFlagThreshold) / (double) Math.Max(1L, this.Usage - num1))) * num2);
          double currentDpImpact2 = this.CalculateCurrentDPImpact(dateTimeOffset, dpImpactDecayTimeConstant);
          ResourceState2 state = this.State & (ResourceState2) 65534;
          currentDpImpact1 = new ResourceUtilization2Service.RedisResult(dpFlagThreshold, currentDpImpact2, dateTimeOffset, state).CalculateCurrentDPImpact(utcNow, dpImpactDecayTimeConstant);
        }
        else
          currentDpImpact1 = this.CalculateCurrentDPImpact(utcNow, dpImpactDecayTimeConstant);
        return (num1, currentDpImpact1);
      }
    }

    internal class RequestConcurrentCounter : IRequestResourceCounter
    {
      private ProcessedRURule m_rule;
      private Guid m_namespaceId;
      private string m_key;
      private ResourceUtilization2Service.ConcurrentCounter m_resourceCounter;
      private bool m_releaseSemaphore;

      public RequestConcurrentCounter(
        ProcessedRURule rule,
        Guid namespaceId,
        string key,
        ResourceUtilization2Service.ConcurrentCounter resourceCounter)
      {
        this.m_rule = rule;
        this.m_namespaceId = namespaceId;
        this.m_key = key;
        this.m_resourceCounter = resourceCounter;
        this.m_releaseSemaphore = false;
      }

      public ProcessedRURule Rule => this.m_rule;

      public Guid NamespaceId => this.m_namespaceId;

      public string Key => this.m_key;

      public async Task<SemaphoreWaitStatus> WaitAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken)
      {
        SemaphoreWaitStatus semaphoreWaitStatus = new SemaphoreWaitStatus();
        if (!this.m_releaseSemaphore)
        {
          semaphoreWaitStatus = await this.m_resourceCounter.WaitAsync(timeout, cancellationToken);
          this.m_releaseSemaphore = !semaphoreWaitStatus.EnterSemaphoreFailed;
        }
        return semaphoreWaitStatus;
      }

      public long Decrement() => this.m_resourceCounter.Decrement(this.m_releaseSemaphore);
    }

    internal class ConcurrentCounter : IDisposable
    {
      private long m_concurrentCount;
      private SemaphoreSlim m_semaphoreSlim;
      protected static readonly string s_area = "ResourceUtilizationService";

      public long ConcurrentCount => this.m_concurrentCount;

      public long MaximumCount { get; set; }

      public DateTime LastUpdate { get; private set; }

      public ConcurrentCounter(long initialCount)
      {
        this.m_concurrentCount = 0L;
        this.m_semaphoreSlim = initialCount > 0L ? new SemaphoreSlim((int) initialCount) : (SemaphoreSlim) null;
        this.MaximumCount = 0L;
        this.LastUpdate = DateTime.UtcNow;
      }

      public void Dispose()
      {
        if (this.m_semaphoreSlim == null)
          return;
        this.m_semaphoreSlim.Dispose();
        this.m_semaphoreSlim = (SemaphoreSlim) null;
      }

      public long Increment()
      {
        long concurrentCount;
        long num;
        do
        {
          concurrentCount = this.m_concurrentCount;
          if (concurrentCount < 0L)
            return -1;
          num = concurrentCount + 1L;
        }
        while (concurrentCount != Interlocked.CompareExchange(ref this.m_concurrentCount, num, concurrentCount));
        this.LastUpdate = DateTime.UtcNow;
        if (this.MaximumCount < num)
          this.MaximumCount = num;
        return num;
      }

      public async Task<SemaphoreWaitStatus> WaitAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken)
      {
        SemaphoreWaitStatus status = new SemaphoreWaitStatus();
        if (this.m_semaphoreSlim != null)
        {
          try
          {
            long semaphoreStartTimestamp = Stopwatch.GetTimestamp();
            status.EnterSemaphoreFailed = !await this.m_semaphoreSlim.WaitAsync(timeout, cancellationToken);
            status.TicksSpentInConcurrencySemaphore = Stopwatch.GetTimestamp() - semaphoreStartTimestamp;
            return status;
          }
          catch (OperationCanceledException ex)
          {
            throw;
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(522304014, ResourceUtilization2Service.ConcurrentCounter.s_area, ResourceUtilization2Service.ConcurrentCounter.s_area, ex);
          }
        }
        return status;
      }

      public long Decrement(bool releaseSemaphore)
      {
        if (releaseSemaphore)
        {
          if (this.m_semaphoreSlim != null)
          {
            try
            {
              this.m_semaphoreSlim.Release();
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(522304015, ResourceUtilization2Service.ConcurrentCounter.s_area, ResourceUtilization2Service.ConcurrentCounter.s_area, ex);
            }
          }
        }
        this.LastUpdate = DateTime.UtcNow;
        long num = Interlocked.Decrement(ref this.m_concurrentCount);
        return num >= 0L ? num : throw new InvalidOperationException(string.Format("Counter was incorrectly decremented to {0}", (object) num));
      }

      public bool CanCleanup(TimeSpan expiration) => this.m_concurrentCount < 0L || this.m_concurrentCount == 0L && this.LastUpdate.Add(expiration) <= DateTime.UtcNow && Interlocked.CompareExchange(ref this.m_concurrentCount, -1L, 0L) == 0L;
    }

    internal class IncrementResourceTaskArgs : EventArgs
    {
      public IncrementResourceTaskArgs(
        Guid namespaceId,
        string key,
        long data,
        long executionTimeMilliseconds)
      {
        this.NamespaceId = namespaceId;
        this.Key = key;
        this.Data = data;
        this.ExecutionTimeMilliseconds = executionTimeMilliseconds;
      }

      public Guid NamespaceId { get; private set; }

      public string Key { get; private set; }

      public long Data { get; private set; }

      public long ExecutionTimeMilliseconds { get; private set; }
    }

    internal class ServiceSettings
    {
      public readonly TimeSpan CleanupInterval;
      public readonly TimeSpan IncrementInterval;
      public readonly TimeSpan MaxIncrementDuration;
      public readonly TimeSpan InitialDelay;
      public readonly TimeSpan MaxDelay;
      public readonly TimeSpan ConcurrencySemaphoreTimeout;
      public readonly bool BlockOnTimeout;
      public readonly bool SendConcurrencyBlockNotifications;
      public readonly TimeSpan MinRetryAfter;
      public readonly double DelayPowerLawIndex;
      public readonly double MaxIncrementFactor;
      public readonly double PostRequestTarpittingFactor;
      public readonly double PostRequestTarpittingRelativeCap;
      public readonly TimeSpan PostRequestTarpittingAbsoluteCap;
      public readonly double ModulationFactor;
      public readonly double DelayRandomizationFactor;
      public readonly TimeSpan DelayTimeToAlwaysAdd;
      public readonly bool SpoofIPAddresses;
      public readonly double DPSignificanceThresholdFraction;
      public readonly double DPImpactDecayTimeConstant;
      public readonly double DPImpactNotificationThreshold;
      public readonly bool DoNotTarpit;
      public readonly bool DoNotBlockDistributed;
      public readonly bool OutputThrottleInfoToTelemetry;
      public readonly Guid Version;

      public ServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, ResourceUtilization2Service.s_serviceSettingsRegistryQuery);
        this.CleanupInterval = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (CleanupInterval), TimeSpan.FromMinutes(5.0));
        this.CleanupInterval = this.ClampSetting<TimeSpan>(this.CleanupInterval, TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(1.0));
        this.IncrementInterval = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (IncrementInterval), TimeSpan.FromMinutes(1.0));
        this.IncrementInterval = this.ClampSetting<TimeSpan>(this.IncrementInterval, TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMinutes(5.0));
        this.MaxIncrementDuration = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (MaxIncrementDuration), TimeSpan.FromSeconds(15.0));
        this.InitialDelay = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (InitialDelay), TimeSpan.FromMilliseconds(25.0));
        this.InitialDelay = this.ClampSetting<TimeSpan>(this.InitialDelay, TimeSpan.FromMilliseconds(1.0), TimeSpan.FromMilliseconds(100.0));
        this.MaxDelay = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (MaxDelay), TimeSpan.FromSeconds(25.0));
        this.MaxDelay = this.ClampSetting<TimeSpan>(this.MaxDelay, TimeSpan.FromMilliseconds(1.0), TimeSpan.FromSeconds(60.0));
        this.ConcurrencySemaphoreTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (ConcurrencySemaphoreTimeout), TimeSpan.FromSeconds(25.0));
        this.ConcurrencySemaphoreTimeout = this.ClampSetting<TimeSpan>(this.ConcurrencySemaphoreTimeout, TimeSpan.FromMilliseconds(1.0), TimeSpan.FromSeconds(60.0));
        this.BlockOnTimeout = registryEntryCollection.GetValueFromPath<bool>(nameof (BlockOnTimeout), false);
        this.SendConcurrencyBlockNotifications = registryEntryCollection.GetValueFromPath<bool>(nameof (SendConcurrencyBlockNotifications), false);
        this.MinRetryAfter = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (MinRetryAfter), TimeSpan.FromSeconds(60.0));
        this.MinRetryAfter = this.ClampSetting<TimeSpan>(this.MinRetryAfter, TimeSpan.FromSeconds(0.0), TimeSpan.FromMinutes(5.0));
        this.DelayPowerLawIndex = registryEntryCollection.GetValueFromPath<double>(nameof (DelayPowerLawIndex), 2.0);
        this.DelayPowerLawIndex = this.ClampSetting<double>(this.DelayPowerLawIndex, 0.0, 4.0);
        this.MaxIncrementFactor = registryEntryCollection.GetValueFromPath<double>(nameof (MaxIncrementFactor), 20.0);
        this.MaxIncrementFactor = this.ClampSetting<double>(this.MaxIncrementFactor, 1.0, 1024.0);
        this.PostRequestTarpittingFactor = registryEntryCollection.GetValueFromPath<double>(nameof (PostRequestTarpittingFactor), 1.0);
        this.PostRequestTarpittingFactor = this.ClampSetting<double>(this.PostRequestTarpittingFactor, 0.0, 4.0);
        this.PostRequestTarpittingRelativeCap = registryEntryCollection.GetValueFromPath<double>(nameof (PostRequestTarpittingRelativeCap), 2.0);
        this.PostRequestTarpittingAbsoluteCap = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (PostRequestTarpittingAbsoluteCap), TimeSpan.FromHours(1.0));
        this.ModulationFactor = registryEntryCollection.GetValueFromPath<double>(nameof (ModulationFactor), 0.0);
        this.ModulationFactor = this.ClampSetting<double>(this.ModulationFactor, 0.0, 0.5);
        this.DelayRandomizationFactor = registryEntryCollection.GetValueFromPath<double>(nameof (DelayRandomizationFactor), 0.2);
        this.DelayRandomizationFactor = this.ClampSetting<double>(this.DelayRandomizationFactor, 0.0, 1.0);
        this.DelayTimeToAlwaysAdd = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (DelayTimeToAlwaysAdd), TimeSpan.FromMilliseconds(500.0));
        this.DelayTimeToAlwaysAdd = this.ClampSetting<TimeSpan>(this.DelayTimeToAlwaysAdd, TimeSpan.FromMilliseconds(0.0), TimeSpan.FromMilliseconds(30000.0));
        this.SpoofIPAddresses = registryEntryCollection.GetValueFromPath<bool>(nameof (SpoofIPAddresses), false);
        this.DPSignificanceThresholdFraction = registryEntryCollection.GetValueFromPath<double>(nameof (DPSignificanceThresholdFraction), 0.3333);
        this.DPImpactDecayTimeConstant = registryEntryCollection.GetValueFromPath<double>(nameof (DPImpactDecayTimeConstant), 3600.0);
        this.DPImpactNotificationThreshold = registryEntryCollection.GetValueFromPath<double>(nameof (DPImpactNotificationThreshold), 900.0);
        this.DoNotTarpit = registryEntryCollection.GetValueFromPath<bool>(nameof (DoNotTarpit), false);
        this.DoNotBlockDistributed = registryEntryCollection.GetValueFromPath<bool>(nameof (DoNotBlockDistributed), false);
        this.OutputThrottleInfoToTelemetry = registryEntryCollection.GetValueFromPath<bool>(nameof (OutputThrottleInfoToTelemetry), false);
        this.Version = Guid.NewGuid();
      }

      private T ClampSetting<T>(T value, T min, T max) where T : IComparable<T>
      {
        if (value.CompareTo(min) < 0)
          return min;
        return value.CompareTo(max) > 0 ? max : value;
      }
    }
  }
}
