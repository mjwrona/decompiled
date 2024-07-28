// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CargoIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  public class CargoIdentityResolver : 
    IdentityResolverBase<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>
  {
    public static CargoIdentityResolver Instance { get; } = new CargoIdentityResolver();

    public override CargoPackageName ResolvePackageName(string name) => CargoPackageNameParser.Parse(name);

    public override CargoPackageVersion ResolvePackageVersion(string version) => CargoPackageVersionParser.Parse(version);

    public override CargoPackageIdentity FusePackageIdentity(
      CargoPackageName name,
      CargoPackageVersion version)
    {
      return new CargoPackageIdentity(name, version);
    }

    public override PackageUrlIdentity GetPackageUrlIdentity(
      CargoPackageIdentity packageIdentity,
      SimplePackageFileName? packageFileName)
    {
      return new PackageUrlIdentity(Protocol.Cargo.LowercasedName, (string) null, packageIdentity.Name.DisplayName, packageIdentity.Version.DisplayVersion, (IImmutableDictionary<string, string>) ImmutableDictionary<string, string>.Empty);
    }
  }
}
