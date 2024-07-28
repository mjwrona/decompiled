// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CachedGitObjectSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class CachedGitObjectSet : ICachedGitObjectSet, IGitObjectSet, IDisposable
  {
    private readonly CacheKeys.CrossHostOdbId m_id;
    private readonly GitObjectCoreCacheService m_objectCoreCacheSvc;
    private IContentDB m_contentDB;
    private bool m_disposed;

    public CachedGitObjectSet(
      CacheKeys.CrossHostOdbId id,
      IContentDB contentDB,
      GitObjectCoreCacheService objectCoreCacheSvc)
    {
      this.m_id = id;
      this.m_contentDB = contentDB;
      this.m_objectCoreCacheSvc = objectCoreCacheSvc;
    }

    internal IContentDB ContentDB => this.m_contentDB;

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.m_contentDB.Dispose();
      this.m_contentDB = (IContentDB) null;
      this.m_disposed = true;
    }

    public bool TryGetContent(Sha1Id objectId, out Stream content, out GitObjectType objectType)
    {
      this.EnsureNotDisposed();
      GitPackObjectType packType;
      if (this.m_contentDB.TryLookupObjectAndGetContent(objectId, out packType, out content))
      {
        objectType = packType.GetObjectType();
        return true;
      }
      objectType = GitObjectType.Bad;
      return false;
    }

    public GitObjectType TryLookupObjectType(Sha1Id objectId)
    {
      this.EnsureNotDisposed();
      GitPackObjectType packType;
      return this.m_contentDB.TryLookupObjectType(objectId, out packType) ? packType.GetObjectType() : GitObjectType.Bad;
    }

    public IEnumerable<TGitObject> FindObjectsBetween<TGitObject>(Sha1Id fromId, Sha1Id toId) where TGitObject : TfsGitObject => this.m_contentDB.FindObjectsBetween(fromId, toId).Select<ObjectIdAndType, TGitObject>((Func<ObjectIdAndType, TGitObject>) (o => this.TryLookupObject(o.ObjectId) as TGitObject)).Where<TGitObject>((Func<TGitObject, bool>) (o => (object) o != null));

    void ICachedGitObjectSet.TryCacheObjectCore(Sha1Id objectId, IGitObjectCore value)
    {
      this.EnsureNotDisposed();
      this.m_objectCoreCacheSvc.TryCache(new CacheKeys.CrossHostOdbScopedObjectId(this.m_id, objectId), value);
    }

    bool ICachedGitObjectSet.TryGetObjectCoreFromCache(Sha1Id objectId, out IGitObjectCore value)
    {
      this.EnsureNotDisposed();
      return this.m_objectCoreCacheSvc.MemoryCache.TryGetValue(new CacheKeys.CrossHostOdbScopedObjectId(this.m_id, objectId), out value);
    }

    private void EnsureNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("GitObjectSet");
    }
  }
}
