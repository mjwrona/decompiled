// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.RetryPolicies.LinearRetry
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;

namespace Microsoft.Azure.Storage.RetryPolicies
{
  public sealed class LinearRetry : IExtendedRetryPolicy, IRetryPolicy
  {
    private const int DefaultClientRetryCount = 3;
    private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(30.0);
    private TimeSpan deltaBackoff;
    private int maximumAttempts;
    private DateTimeOffset? lastPrimaryAttempt;
    private DateTimeOffset? lastSecondaryAttempt;

    public LinearRetry()
      : this(LinearRetry.DefaultClientBackoff, 3)
    {
    }

    public LinearRetry(TimeSpan deltaBackoff, int maxAttempts)
    {
      this.deltaBackoff = deltaBackoff;
      this.maximumAttempts = maxAttempts;
    }

    public bool ShouldRetry(
      int currentRetryCount,
      int statusCode,
      Exception lastException,
      out TimeSpan retryInterval,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (lastException), (object) lastException);
      retryInterval = TimeSpan.Zero;
      if (statusCode >= 300 && statusCode < 500 && statusCode != 408 || statusCode == 501 || statusCode == 505 || lastException.Message == "Blob type of the blob reference doesn't match blob type of the blob.")
        return false;
      retryInterval = this.deltaBackoff;
      return currentRetryCount < this.maximumAttempts;
    }

    public RetryInfo Evaluate(RetryContext retryContext, OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (retryContext), (object) retryContext);
      if (retryContext.LastRequestResult.TargetLocation == StorageLocation.Primary)
        this.lastPrimaryAttempt = new DateTimeOffset?((DateTimeOffset) retryContext.LastRequestResult.EndTime);
      else
        this.lastSecondaryAttempt = new DateTimeOffset?((DateTimeOffset) retryContext.LastRequestResult.EndTime);
      bool flag = retryContext.LastRequestResult.TargetLocation == StorageLocation.Secondary && retryContext.LastRequestResult.HttpStatusCode == 404;
      TimeSpan retryInterval;
      if (!this.ShouldRetry(retryContext.CurrentRetryCount, flag ? 500 : retryContext.LastRequestResult.HttpStatusCode, retryContext.LastRequestResult.Exception, out retryInterval, operationContext))
        return (RetryInfo) null;
      RetryInfo retryInfo = new RetryInfo(retryContext);
      if (flag && retryContext.LocationMode != LocationMode.SecondaryOnly)
      {
        retryInfo.UpdatedLocationMode = LocationMode.PrimaryOnly;
        retryInfo.TargetLocation = StorageLocation.Primary;
      }
      DateTimeOffset? nullable = retryInfo.TargetLocation == StorageLocation.Primary ? this.lastPrimaryAttempt : this.lastSecondaryAttempt;
      if (nullable.HasValue)
      {
        TimeSpan timeSpan = CommonUtility.MaxTimeSpan(DateTimeOffset.Now - nullable.Value, TimeSpan.Zero);
        retryInfo.RetryInterval = retryInterval - timeSpan;
      }
      else
        retryInfo.RetryInterval = TimeSpan.Zero;
      return retryInfo;
    }

    public IRetryPolicy CreateInstance() => (IRetryPolicy) new LinearRetry(this.deltaBackoff, this.maximumAttempts);
  }
}
