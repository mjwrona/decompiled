// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultFileSystem
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class DefaultFileSystem : IFileSystem
  {
    public bool DeleteFileIfExists(string filePath)
    {
      try
      {
        File.Delete(filePath);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public string GetTempPath() => Path.GetTempPath();

    public bool CreateFileFromString(string filePath, string content)
    {
      try
      {
        File.WriteAllText(filePath, content);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public bool DeleteFolderIfExists(string folderPath, bool recursive)
    {
      try
      {
        Directory.Delete(folderPath, recursive);
      }
      catch
      {
        return false;
      }
      return true;
    }
  }
}
