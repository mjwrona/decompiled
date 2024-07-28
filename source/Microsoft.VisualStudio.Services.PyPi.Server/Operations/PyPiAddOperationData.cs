// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Operations.PyPiAddOperationData
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Operations
{
  public class PyPiAddOperationData : 
    IAddOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public PyPiAddOperationData(
      PyPiPackageIdentity identity,
      IReadOnlyDictionary<string, string[]> metadataFields,
      IStorageId packageStorageId,
      long packageSize,
      string fileName,
      string computedSha256,
      string computedMd5,
      DeflateCompressibleBytes gpgSignature,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      ProvenanceInfo provenance)
    {
      this.Identity = identity ?? throw new ArgumentNullException(nameof (identity));
      this.MetadataFields = metadataFields;
      this.PackageStorageId = packageStorageId ?? throw new ArgumentNullException(nameof (packageStorageId));
      this.PackageSize = packageSize;
      this.FileName = fileName;
      this.ComputedSha256 = computedSha256;
      this.ComputedMd5 = computedMd5;
      this.GpgSignature = gpgSignature;
      this.SourceChain = sourceChain;
      this.Provenance = provenance;
    }

    public IStorageId PackageStorageId { get; }

    public long PackageSize { get; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public PyPiPackageIdentity Identity { get; }

    public IReadOnlyDictionary<string, string[]> MetadataFields { get; }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    public RingOrder RingOrder { get; }

    public FeedPermissionConstants PermissionDemand => PackageIngestionUtils.GetRequiredAddPackagePermission(this.GetIngestionDirection());

    public string FileName { get; }

    public string ComputedSha256 { get; }

    public string ComputedMd5 { get; }

    public DeflateCompressibleBytes GpgSignature { get; }

    public ProvenanceInfo Provenance { get; }

    public PackageIndexEntry ConvertToIndexEntry(ICommitLogEntry commitLogEntry, FeedCore feed)
    {
      PyPiResolvedMetadata from = PyPiResolvedMetadata.ParseFrom(this.MetadataFields);
      ProtocolMetadata protocolMetadata1 = new ProtocolMetadata()
      {
        SchemaVersion = 1,
        Data = (object) new PyPiFeedIndexPackageInfo(this.MetadataFields)
      };
      ProtocolMetadata protocolMetadata2 = new ProtocolMetadata()
      {
        SchemaVersion = 1,
        Data = (object) new PyPiFeedIndexPackageFileInfo(this.MetadataFields, this.FileName, this.PackageStorageId, this.PackageSize, commitLogEntry.CreatedDate)
      };
      string optionalMetadataField = PyPiMetadataUtils.GetOptionalMetadataField("summary", this.MetadataFields);
      PackageIndexEntry indexEntry = new PackageIndexEntry();
      indexEntry.Name = this.Identity.Name.DisplayName;
      indexEntry.NormalizedName = this.Identity.Name.NormalizedName;
      indexEntry.ProtocolType = PyPiFeedIndexConstants.FeedIndexPyPiProtocolType;
      PackageVersionIndexEntry versionIndexEntry = new PackageVersionIndexEntry();
      versionIndexEntry.NormalizedVersion = this.Identity.Version.NormalizedVersion;
      versionIndexEntry.SortableVersion = this.Identity.Version.SortableVersion;
      versionIndexEntry.Version = this.Identity.Version.DisplayVersion;
      versionIndexEntry.VersionProtocolMetadata = protocolMetadata1;
      versionIndexEntry.PublishDate = new DateTime?(commitLogEntry.CreatedDate);
      versionIndexEntry.IsCached = false;
      versionIndexEntry.IsRelease = this.Identity.Version.IsRelease;
      versionIndexEntry.Summary = string.Empty;
      versionIndexEntry.Description = string.IsNullOrWhiteSpace(optionalMetadataField) ? string.Empty : optionalMetadataField;
      IReadOnlyList<RequirementSpec> requiresDistributions = from.RequiresDistributions;
      versionIndexEntry.Dependencies = (requiresDistributions != null ? requiresDistributions.Select<RequirementSpec, PackageDependency>(new Func<RequirementSpec, PackageDependency>(ConvertDependency)) : (IEnumerable<PackageDependency>) null) ?? Enumerable.Empty<PackageDependency>();
      versionIndexEntry.Files = (IEnumerable<PackageFile>) new List<PackageFile>()
      {
        new PackageFile()
        {
          Name = this.FileName,
          ProtocolMetadata = protocolMetadata2
        }
      };
      versionIndexEntry.Provenance = ProvenanceUtils.GetFeedProvenance(this.Provenance, commitLogEntry.UserId);
      IEnumerable<UpstreamSourceInfo> sourceChain = this.SourceChain;
      versionIndexEntry.SourceChain = sourceChain != null ? sourceChain.Select<UpstreamSourceInfo, UpstreamSource>((Func<UpstreamSourceInfo, UpstreamSource>) (x => x.ToFeedApiSource(PyPiFeedIndexConstants.FeedIndexPyPiProtocolType))) : (IEnumerable<UpstreamSource>) null;
      indexEntry.PackageVersion = versionIndexEntry;
      return indexEntry;

      static PackageDependency ConvertDependency(RequirementSpec spec) => new PackageDependency()
      {
        Group = (string) null,
        PackageName = spec.FormatNameAndExtras(),
        VersionRange = spec.FormatVersionAndMarker(RequirementVersionFormattingOptions.NoParenthesesAroundVersionList)
      };
    }
  }
}
