// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ListOfFiles
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class ListOfFiles
  {
    public static Task<List<FileInfo>> LoadFileListAsync(
      string inputFilename,
      string requiredCommonRootDirectory = null)
    {
      FileInfo fileListFile = new FileInfo(inputFilename);
      if (!fileListFile.Exists)
        throw new FileNotFoundException("File to be read (" + inputFilename + ") does not exist", inputFilename);
      DirectoryInfo requiredCommonRootDirectory1 = (DirectoryInfo) null;
      if (requiredCommonRootDirectory != null)
      {
        requiredCommonRootDirectory1 = new DirectoryInfo(LongPathUtility.GetFullNormalizedPath(requiredCommonRootDirectory));
        if (!requiredCommonRootDirectory1.Exists)
          throw new DirectoryNotFoundException("Root directory (" + requiredCommonRootDirectory + ") does not exist.");
      }
      return ListOfFiles.EnumerateFilesAsync(fileListFile, requiredCommonRootDirectory1);
    }

    private static async Task<List<FileInfo>> EnumerateFilesAsync(
      FileInfo fileListFile,
      DirectoryInfo requiredCommonRootDirectory = null)
    {
      List<FileInfo> filesRead = new List<FileInfo>();
      StreamReader reader = new StreamReader(fileListFile.FullName);
      try
      {
        FileInfo fileInfo;
        while (true)
        {
          string line;
          do
          {
            if ((line = (await reader.ReadLineAsync().ConfigureAwait(false))?.Trim()) == null)
              goto label_11;
          }
          while (ListOfFiles.ShouldSkipLine(line));
          fileInfo = ListOfFiles.GetFileInfo(line);
          if (requiredCommonRootDirectory == null || fileInfo.FullName.StartsWith(requiredCommonRootDirectory.FullName, StringComparison.OrdinalIgnoreCase))
            filesRead.Add(fileInfo);
          else
            break;
        }
        throw new ArgumentException("Only paths begining with directory '" + requiredCommonRootDirectory.FullName + "' are accepted in filelist, File: '" + fileInfo.FullName + "'");
      }
      finally
      {
        reader?.Dispose();
      }
label_11:
      reader = (StreamReader) null;
      List<FileInfo> fileInfoList = filesRead;
      filesRead = (List<FileInfo>) null;
      return fileInfoList;
    }

    private static FileInfo GetFileInfo(string line)
    {
      bool flag = false;
      if (!LongPathUtility.IsAbsolutePath(line))
        flag = true;
      if (flag)
        throw new ArgumentException("Only absolute paths are accepted in filelist. File: '" + line + "'");
      FileInfo fileInfo = new FileInfo(line);
      return fileInfo.Exists ? fileInfo : throw new FileNotFoundException("File " + line + " does not exist.", line);
    }

    private static bool ShouldSkipLine(string line) => line == string.Empty || line.StartsWith("#");
  }
}
