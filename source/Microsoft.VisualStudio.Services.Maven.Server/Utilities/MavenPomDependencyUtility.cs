// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPomDependencyUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenPomDependencyUtility
  {
    private static IEnumerable<MavenPomDependency> EmptyDependencyList = Enumerable.Empty<MavenPomDependency>();

    public static List<PackageDependency> GetAllDependencies(this MavenPomMetadata metadata) => ((IEnumerable<MavenPomDependency>) metadata.Dependencies ?? MavenPomDependencyUtility.EmptyDependencyList).Union<MavenPomDependency>((IEnumerable<MavenPomDependency>) metadata.DependencyManagement?.Dependencies ?? MavenPomDependencyUtility.EmptyDependencyList).Select<MavenPomDependency, PackageDependency>(MavenPomDependencyUtility.\u003C\u003EO.\u003C0\u003E__ToMavenPackageDependency ?? (MavenPomDependencyUtility.\u003C\u003EO.\u003C0\u003E__ToMavenPackageDependency = new Func<MavenPomDependency, PackageDependency>(MavenPomDependencyUtility.ToMavenPackageDependency))).ToList<PackageDependency>();

    private static PackageDependency ToMavenPackageDependency(MavenPomDependency dependency)
    {
      List<string> values = new List<string>();
      if (!string.IsNullOrWhiteSpace(dependency.GroupId))
        values.Add(dependency.GroupId);
      if (!string.IsNullOrWhiteSpace(dependency.ArtifactId))
        values.Add(dependency.ArtifactId);
      string str = dependency.Scope;
      if (string.IsNullOrWhiteSpace(str))
        str = "compile";
      return new PackageDependency()
      {
        PackageName = string.Join(":", (IEnumerable<string>) values),
        VersionRange = dependency.Version ?? string.Empty,
        Group = dependency.Optional ? Microsoft.VisualStudio.Services.Maven.Server.Resources.Info_DepenencyOptional((object) str) : str
      };
    }
  }
}
