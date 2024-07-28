// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MultiCollectionMigrationStateWriter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class MultiCollectionMigrationStateWriter : IMigrationStateWriter, IMigrationStateReader
  {
    private readonly IHostInfoService hostInfoService;
    private readonly IFactory<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>> migrationStateWriterFromRequestContextFactory;
    private readonly IFactory<CollectionId, IVssRequestContext> requestContextFactory;

    public MultiCollectionMigrationStateWriter(
      IHostInfoService hostInfoService,
      IFactory<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>> multiCollectionStateWriterFactory,
      IFactory<CollectionId, IVssRequestContext> requestContextFactory)
    {
      this.hostInfoService = hostInfoService;
      this.migrationStateWriterFromRequestContextFactory = multiCollectionStateWriterFactory;
      this.requestContextFactory = requestContextFactory;
    }

    public async Task<IEnumerable<MigrationEntry>> GetStateEntries(MigrationStateFilter filter = null)
    {
      MultiCollectionMigrationStateWriter migrationStateWriter = this;
      MigrationStateFilter migrationStateFilter = filter;
      IEnumerable<IMigrationStateWriter> source;
      if (migrationStateFilter == null)
      {
        source = (IEnumerable<IMigrationStateWriter>) null;
      }
      else
      {
        IEnumerable<CollectionId> collectionIds = migrationStateFilter.CollectionIds;
        source = collectionIds != null ? collectionIds.Select<CollectionId, IMigrationStateWriter>(new Func<CollectionId, IMigrationStateWriter>(migrationStateWriter.GetWriter)) : (IEnumerable<IMigrationStateWriter>) null;
      }
      if (source == null)
        source = migrationStateWriter.GetActiveCollectionStateWriters();
      return (IEnumerable<MigrationEntry>) ((IEnumerable<IEnumerable<MigrationEntry>>) await Task.WhenAll<IEnumerable<MigrationEntry>>((IEnumerable<Task<IEnumerable<MigrationEntry>>>) source.Where<IMigrationStateWriter>((Func<IMigrationStateWriter, bool>) (w => w != null)).ToList<IMigrationStateWriter>().Select<IMigrationStateWriter, Task<IEnumerable<MigrationEntry>>>((Func<IMigrationStateWriter, Task<IEnumerable<MigrationEntry>>>) (writer => writer.GetStateEntries(filter))).ToList<Task<IEnumerable<MigrationEntry>>>())).SelectMany<IEnumerable<MigrationEntry>, MigrationEntry>((Func<IEnumerable<MigrationEntry>, IEnumerable<MigrationEntry>>) (x => x)).ToList<MigrationEntry>();
    }

    public IConcurrentIterator<MigrationEntry> GetStateEntriesConcurrentIterator(
      MigrationStateFilter filter = null)
    {
      MigrationStateFilter migrationStateFilter = filter;
      IEnumerable<IMigrationStateWriter> source;
      if (migrationStateFilter == null)
      {
        source = (IEnumerable<IMigrationStateWriter>) null;
      }
      else
      {
        IEnumerable<CollectionId> collectionIds = migrationStateFilter.CollectionIds;
        source = collectionIds != null ? collectionIds.Select<CollectionId, IMigrationStateWriter>(new Func<CollectionId, IMigrationStateWriter>(this.GetWriter)) : (IEnumerable<IMigrationStateWriter>) null;
      }
      if (source == null)
        source = this.GetActiveCollectionStateWriters();
      return (IConcurrentIterator<MigrationEntry>) new ConcurrentIterator<IMigrationStateWriter, MigrationEntry>((IEnumerable<IMigrationStateWriter>) source.Where<IMigrationStateWriter>((Func<IMigrationStateWriter, bool>) (w => w != null)).ToList<IMigrationStateWriter>(), ConcurrentIterator.UnboundedCapacity, CancellationToken.None, (Func<IMigrationStateWriter, TryAddValueAsyncFunc<MigrationEntry>, CancellationToken, Task>) (async (writer, tryAddValueAsync, cancellationToken) => await writer.GetStateEntriesConcurrentIterator(filter).DoWhileAsyncNoContext<MigrationEntry>(cancellationToken, (Func<MigrationEntry, Task<bool>>) (async entry => await tryAddValueAsync(entry)))));
    }

    public async Task<IList<MigrationEntry>> Apply(
      IProtocol protocol,
      CollectionId collectionId,
      ApplyTransform applyTransform,
      IList<Guid> feedListForCacheInvalidation = null)
    {
      try
      {
        using (IVssRequestContext collectionRequestContext = this.requestContextFactory.Get(collectionId))
          return await this.migrationStateWriterFromRequestContextFactory.Get(collectionRequestContext).Get(collectionId).Apply(protocol, collectionId, applyTransform, feedListForCacheInvalidation);
      }
      catch (HostDoesNotExistException ex)
      {
        return (IList<MigrationEntry>) new List<MigrationEntry>();
      }
      catch (HostShutdownException ex)
      {
        return (IList<MigrationEntry>) new List<MigrationEntry>();
      }
    }

    private IEnumerable<IMigrationStateWriter> GetActiveCollectionStateWriters() => (IEnumerable<IMigrationStateWriter>) this.hostInfoService.GetActiveCollectionHosts().ToList<HostProperties>().Select<HostProperties, IMigrationStateWriter>((Func<HostProperties, IMigrationStateWriter>) (h => this.GetWriter((CollectionId) h.Id))).Where<IMigrationStateWriter>((Func<IMigrationStateWriter, bool>) (w => w != null)).ToList<IMigrationStateWriter>();

    private IMigrationStateWriter GetWriter(CollectionId collectionId)
    {
      try
      {
        return this.migrationStateWriterFromRequestContextFactory.Get((IVssRequestContext) null).Get(collectionId);
      }
      catch (HostDoesNotExistException ex)
      {
        return (IMigrationStateWriter) null;
      }
      catch (HostShutdownException ex)
      {
        return (IMigrationStateWriter) null;
      }
    }
  }
}
