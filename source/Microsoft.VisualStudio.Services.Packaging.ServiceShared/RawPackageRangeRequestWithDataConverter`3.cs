// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageRangeRequestWithDataConverter`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageRangeRequestWithDataConverter<TPackageName, TPackageVersion, TData> : 
    IConverter<RawPackageRangeRequest<TData>, PackageRangeWithDataRequest<TPackageName, TPackageVersion, TData>>,
    IHaveInputType<RawPackageRangeRequest<TData>>,
    IHaveOutputType<PackageRangeWithDataRequest<TPackageName, TPackageVersion, TData>>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
    where TData : class
  {
    private readonly IConverter<string, TPackageName> packageNameConverter;
    private readonly IConverter<string, TPackageVersion> packageVersionConverter;

    public RawPackageRangeRequestWithDataConverter(
      IConverter<string, TPackageName> packageNameConverter,
      IConverter<string, TPackageVersion> packageVersionConverter)
    {
      this.packageNameConverter = packageNameConverter;
      this.packageVersionConverter = packageVersionConverter;
    }

    public PackageRangeWithDataRequest<TPackageName, TPackageVersion, TData> Convert(
      RawPackageRangeRequest<TData> request)
    {
      return new PackageRangeWithDataRequest<TPackageName, TPackageVersion, TData>(new PackageRangeRequest<TPackageName, TPackageVersion>((IFeedRequest) request, this.packageNameConverter.Convert(request.PackageName), this.packageVersionConverter.Convert(request.PackageVersionLower), this.packageVersionConverter.Convert(request.PackageVersionUpper)), request.AdditionalData);
    }
  }
}
