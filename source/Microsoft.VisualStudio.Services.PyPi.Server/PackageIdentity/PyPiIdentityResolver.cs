// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public class PyPiIdentityResolver : 
    IdentityResolverBase<PyPiPackageName, PyPiPackageVersion, PyPiPackageIdentity, SimplePackageFileName>
  {
    public static PyPiIdentityResolver Instance { get; } = new PyPiIdentityResolver();

    public override PyPiPackageName ResolvePackageName(string name) => new PyPiPackageName(name);

    public override PyPiPackageVersion ResolvePackageVersion(string version) => PyPiPackageVersionParser.Parse(version);

    public override PyPiPackageIdentity FusePackageIdentity(
      PyPiPackageName name,
      PyPiPackageVersion version)
    {
      return new PyPiPackageIdentity(name, version);
    }

    public override PackageUrlIdentity GetPackageUrlIdentity(
      PyPiPackageIdentity packageIdentity,
      SimplePackageFileName? packageFileName)
    {
      return new PackageUrlIdentity(packageIdentity.Name.Protocol.LowercasedName, (string) null, packageIdentity.Name.NormalizedName, packageIdentity.Version.DisplayVersion, (IImmutableDictionary<string, string>) ImmutableDictionary<string, string>.Empty);
    }
  }
}
