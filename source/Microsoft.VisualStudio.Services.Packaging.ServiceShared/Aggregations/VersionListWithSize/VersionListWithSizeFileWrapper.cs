// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeFileWrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public class VersionListWithSizeFileWrapper : 
    IMutableVersionListWithSizeFile,
    ILazyVersionListWithSizeFile,
    IVersionCountsImplementationMetrics
  {
    private readonly IConverter<string, IPackageName> PackageNameConverter;
    private readonly IConverter<string, IPackageVersion> PackageVersionConverter;
    private readonly IEqualityComparer<string> FileNameComparer;

    public VersionListWithSizeFile Wrapped { get; }

    public IEnumerable<ILazyVersionListWithSizePackage> Packages => (IEnumerable<ILazyVersionListWithSizePackage>) this.Wrapped.Packages;

    public DateTime LastModified => this.Wrapped.LastModifiedAsDateTime;

    public bool NeedsSave => this.Wrapped.NeedsSave;

    public VersionListWithSizeFileWrapper(
      VersionListWithSizeFile wrapped,
      IConverter<string, IPackageName> packageNameConverter,
      IConverter<string, IPackageVersion> packageVersionConverter,
      IEqualityComparer<string> fileNameComparer)
    {
      this.Wrapped = wrapped;
      this.PackageNameConverter = packageNameConverter;
      this.PackageVersionConverter = packageVersionConverter;
      this.FileNameComparer = fileNameComparer;
    }

    public void AddPackageVersionToFeed(
      IPackageIdentity packageIdentity,
      DateTime modTime,
      List<IPackageFile> packageFiles)
    {
      VersionListWithSizePackage package = this.GetUnpackedPackageOrDefault(packageIdentity.Name);
      if (package == null)
      {
        package = VersionListWithSizePackage.CreateNewUnpacked(packageIdentity.Name.DisplayName, this.Wrapped, modTime);
        this.Wrapped.Packages.Add(package);
        this.NotifyPackageAndFileModified(package, modTime);
      }
      VersionListWithSizePackageVersion version = this.GetPackageVersionWithSizeOrDefault(package, packageIdentity.Version);
      if (version == null)
      {
        version = VersionListWithSizePackageVersion.CreateNewUnpacked(packageIdentity, package);
        package.Versions.Add(version);
        this.NotifyVersionPackageAndFileModified(version, modTime);
      }
      IEnumerable<IPackageFile> packageFiles1;
      if (packageFiles == null)
      {
        packageFiles1 = (IEnumerable<IPackageFile>) null;
      }
      else
      {
        IEnumerable<IPackageFile> source = packageFiles.Where<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId != null));
        packageFiles1 = source != null ? source.Where<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal)) : (IEnumerable<IPackageFile>) null;
      }
      foreach (IPackageFile packageFile in packageFiles1)
      {
        if (this.GetVersionFileWithSizeOrDefault(version, packageFile) == null)
        {
          VersionListWithSizePackageVersionFile newUnpacked = VersionListWithSizePackageVersionFile.CreateNewUnpacked(packageFile.Path, version);
          newUnpacked.SizeInBytes = (double) packageFile.SizeInBytes;
          version.PackageFiles.Add(newUnpacked);
          this.NotifyVersionFileVersionPackageAndFileModified(newUnpacked, modTime);
        }
      }
      if (!package.DisplayName.Equals(packageIdentity.Name.DisplayName, StringComparison.Ordinal))
      {
        package.DisplayName = packageIdentity.Name.DisplayName;
        this.NotifyPackageAndFileModified(package, modTime);
      }
      if (version.DisplayVersion.Equals(packageIdentity.Version.DisplayVersion, StringComparison.Ordinal))
        return;
      version.DisplayVersion = packageIdentity.Version.DisplayVersion;
      this.NotifyVersionPackageAndFileModified(version, modTime);
    }

    public void SetPackageVersionDeletedState(
      IPackageIdentity packageIdentity,
      bool isDeleted,
      DateTime modTime)
    {
      VersionListWithSizePackageVersion withSizeOrDefault = this.GetPackageVersionWithSizeOrDefault(packageIdentity);
      if (withSizeOrDefault == null)
        return;
      withSizeOrDefault.IsDeleted = isDeleted;
      this.NotifyVersionPackageAndFileModified(withSizeOrDefault, modTime);
    }

    public void PermanentlyDeletePackageVersionFromFeed(
      IPackageIdentity packageIdentity,
      DateTime modTime)
    {
      VersionListWithSizePackageVersion withSizeOrDefault = this.GetPackageVersionWithSizeOrDefault(packageIdentity);
      if (withSizeOrDefault == null)
        return;
      VersionListWithSizePackage package = withSizeOrDefault.Package;
      package.Versions.Remove(withSizeOrDefault);
      if (package.Versions.Count == 0)
        this.Wrapped.Packages.Remove(package);
      this.NotifyVersionPackageAndFileModified(withSizeOrDefault, modTime);
    }

    private VersionListWithSizePackageVersion GetPackageVersionWithSizeOrDefault(
      VersionListWithSizePackage package,
      IPackageVersion packageVersion)
    {
      return package.Versions.FirstOrDefault<VersionListWithSizePackageVersion>((Func<VersionListWithSizePackageVersion, bool>) (x => PackageVersionComparer.NormalizedVersion.Equals(packageVersion, this.PackageVersionConverter.Convert(x.DisplayVersion))));
    }

    private VersionListWithSizePackageVersionFile GetVersionFileWithSizeOrDefault(
      VersionListWithSizePackageVersion version,
      IPackageFile packageFile)
    {
      return version.PackageFiles.FirstOrDefault<VersionListWithSizePackageVersionFile>((Func<VersionListWithSizePackageVersionFile, bool>) (x => this.FileNameComparer.Equals(packageFile.Path, x.FileName)));
    }

    private VersionListWithSizePackage GetUnpackedPackageOrDefault(IPackageName packageName)
    {
      VersionListWithSizePackage package = this.Wrapped.Packages.FirstOrDefault<VersionListWithSizePackage>((Func<VersionListWithSizePackage, bool>) (x => PackageNameComparer.NormalizedName.Equals(packageName, this.PackageNameConverter.Convert(x.DisplayName))));
      if (package != null)
        VersionListWithSizeFileUnpacker.EnsurePackageUnpacked(package);
      return package;
    }

    private VersionListWithSizePackageVersion GetPackageVersionWithSizeOrDefault(
      IPackageIdentity packageIdentity)
    {
      VersionListWithSizePackage packageOrDefault = this.GetUnpackedPackageOrDefault(packageIdentity.Name);
      return packageOrDefault == null ? (VersionListWithSizePackageVersion) null : this.GetPackageVersionWithSizeOrDefault(packageOrDefault, packageIdentity.Version);
    }

    private void NotifyVersionPackageAndFileModified(
      VersionListWithSizePackageVersion version,
      DateTime modTime)
    {
      this.NotifyPackageAndFileModified(version.Package, modTime);
    }

    private void NotifyVersionFileVersionPackageAndFileModified(
      VersionListWithSizePackageVersionFile file,
      DateTime modTime)
    {
      this.NotifyVersionPackageAndFileModified(file.Version, modTime);
    }

    private void NotifyPackageAndFileModified(VersionListWithSizePackage package, DateTime modTime)
    {
      package.NotifyModified(modTime);
      this.NotifyFileModified(this.Wrapped, modTime);
    }

    private void NotifyFileModified(
      VersionListWithSizeFile versionListWithSizeFile,
      DateTime modTime)
    {
      versionListWithSizeFile.NotifyModified(modTime);
    }

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
