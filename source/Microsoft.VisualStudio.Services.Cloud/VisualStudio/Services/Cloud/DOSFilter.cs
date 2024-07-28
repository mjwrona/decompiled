// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DOSFilter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DOSFilter : ITeamFoundationRequestFilter
  {
    private const string c_area = "DOSFilter";
    private static VssPerformanceCounter m_perfCounter_AverageRequestReadyDelayTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageRequestReadyDelayTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageRequestReadyDelayTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageRequestReadyDelayTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageConcurrencySemaphoreTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageConcurrencySemaphoreTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageConcurrencySemaphoreTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageConcurrencySemaphoreTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageRequestReadyTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageRequestReadyTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageRequestReadyTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageRequestReadyTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageBeginRequestTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageBeginRequestTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageBeginRequestTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageBeginRequestTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageEnterMethodTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageEnterMethodTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageEnterMethodTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageEnterMethodTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AveragePostLogDelayTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AveragePostLogDelayTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AveragePostLogDelayTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AveragePostLogDelayTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AveragePostLogTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AveragePostLogTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AveragePostLogTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AveragePostLogTimeBase", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageEndRequestTime = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageEndRequestTime", nameof (DOSFilter));
    private static VssPerformanceCounter m_perfCounter_AverageEndRequestTimeBase = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_ResourceUtilization_AverageEndRequestTimeBase", nameof (DOSFilter));

    public void BeginRequest(IVssRequestContext requestContext)
    {
    }

    public async Task BeginRequestAsync(IVssRequestContext requestContext)
    {
      Stopwatch sw;
      if (!requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity))
        sw = (Stopwatch) null;
      else if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.UseResourceUtilization21"))
      {
        sw = (Stopwatch) null;
      }
      else
      {
        IEnumerable<IResourceUtilizationService> resourceUtilizationServices;
        if (!this.TryGetResourceUtilizationServices(requestContext, out resourceUtilizationServices))
        {
          sw = (Stopwatch) null;
        }
        else
        {
          sw = Stopwatch.StartNew();
          try
          {
            foreach (IResourceUtilizationService utilizationService in resourceUtilizationServices)
              await utilizationService.ThrottleRequestAsync(requestContext, RUStage.BeginRequest);
            sw = (Stopwatch) null;
          }
          finally
          {
            sw.Stop();
            DOSFilter.m_perfCounter_AverageBeginRequestTime.IncrementTicks(sw);
            DOSFilter.m_perfCounter_AverageBeginRequestTimeBase.Increment();
          }
        }
      }
    }

    public async Task PostAuthenticateRequest(IVssRequestContext requestContext)
    {
      Stopwatch sw;
      if (!requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity))
      {
        sw = (Stopwatch) null;
      }
      else
      {
        IEnumerable<IResourceUtilizationService> resourceUtilizationServices;
        if (!this.TryGetResourceUtilizationServices(requestContext, out resourceUtilizationServices))
        {
          sw = (Stopwatch) null;
        }
        else
        {
          sw = Stopwatch.StartNew();
          try
          {
            foreach (IResourceUtilizationService utilizationService in resourceUtilizationServices)
              await utilizationService.ThrottleRequestAsync(requestContext, RUStage.RequestReady);
            sw = (Stopwatch) null;
          }
          finally
          {
            sw.Stop();
            if (requestContext.RequestTimer.DelaySpan > TimeSpan.Zero)
            {
              DOSFilter.m_perfCounter_AverageRequestReadyDelayTime.IncrementTicks(requestContext.RequestTimer.DelaySpan);
              DOSFilter.m_perfCounter_AverageRequestReadyDelayTimeBase.Increment();
            }
            TimeSpan timeSpan1 = TimeSpan.FromMilliseconds(0.001 * (double) requestContext.ConcurrencySemaphoreTime());
            DOSFilter.m_perfCounter_AverageConcurrencySemaphoreTime.IncrementTicks(timeSpan1);
            DOSFilter.m_perfCounter_AverageConcurrencySemaphoreTimeBase.Increment();
            ref VssPerformanceCounter local = ref DOSFilter.m_perfCounter_AverageRequestReadyTime;
            TimeSpan timeSpan2 = sw.Elapsed;
            timeSpan2 = timeSpan2.Subtract(requestContext.RequestTimer.DelaySpan);
            TimeSpan timeSpan3 = timeSpan2.Subtract(timeSpan1);
            local.IncrementTicks(timeSpan3);
            DOSFilter.m_perfCounter_AverageRequestReadyTimeBase.Increment();
          }
        }
      }
    }

    public void EnterMethod(IVssRequestContext requestContext)
    {
      IEnumerable<IResourceUtilizationService> resourceUtilizationServices;
      if (!requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity) || !this.TryGetResourceUtilizationServices(requestContext, out resourceUtilizationServices))
        return;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        foreach (IResourceUtilizationService utilizationService in resourceUtilizationServices)
        {
          utilizationService.ThrottleRequest(requestContext);
          utilizationService.QueuePreMethodResourcesIncrementData(requestContext);
        }
      }
      finally
      {
        stopwatch.Stop();
        DOSFilter.m_perfCounter_AverageEnterMethodTime.IncrementTicks(stopwatch);
        DOSFilter.m_perfCounter_AverageEnterMethodTimeBase.Increment();
      }
    }

    public void LeaveMethod(IVssRequestContext requestContext)
    {
    }

    public async Task PostLogRequestAsync(IVssRequestContext requestContext)
    {
      Stopwatch sw;
      if (!requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity))
      {
        sw = (Stopwatch) null;
      }
      else
      {
        IEnumerable<IResourceUtilizationService> resourceUtilizationServices;
        if (!this.TryGetResourceUtilizationServices(requestContext, out resourceUtilizationServices))
        {
          sw = (Stopwatch) null;
        }
        else
        {
          sw = Stopwatch.StartNew();
          TimeSpan preRequestDelay = requestContext.RequestTimer.DelaySpan;
          TimeSpan zero = TimeSpan.Zero;
          try
          {
            string b;
            if (requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.CancellationReason, out b) && !string.Equals(FrameworkResources.RequestTimeoutException(), b))
            {
              sw = (Stopwatch) null;
            }
            else
            {
              if (requestContext is IWebRequestContextInternal requestContextInternal && requestContextInternal.HttpContext != null)
              {
                CancellationToken cancellationToken = requestContextInternal.HttpContext.Response.ClientDisconnectedToken;
                if (cancellationToken.IsCancellationRequested)
                {
                  cancellationToken = requestContextInternal.HttpContext.Request.TimedOutToken;
                  if (!cancellationToken.IsCancellationRequested)
                  {
                    sw = (Stopwatch) null;
                    return;
                  }
                }
              }
              foreach (IResourceUtilizationService utilizationService in resourceUtilizationServices)
                await utilizationService.ThrottleRequestAsync(requestContext, RUStage.PostLog);
              sw = (Stopwatch) null;
            }
          }
          finally
          {
            sw.Stop();
            TimeSpan timeSpan = requestContext.RequestTimer.DelaySpan - preRequestDelay;
            if (timeSpan > TimeSpan.Zero)
            {
              DOSFilter.m_perfCounter_AveragePostLogDelayTime.IncrementTicks(timeSpan);
              DOSFilter.m_perfCounter_AveragePostLogDelayTimeBase.Increment();
            }
            DOSFilter.m_perfCounter_AveragePostLogTime.IncrementTicks(sw.Elapsed.Subtract(timeSpan));
            DOSFilter.m_perfCounter_AveragePostLogTimeBase.Increment();
          }
        }
      }
    }

    public void EndRequest(IVssRequestContext requestContext)
    {
      bool flag;
      if (requestContext.TryGetItem<bool>("RU2FF", out flag) & flag)
        ResourceUtilization2Service.DecrementRequestCounters(requestContext);
      IEnumerable<IResourceUtilizationService> resourceUtilizationServices;
      if (!requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity) || !this.TryGetResourceUtilizationServices(requestContext, out resourceUtilizationServices))
        return;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        foreach (IResourceUtilizationService utilizationService in resourceUtilizationServices)
          utilizationService.QueuePostMethodResourcesIncrementData(requestContext);
        ThrottleInfo throttleInfo1 = (ThrottleInfo) null;
        requestContext.RootContext.TryGetItem<ThrottleInfo>(RequestContextItemsKeys.ThrottleInfo, out throttleInfo1);
        ThrottleInfo throttleInfo2 = (ThrottleInfo) null;
        requestContext.RootContext.TryGetItem<ThrottleInfo>(RequestContextItemsKeys.ThrottleInfo2, out throttleInfo2);
        if (throttleInfo1 == null && throttleInfo2 == null)
          return;
        if (throttleInfo2 != null)
        {
          if (throttleInfo1 != null)
          {
            if (throttleInfo2.ThrottleType == ResourceState.Block)
              throttleInfo1 = throttleInfo2;
            else if (throttleInfo2.ThrottleType == ResourceState.Tarpit && throttleInfo1.ThrottleType != ResourceState.Block)
              throttleInfo1 = throttleInfo2;
          }
          else
            throttleInfo1 = throttleInfo2;
        }
        if (throttleInfo1.ThrottleType != ResourceState.Tarpit && throttleInfo1.ThrottleType != ResourceState.Block)
          return;
        if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.ThrottleReasonWithRule"))
          requestContext.RootContext.Items[RequestContextItemsKeys.ThrottleReason] = (object) throttleInfo1.GetThrottleReasonWithRule();
        else
          requestContext.RootContext.Items[RequestContextItemsKeys.ThrottleReason] = (object) throttleInfo1.GetThrottleReason();
      }
      finally
      {
        this.ConditionalTraceTelemetry(requestContext);
        stopwatch.Stop();
        DOSFilter.m_perfCounter_AverageEndRequestTime.IncrementTicks(stopwatch);
        DOSFilter.m_perfCounter_AverageEndRequestTimeBase.Increment();
      }
    }

    private bool TryGetResourceUtilizationServices(
      IVssRequestContext requestContext,
      out IEnumerable<IResourceUtilizationService> resourceUtilizationServices)
    {
      resourceUtilizationServices = (IEnumerable<IResourceUtilizationService>) new List<IResourceUtilizationService>();
      bool utilizationServices;
      if (!requestContext.TryGetItem<bool>("RU2FF", out utilizationServices))
      {
        utilizationServices = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2");
        requestContext.Items["RU2FF"] = (object) utilizationServices;
      }
      bool flag;
      if (!requestContext.TryGetItem<bool>("RU21FF", out flag))
      {
        flag = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ResourceUtilization2.UseResourceUtilization21");
        requestContext.Items["RU21FF"] = (object) flag;
      }
      if (utilizationServices)
      {
        if (flag)
          (resourceUtilizationServices as List<IResourceUtilizationService>).Add((IResourceUtilizationService) requestContext.GetService<ResourceUtilization21Service>());
        else
          (resourceUtilizationServices as List<IResourceUtilizationService>).Add((IResourceUtilizationService) requestContext.GetService<ResourceUtilization2Service>());
      }
      return utilizationServices;
    }

    internal void ConditionalTraceTelemetry(IVssRequestContext requestContext)
    {
      IDictionary<string, object> dictionary;
      if (!requestContext.RootContext.TryGetItem<IDictionary<string, object>>(RequestContextItemsKeys.ResourceUtilizationEvents, out dictionary) || !dictionary.Remove("ShouldOutputTelemetry"))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dictionary)
      {
        string str = keyValuePair.Value != null ? (!(keyValuePair.Value is double num) ? keyValuePair.Value.ToString() : num.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : string.Empty;
        stringBuilder.Append(keyValuePair.Key + "=" + str + "; ");
      }
      this.TraceTelemetry(requestContext, stringBuilder.ToString());
    }

    internal virtual void TraceTelemetry(IVssRequestContext requestContext, string datafeed) => requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTracingService>().TraceResourceUtilization(requestContext, 0, datafeed.ToString());

    void ITeamFoundationRequestFilter.PostAuthorizeRequest(IVssRequestContext requestContext)
    {
    }
  }
}
