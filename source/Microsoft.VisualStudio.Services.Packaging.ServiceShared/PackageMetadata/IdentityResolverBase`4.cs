// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.IdentityResolverBase`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public abstract class IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> : 
    IIdentityResolver<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>,
    IIdentityResolver
    where TPackageName : class, IPackageName
    where TPackageVersion : class, IPackageVersion
    where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageFileName : class, IPackageFileName
  {
    private IConverter<(IPackageName Name, IPackageVersion Version), IPackageIdentity> genericIdentityFuser;

    public abstract TPackageName ResolvePackageName(string name);

    IPackageName IIdentityResolver.ResolvePackageName(string name) => (IPackageName) this.ResolvePackageName(name);

    public abstract TPackageVersion ResolvePackageVersion(string version);

    IPackageVersion IIdentityResolver.ResolvePackageVersion(string version) => (IPackageVersion) this.ResolvePackageVersion(version);

    public abstract TPackageIdentity FusePackageIdentity(TPackageName name, TPackageVersion version);

    public IPackageIdentity FusePackageIdentity(IPackageName name, IPackageVersion version)
    {
      if (!(name is TPackageName name1))
        name1 = this.ResolvePackageName(name.DisplayName);
      if (!(version is TPackageVersion version1))
        version1 = this.ResolvePackageVersion(version.DisplayVersion);
      return (IPackageIdentity) this.FusePackageIdentity(name1, version1);
    }

    public abstract PackageUrlIdentity GetPackageUrlIdentity(
      TPackageIdentity packageIdentity,
      TPackageFileName? packageFileName);

    PackageUrlIdentity IIdentityResolver.GetPackageUrlIdentity(
      IPackageIdentity packageIdentity,
      IPackageFileName? packageFileName)
    {
      return this.GetPackageUrlIdentity((TPackageIdentity) packageIdentity, (TPackageFileName) packageFileName);
    }

    public TPackageIdentity ResolvePackageIdentity(string packageName, string packageVersion) => this.FusePackageIdentity(this.ResolvePackageName(packageName), this.ResolvePackageVersion(packageVersion));

    IPackageIdentity IIdentityResolver.ResolvePackageIdentity(
      string packageName,
      string packageVersion)
    {
      return (IPackageIdentity) this.ResolvePackageIdentity(packageName, packageVersion);
    }

    public IConverter<string, TPackageName> NameResolver { get; }

    IConverter<string, IPackageName> IIdentityResolver.NameResolver => (IConverter<string, IPackageName>) this.NameResolver;

    public IConverter<string, TPackageVersion> VersionResolver { get; }

    IConverter<string, IPackageVersion> IIdentityResolver.VersionResolver => (IConverter<string, IPackageVersion>) this.VersionResolver;

    public IConverter<(TPackageName Name, TPackageVersion Version), TPackageIdentity> IdentityFuser { get; }

    IConverter<(IPackageName Name, IPackageVersion Version), IPackageIdentity> IIdentityResolver.IdentityFuser => this.genericIdentityFuser;

    public IConverter<(string Name, string Version), TPackageIdentity> IdentityResolver { get; }

    IConverter<(string Name, string Version), IPackageIdentity> IIdentityResolver.IdentityResolver => (IConverter<(string, string), IPackageIdentity>) this.IdentityResolver;

    public IConverter<IPackageIdentity, TPackageIdentity> IdentityDowncaster { get; }

    protected IdentityResolverBase()
    {
      this.NameResolver = (IConverter<string, TPackageName>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.NameResolvingConverter(this);
      this.VersionResolver = (IConverter<string, TPackageVersion>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.VersionResolvingConverter(this);
      this.IdentityFuser = (IConverter<(TPackageName, TPackageVersion), TPackageIdentity>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.IdentityFusingConverter(this);
      this.IdentityResolver = (IConverter<(string, string), TPackageIdentity>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.IdentityResolvingConverter(this);
      this.IdentityDowncaster = (IConverter<IPackageIdentity, TPackageIdentity>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.IdentityDowncastConverter(this);
      this.genericIdentityFuser = (IConverter<(IPackageName, IPackageVersion), IPackageIdentity>) new IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>.GenericIdentityFusingConverter(this);
    }

    private class NameResolvingConverter : 
      IConverter<string, TPackageName>,
      IHaveInputType<string>,
      IHaveOutputType<TPackageName>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public NameResolvingConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public TPackageName Convert(string name) => this.identityResolver.ResolvePackageName(name);
    }

    private class VersionResolvingConverter : 
      IConverter<string, TPackageVersion>,
      IHaveInputType<string>,
      IHaveOutputType<TPackageVersion>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public VersionResolvingConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public TPackageVersion Convert(string name) => this.identityResolver.ResolvePackageVersion(name);
    }

    private class GenericIdentityFusingConverter : 
      IConverter<(IPackageName Name, IPackageVersion Version), IPackageIdentity>,
      IHaveInputType<(IPackageName Name, IPackageVersion Version)>,
      IHaveOutputType<IPackageIdentity>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public GenericIdentityFusingConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public IPackageIdentity Convert((IPackageName Name, IPackageVersion Version) input) => this.identityResolver.FusePackageIdentity(input.Name, input.Version);
    }

    private class IdentityFusingConverter : 
      IConverter<(TPackageName Name, TPackageVersion Version), TPackageIdentity>,
      IHaveInputType<(TPackageName Name, TPackageVersion Version)>,
      IHaveOutputType<TPackageIdentity>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public IdentityFusingConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public TPackageIdentity Convert((TPackageName Name, TPackageVersion Version) input) => this.identityResolver.FusePackageIdentity(input.Name, input.Version);
    }

    private class IdentityResolvingConverter : 
      IConverter<(string Name, string Version), TPackageIdentity>,
      IHaveInputType<(string Name, string Version)>,
      IHaveOutputType<TPackageIdentity>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public IdentityResolvingConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public TPackageIdentity Convert((string Name, string Version) input) => this.identityResolver.ResolvePackageIdentity(input.Name, input.Version);
    }

    private class IdentityDowncastConverter : 
      IConverter<IPackageIdentity, TPackageIdentity>,
      IHaveInputType<IPackageIdentity>,
      IHaveOutputType<TPackageIdentity>
    {
      private readonly IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver;

      public IdentityDowncastConverter(
        IdentityResolverBase<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver)
      {
        this.identityResolver = identityResolver;
      }

      public TPackageIdentity Convert(IPackageIdentity input)
      {
        if (input is TPackageIdentity packageIdentity)
          return packageIdentity;
        return input.Name is TPackageName name && input.Version is TPackageVersion version ? this.identityResolver.FusePackageIdentity(name, version) : this.identityResolver.ResolvePackageIdentity(input.Name.DisplayName, input.Version.DisplayVersion);
      }
    }
  }
}
