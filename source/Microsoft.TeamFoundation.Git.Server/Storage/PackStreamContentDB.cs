// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.PackStreamContentDB
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal sealed class PackStreamContentDB : BaseContentDB<OffsetLength>
  {
    private readonly GitDataFile m_dataFile;
    private readonly GitPackSplitter m_packSplitter;

    public PackStreamContentDB(
      CacheKeys.CrossHostOdbId crossHostOdbId,
      FileBufferedStreamBase packStream,
      IBufferStreamFactory bufferStreamFactory,
      TfsGitContentCacheService rawContentCache,
      IContentDB fallbackContentDB,
      GitPackSplitter splitter)
      : base(crossHostOdbId, bufferStreamFactory, rawContentCache, fallbackContentDB)
    {
      ArgumentUtility.CheckForNull<FileBufferedStreamBase>(packStream, nameof (packStream));
      ArgumentUtility.CheckForNull<GitPackSplitter>(splitter, nameof (splitter));
      this.m_packSplitter = splitter;
      FileStream fileStream = (FileStream) null;
      MemoryMappedFile file = (MemoryMappedFile) null;
      try
      {
        fileStream = new FileStream(packStream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        file = MemoryMappedFile.CreateFromFile(fileStream, (string) null, fileStream.Length, MemoryMappedFileAccess.Read, (MemoryMappedFileSecurity) null, HandleInheritability.None, false);
        this.m_dataFile = new GitDataFile(file, fileStream.Length, packStream.Name);
      }
      catch
      {
        fileStream?.Dispose();
        file?.Dispose();
        throw;
      }
    }

    public override int ObjectCount
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_packSplitter.PendingObjectsCount;
      }
    }

    public override bool TryLookupObject(
      Sha1Id objectId,
      out GitPackObjectType packType,
      out OffsetLength rawKey)
    {
      this.EnsureNotDisposed();
      PendingObject pendingObject;
      if (this.m_packSplitter.TryGetPendingObject(objectId, out pendingObject))
      {
        packType = pendingObject.ObjectType;
        rawKey = new OffsetLength(pendingObject.Offset, pendingObject.Length);
        return true;
      }
      packType = GitPackObjectType.None;
      rawKey = new OffsetLength();
      return false;
    }

    public override Stream GetRawContent(OffsetLength location)
    {
      this.EnsureNotDisposed();
      return (Stream) this.m_dataFile.CreateStream(location.Offset, location.Length);
    }

    public override IEnumerable<ObjectIdAndType> FindObjectsBetween(Sha1Id fromId, Sha1Id toId)
    {
      this.EnsureNotDisposed();
      throw new NotImplementedException();
    }

    public override IEnumerable<OffsetLength> GetAllObjectLocations()
    {
      this.EnsureNotDisposed();
      return this.m_packSplitter.PendingObjectLocations;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.m_dataFile?.Dispose();
      base.Dispose(disposing);
    }
  }
}
