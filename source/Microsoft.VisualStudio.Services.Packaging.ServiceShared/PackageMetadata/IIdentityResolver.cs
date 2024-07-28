// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.IIdentityResolver
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public interface IIdentityResolver
  {
    IPackageName ResolvePackageName(string name);

    IPackageVersion ResolvePackageVersion(string version);

    IPackageIdentity FusePackageIdentity(IPackageName name, IPackageVersion version);

    IPackageIdentity ResolvePackageIdentity(string packageName, string packageVersion);

    PackageUrlIdentity GetPackageUrlIdentity(
      IPackageIdentity packageIdentity,
      IPackageFileName? packageFileName);

    IConverter<string, IPackageName> NameResolver { get; }

    IConverter<string, IPackageVersion> VersionResolver { get; }

    IConverter<(IPackageName Name, IPackageVersion Version), IPackageIdentity> IdentityFuser { get; }

    IConverter<(string Name, string Version), IPackageIdentity> IdentityResolver { get; }
  }
}
