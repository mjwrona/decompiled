// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.NpmUpdatePackageVersionCiDataHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public class NpmUpdatePackageVersionCiDataHandler : 
    IAsyncHandler<(PackageRequest<NpmPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<NpmPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public NpmUpdatePackageVersionCiDataHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<NpmPackageIdentity, PackageVersionDetails> Request, ICommitOperationData Op) input)
    {
      PackageRequest<NpmPackageIdentity, PackageVersionDetails> request = input.Request;
      switch (input.Op)
      {
        case IViewOperationData viewOperationData:
          IVssRequestContext requestContext = this.requestContext;
          IProtocol protocol = request.Protocol;
          FeedCore feed = request.Feed;
          FeedView promotedView = new FeedView();
          promotedView.Id = viewOperationData.ViewId;
          FeedView[] existingViews = new FeedView[0];
          string normalizedName = viewOperationData.Identity.Name.NormalizedName;
          string normalizedVersion = viewOperationData.Identity.Version.NormalizedVersion;
          return Task.FromResult<ICiData>((ICiData) new PromotePackageCiData(requestContext, protocol, feed, promotedView, existingViews, normalizedName, normalizedVersion, long.MinValue));
        case NpmDeprecateOperationData deprecateOperationData:
          return Task.FromResult<ICiData>((ICiData) NpmCiDataFactory.GetNpmDeprecateCiData(this.requestContext, request.Feed, deprecateOperationData.Identity.Name.NormalizedName, deprecateOperationData.Identity.Version.NormalizedVersion, long.MinValue, "feed"));
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
