// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.MavenUpstreamMetadataManagerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class MavenUpstreamMetadataManagerFactoryBootstrapper : 
    IBootstrapper<IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenUpstreamMetadataManagerFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> Bootstrap()
    {
      Func<IMavenMetadataAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>, IUpstreamMetadataManager> bootstrapFunc = (Func<IMavenMetadataAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>, IUpstreamMetadataManager>) ((localMetadataService, upstreamVersionListService) => new MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) localMetadataService, upstreamVersionListService).Bootstrap());
      return MavenAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IUpstreamMetadataManager, IMavenMetadataAggregationAccessor, IUpstreamVersionListService<MavenPackageName, MavenPackageVersion>>(bootstrapFunc);
    }
  }
}
