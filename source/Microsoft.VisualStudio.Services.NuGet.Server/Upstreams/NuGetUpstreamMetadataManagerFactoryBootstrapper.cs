// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetUpstreamMetadataManagerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetUpstreamMetadataManagerFactoryBootstrapper : 
    IBootstrapper<
    #nullable disable
    IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> upstreamMetadataServiceFactory;

    public NuGetUpstreamMetadataManagerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> upstreamMetadataServiceFactory = null)
    {
      this.requestContext = requestContext;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory;
    }

    public IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> Bootstrap() => NuGetAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IUpstreamMetadataManager, INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>>((Func<INuGetMetadataService, IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion>, IUpstreamMetadataManager>) ((localMetadataService, upstreamVersionListService) => new NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) localMetadataService, upstreamVersionListService, this.upstreamMetadataServiceFactory).Bootstrap()));
  }
}
