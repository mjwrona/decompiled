// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CircuitBreakerPerfCounters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class CircuitBreakerPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreaker";
    internal const string SuccessPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerSuccessPerSec";
    internal const string FailurePerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFailurePerSec";
    internal const string TimeoutPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerTimeoutPerSec";
    internal const string ShortCircuitedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerShortCircuitedPerSec";
    internal const string FallbackSuccessPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackSuccessPerSec";
    internal const string FallbackFailurePerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackFailurePerSec";
    internal const string CommandExecutionTime = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerCommandExecutionTime";
    internal const string ConcurrencyRejectedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerConcurrencyRejectedPerSec";
    internal const string FallbackConcurrencyRejectedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackConcurrencyRejectedPerSec";
    internal const string ExecutionConcurrency = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerExecutionConcurrency";
    internal const string FallbackConcurrency = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackConcurrency";
    internal const string LimitRejectedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerLimitRejectedPerSec";
    internal const string FallbackLimitRejectedPerSec = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackLimitRejectedPerSec";
    internal const string ExecutionCount = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerExecutionCount";
    internal const string FallbackCount = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerFallbackCount";
    internal const string MaxExecutionConcurrency = "Microsoft.TeamFoundation.Framework.Server.Perf_Service_CircuitBreakerMaxExecutionConcurrency";
  }
}
