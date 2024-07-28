// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmCommitLogFacadeBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmCommitLogFacadeBootstrapper : IBootstrapper<ICommitLog>
  {
    private IVssRequestContext requestContext;

    public NpmCommitLogFacadeBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICommitLog Bootstrap() => (ICommitLog) new CommitLogFacade<INpmCommitLogService>(this.requestContext, (IFeedItemStoreContainerProvider) new FeedItemStoreContainerProvider<NpmItemStore>(this.requestContext));
  }
}
