// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetGetPackageIndexRawToResolvedRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetGetPackageIndexRawToResolvedRequestConverter : 
    IConverter<NuGetGetPackageIndexRequest<RawPackageNameRequest>, NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>>,
    IHaveInputType<NuGetGetPackageIndexRequest<RawPackageNameRequest>>,
    IHaveOutputType<NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>>
  {
    private readonly IConverter<string, VssNuGetPackageName> packageNameConverter;

    public NuGetGetPackageIndexRawToResolvedRequestConverter(
      IConverter<string, VssNuGetPackageName> packageNameConverter)
    {
      this.packageNameConverter = packageNameConverter;
    }

    public NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>> Convert(
      NuGetGetPackageIndexRequest<RawPackageNameRequest> request)
    {
      return new NuGetGetPackageIndexRequest<PackageNameRequest<VssNuGetPackageName>>(new PackageNameRequest<VssNuGetPackageName>((IFeedRequest) request, this.packageNameConverter.Convert(request.PackageRequest.PackageName)), request.PageSize, request.SpillToPagesThreshold, request.IncludeSemVer2Versions);
    }
  }
}
