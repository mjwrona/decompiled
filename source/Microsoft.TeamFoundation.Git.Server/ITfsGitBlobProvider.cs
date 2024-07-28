// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITfsGitBlobProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ITfsGitBlobProvider
  {
    void CreateRepository(IVssRequestContext rc, OdbId odbId);

    void DeleteBlob(IVssRequestContext rc, OdbId odbId, string resourceId);

    void DeleteContainer(IVssRequestContext rc, OdbId odbId, bool throwIfContainerNotExists = true);

    bool DownloadToStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream destination,
      bool throwIfNotFound = true);

    IEnumerable<string> EnumerateBlobs(
      IVssRequestContext rc,
      OdbId odbId,
      TimeSpan? enumerateBlobsClientTimeout = null);

    Stream GetStream(IVssRequestContext rc, OdbId odbId, string resourceId, bool throwIfNotFound = true);

    BlobProperties GetProperties(
      IVssRequestContext rc,
      OdbId odbId,
      string blobName,
      bool throwIfNotFound = true);

    void PutChunk(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long fileLength,
      long offset,
      bool isLastChunk);

    void PutStream(
      IVssRequestContext rc,
      OdbId odbId,
      string resourceId,
      Stream stream,
      long streamLength);

    void RenameBlob(
      IVssRequestContext rc,
      OdbId odbId,
      string sourceResourceId,
      string targetResourceId);
  }
}
