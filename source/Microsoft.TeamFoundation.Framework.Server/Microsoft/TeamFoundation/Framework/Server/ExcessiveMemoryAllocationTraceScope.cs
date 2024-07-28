// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExcessiveMemoryAllocationTraceScope
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ExcessiveMemoryAllocationTraceScope : IDisposable
  {
    private IVssRequestContext RequestContext { get; set; }

    public int Tracepoint { get; private set; }

    public TraceLevel TraceLevel { get; private set; }

    public string Area { get; private set; }

    public string Layer { get; private set; }

    public Lazy<string> CustomInfo { get; private set; }

    public bool TraceAlways { get; private set; }

    public int ThresholdMB { get; set; }

    public string MethodName { get; private set; }

    public long CtorAllocatedBytes { get; private set; }

    public ExcessiveMemoryAllocationTraceScope(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Lazy<string> customInfo = null,
      int thresholdMB = 50,
      TraceLevel traceLevel = TraceLevel.Warning,
      bool traceAlways = true,
      [CallerMemberName] string caller = "")
    {
      this.RequestContext = requestContext;
      this.Tracepoint = tracepoint;
      this.Area = area;
      this.Layer = layer;
      this.CustomInfo = customInfo;
      this.ThresholdMB = thresholdMB;
      this.TraceLevel = traceLevel;
      this.TraceAlways = traceAlways;
      this.MethodName = caller;
      this.CtorAllocatedBytes = GC.GetAllocatedBytesForCurrentThread();
    }

    public void Dispose()
    {
      if (!this.TraceAlways && !this.RequestContext.IsTracing(this.Tracepoint, this.TraceLevel, this.Area, this.Layer))
        return;
      long num = (GC.GetAllocatedBytesForCurrentThread() - this.CtorAllocatedBytes) / 1024L / 1024L;
      if (num < (long) this.ThresholdMB)
        return;
      this.RequestContext.TraceAlways(this.Tracepoint, this.TraceLevel, this.Area, this.Layer, new
      {
        Msg = ("Excessive memory allocation in " + this.MethodName + "()!"),
        AllocatedMB = num,
        CustomInfo = this.TryGetCustomInfo()
      }.Serialize());
    }

    private string TryGetCustomInfo()
    {
      try
      {
        return this.CustomInfo != null ? this.CustomInfo.Value : "";
      }
      catch (Exception ex)
      {
        return string.Format("{0} was thrown during lazy initialization of CustomInfo!", (object) ex.GetType());
      }
    }
  }
}
