// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamMetadataServiceFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamMetadataServiceFactoryBootstrapper : 
    IBootstrapper<IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmUpstreamMetadataServiceFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> Bootstrap()
    {
      IFactory<UpstreamSource, Task<IUpstreamNpmClient>> upstreamClientFactory = new UpstreamNpmClientFactoryBootstrapper(this.requestContext).Bootstrap();
      return (IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>>) new NpmUpstreamMetadataServiceCacheFactory(ByFuncInputFactory.For<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>>((Func<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>>) (async upstreamSource =>
      {
        UpstreamSource source = upstreamSource;
        return PackageFilteringUpstreamMetadataServiceDecorator.Create<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>((IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>) new NpmUpstreamOnlyMetadataStore(source, await upstreamClientFactory.Get(upstreamSource), FeatureFlagConstants.PropagateDeprecateFromUpstream.Bootstrap(this.requestContext)), (IConverter<NpmPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap(), NpmIdentityResolver.Instance.IdentityFuser);
      })));
    }
  }
}
