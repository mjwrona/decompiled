// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.FileSystem
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class FileSystem : IFileSystem
  {
    public static readonly FileSystem Instance = new FileSystem();

    private FileSystem()
    {
    }

    public void CreateDirectory(string directoryPath) => Directory.CreateDirectory(directoryPath);

    public void DeleteFile(string filePath) => File.Delete(filePath);

    public bool DirectoryExists(string directoryPath) => Directory.Exists(directoryPath);

    public IEnumerable<string> EnumerateDirectories(string directoryFullPath, bool recursiveSearch) => LongPathUtility.EnumerateDirectories(directoryFullPath, recursiveSearch);

    public IEnumerable<string> EnumerateFiles(string directoryFullPath, bool recursiveSearch) => LongPathUtility.EnumerateFiles(directoryFullPath, recursiveSearch);

    public bool FileExists(string filePath) => File.Exists(filePath);

    public long GetFileSize(string filePath) => this.FileExists(filePath) ? new FileInfo(filePath).Length : throw new FileNotFoundException("FileSystem file not found: " + filePath);

    public TempFile GetTempFileFullPath() => new TempFile((IFileSystem) this, Path.GetTempPath());

    public string GetWorkingDirectory() => Directory.GetCurrentDirectory();

    public Stream OpenStreamForFile(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      return (Stream) FileStreamUtils.OpenFileStreamForAsync(fileFullPath, fileMode, fileAccess, fileShare);
    }

    public FileStream OpenFileStreamForAsync(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      return FileStreamUtils.OpenFileStreamForAsync(fileFullPath, fileMode, fileAccess, fileShare);
    }

    public StreamReader OpenText(string filePath) => File.OpenText(filePath);

    public string ReadAllText(string filePath) => File.ReadAllText(filePath);

    public byte[] ReadAllBytes(string filePath) => File.ReadAllBytes(filePath);

    public void WriteAllBytes(string filePath, byte[] bytes) => File.WriteAllBytes(filePath, bytes);

    public void WriteAllText(string filePath, string content) => File.WriteAllText(filePath, content);
  }
}
