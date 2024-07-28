// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.NuGetPackageIngestionService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Drop.WebApi;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.HttpStreams;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.Server.ItemStore;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  internal class NuGetPackageIngestionService : 
    PackageIngestionServiceBase,
    IPackageIngestionService,
    IVssFrameworkService
  {
    private const string BlobScope = "nuget";
    private readonly ITimeProvider timeProvider;
    private static readonly TimeSpan TemporaryReferenceLifespan = TimeSpan.FromHours(1.0);

    public NuGetPackageIngestionService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public NuGetPackageIngestionService(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    protected override string ProtocolReadOnlyFeatureFlagName => "Packaging.NuGet.ReadOnly";

    public async Task AddPackageFromStreamAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      Stream nupkgStream,
      string protocolVersion,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      bool addAsDelisted,
      VssNuGetPackageIdentity expectedIdentity = null)
    {
      NuGetPackageIngestionService ingestionService = this;
      if (ingestionService.IsFeatureReadOnly(requestContext))
        throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuGetServiceReadOnly());
      ArgumentUtility.CheckForNull<Stream>(nupkgStream, nameof (nupkgStream));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722830, nameof (AddPackageFromStreamAsync)))
      {
        tracer.TraceInfo(string.Format("Adding package from stream (feed : {0})", (object) feedRequest.Feed.Id), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        NuGetPackageIngestionService.CheckIngestionPermission(requestContext, feedRequest, sourceChain);
        try
        {
          long maxPackageSize = IngestionUtils.GetMaxPackageSize(requestContext);
          if (nupkgStream.Length > maxPackageSize)
          {
            if (sourceChain == null || !sourceChain.Any<UpstreamSourceInfo>())
              throw new TooLargePackagePushException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageTooLarge((object) maxPackageSize));
            tracer.TraceInfo(string.Format("Adding large package from stream (size : {0}) (feed : {1})", (object) nupkgStream.Length, (object) feedRequest.Feed.Id), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
          }
          nupkgStream.Seek(0L, SeekOrigin.Begin);
          Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdWithBlocks = await nupkgStream.CalculateBlobIdentifierWithBlocksAsync((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance);
          BlobStorageId packageContent = new BlobStorageId(blobIdWithBlocks.BlobId);
          StoredNupkgItemInfo nupkgInfo = await ingestionService.PrepareAddPackageAsync(requestContext, feedRequest, nupkgStream, (IStorageId) packageContent, sourceChain, expectedIdentity, addAsDelisted);
          IBlobStore blobStore = NuGetPackageIngestionService.GetBlobStore(requestContext);
          BlobReference temporaryRef = ingestionService.GetTemporaryReference();
          if (!await blobStore.TryReferenceWithBlocksAsync(requestContext, WellKnownDomainIds.OriginalDomainId, blobIdWithBlocks, temporaryRef))
            await ingestionService.UploadBlobAndReferenceAsync(requestContext, nupkgStream, blobStore, blobIdWithBlocks.BlobId, temporaryRef);
          await ingestionService.AddPackageCoreAsync(requestContext, feedRequest, nupkgInfo, protocolVersion, new NuGetPackagingTelemetryBuilder().Build(requestContext));
          blobIdWithBlocks = (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null;
          nupkgInfo = (StoredNupkgItemInfo) null;
          blobStore = (IBlobStore) null;
          temporaryRef = (BlobReference) null;
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    public async Task AddPackageFromBlobAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      BlobIdentifierWithOrWithoutBlocks blobIdWithOrWithoutBlocks,
      string protocolVersion,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      bool addAsDelisted,
      VssNuGetPackageIdentity expectedIdentity = null)
    {
      NuGetPackageIngestionService ingestionService = this;
      if (ingestionService.IsFeatureReadOnly(requestContext))
        throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuGetServiceReadOnly());
      Guid id = feedRequest.Feed.Id;
      ArgumentUtility.CheckForNull<BlobIdentifierWithOrWithoutBlocks>(blobIdWithOrWithoutBlocks, nameof (blobIdWithOrWithoutBlocks));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722840, nameof (AddPackageFromBlobAsync)))
      {
        tracer.TraceInfo(string.Format("Adding package from blob (feed : {0}, blobId : {1})", (object) id, (object) blobIdWithOrWithoutBlocks), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        NuGetPackageIngestionService.CheckIngestionPermission(requestContext, feedRequest, sourceChain);
        try
        {
          if (!await ingestionService.TryReferenceWithOrWithoutBlocksAsync(requestContext, blobIdWithOrWithoutBlocks, ingestionService.GetTemporaryReference()))
            throw Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageContentBlobNotFoundException.Create(blobIdWithOrWithoutBlocks.ToString());
          Stream blobStreamAsync = await NuGetPackageIngestionService.GetBlobStreamAsync(requestContext, blobIdWithOrWithoutBlocks.BlobId);
          StoredNupkgItemInfo nupkgInfo = await ingestionService.PrepareAddPackageAsync(requestContext, feedRequest, blobStreamAsync, (IStorageId) new BlobStorageId(blobIdWithOrWithoutBlocks.BlobId), sourceChain, expectedIdentity, addAsDelisted);
          await ingestionService.AddPackageCoreAsync(requestContext, feedRequest, nupkgInfo, protocolVersion, new NuGetPackagingTelemetryBuilder().Build(requestContext));
        }
        catch (HttpSeekableStreamRequestException ex)
        {
          tracer.TraceException((Exception) ex);
          switch (ex.StatusCode)
          {
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
              throw new HttpException((int) ex.StatusCode, "Unable to read required data from source", (Exception) ex);
            default:
              throw;
          }
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    private static void CheckIngestionPermission(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      if (sourceChain != null && sourceChain.Any<UpstreamSourceInfo>())
        FeedSecurityHelper.CheckAddUpstreamPackagePermissions(requestContext, feedRequest.Feed);
      else
        FeedSecurityHelper.CheckAddPackagePermissions(requestContext, feedRequest.Feed);
    }

    public async Task AddPackageFromDropAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      string dropName,
      string protocolVersion)
    {
      NuGetPackageIngestionService ingestionService = this;
      if (ingestionService.IsFeatureReadOnly(requestContext))
        throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuGetServiceReadOnly());
      Guid id = feedRequest.Feed.Id;
      Microsoft.VisualStudio.Services.NuGet.Server.Constants.FeatureEnabledConstants.EnableNuGetLargePackages.Bootstrap(requestContext).RequireFeatureEnabled();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(dropName, nameof (dropName));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722850, nameof (AddPackageFromDropAsync)))
      {
        tracer.TraceInfo(string.Format("Adding package from drop (feed : {0}, drop : {1})", (object) id, (object) dropName), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        FeedSecurityHelper.CheckAddPackagePermissions(requestContext, feedRequest.Feed);
        try
        {
          StoredNupkgItemInfo nupkgInfo = await ingestionService.PrepareDropAddPackageAsync(requestContext, feedRequest, dropName);
          nupkgInfo.SourceChain = (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>();
          await ingestionService.AddPackageCoreAsync(requestContext, feedRequest, nupkgInfo, protocolVersion, new NuGetPackagingTelemetryBuilder().Build(requestContext));
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    internal static IBlobStore GetBlobStore(IVssRequestContext requestContext) => (IBlobStore) new TracingBlobStoreWrapper((IBlobStore) requestContext.GetService<IElevatedBlobStore>());

    internal static async Task<Stream> GetBlobStreamAsync(
      IVssRequestContext requestContext,
      BlobIdentifier blobId)
    {
      return await new GetSeekableBlobHttpStreamHandlerBootstrapper(requestContext).Bootstrap().Handle(blobId);
    }

    internal StoredNupkgItemInfo GetStoredNupkgInfo(
      IVssRequestContext requestContext,
      Guid feedId,
      IStorageId packageStorageId,
      Stream nupkgStream,
      bool addAsDelisted)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722870, nameof (GetStoredNupkgInfo)))
      {
        byte[] nuspecBytes = NuGetNuspecUtils.GetNuspecBytes(nupkgStream);
        XDocument nuspec = NuGetNuspecUtils.LoadXDocumentFromBytes(nuspecBytes);
        NuGetPackageMetadata getPackageMetadata = new NuGetPackageMetadata(nuspec);
        requestContext.Items["Packaging.PackageIdentity"] = (object) getPackageMetadata.Identity;
        tracer.TraceInfo(string.Format("Preparing to save nupkg (feedId : {0}, identity : {1}, blobId : {2})", (object) feedId, (object) getPackageMetadata.Identity, (object) packageStorageId.ValueString), Microsoft.VisualStudio.Services.Content.Server.Common.Offset.Info);
        return new StoredNupkgItemInfo()
        {
          NuspecBytes = nuspecBytes,
          Nuspec = nuspec,
          Metadata = getPackageMetadata,
          Identity = getPackageMetadata.Identity,
          PackageStorageId = packageStorageId,
          PackageSize = nupkgStream.Length,
          AddAsDelisted = addAsDelisted
        };
      }
    }

    internal async Task CheckForDuplicatePackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      StoredNupkgItemInfo nupkgInfo,
      bool isDirectPush)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722820, nameof (CheckForDuplicatePackageAsync)))
      {
        INuGetMetadataEntry stateFor = await NuGetServerUtils.GetStateFor(requestContext, feed, nupkgInfo.Identity.Name, nupkgInfo.Identity.Version);
        if (stateFor == null)
          return;
        if (stateFor.IsDeleted())
          throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageExistsAsDeletedException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_FeedAlreadyContainsPackageDeleted((object) nupkgInfo.Identity.Version, (object) nupkgInfo.Identity.Name));
        if (stateFor.PackageStorageId is UpstreamStorageId packageStorageId)
        {
          if (isDirectPush)
            throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPublishExistsOnUpstream((object) nupkgInfo.Identity, (object) packageStorageId.UpstreamContentSource.Name));
        }
        else
        {
          if (isDirectPush)
            throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackage((object) nupkgInfo.Identity));
          throw new PackageExistsIngestingFromUpstreamException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackage((object) nupkgInfo.Identity));
        }
      }
    }

    private void ValidatePackage(IVssRequestContext requestContext, StoredNupkgItemInfo nupkgInfo)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722810, nameof (ValidatePackage)))
      {
        VssNuGetPackageIdentity identity = nupkgInfo.Identity;
        NuGetPackageIngestionValidationUtils.ValidatePackageId(identity.Name.DisplayName);
        new ThrowIfInvalidValidator<string>((IConverter<string, Exception>) new PackageIngestionVersionToExceptionConverter(requestContext.GetFeatureFlagFacade())).Validate(identity.Version.DisplayVersion);
        new BlockedPackageIdentityToExceptionConverterBootstrapper(requestContext, BlockedIdentityContext.Upload).Bootstrap().AsThrowingValidator<IPackageIdentity>().Validate((IPackageIdentity) identity);
        int maxNuspecSize = IngestionUtils.GetMaxNuspecSize(requestContext);
        NuGetPackageIngestionValidationUtils.ValidateNuspecSize(nupkgInfo.NuspecBytes.Length, maxNuspecSize);
      }
    }

    private async Task UploadBlobAndReferenceAsync(
      IVssRequestContext requestContext,
      Stream nupkgStream,
      IBlobStore blobStore,
      BlobIdentifier blobId,
      BlobReference blobReference)
    {
      using (requestContext.CreateTimeToFirstPageExclusionBlock())
      {
        using (ReadProgressStream progressStream = new ReadProgressStream(nupkgStream, true, (IStopwatch) new RealStopwatch(), (ITimeProvider) new DefaultTimeProvider(), (long) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Packaging/PushProgressReportIntervalBytes", true, 10000000)))
        {
          try
          {
            await blobStore.PutBlobAndReferenceAsync(requestContext, WellKnownDomainIds.OriginalDomainId, blobId, (Stream) progressStream, blobReference);
          }
          finally
          {
            try
            {
              progressStream.RecordProgressEntry();
              TransferRateResults results = progressStream.GetTransferRateResults();
              requestContext.TraceConditionally(5720930, TraceLevel.Info, NuGetTracePoints.V2PushController.TraceData.Area, NuGetTracePoints.V2PushController.TraceData.Layer, (Func<string>) (() => results.Serialize<TransferRateResults>()));
              requestContext.AddPackagingTracesProperty("pkgsToBlobstoreUpload", (object) results.WithoutEntries());
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }

    private async Task<StoredNupkgItemInfo> PrepareAddPackageAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      Stream nupkgStream,
      IStorageId packageContent,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      VssNuGetPackageIdentity expectedIdentity,
      bool addAsDelisted)
    {
      Guid id = feedRequest.Feed.Id;
      ZipUtils.ValidateZipEndOfCentralDirectoryIsInRange(nupkgStream);
      StoredNupkgItemInfo nupkgInfo = this.GetStoredNupkgInfo(requestContext, feedRequest.Feed.Id, packageContent, nupkgStream, addAsDelisted);
      this.ValidatePackage(requestContext, nupkgInfo);
      if (expectedIdentity != null && !nupkgInfo.Identity.Equals(expectedIdentity))
        throw new UpstreamUnexpectedPackageDataException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_IngestIncorrectPackageIdentity((object) expectedIdentity, (object) nupkgInfo.Identity));
      bool isDirectPush = !sourceChain.Any<UpstreamSourceInfo>();
      await this.CheckForDuplicatePackageAsync(requestContext, feedRequest.Feed, nupkgInfo, isDirectPush);
      nupkgInfo.SourceChain = sourceChain;
      StoredNupkgItemInfo storedNupkgItemInfo = nupkgInfo;
      storedNupkgItemInfo.Provenance = await new ProvenanceInfoProviderBootstrapper(requestContext).Bootstrap().GetProvenanceInfoAsync();
      storedNupkgItemInfo = (StoredNupkgItemInfo) null;
      await new TerrapinIngestionValidatorBootstrapper(requestContext, (ICommitLogWriter<ICommitLogEntry>) new NuGetCommitLogFacadeBootstrapper(requestContext).Bootstrap(), new NuGetChangeProcessingJobQueuerBootstrapper(requestContext).Bootstrap(), (IIdentityResolver) NuGetIdentityResolver.Instance).Bootstrap().ValidateAsync((IPackageRequest) feedRequest.WithPackage<VssNuGetPackageIdentity>(nupkgInfo.Identity), (IPackageFileName) new SimplePackageFileName(nupkgInfo.Identity.ToNupkgFilePath()), sourceChain);
      StoredNupkgItemInfo storedNupkgItemInfo1 = nupkgInfo;
      nupkgInfo = (StoredNupkgItemInfo) null;
      return storedNupkgItemInfo1;
    }

    private async Task<StoredNupkgItemInfo> PrepareDropAddPackageAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      string dropName)
    {
      DropHttpClient dropHttpClient = requestContext.GetClient<DropHttpClient>();
      DropItem dropAsync = await dropHttpClient.GetDropAsync(dropName, requestContext.CancellationToken);
      if (dropAsync.FinalizedStatus != 2)
      {
        dropAsync.FinalizedStatus = (DropCommitStatus) 1;
        if (!(await dropHttpClient.UpdateDropAsync(dropAsync, requestContext.CancellationToken, false, false)).IsSuccessStatusCode)
          throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.InvalidPackageException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_FailedToFinalizeDrop());
      }
      byte[] nuspecBytesFromDrop = await NuGetPackageIngestionService.GetNuspecBytesFromDrop(requestContext, dropName);
      XDocument nuspec = NuGetNuspecUtils.LoadXDocumentFromBytes(nuspecBytesFromDrop);
      NuGetPackageMetadata getPackageMetadata = new NuGetPackageMetadata(nuspec);
      requestContext.Items["Packaging.PackageIdentity"] = (object) getPackageMetadata.Identity;
      StoredNupkgItemInfo nupkgInfo = new StoredNupkgItemInfo()
      {
        NuspecBytes = nuspecBytesFromDrop,
        Nuspec = nuspec,
        Metadata = getPackageMetadata,
        Identity = getPackageMetadata.Identity,
        PackageStorageId = (IStorageId) new DropStorageId(dropName),
        PackageSize = 0
      };
      this.ValidatePackage(requestContext, nupkgInfo);
      await this.CheckForDuplicatePackageAsync(requestContext, feedRequest.Feed, nupkgInfo, true);
      StoredNupkgItemInfo storedNupkgItemInfo = nupkgInfo;
      dropHttpClient = (DropHttpClient) null;
      nupkgInfo = (StoredNupkgItemInfo) null;
      return storedNupkgItemInfo;
    }

    private static async Task<byte[]> GetNuspecBytesFromDrop(
      IVssRequestContext requestContext,
      string dropName)
    {
      BlobIdentifier blobId = await new GetNuspecBlobIdFromDropHandler((IDropHttpClient) new DropClientFacade(requestContext)).Handle(dropName);
      if (blobId == (BlobIdentifier) null)
        throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.InvalidPackageException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuspecNotFound());
      return await new GetBytesFromBlobIdHandler(new ContentBlobStoreFacadeBootstrapper(requestContext).Bootstrap()).Handle(blobId);
    }

    private async Task AddPackageCoreAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      StoredNupkgItemInfo nupkgInfo,
      string protocolVersion,
      ITelemetryPublisher telemetryPublisher)
    {
      Guid feedId = feedRequest.Feed.Id;
      await this.EnsureNotTooManyVersions(requestContext, feedRequest.Feed, (IPackageName) nupkgInfo.Identity.Name);
      if (nupkgInfo.PackageStorageId is BlobStorageId blobStorageId)
      {
        string packageBlobReferenceId = new NuGetServerUtils().GetPackageBlobReferenceId(feedId, nupkgInfo.Metadata.Identity);
        IBlobStore blobStore = NuGetPackageIngestionService.GetBlobStore(requestContext);
        BlobIdentifier blobId1 = blobStorageId.BlobId;
        IVssRequestContext requestContext1 = requestContext;
        IDomainId originalDomainId = WellKnownDomainIds.OriginalDomainId;
        BlobIdentifier blobId2 = blobId1;
        BlobReference reference = new BlobReference(packageBlobReferenceId, "nuget");
        if (!await blobStore.TryReferenceAsync(requestContext1, originalDomainId, blobId2, reference))
          throw new FailedToAddReferenceToPackageContentException(blobStorageId, (IPackageIdentity) nupkgInfo.Identity);
      }
      await PackagingUtils.CreateContainerIfNotExistsAsync<NuGetItemStore>(requestContext, feedRequest.Feed);
      await this.AddPackageToNewStoreAsync(requestContext, feedRequest, nupkgInfo);
      IVssRequestContext requestContext2 = requestContext;
      string protocolVersion1 = protocolVersion;
      FeedCore feed = feedRequest.Feed;
      VssNuGetPackageIdentity identity = nupkgInfo.Identity;
      long packageSize = nupkgInfo.PackageSize;
      IEnumerable<UpstreamSourceInfo> sourceChain = nupkgInfo.SourceChain;
      int num = sourceChain != null ? (sourceChain.Any<UpstreamSourceInfo>() ? 1 : 0) : 0;
      string telemetryStorageType = nupkgInfo.PackageStorageId.ToTelemetryStorageType();
      telemetryPublisher.Publish(requestContext, (ICiData) NuGetCiDataFactory.GetNuGetPushPackageCiData(requestContext2, protocolVersion1, feed, identity, packageSize, num != 0, telemetryStorageType));
      blobStorageId = (BlobStorageId) null;
    }

    private async Task EnsureNotTooManyVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName)
    {
      if (await PackageIngestionHelper.HasTooManyVersions(requestContext, feed, (IProtocol) Protocol.NuGet, packageName.NormalizedName))
        throw new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.TooManyVersionsException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageTooManyVersions((object) packageName.DisplayName, (object) PackageIngestionHelper.GetMaxVersionsForFeedAndProtocol(requestContext, feed, (IProtocol) Protocol.NuGet)));
    }

    private async Task AddPackageToCodeOnlyStoreAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      CommitLogEntry commitLogEntry)
    {
      NuGetPackageIngestionService sendInTheThisObject = this;
      Guid feedId = feedRequest.Feed.Id;
      using (ITracerBlock tracer = requestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (AddPackageToCodeOnlyStoreAsync)))
      {
        try
        {
          IReadOnlyList<IAggregationAccessor> accessorsFor = await new WriteAggregationAccessorFactoryBootstrapper(requestContext, new NuGetMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(requestContext).Bootstrap()).Bootstrap().GetAccessorsFor(feedRequest);
          tracer.TraceInfo(string.Format("updating metadata from {0} for feed: {1} to resolved metadata accessors: {2}", (object) commitLogEntry.CommitId, (object) feedId, (object) string.Join<IAggregationAccessor>(",", (IEnumerable<IAggregationAccessor>) accessorsFor)));
          AggregationApplyTimings aggregationApplyTimings = await new DependencyResolvingAggregationCommitApplierBootstrapper(requestContext, true).Bootstrap().ApplyCommitAsync(accessorsFor, feedRequest, (IReadOnlyList<ICommitLogEntry>) new CommitLogEntry[1]
          {
            commitLogEntry
          });
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          requestContext.AddPackagingTracesProperty("Packaging.Properties.CommitDidNotApply", (object) 1);
        }
        finally
        {
          await FeedJobQueuer.TryQueueUserInitiatedFeedJob(requestContext, FeedJobDefinitionFactory.CreateJobDefinition(requestContext, sendInTheThisObject.GetChangeProcessingFeedJobDefinitionsProvider(requestContext), feedRequest));
        }
      }
    }

    private BlobReference GetTemporaryReference() => new BlobReference(this.timeProvider.Now + NuGetPackageIngestionService.TemporaryReferenceLifespan);

    private Task<bool> TryReferenceWithOrWithoutBlocksAsync(
      IVssRequestContext requestContext,
      BlobIdentifierWithOrWithoutBlocks blobIdWithOrWithoutBlocks,
      BlobReference temporaryRef)
    {
      IBlobStore blobStore = NuGetPackageIngestionService.GetBlobStore(requestContext);
      return !(blobIdWithOrWithoutBlocks.BlobIdWithBlocks != (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null) ? blobStore.TryReferenceAsync(requestContext, WellKnownDomainIds.OriginalDomainId, blobIdWithOrWithoutBlocks.BlobId, temporaryRef) : blobStore.TryReferenceWithBlocksAsync(requestContext, WellKnownDomainIds.OriginalDomainId, blobIdWithOrWithoutBlocks.BlobIdWithBlocks, temporaryRef);
    }

    private async Task AddPackageToNewStoreAsync(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      StoredNupkgItemInfo nupkgItemInfo)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NuGetTracePoints.PackageIngestionService.TraceData, 5722800, nameof (AddPackageToNewStoreAsync)))
      {
        INuGetCommitLogService service = requestContext.GetService<INuGetCommitLogService>();
        NuGetAddOperationData addOperationData = new NuGetAddOperationData(nupkgItemInfo.Identity, nupkgItemInfo.PackageStorageId, nupkgItemInfo.PackageSize, nupkgItemInfo.NuspecBytes, nupkgItemInfo.SourceChain, nupkgItemInfo.Provenance, nupkgItemInfo.AddAsDelisted);
        IVssRequestContext requestContext1 = requestContext;
        FeedCore feed = feedRequest.Feed;
        NuGetAddOperationData operationData = addOperationData;
        CommitLogEntry commitLogEntry = await service.AppendEntryAsync(requestContext1, feed, (ICommitOperationData) operationData);
        await this.AddPackageToCodeOnlyStoreAsync(requestContext, feedRequest, commitLogEntry);
      }
    }

    protected virtual IFeedJobDefinitionProvider GetChangeProcessingFeedJobDefinitionsProvider(
      IVssRequestContext requestContext)
    {
      return (IFeedJobDefinitionProvider) new ChangeProcessingFeedJobDefinitionProvider(new NuGetCommitLogFacadeBootstrapper(requestContext).Bootstrap(), "Microsoft.VisualStudio.Services.NuGet.Server.Plugins.ChangeProcessing.NuGetFeedChangeProcessingJob");
    }
  }
}
