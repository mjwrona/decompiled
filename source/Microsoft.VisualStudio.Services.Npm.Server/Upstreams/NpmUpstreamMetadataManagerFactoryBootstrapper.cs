// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamMetadataManagerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamMetadataManagerFactoryBootstrapper : 
    IBootstrapper<
    #nullable disable
    IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory;

    public NpmUpstreamMetadataManagerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IFactory<UpstreamSource, Task<IUpstreamMetadataService<NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>>> upstreamMetadataServiceFactory = null)
    {
      this.requestContext = requestContext;
      this.upstreamMetadataServiceFactory = upstreamMetadataServiceFactory;
    }

    public IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> Bootstrap() => NpmAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IUpstreamMetadataManager, INpmMetadataService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>>((Func<INpmMetadataService, IUpstreamVersionListService<NpmPackageName, SemanticVersion>, IUpstreamMetadataManager>) ((localMetadataService, upstreamVersionListService) => new NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) localMetadataService, upstreamVersionListService, this.upstreamMetadataServiceFactory).Bootstrap()));
  }
}
