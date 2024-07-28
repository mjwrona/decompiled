// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamMetadataManagerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamMetadataManagerFactoryBootstrapper : 
    IBootstrapper<
    #nullable disable
    IFactory<IFeedRequest, Task<IUpstreamMetadataManager>>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiUpstreamMetadataManagerFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<IFeedRequest, Task<IUpstreamMetadataManager>> Bootstrap() => PyPiAggregationResolver.Bootstrap(this.requestContext).FactoryFor<IUpstreamMetadataManager, IPyPiMetadataAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>>((Func<IPyPiMetadataAggregationAccessor, IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion>, IUpstreamMetadataManager>) ((localMetadataService, upstreamVersionListService) => new PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper(this.requestContext, (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) localMetadataService, upstreamVersionListService).Bootstrap()));
  }
}
