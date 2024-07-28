// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RetryOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal class RetryOptions
  {
    internal const int DefaultMaxRetryWaitTimeInSeconds = 30;
    internal const int DefaultMaxRetryAttemptsOnThrottledRequests = 9;
    private readonly RetryWithConfiguration retryWithConfiguration;
    private int maxRetryAttemptsOnThrottledRequests;
    private int maxRetryWaitTime;

    public RetryOptions()
    {
      this.maxRetryAttemptsOnThrottledRequests = 9;
      this.maxRetryWaitTime = 30;
      this.retryWithConfiguration = new RetryWithConfiguration();
    }

    public int MaxRetryAttemptsOnThrottledRequests
    {
      get => this.maxRetryAttemptsOnThrottledRequests;
      set => this.maxRetryAttemptsOnThrottledRequests = value >= 0 ? value : throw new ArgumentException("value must be a positive integer.");
    }

    public int MaxRetryWaitTimeInSeconds
    {
      get => this.maxRetryWaitTime;
      set => this.maxRetryWaitTime = value >= 0 && value <= 2147483 ? value : throw new ArgumentException("value must be a positive integer between the range of 0 to " + 2147483.ToString());
    }

    internal int? InitialRetryForRetryWithMilliseconds
    {
      get => this.retryWithConfiguration.InitialRetryIntervalMilliseconds;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num1 = 0;
          if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
          {
            nullable = value;
            int num2 = 2147483;
            if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
              goto label_4;
          }
          throw new ArgumentException("value must be a positive integer between the range of 0 to " + 2147483.ToString());
        }
label_4:
        this.retryWithConfiguration.InitialRetryIntervalMilliseconds = value;
      }
    }

    internal int? MaximumRetryForRetryWithMilliseconds
    {
      get => this.retryWithConfiguration.MaximumRetryIntervalMilliseconds;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num1 = 0;
          if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
          {
            nullable = value;
            int num2 = 2147483;
            if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
              goto label_4;
          }
          throw new ArgumentException("value must be a positive integer between the range of 0 to " + 2147483.ToString());
        }
label_4:
        this.retryWithConfiguration.MaximumRetryIntervalMilliseconds = value;
      }
    }

    internal int? RandomSaltForRetryWithMilliseconds
    {
      get => this.retryWithConfiguration.RandomSaltMaxValueMilliseconds;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num1 = 0;
          if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
          {
            nullable = value;
            int num2 = 2147483;
            if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
              goto label_4;
          }
          throw new ArgumentException("value must be a positive integer between the range of 0 to " + 2147483.ToString());
        }
label_4:
        this.retryWithConfiguration.RandomSaltMaxValueMilliseconds = value;
      }
    }

    internal int? TotalWaitTimeForRetryWithMilliseconds
    {
      get => this.retryWithConfiguration.TotalWaitTimeMilliseconds;
      set
      {
        if (value.HasValue)
        {
          int? nullable = value;
          int num1 = 0;
          if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
          {
            nullable = value;
            int num2 = 2147483;
            if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
              goto label_4;
          }
          throw new ArgumentException("value must be a positive integer between the range of 0 to " + 2147483.ToString());
        }
label_4:
        this.retryWithConfiguration.TotalWaitTimeMilliseconds = value;
      }
    }

    internal RetryWithConfiguration GetRetryWithConfiguration() => this.retryWithConfiguration;
  }
}
