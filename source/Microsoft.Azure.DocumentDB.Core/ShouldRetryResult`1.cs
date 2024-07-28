// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ShouldRetryResult`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal class ShouldRetryResult<TPolicyArg1> : ShouldRetryResult
  {
    public TPolicyArg1 PolicyArg1 { get; private set; }

    public static ShouldRetryResult<TPolicyArg1> NoRetry(Exception exception = null)
    {
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
  }
}
