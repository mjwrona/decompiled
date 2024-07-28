// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedObserverFactoryCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedObserverFactoryCore : ChangeFeedObserverFactory
  {
    private readonly Container.ChangeFeedStreamHandler onChanges;
    private readonly Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesWithManualCheckpoint;

    public ChangeFeedObserverFactoryCore(Container.ChangeFeedStreamHandler onChanges) => this.onChanges = onChanges ?? throw new ArgumentNullException(nameof (onChanges));

    public ChangeFeedObserverFactoryCore(
      Container.ChangeFeedStreamHandlerWithManualCheckpoint onChanges)
    {
      this.onChangesWithManualCheckpoint = onChanges ?? throw new ArgumentNullException(nameof (onChanges));
    }

    public override ChangeFeedObserver CreateObserver() => (ChangeFeedObserver) new ChangeFeedObserverBase(new ChangeFeedObserverBase.ChangeFeedObserverBaseHandler(this.ChangesStreamHandlerAsync));

    private Task ChangesStreamHandlerAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken)
    {
      return this.onChanges != null ? this.onChanges((ChangeFeedProcessorContext) context, stream, cancellationToken) : this.onChangesWithManualCheckpoint((ChangeFeedProcessorContext) context, stream, new Func<Task>(context.CheckpointAsync), cancellationToken);
    }
  }
}
