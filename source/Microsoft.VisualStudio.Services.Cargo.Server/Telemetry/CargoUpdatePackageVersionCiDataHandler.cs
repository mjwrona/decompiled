// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Telemetry.CargoUpdatePackageVersionCiDataHandler
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Telemetry
{
  public class CargoUpdatePackageVersionCiDataHandler : 
    IAsyncHandler<(PackageRequest<CargoPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<CargoPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public CargoUpdatePackageVersionCiDataHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<CargoPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op) input)
    {
      if (!(input.Op is IViewOperationData op))
        throw new InvalidOperationException();
      CargoPackageIdentity identity = (CargoPackageIdentity) op.Identity;
      IVssRequestContext requestContext = this.requestContext;
      Protocol cargo = Protocol.Cargo;
      FeedCore feed = input.Request.Feed;
      FeedView promotedView = new FeedView();
      promotedView.Id = op.ViewId;
      FeedView[] existingViews = new FeedView[0];
      string normalizedName = identity.Name.NormalizedName;
      string normalizedVersion = identity.Version.NormalizedVersion;
      return Task.FromResult<ICiData>((ICiData) new PromotePackageCiData(requestContext, (IProtocol) cargo, feed, promotedView, existingViews, normalizedName, normalizedVersion, long.MinValue));
    }
  }
}
