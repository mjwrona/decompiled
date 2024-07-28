// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.Management.RegisteredMetricManager
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Microsoft.VisualStudio.Telemetry.Metrics.Management
{
  public class RegisteredMetricManager : IDisposable
  {
    public const int DEFAULT_MAX_CONCURRENT_METRICS = 255;
    public const int DEFAULT_TIMEOUT_IN_SECONDS = 120;
    public readonly int MaxConcurrentMetrics;
    private readonly int timeoutInSeconds;
    private readonly TimeSpan timeout;
    private object metricSetSync = new object();
    private bool disposedValue;

    internal RegisteredCounterSet Counters { get; private set; }

    internal RegisteredHistogramSet Histograms { get; private set; }

    internal bool IsExpirationTimerRunning => this.ExpirationTimer != null && this.ExpirationTimer.Enabled;

    private TelemetrySession Session { get; set; }

    private Timer ExpirationTimer { get; set; }

    public int ConcurrentMetricsCount => this.Counters.ConcurrentMetricsCount + this.Histograms.ConcurrentMetricsCount;

    public RegisteredMetricManager(
      TelemetrySession session,
      int maxConcurrentMetrics = 255,
      int timeoutInSeconds = 120)
    {
      this.MaxConcurrentMetrics = maxConcurrentMetrics;
      this.Counters = new RegisteredCounterSet();
      this.Histograms = new RegisteredHistogramSet();
      this.Session = session;
      this.timeoutInSeconds = timeoutInSeconds;
      this.timeout = TimeSpan.FromSeconds((double) this.timeoutInSeconds);
    }

    public void RecordCounterData<T>(
      string key,
      T data,
      string metricName,
      TelemetryEvent metricEvent)
      where T : struct
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (metricName == null)
        throw new ArgumentNullException(nameof (metricName));
      if (metricEvent == null)
        throw new ArgumentNullException(nameof (metricEvent));
      this.RecordInternal<T>((RegisteredMetricSetBase) this.Counters, key, data, metricName, metricEvent);
    }

    public void RecordHistogramData<T>(
      string key,
      T data,
      string metricName,
      TelemetryEvent metricEvent,
      double[] buckets = null)
      where T : struct
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (metricName == null)
        throw new ArgumentNullException(nameof (metricName));
      if (metricEvent == null)
        throw new ArgumentNullException(nameof (metricEvent));
      this.RecordInternal<T>((RegisteredMetricSetBase) this.Histograms, key, data, metricName, metricEvent, buckets);
    }

    public void CloseCounter(TelemetrySession session, string key)
    {
      if (session == null)
        throw new ArgumentNullException(nameof (session));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      lock (this.metricSetSync)
        this.CloseInternal((RegisteredMetricSetBase) this.Counters, session, key);
    }

    public void CloseHistogram(TelemetrySession session, string key)
    {
      if (session == null)
        throw new ArgumentNullException(nameof (session));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      lock (this.metricSetSync)
        this.CloseInternal((RegisteredMetricSetBase) this.Histograms, session, key);
    }

    private void RecordInternal<T>(
      RegisteredMetricSetBase set,
      string key,
      T data,
      string metricName,
      TelemetryEvent metricEvent,
      double[] buckets = null)
      where T : struct
    {
      if (set.GetMetric(key) == null && this.ConcurrentMetricsCount >= this.MaxConcurrentMetrics)
        throw new RegisteredMetricLimitExceededException();
      lock (this.metricSetSync)
      {
        set.Record<T>(key, data, metricName, metricEvent, this.timeout, buckets);
        this.StartExpiryTimer();
      }
    }

    private void CloseInternal(RegisteredMetricSetBase set, TelemetrySession session, string key)
    {
      RegisteredMetric metric = set.GetMetric(key);
      if (metric != null)
        session.PostMetricEvent(metric.MetricEvent);
      set.CloseMetric(key);
      if (this.ConcurrentMetricsCount != 0)
        return;
      this.StopExpiryTimer();
    }

    private void StartExpiryTimer()
    {
      if (this.ExpirationTimer == null)
      {
        this.ExpirationTimer = new Timer(this.timeout.TotalMilliseconds);
        this.ExpirationTimer.Elapsed += new ElapsedEventHandler(this.CloseExpired);
      }
      this.ExpirationTimer?.Start();
    }

    private void StopExpiryTimer() => this.ExpirationTimer?.Stop();

    private void CloseExpired(object sender, ElapsedEventArgs e)
    {
      DateTime utcNow = DateTime.UtcNow;
      lock (this.metricSetSync)
      {
        this.CloseExpiredInSet((RegisteredMetricSetBase) this.Counters, this.Session, utcNow);
        this.CloseExpiredInSet((RegisteredMetricSetBase) this.Histograms, this.Session, utcNow);
      }
    }

    private void CloseExpiredInSet(
      RegisteredMetricSetBase set,
      TelemetrySession session,
      DateTime currentTime)
    {
      List<string> list1 = set.IntegerMetrics.Keys.ToList<string>();
      List<string> list2 = set.FloatingPointMetrics.Keys.ToList<string>();
      this.CloseExpiredInMetricType(set, list1, session, currentTime);
      this.CloseExpiredInMetricType(set, list2, session, currentTime);
    }

    private void CloseExpiredInMetricType(
      RegisteredMetricSetBase set,
      List<string> keys,
      TelemetrySession session,
      DateTime currentTime)
    {
      foreach (string key in keys)
      {
        RegisteredMetric metric = set.GetMetric(key);
        if (metric != null && metric != null && metric.Expiry <= currentTime)
          this.CloseInternal(set, session, metric.Key);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        this.ExpirationTimer?.Dispose();
        this.ExpirationTimer = (Timer) null;
        this.Session = (TelemetrySession) null;
      }
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
