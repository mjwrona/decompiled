// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping.BootstrapperCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.FeedManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Bootstrapping
{
  internal sealed class BootstrapperCore : Bootstrapper
  {
    internal static readonly TimeSpan DefaultSleepTime = TimeSpan.FromSeconds(15.0);
    internal static readonly TimeSpan DefaultLockTime = TimeSpan.FromSeconds(30.0);
    private readonly PartitionSynchronizer synchronizer;
    private readonly DocumentServiceLeaseStore leaseStore;
    private readonly TimeSpan lockTime;
    private readonly TimeSpan sleepTime;

    public BootstrapperCore(
      PartitionSynchronizer synchronizer,
      DocumentServiceLeaseStore leaseStore,
      TimeSpan lockTime,
      TimeSpan sleepTime)
    {
      if (synchronizer == null)
        throw new ArgumentNullException(nameof (synchronizer));
      if (leaseStore == null)
        throw new ArgumentNullException(nameof (leaseStore));
      if (lockTime <= TimeSpan.Zero)
        throw new ArgumentException("should be positive", nameof (lockTime));
      if (sleepTime <= TimeSpan.Zero)
        throw new ArgumentException("should be positive", nameof (sleepTime));
      this.synchronizer = synchronizer;
      this.leaseStore = leaseStore;
      this.lockTime = lockTime;
      this.sleepTime = sleepTime;
    }

    public override async Task InitializeAsync()
    {
      object obj;
      int num;
      do
      {
        if (!await this.leaseStore.IsInitializedAsync().ConfigureAwait(false))
        {
          bool isLockAcquired = await this.leaseStore.AcquireInitializationLockAsync(this.lockTime).ConfigureAwait(false);
          obj = (object) null;
          num = 0;
          try
          {
            if (!isLockAcquired)
            {
              DefaultTrace.TraceInformation("Another instance is initializing the store");
              await Task.Delay(this.sleepTime).ConfigureAwait(false);
              num = 1;
            }
            else
            {
              DefaultTrace.TraceInformation("Initializing the store");
              await this.synchronizer.CreateMissingLeasesAsync().ConfigureAwait(false);
              await this.leaseStore.MarkInitializedAsync().ConfigureAwait(false);
            }
          }
          catch (object ex)
          {
            obj = ex;
          }
          if (isLockAcquired)
          {
            int num1 = await this.leaseStore.ReleaseInitializationLockAsync().ConfigureAwait(false) ? 1 : 0;
          }
          object obj1 = obj;
          if (obj1 != null)
          {
            if (!(obj1 is Exception source))
              throw obj1;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
        }
        else
          goto label_19;
      }
      while (num == 1);
      obj = (object) null;
label_19:
      DefaultTrace.TraceInformation("The store is initialized");
    }
  }
}
