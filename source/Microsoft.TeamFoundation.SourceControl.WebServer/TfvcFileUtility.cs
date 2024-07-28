// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcFileUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcFileUtility
  {
    public static FileContentMetadata GetFileContentMetadata(
      IVssRequestContext context,
      string path,
      bool isFolder,
      int encoding = 0)
    {
      FileContentMetadata fileContentMetadata = new FileContentMetadata();
      fileContentMetadata.FileName = VersionControlPath.IsRootFolder(path) ? string.Empty : VersionControlPath.GetFileName(path);
      if (!string.IsNullOrEmpty(fileContentMetadata.FileName) && !isFolder)
      {
        fileContentMetadata.Extension = VersionControlPath.GetExtension(path);
        fileContentMetadata.ContentType = MimeMapper.GetContentType(fileContentMetadata.Extension);
        if (string.IsNullOrEmpty(fileContentMetadata.ContentType))
          fileContentMetadata.ContentType = "application/octet-stream";
        ContentViewerType contentViewerType = MimeMapper.GetContentViewerType(fileContentMetadata.Extension, fileContentMetadata.ContentType);
        fileContentMetadata.IsImage = contentViewerType == ContentViewerType.Image;
        fileContentMetadata.IsBinary = fileContentMetadata.IsImage || encoding == -1 && contentViewerType == ContentViewerType.None;
        fileContentMetadata.Encoding = encoding;
        string teamProjectName = VersionControlPath.GetTeamProjectName(path);
        if (!string.IsNullOrEmpty(teamProjectName))
          fileContentMetadata.VisualStudioWebLink = TfvcFileUtility.GetVisualStudioWebLink(context, teamProjectName, path);
      }
      return fileContentMetadata;
    }

    private static string GetVisualStudioWebLink(
      IVssRequestContext context,
      string projectName,
      string path)
    {
      ArtifactId artifactId = new ArtifactId("VersionControl", "LaunchLatestVersionedItem", path);
      return VisualStudioLinkingUtility.TryGetProjectArtifactLink(context, projectName, artifactId);
    }

    public static int TryDetectFileEncoding(
      IVssRequestContext requestContext,
      ItemModel file,
      int defaultEncoding)
    {
      return TfvcFileUtility.TryDetectFileEncoding(requestContext, file, defaultEncoding, 0L, out bool _);
    }

    public static int TryDetectFileEncoding(
      IVssRequestContext requestContext,
      ItemModel file,
      int defaultEncoding,
      long scanBytes,
      out bool containsByteOrderMark)
    {
      using (Stream fileContentStream = TfvcFileUtility.GetFileContentStream(requestContext, file))
        return VersionControlFileUtility.TryDetectFileEncoding(fileContentStream, defaultEncoding, scanBytes, out containsByteOrderMark);
    }

    public static Stream GetFileContentStream(IVssRequestContext requestContext, ItemModel file)
    {
      TfvcItem tfvcItem = (TfvcItem) file;
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      VersionControlRequestContext versionControlRequestContext = new VersionControlRequestContext(requestContext, service);
      new SecurityManager(requestContext).CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, tfvcItem.Path);
      if (file.IsFolder)
        throw new TeamFoundationServiceException(Resources.Get("ErrorInvalidFileType"));
      if (tfvcItem.FileId == 1023)
        throw new Microsoft.TeamFoundation.VersionControl.Server.DestroyedContentUnavailableException(Resources.Format("DestroyedFileContentUnavailableException", (object) tfvcItem.ChangesetVersion, (object) file.Path));
      if (tfvcItem.FileId == 0)
        throw new TeamFoundationServiceException(Resources.Format("UnexpectedFileIdForFileException", (object) tfvcItem.Path, (object) tfvcItem.ChangesetVersion), TeamFoundationEventId.ServerItemException);
      return requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) tfvcItem.FileId, false, out byte[] _, out long _, out CompressionType _) ?? throw new TeamFoundationServiceException(Resources.Format("DestroyedFileContentUnavailableException", (object) tfvcItem.ChangesetVersion, (object) file.Path), TeamFoundationEventId.ServerItemException);
    }

    public static MemoryStream GetFileContentStream(
      Stream fileStream,
      long maxLength,
      out bool truncated)
    {
      truncated = false;
      MemoryStream fileContentStream = new MemoryStream();
      int count1 = 65536;
      byte[] buffer = new byte[count1];
      int val1 = 0;
      int num = 0;
      int count2 = 0;
      while ((long) num < maxLength && (val1 = fileStream.Read(buffer, 0, count1)) > 0)
      {
        count2 = (int) Math.Min((long) val1, maxLength - (long) num);
        num += count2;
        if (count2 > 0)
          fileContentStream.Write(buffer, 0, count2);
      }
      if (val1 > count2)
        truncated = true;
      else if (val1 > 0)
        truncated = fileStream.Read(buffer, 0, 1) > 0;
      fileContentStream.Seek(0L, SeekOrigin.Begin);
      return fileContentStream;
    }

    public static void WriteZipFileContents(
      IVssRequestContext requestContext,
      IEnumerable<TfvcItem> itemModelList,
      Stream responseStream,
      string rootFolder)
    {
      ArgumentUtility.CheckForNull<Stream>(responseStream, "stream");
      using (ZipArchive zipArchive = new ZipArchive((Stream) new PositionedStreamWrapper(responseStream), ZipArchiveMode.Create))
      {
        int count = rootFolder.LastIndexOf('/') + 1;
        foreach (TfvcItem itemModel in itemModelList)
        {
          string entryName = itemModel.Path.Remove(0, count);
          if (!itemModel.IsFolder && !itemModel.IsBranch)
          {
            using (Stream destination = zipArchive.CreateEntry(entryName, GitStreamUtil.OptimalCompressionLevel).Open())
            {
              using (Stream fileContentStream = TfvcFileUtility.GetFileContentStream(requestContext, (ItemModel) itemModel))
                fileContentStream.CopyTo(destination);
            }
          }
          else
          {
            if (!entryName.EndsWith("/", StringComparison.Ordinal) && !entryName.EndsWith("\\", StringComparison.Ordinal))
              entryName += "/";
            zipArchive.CreateEntry(entryName);
          }
        }
      }
    }

    public static HttpResponseMessage CreateZipDownloadResponse(
      HttpRequestMessage Request,
      IVssRequestContext TfsRequestContext,
      IEnumerable<TfvcItem> itemModelList,
      string zipFileName,
      string rootItemPath)
    {
      HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContent) =>
      {
        using (SmartPushStreamContentStream responseStream = new SmartPushStreamContentStream(stream))
        {
          TfsRequestContext.UpdateTimeToFirstPage();
          TfvcFileUtility.WriteZipFileContents(TfsRequestContext, itemModelList, (Stream) responseStream, rootItemPath);
        }
      }), new MediaTypeHeaderValue("application/zip"));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(zipFileName);
      return response;
    }
  }
}
