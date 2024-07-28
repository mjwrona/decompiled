// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.AutoCheckpointer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class AutoCheckpointer : ChangeFeedObserver
  {
    private readonly ChangeFeedObserver observer;

    public AutoCheckpointer(ChangeFeedObserver observer) => this.observer = observer ?? throw new ArgumentNullException(nameof (observer));

    public override Task OpenAsync(string leaseToken) => this.observer.OpenAsync(leaseToken);

    public override Task CloseAsync(string leaseToken, ChangeFeedObserverCloseReason reason) => this.observer.CloseAsync(leaseToken, reason);

    public override async Task ProcessChangesAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken)
    {
      await this.observer.ProcessChangesAsync(context, stream, cancellationToken).ConfigureAwait(false);
      await context.CheckpointAsync();
    }
  }
}
