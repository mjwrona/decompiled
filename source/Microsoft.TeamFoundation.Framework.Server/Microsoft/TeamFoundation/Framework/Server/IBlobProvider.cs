// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IBlobProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [InheritedExport]
  public interface IBlobProvider
  {
    void ServiceStart(IVssRequestContext requestContext);

    void ServiceStart(IVssRequestContext requestContext, IDictionary<string, string> settings);

    void ServiceEnd(IVssRequestContext requestContext);

    Stream GetStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null);

    void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null);

    void DownloadToStreamLargeBlocks(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null);

    void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata = null,
      TimeSpan? clientTimeout = null);

    void PutStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null);

    void PutStream(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null);

    bool DeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null);

    bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null);

    List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null);

    List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null);

    void RenameBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null);

    void RenameBlob(
      IVssRequestContext requestContext,
      string containerName,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null);

    void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null);

    void WriteBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      IDictionary<string, string> tags,
      TimeSpan? clientTimeout = null);

    IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId);

    IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null);

    IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null);

    bool BlobExists(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null);

    bool BlobExists(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      TimeSpan? clientTimeout = null);

    bool ContainerExists(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null);

    BlobProperties ReadBlobProperties(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null);

    void DeleteContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null);

    void SetBlobHeaders(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null);

    void SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null);

    RemoteStoreId RemoteStoreId { get; }

    IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null);
  }
}
