// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.IFileSystem
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public interface IFileSystem
  {
    void CreateDirectory(string directoryPath);

    void DeleteFile(string filePath);

    bool DirectoryExists(string directoryPath);

    IEnumerable<string> EnumerateFiles(string directoryFullPath, bool recursiveSearch);

    IEnumerable<string> EnumerateDirectories(string directoryFullPath, bool recursiveSearch);

    bool FileExists(string filePath);

    long GetFileSize(string filePath);

    TempFile GetTempFileFullPath();

    string GetWorkingDirectory();

    StreamReader OpenText(string filePath);

    Stream OpenStreamForFile(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare);

    FileStream OpenFileStreamForAsync(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare);

    string ReadAllText(string filePath);

    byte[] ReadAllBytes(string filePath);

    void WriteAllBytes(string filePath, byte[] bytes);

    void WriteAllText(string filePath, string content);
  }
}
