// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackageNameRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class PackageNameRequestConverter : 
    IConverter<RawPackageNameRequest, PackageNameRequest<VssNuGetPackageName>>,
    IHaveInputType<RawPackageNameRequest>,
    IHaveOutputType<PackageNameRequest<VssNuGetPackageName>>
  {
    private readonly IConverter<string, VssNuGetPackageName> nameConverter;

    public PackageNameRequestConverter(
      IConverter<string, VssNuGetPackageName> nameConverter)
    {
      this.nameConverter = nameConverter;
    }

    public PackageNameRequest<VssNuGetPackageName> Convert(RawPackageNameRequest request) => new PackageNameRequest<VssNuGetPackageName>((IFeedRequest) request, this.nameConverter.Convert(request.PackageName));
  }
}
