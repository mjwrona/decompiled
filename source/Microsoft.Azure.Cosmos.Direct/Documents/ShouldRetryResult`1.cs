// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ShouldRetryResult`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal class ShouldRetryResult<TPolicyArg1> : ShouldRetryResult
  {
    private static readonly ShouldRetryResult<TPolicyArg1> EmptyNoRetry;

    public TPolicyArg1 PolicyArg1 { get; private set; }

    public static ShouldRetryResult<TPolicyArg1> NoRetry(Exception exception = null)
    {
      if (exception == null)
        return ShouldRetryResult<TPolicyArg1>.EmptyNoRetry;
      ShouldRetryResult<TPolicyArg1> shouldRetryResult = new ShouldRetryResult<TPolicyArg1>();
      shouldRetryResult.ShouldRetry = false;
      shouldRetryResult.ExceptionToThrow = exception;
      return shouldRetryResult;
    }

    public static ShouldRetryResult<TPolicyArg1> RetryAfter(
      TimeSpan backoffTime,
      TPolicyArg1 policyArg1)
    {
      ShouldRetryResult<TPolicyArg1> shouldRetryResult = new ShouldRetryResult<TPolicyArg1>();
      shouldRetryResult.ShouldRetry = true;
      shouldRetryResult.BackoffTime = backoffTime;
      shouldRetryResult.PolicyArg1 = policyArg1;
      return shouldRetryResult;
    }

    static ShouldRetryResult()
    {
      ShouldRetryResult<TPolicyArg1> shouldRetryResult = new ShouldRetryResult<TPolicyArg1>();
      shouldRetryResult.ShouldRetry = false;
      ShouldRetryResult<TPolicyArg1>.EmptyNoRetry = shouldRetryResult;
    }
  }
}
