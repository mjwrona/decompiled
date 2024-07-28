// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationStateWriter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class MigrationStateWriter : IMigrationStateWriter, IMigrationStateReader
  {
    public const string MigrationStateContainerNameSegment = "migrationstate";
    private readonly IBlobService blobService;
    private readonly ISerializer<IEnumerable<MigrationEntry>> serializer;
    private readonly CollectionId collectionId;
    private readonly Locator statePath = new Locator(new string[1]
    {
      "state.txt"
    });
    private readonly IInvalidatableCache<string> migrationStateCache;
    private readonly bool isHosted;
    private readonly ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>> migrationStateDocumentCacheService;
    private readonly bool useMigrationStateDocumentCache;

    public MigrationStateWriter(
      IBlobService blobService,
      ISerializer<IEnumerable<MigrationEntry>> serializer,
      CollectionId collectionId,
      IInvalidatableCache<string> migrationStateCache,
      bool isHosted,
      ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>> migrationStateDocumentCacheService,
      bool useMigrationStateDocumentCache)
    {
      this.blobService = blobService;
      this.serializer = serializer;
      this.collectionId = collectionId;
      this.migrationStateCache = migrationStateCache;
      this.isHosted = isHosted;
      this.migrationStateDocumentCacheService = migrationStateDocumentCacheService;
      this.useMigrationStateDocumentCache = useMigrationStateDocumentCache;
    }

    public async Task<IEnumerable<MigrationEntry>> GetStateEntries(MigrationStateFilter filter) => (IEnumerable<MigrationEntry>) (await this.GetAllStateWithEtagCached(filter.Protocol, true)).Value.Where<MigrationEntry>((Func<MigrationEntry, bool>) (e => this.MatchesFilter(e, filter))).ToList<MigrationEntry>();

    public IConcurrentIterator<MigrationEntry> GetStateEntriesConcurrentIterator(
      MigrationStateFilter filter)
    {
      return (IConcurrentIterator<MigrationEntry>) new ConcurrentIterator<MigrationEntry>(ConcurrentIterator.UnboundedCapacity, CancellationToken.None, (Func<TryAddValueAsyncFunc<MigrationEntry>, CancellationToken, Task>) (async (tryAddValue, cancellationToken) =>
      {
        foreach (MigrationEntry stateEntry in await this.GetStateEntries(filter))
        {
          if (!await tryAddValue(stateEntry))
            break;
        }
      }));
    }

    private bool MatchesFilter(MigrationEntry migrationEntry, MigrationStateFilter filter) => migrationEntry.Protocol.Equals(filter.Protocol.ToString()) && (filter.CollectionIds == null || filter.CollectionIds.Any<CollectionId>((Func<CollectionId, bool>) (c => migrationEntry.CollectionId.Equals(c.Guid)))) && (filter.IncludeDeletedFeeds || !migrationEntry.FeedIsDeleted);

    private async Task<EtagValue<IEnumerable<MigrationEntry>>> GetAllStateWithEtagCached(
      IProtocol protocol,
      bool setIfCacheMiss)
    {
      EtagValue<IEnumerable<MigrationEntry>> val;
      if (this.useMigrationStateDocumentCache && this.migrationStateDocumentCacheService.TryGet(protocol, out val))
        return val;
      EtagValue<IEnumerable<MigrationEntry>> withEtagUncached = await this.GetAllStateWithEtagUncached(protocol);
      if (setIfCacheMiss)
        this.migrationStateDocumentCacheService.Set(protocol, withEtagUncached);
      return withEtagUncached;
    }

    private async Task<EtagValue<IEnumerable<MigrationEntry>>> GetAllStateWithEtagUncached(
      IProtocol protocol)
    {
      IEnumerable<MigrationEntry> states = (IEnumerable<MigrationEntry>) null;
      string etag = (string) null;
      using (Stream blobStream = (Stream) new MemoryStream())
      {
        etag = await this.blobService.GetBlobAsync(this.GetStatePathFor(protocol), blobStream);
        if (etag != null)
        {
          states = this.serializer.Deserialize(blobStream);
          states = (IEnumerable<MigrationEntry>) states.Where<MigrationEntry>((Func<MigrationEntry, bool>) (s => s.Protocol == protocol.ToString() || s.Protocol == null)).ToList<MigrationEntry>();
          states.ForEach<MigrationEntry>((Action<MigrationEntry>) (s => this.DecorateStateForReads(protocol, s)));
        }
      }
      if (states == null)
        states = Enumerable.Empty<MigrationEntry>();
      EtagValue<IEnumerable<MigrationEntry>> withEtagUncached = new EtagValue<IEnumerable<MigrationEntry>>(states, etag);
      states = (IEnumerable<MigrationEntry>) null;
      return withEtagUncached;
    }

    private Locator GetStatePathFor(IProtocol protocol)
    {
      Locator statePathFor = this.statePath;
      if (protocol.LowercasedName != "nuget")
        statePathFor = new Locator(new string[2]
        {
          protocol.LowercasedName,
          this.statePath.Value
        });
      return statePathFor;
    }

    private void DecorateStateForReads(IProtocol protocol, MigrationEntry e)
    {
      e.Protocol = protocol.ToString();
      if (e.CollectionId == Guid.Empty)
        e.CollectionId = this.collectionId.Guid;
      if (!e.VNextMigration.IsNullOrEmpty<char>())
        return;
      e.VNextMigration = e.CurrentMigration;
    }

    private void DecorateStateForWrites(MigrationEntry e)
    {
      e.Protocol = (string) null;
      e.CollectionId = Guid.Empty;
      if (!e.CurrentMigration.Equals(e.VNextMigration, StringComparison.OrdinalIgnoreCase))
        return;
      e.VNextMigration = (string) null;
    }

    private string ToKey(Guid collectionId, Guid feedId, string protocol) => string.Format("{0:N}{1:N}{2}", (object) collectionId, (object) feedId, (object) protocol);

    public async Task<IList<MigrationEntry>> Apply(
      IProtocol protocol,
      CollectionId collectionId,
      ApplyTransform applyTransform,
      IList<Guid> feedsListForCacheInvalidation = null)
    {
      MigrationStateWriter migrationStateWriter = this;
      if ((GuidBasedId) collectionId != (GuidBasedId) migrationStateWriter.collectionId && migrationStateWriter.isHosted)
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CollectionIDMismatch((object) collectionId, (object) migrationStateWriter.collectionId));
      Locator pathLocator = migrationStateWriter.GetStatePathFor(protocol);
      string str = (string) null;
      Random r = new Random();
      IList<MigrationEntry> states = (IList<MigrationEntry>) null;
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        EtagValue<IEnumerable<MigrationEntry>> etagValue1;
        if (iterations == 0)
          etagValue1 = await migrationStateWriter.GetAllStateWithEtagCached(protocol, false);
        else
          etagValue1 = await migrationStateWriter.GetAllStateWithEtagUncached(protocol);
        EtagValue<IEnumerable<MigrationEntry>> etagValue2 = etagValue1;
        states = (IList<MigrationEntry>) etagValue2.Value.ToList<MigrationEntry>();
        string etag = etagValue2.Etag;
        if (applyTransform(ref states) == TransformResult.NoOp)
          return states;
        states.ForEach<MigrationEntry>(new Action<MigrationEntry>(migrationStateWriter.DecorateStateForWrites));
        str = await migrationStateWriter.blobService.PutBlobAsync(pathLocator, migrationStateWriter.serializer.Serialize((IEnumerable<MigrationEntry>) states).AsArraySegment(), etag);
      }
      if (str == null)
        throw new ChangeConflictException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FailedToApplyMigrationStateChange());
      migrationStateWriter.migrationStateDocumentCacheService.Invalidate(protocol);
      if (feedsListForCacheInvalidation != null && migrationStateWriter.isHosted)
      {
        foreach (Guid feedId in (IEnumerable<Guid>) feedsListForCacheInvalidation)
          migrationStateWriter.migrationStateCache.Invalidate(migrationStateWriter.ToKey(migrationStateWriter.collectionId.Guid, feedId, protocol.ToString()));
      }
      states.ForEach<MigrationEntry>((Action<MigrationEntry>) (e => this.DecorateStateForReads(protocol, e)));
      return states;
    }
  }
}
