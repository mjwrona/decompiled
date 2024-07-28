// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetV2DependencyUtils
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using NuGet.Frameworks;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  internal static class NuGetV2DependencyUtils
  {
    public static string FlattenDependencies(IReadOnlyList<NuGetDependencyGroup> groups) => groups == null || !groups.Any<NuGetDependencyGroup>() ? (string) null : string.Join("|", groups.Select<NuGetDependencyGroup, string>(NuGetV2DependencyUtils.\u003C\u003EO.\u003C0\u003E__FlattenDependencyGroup ?? (NuGetV2DependencyUtils.\u003C\u003EO.\u003C0\u003E__FlattenDependencyGroup = new Func<NuGetDependencyGroup, string>(NuGetV2DependencyUtils.FlattenDependencyGroup))));

    private static string FlattenDependencyGroup(NuGetDependencyGroup group) => group.Dependencies.Any<NuGetDependency>() ? string.Join("|", group.Dependencies.Select<NuGetDependency, string>((Func<NuGetDependency, string>) (d => NuGetV2DependencyUtils.FlattenDependency(d, group.TargetFramework)))) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "::{0}", (object) NuGetV2DependencyUtils.GetShortFrameworkName(group.TargetFramework));

    private static string FlattenDependency(NuGetDependency dependency, string targetFramework)
    {
      string shortFrameworkName = NuGetV2DependencyUtils.GetShortFrameworkName(targetFramework);
      string str = dependency.Range.ToString("S", (IFormatProvider) new VersionRangeFormatter());
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}:{2}", (object) dependency.Name.DisplayName, (object) str, (object) shortFrameworkName);
    }

    private static string GetShortFrameworkName(string targetFramework)
    {
      if (string.IsNullOrWhiteSpace(targetFramework))
        return string.Empty;
      try
      {
        if (targetFramework.IndexOf(';') != -1)
          return NuGetV2DependencyUtils.GetUnsupportedShortName();
        NuGetFramework nuGetFramework = NuGetFramework.Parse(targetFramework);
        return nuGetFramework.Equals(NuGetFramework.UnsupportedFramework) ? targetFramework : nuGetFramework.GetShortFolderName();
      }
      catch (FrameworkException ex)
      {
        return NuGetV2DependencyUtils.GetUnsupportedShortName();
      }
    }

    private static string GetUnsupportedShortName() => NuGetFramework.UnsupportedFramework.GetShortFolderName();
  }
}
