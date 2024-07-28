// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenCommitLogFacadeBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Services;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenCommitLogFacadeBootstrapper : IBootstrapper<ICommitLog>
  {
    private IVssRequestContext requestContext;

    public MavenCommitLogFacadeBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICommitLog Bootstrap() => (ICommitLog) new CommitLogFacade<MavenCommitLogService>(this.requestContext, (IFeedItemStoreContainerProvider) new FeedItemStoreContainerProvider<MavenItemStore>(this.requestContext));
  }
}
