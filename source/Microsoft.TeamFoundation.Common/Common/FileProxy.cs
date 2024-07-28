// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.FileProxy
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.IO;

namespace Microsoft.TeamFoundation.Common
{
  public class FileProxy : IFileProxy
  {
    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

    public void Copy(string sourceFileName, string destFileName, bool overwrite) => File.Copy(sourceFileName, destFileName, overwrite);

    public void Delete(string path) => File.Delete(path);
  }
}
