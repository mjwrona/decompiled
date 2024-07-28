// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning.SemanticVersion
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using NuGet.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning
{
  public class SemanticVersion : 
    IEquatable<SemanticVersion>,
    IComparable<SemanticVersion>,
    IComparable,
    IPackageVersion
  {
    private readonly NuGet.Versioning.SemanticVersion nugetVersion;

    public SemanticVersion(int major, int minor, int patch, string releaseLabel = null, string metadata = null) => this.nugetVersion = new NuGet.Versioning.SemanticVersion(major, minor, patch, releaseLabel, metadata);

    private SemanticVersion(NuGet.Versioning.SemanticVersion nugetVersion) => this.nugetVersion = nugetVersion;

    public int Major => this.nugetVersion.Major;

    public int Minor => this.nugetVersion.Minor;

    public int Patch => this.nugetVersion.Patch;

    public string Prerelease => this.nugetVersion.Release;

    public string Build => this.nugetVersion.Metadata;

    public string DisplayVersion => this.nugetVersion.ToString();

    public string NormalizedVersion => this.nugetVersion.ToNormalizedString();

    public bool IsPrerelease => this.nugetVersion.IsPrerelease;

    public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
    {
      if ((object) version1 == (object) version2)
        return true;
      return (object) version1 != null && (object) version2 != null && version1.nugetVersion == version2.nugetVersion;
    }

    public static bool operator !=(SemanticVersion version1, SemanticVersion version2) => !(version1 == version2);

    public static SemanticVersion Parse(string version)
    {
      NuGet.Versioning.SemanticVersion version1;
      if (!NuGet.Versioning.SemanticVersion.TryParse(version, out version1))
        throw new InvalidSemanticVersionException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidVersion((object) version));
      return new SemanticVersion(version1);
    }

    public static bool TryParse(string version, out SemanticVersion parsedVersion)
    {
      parsedVersion = (SemanticVersion) null;
      NuGet.Versioning.SemanticVersion version1;
      if (!NuGet.Versioning.SemanticVersion.TryParse(version, out version1))
        return false;
      parsedVersion = new SemanticVersion(version1);
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((SemanticVersion) obj);
    }

    public int CompareTo(object other) => this.CompareTo(other as SemanticVersion);

    public int CompareTo(SemanticVersion otherVersion)
    {
      ArgumentUtility.CheckForNull<SemanticVersion>(otherVersion, nameof (otherVersion));
      int num = this.nugetVersion.CompareTo(otherVersion.nugetVersion);
      return num == 0 ? string.CompareOrdinal(this.nugetVersion.Release, otherVersion.Prerelease) : num;
    }

    public override string ToString() => this.nugetVersion.ToString();

    public override int GetHashCode() => this.nugetVersion.GetHashCode();

    public bool Equals(SemanticVersion otherVersion)
    {
      ArgumentUtility.CheckForNull<SemanticVersion>(otherVersion, nameof (otherVersion));
      return this.nugetVersion.Equals(otherVersion.nugetVersion, VersionComparison.Default) && this.nugetVersion.Release.Equals(otherVersion.Prerelease, StringComparison.Ordinal);
    }
  }
}
