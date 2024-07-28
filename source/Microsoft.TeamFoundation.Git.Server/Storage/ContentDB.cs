// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.ContentDB
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal sealed class ContentDB : BaseContentDB<TfsGitObjectLocation>
  {
    private readonly CacheKeys.CrossHostOdbId m_odbId;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly TfsGitContentCacheService m_cache;
    private Lazy<ConcatGitPackIndex> m_index;

    public ContentDB(
      CacheKeys.CrossHostOdbId odbId,
      Lazy<ConcatGitPackIndex> index,
      IGitDataFileProvider dataFilePrv,
      TfsGitContentCacheService rawContentCache)
      : base(odbId, (IBufferStreamFactory) dataFilePrv, rawContentCache, (IContentDB) null)
    {
      this.m_odbId = odbId;
      this.m_index = index;
      this.m_dataFilePrv = dataFilePrv;
      this.m_cache = rawContentCache;
    }

    public override int ObjectCount
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_index.Value.ObjectIds.Count;
      }
    }

    public IGitDataFileProvider DataFileProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_dataFilePrv;
      }
    }

    public ConcatGitPackIndex Index
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_index.Value;
      }
      set
      {
        this.EnsureNotDisposed();
        this.DisposeIndexIfCreated();
        this.m_index = new Lazy<ConcatGitPackIndex>((Func<ConcatGitPackIndex>) (() => value));
        ConcatGitPackIndex concatGitPackIndex = this.m_index.Value;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.DisposeIndexIfCreated();
    }

    private void DisposeIndexIfCreated()
    {
      if (!this.m_index.IsValueCreated)
        return;
      this.m_index.Value.Dispose();
    }

    public override bool TryLookupObject(
      Sha1Id objectId,
      out GitPackObjectType packType,
      out TfsGitObjectLocation location)
    {
      this.EnsureNotDisposed();
      GitPackIndexEntry entry;
      if (this.m_index.Value.TryLookupObject(objectId, out entry))
      {
        packType = entry.ObjectType;
        location = entry.Location;
        return true;
      }
      packType = GitPackObjectType.None;
      location = new TfsGitObjectLocation();
      return false;
    }

    public override Stream GetRawContent(TfsGitObjectLocation location)
    {
      this.EnsureNotDisposed();
      if (!location.IsValid)
        throw new ArgumentOutOfRangeException(nameof (location));
      return this.m_dataFilePrv.GetStream(StorageUtils.GetPackFileName(this.m_index.Value.PackIds[(int) location.PackIntId]), location.Offset, location.Length);
    }

    public override IEnumerable<ObjectIdAndType> FindObjectsBetween(Sha1Id fromId, Sha1Id toId)
    {
      this.EnsureNotDisposed();
      return this.m_index.Value.ObjectIds.FindBetween(fromId, toId).Select<Sha1Id, ObjectIdAndType>((Func<Sha1Id, ObjectIdAndType>) (id => new ObjectIdAndType(id, this.m_index.Value.LookupObject(id).ObjectType)));
    }

    public override IEnumerable<TfsGitObjectLocation> GetAllObjectLocations()
    {
      ContentDB contentDb = this;
      contentDb.EnsureNotDisposed();
      foreach (GitPackIndexEntry entry in (IEnumerable<GitPackIndexEntry>) contentDb.Index.Entries)
        yield return entry.Location;
    }
  }
}
