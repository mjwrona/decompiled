// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ThrowingFileSystem
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ThrowingFileSystem : IFileSystem
  {
    private void Throw(string message = null, [CallerMemberName] string callingMethod = null) => throw new InvalidOperationException("Unexpected call to " + callingMethod + ": " + message);

    public void CreateDirectory(string directoryPath) => this.Throw(directoryPath, nameof (CreateDirectory));

    public void DeleteFile(string filePath) => this.Throw(filePath, nameof (DeleteFile));

    public bool DirectoryExists(string directoryPath)
    {
      this.Throw(directoryPath, nameof (DirectoryExists));
      return false;
    }

    public IEnumerable<string> EnumerateDirectories(string directoryFullPath, bool recursiveSearch)
    {
      this.Throw(directoryFullPath, nameof (EnumerateDirectories));
      return (IEnumerable<string>) null;
    }

    public IEnumerable<string> EnumerateFiles(string directoryFullPath, bool recursiveSearch)
    {
      this.Throw(directoryFullPath, nameof (EnumerateFiles));
      return (IEnumerable<string>) null;
    }

    public bool FileExists(string filePath)
    {
      this.Throw(filePath, nameof (FileExists));
      return false;
    }

    public long GetFileSize(string filePath)
    {
      this.Throw(filePath, nameof (GetFileSize));
      return -1;
    }

    public TempFile GetTempFileFullPath()
    {
      this.Throw(callingMethod: nameof (GetTempFileFullPath));
      return (TempFile) null;
    }

    public string GetWorkingDirectory()
    {
      this.Throw(callingMethod: nameof (GetWorkingDirectory));
      return (string) null;
    }

    public FileStream OpenFileStreamForAsync(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      this.Throw(string.Format("{0}, {1}, {2}, {3}", (object) fileFullPath, (object) fileMode, (object) fileAccess, (object) fileShare), nameof (OpenFileStreamForAsync));
      return (FileStream) null;
    }

    public Stream OpenStreamForFile(
      string fileFullPath,
      FileMode fileMode,
      FileAccess fileAccess,
      FileShare fileShare)
    {
      this.Throw(string.Format("{0}, {1}, {2}, {3}", (object) fileFullPath, (object) fileMode, (object) fileAccess, (object) fileShare), nameof (OpenStreamForFile));
      return (Stream) null;
    }

    public StreamReader OpenText(string filePath)
    {
      this.Throw(filePath, nameof (OpenText));
      return (StreamReader) null;
    }

    public byte[] ReadAllBytes(string filePath)
    {
      this.Throw(filePath, nameof (ReadAllBytes));
      return (byte[]) null;
    }

    public string ReadAllText(string filePath)
    {
      this.Throw(filePath, nameof (ReadAllText));
      return (string) null;
    }

    public void WriteAllBytes(string filePath, byte[] bytes) => this.Throw(filePath, nameof (WriteAllBytes));

    public void WriteAllText(string filePath, string content) => this.Throw(filePath, nameof (WriteAllText));
  }
}
