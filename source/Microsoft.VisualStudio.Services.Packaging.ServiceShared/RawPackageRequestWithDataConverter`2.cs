// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageRequestWithDataConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageRequestWithDataConverter<TPackageId, TData> : 
    IConverter<RawPackageRequest<TData>, PackageRequest<TPackageId, TData>>,
    IHaveInputType<RawPackageRequest<TData>>,
    IHaveOutputType<PackageRequest<TPackageId, TData>>
    where TPackageId : IPackageIdentity
    where TData : class
  {
    private IConverter<IRawPackageRequest, TPackageId> rawPackageRequestToIdentityConverter;

    public RawPackageRequestWithDataConverter(
      IConverter<IRawPackageRequest, TPackageId> rawPackageRequestToIdentityConverter)
    {
      this.rawPackageRequestToIdentityConverter = rawPackageRequestToIdentityConverter;
    }

    public PackageRequest<TPackageId, TData> Convert(RawPackageRequest<TData> input)
    {
      TPackageId packageId = this.rawPackageRequestToIdentityConverter.Convert((IRawPackageRequest) input);
      return new PackageRequest<TPackageId, TData>((IPackageRequest<TPackageId>) new PackageRequest<TPackageId>((IFeedRequest) input, packageId), input.AdditionalData);
    }
  }
}
