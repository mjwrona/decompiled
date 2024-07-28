// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.CargoPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity
{
  public sealed class CargoPackageIdentity : 
    PackageIdentityBase<CargoPackageName, CargoPackageVersion>,
    ISingleFileProtocolPackageIdentity,
    IPackageIdentity,
    IEquatable<CargoPackageIdentity>
  {
    public CargoPackageIdentity(CargoPackageName name, CargoPackageVersion version)
      : base(name, version)
    {
    }

    public bool Equals(CargoPackageIdentity? other) => other != null && object.Equals((object) this.Name.NormalizedName, (object) other.Name.NormalizedName) && object.Equals((object) this.Version.NormalizedVersion, (object) other.Version.NormalizedVersion);

    public override bool Equals(object obj) => this == obj || this.Equals(obj as CargoPackageIdentity);

    public override int GetHashCode() => this.Name.NormalizedName.GetHashCode() ^ this.Version.NormalizedVersion.GetHashCode();

    public string GetCanonicalCrateFileName() => this.Name.DisplayName + "-" + this.Version.DisplayVersion + ".crate";

    string ISingleFileProtocolPackageIdentity.GetCanonicalFileName() => this.GetCanonicalCrateFileName();
  }
}
