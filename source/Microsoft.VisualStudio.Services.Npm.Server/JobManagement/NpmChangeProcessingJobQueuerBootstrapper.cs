// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.JobManagement.NpmChangeProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.Npm.Server.JobManagement
{
  public class NpmChangeProcessingJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NpmChangeProcessingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedJobQueuer Bootstrap()
    {
      ICommitLog commitLogReader = new NpmCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new ChangeProcessingFeedJobQueuerBootstrapper(this.requestContext, NpmConstants.ChangeProcessingJobConstants.NpmChangeProcessingJobCreationInfo, (ICommitLogEndpointReader) commitLogReader).Bootstrap();
    }
  }
}
