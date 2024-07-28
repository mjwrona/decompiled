// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryFileSystemHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class GalleryFileSystemHelper
  {
    public virtual bool Exists(string path) => File.Exists(path);

    public virtual FileStream Create(string path) => File.Create(path);

    public virtual void Delete(string path) => File.Delete(path);

    public virtual void DeleteFiles(List<string> filesToDelete)
    {
      foreach (string path in filesToDelete)
      {
        try
        {
          if (!string.IsNullOrWhiteSpace(path))
          {
            if (this.Exists(path))
              this.Delete(path);
          }
        }
        catch
        {
        }
      }
    }

    public virtual DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

    public virtual string ReadAllText(string path) => File.ReadAllText(path);

    public virtual string ReadFileContent(string filePath)
    {
      string str = string.Empty;
      if (!string.IsNullOrWhiteSpace(filePath) && this.Exists(filePath))
        str = this.ReadAllText(filePath);
      return str;
    }

    public virtual void SerializeAndWriteObjectToFile(
      object objectToSerialize,
      string filePathToWrite)
    {
      this.CreateDirectory(Path.GetDirectoryName(filePathToWrite));
      string str = JsonConvert.SerializeObject(objectToSerialize);
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = this.Create(filePathToWrite);
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
        {
          fileStream = (FileStream) null;
          streamWriter.Write(str);
        }
      }
      finally
      {
        fileStream?.Dispose();
      }
    }
  }
}
