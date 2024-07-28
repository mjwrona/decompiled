// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Configuration.ChangeFeedLeaseOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Configuration
{
  internal class ChangeFeedLeaseOptions
  {
    internal static readonly TimeSpan DefaultRenewInterval = TimeSpan.FromSeconds(17.0);
    internal static readonly TimeSpan DefaultAcquireInterval = TimeSpan.FromSeconds(13.0);
    internal static readonly TimeSpan DefaultExpirationInterval = TimeSpan.FromSeconds(60.0);

    public ChangeFeedLeaseOptions()
    {
      this.LeaseRenewInterval = ChangeFeedLeaseOptions.DefaultRenewInterval;
      this.LeaseAcquireInterval = ChangeFeedLeaseOptions.DefaultAcquireInterval;
      this.LeaseExpirationInterval = ChangeFeedLeaseOptions.DefaultExpirationInterval;
    }

    public TimeSpan LeaseRenewInterval { get; set; }

    public TimeSpan LeaseAcquireInterval { get; set; }

    public TimeSpan LeaseExpirationInterval { get; set; }

    public string LeasePrefix { get; set; }
  }
}
