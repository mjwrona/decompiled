// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ShouldRetryResult
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Runtime.ExceptionServices;

namespace Microsoft.Azure.Documents
{
  internal class ShouldRetryResult
  {
    private static ShouldRetryResult EmptyNoRetry = new ShouldRetryResult()
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
