// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.VssNuGetPackageVersion
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using NuGet.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public sealed class VssNuGetPackageVersion : 
    IPackageVersion,
    IComparable<VssNuGetPackageVersion>,
    IEquatable<VssNuGetPackageVersion>,
    IComparable
  {
    public VssNuGetPackageVersion(NuGetVersion version)
    {
      this.DisplayVersion = version.OriginalVersion;
      this.NuGetVersion = version;
      this.NormalizedOriginalCaseVersion = version.ToNormalizedString();
      this.NormalizedVersion = this.NormalizedOriginalCaseVersion.ToLowerInvariant();
    }

    public VssNuGetPackageVersion(string versionString)
      : this(NuGetVersion.Parse(versionString))
    {
    }

    public NuGetVersion NuGetVersion { get; }

    public string NormalizedOriginalCaseVersion { get; }

    public string DisplayVersion { get; }

    public string NormalizedVersion { get; }

    public static VssNuGetPackageVersion ParseOrDefault(string versionString)
    {
      NuGetVersion version;
      return !NuGetVersion.TryParse(versionString, out version) ? (VssNuGetPackageVersion) null : new VssNuGetPackageVersion(version);
    }

    public int CompareTo(object other) => this.CompareTo(other as VssNuGetPackageVersion);

    public int CompareTo(VssNuGetPackageVersion other) => this.NuGetVersion.CompareTo((SemanticVersion) other.NuGetVersion);

    public bool Equals(VssNuGetPackageVersion other) => PackageVersionComparer.NormalizedVersion.Equals((IPackageVersion) this, (IPackageVersion) other);

    public override bool Equals(object other) => this.Equals(other as VssNuGetPackageVersion);

    public override int GetHashCode() => PackageVersionComparer.NormalizedVersion.GetHashCode((IPackageVersion) this);

    public override string ToString() => this.DisplayVersion;
  }
}
