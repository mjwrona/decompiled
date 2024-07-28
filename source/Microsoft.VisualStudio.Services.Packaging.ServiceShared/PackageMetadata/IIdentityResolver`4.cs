// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.IIdentityResolver`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public interface IIdentityResolver<TPackageName, TPackageVersion, TPackageIdentity, in TPackageFileName> : 
    IIdentityResolver
    where TPackageName : class, IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageFileName : class, IPackageFileName
  {
    TPackageName ResolvePackageName(string name);

    TPackageVersion ResolvePackageVersion(string version);

    TPackageIdentity FusePackageIdentity(TPackageName name, TPackageVersion version);

    TPackageIdentity ResolvePackageIdentity(string packageName, string packageVersion);

    PackageUrlIdentity GetPackageUrlIdentity(
      TPackageIdentity packageIdentity,
      TPackageFileName? packageFileName);

    IConverter<string, TPackageName> NameResolver { get; }

    IConverter<string, TPackageVersion> VersionResolver { get; }

    IConverter<(TPackageName Name, TPackageVersion Version), TPackageIdentity> IdentityFuser { get; }

    IConverter<(string Name, string Version), TPackageIdentity> IdentityResolver { get; }

    IConverter<IPackageIdentity, TPackageIdentity> IdentityDowncaster { get; }
  }
}
