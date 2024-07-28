// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedObserverFactoryCore`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedObserverFactoryCore<T> : ChangeFeedObserverFactory
  {
    private readonly Container.ChangesHandler<T> legacyOnChanges;
    private readonly Container.ChangeFeedHandler<T> onChanges;
    private readonly Container.ChangeFeedHandlerWithManualCheckpoint<T> onChangesWithManualCheckpoint;
    private readonly CosmosSerializerCore serializerCore;

    public ChangeFeedObserverFactoryCore(
      Container.ChangesHandler<T> onChanges,
      CosmosSerializerCore serializerCore)
      : this(serializerCore)
    {
      this.legacyOnChanges = onChanges ?? throw new ArgumentNullException(nameof (onChanges));
    }

    public ChangeFeedObserverFactoryCore(
      Container.ChangeFeedHandler<T> onChanges,
      CosmosSerializerCore serializerCore)
      : this(serializerCore)
    {
      this.onChanges = onChanges ?? throw new ArgumentNullException(nameof (onChanges));
    }

    public ChangeFeedObserverFactoryCore(
      Container.ChangeFeedHandlerWithManualCheckpoint<T> onChanges,
      CosmosSerializerCore serializerCore)
      : this(serializerCore)
    {
      this.onChangesWithManualCheckpoint = onChanges ?? throw new ArgumentNullException(nameof (onChanges));
    }

    private ChangeFeedObserverFactoryCore(CosmosSerializerCore serializerCore) => this.serializerCore = serializerCore ?? throw new ArgumentNullException(nameof (serializerCore));

    public override ChangeFeedObserver CreateObserver() => (ChangeFeedObserver) new ChangeFeedObserverBase(new ChangeFeedObserverBase.ChangeFeedObserverBaseHandler(this.ChangesStreamHandlerAsync));

    private Task ChangesStreamHandlerAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken)
    {
      IReadOnlyCollection<T> changes = this.AsIReadOnlyCollection(stream, context);
      if (changes.Count == 0)
        return Task.CompletedTask;
      if (this.legacyOnChanges != null)
        return this.legacyOnChanges(changes, cancellationToken);
      return this.onChanges != null ? this.onChanges((ChangeFeedProcessorContext) context, changes, cancellationToken) : this.onChangesWithManualCheckpoint((ChangeFeedProcessorContext) context, changes, new Func<Task>(context.CheckpointAsync), cancellationToken);
    }

    private IReadOnlyCollection<T> AsIReadOnlyCollection(
      Stream stream,
      ChangeFeedObserverContextCore context)
    {
      try
      {
        return CosmosFeedResponseSerializer.FromFeedResponseStream<T>(this.serializerCore, stream);
      }
      catch (Exception ex)
      {
        ChangeFeedProcessorContext context1 = (ChangeFeedProcessorContext) context;
        throw new ChangeFeedProcessorUserException(ex, context1);
      }
    }
  }
}
