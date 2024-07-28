// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData.MavenCommitOperationData
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData
{
  public class MavenCommitOperationData : 
    IMavenCommitOperationData,
    IAddOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public MavenCommitOperationData(
      MavenPackageIdentity identity,
      IEnumerable<MavenPackageFileNew> files,
      byte[] pomBytes,
      ProvenanceInfo provenance,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      ArgumentUtility.CheckForNull<MavenPackageIdentity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IEnumerable<MavenPackageFileNew>>(files, nameof (files));
      this.Identity = identity;
      this.Files = files;
      this.PomBytes = pomBytes;
      this.Pom = this.GetParsedPom();
      this.Provenance = provenance;
      this.SourceChain = sourceChain ?? Enumerable.Empty<UpstreamSourceInfo>();
    }

    public IPackageName PackageName => (IPackageName) this.Identity.Name;

    IPackageIdentity IPackageVersionOperationData.Identity => (IPackageIdentity) this.Identity;

    public MavenPackageIdentity Identity { get; }

    public MavenPomMetadata Pom { get; }

    public byte[] PomBytes { get; }

    public IEnumerable<MavenPackageFileNew> Files { get; }

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => PackageIngestionUtils.GetRequiredAddPackagePermission(this.GetIngestionDirection());

    public IStorageId PackageStorageId { get; } = (IStorageId) new BlobStorageId(BlobIdentifier.MinValue);

    public long PackageSize { get; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    public ProvenanceInfo Provenance { get; }

    public PackageIndexEntry ConvertToIndexEntry(ICommitLogEntry commitLogEntry, FeedCore feed)
    {
      MavenPomMetadata pom = this.Pom;
      MavenPomMetadata patchedObject = pom != null ? pom.GetPatchedObject() : (MavenPomMetadata) null;
      PackageIndexEntry indexEntry = new PackageIndexEntry();
      indexEntry.Name = this.Identity.Name.DisplayName;
      indexEntry.NormalizedName = this.Identity.Name.NormalizedName;
      indexEntry.ProtocolType = "maven";
      PackageVersionIndexEntry versionIndexEntry = new PackageVersionIndexEntry();
      versionIndexEntry.Version = this.Identity.Version.DisplayVersion;
      versionIndexEntry.NormalizedVersion = this.Identity.Version.NormalizedVersion;
      versionIndexEntry.SortableVersion = this.Identity.Version.SortableVersion;
      versionIndexEntry.IsRelease = this.Identity.Version.Parser.IsRelease;
      versionIndexEntry.Description = string.IsNullOrWhiteSpace(patchedObject?.Description) ? string.Empty : patchedObject.Description;
      versionIndexEntry.Summary = string.Empty;
      versionIndexEntry.Tags = (IEnumerable<string>) new List<string>();
      versionIndexEntry.PublishDate = new DateTime?(commitLogEntry.CreatedDate);
      versionIndexEntry.Files = (IEnumerable<PackageFile>) new MavenPackageFilesNewToLegacyConverter().Convert(this.Files);
      versionIndexEntry.Dependencies = (IEnumerable<PackageDependency>) ((patchedObject != null ? patchedObject.GetAllDependencies() : (List<PackageDependency>) null) ?? new List<PackageDependency>());
      ProtocolMetadata protocolMetadata;
      if (patchedObject == null)
      {
        protocolMetadata = (ProtocolMetadata) null;
      }
      else
      {
        protocolMetadata = new ProtocolMetadata();
        protocolMetadata.SchemaVersion = 1;
        protocolMetadata.Data = (object) patchedObject;
      }
      versionIndexEntry.VersionProtocolMetadata = protocolMetadata;
      versionIndexEntry.Provenance = ProvenanceUtils.GetFeedProvenance(this.Provenance, commitLogEntry.UserId);
      versionIndexEntry.SourceChain = this.SourceChain.Select<UpstreamSourceInfo, UpstreamSource>((Func<UpstreamSourceInfo, UpstreamSource>) (x => x.ToFeedApiSource(Protocol.Maven.CorrectlyCasedName)));
      indexEntry.PackageVersion = versionIndexEntry;
      return indexEntry;
    }

    private MavenPomMetadata GetParsedPom() => this.PomBytes != null ? MavenPomUtility.Parse((Stream) new MemoryStream(this.PomBytes)) : (MavenPomMetadata) null;
  }
}
