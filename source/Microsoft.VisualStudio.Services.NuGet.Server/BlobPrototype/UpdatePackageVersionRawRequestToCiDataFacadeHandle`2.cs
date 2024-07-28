// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.UpdatePackageVersionRawRequestToCiDataFacadeHandler`2
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class UpdatePackageVersionRawRequestToCiDataFacadeHandler<TReq, TOp> : 
    IAsyncHandler<(TReq Request, TOp Op), ICiData>,
    IHaveInputType<(TReq Request, TOp Op)>,
    IHaveOutputType<ICiData>
    where TReq : IFeedRequest
    where TOp : ICommitOperationData
  {
    private readonly IVssRequestContext requestContext;

    public UpdatePackageVersionRawRequestToCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle((TReq Request, TOp Op) input)
    {
      switch (input.Op)
      {
        case ViewOperationData viewOperationData:
          VssNuGetPackageIdentity identity1 = (VssNuGetPackageIdentity) viewOperationData.Identity;
          IVssRequestContext requestContext = this.requestContext;
          FeedCore feed = input.Request.Feed;
          FeedView view = new FeedView();
          view.Id = viewOperationData.ViewId;
          FeedView[] existingViews = new FeedView[0];
          string displayName = identity1.Name.DisplayName;
          string displayVersion = identity1.Version.DisplayVersion;
          return Task.FromResult<ICiData>((ICiData) NuGetCiDataFactory.GetNuGetPromoteCiData(requestContext, feed, view, existingViews, displayName, displayVersion, long.MinValue));
        case RelistOperationData relistOperationData:
          VssNuGetPackageIdentity identity2 = (VssNuGetPackageIdentity) relistOperationData.Identity;
          return Task.FromResult<ICiData>((ICiData) NuGetCiDataFactory.GetNuGetRelistCiData(this.requestContext, "none", input.Request.Feed, identity2.Name.DisplayName, identity2.Version.DisplayVersion, long.MinValue));
        case DelistOperationData delistOperationData:
          VssNuGetPackageIdentity identity3 = (VssNuGetPackageIdentity) delistOperationData.Identity;
          return Task.FromResult<ICiData>((ICiData) NuGetCiDataFactory.GetNuGetUnlistCiData(this.requestContext, "none", input.Request.Feed, identity3.Name.DisplayName, identity3.Version.DisplayVersion, long.MinValue));
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
