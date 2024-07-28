// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerTracePoints
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public static class CircuitBreakerTracePoints
  {
    public const int ExecutionFailure = 10003202;
    public const int ExecutionConcurrencyRejection = 10003203;
    public const int ExecutionLimitRejection = 10003201;
    public const int ShortCircuited = 10003204;
    public const int FallbackMissingDelegate = 10003205;
    public const int FallbackFailure = 10003206;
    public const int FallbackConcurrencyRejection = 10003207;
    public const int FallbackLimitRejection = 10003209;
    public const int FallbackDisabled = 10003208;
  }
}
