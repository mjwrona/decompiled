// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.JobManagement.MavenChangeProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.Maven.Server.JobManagement
{
  public class MavenChangeProcessingJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public MavenChangeProcessingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedJobQueuer Bootstrap()
    {
      ICommitLog commitLogReader = new MavenCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new ChangeProcessingFeedJobQueuerBootstrapper(this.requestContext, MavenServerConstants.ChangeProcessingJobConstants.MavenChangeProcessingJobCreationInfo, (ICommitLogEndpointReader) commitLogReader).Bootstrap();
    }
  }
}
