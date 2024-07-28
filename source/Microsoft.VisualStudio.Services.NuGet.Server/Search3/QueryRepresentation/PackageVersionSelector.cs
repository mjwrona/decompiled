// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.PackageVersionSelector
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class PackageVersionSelector
  {
    public VersionMatchFields MatchFields { get; }

    public VersionMatchType MatchType { get; }

    public string VersionString { get; }

    public static PackageVersionSelector AllVersions { get; } = new PackageVersionSelector((string) null, VersionMatchFields.NormalizedVersion, VersionMatchType.AllVersions);

    public PackageVersionSelector(
      string versionString,
      VersionMatchFields matchFields,
      VersionMatchType matchType)
    {
      this.MatchFields = matchFields;
      this.MatchType = matchType;
      this.VersionString = versionString;
      bool flag = !string.IsNullOrWhiteSpace(versionString);
      if (this.MatchType == VersionMatchType.Exact && !flag)
        throw new ArgumentException("Exact version match requires a version string");
      if (this.MatchType == VersionMatchType.AllVersions & flag)
        throw new ArgumentException("All-versions version match cannot be combined with a version string");
    }

    public override string ToString() => string.Format("<{0} {1} '{2}'>", (object) this.MatchFields, (object) this.MatchType, (object) this.VersionString);
  }
}
