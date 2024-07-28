// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2PackageNameRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2PackageNameRequestConverter : 
    IConverter<RawV2PackageNameRequest, V2PackageNameRequest>,
    IHaveInputType<RawV2PackageNameRequest>,
    IHaveOutputType<V2PackageNameRequest>
  {
    private readonly IConverter<string, VssNuGetPackageName> converter;

    public V2PackageNameRequestConverter(IConverter<string, VssNuGetPackageName> converter) => this.converter = converter;

    public V2PackageNameRequest Convert(RawV2PackageNameRequest rawRequest)
    {
      VssNuGetPackageName packageName = this.converter.Convert(rawRequest.PackageName);
      return new V2PackageNameRequest((IFeedRequest) rawRequest, packageName, rawRequest.ODataQueryOptions);
    }
  }
}
