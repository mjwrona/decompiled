// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.EuiiDetectionResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  public readonly struct EuiiDetectionResult
  {
    public readonly bool ShouldTraceCounterInfo;
    public readonly long EmailLeakCount;
    public readonly long TracesCheckedCount;
    public readonly long CPUCycles;
    public readonly bool RedactionEnabled;

    public EuiiDetectionResult(
      bool shouldTraceCounterInfo,
      long emailLeakCounter,
      long tracesCheckedCounter,
      long cpuCycles,
      bool redactionEnabled)
    {
      this.ShouldTraceCounterInfo = shouldTraceCounterInfo;
      this.EmailLeakCount = emailLeakCounter;
      this.TracesCheckedCount = tracesCheckedCounter;
      this.CPUCycles = cpuCycles;
      this.RedactionEnabled = redactionEnabled;
    }
  }
}
