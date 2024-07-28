// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BaseContentDB`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal abstract class BaseContentDB<TRawKey> : ITfsGitContentDB<TRawKey>, IContentDB, IDisposable
    where TRawKey : struct
  {
    protected IContentDB FallbackContentDB;
    private readonly CacheKeys.CrossHostOdbId m_odbId;
    private readonly IBufferStreamFactory m_bufferStreamFactory;
    private readonly TfsGitContentCacheService m_cache;
    private bool m_disposed;

    public BaseContentDB(
      CacheKeys.CrossHostOdbId odbId,
      IBufferStreamFactory bufferStreamFactory,
      TfsGitContentCacheService rawContentCache,
      IContentDB fallbackContentDB)
    {
      this.m_odbId = odbId;
      this.m_bufferStreamFactory = bufferStreamFactory;
      this.m_cache = rawContentCache;
      this.FallbackContentDB = fallbackContentDB;
    }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.Dispose(true);
      this.m_disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected void EnsureNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("ContentDB");
    }

    public bool TryLookupObjectType(Sha1Id objectId, out GitPackObjectType packType) => this.TryLookupObject(objectId, out packType, out TRawKey _);

    public bool TryLookupObjectAndGetContent(
      Sha1Id objectId,
      out GitPackObjectType packType,
      out Stream content)
    {
      this.EnsureNotDisposed();
      TRawKey rawKey;
      if (!this.TryLookupObject(objectId, out packType, out rawKey))
      {
        content = (Stream) null;
        return false;
      }
      StreamAndType content1;
      this.GetFullContent(objectId, rawKey, out content1);
      packType = content1.PackType;
      content = content1.Stream;
      return true;
    }

    public Stream GetContent(TRawKey rawKey, Sha1Id objectId)
    {
      this.EnsureNotDisposed();
      StreamAndType content;
      this.GetFullContent(objectId, rawKey, out content);
      return content.Stream;
    }

    private bool GetFullContent(Sha1Id objectId, TRawKey rawKey, out StreamAndType content)
    {
      Stream stream = (Stream) null;
      Stack<BaseContentDB<TRawKey>.StreamAndObjectId> streamAndObjectIdStack = (Stack<BaseContentDB<TRawKey>.StreamAndObjectId>) null;
      try
      {
        Sha1Id objectId1 = objectId;
        TRawKey? rawKey1 = new TRawKey?(rawKey);
        StreamAndType contentOrDelta;
        while (true)
        {
          if (!this.TryGetFullOrDeltaContent(objectId1, rawKey1, out contentOrDelta))
          {
            if (this.FallbackContentDB != null)
            {
              GitPackObjectType packType;
              contentOrDelta = new StreamAndType(this.FallbackContentDB.LookupObjectAndGetContent(objectId1, out packType), packType, new Sha1Id?());
            }
            else if (objectId1 != objectId)
              break;
          }
          if (contentOrDelta.BaseObjectId.HasValue)
          {
            streamAndObjectIdStack = streamAndObjectIdStack ?? new Stack<BaseContentDB<TRawKey>.StreamAndObjectId>();
            streamAndObjectIdStack.Push(new BaseContentDB<TRawKey>.StreamAndObjectId(contentOrDelta.Stream, objectId1));
            objectId1 = contentOrDelta.BaseObjectId.Value;
            rawKey1 = new TRawKey?();
          }
          else
            goto label_8;
        }
        throw new GitObjectDoesNotExistException(objectId);
label_8:
        stream = contentOrDelta.Stream;
        GitPackObjectType packType1 = contentOrDelta.PackType;
        while (streamAndObjectIdStack != null && streamAndObjectIdStack.Count > 0)
        {
          GitStreamUtil.MakeStreamSeekable(this.m_bufferStreamFactory, ref stream);
          BaseContentDB<TRawKey>.StreamAndObjectId streamAndObjectId = streamAndObjectIdStack.Peek();
          stream = (Stream) new DeltaStream(streamAndObjectId.Stream, stream);
          streamAndObjectIdStack.Pop();
          this.m_cache.TryMakeStreamCached(new CacheKeys.CrossHostOdbScopedObjectId(this.m_odbId, streamAndObjectId.ObjectId), packType1, ref stream);
        }
        content = new StreamAndType(stream, packType1, new Sha1Id?());
        stream = (Stream) null;
        return true;
      }
      finally
      {
        stream?.Dispose();
        while (streamAndObjectIdStack != null && streamAndObjectIdStack.Count > 0)
          streamAndObjectIdStack.Pop().Stream.Dispose();
      }
    }

    private bool TryGetFullOrDeltaContent(
      Sha1Id objectId,
      TRawKey? rawKey,
      out StreamAndType contentOrDelta)
    {
      StreamAndType content;
      if (this.m_cache.TryGetContent(new CacheKeys.CrossHostOdbScopedObjectId(this.m_odbId, objectId), out content))
      {
        contentOrDelta = content;
        return true;
      }
      if (!rawKey.HasValue)
      {
        TRawKey rawKey1;
        if (!this.TryLookupObject(objectId, out GitPackObjectType _, out rawKey1))
        {
          contentOrDelta = new StreamAndType();
          return false;
        }
        rawKey = new TRawKey?(rawKey1);
      }
      Stream stream1 = (Stream) null;
      Stream stream2 = (Stream) null;
      try
      {
        stream1 = this.GetRawContent(rawKey.Value);
        GitPackObjectType type;
        long uncompressedSize;
        GitServerUtils.ReadPackEntryHeader(stream1, out type, out uncompressedSize);
        if (type == GitPackObjectType.OfsDelta)
          throw new InvalidGitPackEntryHeaderException("OfsDelta");
        Sha1Id? baseObjectId;
        if (type == GitPackObjectType.RefDelta)
        {
          try
          {
            baseObjectId = new Sha1Id?(Sha1Id.FromStream(stream1));
          }
          catch (Sha1IdStreamReadException ex)
          {
            throw new InvalidGitPackEntryHeaderException((Exception) ex);
          }
        }
        else
          baseObjectId = new Sha1Id?();
        stream2 = GitServerUtils.CreateInflateStream(stream1, uncompressedLength: uncompressedSize);
        stream1 = (Stream) null;
        this.m_cache.TryMakeStreamCached(new CacheKeys.CrossHostOdbScopedObjectId(this.m_odbId, objectId), type, ref stream2);
        contentOrDelta = new StreamAndType(stream2, type, baseObjectId);
        stream2 = (Stream) null;
        return true;
      }
      finally
      {
        stream2?.Dispose();
        stream1?.Dispose();
      }
    }

    public abstract bool TryLookupObject(
      Sha1Id objectId,
      out GitPackObjectType packType,
      out TRawKey rawKey);

    public abstract Stream GetRawContent(TRawKey rawKey);

    public abstract IEnumerable<TRawKey> GetAllObjectLocations();

    public abstract int ObjectCount { get; }

    public abstract IEnumerable<ObjectIdAndType> FindObjectsBetween(Sha1Id fromId, Sha1Id toId);

    private struct StreamAndObjectId
    {
      public readonly Stream Stream;
      public readonly Sha1Id ObjectId;

      public StreamAndObjectId(Stream stream, Sha1Id objectId)
      {
        this.Stream = stream;
        this.ObjectId = objectId;
      }
    }
  }
}
