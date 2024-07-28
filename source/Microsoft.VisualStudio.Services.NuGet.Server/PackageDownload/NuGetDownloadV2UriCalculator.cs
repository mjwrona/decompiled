// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload.NuGetDownloadV2UriCalculator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.Facades.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Location;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageDownload
{
  public class NuGetDownloadV2UriCalculator : 
    IHandler<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>,
    IConverter<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>>,
    IHaveInputType<PackageIdentityBatchRequest>,
    IHaveOutputType<IDictionary<IPackageIdentity, Uri>>
  {
    private readonly ILocationFacade locationFacade;

    public NuGetDownloadV2UriCalculator(ILocationFacade locationFacade) => this.locationFacade = locationFacade;

    public IDictionary<IPackageIdentity, Uri> Handle(PackageIdentityBatchRequest request) => this.Compute(request);

    public IDictionary<IPackageIdentity, Uri> Convert(PackageIdentityBatchRequest input) => this.Compute(input);

    private IDictionary<IPackageIdentity, Uri> Compute(PackageIdentityBatchRequest request)
    {
      UriBuilder uriBuilder = new UriBuilder(this.locationFacade.GetResourceUri("nuget", ResourceIds.V2BaseResourceId, (IFeedRequest) request, PackagingUriNamePreference.PreferUserSuppliedNameOrId, (object) null));
      return (IDictionary<IPackageIdentity, Uri>) request.Packages.ToDictionary<IPackageIdentity, IPackageIdentity, Uri>((Func<IPackageIdentity, IPackageIdentity>) (packageIdentity => packageIdentity), (Func<IPackageIdentity, Uri>) (packageIdentity =>
      {
        uriBuilder.Query = "id=" + Uri.EscapeDataString(packageIdentity.Name.NormalizedName) + "&version=" + Uri.EscapeDataString(packageIdentity.Version.NormalizedVersion);
        return uriBuilder.Uri;
      }));
    }
  }
}
