// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ReviewFilesUtility
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class ReviewFilesUtility
  {
    internal static ChangeEntryStream GetFileContentStream(
      IVssRequestContext requestContext,
      ReviewFileContentInfo fileInfo,
      string fileName)
    {
      CompressionType compressionType;
      return new ChangeEntryStream(requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) fileInfo.FileServiceFileId, out compressionType), compressionType, fileName);
    }

    internal static PushStreamContent GetZipPushStreamContentFromExtension(
      IVssRequestContext requestContext,
      Review review,
      List<ReviewFileContentInfo> fileInfos,
      IReviewContentProvider extension)
    {
      ArtifactId artifact = LinkingUtilities.DecodeUri(review.SourceArtifactId);
      List<ContentInfo> contentInfo = new List<ContentInfo>();
      foreach (ReviewFileContentInfo fileInfo in fileInfos)
      {
        if (fileInfo != null)
          contentInfo.Add(new ContentInfo(fileInfo.SHA1Hash, fileInfo.Flags));
      }
      return extension.GetZipPushStreamContent(requestContext, artifact, (IEnumerable<ContentInfo>) contentInfo);
    }

    internal static PushStreamContent GetZipPushStreamContentFromExtension(
      IVssRequestContext requestContext,
      string sourceArtifactId,
      List<ContentInfo> contentsInfo,
      IReviewContentProvider extension)
    {
      ArtifactId artifact = LinkingUtilities.DecodeUri(sourceArtifactId);
      return extension.GetZipPushStreamContent(requestContext, artifact, (IEnumerable<ContentInfo>) contentsInfo);
    }

    internal static PushStreamContent GetZipPushStreamContent(
      IVssRequestContext requestContext,
      List<ReviewFileContentInfo> fileInfos)
    {
      Dictionary<ContentInfo, int> filesToFetch = new Dictionary<ContentInfo, int>();
      foreach (ReviewFileContentInfo fileInfo in fileInfos)
        ReviewFilesUtility.UpdateFileList(filesToFetch, fileInfo);
      foreach (KeyValuePair<ContentInfo, int> keyValuePair in filesToFetch)
      {
        if (keyValuePair.Value <= 0)
          throw new FileNotFoundException(CodeReviewResources.FileNotFoundException((object) keyValuePair.Key));
      }
      return ReviewFilesUtility.GetZipPushStreamContent(requestContext, filesToFetch);
    }

    internal static PushStreamContent GetZipPushStreamContent(
      IVssRequestContext requestContext,
      Guid projectId,
      Review review,
      List<int> iterationIds,
      ChangeEntryFileType? fileType,
      int top,
      int skip,
      IReviewContentProvider extension,
      out bool needsNextPage)
    {
      Dictionary<ContentInfo, int> filesToDownloadList = ReviewFilesUtility.GetFilesToDownloadList(requestContext, projectId, review.Id, iterationIds, fileType, top, skip, out needsNextPage);
      if (extension != null)
      {
        List<ContentInfo> list = filesToDownloadList.Select<KeyValuePair<ContentInfo, int>, ContentInfo>((Func<KeyValuePair<ContentInfo, int>, ContentInfo>) (fileToFetch => fileToFetch.Key)).ToList<ContentInfo>();
        return ReviewFilesUtility.GetZipPushStreamContentFromExtension(requestContext, review.SourceArtifactId, list, extension);
      }
      Dictionary<ContentInfo, int> filesToFetch = new Dictionary<ContentInfo, int>();
      foreach (KeyValuePair<ContentInfo, int> keyValuePair in filesToDownloadList)
      {
        if (keyValuePair.Value > 0)
          filesToFetch.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return ReviewFilesUtility.GetZipPushStreamContent(requestContext, filesToFetch);
    }

    internal static Dictionary<ContentInfo, int> GetFilesToDownloadList(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      ChangeEntryFileType? fileType,
      int top,
      int skip,
      out bool needsNextPage)
    {
      List<string> stringList = new List<string>();
      Dictionary<ContentInfo, int> filesToDownloadList = new Dictionary<ContentInfo, int>();
      bool flag = false;
      needsNextPage = false;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
      {
        using (VirtualResultCollection<ChangeEntry> changeList = component.GetChangeList(projectId, reviewId, (IEnumerable<int>) iterationIds))
        {
          int num1 = 0;
          int num2 = 0;
          foreach (ChangeEntry entry in changeList.GetCurrent<ChangeEntry>())
          {
            if (entry.Base != null || entry.Modified != null)
            {
              foreach (KeyValuePair<ContentInfo, int> filteredFile in ReviewFilesUtility.GetFilteredFiles(entry, fileType))
              {
                if (!stringList.Contains(filteredFile.Key.Sha1Hash))
                {
                  if (num1 < skip)
                  {
                    ++num1;
                    stringList.Add(filteredFile.Key.Sha1Hash);
                    continue;
                  }
                  if (flag)
                  {
                    needsNextPage = true;
                    return filesToDownloadList;
                  }
                  ++num2;
                  filesToDownloadList.Add(filteredFile.Key, filteredFile.Value);
                  stringList.Add(filteredFile.Key.Sha1Hash);
                }
                if (num2 == top)
                  flag = true;
              }
            }
          }
        }
      }
      return filesToDownloadList;
    }

    internal static List<string> GetFilesToDownloadList(
      List<string> contentHashes,
      int top,
      int skip,
      out bool needsNextPage)
    {
      needsNextPage = false;
      int num = contentHashes.Count<string>();
      if (num > skip + top)
      {
        needsNextPage = true;
        num = skip + top;
      }
      return contentHashes.GetRange(skip, num - skip);
    }

    private static PushStreamContent GetZipPushStreamContent(
      IVssRequestContext requestContext,
      Dictionary<ContentInfo, int> filesToFetch)
    {
      return new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
        using (ByteArray byteArray = new ByteArray(81920))
        {
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              foreach (KeyValuePair<ContentInfo, int> keyValuePair in filesToFetch)
              {
                using (Stream destinationStream = zipArchive.CreateEntry(keyValuePair.Key.Sha1Hash).Open())
                {
                  using (Stream sourceStream = service.RetrieveFile(requestContext, (long) keyValuePair.Value, false, out byte[] _, out long _, out CompressionType _))
                    ReviewFilesUtility.CopyStreamToStreamWithByteArray(sourceStream, destinationStream, byteArray);
                }
              }
            }
          }
        }
      }), new MediaTypeHeaderValue("application/zip"));
    }

    private static Dictionary<ContentInfo, int> GetFilteredFiles(
      ChangeEntry entry,
      ChangeEntryFileType? fileType)
    {
      Dictionary<ContentInfo, int> filesToFetch = new Dictionary<ContentInfo, int>();
      if (fileType.HasValue)
      {
        if (fileType.Value == ChangeEntryFileType.Base)
          ReviewFilesUtility.UpdateFileList(filesToFetch, (ReviewFileContentInfo) entry.Base);
        else if (fileType.Value == ChangeEntryFileType.Modified)
          ReviewFilesUtility.UpdateFileList(filesToFetch, (ReviewFileContentInfo) entry.Modified);
      }
      else
      {
        ReviewFilesUtility.UpdateFileList(filesToFetch, (ReviewFileContentInfo) entry.Base);
        ReviewFilesUtility.UpdateFileList(filesToFetch, (ReviewFileContentInfo) entry.Modified);
      }
      return filesToFetch;
    }

    private static void UpdateFileList(
      Dictionary<ContentInfo, int> filesToFetch,
      ReviewFileContentInfo fileInfo)
    {
      if (fileInfo == null)
        return;
      ContentInfo key = new ContentInfo(fileInfo.SHA1Hash, fileInfo.Flags);
      filesToFetch[key] = fileInfo.FileServiceFileId;
    }

    internal static void CopyStreamToStreamWithByteArray(
      Stream sourceStream,
      Stream destinationStream,
      ByteArray byteArray)
    {
      int count;
      while ((count = sourceStream.Read(byteArray.Bytes, 0, byteArray.SizeRequested)) != 0)
        destinationStream.Write(byteArray.Bytes, 0, count);
    }
  }
}
