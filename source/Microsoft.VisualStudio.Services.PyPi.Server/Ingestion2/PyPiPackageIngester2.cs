// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2.PyPiPackageIngester2
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using BuildXL.Cache.ContentStore.Interfaces.Utils;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion2
{
  public class PyPiPackageIngester2 : 
    IProtocolPackageIngestion<PackageIngestionFormData, PyPiParsedIngestionParams, IPyPiMetadataEntry>
  {
    private readonly IValidator<long> gpgSignatureSizeValidator;
    private readonly IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult> mainValidatingHandler;
    private readonly IAsyncHandler<(IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry)> onlyOneSourceDistValidatingHandler;
    private readonly IAsyncHandler<long> packageFileSizeValidator;
    private readonly IContentBlobStore blobStore;
    private readonly IConverter<IPackageFileRequest<PyPiPackageIdentity>, string> refCalculatingConverter;
    private readonly IHttpClient httpClient;
    private readonly IOrgLevelPackagingSetting<bool> suppressHashingInBlobFirstIngestFlowsSetting;
    private readonly IOrgLevelPackagingSetting<bool> trustClientProvidedHashInBlobFirstIngestFlowsSetting;

    public PyPiPackageIngester2(
      IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult> mainValidatingHandler,
      IAsyncHandler<(IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry)> onlyOneSourceDistValidatingHandler,
      IAsyncHandler<long> packageFileSizeValidator,
      IValidator<long> gpgSignatureSizeValidator,
      IContentBlobStore blobStore,
      IConverter<IPackageFileRequest<PyPiPackageIdentity>, string> refCalculatingConverter,
      IHttpClient httpClient,
      IOrgLevelPackagingSetting<bool> suppressHashingInBlobFirstIngestFlowsSetting,
      IOrgLevelPackagingSetting<bool> trustClientProvidedHashInBlobFirstIngestFlowsSetting)
    {
      this.mainValidatingHandler = mainValidatingHandler;
      this.onlyOneSourceDistValidatingHandler = onlyOneSourceDistValidatingHandler;
      this.packageFileSizeValidator = packageFileSizeValidator;
      this.gpgSignatureSizeValidator = gpgSignatureSizeValidator;
      this.blobStore = blobStore;
      this.refCalculatingConverter = refCalculatingConverter;
      this.httpClient = httpClient;
      this.suppressHashingInBlobFirstIngestFlowsSetting = suppressHashingInBlobFirstIngestFlowsSetting;
      this.trustClientProvidedHashInBlobFirstIngestFlowsSetting = trustClientProvidedHashInBlobFirstIngestFlowsSetting;
    }

    public async Task CheckInputContentSize(
      IFeedRequest feedRequest,
      PackageIngestionFormData input,
      IngestionDirection ingestionDirection)
    {
      if ((object) input.PackageFileStream.ExistingBlobIdentifier == null && ingestionDirection == IngestionDirection.DirectPush)
      {
        NullResult nullResult = await this.packageFileSizeValidator.Handle(input.PackageFileStream.Length);
      }
      if (!(input.GpgSignatureFileStream != (PackageFileStream) null))
        return;
      this.gpgSignatureSizeValidator.Validate(input.GpgSignatureFileStream.Length);
    }

    public async Task<PyPiParsedIngestionParams> Parse(
      PackageIngestionFormData input,
      IngestionDirection ingestionDirection)
    {
      IReadOnlyDictionary<string, string[]> metadataFields = input.RawMetadata;
      PyPiPackageIdentity packageIdentity = new PyPiPackageIdentity(new PyPiPackageName(((IEnumerable<string>) metadataFields["name"]).FirstOrDefault<string>()), PyPiPackageVersionParser.Parse(((IEnumerable<string>) metadataFields["version"]).FirstOrDefault<string>()));
      DeflateCompressibleBytes gpgSignature = input.GpgSignatureFileStream != (PackageFileStream) null ? DeflateCompressibleBytes.FromUncompressedBytes(input.GpgSignatureFileStream.Stream.ReadToByteArray()) : (DeflateCompressibleBytes) null;
      PackageFileStream packageFileStream1 = input.PackageFileStream;
      IEnumerable<HashAndType> hashes;
      BlobStorageId storageId;
      if (packageFileStream1.Stream == null && this.suppressHashingInBlobFirstIngestFlowsSetting.Get())
      {
        (hashes, storageId) = this.ExtractHashesFromInput(input);
      }
      else
      {
        Stream packageFileStream2 = packageFileStream1.Stream;
        if (packageFileStream2 == null)
          packageFileStream2 = await this.GetSequentialStreamFromBlob(packageFileStream1);
        (hashes, storageId) = await PyPiPackageIngester2.ComputeHashes(packageFileStream2);
      }
      PyPiParsedIngestionParams parsedIngestionParams = new PyPiParsedIngestionParams(packageIdentity, metadataFields, PyPiResolvedMetadata.ParseFrom(metadataFields), gpgSignature, input.PackageFileStream, hashes, (IStorageId) storageId, ingestionDirection);
      metadataFields = (IReadOnlyDictionary<string, string[]>) null;
      packageIdentity = (PyPiPackageIdentity) null;
      gpgSignature = (DeflateCompressibleBytes) null;
      return parsedIngestionParams;
    }

    private (IEnumerable<HashAndType> hashes, BlobStorageId storageId) ExtractHashesFromInput(
      PackageIngestionFormData input)
    {
      IReadOnlyDictionary<string, string[]> rawMetadata = input.RawMetadata;
      BlobStorageId blobStorageId = new BlobStorageId(input.PackageFileStream.ExistingBlobIdentifier ?? throw new InvalidOperationException("PackageFileStream has neither stream nor existing blob ID"));
      if (!this.trustClientProvidedHashInBlobFirstIngestFlowsSetting.Get())
        return (Enumerable.Empty<HashAndType>(), blobStorageId);
      List<HashAndType> hashAndTypeList = new List<HashAndType>();
      string[] strArray1;
      if (rawMetadata.TryGetValue("sha256_digest", out strArray1) && strArray1.Length != 0)
        hashAndTypeList.Add(new HashAndType()
        {
          HashType = Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.SHA256,
          Value = strArray1[0]
        });
      string[] strArray2;
      if (rawMetadata.TryGetValue("md5_digest", out strArray2) && strArray2.Length != 0)
        hashAndTypeList.Add(new HashAndType()
        {
          HashType = Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.MD5,
          Value = strArray2[0]
        });
      return ((IEnumerable<HashAndType>) hashAndTypeList, blobStorageId);
    }

    private async Task<Stream> GetSequentialStreamFromBlob(PackageFileStream inputPackageFileStream)
    {
      if ((object) inputPackageFileStream.ExistingBlobIdentifier == null)
        throw new InvalidOperationException("PackageFileStream object must reference either a stream or a BlobIdentifier");
      HttpResponseMessage async = await this.httpClient.GetAsync(await this.blobStore.GetDownloadUriAsync(new BlobIdWithHeaders(inputPackageFileStream.ExistingBlobIdentifier, EdgeCache.NotAllowed)), HttpCompletionOption.ResponseHeadersRead);
      async.EnsureSuccessStatusCode();
      return await async.Content.ReadAsStreamAsync();
    }

    private static async Task<(IEnumerable<HashAndType> hashes, BlobStorageId storageId)> ComputeHashes(
      Stream packageFileStream)
    {
      MD5 md5 = MD5.Create();
      SHA256 sha256 = SHA256.Create();
      BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm vsoHash = new BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm();
      vsoHash.Initialize();
      (IEnumerable<HashAndType>, BlobStorageId) hashes;
      try
      {
        if (packageFileStream.CanSeek)
          packageFileStream.Position = 0L;
        await packageFileStream.ReadIntoMultipleHashAlgorithmsAsync((IReadOnlyList<HashAlgorithm>) new HashAlgorithm[3]
        {
          (HashAlgorithm) md5,
          (HashAlgorithm) sha256,
          (HashAlgorithm) vsoHash
        });
        if (packageFileStream.CanSeek)
          packageFileStream.Position = 0L;
        hashes = ((IEnumerable<HashAndType>) new HashAndType[2]
        {
          new HashAndType()
          {
            HashType = Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.MD5,
            Value = ((IList<byte>) md5.Hash).ToHex()
          },
          new HashAndType()
          {
            HashType = Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.SHA256,
            Value = ((IList<byte>) sha256.Hash).ToHex()
          }
        }, new BlobStorageId(new BlobIdentifier(vsoHash.Hash)));
      }
      finally
      {
        ((IHashAlgorithmWithCleanup) vsoHash).Cleanup();
      }
      md5 = (MD5) null;
      sha256 = (SHA256) null;
      vsoHash = (BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm) null;
      return hashes;
    }

    public async Task Validate(IFeedRequest feedRequest, PyPiParsedIngestionParams parsed)
    {
      string sortableVersion = parsed.PackageIdentity.Version.SortableVersion;
      NullResult nullResult = await this.mainValidatingHandler.Handle((IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>) new PyPiPackageIngester2.StorablePackageWrapper(feedRequest, parsed));
    }

    public async Task Validate(
      IFeedRequest feedRequest,
      PyPiParsedIngestionParams parsed,
      MetadataDocument<IPyPiMetadataEntry>? metadataDoc,
      IPyPiMetadataEntry? existingEntryForVersionBeingIngested)
    {
      NullResult nullResult = await this.onlyOneSourceDistValidatingHandler.Handle(((IPyPiStorablePackageInfo) new PyPiPackageIngester2.StorablePackageWrapper(feedRequest, parsed), existingEntryForVersionBeingIngested));
    }

    public async Task CommitStorage(IFeedRequest feedRequest, PyPiParsedIngestionParams parsed)
    {
      BlobReference blobReference = new BlobReference(this.refCalculatingConverter.Convert(feedRequest.WithPackage<PyPiPackageIdentity>(parsed.PackageIdentity).WithFile<PyPiPackageIdentity>(parsed.FileName.Path)), Protocol.PyPi.LowercasedName);
      BlobIdentifier existingBlobIdentifier = parsed.PackageFileStream.ExistingBlobIdentifier;
      if ((object) existingBlobIdentifier != null)
      {
        if (!await this.blobStore.TryReferenceAsync(existingBlobIdentifier, blobReference))
          throw PackageContentBlobNotFoundException.Create(existingBlobIdentifier.ValueString);
        existingBlobIdentifier = (BlobIdentifier) null;
      }
      else
      {
        if (parsed.PackageFileStream.Stream == null)
          throw new InvalidOperationException("PackageFileStream has neither stream nor existing blob ID");
        await this.blobStore.PutBlobAndReferenceAsync(((BlobStorageId) parsed.StorageId).BlobId, parsed.PackageFileStream.Stream, blobReference);
        existingBlobIdentifier = (BlobIdentifier) null;
      }
    }

    public IAddOperationData PrepareAddOperation(
      PyPiParsedIngestionParams parsed,
      ProvenanceInfo provenanceInfo,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return (IAddOperationData) new PyPiAddOperationData(parsed.PackageIdentity, parsed.MetadataFields, parsed.StorageId, parsed.PackageFileStream.Length, parsed.PackageFileStream.FilePath, parsed.Hashes.SingleOrDefault<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.SHA256))?.Value, parsed.Hashes.SingleOrDefault<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.MD5))?.Value, parsed.GpgSignature, sourceChain, provenanceInfo);
    }

    private class StorablePackageWrapper : 
      IPyPiStorablePackageInfo,
      IStorablePackageInfo<
      #nullable disable
      PyPiPackageIdentity, PyPiUploadedPackageMetadata>,
      IStorablePackageInfo<PyPiPackageIdentity>,
      IPackageRequest<PyPiPackageIdentity>,
      IPackageRequest,
      IFeedRequest,
      IProtocolAgnosticFeedRequest,
      IPackageFileRequest<PyPiPackageIdentity>,
      IPackageFileRequest
    {
      private readonly 
      #nullable enable
      IFeedRequest feedRequest;
      private readonly PyPiParsedIngestionParams parsed;

      public StorablePackageWrapper(IFeedRequest feedRequest, PyPiParsedIngestionParams parsed)
      {
        this.feedRequest = feedRequest;
        this.parsed = parsed;
        this.ProtocolSpecificInfo = new PyPiUploadedPackageMetadata()
        {
          MetadataFields = parsed.MetadataFields,
          Metadata = (IPyPiResolvedMetadata) parsed.ResolvedMetadata,
          PackageFileStream = parsed.PackageFileStream,
          ComputedMd5 = parsed.Hashes.SingleOrDefault<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.MD5))?.Value,
          ComputedSha256 = parsed.Hashes.SingleOrDefault<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.SHA256))?.Value,
          GpgSignature = parsed.GpgSignature
        };
      }

      public IStorageId PackageStorageId => this.parsed.StorageId;

      public IEnumerable<UpstreamSourceInfo> SourceChain => throw new NotSupportedException("Can't use SourceChain in this context. Instead, use IngestionDirection");

      public IngestionDirection IngestionDirection => this.parsed.IngestionDirection;

      IPackageIdentity IPackageRequest.PackageId => (IPackageIdentity) this.PackageId;

      public PyPiPackageIdentity PackageId => this.parsed.PackageIdentity;

      public string FilePath => this.parsed.FileName.Path;

      public PyPiUploadedPackageMetadata ProtocolSpecificInfo { get; }

      public FeedCore Feed => this.feedRequest.Feed;

      public string? UserSuppliedFeedNameOrId => this.feedRequest.UserSuppliedFeedNameOrId;

      public string? UserSuppliedProjectNameOrId => this.feedRequest.UserSuppliedProjectNameOrId;

      public IProtocol Protocol => this.feedRequest.Protocol;
    }
  }
}
