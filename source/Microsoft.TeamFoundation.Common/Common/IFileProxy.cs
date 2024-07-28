// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.IFileProxy
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Common
{
  public interface IFileProxy
  {
    string ReadAllText(string path);

    void WriteAllText(string path, string contents);

    void Copy(string sourceFileName, string destFileName, bool overwrite);

    void Delete(string path);
  }
}
