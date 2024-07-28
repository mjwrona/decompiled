// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NuGetRefreshListTrimmingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NuGetRefreshListTrimmingJobQueuerBootstrapper : 
    IBootstrapper<IMultiFeedRefreshListTrimmingJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRefreshListTrimmingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMultiFeedRefreshListTrimmingJobQueuer Bootstrap() => (IMultiFeedRefreshListTrimmingJobQueuer) new MultiFeedRefreshListTrimmingJobQueuer((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.Upstreams.NuGetMultiFeedRefreshListTrimmingJob", "NuGetMultiFeedRefreshListTrimmingJob");
  }
}
