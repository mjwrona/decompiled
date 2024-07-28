// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationFileService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationFileService))]
  public interface ITeamFoundationFileService : IVssFrameworkService
  {
    ComputePendingDeltasResult ComputePendingDeltas(
      IVssRequestContext requestContext,
      out string resultMessage);

    FileStream CopyStreamToTempFile(
      IVssRequestContext requestContext,
      Stream stream,
      ref CompressionType compressionType,
      bool compressOutput);

    void DeleteFile(IVssRequestContext requestContext, long fileId);

    void DeleteFiles(IVssRequestContext requestContext, IEnumerable<int> fileIds);

    void DeleteFiles(IVssRequestContext requestContext, IEnumerable<long> fileIds);

    void DeleteNamedFiles(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      IEnumerable<string> fileNames);

    (int numberOfFilesDeleted, int numberOfFailures) DoCleanup(IVssRequestContext requestContext);

    FileStatistics GetFileStatistics(IVssRequestContext requestContext, long fileId);

    IDictionary<int, long> QueryFileSizes(
      IVssRequestContext requestContext,
      IEnumerable<long> fileIds,
      Guid dataspaceId,
      OwnerId ownerId);

    IDictionary<long, long> QueryFileSizes64(
      IVssRequestContext requestContext,
      IEnumerable<long> fileIds,
      Guid dataspaceId,
      OwnerId ownerId);

    List<FileStatistics> QueryNamedFiles(IVssRequestContext requestContext, OwnerId ownerId);

    List<FileStatistics> QueryNamedFiles(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string pattern);

    void RenameFile(IVssRequestContext requestContext, long fileId, string newFileName);

    Stream RetrieveFile(
      IVssRequestContext requestContext,
      long fileId,
      out CompressionType compressionType);

    Stream RetrieveFile(
      IVssRequestContext requestContext,
      long fileId,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType);

    Stream RetrieveFile(
      IVssRequestContext requestContext,
      FileIdentifier fileId,
      out CompressionType compressionType);

    Stream RetrieveNamedFile(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType);

    bool TryGetFileId(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      out int fileId);

    bool TryGetFileId64(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      out long fileId);

    int UploadFile(IVssRequestContext requestContext, byte[] content);

    int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier);

    int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      OwnerId ownerId,
      Guid dataspaceIdentifier);

    int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName);

    bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent = false);

    bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent,
      bool useRemoteBlobStoreIfPossible);

    bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName);

    long UploadFile64(IVssRequestContext requestContext, byte[] content);

    long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier);

    long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName);

    long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      OwnerId ownerId,
      Guid dataspaceIdentifier);

    long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName);

    bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent = false);

    bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent,
      bool useRemoteBlobStoreIfPossible);

    bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName);
  }
}
