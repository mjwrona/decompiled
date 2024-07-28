// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.NuGetVersionsService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.Shared.NuGet;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class NuGetVersionsService : INuGetVersionsService, IVssFrameworkService
  {
    public async Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageName, nameof (packageName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.NuGet);
      return await new GetPackageVersionHandlerBootstrapper(requestContext).Bootstrap().TaskYieldOnException<RawPackageRequest<ShowDeletedBool>, Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package>().Handle(new RawPackageRequest<ShowDeletedBool>((IFeedRequest) feedRequest, packageName, packageVersion, (ShowDeletedBool) showDeleted));
    }

    public async Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion)
    {
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageName, nameof (packageName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.NuGet);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      return await IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(requestContext, BlockedIdentityContext.Delete).Bootstrap().ThenActuallyHandleWith<IRawPackageRequest, ContentResult, Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package>(new NuGetDeleteRequestHandlerBootstrapper(requestContext).Bootstrap()).TaskYieldOnException<IRawPackageRequest, Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package>().Handle((IRawPackageRequest) new RawPackageRequest((IFeedRequest) feed1, packageName, packageVersion));
    }

    public async Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageName, nameof (packageName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.NuGet);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      NullResult nullResult = await ((IAsyncHandler<RawPackageRequest<PackageVersionDetails>, ContentResult>) IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(requestContext, BlockedIdentityContext.Update).Bootstrap()).ThenActuallyHandleWith<RawPackageRequest<PackageVersionDetails>, ContentResult, NullResult>(new UpdatePackageVersionHandlerBootstrapper(requestContext).Bootstrap()).TaskYieldOnException<RawPackageRequest<PackageVersionDetails>, NullResult>().Handle(new RawPackageRequest<PackageVersionDetails>((IFeedRequest) feedRequest, packageName, packageVersion, packageVersionDetails));
    }

    public async Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NuGetPackagesBatchRequest batchRequest)
    {
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<NuGetPackagesBatchRequest>(batchRequest, nameof (batchRequest));
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.NuGet);
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      NullResult nullResult = await new NuGetPackagesBatchBootstrapper(requestContext).Bootstrap().TaskYieldOnException<BatchRawRequest, NullResult>().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }

    public async Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NuGetPackagesBatchRequest batchRequest)
    {
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.NuGet);
      HttpResponseMessage httpResponseMessage = await new NuGetRecycleBinPackagesBatchBootstrapper(requestContext).Bootstrap().Handle(new BatchRawRequest((IFeedRequest) feed1, (IPackagesBatchRequest) batchRequest));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
