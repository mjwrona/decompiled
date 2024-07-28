// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.VssThrottlingFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class VssThrottlingFaultMapper : FaultMapper
  {
    public VssThrottlingFaultMapper()
      : base("VssThrottling", IndexerFaultSource.TFS)
    {
    }

    public override bool IsMatch(Exception ex)
    {
      if (ex == null)
        return false;
      return ex.ToString().Contains("Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerExceededConcurrencyException") || ex.ToString().Contains("Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerShortCircuitException") || ex.ToString().Contains("HttpClientThrottler-") || ex.ToString().Contains("System.Threading.Tasks.TaskCanceledException") && !ex.ToString().Contains("System.TimeoutException") || ex.ToString().Contains("TF400733");
    }
  }
}
