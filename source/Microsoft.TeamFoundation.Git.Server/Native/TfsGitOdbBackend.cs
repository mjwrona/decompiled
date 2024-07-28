// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.TfsGitOdbBackend
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  public class TfsGitOdbBackend : OdbBackend, IDisposable
  {
    private readonly ContentDB m_repoStorage;
    private readonly IVssRequestContext m_rc;
    private readonly int m_maxMergeableSize;
    private readonly DirectoryInfo m_tempDirectory;
    private readonly IDictionary<ObjectId, TfsGitOdbBackend.GitObjectData> m_tempObjects;
    private readonly bool m_isTracing_1013202;
    private bool m_disposed;
    private static readonly string s_Layer = typeof (TfsGitOdbBackend).Name;
    private const int c_fallbackMaxMergeableSize = 104857600;

    internal TfsGitOdbBackend(
      IVssRequestContext rc,
      DirectoryInfo tempDirectory,
      ContentDB repoStorage)
    {
      this.m_rc = rc;
      this.m_repoStorage = repoStorage;
      this.m_tempDirectory = tempDirectory;
      this.m_isTracing_1013202 = this.m_rc.IsTracing(1013202, TraceLevel.Verbose, GitServerUtils.TraceArea, TfsGitOdbBackend.s_Layer);
      this.m_maxMergeableSize = rc.GetService<IVssRegistryService>().GetValue<int>(rc, (RegistryQuery) "/Service/Git/Settings/MergeableSize", 104857600);
      this.m_tempObjects = (IDictionary<ObjectId, TfsGitOdbBackend.GitObjectData>) new Dictionary<ObjectId, TfsGitOdbBackend.GitObjectData>();
    }

    public override int Read(
      ObjectId id,
      out UnmanagedMemoryStream data,
      out ObjectType libObjectType)
    {
      try
      {
        this.EnsureNotDisposed();
        TfsGitOdbBackend.GitObjectData gitObjectData;
        if (this.m_tempObjects.TryGetValue(id, out gitObjectData))
        {
          data = this.Allocate(gitObjectData.Stream.Length);
          gitObjectData.Stream.Seek(0L, SeekOrigin.Begin);
          GitStreamUtil.SmartCopyTo(gitObjectData.Stream, (Stream) data);
          gitObjectData.Stream.Seek(0L, SeekOrigin.Begin);
          libObjectType = gitObjectData.ObjectType;
          return 0;
        }
        Sha1Id objectId = new Sha1Id(id.RawId);
        GitPackObjectType packType;
        Stream content;
        if (!this.m_repoStorage.TryLookupObjectAndGetContent(objectId, out packType, out content))
        {
          data = (UnmanagedMemoryStream) null;
          libObjectType = ObjectType.Commit;
          return -3;
        }
        using (content)
        {
          long length = content.Length;
          if (length > (long) this.m_maxMergeableSize)
            throw new GitObjectTooLargeException(objectId, length, (long) this.m_maxMergeableSize);
          if (length > 104857600L)
            this.m_rc.Trace(1013700, TraceLevel.Info, GitServerUtils.TraceArea, TfsGitOdbBackend.s_Layer, "Merging large object. ObjectId: {0}, Size: {1}", (object) objectId, (object) length);
          data = this.Allocate(length);
          GitStreamUtil.SmartCopyTo(content, (Stream) data);
          this.TotalBytesRead += length;
        }
        switch (packType)
        {
          case GitPackObjectType.Commit:
            ++this.NumberOfCommitsRead;
            libObjectType = ObjectType.Commit;
            break;
          case GitPackObjectType.Tree:
            ++this.NumberOfTreesRead;
            libObjectType = ObjectType.Tree;
            break;
          case GitPackObjectType.Blob:
            ++this.NumberOfBlobsRead;
            libObjectType = ObjectType.Blob;
            break;
          case GitPackObjectType.Tag:
            ++this.NumberOfTagsRead;
            libObjectType = ObjectType.Tag;
            break;
          default:
            throw new InvalidOperationException();
        }
        if (this.m_isTracing_1013202)
          this.m_rc.Trace(1013202, TraceLevel.Verbose, GitServerUtils.TraceArea, TfsGitOdbBackend.s_Layer, "Object {0} streamed to native code", (object) objectId);
        return 0;
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1013796, GitServerUtils.TraceArea, nameof (TfsGitOdbBackend), ex);
        throw;
      }
    }

    public override bool Exists(ObjectId id)
    {
      try
      {
        this.EnsureNotDisposed();
        return this.m_tempObjects.ContainsKey(id) || this.m_repoStorage.TryLookupObject(new Sha1Id(id.RawId), out GitPackObjectType _, out TfsGitObjectLocation _);
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1013796, GitServerUtils.TraceArea, nameof (TfsGitOdbBackend), ex);
        throw;
      }
    }

    public override int Write(ObjectId id, Stream dataStream, long length, ObjectType objectType)
    {
      try
      {
        this.EnsureNotDisposed();
        if (!this.Exists(id))
        {
          Stream bufferStream = this.m_repoStorage.DataFileProvider.GetBufferStream(dataStream.Length);
          GitStreamUtil.SmartCopyTo(dataStream, bufferStream);
          bufferStream.Seek(0L, SeekOrigin.Begin);
          this.m_tempObjects.Add(id, new TfsGitOdbBackend.GitObjectData(bufferStream, objectType));
        }
        return 0;
      }
      catch (Exception ex)
      {
        this.m_rc.TraceException(1013796, GitServerUtils.TraceArea, nameof (TfsGitOdbBackend), ex);
        throw;
      }
    }

    public void Complete(ITfsGitRepository targetRepo)
    {
      this.EnsureNotDisposed();
      Stream tempPackStream;
      PackAndRefIngester packAndRefIngester = targetRepo.CreatePackAndRefIngester(out tempPackStream);
      using (tempPackStream)
      {
        using (GitPackSerializer gitPackSerializer = new GitPackSerializer(tempPackStream, this.m_tempObjects.Count, true))
        {
          foreach (TfsGitOdbBackend.GitObjectData gitObjectData in (IEnumerable<TfsGitOdbBackend.GitObjectData>) this.m_tempObjects.Values)
          {
            gitPackSerializer.AddInflatedStreamWithTypeAndSize(gitObjectData.Stream, gitObjectData.ObjectType.ToOurObjectType(), gitObjectData.Stream.Length);
            gitObjectData.Stream.Dispose();
          }
          gitPackSerializer.Complete();
        }
        tempPackStream.Seek(0L, SeekOrigin.Begin);
        this.m_rc.Trace(1013208, TraceLevel.Verbose, GitServerUtils.TraceArea, TfsGitOdbBackend.s_Layer, "Pushing {0} objects for a native merge.", (object) this.m_tempObjects.Count);
        packAndRefIngester.Ingest();
      }
      this.m_tempObjects.Clear();
      this.m_rc.Trace(1013203, TraceLevel.Verbose, GitServerUtils.TraceArea, TfsGitOdbBackend.s_Layer, "Push from native merge completed successfully.");
    }

    public void Dispose()
    {
      if (!this.m_disposed && this.m_tempObjects != null)
      {
        foreach (TfsGitOdbBackend.GitObjectData gitObjectData in (IEnumerable<TfsGitOdbBackend.GitObjectData>) this.m_tempObjects.Values)
          gitObjectData.Stream.Dispose();
      }
      this.m_disposed = true;
    }

    private void EnsureNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (TfsGitOdbBackend));
    }

    public override int ReadPrefix(
      string shortSha,
      out ObjectId oid,
      out UnmanagedMemoryStream data,
      out ObjectType objectType)
    {
      throw new NotImplementedException();
    }

    public override int ReadHeader(ObjectId id, out int length, out ObjectType objectType) => throw new NotImplementedException();

    public override int ReadStream(ObjectId id, out OdbBackendStream stream) => throw new NotImplementedException();

    public override int WriteStream(
      long length,
      ObjectType objectType,
      out OdbBackendStream stream)
    {
      throw new NotImplementedException();
    }

    public override int ExistsPrefix(string shortSha, out ObjectId found) => throw new NotImplementedException();

    public override int ForEach(OdbBackend.ForEachCallback callback) => throw new NotImplementedException();

    public long TotalBytesRead { get; private set; }

    public int NumberOfBlobsRead { get; private set; }

    public int NumberOfTreesRead { get; private set; }

    public int NumberOfCommitsRead { get; private set; }

    public int NumberOfTagsRead { get; private set; }

    protected override OdbBackend.OdbBackendOperations SupportedOperations => OdbBackend.OdbBackendOperations.Read | OdbBackend.OdbBackendOperations.Write | OdbBackend.OdbBackendOperations.Exists;

    private struct GitObjectData
    {
      public Stream Stream;
      public ObjectType ObjectType;

      public GitObjectData(Stream stream, ObjectType objectType)
      {
        this.Stream = stream;
        this.ObjectType = objectType;
      }
    }
  }
}
