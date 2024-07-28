// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V2BatchDownloadUrlPopulator
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V2BatchDownloadUrlPopulator : 
    IConverter<V2GetDownloadUrlBatchRequest, IEnumerable<V2FeedPackage>>,
    IHaveInputType<V2GetDownloadUrlBatchRequest>,
    IHaveOutputType<IEnumerable<V2FeedPackage>>
  {
    private readonly IConverter<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> batchPackageDownloadUriComputer;

    public V2BatchDownloadUrlPopulator(
      IConverter<PackageIdentityBatchRequest, IDictionary<IPackageIdentity, Uri>> batchPackageDownloadUriComputer)
    {
      this.batchPackageDownloadUriComputer = batchPackageDownloadUriComputer;
    }

    public IEnumerable<V2FeedPackage> Convert(V2GetDownloadUrlBatchRequest input)
    {
      IDictionary<IPackageIdentity, Uri> packageIdentityToDownloadUri = this.batchPackageDownloadUriComputer.Convert(new PackageIdentityBatchRequest((IFeedRequest) input, (ICollection<IPackageIdentity>) input.Packages.Select<ServerV2FeedPackage, VssNuGetPackageIdentity>((Func<ServerV2FeedPackage, VssNuGetPackageIdentity>) (package => package.PackageIdentity)).Distinct<VssNuGetPackageIdentity>().ToArray<VssNuGetPackageIdentity>()));
      return (IEnumerable<V2FeedPackage>) input.Packages.Select<ServerV2FeedPackage, ServerV2FeedPackage>((Func<ServerV2FeedPackage, ServerV2FeedPackage>) (package =>
      {
        package.DownloadUrl = packageIdentityToDownloadUri[(IPackageIdentity) package.PackageIdentity];
        return package;
      }));
    }
  }
}
