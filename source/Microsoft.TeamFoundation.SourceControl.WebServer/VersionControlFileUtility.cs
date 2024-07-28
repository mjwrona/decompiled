// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionControlFileUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class VersionControlFileUtility
  {
    public static int TryDetectFileEncoding(Stream fileContentStream, int defaultEncoding) => VersionControlFileUtility.TryDetectFileEncoding(fileContentStream, defaultEncoding, 0L, out bool _);

    public static int TryDetectFileEncoding(
      Stream fileContentStream,
      int defaultEncoding,
      long scanBytes,
      out bool containsByteOrderMark)
    {
      Encoding encoding1;
      try
      {
        encoding1 = Encoding.GetEncoding(defaultEncoding);
      }
      catch (Exception ex)
      {
        encoding1 = Encoding.Default;
      }
      Encoding encoding2 = FileTypeUtil.DetermineEncoding(fileContentStream, true, encoding1, scanBytes, out containsByteOrderMark);
      return encoding2 == null ? -1 : encoding2.CodePage;
    }

    public static string GetFileExtensionFromPath(string path)
    {
      if (!string.IsNullOrEmpty(path))
      {
        int num = path.LastIndexOf('.');
        if (num >= 0)
          return path.Substring(num + 1);
      }
      return string.Empty;
    }

    public static FileContentMetadata GetFileContentMetadata(
      string fileName,
      bool isFolder,
      int encoding = 0)
    {
      FileContentMetadata fileContentMetadata = new FileContentMetadata();
      fileContentMetadata.FileName = fileName;
      if (!isFolder)
      {
        fileContentMetadata.Extension = VersionControlFileUtility.GetFileExtensionFromPath(fileName);
        fileContentMetadata.ContentType = MimeMapper.GetContentType(fileContentMetadata.Extension);
        if (string.IsNullOrEmpty(fileContentMetadata.ContentType))
          fileContentMetadata.ContentType = "application/octet-stream";
        switch (MimeMapper.GetContentViewerType(fileContentMetadata.Extension, fileContentMetadata.ContentType))
        {
          case ContentViewerType.Text:
            fileContentMetadata.Encoding = Math.Max(encoding, 0);
            break;
          case ContentViewerType.Image:
            fileContentMetadata.IsImage = true;
            fileContentMetadata.IsBinary = true;
            fileContentMetadata.Encoding = -1;
            break;
          default:
            fileContentMetadata.Encoding = encoding;
            fileContentMetadata.IsBinary = encoding < 0;
            break;
        }
      }
      return fileContentMetadata;
    }
  }
}
