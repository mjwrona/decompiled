// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerNoOpImpl
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  internal class CircuitBreakerNoOpImpl : ICircuitBreaker
  {
    internal CircuitBreakerNoOpImpl()
    {
      this.ExecutionSemaphore = (ITryableSemaphore) new TryableSemaphoreNoOpImpl();
      this.FallbackSemaphore = (ITryableSemaphore) new TryableSemaphoreNoOpImpl();
      this.ExecutionRequests = (IRollingNumber) new RollingNumberNoOpImpl();
      this.FallbackRequests = (IRollingNumber) new RollingNumberNoOpImpl();
    }

    public bool AllowRequest(ICommandProperties properties) => true;

    public bool IsOpen(ICommandProperties properties) => false;

    public CircuitBreakerStatus GetCircuitBreakerState(ICommandProperties properties) => CircuitBreakerStatus.Closed;

    public void MarkSuccess()
    {
    }

    public bool IsOlderThan(TimeSpan time) => false;

    public ITryableSemaphore ExecutionSemaphore { get; private set; }

    public ITryableSemaphore FallbackSemaphore { get; private set; }

    public IRollingNumber ExecutionRequests { get; private set; }

    public IRollingNumber FallbackRequests { get; private set; }
  }
}
