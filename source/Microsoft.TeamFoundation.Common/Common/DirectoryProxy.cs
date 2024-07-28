// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.DirectoryProxy
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Common
{
  public class DirectoryProxy : IDirectoryProxy
  {
    public bool Exists(string path) => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

    public IEnumerable<string> EnumerateFiles(string path) => Directory.EnumerateFiles(path);

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => Directory.EnumerateFiles(path, searchPattern);

    public string[] GetFiles(string path) => Directory.GetFiles(path);

    public void Delete(string path) => Directory.Delete(path);
  }
}
