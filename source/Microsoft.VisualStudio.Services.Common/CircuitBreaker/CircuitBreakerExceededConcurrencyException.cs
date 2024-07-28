// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerExceededConcurrencyException
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  [ExceptionMapping("0.0", "3.0", "CircuitBreakerExceededConcurrencyException", "Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerExceededConcurrencyException, Microsoft.VisualStudio.Services.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class CircuitBreakerExceededConcurrencyException : CircuitBreakerException
  {
    public CircuitBreakerExceededConcurrencyException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }

    protected CircuitBreakerExceededConcurrencyException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
