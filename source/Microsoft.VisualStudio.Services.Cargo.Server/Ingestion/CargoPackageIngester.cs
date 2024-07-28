// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoPackageIngester
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using BuildXL.Cache.ContentStore.Interfaces.Utils;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Upload;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public class CargoPackageIngester : 
    IProtocolPackageIngestion<CargoIngestionInput, CargoParsedIngestionParams, ICargoMetadataEntry>
  {
    private readonly IFrotocolLevelPackagingSetting<long> maxPackageSizeSetting;
    private readonly IFrotocolLevelPackagingSetting<int> maxManifestSizeSetting;
    private readonly IOrgLevelPackagingSetting<bool> allowNonAsciiNamesSetting;
    private readonly IContentBlobStore blobStore;
    private readonly IOrgLevelPackagingSetting<bool> enforceConsistentIdentitySetting;
    private static readonly ImmutableArray<string> DefaultReadmeFileNames = ImmutableArray.Create<string>("README.md", "README.txt", "README");
    private static readonly ImmutableArray<string> ArrayOfReadmeMd = ImmutableArray.Create<string>("README.md");
    private static readonly ImmutableArray<string> LikelyLicenseFileNames = ImmutableArray.Create<string>("LICENSE.md", "LICENSE.txt", "LICENSE");
    private static readonly StringComparer ArchiveFileNameComparer = StringComparer.OrdinalIgnoreCase;

    public CargoPackageIngester(
      IFrotocolLevelPackagingSetting<long> maxPackageSizeSetting,
      IFrotocolLevelPackagingSetting<int> maxManifestSizeSetting,
      IOrgLevelPackagingSetting<bool> allowNonAsciiNamesSetting,
      IContentBlobStore blobStore,
      IOrgLevelPackagingSetting<bool> enforceConsistentIdentitySetting)
    {
      this.maxPackageSizeSetting = maxPackageSizeSetting;
      this.maxManifestSizeSetting = maxManifestSizeSetting;
      this.allowNonAsciiNamesSetting = allowNonAsciiNamesSetting;
      this.blobStore = blobStore;
      this.enforceConsistentIdentitySetting = enforceConsistentIdentitySetting;
    }

    public Task CheckInputContentSize(
      IFeedRequest feedRequest,
      CargoIngestionInput input,
      IngestionDirection ingestionDirection)
    {
      long num1 = this.maxPackageSizeSetting.Get(feedRequest);
      if (input.PackageStream.Length > num1)
        throw new PackageLimitExceededException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_FileLimitExceeded((object) num1));
      int num2 = this.maxManifestSizeSetting.Get(feedRequest);
      LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> publishManifest = input.PublishManifest;
      if ((publishManifest != null ? (publishManifest.Serialized.AsUncompressedBytes().Length > num2 ? 1 : 0) : 0) != 0)
        throw new PackageLimitExceededException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_ManifestLengthExceeded((object) num2));
      return Task.CompletedTask;
    }

    public async Task<CargoParsedIngestionParams> Parse(
      CargoIngestionInput input,
      IngestionDirection ingestionDirection)
    {
      LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> inputPublishManifest = input.PublishManifest;
      LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> inputIndexRow = input.IndexRow;
      if (inputPublishManifest == null && inputIndexRow == null)
        throw new InvalidOperationException("ingestion input must contain either a publish manifest or an index row");
      CargoPackageIdentity externalPackageIdentity = inputPublishManifest != null ? inputPublishManifest.Value.ExtractPackageIdentity() : inputIndexRow.Value.ExtractPackageIdentity();
      SHA256 hash = SHA256.Create();
      BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm vsoHash = new BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm();
      vsoHash.Initialize();
      List<CapturedFile> capturedFiles = new List<CapturedFile>();
      CargoParsedIngestionParams parsedIngestionParams1;
      try
      {
        input.PackageStream.Position = 0L;
        string folderPrefix = externalPackageIdentity.Name.DisplayName + "-" + externalPackageIdentity.Version.DisplayVersion + "/";
        LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> lazyCargoToml;
        string sha256HashOfStream;
        BlobStorageId blobStorageId;
        ImmutableArray<string> configuredLicenseFiles;
        ImmutableArray<string> configuredReadmeFiles;
        using (Microsoft.TeamFoundation.Framework.Common.HashingStream sha256Stream = new Microsoft.TeamFoundation.Framework.Common.HashingStream(input.PackageStream, (HashAlgorithm) hash, true))
        {
          using (Microsoft.TeamFoundation.Framework.Common.HashingStream vsoHashStream = new Microsoft.TeamFoundation.Framework.Common.HashingStream((Stream) sha256Stream, (HashAlgorithm) vsoHash, true))
          {
            CargoPackageIngester.ExtractFilesFromCrateTarball((Stream) vsoHashStream, folderPrefix, CargoPackageIngester.DefaultReadmeFileNames, CargoPackageIngester.LikelyLicenseFileNames, (ICollection<CapturedFile>) capturedFiles, false, out lazyCargoToml, out configuredReadmeFiles, out configuredLicenseFiles);
            await vsoHashStream.CopyToAsync(Stream.Null);
            sha256HashOfStream = ((IList<byte>) sha256Stream.HashValue).ToHex();
            blobStorageId = new BlobStorageId(new BlobIdentifier(vsoHashStream.HashValue));
            string sha256 = inputIndexRow?.Value.Sha256;
            if (!string.IsNullOrWhiteSpace(sha256))
            {
              if (!sha256HashOfStream.Equals(sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_PackageDoesNotMatchHash());
            }
          }
        }
        input.PackageStream.Position = 0L;
        if (lazyCargoToml == null)
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_CrateMissingRequiredFile((object) (folderPrefix + "Cargo.toml")));
        CapturedFile actualLicenseFile = FindBestMatchingFile((ICollection<CapturedFile>) capturedFiles, configuredLicenseFiles);
        CapturedFile actualReadmeFile = FindBestMatchingFile((ICollection<CapturedFile>) capturedFiles, configuredReadmeFiles);
        ImmutableArray<string> immutableArray1 = configuredLicenseFiles.Except<string>((IEnumerable<string>) CargoPackageIngester.LikelyLicenseFileNames).ToImmutableArray<string>();
        ImmutableArray<string> immutableArray2 = configuredReadmeFiles.Except<string>((IEnumerable<string>) CargoPackageIngester.DefaultReadmeFileNames).ToImmutableArray<string>();
        if ((object) actualLicenseFile == null && !immutableArray1.IsEmpty || (object) actualReadmeFile == null && !immutableArray2.IsEmpty)
        {
          CargoPackageIngester.ExtractFilesFromCrateTarball(input.PackageStream, folderPrefix, immutableArray2, immutableArray1, (ICollection<CapturedFile>) capturedFiles, true, out LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> _, out ImmutableArray<string> _, out ImmutableArray<string> _);
          input.PackageStream.Position = 0L;
          actualLicenseFile = FindBestMatchingFile((ICollection<CapturedFile>) capturedFiles, configuredLicenseFiles);
          actualReadmeFile = FindBestMatchingFile((ICollection<CapturedFile>) capturedFiles, configuredReadmeFiles);
        }
        ImmutableArray<(CapturedFile, InnerFileReference)>.Builder innerFiles = ImmutableArray.CreateBuilder<(CapturedFile, InnerFileReference)>();
        ImmutableArray<(CapturedFile, InnerFileReference)>.Builder builder;
        CapturedFile capturedFile;
        if ((object) actualLicenseFile != null)
        {
          builder = innerFiles;
          capturedFile = actualLicenseFile;
          builder.Add((capturedFile, await MakeInnerFileReference(actualLicenseFile)));
          builder = (ImmutableArray<(CapturedFile, InnerFileReference)>.Builder) null;
          capturedFile = (CapturedFile) null;
        }
        if ((object) actualReadmeFile != null)
        {
          builder = innerFiles;
          capturedFile = actualReadmeFile;
          builder.Add((capturedFile, await MakeInnerFileReference(actualReadmeFile)));
          builder = (ImmutableArray<(CapturedFile, InnerFileReference)>.Builder) null;
          capturedFile = (CapturedFile) null;
        }
        int num1;
        if (ingestionDirection == IngestionDirection.PullFromUpstream)
        {
          LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> lazySerDesValue = inputIndexRow;
          num1 = lazySerDesValue != null ? (lazySerDesValue.Value.Yanked ? 1 : 0) : 0;
        }
        else
          num1 = 0;
        bool flag = num1 != 0;
        CargoPackageIdentity PackageIdentity = externalPackageIdentity;
        LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> PublishManifest = inputPublishManifest;
        LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> UpstreamIndexRow = inputIndexRow;
        LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> CargoToml = lazyCargoToml;
        string nameInArchive1 = actualLicenseFile?.NameInArchive;
        string nameInArchive2 = actualReadmeFile?.NameInArchive;
        string ActualLicenseFilePath = nameInArchive1;
        LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> Metadata = CargoPackageMetadata.LazyDeserialize(new CargoRawPackageMetadata(PublishManifest, UpstreamIndexRow, CargoToml, nameInArchive2, ActualLicenseFilePath));
        Stream packageStream = input.PackageStream;
        BlobStorageId StorageId = blobStorageId;
        int num2 = (int) ingestionDirection;
        ImmutableArray<HashAndType> Hashes = ImmutableArray.Create<HashAndType>(new HashAndType()
        {
          HashType = Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.HashType.SHA256,
          Value = sha256HashOfStream
        });
        ImmutableArray<(CapturedFile, InnerFileReference)> immutable = innerFiles.ToImmutable();
        int num3 = flag ? 1 : 0;
        CargoParsedIngestionParams parsedIngestionParams2 = new CargoParsedIngestionParams(PackageIdentity, Metadata, packageStream, StorageId, (IngestionDirection) num2, Hashes, immutable, num3 != 0);
        if ((object) actualLicenseFile != null)
          capturedFiles.Remove(actualLicenseFile);
        if ((object) actualReadmeFile != null)
          capturedFiles.Remove(actualReadmeFile);
        parsedIngestionParams1 = parsedIngestionParams2;
      }
      finally
      {
        ((IHashAlgorithmWithCleanup) vsoHash).Cleanup();
        foreach (CapturedFile capturedFile in capturedFiles)
          capturedFile.TempFileStream.Close();
      }
      inputPublishManifest = (LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null;
      inputIndexRow = (LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>) null;
      externalPackageIdentity = (CargoPackageIdentity) null;
      vsoHash = (BuildXL.Cache.ContentStore.Hashing.VsoHashAlgorithm) null;
      capturedFiles = (List<CapturedFile>) null;
      return parsedIngestionParams1;

      static CapturedFile? FindBestMatchingFile(
        ICollection<CapturedFile> files,
        ImmutableArray<string> fileNames)
      {
        foreach (string fileName1 in fileNames)
        {
          string fileName = fileName1;
          CapturedFile capturedFile = files.FirstOrDefault<CapturedFile>((Func<CapturedFile, bool>) (x => x.NameInPackageFolder.Equals(fileName, StringComparison.Ordinal)));
          if ((object) capturedFile == null)
            capturedFile = files.FirstOrDefault<CapturedFile>((Func<CapturedFile, bool>) (x => x.NameInPackageFolder.Equals(fileName, StringComparison.OrdinalIgnoreCase)));
          CapturedFile bestMatchingFile = capturedFile;
          if ((object) bestMatchingFile != null)
            return bestMatchingFile;
        }
        return (CapturedFile) null;
      }

      static async Task<InnerFileReference> MakeInnerFileReference(CapturedFile actualLicenseFile)
      {
        BlobIdentifier blobIdentifierAsync = await Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.CalculateBlobIdentifierAsync(actualLicenseFile.TempFileStream);
        actualLicenseFile.TempFileStream.Position = 0L;
        return new InnerFileReference(actualLicenseFile.NameInArchive, (IStorageId) new BlobStorageId(blobIdentifierAsync), actualLicenseFile.TempFileStream.Length);
      }
    }

    private static void ExtractFilesFromCrateTarball(
      Stream crateTarballStream,
      string folderPrefix,
      ImmutableArray<string> readmeFileNamesToCapture,
      ImmutableArray<string> licenseFileNamesToCapture,
      ICollection<CapturedFile> capturedFiles,
      bool stopBeforeCargoToml,
      out LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>? lazyCargoToml,
      out ImmutableArray<string> configuredReadmeFiles,
      out ImmutableArray<string> configuredLicenseFiles)
    {
      lazyCargoToml = (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null;
      configuredReadmeFiles = ImmutableArray<string>.Empty;
      configuredLicenseFiles = ImmutableArray<string>.Empty;
      GZipInputStream gzipInputStream = new GZipInputStream(crateTarballStream);
      gzipInputStream.IsStreamOwner = false;
      using (GZipInputStream inputStream = gzipInputStream)
      {
        using (TarInputStream tarStream = new TarInputStream((Stream) inputStream, Encoding.UTF8)
        {
          IsStreamOwner = false
        })
        {
          while (true)
          {
            do
            {
              string str;
              do
              {
                TarEntry nextEntry;
                do
                {
                  nextEntry = tarStream.GetNextEntry();
                  if (nextEntry == null)
                    goto label_8;
                }
                while (!nextEntry.Name.StartsWith(folderPrefix, StringComparison.OrdinalIgnoreCase));
                str = nextEntry.Name.Substring(folderPrefix.Length);
                if (readmeFileNamesToCapture.Contains<string>(str, (IEqualityComparer<string>) CargoPackageIngester.ArchiveFileNameComparer) || licenseFileNamesToCapture.Contains<string>(str, (IEqualityComparer<string>) CargoPackageIngester.ArchiveFileNameComparer))
                  capturedFiles.Add(CargoPackageIngester.CaptureFile(tarStream, nextEntry, str));
              }
              while (!CargoPackageIngester.ArchiveFileNameComparer.Equals(str, "Cargo.toml"));
              if (stopBeforeCargoToml)
                goto label_11;
            }
            while (lazyCargoToml != null);
            lazyCargoToml = CargoPackageIngester.ExtractCargoToml(tarStream);
            CargoTomlManifestPackageOrProject package = lazyCargoToml.Value.Package;
            configuredReadmeFiles = ReadmeFileNamesFromReadmeSetting(package.Readme);
            readmeFileNamesToCapture = configuredReadmeFiles;
            configuredLicenseFiles = LicenseFileNamesFromLicenseFileSetting(package.LicenseFile);
            licenseFileNamesToCapture = configuredLicenseFiles;
          }
label_8:
          return;
label_11:;
        }
      }

      static ImmutableArray<string> ReadmeFileNamesFromReadmeSetting(StringOrBool? readmeSetting)
      {
        StringOrBool.String @string = readmeSetting as StringOrBool.String;
        if ((object) @string != null)
        {
          string fileName;
          @string.Deconstruct(out fileName);
          return FileNameToArray(fileName);
        }
        StringOrBool.Bool @bool = readmeSetting as StringOrBool.Bool;
        if ((object) @bool != null)
        {
          bool flag;
          @bool.Deconstruct(out flag);
          return !flag ? ImmutableArray<string>.Empty : CargoPackageIngester.ArrayOfReadmeMd;
        }
        if ((object) readmeSetting == null)
          return CargoPackageIngester.DefaultReadmeFileNames;
        throw new ArgumentOutOfRangeException();
      }

      static ImmutableArray<string> LicenseFileNamesFromLicenseFileSetting(string? licenseFileSetting) => FileNameToArray(licenseFileSetting);

      static ImmutableArray<string> FileNameToArray(string? fileName)
      {
        string str = NormalizeFileNameFromCargoToml(fileName);
        return str == null ? ImmutableArray<string>.Empty : ImmutableArray.Create<string>(str);
      }

      static string? NormalizeFileNameFromCargoToml(string? fileName)
      {
        if (fileName == null)
          return (string) null;
        fileName = fileName.Trim();
        if (fileName.StartsWith("./") || fileName.StartsWith(".\\"))
          fileName = fileName.Substring(2).Trim();
        return fileName.Length <= 0 ? (string) null : fileName;
      }
    }

    private static LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> ExtractCargoToml(
      TarInputStream tarStream)
    {
      using (MemoryStream outputStream = new MemoryStream())
      {
        tarStream.CopyEntryContents((Stream) outputStream);
        return CargoTomlManifest.LazyDeserialize(DeflateCompressibleBytes.FromUncompressedBytes(outputStream.ToArray()));
      }
    }

    private static CapturedFile CaptureFile(
      TarInputStream tarStream,
      TarEntry entry,
      string nameInFolder)
    {
      FileInfo fileForIngestion = IngestionTemporaryFileHelper.CreateTemporaryFileForIngestion((IProtocol) Protocol.Cargo);
      FileStream fileStream = IngestionTemporaryFileHelper.OpenAutoDeletingTemporaryFile(fileForIngestion.FullName);
      try
      {
        tarStream.CopyEntryContents((Stream) fileStream);
        fileStream.Position = 0L;
        return new CapturedFile(entry.Name, nameInFolder, fileForIngestion, (Stream) fileStream);
      }
      catch
      {
        fileStream.Dispose();
        throw;
      }
    }

    public Task Validate(IFeedRequest feedRequest, CargoParsedIngestionParams parsed)
    {
      CargoPackageNameValidator.Validate(parsed.PackageIdentity.Name, !this.allowNonAsciiNamesSetting.Get());
      CargoPackageVersionValidator.Validate(parsed.PackageIdentity.Version, parsed.IngestionDirection != IngestionDirection.PullFromUpstream);
      if (this.enforceConsistentIdentitySetting.Get())
        CargoPackageIngester.ValidateConsistentNameAndVersion(parsed);
      return Task.CompletedTask;
    }

    private static void ValidateConsistentNameAndVersion(CargoParsedIngestionParams parsed)
    {
      LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> cargoToml = parsed.Metadata.Serialized.CargoToml;
      Check(Microsoft.VisualStudio.Services.Cargo.Server.Resources.CrateIdLocation_CargoToml(), parsed.PackageIdentity, cargoToml.Value.Package.Name, cargoToml.Value.Package.Version);
      LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> publishManifest = parsed.Metadata.Serialized.PublishManifest;
      if (publishManifest != null)
        Check(Microsoft.VisualStudio.Services.Cargo.Server.Resources.CrateIdLocation_PublishManifest(), parsed.PackageIdentity, publishManifest.Value.name, publishManifest.Value.vers);
      LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> upstreamIndexRow = parsed.Metadata.Serialized.UpstreamIndexRow;
      if (upstreamIndexRow == null)
        return;
      Check(Microsoft.VisualStudio.Services.Cargo.Server.Resources.CrateIdLocation_UpstreamIndexRow(), parsed.PackageIdentity, upstreamIndexRow.Value.PackageName, upstreamIndexRow.Value.Version);

      static void Check(
        string location,
        CargoPackageIdentity id,
        string? packageName,
        string? packageVersion)
      {
        if (!string.IsNullOrWhiteSpace(packageName) && !CargoPackageNameParser.ParseWithoutValidating(packageName).Equals(id.Name))
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_MismatchedCrateName((object) id.Name.DisplayName, (object) location, (object) packageName));
        if (!string.IsNullOrWhiteSpace(packageVersion) && !CargoPackageVersionParser.ParseWithoutValidating(packageVersion).Equals(id.Version))
          throw new InvalidPackageException(Microsoft.VisualStudio.Services.Cargo.Server.Resources.Error_MismatchedCrateVersion((object) id.Version.DisplayVersion, (object) location, (object) packageVersion));
      }
    }

    public Task Validate(
      IFeedRequest feedRequest,
      CargoParsedIngestionParams parsed,
      MetadataDocument<ICargoMetadataEntry>? metadataDoc,
      ICargoMetadataEntry? existingEntryForVersionBeingIngested)
    {
      return Task.CompletedTask;
    }

    public async Task CommitStorage(IFeedRequest feedRequest, CargoParsedIngestionParams parsed)
    {
      IdBlobReference reference1 = CargoBlobRefGenerator.ForCrate(feedRequest.Feed.Id, parsed.PackageIdentity);
      await this.blobStore.PutBlobAndReferenceAsync(parsed.StorageId.BlobId, parsed.CrateStream, new BlobReference(reference1));
      parsed.CrateStream.Position = 0L;
      foreach ((CapturedFile capturedFile, InnerFileReference innerFileReference) in parsed.InnerFiles)
      {
        if (!(innerFileReference.StorageId is BlobStorageId storageId))
          throw new InvalidOperationException("Expected inner file storage id to be BlobStorageId, but it was " + innerFileReference.StorageId.GetType().FullName);
        IdBlobReference reference2 = CargoBlobRefGenerator.ForCrateInnerFile(feedRequest.Feed.Id, parsed.PackageIdentity, innerFileReference.Path);
        await this.blobStore.PutBlobAndReferenceAsync(storageId.BlobId, capturedFile.TempFileStream, new BlobReference(reference2));
        capturedFile.TempFileStream.Position = 0L;
        capturedFile = (CapturedFile) null;
      }
    }

    public IAddOperationData PrepareAddOperation(
      CargoParsedIngestionParams parsed,
      ProvenanceInfo provenanceInfo,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return (IAddOperationData) new CargoAddOperationData(parsed.PackageIdentity, (IStorageId) parsed.StorageId, parsed.Size, parsed.Metadata, (IEnumerable<HashAndType>) parsed.Hashes, sourceChain, provenanceInfo, parsed.InnerFiles.Select<(CapturedFile, InnerFileReference), InnerFileReference>((Func<(CapturedFile, InnerFileReference), InnerFileReference>) (x => x.InnerFileReference)).ToImmutableArray<InnerFileReference>(), parsed.AddAsYanked);
    }
  }
}
