// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmRefreshListTrimmingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmRefreshListTrimmingJobQueuerBootstrapper : 
    IBootstrapper<IMultiFeedRefreshListTrimmingJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRefreshListTrimmingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMultiFeedRefreshListTrimmingJobQueuer Bootstrap() => (IMultiFeedRefreshListTrimmingJobQueuer) new MultiFeedRefreshListTrimmingJobQueuer((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), "Microsoft.VisualStudio.Services.Npm.Upstreams.NpmMultiFeedRefreshListTrimmingJob", "NpmMultiFeedRefreshListTrimmingJob");
  }
}
