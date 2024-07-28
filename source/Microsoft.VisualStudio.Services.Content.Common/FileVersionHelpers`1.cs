// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.FileVersionHelpers`1
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class FileVersionHelpers<T>
  {
    private static readonly Lazy<string> CachedVersionAndComments = new Lazy<string>((Func<string>) (() => FileVersionInfo.GetVersionInfo(typeof (T).Assembly.Location).FileVersion));
    private static readonly Lazy<string> AssemblyVersion = new Lazy<string>((Func<string>) (() =>
    {
      string[] source = FileVersionHelpers<T>.CachedVersionAndComments.Value.Split(' ');
      string str = source[0];
      if (source.Length > 1)
      {
        Match match = Regex.Match(((IEnumerable<string>) source).Last<string>(), "\\(([0-9a-fA-F]+)\\)");
        if (match.Success && match.Groups.Count > 1)
          str = str + "." + match.Groups[1].Value;
      }
      return str;
    }));

    public static string GetAssemblyVersionAndComments() => FileVersionHelpers<T>.CachedVersionAndComments.Value;

    public static string GetAssemblyVersion() => FileVersionHelpers<T>.AssemblyVersion.Value;

    public static string GetThreePartsVersion()
    {
      string[] strArray1 = new string[3];
      string[] strArray2 = FileVersionHelpers<T>.AssemblyVersion.Value.Split('.');
      int length = strArray2.Length;
      for (int index = 0; index < 3; ++index)
        strArray1[index] = length > index ? strArray2[index] : "0";
      return string.Join(".", strArray1);
    }
  }
}
