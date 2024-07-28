// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion.NpmPackageIngestionService
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Npm.Server.ItemStore;
using Microsoft.VisualStudio.Services.Npm.Server.JobManagement;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.Migration;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.PackageExtraction;
using Microsoft.VisualStudio.Services.Npm.Server.Readme;
using Microsoft.VisualStudio.Services.Npm.Server.Telemetry;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion
{
  public class NpmPackageIngestionService : 
    PackageIngestionServiceBase,
    INpmPackageIngestionService,
    IVssFrameworkService
  {
    private IAsyncHandler<FeedRequest<Stream>, FileStream> temporarilyStorePackageHandler = (IAsyncHandler<FeedRequest<Stream>, FileStream>) new TemporarilyStoreStreamAsFileHandler();

    protected override string ProtocolReadOnlyFeatureFlagName => "Packaging.Npm.ReadOnly";

    protected virtual async Task<INpmMetadataEntry> GetStateFor(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackageIdentity identity)
    {
      return await new NpmMetadataHandlerBootstrapper(requestContext).Bootstrap().Handle((IPackageRequest<NpmPackageIdentity>) new PackageRequest<NpmPackageIdentity>(feed, identity));
    }

    public async Task AddPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata newPackageMetadata)
    {
      NpmPackageIngestionService ingestionService = this;
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NpmTracePoints.NpmPackageIngestionService.TraceData, 12000100, nameof (AddPackageAsync));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata>(newPackageMetadata, nameof (newPackageMetadata));
        if (feed.IsReadOnly)
          throw new ReadOnlyFeedOperationException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_FeedIsReadOnly());
        if (ingestionService.IsFeatureReadOnly(requestContext))
          throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_NpmServiceReadOnly());
        FeedSecurityHelper.CheckAddPackagePermissions(requestContext, feed);
        if (newPackageMetadata.Attachments != null && newPackageMetadata.Attachments.Count == 1)
        {
          KeyValuePair<string, Attachment> keyValuePair = newPackageMetadata.Attachments.First<KeyValuePair<string, Attachment>>();
          if (keyValuePair.Value.Data != null)
          {
            IVssRequestContext requestContext1 = requestContext;
            keyValuePair = newPackageMetadata.Attachments.First<KeyValuePair<string, Attachment>>();
            long packageSize = keyValuePair.Value.Data.LongCount<char>();
            IngestionValidationUtils.ValidatePackageSize(requestContext1, packageSize);
            keyValuePair = newPackageMetadata.Attachments.First<KeyValuePair<string, Attachment>>();
            using (MemoryStream packageTarballStream = new MemoryStream(Convert.FromBase64String(keyValuePair.Value.Data)))
            {
              NpmPackageName npmPackageName = new NpmPackageName(newPackageMetadata.Name);
              IPackageExtractor packageExtractor = ingestionService.CreatePackageExtractor((Stream) packageTarballStream, npmPackageName, (SemanticVersion) null, (string) null, requestContext.GetTracerFacade());
              byte[] packageJsonBytes = packageExtractor.GetPackageJsonBytes();
              string readmeMdPath = packageExtractor.GetReadmeMdPath();
              byte[] readmeMdBytes = packageExtractor.GetReadmeMdBytes();
              PackageJson packageJson = packageExtractor.GetPackageJson();
              PackageJsonOptions packageJsonOptions1 = new PackageJsonOptions();
              packageJsonOptions1.ContainsServerJsFileAtRoot = packageExtractor.PackageHasServerJsFileAtRoot();
              packageJsonOptions1.ContainsBindingGypFileAtRoot = packageExtractor.PackageHasBindingGypFileAtRoot();
              IDictionary<string, VersionMetadata> versions = newPackageMetadata.Versions;
              packageJsonOptions1.AdditionalClientProvidedData = versions != null ? versions.GetValueOrDefault<string, VersionMetadata>(packageJson.Version)?.AdditionalData : (IDictionary<string, JToken>) null;
              PackageJsonOptions packageJsonOptions = packageJsonOptions1;
              IngestionValidationUtils.ValidatePackageJsonSize((long) packageJsonBytes.Length);
              NpmPackageName parsedName = IngestionValidationUtils.ParseAndValidatePackageName(packageJson.Name);
              if (!parsedName.Equals(npmPackageName))
                throw new InvalidPackageTarballException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageNamesDontMatch((object) newPackageMetadata.Name, (object) parsedName.FullName));
              SemanticVersion parsedVersion = IngestionValidationUtils.ParseAndValidatePackageVersion(packageJson.Version);
              requestContext.Items["Packaging.PackageIdentity"] = (object) new NpmPackageIdentity(parsedName, parsedVersion);
              string parsedVersionString = parsedVersion.ToString();
              await ingestionService.EnsureNotTooManyVersions(requestContext, feed, (IPackageName) parsedName);
              if (newPackageMetadata.Versions == null || !newPackageMetadata.Versions.ContainsKey(parsedVersionString))
                throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_RequestMustContainPackageJsonVersion());
              IngestionValidationUtils.ValidatePackageJsonAgainstRequest(packageJson, parsedVersionString, newPackageMetadata.Versions[parsedVersionString]);
              IDictionary<string, string> distributionTags = newPackageMetadata.DistributionTags;
              string key = distributionTags != null ? distributionTags.Single<KeyValuePair<string, string>>().Key : (string) null;
              ICommitLogEntry commitLogEntry = await ingestionService.StorePackage(requestContext, feed, parsedName, parsedVersion, (Stream) packageTarballStream, packageJsonBytes, key, packageJsonOptions, readmeMdPath, readmeMdBytes, Enumerable.Empty<UpstreamSourceInfo>(), (string) null);
              await ingestionService.TryAddPackageToDataViews(requestContext, feed, commitLogEntry, parsedName, parsedVersion);
              packageJsonBytes = (byte[]) null;
              readmeMdPath = (string) null;
              readmeMdBytes = (byte[]) null;
              packageJson = (PackageJson) null;
              packageJsonOptions = (PackageJsonOptions) null;
              parsedName = (NpmPackageName) null;
              parsedVersion = (SemanticVersion) null;
              parsedVersionString = (string) null;
            }
            goto label_24;
          }
        }
        throw new InvalidPublishException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackagePublishMustHaveOneAttachment());
      }
      finally
      {
        tracer?.Dispose();
      }
label_24:
      tracer = (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer) null;
    }

    public async Task SaveStreamToFeedAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      UpstreamPackageContent upstreamPackageContent,
      NpmPackageIdentity expectedIdentity,
      string deprecateMessage)
    {
      NpmPackageIngestionService ingestionService = this;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NpmTracePoints.NpmPackageIngestionService.TraceData, 12000130, nameof (SaveStreamToFeedAsync)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
        ArgumentUtility.CheckForNull<UpstreamPackageContent>(upstreamPackageContent, nameof (upstreamPackageContent));
        if (feed.IsReadOnly)
          throw new ReadOnlyFeedOperationException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_FeedIsReadOnly());
        if (ingestionService.IsFeatureReadOnly(requestContext))
          throw new FeatureReadOnlyException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_NpmServiceReadOnly());
        if (new ValueFromCacheFactory<bool>("Packaging.BlockWriteOperationOnGetRequest", (ICache<string, object>) new RequestContextItemsAsCacheFacade(requestContext)).Get())
          throw new PotentiallyDangerousRequestException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_UpstreamIngestion_CannotSkipIngestion());
        FeedSecurityHelper.CheckAddUpstreamPackagePermissions(requestContext, feed);
        upstreamPackageContent.TarballStream.Seek(0L, SeekOrigin.Begin);
        IPackageExtractor packageExtractor = ingestionService.CreatePackageExtractor(upstreamPackageContent.TarballStream, expectedIdentity.Name, expectedIdentity.Version, upstreamPackageContent.SourceChain.FirstOrDefault<UpstreamSourceInfo>()?.Location, requestContext.GetTracerFacade());
        byte[] packageJsonBytes = packageExtractor.GetPackageJsonBytes();
        string readmeMdPath = packageExtractor.GetReadmeMdPath();
        byte[] readmeMdBytes = packageExtractor.GetReadmeMdBytes();
        PackageJson packageJson = packageExtractor.GetPackageJson();
        PackageJsonOptions packageJsonOptions = new PackageJsonOptions()
        {
          ContainsServerJsFileAtRoot = packageExtractor.PackageHasServerJsFileAtRoot(),
          ContainsBindingGypFileAtRoot = packageExtractor.PackageHasBindingGypFileAtRoot()
        };
        NpmPackageName parsedName = IngestionValidationUtils.ParseAndValidatePackageName(packageJson.Name);
        if (!parsedName.Equals(expectedIdentity.Name))
          throw new UpstreamUnexpectedPackageDataException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageNamesDontMatch((object) expectedIdentity.Name.FullName, (object) parsedName.FullName));
        SemanticVersion parsedVersion = IngestionValidationUtils.ParseAndValidatePackageVersion(packageJson.Version);
        if (!parsedVersion.Equals(expectedIdentity.Version))
          throw new UpstreamUnexpectedPackageDataException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageVersionsDontMatch((object) expectedIdentity.Version.DisplayVersion, (object) parsedVersion.DisplayVersion));
        ICommitLogEntry commitLogEntry = await ingestionService.StorePackage(requestContext, feed, parsedName, parsedVersion, upstreamPackageContent.TarballStream, packageJsonBytes, (string) null, packageJsonOptions, readmeMdPath, readmeMdBytes, upstreamPackageContent.SourceChain, deprecateMessage);
        await ingestionService.TryAddPackageToDataViews(requestContext, feed, commitLogEntry, parsedName, parsedVersion);
        parsedName = (NpmPackageName) null;
        parsedVersion = (SemanticVersion) null;
      }
    }

    protected virtual IPackageExtractor CreatePackageExtractor(
      Stream packageTarball,
      NpmPackageName expectedName,
      SemanticVersion expectedVersion,
      string upstream,
      ITracerService tracerService)
    {
      return (IPackageExtractor) new PackageExtractor(packageTarball, tracerService, expectedName?.FullName, expectedVersion?.DisplayVersion, upstream, nameof (NpmPackageIngestionService));
    }

    private async Task<ICommitLogEntry> StorePackage(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      Stream packageTarballStream,
      byte[] packageJsonBytes,
      string distTag,
      PackageJsonOptions packageJsonOptions,
      string readmeMdPath,
      byte[] readmeMdBytes,
      IEnumerable<UpstreamSourceInfo> upstreamSourceHistory,
      string deprecateMessage)
    {
      IEnumerable<UpstreamSourceInfo> items = upstreamSourceHistory;
      ImmutableArray<UpstreamSourceInfo> upstreamSourceChain = items != null ? items.ToImmutableArray<UpstreamSourceInfo>() : ImmutableArray<UpstreamSourceInfo>.Empty;
      IValidator<IPackageIdentity> validator = new BlockedPackageIdentityToExceptionConverterBootstrapper(requestContext, BlockedIdentityContext.Upload).Bootstrap().AsThrowingValidator<IPackageIdentity>();
      NpmPackageIdentity parsedIdentity = new NpmPackageIdentity(packageName, packageVersion);
      NpmPackageIdentity valueToValidate = parsedIdentity;
      validator.Validate((IPackageIdentity) valueToValidate);
      INpmMetadataEntry stateFor = await this.GetStateFor(requestContext, feed, new NpmPackageIdentity(packageName, packageVersion));
      this.CheckForDuplicatePackageAsync(requestContext, feed, stateFor, (ICollection<UpstreamSourceInfo>) upstreamSourceChain);
      await PackagingUtils.CreateContainerIfNotExistsAsync<NpmItemStore>(requestContext, feed);
      IContentBlobStore blobStore = this.GetContentBlobStore(requestContext);
      IdBlobReference blobReference = NpmBlobUtils.GetPackageBlobReference(feed.Id, packageName, packageVersion);
      BlobIdentifier packageTarballBlobId = packageTarballStream.CalculateBlobIdentifier((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance);
      ICommitLog commitLog = new NpmCommitLogFacadeBootstrapper(requestContext).Bootstrap();
      await new TerrapinIngestionValidatorBootstrapper(requestContext, (ICommitLogWriter<ICommitLogEntry>) commitLog, new NpmChangeProcessingJobQueuerBootstrapper(requestContext).Bootstrap(), (IIdentityResolver) NpmIdentityResolver.Instance).Bootstrap().ValidateAsync((IPackageRequest) new FeedRequest(feed, (IProtocol) Protocol.npm).WithPackage<NpmPackageIdentity>(parsedIdentity), (IPackageFileName) new SimplePackageFileName(parsedIdentity.ToTgzFilePath()), (IEnumerable<UpstreamSourceInfo>) upstreamSourceChain);
      await blobStore.PutBlobAndReferenceAsync(packageTarballBlobId, packageTarballStream, new BlobReference(blobReference));
      PackageManifest packageManifest = (PackageManifest) null;
      if (readmeMdBytes != null)
      {
        PackageFileMetadata packageFileMetadata = await new NpmReadmeUtils().StoreReadme(requestContext, feed.Id, packageName, packageVersion, readmeMdPath, readmeMdBytes);
        packageManifest = new PackageManifest()
        {
          FilesMetadata = new Dictionary<string, PackageFileMetadata>()
          {
            {
              readmeMdPath,
              packageFileMetadata
            }
          }
        };
      }
      packageTarballStream.Position = 0L;
      SHA1Cng shA1Cng = new SHA1Cng();
      shA1Cng.Initialize();
      string hashHex = HexConverter.ToStringLowerCase(shA1Cng.ComputeHash(packageTarballStream));
      requestContext.GetService<INpmCommitLogService>();
      long streamLength = packageTarballStream.Length;
      PackagingCiData pushPackageCiData = (PackagingCiData) NpmCiDataFactory.GetNpmPushPackageCiData(requestContext, feed, packageName.FullName, packageVersion.DisplayVersion, streamLength, upstreamSourceChain.Any<UpstreamSourceInfo>());
      new NpmPackagingTelemetryBuilder().Build(requestContext).Publish(requestContext, (ICiData) pushPackageCiData);
      ICommitLogEntry commitLogEntry = await commitLog.AppendEntryAsync(feed, (ICommitOperationData) new NpmAddOperationData((IStorageId) new BlobStorageId(packageTarballBlobId), packageName, streamLength, packageJsonBytes, hashHex, distTag, false, packageJsonOptions, packageManifest, (IEnumerable<UpstreamSourceInfo>) upstreamSourceChain, await new ProvenanceInfoProviderBootstrapper(requestContext).Bootstrap().GetProvenanceInfoAsync(), (IEnumerable<Guid>) null, deprecateMessage));
      NullResult nullResult = await new GdprDataHandlerLazyBootstrapper<FeedRequest>(requestContext.GetExecutionEnvironmentFacade(), (IBootstrapper<IAsyncHandler<(FeedRequest, ICommitLogEntry), bool>>) new GdprDataWriterBootstrapper<FeedRequest>(requestContext)).Bootstrap().Handle((new FeedRequest(feed, (IProtocol) Protocol.npm), commitLogEntry));
      ICommitLogEntry commitLogEntry1 = commitLogEntry;
      upstreamSourceChain = new ImmutableArray<UpstreamSourceInfo>();
      parsedIdentity = (NpmPackageIdentity) null;
      blobStore = (IContentBlobStore) null;
      blobReference = new IdBlobReference();
      packageTarballBlobId = (BlobIdentifier) null;
      commitLog = (ICommitLog) null;
      packageManifest = (PackageManifest) null;
      hashHex = (string) null;
      commitLogEntry = (ICommitLogEntry) null;
      return commitLogEntry1;
    }

    internal void CheckForDuplicatePackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      INpmMetadataEntry entry,
      ICollection<UpstreamSourceInfo> sourceChain)
    {
      if (entry == null)
        return;
      IPackageFile packageFile = !entry.IsDeleted() ? entry.GetPackageFileWithPath(entry.PackageIdentity.ToTgzFilePath()) : throw new PackageExistsAsDeletedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageDeleted((object) entry.PackageIdentity.Version, (object) entry.PackageIdentity.Name));
      bool flag = sourceChain.IsNullOrEmpty<UpstreamSourceInfo>();
      if (packageFile != null && (packageFile.StorageId == null || packageFile.StorageId.IsLocal))
      {
        if (flag)
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageFile((object) feed.FullyQualifiedName, (object) packageFile.Path, (object) entry.PackageIdentity));
        throw new PackageExistsIngestingFromUpstreamException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageFile((object) feed.FullyQualifiedName, (object) packageFile.Path, (object) entry.PackageIdentity));
      }
      if (flag)
      {
        UpstreamSourceInfo upstreamSourceInfo1 = entry.PackageFiles.FirstOrDefault<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId is UpstreamStorageId))?.StorageId is UpstreamStorageId storageId ? storageId.UpstreamContentSource : (UpstreamSourceInfo) null;
        if (upstreamSourceInfo1 == null)
        {
          IEnumerable<UpstreamSourceInfo> sourceChain1 = entry.SourceChain;
          upstreamSourceInfo1 = sourceChain1 != null ? sourceChain1.FirstOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
        }
        UpstreamSourceInfo upstreamSourceInfo2 = upstreamSourceInfo1;
        if (upstreamSourceInfo2 != null)
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPublishExistsOnUpstream((object) entry.PackageIdentity, (object) upstreamSourceInfo2.Name));
      }
      else
      {
        Guid? expectedUpstream = packageFile?.StorageId is UpstreamStorageId storageId ? new Guid?(storageId.UpstreamContentSource.Id) : new Guid?();
        if (!expectedUpstream.HasValue && packageFile == null)
        {
          IEnumerable<UpstreamSourceInfo> sourceChain2 = entry.SourceChain;
          expectedUpstream = sourceChain2 != null ? sourceChain2.FirstOrDefault<UpstreamSourceInfo>()?.Id : new Guid?();
        }
        this.ThrowOnUnexpectedUpstreamException(new Guid?(sourceChain.Select<UpstreamSourceInfo, Guid>((Func<UpstreamSourceInfo, Guid>) (c => c.Id)).FirstOrDefault<Guid>()), expectedUpstream, (IPackageIdentity) entry.PackageIdentity);
      }
    }

    private void ThrowOnUnexpectedUpstreamException(
      Guid? actualUpstream,
      Guid? expectedUpstream,
      IPackageIdentity packageIdentity)
    {
      if (!actualUpstream.Equals((object) expectedUpstream))
        throw new CannotMixFilesFromDifferentUpstreamsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotMixFilesFromDifferentUpstreams((object) packageIdentity.DisplayStringForMessages, (object) actualUpstream, (object) expectedUpstream));
    }

    private IContentBlobStore GetContentBlobStore(IVssRequestContext requestContext) => new ContentBlobStoreFacadeBootstrapper(requestContext).Bootstrap();

    protected virtual async Task TryAddPackageToDataViews(
      IVssRequestContext requestContext,
      FeedCore feed,
      ICommitLogEntry commitLogEntry,
      NpmPackageName packageName,
      SemanticVersion packageVersion)
    {
      FeedRequest feedRequest = new FeedRequest(feed, (IProtocol) Protocol.npm);
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, NpmTracePoints.NpmPackageIngestionService.TraceData, 12000110, nameof (TryAddPackageToDataViews)))
      {
        try
        {
          IReadOnlyList<IAggregationAccessor> accessorsFor = await new WriteAggregationAccessorFactoryBootstrapper(requestContext, new NpmMigrationDefinitionsProviderBootstrapper(requestContext).Bootstrap(), new AggregationAccessorFactoryBootstrapper(requestContext).Bootstrap()).Bootstrap().GetAccessorsFor((IFeedRequest) feedRequest);
          tracer.TraceInfo(string.Format("updating metadata from {0} for feed: {1} to resolved metadata accessors: {2}", (object) commitLogEntry.CommitId, (object) feed.Id, (object) string.Join<IAggregationAccessor>(",", (IEnumerable<IAggregationAccessor>) accessorsFor)));
          AggregationApplyTimings aggregationApplyTimings = await new DependencyResolvingAggregationCommitApplierBootstrapper(requestContext, true).Bootstrap().ApplyCommitAsync(accessorsFor, (IFeedRequest) feedRequest, (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
          {
            commitLogEntry
          });
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          tracer.TraceInfo(string.Format("Failed to update the data stores after commit log write. Queueing the change processing job for the feed: {0}", (object) feed.Id));
          requestContext.AddPackagingTracesProperty("Packaging.Properties.CommitDidNotApply", (object) 1);
        }
        finally
        {
          await FeedJobQueuer.TryQueueUserInitiatedFeedJob(requestContext, FeedJobDefinitionFactory.CreateJobDefinition(requestContext, this.GetChangeProcessingFeedJobDefinitionsProvider(requestContext), (IFeedRequest) new FeedRequest(feed, (IProtocol) Protocol.npm)));
        }
      }
      feedRequest = (FeedRequest) null;
    }

    private async Task EnsureNotTooManyVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      IPackageName packageName)
    {
      if (await PackageIngestionHelper.HasTooManyVersions(requestContext, feed, (IProtocol) Protocol.npm, packageName.NormalizedName))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.TooManyVersionsException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageTooManyVersions((object) packageName.DisplayName, (object) PackageIngestionHelper.GetMaxVersionsForFeedAndProtocol(requestContext, feed, (IProtocol) Protocol.npm)));
    }

    protected virtual IFeedJobDefinitionProvider GetChangeProcessingFeedJobDefinitionsProvider(
      IVssRequestContext requestContext)
    {
      return (IFeedJobDefinitionProvider) new ChangeProcessingFeedJobDefinitionProvider(new NpmCommitLogFacadeBootstrapper(requestContext).Bootstrap(), "Microsoft.VisualStudio.Services.Npm.Server.Plugins.ChangeProcessing.NpmFeedChangeProcessingJob");
    }
  }
}
