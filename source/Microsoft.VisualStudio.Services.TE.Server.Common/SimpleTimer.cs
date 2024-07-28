// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.SimpleTimer
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class SimpleTimer : IDisposable
  {
    private readonly Stopwatch _timer;
    private readonly string _name;
    private readonly long _threshold;
    private bool _disposed;
    private readonly bool _publishTelemetry;
    private readonly TestExecutionRequestContext _requestContext;
    private readonly DtaLogger _logger;
    private readonly string _sessionId;

    public SimpleTimer(TestExecutionRequestContext requestContext, string timerName)
      : this(requestContext, timerName, 1000L, sessionId: string.Empty)
    {
    }

    public SimpleTimer(
      TestExecutionRequestContext requestContext,
      string timerName,
      string sessionId)
      : this(requestContext, timerName, 1000L, sessionId: sessionId)
    {
    }

    public SimpleTimer(
      TestExecutionRequestContext requestContext,
      string timerName,
      bool publishTelemetry,
      string sessionId)
      : this(requestContext, timerName, 1000L, publishTelemetry, sessionId)
    {
    }

    public SimpleTimer(
      TestExecutionRequestContext requestContext,
      string timerName,
      long thresholdInMilliseconds = 2147483647,
      bool publishTelemetry = false,
      string sessionId = "")
    {
      this._name = timerName;
      this._threshold = thresholdInMilliseconds;
      this._timer = Stopwatch.StartNew();
      this._publishTelemetry = publishTelemetry;
      this._requestContext = requestContext;
      this._logger = new DtaLogger(requestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.SimpleTimer);
      this._sessionId = sessionId;
    }

    public void Dispose() => this.Dispose(true);

    public void StopAndLog()
    {
      this._timer.Stop();
      if (this._threshold == 0L)
      {
        this._logger.Info(6205000, string.Format("PERF: {0}: took {1} ms", (object) this._name, (object) this._timer.ElapsedMilliseconds));
      }
      else
      {
        if (this._timer.ElapsedMilliseconds < this._threshold)
          return;
        this._logger.Info(6205001, string.Format("PERF WARNING: {0}: took {1} ms", (object) this._name, (object) this._timer.ElapsedMilliseconds));
        if (!this._publishTelemetry)
          return;
        CILogger.Instance.PublishCI(this._requestContext, "AutomationRunExecutionDetails", new Dictionary<string, object>()
        {
          {
            "SimpleTimerSessionId",
            (object) this._sessionId
          },
          {
            "SimpleTimerCounterName",
            (object) this._name
          },
          {
            "SimpleTimerElapsedTime",
            (object) this._timer.ElapsedMilliseconds
          }
        });
      }
    }

    private void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing)
        this.StopAndLog();
      this._disposed = true;
    }
  }
}
