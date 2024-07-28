// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetFindPackageVersionHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Upstreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.FindPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  public class NuGetFindPackageVersionHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetFindPackageVersionHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageRequest, NullResult> Bootstrap()
    {
      IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> upstreamMetadataManagerFactory = new NuGetUpstreamMetadataManagerFactoryBootstrapper(this.requestContext).Bootstrap();
      return (IAsyncHandler<RawPackageRequest, NullResult>) new RawPackageRequestConverter<VssNuGetPackageIdentity>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap()).ThenDelegateTo<IRawPackageRequest, PackageRequest<VssNuGetPackageIdentity>, NullResult>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NullResult>) new FindPackageVersionHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>(new NuGetMetadataHandlerBootstrapper(this.requestContext).Bootstrap(), upstreamMetadataManagerFactory, (IPackagingTraces) new PackagingTracesFacade(this.requestContext)));
    }
  }
}
