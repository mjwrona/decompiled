// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.PathUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class PathUtility
  {
    public static string Combine(string path1, string path2)
    {
      if (string.IsNullOrEmpty(path1))
        return path2;
      if (string.IsNullOrEmpty(path2))
        return path1;
      char ch = path1.Contains("/") ? '/' : '\\';
      char[] chArray = new char[2]{ '\\', '/' };
      return path1.TrimEnd(chArray) + ch.ToString() + path2.TrimStart(chArray);
    }
  }
}
