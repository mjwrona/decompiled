// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageFileRequestConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageFileRequestConverter<TPackageId, TData> : 
    IConverter<IRawPackageFileRequest<TData>, PackageFileRequest<TPackageId, TData>>,
    IHaveInputType<IRawPackageFileRequest<TData>>,
    IHaveOutputType<PackageFileRequest<TPackageId, TData>>
    where TPackageId : IPackageIdentity
    where TData : class
  {
    private readonly IConverter<IRawPackageRequest, TPackageId> rawRequestToIdentityConverter;

    public RawPackageFileRequestConverter(
      IConverter<IRawPackageRequest, TPackageId> rawRequestToIdentityConverter)
    {
      this.rawRequestToIdentityConverter = rawRequestToIdentityConverter;
    }

    public PackageFileRequest<TPackageId, TData> Convert(IRawPackageFileRequest<TData> input)
    {
      TPackageId packageId = this.rawRequestToIdentityConverter.Convert((IRawPackageRequest) input);
      return new PackageFileRequest<TPackageId, TData>((IPackageRequest<TPackageId>) new PackageRequest<TPackageId>((IFeedRequest) input, packageId), input.FilePath, input.AdditionalData);
    }
  }
}
