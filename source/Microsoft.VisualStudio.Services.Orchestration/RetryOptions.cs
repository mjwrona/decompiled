// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.RetryOptions
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class RetryOptions
  {
    public RetryOptions(TimeSpan firstRetryInterval, int maxNumberOfAttempts)
    {
      this.FirstRetryInterval = !(firstRetryInterval <= TimeSpan.Zero) ? firstRetryInterval : throw new ArgumentException("Invalid interval.  Specify a TimeSpan value greater then TimeSpan.Zero.", nameof (firstRetryInterval));
      this.MaxNumberOfAttempts = maxNumberOfAttempts;
      this.MaxRetryInterval = TimeSpan.MaxValue;
      this.BackoffCoefficient = 1.0;
      this.RetryTimeout = TimeSpan.MaxValue;
      this.Handle = (Func<Exception, bool>) (e => true);
    }

    public TimeSpan FirstRetryInterval { get; set; }

    public TimeSpan MaxRetryInterval { get; set; }

    public double BackoffCoefficient { get; set; }

    public TimeSpan RetryTimeout { get; set; }

    public int MaxNumberOfAttempts { get; set; }

    public Func<Exception, bool> Handle { get; set; }
  }
}
