// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Telemetry.PyPiUpdatePackageVersionCiDataHandler`2
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Telemetry
{
  public class PyPiUpdatePackageVersionCiDataHandler<TReq, TOp> : 
    IAsyncHandler<(TReq Request, TOp Op), ICiData>,
    IHaveInputType<(TReq Request, TOp Op)>,
    IHaveOutputType<ICiData>
    where TReq : IFeedRequest
    where TOp : ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;

    public PyPiUpdatePackageVersionCiDataHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle((TReq Request, TOp Op) input)
    {
      if (!(input.Op is IViewOperationData op))
        throw new InvalidOperationException();
      PyPiPackageIdentity identity = (PyPiPackageIdentity) op.Identity;
      IVssRequestContext requestContext = this.requestContext;
      Protocol pyPi = Protocol.PyPi;
      FeedCore feed = input.Request.Feed;
      FeedView promotedView = new FeedView();
      promotedView.Id = op.ViewId;
      FeedView[] existingViews = new FeedView[0];
      string normalizedName = identity.Name.NormalizedName;
      string normalizedVersion = identity.Version.NormalizedVersion;
      return Task.FromResult<ICiData>((ICiData) new PromotePackageCiData(requestContext, (IProtocol) pyPi, feed, promotedView, existingViews, normalizedName, normalizedVersion, long.MinValue));
    }
  }
}
