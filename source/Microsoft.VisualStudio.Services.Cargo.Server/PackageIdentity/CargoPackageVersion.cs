// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.CargoPackageVersion
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity
{
  public sealed class CargoPackageVersion : 
    IPackageVersion,
    IEquatable<CargoPackageVersion>,
    IComparable<CargoPackageVersion>,
    IComparable
  {
    internal CargoPackageVersion(
      string displayVersion,
      string normalizedVersion,
      CargoMajorMinorPatchLabel majorMinorPatchLabel,
      CargoPrereleaseLabel? prereleaseLabel,
      CargoBuildMetadataLabel? buildMetadataLabel)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayVersion, nameof (displayVersion));
      this.MajorMinorPatchLabel = majorMinorPatchLabel;
      this.PrereleaseLabel = prereleaseLabel;
      this.BuildMetadataLabel = buildMetadataLabel;
      this.DisplayVersion = displayVersion;
      this.NormalizedVersion = normalizedVersion;
    }

    public CargoMajorMinorPatchLabel MajorMinorPatchLabel { get; }

    public CargoPrereleaseLabel? PrereleaseLabel { get; }

    public CargoBuildMetadataLabel? BuildMetadataLabel { get; }

    public string DisplayVersion { get; }

    public string NormalizedVersion { get; }

    int IComparable.CompareTo(object other) => this.CompareTo(other as CargoPackageVersion);

    public int CompareTo(CargoPackageVersion? other)
    {
      if ((object) this == (object) other)
        return 0;
      if ((object) other == null)
        return 1;
      int num1 = this.MajorMinorPatchLabel.CompareTo(other.MajorMinorPatchLabel);
      if (num1 != 0)
        return num1;
      int num2 = CompareLabels<CargoPrereleaseLabel>(this.PrereleaseLabel, other.PrereleaseLabel, false);
      return num2 != 0 ? num2 : CompareLabels<CargoBuildMetadataLabel>(this.BuildMetadataLabel, other.BuildMetadataLabel, true);

      static int CompareLabels<T>(T? leftLabel, T? rightLabel, bool presentIsGreaterThanNotPresent) where T : CargoVersionLabel<T> => (object) leftLabel != null ? ((object) rightLabel == null ? (presentIsGreaterThanNotPresent ? 1 : -1) : leftLabel.CompareTo(rightLabel)) : ((object) rightLabel == null ? 0 : (presentIsGreaterThanNotPresent ? -1 : 1));
    }

    public bool Equals(CargoPackageVersion? other) => (object) other != null && PackageVersionComparer.NormalizedVersion.Equals((IPackageVersion) this, (IPackageVersion) other);

    public override bool Equals(object? obj)
    {
      if ((object) this == obj)
        return true;
      CargoPackageVersion other = obj as CargoPackageVersion;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => PackageVersionComparer.NormalizedVersion.GetHashCode((IPackageVersion) this);

    public static bool operator ==(CargoPackageVersion? left, CargoPackageVersion? right) => object.Equals((object) left, (object) right);

    public static bool operator !=(CargoPackageVersion? left, CargoPackageVersion? right) => !object.Equals((object) left, (object) right);

    public override string ToString() => this.DisplayVersion;
  }
}
