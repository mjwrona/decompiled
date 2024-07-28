// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.IngestionTemporaryFileHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.IO;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers
{
  public class IngestionTemporaryFileHelper
  {
    private static string GenerateTemporaryFileName(IProtocol protocol)
    {
      string str = Guid.NewGuid().ToString("N");
      return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "PackageIngestion", protocol.CorrectlyCasedName, str.Substring(0, 2), str.Substring(2, 2), str));
    }

    public static FileInfo CreateTemporaryFileForIngestion(IProtocol protocol)
    {
      string temporaryFileName = IngestionTemporaryFileHelper.GenerateTemporaryFileName(protocol);
      FileInfo fileForIngestion = new FileInfo(temporaryFileName);
      Directory.CreateDirectory(Path.GetDirectoryName(temporaryFileName));
      using (fileForIngestion.Open(FileMode.CreateNew))
        ;
      fileForIngestion.Refresh();
      return fileForIngestion;
    }

    public static DirectoryInfo CreateTemporaryDirectoryForIngestion(IProtocol protocol)
    {
      string temporaryFileName = IngestionTemporaryFileHelper.GenerateTemporaryFileName(protocol);
      DirectoryInfo directoryForIngestion = new DirectoryInfo(temporaryFileName);
      if (directoryForIngestion.Exists)
        throw new IOException("The path for assembling package data during upload already exists: " + temporaryFileName);
      directoryForIngestion.Create();
      directoryForIngestion.Refresh();
      return directoryForIngestion;
    }

    public static FileStream OpenAutoDeletingTemporaryFile(string tempFileName) => new FileStream(tempFileName, FileMode.Truncate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
  }
}
