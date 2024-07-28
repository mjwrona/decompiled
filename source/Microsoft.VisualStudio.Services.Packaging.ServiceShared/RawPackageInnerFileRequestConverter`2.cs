// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RawPackageInnerFileRequestConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RawPackageInnerFileRequestConverter<TPackageId, TData> : 
    IConverter<IRawPackageInnerFileRequest<TData>, IPackageInnerFileRequest<TPackageId, TData>>,
    IHaveInputType<IRawPackageInnerFileRequest<TData>>,
    IHaveOutputType<IPackageInnerFileRequest<TPackageId, TData>>
    where TPackageId : IPackageIdentity
    where TData : class
  {
    private readonly IConverter<IRawPackageRequest, TPackageId> rawRequestToIdentityConverter;

    public RawPackageInnerFileRequestConverter(
      IConverter<IRawPackageRequest, TPackageId> rawRequestToIdentityConverter)
    {
      this.rawRequestToIdentityConverter = rawRequestToIdentityConverter;
    }

    public IPackageInnerFileRequest<TPackageId, TData> Convert(
      IRawPackageInnerFileRequest<TData> input)
    {
      TPackageId packageId = this.rawRequestToIdentityConverter.Convert((IRawPackageRequest) input);
      return (IPackageInnerFileRequest<TPackageId, TData>) new PackageInnerFileRequest<TPackageId, TData>((IFeedRequest) input, packageId, input.FilePath, input.InnerFilePath, input.AdditionalData);
    }
  }
}
