// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.PackageIngester2`5
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed> : 
    IPackageIngester2<TInput>
    where TPackageIdentity : class, IPackageIdentity
    where TPackageFileName : class, IPackageFileName
    where TMetadataEntry : class, IMetadataEntry<TPackageIdentity>
    where TParsed : IParsedPackageIngestionParams<TPackageIdentity, TPackageFileName>
  {
    private readonly IFeedPerms feedPerms;
    private readonly IValidator<FeedCore> feedWritableValidator;
    private readonly IFrotocolLevelPackagingSetting<int> maxNonDeletedVersionsSetting;
    private readonly IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> metadataDocumentService;
    private readonly IPackagingTracesBasicInfo packagingTracesBasicInfo;
    private readonly IProtocolPackageIngestion<TInput, TParsed, TMetadataEntry> protocolPackageIngestion;
    private readonly IValidator<IProtocol> protocolWritableValidator;
    private readonly IProvenanceInfoProvider provenanceInfoProvider;
    private readonly IValidator<IPackageRequest> blockedPackageValidator;
    private readonly ITerrapinIngestionValidator terrapinValidator;

    public PackageIngester2(
      IValidator<FeedCore> feedWritableValidator,
      IValidator<IProtocol> protocolWritableValidator,
      IFeedPerms feedPerms,
      IProtocolPackageIngestion<TInput, TParsed, TMetadataEntry> protocolPackageIngestion,
      ITerrapinIngestionValidator terrapinValidator,
      IReadMetadataDocumentService<TPackageIdentity, TMetadataEntry> metadataDocumentService,
      IFrotocolLevelPackagingSetting<int> maxNonDeletedVersionsSetting,
      IPackagingTracesBasicInfo packagingTracesBasicInfo,
      IProvenanceInfoProvider provenanceInfoProvider,
      IValidator<IPackageRequest> blockedPackageValidator)
    {
      this.feedWritableValidator = feedWritableValidator;
      this.protocolWritableValidator = protocolWritableValidator;
      this.feedPerms = feedPerms;
      this.protocolPackageIngestion = protocolPackageIngestion;
      this.terrapinValidator = terrapinValidator;
      this.metadataDocumentService = metadataDocumentService;
      this.maxNonDeletedVersionsSetting = maxNonDeletedVersionsSetting;
      this.packagingTracesBasicInfo = packagingTracesBasicInfo;
      this.provenanceInfoProvider = provenanceInfoProvider;
      this.blockedPackageValidator = blockedPackageValidator;
    }

    public async Task<IAddOperationData> IngestPackageAsync(
      IFeedRequest feedRequest,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      IPackageIdentity? expectedIdentity,
      TInput input)
    {
      ArgumentUtility.CheckForNull<IFeedRequest>(feedRequest, nameof (feedRequest));
      ArgumentUtility.CheckForNull<IEnumerable<UpstreamSourceInfo>>(sourceChain, nameof (sourceChain));
      ImmutableList<UpstreamSourceInfo> sourceChainList = sourceChain.ToImmutableList<UpstreamSourceInfo>();
      IngestionDirection ingestionDirection = PackageIngestionUtils.GetIngestionDirection((IEnumerable<UpstreamSourceInfo>) sourceChainList);
      this.packagingTracesBasicInfo.SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      this.feedWritableValidator.Validate(feedRequest.Feed);
      this.protocolWritableValidator.Validate(feedRequest.Protocol);
      FeedPermissionConstants packagePermission = PackageIngestionUtils.GetRequiredAddPackagePermission(ingestionDirection);
      this.feedPerms.Validate(feedRequest.Feed, packagePermission);
      await this.protocolPackageIngestion.CheckInputContentSize(feedRequest, input, ingestionDirection);
      TParsed parsed = await this.protocolPackageIngestion.Parse(input, ingestionDirection);
      PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.EnsurePackageHasCorrectIdentity(expectedIdentity, parsed.PackageIdentity, ingestionDirection);
      IPackageNameRequest<IPackageName> packageNameRequest = feedRequest.WithPackageName<IPackageName>(parsed.PackageIdentity.Name);
      IPackageRequest<TPackageIdentity> packageRequest = feedRequest.WithPackage<TPackageIdentity>(parsed.PackageIdentity);
      this.blockedPackageValidator.Validate((IPackageRequest) packageRequest);
      this.packagingTracesBasicInfo.SetFromFeedRequest((IProtocolAgnosticFeedRequest) packageRequest);
      await this.terrapinValidator.ValidateAsync((IPackageRequest) packageRequest, (IPackageFileName) parsed.FileName, (IEnumerable<UpstreamSourceInfo>) sourceChainList);
      await this.protocolPackageIngestion.Validate(feedRequest, parsed);
      PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.EnsureIdentityPartsNotTooLong(parsed.PackageIdentity);
      MetadataDocument<TMetadataEntry> statesDocumentAsync = await this.metadataDocumentService.GetPackageVersionStatesDocumentAsync(new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) packageNameRequest));
      TMetadataEntry existingEntryForVersionBeingIngested = default (TMetadataEntry);
      if (statesDocumentAsync != null)
      {
        existingEntryForVersionBeingIngested = statesDocumentAsync.Entries.FirstOrDefault<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => PackageIdentityComparer.NormalizedNameAndVersion.Equals((IPackageIdentity) x.PackageIdentity, (IPackageIdentity) parsed.PackageIdentity)));
        this.EnsureNotTooManyVersions(feedRequest, parsed.PackageIdentity, statesDocumentAsync);
        PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.EnsureNotDuplicateUpload(feedRequest, parsed, existingEntryForVersionBeingIngested, ingestionDirection);
        PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.EnsureNotMixingUpstreams(ingestionDirection, existingEntryForVersionBeingIngested, sourceChainList.FirstOrDefault<UpstreamSourceInfo>(), (IPackageIdentity) parsed.PackageIdentity);
      }
      await this.protocolPackageIngestion.Validate(feedRequest, parsed, statesDocumentAsync, existingEntryForVersionBeingIngested);
      ProvenanceInfo provenanceInfoAsync = await this.provenanceInfoProvider.GetProvenanceInfoAsync();
      IAddOperationData addOperationData = this.protocolPackageIngestion.PrepareAddOperation(parsed, provenanceInfoAsync, (IEnumerable<UpstreamSourceInfo>) sourceChainList);
      addOperationData.ConvertToIndexEntry((ICommitLogEntry) new CommitLogEntry((ICommitOperationData) addOperationData, PackagingCommitId.Empty, PackagingCommitId.Empty, PackagingCommitId.Empty, 0L, DateTime.MinValue, DateTime.MinValue, Guid.Empty), feedRequest.Feed);
      await this.protocolPackageIngestion.CommitStorage(feedRequest, parsed);
      IAddOperationData addOperationData1 = addOperationData;
      sourceChainList = (ImmutableList<UpstreamSourceInfo>) null;
      packageNameRequest = (IPackageNameRequest<IPackageName>) null;
      addOperationData = (IAddOperationData) null;
      return addOperationData1;
    }

    private static void EnsureIdentityPartsNotTooLong(TPackageIdentity packageIdentity)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Check(packageIdentity.Name.NormalizedName, (int) byte.MaxValue, PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C0\u003E__Error_NormalizedNameTooLong ?? (PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C0\u003E__Error_NormalizedNameTooLong = new Func<object, object, string>(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NormalizedNameTooLong)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Check(packageIdentity.Name.DisplayName, (int) byte.MaxValue, PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C1\u003E__Error_DisplayNameTooLong ?? (PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C1\u003E__Error_DisplayNameTooLong = new Func<object, object, string>(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_DisplayNameTooLong)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Check(packageIdentity.Version.NormalizedVersion, (int) sbyte.MaxValue, PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C2\u003E__Error_NormalizedVersionTooLong ?? (PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C2\u003E__Error_NormalizedVersionTooLong = new Func<object, object, string>(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NormalizedVersionTooLong)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Check(packageIdentity.Version.DisplayVersion, (int) sbyte.MaxValue, PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C3\u003E__Error_DisplayVersionTooLong ?? (PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003EO.\u003C3\u003E__Error_DisplayVersionTooLong = new Func<object, object, string>(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_DisplayVersionTooLong)));

      static void Check(
        string nameOrVersion,
        int maxLength,
        Func<object, object, string> messageFormatter)
      {
        if (nameOrVersion.Length > maxLength)
          throw new InvalidPackageException(messageFormatter((object) nameOrVersion, (object) maxLength));
      }
    }

    private static void EnsureNotMixingUpstreams(
      IngestionDirection ingestionDirection,
      TMetadataEntry? existingEntryForVersionBeingIngested,
      UpstreamSourceInfo? incomingDirectUpstream,
      IPackageIdentity packageIdentity)
    {
      // ISSUE: variable of a compiler-generated type
      PackageIngester2<TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003Ec__DisplayClass13_0 cDisplayClass130;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass130.packageIdentity = packageIdentity;
      if (ingestionDirection == IngestionDirection.DirectPush)
        return;
      if (incomingDirectUpstream == null)
        throw new Exception("ingestionDirection != DirectPush, but incomingDirectUpstream is null");
      if ((object) existingEntryForVersionBeingIngested == null)
        return;
      IEnumerable<UpstreamSourceInfo> sourceChain = existingEntryForVersionBeingIngested.SourceChain;
      UpstreamSourceInfo expectedUpstream = sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
      if (expectedUpstream != null && expectedUpstream.Id != incomingDirectUpstream.Id)
        throw GetCannotMixFilesException(expectedUpstream, incomingDirectUpstream, ref cDisplayClass130);
      foreach (IPackageFile packageFile in (IEnumerable<IPackageFile>) existingEntryForVersionBeingIngested.PackageFiles)
      {
        if (packageFile.StorageId is UpstreamStorageId storageId && storageId.UpstreamContentSource.Id != incomingDirectUpstream.Id)
          throw GetCannotMixFilesException(storageId.UpstreamContentSource, incomingDirectUpstream, ref cDisplayClass130);
      }

      static Exception GetCannotMixFilesException(
        UpstreamSourceInfo expectedUpstream,
        UpstreamSourceInfo actualUpstream,
        ref PackageIngester2<
        #nullable disable
        TPackageIdentity, TPackageFileName, TMetadataEntry, TInput, TParsed>.\u003C\u003Ec__DisplayClass13_0 _param2)
      {
        // ISSUE: reference to a compiler-generated field
        return (Exception) new CannotMixFilesFromDifferentUpstreamsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotMixFilesFromDifferentUpstreams((object) _param2.packageIdentity.DisplayStringForMessages, (object) actualUpstream.Name, (object) expectedUpstream.Name));
      }
    }

    private static void EnsureNotDuplicateUpload(

      #nullable enable
      IFeedRequest feedRequest,
      TParsed parsed,
      TMetadataEntry? existingEntryForVersionBeingIngested,
      IngestionDirection ingestionDirection)
    {
      if ((object) parsed.FileName == null)
        throw new InvalidOperationException("Protocols must provide a file path in the parsed ingestion information, even if single-file");
      if ((object) existingEntryForVersionBeingIngested == null)
        return;
      if (existingEntryForVersionBeingIngested.IsDeleted())
        throw new PackageExistsAsDeletedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageDeleted((object) parsed.PackageIdentity.Version, (object) parsed.PackageIdentity.Name));
      IPackageFile packageFileWithPath = existingEntryForVersionBeingIngested.GetPackageFileWithPath(parsed.FileName.Path);
      if (packageFileWithPath == null)
      {
        if (!feedRequest.Protocol.IsMultiFile)
        {
          string message = string.Format("Expected metadata entry object for single-file protocol {0} to contain dummy file {1}, but it did not. This indicates a bug.", (object) feedRequest.Protocol, (object) parsed.FileName.Path);
          if (existingEntryForVersionBeingIngested is MetadataEntryWithSingleFile<TPackageIdentity> entryWithSingleFile)
            message = message + " Expected path for single file according to metadata entry is " + entryWithSingleFile.SingleFilePath + ".";
          throw new InvalidOperationException(message);
        }
      }
      else
      {
        if (packageFileWithPath.StorageId == null || packageFileWithPath.StorageId.IsLocal)
        {
          string message = feedRequest.Protocol.IsMultiFile ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackageFile((object) feedRequest.Feed.FullyQualifiedName, (object) parsed.FileName.Path, (object) parsed.PackageIdentity.DisplayStringForMessages) : Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FeedAlreadyContainsPackage((object) parsed.PackageIdentity.DisplayStringForMessages);
          throw ingestionDirection == IngestionDirection.PullFromUpstream ? (PackageAlreadyExistsException) new PackageExistsIngestingFromUpstreamException(message) : new PackageAlreadyExistsException(message);
        }
        if (ingestionDirection == IngestionDirection.DirectPush && existingEntryForVersionBeingIngested.IsFromUpstream)
        {
          UpstreamSourceInfo upstreamSourceInfo = existingEntryForVersionBeingIngested.PackageFiles.Select<IPackageFile, UpstreamStorageId>((Func<IPackageFile, UpstreamStorageId>) (f => f.StorageId as UpstreamStorageId)).FirstOrDefault<UpstreamStorageId>((Func<UpstreamStorageId, bool>) (x => x != null))?.UpstreamContentSource ?? existingEntryForVersionBeingIngested.SourceChain.FirstOrDefault<UpstreamSourceInfo>();
          throw new PackageAlreadyExistsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotPublishExistsOnUpstream((object) parsed.PackageIdentity.DisplayStringForMessages, (object) upstreamSourceInfo?.Name));
        }
      }
    }

    private void EnsureNotTooManyVersions(
      IFeedRequest feedRequest,
      TPackageIdentity packageIdentity,
      MetadataDocument<TMetadataEntry> metadataDoc)
    {
      PackageVersionComparer versionComparer = PackageVersionComparer.NormalizedVersion;
      int num1 = metadataDoc.Entries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (x => x.IsLocal && !x.IsDeleted() && !versionComparer.Equals(packageIdentity.Version, x.PackageIdentity.Version))) + 1;
      int num2 = this.maxNonDeletedVersionsSetting.Get(feedRequest);
      int num3 = num2;
      if (num1 > num3)
        throw new TooManyVersionsException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageTooManyVersions((object) packageIdentity.Name.DisplayName, (object) num2));
    }

    private static void EnsurePackageHasCorrectIdentity(
      IPackageIdentity? expectedIdentity,
      TPackageIdentity actualIdentity,
      IngestionDirection ingestionDirection)
    {
      if (expectedIdentity != null && !actualIdentity.Equals((object) expectedIdentity))
        throw new UpstreamUnexpectedPackageDataException(ingestionDirection == IngestionDirection.PullFromUpstream ? Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_IncorrectPackageIdentityUpstream((object) expectedIdentity.DisplayStringForMessages, (object) actualIdentity.DisplayStringForMessages) : Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_IncorrectPackageIdentityDirectPush((object) expectedIdentity.DisplayStringForMessages, (object) actualIdentity.DisplayStringForMessages));
    }
  }
}
