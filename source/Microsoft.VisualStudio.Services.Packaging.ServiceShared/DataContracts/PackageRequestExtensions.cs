// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public static class PackageRequestExtensions
  {
    public static PackageNameQuery<TMetadataEntry> ToSingleVersionPackageNameQuery<TPackageIdentity, TMetadataEntry>(
      this IPackageRequest<TPackageIdentity> packageRequest)
      where TPackageIdentity : IPackageIdentity
      where TMetadataEntry : IMetadataEntry<TPackageIdentity>
    {
      PackageNameQuery<TMetadataEntry> packageNameQuery = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>((IFeedRequest) packageRequest, packageRequest.PackageId.Name));
      packageNameQuery.Options = new QueryOptions<TMetadataEntry>()
      {
        VersionUpper = packageRequest.PackageId.Version,
        VersionLower = packageRequest.PackageId.Version
      };
      return packageNameQuery;
    }

    public static IPackageNameRequest<TPackageName> ToPackageNameRequest<TPackageName, TPackageVersion>(
      this IPackageRequest<IPackageIdentity<TPackageName, TPackageVersion>> that)
      where TPackageName : IPackageName
      where TPackageVersion : IPackageVersion
    {
      return (IPackageNameRequest<TPackageName>) new PackageNameRequest<TPackageName>((IFeedRequest) that, that.PackageId.Name);
    }

    public static PackageNameRequest<TPackageName, TData> WithData<TPackageName, TData>(
      this IPackageNameRequest<TPackageName> packageNameRequest,
      TData data)
      where TPackageName : IPackageName
    {
      return new PackageNameRequest<TPackageName, TData>(packageNameRequest, data);
    }

    public static PackageRequest<TPackageId, TData> WithData<TPackageId, TData>(
      this IPackageRequest<TPackageId> packageRequest,
      TData data)
      where TPackageId : IPackageIdentity
    {
      return new PackageRequest<TPackageId, TData>(packageRequest, data);
    }

    public static IFeedRequest<TData> WithData<TData>(this IFeedRequest feedRequest, TData data) => (IFeedRequest<TData>) new FeedRequest<TData>(feedRequest, data);

    public static IPackageRequest<TPackageIdentity> WithPackage<TPackageIdentity>(
      this IFeedRequest feedRequest,
      TPackageIdentity packageIdentity)
      where TPackageIdentity : IPackageIdentity
    {
      return (IPackageRequest<TPackageIdentity>) new PackageRequest<TPackageIdentity>(feedRequest, packageIdentity);
    }

    public static IPackageNameRequest<TPackageName> WithPackageName<TPackageName>(
      this IFeedRequest feedRequest,
      TPackageName packageName)
      where TPackageName : IPackageName
    {
      return (IPackageNameRequest<TPackageName>) new PackageNameRequest<TPackageName>(feedRequest, packageName);
    }

    public static IPackageFileRequest<TPackageIdentity> WithFile<TPackageIdentity>(
      this IPackageRequest<TPackageIdentity> request,
      string filePath)
      where TPackageIdentity : IPackageIdentity
    {
      return (IPackageFileRequest<TPackageIdentity>) new PackageFileRequest<TPackageIdentity>(request, filePath);
    }

    public static RawPackageNameRequest WithRawPackageName(
      this IFeedRequest feedRequest,
      string packageNameString)
    {
      return new RawPackageNameRequest(feedRequest, packageNameString);
    }
  }
}
