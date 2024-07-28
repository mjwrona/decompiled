// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.LongRunningActionLogger
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class LongRunningActionLogger : IDisposable
  {
    private const string ThresholdRegistryKey = "/Service/CodeSense/Settings/LongRunningActionThreshold";
    private static readonly TimeSpan DefaultThreshold = TimeSpan.FromMinutes(5.0);
    private static TimeSpan threshold;
    private readonly IVssRequestContext requestContext;
    private readonly string thresholdExceededMessage;
    private readonly Stopwatch stopwatch;

    public LongRunningActionLogger(
      IVssRequestContext requestContext,
      string thresholdExceededMessage)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(thresholdExceededMessage, nameof (thresholdExceededMessage));
      this.requestContext = requestContext;
      this.thresholdExceededMessage = thresholdExceededMessage;
      if (LongRunningActionLogger.threshold == new TimeSpan())
      {
        using (new CodeSenseTraceWatch(this.requestContext, 1024800, TraceLayer.ExternalFramework, "Get long running action threshold {0}", new object[1]
        {
          (object) "/Service/CodeSense/Settings/LongRunningActionThreshold"
        }))
          LongRunningActionLogger.threshold = requestContext.GetService<IVssRegistryService>().GetOrDefault<TimeSpan>(requestContext, "/Service/CodeSense/Settings/LongRunningActionThreshold", LongRunningActionLogger.DefaultThreshold);
      }
      this.stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
      this.stopwatch.Stop();
      if (!(this.stopwatch.Elapsed > LongRunningActionLogger.threshold))
        return;
      string message = string.Format("Action exceeded threshold.  Threshold: {0}.  Elapsed Time: {1}.  Action info: {2}.", (object) LongRunningActionLogger.threshold, (object) this.stopwatch.Elapsed, (object) this.thresholdExceededMessage);
      TeamFoundationEventLog.Default.Log(this.requestContext, message, 20006, EventLogEntryType.Information);
    }
  }
}
