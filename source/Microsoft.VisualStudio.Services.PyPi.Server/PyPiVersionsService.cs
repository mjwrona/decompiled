// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiVersionsService
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.PyPi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiVersionsService : IPyPiVersionsService, IVssFrameworkService
  {
    public async Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.PyPi);
      return await new GetPackageVersionHandlerBootstrapper(requestContext).Bootstrap().Handle(new RawPackageRequest<ShowDeletedBool>((IFeedRequest) feedRequest, packageName, packageVersion, (ShowDeletedBool) showDeleted));
    }

    public async Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion)
    {
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.PyPi);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      return await new PyPiDeleteRequestHandlerBootstrapper(requestContext).Bootstrap().Handle(new RawPackageRequest((IFeedRequest) feed1, packageName, packageVersion));
    }

    public async Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.PyPi);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      NullResult nullResult = await new UpdatePackageVersionHandlerBootstrapper(requestContext).Bootstrap().Handle(new RawPackageRequest<PackageVersionDetails>((IFeedRequest) feedRequest, packageName, packageVersion, packageVersionDetails));
    }

    public async Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest)
    {
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.PyPi);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed1.Feed);
      NullResult nullResult = await new PyPiPackagesBatchBootstrapper(requestContext).Bootstrap().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public async Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest)
    {
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.PyPi);
      HttpResponseMessage httpResponseMessage = await new PyPiRecycleBinPackagesBatchBootstrapper(requestContext).Bootstrap().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }
  }
}
