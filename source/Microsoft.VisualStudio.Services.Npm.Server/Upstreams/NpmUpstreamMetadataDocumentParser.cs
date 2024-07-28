// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamMetadataDocumentParser
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamMetadataDocumentParser : INpmUpstreamMetadataDocumentParser
  {
    private readonly ITimeProvider timeProvider;
    private readonly IPackageMetadataSerializer packageMetadataSerializer;
    private readonly IUnknownUpstreamResponseSerializer unknownResponseSerializer;
    private readonly ITracerService tracer;
    private readonly IOrgLevelPackagingSetting<bool> propagateDeprecateSetting;

    public NpmUpstreamMetadataDocumentParser(
      ITimeProvider timeProvider,
      IPackageMetadataSerializer packageMetadataSerializer,
      IUnknownUpstreamResponseSerializer unknownResponseSerializer,
      ITracerService tracer,
      IOrgLevelPackagingSetting<bool> propagateDeprecateSetting)
    {
      this.timeProvider = timeProvider;
      this.packageMetadataSerializer = packageMetadataSerializer;
      this.unknownResponseSerializer = unknownResponseSerializer;
      this.tracer = tracer;
      this.propagateDeprecateSetting = propagateDeprecateSetting;
    }

    public MetadataDocument<INpmMetadataEntry> ParseUpstreamMetadataDocument(
      UpstreamSource upstreamSource,
      NpmPackageName packageName,
      string documentText)
    {
      using (ITracerBlock tracer = this.tracer.Enter((object) this, nameof (ParseUpstreamMetadataDocument)))
      {
        Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata packageMetadata;
        try
        {
          packageMetadata = this.packageMetadataSerializer.DeserializePackageMetadata(documentText, packageName, upstreamSource);
        }
        catch (Exception ex)
        {
          this.ThrowIfNotFoundResponse(documentText, packageName, upstreamSource);
          throw new InvalidUpstreamPackage(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidUpstreamPackageMetadata((object) upstreamSource.Location, (object) documentText), ex, documentText);
        }
        IDictionary<string, VersionMetadata> versions = packageMetadata.Versions;
        PartitionResults<VersionMetadata> partitionResults = versions != null ? versions.Values.Partition<VersionMetadata>((Predicate<VersionMetadata>) (v => NpmVersionUtils.TryParseNpmPackageVersion(v.Version, out SemanticVersion _))) : (PartitionResults<VersionMetadata>) null;
        List<INpmMetadataEntry> npmMetadataEntryList = (partitionResults != null ? partitionResults.MatchingPartition.Select<VersionMetadata, INpmMetadataEntry>((Func<VersionMetadata, INpmMetadataEntry>) (version => this.CreateMetadataEntry(version, upstreamSource))).ToList<INpmMetadataEntry>() : (List<INpmMetadataEntry>) null) ?? new List<INpmMetadataEntry>();
        partitionResults?.NonMatchingPartition.ForEach((Action<VersionMetadata>) (version => tracer.TraceError("Invalid semantic version '" + version.Version + "' from upstream '" + upstreamSource.Name + "'")));
        IGrouping<SemanticVersion, SemanticVersion> grouping = npmMetadataEntryList.Select<INpmMetadataEntry, SemanticVersion>((Func<INpmMetadataEntry, SemanticVersion>) (entry => entry.PackageIdentity.Version)).GroupBy<SemanticVersion, SemanticVersion>((Func<SemanticVersion, SemanticVersion>) (version => version)).Where<IGrouping<SemanticVersion, SemanticVersion>>((Func<IGrouping<SemanticVersion, SemanticVersion>, bool>) (group => group.Count<SemanticVersion>() > 1)).FirstOrDefault<IGrouping<SemanticVersion, SemanticVersion>>();
        if (grouping != null)
          throw new InvalidUpstreamSourceDuplicateVersionsException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidUpstreamSourceDuplicatePackages((object) upstreamSource.Name, (object) packageName.FullName, (object) grouping.Key));
        Dictionary<string, string> dictionary = packageMetadata.DistributionTags == null || !packageMetadata.DistributionTags.Any<KeyValuePair<string, string>>() ? new Dictionary<string, string>() : new Dictionary<string, string>(packageMetadata.DistributionTags);
        return new MetadataDocument<INpmMetadataEntry>(npmMetadataEntryList).UpdateSharedMetadata((object) new NpmSharedPackageMetadata()
        {
          Revision = packageMetadata.Revision,
          DistributionTags = (IDictionary<string, string>) dictionary
        });
      }
    }

    private INpmMetadataEntry CreateMetadataEntry(
      VersionMetadata version,
      UpstreamSource upstreamSource)
    {
      DateTime now = this.timeProvider.Now;
      byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) version));
      return (INpmMetadataEntry) new NpmMetadataEntry(PackagingCommitId.Empty, now, now, Guid.Empty, Guid.Empty, (IStorageId) new UpstreamStorageId(UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(upstreamSource)), 0L, bytes, version.Distribution.ShaSum, (PackageJsonOptions) null, (PackageManifest) null, (IEnumerable<UpstreamSourceInfo>) null, (IEnumerable<Guid>) null)
      {
        Deprecated = (this.propagateDeprecateSetting.Get() ? version.Deprecated : (string) null)
      };
    }

    private void ThrowIfNotFoundResponse(
      string responseString,
      NpmPackageName packageName,
      UpstreamSource upstreamSource)
    {
      IUnknownUpstreamResponse response;
      if (upstreamSource.UpstreamSourceType == UpstreamSourceType.Public && this.unknownResponseSerializer.TryDeserialize(responseString, out response) && response.IsNotFoundResponse)
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageNotFound((object) packageName.FullName, (object) upstreamSource.Location));
    }
  }
}
