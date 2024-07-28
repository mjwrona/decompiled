// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers.FileContentHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using System.IO;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers
{
  public static class FileContentHelper
  {
    public static FileContent GetFileContent(
      VersionControlProvider vcProvider,
      ItemModel item,
      long? maxContentLength,
      bool? splitContentIntoLines,
      bool? includeBinaryContent)
    {
      FileContent fileContent = new FileContent();
      fileContent.Metadata = item.ContentMetadata;
      if (item.ContentMetadata.Encoding != -1 || includeBinaryContent.GetValueOrDefault(true))
      {
        bool truncated;
        using (MemoryStream fileContentStream = vcProvider.GetFileContentStream(item, maxContentLength.Value, false, out truncated))
        {
          fileContent.ExceededMaxContentLength = truncated;
          if (fileContent.Metadata.Encoding == -1)
            fileContent.ContentBytes = fileContentStream.ToArray();
          else if (splitContentIntoLines.GetValueOrDefault(false))
            fileContent.ContentLines = VersionControlFileReader.ReadFileLines((Stream) fileContentStream, fileContent.Metadata.Encoding);
          else
            fileContent.Content = VersionControlFileReader.ReadFileContent((Stream) fileContentStream, fileContent.Metadata.Encoding);
        }
      }
      return fileContent;
    }
  }
}
