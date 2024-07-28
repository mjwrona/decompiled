// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedObserverBase
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedObserverBase : ChangeFeedObserver
  {
    private readonly ChangeFeedObserverBase.ChangeFeedObserverBaseHandler onChanges;

    public ChangeFeedObserverBase(
      ChangeFeedObserverBase.ChangeFeedObserverBaseHandler onChanges)
    {
      this.onChanges = onChanges;
    }

    public override Task OpenAsync(string leaseToken) => Task.CompletedTask;

    public override Task CloseAsync(string leaseToken, ChangeFeedObserverCloseReason reason) => Task.CompletedTask;

    public override Task ProcessChangesAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken)
    {
      return this.onChanges(context, stream, cancellationToken);
    }

    internal delegate Task ChangeFeedObserverBaseHandler(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken);
  }
}
