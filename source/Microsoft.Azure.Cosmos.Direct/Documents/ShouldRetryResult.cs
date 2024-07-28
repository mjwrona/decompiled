// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ShouldRetryResult
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Runtime.ExceptionServices;

namespace Microsoft.Azure.Documents
{
  internal class ShouldRetryResult
  {
    private static readonly ShouldRetryResult EmptyNoRetry = new ShouldRetryResult()
    {
      ShouldRetry = false
    };

    protected ShouldRetryResult()
    {
    }

    public bool ShouldRetry { get; protected set; }

    public TimeSpan BackoffTime { get; protected set; }

    public Exception ExceptionToThrow { get; protected set; }

    public void ThrowIfDoneTrying(ExceptionDispatchInfo capturedException)
    {
      if (this.ShouldRetry)
        return;
      if (this.ExceptionToThrow == null)
        capturedException.Throw();
      if (capturedException == null || this.ExceptionToThrow != capturedException.SourceException)
        throw this.ExceptionToThrow;
      capturedException.Throw();
    }

    public static ShouldRetryResult NoRetry(Exception exception = null)
    {
      if (exception == null)
        return ShouldRetryResult.EmptyNoRetry;
      return new ShouldRetryResult()
      {
        ShouldRetry = false,
        ExceptionToThrow = exception
      };
    }

    public static ShouldRetryResult RetryAfter(TimeSpan backoffTime) => new ShouldRetryResult()
    {
      ShouldRetry = true,
      BackoffTime = backoffTime
    };
  }
}
