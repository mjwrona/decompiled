// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiRefreshListTrimmingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiRefreshListTrimmingJobQueuerBootstrapper : 
    IBootstrapper<IMultiFeedRefreshListTrimmingJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiRefreshListTrimmingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IMultiFeedRefreshListTrimmingJobQueuer Bootstrap() => (IMultiFeedRefreshListTrimmingJobQueuer) new MultiFeedRefreshListTrimmingJobQueuer((IJobQueuer) new JobServiceFacade(this.requestContext, this.requestContext.GetService<ITeamFoundationJobService>()), this.requestContext.GetTracerFacade(), "Microsoft.VisualStudio.Services.PyPi.Server.Plugins.Upstreams.PyPiMultiFeedRefreshListTrimmingJob", "PyPiMultiFeedRefreshListTrimmingJob");
  }
}
