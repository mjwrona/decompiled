// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CommitLog.CargoCommitLogFacadeBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CommitLog
{
  [ExcludeFromCodeCoverage]
  public class CargoCommitLogFacadeBootstrapper : IBootstrapper<ICommitLog>
  {
    private readonly IVssRequestContext requestContext;

    public CargoCommitLogFacadeBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ICommitLog Bootstrap() => (ICommitLog) new CommitLogFacade<CargoCommitLogService>(this.requestContext, (IFeedItemStoreContainerProvider) new FeedItemStoreContainerProvider<CargoItemStore>(this.requestContext));
  }
}
