// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiPackageLocalVersionUtils
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public class PyPiPackageLocalVersionUtils
  {
    private static readonly char[] LocalVersionDelimiters = new char[3]
    {
      '-',
      '_',
      '.'
    };

    public static IEnumerable<IVersionLabelSegment> ParseSegments(string localVersion)
    {
      if (string.IsNullOrWhiteSpace(localVersion))
        return Enumerable.Empty<IVersionLabelSegment>();
      localVersion = localVersion.ToLowerInvariant();
      string[] strArray = localVersion.Split(PyPiPackageLocalVersionUtils.LocalVersionDelimiters);
      List<IVersionLabelSegment> segments = new List<IVersionLabelSegment>();
      foreach (string s in strArray)
      {
        ulong result;
        if (ulong.TryParse(s, out result))
          segments.Add((IVersionLabelSegment) new NumericVersionLabelSegment(result));
        else
          segments.Add((IVersionLabelSegment) new StringVersionLabelSegment(s));
      }
      return (IEnumerable<IVersionLabelSegment>) segments;
    }
  }
}
