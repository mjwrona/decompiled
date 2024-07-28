// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRawPackageRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRawPackageRequestConverter : 
    IConverter<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>>,
    IHaveInputType<IRawPackageRequest>,
    IHaveOutputType<PackageRequest<VssNuGetPackageIdentity>>
  {
    private readonly IConverter<IRawPackageRequest, VssNuGetPackageIdentity> rawPackageRequestToIdentityConverter;

    public NuGetRawPackageRequestConverter(
      IConverter<IRawPackageRequest, VssNuGetPackageIdentity> rawPackageRequestToIdentityConverter)
    {
      this.rawPackageRequestToIdentityConverter = rawPackageRequestToIdentityConverter;
    }

    public PackageRequest<VssNuGetPackageIdentity> Convert(IRawPackageRequest request) => new PackageRequest<VssNuGetPackageIdentity>((IFeedRequest) request, this.rawPackageRequestToIdentityConverter.Convert(request));
  }
}
