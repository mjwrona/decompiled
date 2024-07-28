// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoUpstreamMetadataCacheJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoUpstreamMetadataCacheJobQueuerBootstrapper : 
    IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public CargoUpstreamMetadataCacheJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICollectionPackageUpstreamRefreshJobQueuer Bootstrap() => (ICollectionPackageUpstreamRefreshJobQueuer) new CollectionPackageUpstreamRefreshJobQueuer((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), "Microsoft.VisualStudio.Services.Cargo.Server.Plugins.Upstreams.CargoUpstreamMetadataCachePackageJob", "CargoUpstreamMetadataCachePackageJob");
  }
}
