// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitFileUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.LinkedContent;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitFileUtility
  {
    public static string GetFileNameFromPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return string.Empty;
      int num = path.LastIndexOf('/');
      return num >= 0 ? path.Substring(num + 1) : path;
    }

    public static int TryDetectFileEncoding(
      ITfsGitRepository repo,
      Sha1Id objectId,
      int defaultEncoding)
    {
      return GitFileUtility.TryDetectFileEncoding(repo, objectId, defaultEncoding, 0L, out bool _);
    }

    public static int TryDetectFileEncoding(
      ITfsGitRepository repo,
      Sha1Id objectId,
      int defaultEncoding,
      long scanBytes,
      out bool containsByteOrderMark)
    {
      return GitFileUtility.TryDetectFileEncoding(repo, objectId, defaultEncoding, 0L, (IEnumerable<ITfsLinkedContentResolver>) null, out containsByteOrderMark);
    }

    public static int TryDetectFileEncoding(
      ITfsGitRepository repo,
      Sha1Id objectId,
      int defaultEncoding,
      long scanBytes,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers,
      out bool containsByteOrderMark)
    {
      using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repo, objectId, linkedContentResolvers))
        return VersionControlFileUtility.TryDetectFileEncoding(fileContentStream, defaultEncoding, scanBytes, out containsByteOrderMark);
    }

    public static int TryDetectFileEncodingAndLength(
      ITfsGitRepository repo,
      Sha1Id objectId,
      int defaultEncoding,
      long scanBytes,
      out long length)
    {
      using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repo, objectId))
      {
        length = fileContentStream.Length;
        return VersionControlFileUtility.TryDetectFileEncoding(fileContentStream, defaultEncoding, scanBytes, out bool _);
      }
    }

    public static StringContent GetTextContent(
      ITfsGitRepository repo,
      Sha1Id objectId,
      int encoding)
    {
      repo.LookupObject<TfsGitBlob>(objectId);
      return new StringContent(VersionControlFileReader.ReadFileContent(GitFileUtility.GetFileContentStream(repo, objectId), encoding));
    }

    public static Stream GetFileContentStream(ITfsGitRepository repo, Sha1Id objectId) => GitFileUtility.GetFileContentStream(repo, objectId, (IEnumerable<ITfsLinkedContentResolver>) null);

    public static Stream GetFileContentStream(
      ITfsGitRepository repo,
      Sha1Id objectId,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers)
    {
      Stream content = repo.LookupObject<TfsGitBlob>(objectId).GetContent();
      return GitLinkedContentUtility.Resolve(repo, content, linkedContentResolvers);
    }

    public static Stream GetSubmoduleContentStream(GitItem item) => item != null && item.ContentMetadata?.FileName != null ? (Stream) new MemoryStream(Encoding.UTF8.GetBytes(item.ContentMetadata.FileName + "@" + item.ObjectId)) : (Stream) new MemoryStream(Encoding.UTF8.GetBytes(item.ObjectId));

    internal static VssServerPushStreamContent CreateZipPushStreamContent(
      Action<ZipArchive, ByteArray> populate,
      ISecuredObject securedObject = null)
    {
      return new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((outputStream, httpContent, transportContext) =>
      {
        try
        {
          using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
          {
            using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(outputStream))
            {
              using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
                populate(zipArchive, byteArray);
            }
          }
        }
        catch (HttpException ex)
        {
          throw new RequestCanceledException(FrameworkResources.RequestCanceledErrorWithReason((object) ex.Message), (Exception) ex);
        }
      }), new MediaTypeHeaderValue(MediaTypeFormatUtility.AcceptHeaderToString(RequestMediaType.Zip)), (object) securedObject);
    }

    public static VssServerPushStreamContent GetZipPushStreamContent(
      ITfsGitRepository repo,
      GitItemsCollection gitItems,
      ISecuredObject securedObject,
      bool zipForLinux = false)
    {
      return GitFileUtility.GetZipPushStreamContent(repo, gitItems, (IEnumerable<ITfsLinkedContentResolver>) null, securedObject, zipForLinux);
    }

    public static VssServerPushStreamContent GetZipPushStreamContent(
      ITfsGitRepository repo,
      GitItemsCollection gitItems,
      IEnumerable<ITfsLinkedContentResolver> linkedContentResolvers,
      ISecuredObject securedObject,
      bool zipForUnix = false)
    {
      return GitFileUtility.CreateZipPushStreamContent((Action<ZipArchive, ByteArray>) ((archive, byteArray) =>
      {
        int count = gitItems[0].Path.LastIndexOf('/') + 1;
        foreach (GitItem gitItem in (List<GitItem>) gitItems)
        {
          string entryName = gitItem.Path.Remove(0, count);
          if (gitItem.GitObjectType != GitObjectType.Tree)
          {
            ZipArchiveEntry entry = archive.CreateEntry(entryName, GitStreamUtil.OptimalCompressionLevel);
            if (zipForUnix)
              entry.ExternalAttributes = GitFileUtility.GetLinuxFilePermissions(gitItem, entry.ExternalAttributes);
            using (Stream destinationStream = entry.Open())
            {
              if (gitItem.GitObjectType == GitObjectType.Commit)
              {
                byte[] bytes = Encoding.UTF8.GetBytes(gitItem.ObjectId);
                destinationStream.Write(bytes, 0, bytes.Length);
              }
              else
              {
                using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repo, GitCommitUtility.ParseSha1Id(gitItem.ObjectId), linkedContentResolvers))
                  GitFileUtility.CopyStreamToStreamWithByteArray(fileContentStream, destinationStream, byteArray);
              }
            }
          }
        }
      }), securedObject);
    }

    public static PushStreamContent GetZipPushStreamContent(
      ITfsGitRepository repo,
      IList<TfsGitBlob> tfsGitBlobs)
    {
      return (PushStreamContent) GitFileUtility.CreateZipPushStreamContent((Action<ZipArchive, ByteArray>) ((archive, byteArray) =>
      {
        foreach (TfsGitObject tfsGitBlob in (IEnumerable<TfsGitBlob>) tfsGitBlobs)
        {
          using (Stream destinationStream = archive.CreateEntry(tfsGitBlob.ObjectId.ToString()).Open())
          {
            using (Stream content = tfsGitBlob.GetContent())
              GitFileUtility.CopyStreamToStreamWithByteArray(content, destinationStream, byteArray);
          }
        }
      }));
    }

    public static PushStreamContent GetZipPushStreamContent(ITfsGitRepository repo, GitTreeRef tree) => (PushStreamContent) GitFileUtility.CreateZipPushStreamContent((Action<ZipArchive, ByteArray>) ((archive, byteArray) =>
    {
      foreach (GitTreeEntryRef treeEntry in tree.TreeEntries)
      {
        if (treeEntry.GitObjectType == GitObjectType.Blob)
        {
          using (Stream destinationStream = archive.CreateEntry(treeEntry.RelativePath.TrimStart('/')).Open())
          {
            using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repo, GitCommitUtility.ParseSha1Id(treeEntry.ObjectId)))
              GitFileUtility.CopyStreamToStreamWithByteArray(fileContentStream, destinationStream, byteArray);
          }
        }
        else
        {
          string relativePath = treeEntry.RelativePath;
          if (!relativePath.EndsWith("/", StringComparison.Ordinal))
            relativePath += "/";
          archive.CreateEntry(relativePath.TrimStart('/'));
        }
      }
    }));

    public static VssServerPushStreamContent GetZipPushStreamContent(
      ITfsGitRepository repo,
      Sha1Id objectId,
      ISecuredObject securedObject)
    {
      return GitFileUtility.CreateZipPushStreamContent((Action<ZipArchive, ByteArray>) ((archive, byteArray) => GitFileUtility.CreateZipEntry(repo, archive, objectId, byteArray)), securedObject);
    }

    internal static void CreateZipEntry(
      ITfsGitRepository repo,
      ZipArchive archive,
      Sha1Id objectId,
      ByteArray byteArray)
    {
      using (Stream destinationStream = archive.CreateEntry(objectId.ToString()).Open())
      {
        using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repo, objectId))
          GitFileUtility.CopyStreamToStreamWithByteArray(fileContentStream, destinationStream, byteArray);
      }
    }

    private static void CopyStreamToStreamWithByteArray(
      Stream sourceStream,
      Stream destinationStream,
      ByteArray byteArray)
    {
      int count;
      while ((count = sourceStream.Read(byteArray.Bytes, 0, byteArray.SizeRequested)) != 0)
        destinationStream.Write(byteArray.Bytes, 0, count);
    }

    internal static int GetLinuxFilePermissions(GitItem item, int defaultValue)
    {
      int linuxFilePermissions;
      if (item != null)
      {
        if (!item.IsSymbolicLink)
        {
          if (item.IsLinuxExecutable)
          {
            linuxFilePermissions = -2115174384;
            goto label_6;
          }
        }
        else
        {
          linuxFilePermissions = -1577123824;
          goto label_6;
        }
      }
      linuxFilePermissions = defaultValue;
label_6:
      return linuxFilePermissions;
    }
  }
}
