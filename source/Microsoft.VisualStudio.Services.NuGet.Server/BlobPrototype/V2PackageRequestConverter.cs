// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2PackageRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2PackageRequestConverter : 
    IConverter<RawV2PackageRequest, V2PackageRequest>,
    IHaveInputType<RawV2PackageRequest>,
    IHaveOutputType<V2PackageRequest>
  {
    private IConverter<IRawPackageRequest, VssNuGetPackageIdentity> identityConveter;

    public V2PackageRequestConverter(
      IConverter<IRawPackageRequest, VssNuGetPackageIdentity> identityConveter)
    {
      this.identityConveter = identityConveter;
    }

    public V2PackageRequest Convert(RawV2PackageRequest rawRequest)
    {
      VssNuGetPackageIdentity packageIdentity = this.identityConveter.Convert((IRawPackageRequest) rawRequest);
      return new V2PackageRequest((IFeedRequest) rawRequest, packageIdentity, rawRequest.ODataQueryOptions);
    }
  }
}
