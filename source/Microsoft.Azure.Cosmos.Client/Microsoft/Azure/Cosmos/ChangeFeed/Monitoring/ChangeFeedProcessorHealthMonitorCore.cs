// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Monitoring.ChangeFeedProcessorHealthMonitorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Monitoring
{
  internal sealed class ChangeFeedProcessorHealthMonitorCore : ChangeFeedProcessorHealthMonitor
  {
    private Container.ChangeFeedMonitorErrorDelegate errorDelegate;
    private Container.ChangeFeedMonitorLeaseAcquireDelegate acquireDelegate;
    private Container.ChangeFeedMonitorLeaseReleaseDelegate releaseDelegate;

    public void SetErrorDelegate(
      Container.ChangeFeedMonitorErrorDelegate delegateCallback)
    {
      this.errorDelegate = delegateCallback;
    }

    public void SetLeaseAcquireDelegate(
      Container.ChangeFeedMonitorLeaseAcquireDelegate delegateCallback)
    {
      this.acquireDelegate = delegateCallback;
    }

    public void SetLeaseReleaseDelegate(
      Container.ChangeFeedMonitorLeaseReleaseDelegate delegateCallback)
    {
      this.releaseDelegate = delegateCallback;
    }

    public override async Task NotifyLeaseAcquireAsync(string leaseToken)
    {
      DefaultTrace.TraceInformation("Lease with token {0}: acquired", (object) leaseToken);
      if (this.acquireDelegate == null)
        return;
      try
      {
        await this.acquireDelegate(leaseToken);
      }
      catch (Exception ex)
      {
        Extensions.TraceException(ex);
        DefaultTrace.TraceError("Lease acquire notification failed for " + leaseToken + ". ");
      }
    }

    public override async Task NotifyLeaseReleaseAsync(string leaseToken)
    {
      DefaultTrace.TraceInformation("Lease with token {0}: released", (object) leaseToken);
      if (this.releaseDelegate == null)
        return;
      try
      {
        await this.releaseDelegate(leaseToken);
      }
      catch (Exception ex)
      {
        Extensions.TraceException(ex);
        DefaultTrace.TraceError("Lease release notification failed for " + leaseToken + ". ");
      }
    }

    public override async Task NotifyErrorAsync(string leaseToken, Exception exception)
    {
      Extensions.TraceException(exception);
      DefaultTrace.TraceError("Error detected for lease " + leaseToken + ". ");
      if (this.errorDelegate == null)
        return;
      try
      {
        await this.errorDelegate(leaseToken, exception);
      }
      catch (Exception ex)
      {
        Extensions.TraceException(ex);
        DefaultTrace.TraceError("Error notification failed for " + leaseToken + ". ");
      }
    }
  }
}
