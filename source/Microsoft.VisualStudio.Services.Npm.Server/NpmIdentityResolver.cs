// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class NpmIdentityResolver : 
    IdentityResolverBase<NpmPackageName, SemanticVersion, NpmPackageIdentity, SimplePackageFileName>
  {
    public static NpmIdentityResolver Instance { get; } = new NpmIdentityResolver();

    public override NpmPackageName ResolvePackageName(string name) => new NpmPackageName(name);

    public override SemanticVersion ResolvePackageVersion(string version) => SemanticVersion.Parse(version);

    public override NpmPackageIdentity FusePackageIdentity(
      NpmPackageName name,
      SemanticVersion version)
    {
      return new NpmPackageIdentity(name, version);
    }

    public override PackageUrlIdentity GetPackageUrlIdentity(
      NpmPackageIdentity packageIdentity,
      SimplePackageFileName? packageFileName)
    {
      return new PackageUrlIdentity(packageIdentity.Name.Protocol.LowercasedName, packageIdentity.Name.IsScoped ? "@" + packageIdentity.Name.Scope : (string) null, packageIdentity.Name.UnscopedName, packageIdentity.Version.DisplayVersion, (IImmutableDictionary<string, string>) ImmutableDictionary<string, string>.Empty);
    }
  }
}
