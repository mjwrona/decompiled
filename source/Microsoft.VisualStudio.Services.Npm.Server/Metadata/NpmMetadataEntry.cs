// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Metadata.NpmMetadataEntry
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Metadata
{
  public class NpmMetadataEntry : 
    MetadataEntryWithSingleFile<NpmPackageIdentity>,
    INpmMetadataEntryWriteable,
    INpmMetadataEntry,
    IMetadataEntry<NpmPackageIdentity>,
    IMetadataEntry,
    IPackageFiles,
    ICreateWriteable<INpmMetadataEntryWriteable>,
    IMetadataEntryWriteable<NpmPackageIdentity>,
    IMetadataEntryWritable
  {
    private readonly NpmPackageIdentity packageId;
    private readonly Lazy<PackageJson> packageJsonLazy;
    private readonly Lazy<SemanticVersion> parsedVersionLazy;

    public NpmMetadataEntry(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      byte[] packageJsonUncompressedBytes,
      string packageSha1Sum,
      PackageJsonOptions packageJsonOptions,
      PackageManifest packageManifest,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      IEnumerable<Guid> views)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, packageStorageId, packageSize, sourceChain)
    {
      this.PackageJsonCompressedBytes = CompressionHelper.DeflateByteArray((byte[]) packageJsonUncompressedBytes.Clone());
      this.parsedVersionLazy = new Lazy<SemanticVersion>((Func<SemanticVersion>) (() => NpmVersionUtils.ParseNpmPackageVersion(this.packageJsonLazy.Value.Version)));
      this.packageJsonLazy = new Lazy<PackageJson>(new Func<PackageJson>(this.InitializeMetadata));
      this.PackageJsonOptions = packageJsonOptions;
      this.PackageManifest = packageManifest;
      this.PackageSha1Sum = packageSha1Sum;
      this.Views = views;
    }

    public NpmMetadataEntry(NpmPackageIdentity packageId)
    {
      this.packageId = packageId;
      this.packageJsonLazy = new Lazy<PackageJson>(new Func<PackageJson>(this.InitializeMetadata));
    }

    private PackageJson InitializeMetadata()
    {
      if (this.PackageJsonCompressedBytes == null)
        return (PackageJson) null;
      DeflateCompressibleBytes packageJsonBytes = DeflateCompressibleBytes.FromDeflatedBytes(this.PackageJsonCompressedBytes);
      IEnumerable<UpstreamSourceInfo> sourceChain = this.SourceChain;
      string location = sourceChain != null ? sourceChain.FirstOrDefault<UpstreamSourceInfo>()?.Location : (string) null;
      IReadOnlyList<Exception> exceptionList;
      ref IReadOnlyList<Exception> local = ref exceptionList;
      return PackageJsonUtils.DeserializeNpmJsonDocument<PackageJson>(packageJsonBytes, (ITracerService) null, (string) null, (string) null, location, nameof (NpmMetadataEntry), out local);
    }

    public ContentBytes PackageJsonContentBytes => new ContentBytes(this.PackageJsonCompressedBytes, true);

    public byte[] PackageJsonCompressedBytes { get; private set; }

    private SemanticVersion ParsedPackageVersion
    {
      get
      {
        SemanticVersion version = this.packageId?.Version;
        return (object) version != null ? version : this.parsedVersionLazy.Value;
      }
    }

    public override NpmPackageIdentity PackageIdentity => this.packageId != null ? this.packageId : new NpmPackageIdentity(this.packageJsonLazy.Value.Name, this.ParsedPackageVersion);

    public string PackageSha1Sum { get; set; }

    protected override IReadOnlyCollection<HashAndType> SingleFileHashes => (IReadOnlyCollection<HashAndType>) new HashAndType[1]
    {
      new HashAndType()
      {
        HashType = HashType.SHA1,
        Value = this.PackageSha1Sum
      }
    };

    public PackageManifest PackageManifest { get; set; }

    public string Deprecated { get; set; }

    public PackageJson PackageJson => this.packageJsonLazy.Value.ApplyOptions(this.PackageJsonOptions);

    public bool HasGypFileAtRoot => this.PackageJsonOptions != null && this.PackageJsonOptions.ContainsBindingGypFileAtRoot;

    public bool HasServerJsAtRoot => this.PackageJsonOptions != null && this.PackageJsonOptions.ContainsServerJsFileAtRoot;

    public JToken GitHead => this.PackageJsonOptions?.GitHead;

    public PackageJsonOptions PackageJsonOptions { get; set; }

    public void SetPackageJsonBytes(byte[] bytes, bool areBytesCompressed)
    {
      if (!areBytesCompressed)
        bytes = CompressionHelper.DeflateByteArray(bytes);
      this.PackageJsonCompressedBytes = bytes;
    }

    public INpmMetadataEntryWriteable CreateWriteable(ICommitLogEntryHeader commitEntry)
    {
      NpmMetadataEntry writeable = new NpmMetadataEntry(this.PackageIdentity);
      writeable.CopyFrom((MetadataEntry<NpmPackageIdentity, IPackageFile>) this);
      writeable.SetUpdatingCommitInfo(commitEntry);
      writeable.PackageJsonCompressedBytes = (byte[]) this.PackageJsonCompressedBytes?.Clone();
      writeable.PackageJsonOptions = this.PackageJsonOptions;
      writeable.PackageManifest = this.PackageManifest;
      writeable.Deprecated = this.Deprecated;
      writeable.PackageSha1Sum = this.PackageSha1Sum;
      return (INpmMetadataEntryWriteable) writeable;
    }

    public override string SingleFilePath => this.PackageIdentity.ToTgzFilePath();

    public override IEqualityComparer<string> PathEqualityComparer { get; } = (IEqualityComparer<string>) StringComparer.Ordinal;

    protected override string FixupFilePath(string filePath) => !filePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) ? filePath : this.SingleFilePath;

    void IMetadataEntryWritable.set_CommitId(PackagingCommitId value) => this.CommitId = value;

    void IMetadataEntryWritable.set_CreatedBy(Guid value) => this.CreatedBy = value;

    void IMetadataEntryWritable.set_CreatedDate(DateTime value) => this.CreatedDate = value;

    void IMetadataEntryWritable.set_ModifiedBy(Guid value) => this.ModifiedBy = value;

    void IMetadataEntryWritable.set_ModifiedDate(DateTime value) => this.ModifiedDate = value;

    void IMetadataEntryWritable.set_PackageStorageId(IStorageId value) => this.PackageStorageId = value;

    void IMetadataEntryWritable.set_PackageSize(long value) => this.PackageSize = value;

    void IMetadataEntryWritable.set_Views(IEnumerable<Guid> value) => this.Views = value;

    void IMetadataEntryWritable.set_DeletedDate(DateTime? value) => this.DeletedDate = value;

    void IMetadataEntryWritable.set_ScheduledPermanentDeleteDate(DateTime? value) => this.ScheduledPermanentDeleteDate = value;

    void IMetadataEntryWritable.set_PermanentDeletedDate(DateTime? value) => this.PermanentDeletedDate = value;

    void IMetadataEntryWritable.set_QuarantineUntilDate(DateTime? value) => this.QuarantineUntilDate = value;

    void IMetadataEntryWritable.set_SourceChain(IEnumerable<UpstreamSourceInfo> value) => this.SourceChain = value;
  }
}
