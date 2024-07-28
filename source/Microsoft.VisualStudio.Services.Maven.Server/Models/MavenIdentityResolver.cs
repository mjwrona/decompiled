// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenIdentityResolver : 
    IdentityResolverBase<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, MavenArtifactFilePath>
  {
    public static MavenIdentityResolver Instance { get; } = new MavenIdentityResolver();

    public override MavenPackageName ResolvePackageName(string name) => new MavenPackageName(name);

    public override MavenPackageVersion ResolvePackageVersion(string version) => new MavenPackageVersion(version);

    public override MavenPackageIdentity FusePackageIdentity(
      MavenPackageName name,
      MavenPackageVersion version)
    {
      return new MavenPackageIdentity(name, version);
    }

    public override PackageUrlIdentity GetPackageUrlIdentity(
      MavenPackageIdentity packageIdentity,
      MavenArtifactFilePath? packageFileName)
    {
      return new PackageUrlIdentity(packageIdentity.Name.Protocol.LowercasedName, packageIdentity.Name.GroupId, packageIdentity.Name.ArtifactId, packageIdentity.Version.DisplayVersion, (IImmutableDictionary<string, string>) ImmutableDictionary<string, string>.Empty);
    }
  }
}
