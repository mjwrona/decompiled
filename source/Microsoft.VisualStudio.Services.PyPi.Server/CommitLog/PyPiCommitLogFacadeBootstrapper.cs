// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiCommitLogFacadeBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  public class PyPiCommitLogFacadeBootstrapper : IBootstrapper<ICommitLog>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiCommitLogFacadeBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICommitLog Bootstrap() => (ICommitLog) new CommitLogFacade<PyPiCommitLogService>(this.requestContext, (IFeedItemStoreContainerProvider) new FeedItemStoreContainerProvider<PyPiItemStore>(this.requestContext));
  }
}
