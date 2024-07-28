// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SimpleTimer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class SimpleTimer : IDisposable
  {
    private readonly Stopwatch _timer;
    private readonly string _name;
    private readonly long _threshold;
    private bool _disposed;
    private readonly bool _publishTelemetry;
    private readonly bool _alwaysPublishTelemetry;
    private readonly IVssRequestContext _requestContext;
    private readonly TCMLogger _tcmLogger;
    private Dictionary<string, object> _ciData;

    public SimpleTimer(IVssRequestContext requestContext, string timerName, bool publishTelemetry = true)
      : this(requestContext, timerName, (Dictionary<string, object>) null, publishTelemetry: publishTelemetry)
    {
    }

    public SimpleTimer(
      IVssRequestContext requestContext,
      string timerName,
      Dictionary<string, object> ciData)
      : this(requestContext, timerName, ciData, 1000L, true, false)
    {
    }

    public SimpleTimer(
      IVssRequestContext requestContext,
      string timerName,
      long thresholdInMilliseconds,
      bool publishTelemetry = true)
      : this(requestContext, timerName, (Dictionary<string, object>) null, thresholdInMilliseconds, publishTelemetry)
    {
    }

    public SimpleTimer(
      IVssRequestContext requestContext,
      string timerName,
      Dictionary<string, object> ciData,
      long thresholdInMilliseconds = 1000,
      bool publishTelemetry = true,
      bool alwaysPublishTelemetry = false)
    {
      this._name = timerName;
      this._threshold = thresholdInMilliseconds;
      this._timer = Stopwatch.StartNew();
      this._publishTelemetry = publishTelemetry;
      this._alwaysPublishTelemetry = alwaysPublishTelemetry;
      this._requestContext = requestContext;
      this._tcmLogger = new TCMLogger(requestContext);
      this._ciData = ciData;
    }

    public void Dispose() => this.Dispose(true);

    public void StopAndLog()
    {
      this._timer.Stop();
      if (this._threshold == 0L)
      {
        this._tcmLogger.Info(1015119, "PERF: %s: took %s ms", (object) this._name, (object) this._timer.ElapsedMilliseconds);
      }
      else
      {
        if (this._timer.ElapsedMilliseconds > this._threshold)
        {
          this._tcmLogger.Warning(1015120, "PERF: %s: took %s ms", (object) this._name, (object) this._timer.ElapsedMilliseconds);
          if (this._publishTelemetry)
          {
            if (this._ciData != null)
              this.AddCIData(this._ciData, true);
            else
              this.PublishTelemetryData(true);
          }
        }
        if (!this._alwaysPublishTelemetry)
          return;
        if (this._ciData != null)
          this.AddCIData(this._ciData, false);
        else
          this.PublishTelemetryData(false);
      }
    }

    private void AddCIData(Dictionary<string, object> ciData, bool isThresholdViolated)
    {
      ciData["TotalTimeTakenFor" + this._name + "InMs"] = (object) this._timer.ElapsedMilliseconds;
      ciData["ThresholdViolatedFor" + this._name] = (object) isThresholdViolated;
    }

    private void PublishTelemetryData(bool isThresholdViolated)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      this.AddCIData(dictionary, isThresholdViolated);
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
      TelemetryLogger.Instance.PublishData(this._requestContext, nameof (SimpleTimer), cid);
    }

    private void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
      {
        this.StopAndLog();
        GC.SuppressFinalize((object) this);
      }
      this._disposed = true;
    }
  }
}
