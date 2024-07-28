// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Telemetry.MavenUpdatePackageCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Telemetry
{
  public class MavenUpdatePackageCiDataFacadeHandler : 
    IAsyncHandler<(PackageRequest<MavenPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<MavenPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public MavenUpdatePackageCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<MavenPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op) input)
    {
      PackageRequest<MavenPackageIdentity, PackageVersionDetails> request = input.Request;
      Guid result;
      Guid.TryParse(ViewUtils.GetViewFromPatchOperation(request.AdditionalData.Views), out result);
      IVssRequestContext requestContext = this.requestContext;
      Protocol maven = Protocol.Maven;
      FeedCore feed = request.Feed;
      FeedView promotedView = new FeedView();
      promotedView.Id = result;
      FeedView[] existingViews = new FeedView[0];
      string normalizedName = request.PackageId.Name.NormalizedName;
      string normalizedVersion = request.PackageId.Version.NormalizedVersion;
      return Task.FromResult<ICiData>((ICiData) new PromotePackageCiData(requestContext, (IProtocol) maven, feed, promotedView, existingViews, normalizedName, normalizedVersion, long.MinValue));
    }
  }
}
