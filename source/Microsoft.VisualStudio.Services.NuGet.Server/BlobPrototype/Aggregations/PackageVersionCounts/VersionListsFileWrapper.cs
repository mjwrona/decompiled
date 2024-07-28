// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsFileWrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class VersionListsFileWrapper : 
    IMutableVersionListsFile,
    ILazyVersionListsFile,
    IVersionCountsImplementationMetrics
  {
    public VersionListsFile Wrapped { get; }

    public IEnumerable<ILazyVersionListsPackage> Packages => (IEnumerable<ILazyVersionListsPackage>) this.Wrapped.Packages;

    public DateTime LastModified => this.Wrapped.LastModifiedAsDateTime;

    public bool NeedsSave => this.Wrapped.NeedsSave;

    public VersionListsFileWrapper(VersionListsFile wrapped) => this.Wrapped = wrapped;

    public void AddPackageVersionToFeed(VssNuGetPackageIdentity packageIdentity, DateTime modTime)
    {
      VersionListsPackage package = this.GetUnpackedPackageOrDefault((IPackageName) packageIdentity.Name);
      if (package == null)
      {
        package = VersionListsPackage.CreateNewUnpacked(packageIdentity.Name, this.Wrapped, modTime);
        this.Wrapped.Packages.Add(package);
        this.NotifyPackageAndFileModified(package, modTime);
      }
      VersionListsPackageVersion version = this.GetPackageVersionOrDefault(package, (IPackageVersion) packageIdentity.Version);
      if (version == null)
      {
        version = VersionListsPackageVersion.CreateNewUnpacked(packageIdentity, package);
        package.Versions.Add(version);
        this.NotifyVersionPackageAndFileModified(version, modTime);
      }
      if (!package.Name.DisplayName.Equals(packageIdentity.Name.DisplayName, StringComparison.Ordinal))
      {
        package.Name = packageIdentity.Name;
        this.NotifyPackageAndFileModified(package, modTime);
      }
      if (version.PackageIdentity.Name.DisplayName.Equals(packageIdentity.Name.DisplayName, StringComparison.Ordinal) && version.PackageIdentity.Version.DisplayVersion.Equals(packageIdentity.Version.DisplayVersion, StringComparison.Ordinal))
        return;
      version.PackageIdentity = packageIdentity;
      this.NotifyVersionPackageAndFileModified(version, modTime);
    }

    public void AddPackageVersionToView(
      VssNuGetPackageIdentity packageIdentity,
      Guid viewId,
      DateTime modTime)
    {
      VersionListsPackageVersion versionOrDefault = this.GetPackageVersionOrDefault((IPackageIdentity) packageIdentity);
      if (versionOrDefault == null)
        return;
      if (!versionOrDefault.ViewIds.Contains(viewId))
        versionOrDefault.ViewIds.Add(viewId);
      this.NotifyVersionPackageAndFileModified(versionOrDefault, modTime);
    }

    public void SetPackageVersionListedState(
      VssNuGetPackageIdentity packageIdentity,
      bool isListed,
      DateTime modTime)
    {
      VersionListsPackageVersion versionOrDefault = this.GetPackageVersionOrDefault((IPackageIdentity) packageIdentity);
      if (versionOrDefault == null)
        return;
      versionOrDefault.IsUnlisted = !isListed;
      this.NotifyVersionPackageAndFileModified(versionOrDefault, modTime);
    }

    public void SetPackageVersionDeletedState(
      VssNuGetPackageIdentity packageIdentity,
      bool isDeleted,
      DateTime modTime)
    {
      VersionListsPackageVersion versionOrDefault = this.GetPackageVersionOrDefault((IPackageIdentity) packageIdentity);
      if (versionOrDefault == null)
        return;
      versionOrDefault.IsDeleted = isDeleted;
      this.NotifyVersionPackageAndFileModified(versionOrDefault, modTime);
    }

    public void PermanentlyDeletePackageVersionFromFeed(
      VssNuGetPackageIdentity packageIdentity,
      DateTime modTime)
    {
      VersionListsPackageVersion versionOrDefault = this.GetPackageVersionOrDefault((IPackageIdentity) packageIdentity);
      if (versionOrDefault == null)
        return;
      VersionListsPackage package = versionOrDefault.Package;
      package.Versions.Remove(versionOrDefault);
      if (package.Versions.Count == 0)
        this.Wrapped.Packages.Remove(package);
      this.NotifyVersionPackageAndFileModified(versionOrDefault, modTime);
    }

    private VersionListsPackageVersion GetPackageVersionOrDefault(
      VersionListsPackage package,
      IPackageVersion packageVersion)
    {
      return package.Versions.FirstOrDefault<VersionListsPackageVersion>((Func<VersionListsPackageVersion, bool>) (x => PackageVersionComparer.NormalizedVersion.Equals(packageVersion, (IPackageVersion) x.Version)));
    }

    private VersionListsPackage GetUnpackedPackageOrDefault(IPackageName packageName)
    {
      VersionListsPackage package = this.Wrapped.Packages.FirstOrDefault<VersionListsPackage>((Func<VersionListsPackage, bool>) (x => PackageNameComparer.NormalizedName.Equals(packageName, (IPackageName) x.Name)));
      if (package != null)
        VersionListsFileUnpacker.EnsurePackageUnpacked(this.Wrapped, package);
      return package;
    }

    private VersionListsPackageVersion GetPackageVersionOrDefault(IPackageIdentity packageIdentity)
    {
      VersionListsPackage packageOrDefault = this.GetUnpackedPackageOrDefault(packageIdentity.Name);
      return packageOrDefault == null ? (VersionListsPackageVersion) null : this.GetPackageVersionOrDefault(packageOrDefault, packageIdentity.Version);
    }

    private void NotifyVersionPackageAndFileModified(
      VersionListsPackageVersion version,
      DateTime modTime)
    {
      this.NotifyPackageAndFileModified(version.Package, modTime);
    }

    private void NotifyPackageAndFileModified(VersionListsPackage package, DateTime modTime)
    {
      package.NotifyModified(modTime);
      this.NotifyFileModified(this.Wrapped, modTime);
    }

    private void NotifyFileModified(VersionListsFile versionListsFile, DateTime modTime) => versionListsFile.NotifyModified(modTime);

    public void NotifySaved() => this.Wrapped.NotifySaved();

    public int PackagesUnpacked => this.Wrapped.PackagesUnpacked;

    public int PackagesPacked => this.Wrapped.PackagesPacked;

    public int NumPackagesNeedingUnpack => this.Wrapped.NumPackagesNeedingUnpack;

    public int NumPackagesNeedingRepack => this.Wrapped.NumPackagesNeedingRepack;

    public int NumPackagesNeedingSave => this.Wrapped.NumPackagesNeedingSave;

    public int NumPackages => this.Wrapped.NumPackages;

    public int NumTotalVersions => this.Wrapped.NumTotalVersions;
  }
}
