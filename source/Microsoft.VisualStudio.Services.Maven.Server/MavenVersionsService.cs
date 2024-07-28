// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenVersionsService
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Aggregations;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.Maven;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenVersionsService : 
    IMavenPackageVersionService,
    IMavenVersionsService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public async Task DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      ICommitLogEntry commitLogEntry = await new MavenDeleteRequestHandlerBootstrapper(requestContext).Bootstrap().Handle(new MavenRawPackageRequest((IFeedRequest) feed1, groupId, artifactId, packageVersion));
    }

    [ControllerMethodTraceFilter(12090840)]
    public async Task PermanentDeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      ICommitLogEntry commitLogEntry = await new MavenPermanentDeleteRequestHandlerBootstrapper(requestContext).Bootstrap().Handle((RawPackageRequest) new MavenRawPackageRequest((IFeedRequest) feed1, groupId, artifactId, version));
    }

    [ControllerMethodTraceFilter(12090830)]
    public async Task RestorePackageVersionToFeed(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      IRecycleBinPackageVersionDetails packageVersionDetails)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      ArgumentUtility.CheckForNull<IRecycleBinPackageVersionDetails>(packageVersionDetails, nameof (packageVersionDetails));
      MavenSecurityHelper.CheckForReadAndAddPackagePermissions(requestContext, feed);
      MavenRawPackageRequest<IRecycleBinPackageVersionDetails> request = new MavenRawPackageRequest<IRecycleBinPackageVersionDetails>((IFeedRequest) new FeedRequest(feed, (IProtocol) Protocol.Maven), groupId, artifactId, version, packageVersionDetails);
      ICommitLogEntry commitLogEntry = await new MavenRestoreToFeedRequestHandlerBootstrapper(requestContext).Bootstrap().Handle((RawPackageRequest<IRecycleBinPackageVersionDetails>) request);
    }

    public Task<MavenPackageFileResponse> GetPackageFile(
      IVssRequestContext requestContext,
      FeedCore feed,
      string path,
      bool requireContent,
      bool streamContent = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(path, nameof (path));
      MavenSecurityHelper.CheckForReadFeedPermission(requestContext, feed);
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      MavenFileRequest request = new MavenResolveRawFileRequestConverterBootstrapper(requestContext).Bootstrap().Convert(new MavenRawFileRequest(feedRequest, path, requireContent, streamContent));
      IRequireAggHandlerBootstrapper<MavenFileRequest, MavenPackageFileResponse> singleBootstrapper = request.FilePath is IMavenArtifactFilePath ? (IRequireAggHandlerBootstrapper<MavenFileRequest, MavenPackageFileResponse>) new MavenGetPackageFileContentHandlerBootstrapper(requestContext) : (IRequireAggHandlerBootstrapper<MavenFileRequest, MavenPackageFileResponse>) new MavenGetPackageFileMetadataHandlerBootstrapper(requestContext);
      return MavenAggregationResolver.Bootstrap(requestContext).HandlerFor<MavenFileRequest, MavenPackageFileResponse>((IRequireAggBootstrapper<IAsyncHandler<MavenFileRequest, MavenPackageFileResponse>>) singleBootstrapper).Handle(request);
    }

    public async Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion,
      bool showDeleted)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      MavenSecurityHelper.CheckForReadFeedPermission(requestContext, feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      return new MavenMetadataEntryToPackageConverter().Convert(await new MavenGetPackageVersionMetadataHandlerBootstrapper(requestContext).Bootstrap().Handle(new MavenRawPackageRequest<ShowDeletedBool>((IFeedRequest) feed1, groupId, artifactId, packageVersion, (ShowDeletedBool) showDeleted)));
    }

    [ControllerMethodTraceFilter(12090880)]
    public Task<IMavenMetadataEntry> GetPackageVersionMetadataFromRecycleBin(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      MavenSecurityHelper.CheckForReadFeedPermission(requestContext, feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      return new MavenGetPackageVersionMetadataFromRecycleBinHandlerBootstrapper(requestContext).Bootstrap().Handle(new MavenRawPackageRequest((IFeedRequest) feed1, groupId, artifactId, version));
    }

    public async Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<MavenPackagesBatchRequest>(batchRequest, nameof (batchRequest));
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      ICommitLogEntry commitLogEntry = await new MavenBatchUpdateBootstrapper(requestContext).Bootstrap().Handle(new MavenBatchRawRequest((IFeedRequest) feedRequest, batchRequest));
    }

    public async Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      PackageVersionDetails packageVersionDetails)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<string>(groupId, nameof (groupId));
      ArgumentUtility.CheckForNull<string>(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNull<string>(version, nameof (version));
      ArgumentUtility.CheckForNull<PackageVersionDetails>(packageVersionDetails, nameof (packageVersionDetails));
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      FeedRequest feed1 = new FeedRequest(feed, (IProtocol) Protocol.Maven);
      ViewUtils.GetViewFromPatchOperation(packageVersionDetails.Views);
      ICommitLogEntry commitLogEntry = await new MavenUpdateRequestHandlerBootstrapper(requestContext).Bootstrap().Handle(new MavenRawPackageRequest<PackageVersionDetails>((IFeedRequest) feed1, groupId, artifactId, version, packageVersionDetails));
    }

    public async Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest)
    {
      FeedValidator.GetFeedIsNotReadOnlyValidator().Validate(feed);
      ArgumentUtility.CheckForNull<MavenPackagesBatchRequest>(batchRequest, nameof (batchRequest));
      IBatchOperationType operationType = batchRequest.GetOperationType();
      if (operationType != BatchOperationType.PermanentDelete)
        throw new ArgumentException(Resources.Error_InvalidBatchOperation((object) operationType));
      await this.UpdatePackageVersions(requestContext, feed, batchRequest);
    }
  }
}
