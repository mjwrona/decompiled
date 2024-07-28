// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.CodeSenseTraceWatch
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public sealed class CodeSenseTraceWatch : IDisposable
  {
    private readonly TraceWatch traceWatch;
    private readonly IVssRequestContext requestContext;
    private readonly Dictionary<string, object> ciData;

    public CodeSenseTraceWatch(
      IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] args)
    {
      this.traceWatch = new TraceWatch(requestContext, tracepoint, TraceLevel.Info, TimeSpan.MinValue, "CodeSense", layer, format, args);
    }

    public CodeSenseTraceWatch(
      IVssRequestContext requestContext,
      int tracepoint,
      bool shouldQueryRegistryForTimeout,
      string layer,
      string format,
      params object[] args)
    {
      TimeSpan timeout = TimeSpan.MinValue;
      if (shouldQueryRegistryForTimeout)
        timeout = TimeSpan.FromMilliseconds(requestContext.GetService<IVssRegistryService>().GetTimeElapsedThresholdForTraceWatch(requestContext, tracepoint));
      this.traceWatch = new TraceWatch(requestContext, tracepoint, TraceLevel.Info, timeout, "CodeSense", layer, format, args);
      this.ciData = new Dictionary<string, object>();
      this.ciData.Add(CodeLensCIProperty.ActivityId, (object) requestContext.ActivityId.ToString());
      this.ciData.Add(CodeLensCIProperty.LayerCake, (object) string.Format(format, args));
      this.requestContext = requestContext;
    }

    public void Dispose()
    {
      this.traceWatch.Dispose();
      if (this.requestContext == null || this.ciData == null)
        return;
      this.requestContext.PublishLayerCakeCI(CodeLensCILevel.Important, CodeLensCIArea.CodeLensService, CodeLensCIFeature.LayerCakeAnalysis, this.ciData, this.traceWatch.TimeElapsed);
      this.ciData.Clear();
    }
  }
}
