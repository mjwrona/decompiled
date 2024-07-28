// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetChangeProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetChangeProcessingJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetChangeProcessingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedJobQueuer Bootstrap()
    {
      ICommitLog commitLogReader = new NuGetCommitLogFacadeBootstrapper(this.requestContext).Bootstrap();
      return new ChangeProcessingFeedJobQueuerBootstrapper(this.requestContext, NuGetServerConstants.ChangeProcessingJobConstants.NuGetChangeProcessingJobCreationInfo, (ICommitLogEndpointReader) commitLogReader).Bootstrap();
    }
  }
}
