// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetUpstreamMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PublicClient;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetUpstreamMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetUpstreamMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>> Bootstrap()
    {
      IFactory<UpstreamSource, Task<IUpstreamNuGetClient>> upstreamClientFactory = new UpstreamNuGetClientFactoryBootstrapper(this.requestContext).Bootstrap();
      DefaultTimeProvider timeProvider = new DefaultTimeProvider();
      PackageIngestionVersionToExceptionConverter versionValidator = new PackageIngestionVersionToExceptionConverter(this.requestContext.GetFeatureFlagFacade());
      ITracerService tracer = this.requestContext.GetTracerFacade();
      return ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>>>) (async upstreamSource =>
      {
        UpstreamSource source = upstreamSource;
        return PackageFilteringUpstreamMetadataServiceDecorator.Create<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>((IUpstreamMetadataService<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, INuGetMetadataEntry>) new NuGetUpstreamOnlyMetadataStore(source, await upstreamClientFactory.Get(upstreamSource), (ITimeProvider) timeProvider, (IConverter<string, Exception>) versionValidator, tracer, FeatureEnabledConstants.PropagateDelistFromUpstream.Bootstrap(this.requestContext)), (IConverter<VssNuGetPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap(), NuGetIdentityResolver.Instance.IdentityFuser);
      }));
    }
  }
}
