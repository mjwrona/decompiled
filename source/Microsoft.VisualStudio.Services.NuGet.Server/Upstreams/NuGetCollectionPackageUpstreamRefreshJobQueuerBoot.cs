// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetCollectionPackageUpstreamRefreshJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetCollectionPackageUpstreamRefreshJobQueuerBootstrapper : 
    IBootstrapper<ICollectionPackageUpstreamRefreshJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetCollectionPackageUpstreamRefreshJobQueuerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    public ICollectionPackageUpstreamRefreshJobQueuer Bootstrap() => (ICollectionPackageUpstreamRefreshJobQueuer) new CollectionPackageUpstreamRefreshJobQueuer((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.Upstreams.NuGetUpstreamMetadataCachePackageJob", "NuGetUpstreamMetadataCachePackageJob");
  }
}
