// Decompiled with JetBrains decompiler
// Type: WebGrease.Common.FileHelper
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.IO;

namespace WebGrease.Common
{
  internal static class FileHelper
  {
    internal static void WriteFile(string path, string content)
    {
      string directoryName = Path.GetDirectoryName(path);
      if (!string.IsNullOrWhiteSpace(directoryName))
        Directory.CreateDirectory(directoryName);
      File.WriteAllText(path, content);
    }
  }
}
