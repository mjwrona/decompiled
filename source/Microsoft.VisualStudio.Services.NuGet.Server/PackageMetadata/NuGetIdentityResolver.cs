// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata.NuGetIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata
{
  public class NuGetIdentityResolver : 
    IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>
  {
    public static NuGetIdentityResolver Instance { get; } = new NuGetIdentityResolver();

    public override VssNuGetPackageName ResolvePackageName(string name) => NuGetPackageRetrievalValidationUtils.ValidateAndParsePackageName(name);

    public override VssNuGetPackageVersion ResolvePackageVersion(string version) => NuGetPackageRetrievalValidationUtils.ValidateAndParsePackageVersion(version);

    public override VssNuGetPackageIdentity FusePackageIdentity(
      VssNuGetPackageName name,
      VssNuGetPackageVersion version)
    {
      return new VssNuGetPackageIdentity(name, version);
    }

    public override PackageUrlIdentity GetPackageUrlIdentity(
      VssNuGetPackageIdentity packageIdentity,
      SimplePackageFileName? packageFileName)
    {
      return new PackageUrlIdentity(packageIdentity.Name.Protocol.LowercasedName, (string) null, packageIdentity.Name.DisplayName, packageIdentity.Version.DisplayVersion, (IImmutableDictionary<string, string>) ImmutableDictionary<string, string>.Empty);
    }
  }
}
