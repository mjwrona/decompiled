// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement.LeaseRenewerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Exceptions;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement
{
  internal sealed class LeaseRenewerCore : LeaseRenewer
  {
    private readonly DocumentServiceLeaseManager leaseManager;
    private readonly TimeSpan leaseRenewInterval;
    private DocumentServiceLease lease;

    public LeaseRenewerCore(
      DocumentServiceLease lease,
      DocumentServiceLeaseManager leaseManager,
      TimeSpan leaseRenewInterval)
    {
      this.lease = lease;
      this.leaseManager = leaseManager;
      this.leaseRenewInterval = leaseRenewInterval;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
      try
      {
        DefaultTrace.TraceInformation("Lease with token {0}: renewer task started.", (object) this.lease.CurrentLeaseToken);
        ConfiguredTaskAwaitable configuredTaskAwaitable = Task.Delay(TimeSpan.FromTicks(this.leaseRenewInterval.Ticks / 2L), cancellationToken).ConfigureAwait(false);
        await configuredTaskAwaitable;
        while (true)
        {
          configuredTaskAwaitable = this.RenewAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = Task.Delay(this.leaseRenewInterval, cancellationToken).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }
      catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
      {
        DefaultTrace.TraceInformation("Lease with token {0}: renewer task stopped.", (object) this.lease.CurrentLeaseToken);
      }
      catch (Exception ex)
      {
        Extensions.TraceException(ex);
        DefaultTrace.TraceCritical("Lease with token {0}: renew lease loop failed", (object) this.lease.CurrentLeaseToken);
        throw;
      }
    }

    private async Task RenewAsync()
    {
      try
      {
        DocumentServiceLease documentServiceLease = await this.leaseManager.RenewAsync(this.lease).ConfigureAwait(false);
        if (documentServiceLease != null)
          this.lease = documentServiceLease;
        DefaultTrace.TraceInformation("Lease with token {0}: renewed lease with result {1}", (object) this.lease.CurrentLeaseToken, (object) (documentServiceLease != null));
      }
      catch (LeaseLostException ex)
      {
        Extensions.TraceException((Exception) ex);
        DefaultTrace.TraceError("Lease with token {0}: lost lease on renew.", (object) this.lease.CurrentLeaseToken);
        throw;
      }
      catch (Exception ex)
      {
        Extensions.TraceException(ex);
        DefaultTrace.TraceError("Lease with token {0}: failed to renew lease.", (object) this.lease.CurrentLeaseToken);
      }
    }
  }
}
