// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewContentService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewContentService : 
    CodeReviewServiceBase,
    ICodeReviewContentService,
    IVssFrameworkService
  {
    public IEnumerable<ReviewFileContentInfo> UploadFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Stream packageContentStream)
    {
      return (IEnumerable<ReviewFileContentInfo>) this.ExecuteAndTrace<List<ReviewFileContentInfo>>(requestContext, 1384501, 1384502, 1384503, (Func<List<ReviewFileContentInfo>>) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        ArgumentUtility.CheckForNull<Stream>(packageContentStream, nameof (packageContentStream));
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        if (this.ReviewHasCustomStorage(reviewRaw) && this.ShouldUseReviewCustomStorage(ReviewFileType.ChangeEntry))
          throw new CodeReviewContentUploadNotAllowedException(reviewId);
        List<ContentAccessMetadata> fileInfoToSave = new List<ContentAccessMetadata>();
        using (ZipArchive archive = new ZipArchive(packageContentStream, ZipArchiveMode.Read))
        {
          Dictionary<string, PackageFileContentMetadata> fromPackageStream = this.ExtractDataFromPackageStream(requestContext, archive);
          this.EnsureContentHashesAssociatedWithReview(requestContext, projectId, reviewId, fromPackageStream);
          foreach (ZipArchiveEntry entry in archive.Entries)
          {
            string name = entry.Name;
            int fileServiceFileId = fromPackageStream[name].FileServiceFileId;
            if (fileServiceFileId <= 0)
            {
              using (Stream fileStream = entry.Open())
              {
                ContentAccessMetadata? nullable = this.SaveFile(requestContext, projectId, reviewId, fromPackageStream[name].ContentHash, fileServiceFileId, fileStream, fromPackageStream[name].CalculatedLength, 0L, CompressionType.None);
                if (nullable.HasValue)
                {
                  if (nullable.Value.FileId > 0)
                    fileInfoToSave.Add(nullable.Value);
                }
              }
            }
          }
        }
        List<ReviewFileContentInfo> source = this.SaveContentMetadata(requestContext, projectId, reviewId, fileInfoToSave, ReviewFileType.ChangeEntry);
        requestContext.Trace(1384504, TraceLevel.Verbose, this.Area, this.Layer, "Uploaded Files: review id: '{0}', project id: '{1}', Content hashes: '{2}'", (object) reviewId, (object) projectId, (object) string.Join(",", source.Select<ReviewFileContentInfo, string>((Func<ReviewFileContentInfo, string>) (x => x.SHA1Hash))));
        return source;
      }), nameof (UploadFiles));
    }

    internal Dictionary<string, PackageFileContentMetadata> ExtractDataFromPackageStream(
      IVssRequestContext requestContext,
      ZipArchive archive)
    {
      long maxFileSizeInBytes = this.GetMaxFileSizeInBytes(requestContext);
      long zipFileSizeInBytes = (long) this.GetMaxZipFileSizeInBytes(requestContext);
      long fileSize = 0;
      Dictionary<string, PackageFileContentMetadata> dictionary = new Dictionary<string, PackageFileContentMetadata>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ZipArchiveEntry entry in archive.Entries)
      {
        if (entry.CompressedLength > maxFileSizeInBytes)
          throw new CodeReviewExceededMaxAllowedFileSizeException(entry.Length, maxFileSizeInBytes);
        fileSize += entry.CompressedLength;
        if (fileSize > zipFileSizeInBytes)
          throw new CodeReviewExceededMaxAllowedZipPackageSizeException(fileSize, zipFileSizeInBytes);
        string name = entry.Name;
        if (dictionary.ContainsKey(name))
          throw new CodeReviewFileWithSameContentHashInPackageException(name);
        dictionary[name] = new PackageFileContentMetadata()
        {
          CalculatedLength = entry.Length,
          ContentHash = ReviewFileContentExtensions.ToSha1HashBytes(name)
        };
      }
      return dictionary.Count != 0 ? dictionary : throw new CodeReviewNoFilesFoundToUploadInPackageException();
    }

    private void EnsureContentHashesAssociatedWithReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Dictionary<string, PackageFileContentMetadata> contentHashMapping)
    {
      List<byte[]> list = contentHashMapping.Select<KeyValuePair<string, PackageFileContentMetadata>, byte[]>((Func<KeyValuePair<string, PackageFileContentMetadata>, byte[]>) (x => x.Value.ContentHash)).ToList<byte[]>();
      List<ReviewFileContentInfo> metadataInternal = this.GetContentMetadataInternal(requestContext, projectId, reviewId, (IEnumerable<byte[]>) list);
      if (contentHashMapping.Count != metadataInternal.Count)
      {
        foreach (string key in contentHashMapping.Keys)
        {
          string contentHash = key;
          if (!metadataInternal.Any<ReviewFileContentInfo>((Func<ReviewFileContentInfo, bool>) (x => x.SHA1Hash.Equals(contentHash, StringComparison.OrdinalIgnoreCase))))
            throw new CodeReviewContentHashNotAssociatedWithReviewException(contentHash, reviewId);
        }
      }
      foreach (ReviewFileContentInfo reviewFileContentInfo in metadataInternal)
        contentHashMapping[reviewFileContentInfo.SHA1Hash].FileServiceFileId = reviewFileContentInfo.FileServiceFileId;
    }

    private long GetMaxFileSizeInBytes(IVssRequestContext requestContext) => (long) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/BatchUpload/MaxIndividualFileSize", 5242880);

    private int GetMaxZipFileSizeInBytes(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/BatchUpload/MaxZipFileSize", 6291456);
  }
}
