// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPackageExtensionsUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenPackageExtensionsUtility
  {
    private const StringComparison ComparisonType = StringComparison.OrdinalIgnoreCase;

    public static string GetExtension(string fileName, out bool isChecksum)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fileName, nameof (fileName));
      isChecksum = MavenFileNameUtility.IsChecksumFile(fileName);
      char ch = '.';
      string[] source = fileName.ToLowerInvariant().Split(ch);
      if (source.Length == 1)
        return (string) null;
      if (source.Length >= 3)
      {
        string str = "tar";
        int num = !isChecksum || !((IEnumerable<string>) source).Contains<string>(str) || source.Length < 4 ? 2 : 3;
        List<string> list = ((IEnumerable<string>) source).Skip<string>(source.Length - num).ToList<string>();
        if (list.First<string>().Equals(str, StringComparison.OrdinalIgnoreCase) | isChecksum)
          return string.Join(ch.ToString(), (IEnumerable<string>) list);
      }
      if (isChecksum)
        throw new MavenFileExtensionException(Resources.Error_ArtifactFileNameChecksumExtensionMissing((object) fileName));
      return ((IEnumerable<string>) source).Last<string>();
    }
  }
}
