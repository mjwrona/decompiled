// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamOnlyMetadataStore
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamOnlyMetadataStore : 
    IUpstreamMetadataService<
    #nullable disable
    NpmPackageName, SemanticVersion, NpmPackageIdentity, INpmMetadataEntry>
  {
    private readonly UpstreamSource source;
    private readonly IUpstreamNpmClient upstreamNpmClient;
    private readonly IOrgLevelPackagingSetting<bool> propagateDeprecateSetting;

    public NpmUpstreamOnlyMetadataStore(
      UpstreamSource source,
      IUpstreamNpmClient upstreamNpmClient,
      IOrgLevelPackagingSetting<bool> propagateDeprecateSetting)
    {
      this.source = source;
      this.upstreamNpmClient = upstreamNpmClient;
      this.propagateDeprecateSetting = propagateDeprecateSetting;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<SemanticVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      NpmPackageName packageName)
    {
      return await this.upstreamNpmClient.GetVersionList(packageName);
    }

    public async Task<IEnumerable<INpmMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      NpmPackageName name,
      IEnumerable<SemanticVersion> versions)
    {
      HashSet<IPackageVersion> versionSet = ((IEnumerable<IPackageVersion>) versions).ToHashSet<IPackageVersion>((IEqualityComparer<IPackageVersion>) PackageVersionComparer.NormalizedVersion);
      return (await this.upstreamNpmClient.GetPackageRegistrationAsync(name)).Entries.Where<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (x => versionSet.Contains((IPackageVersion) x.PackageIdentity.Version)));
    }

    public async Task<IEnumerable<INpmMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      NpmPackageName packageName,
      IEnumerable<INpmMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      if (!this.propagateDeprecateSetting.Get())
        return Enumerable.Empty<INpmMetadataEntry>();
      MetadataDocument<INpmMetadataEntry> registrationAsync = await this.upstreamNpmClient.GetPackageRegistrationAsync(packageName);
      List<INpmMetadataEntry> npmMetadataEntryList = new List<INpmMetadataEntry>();
      foreach (INpmMetadataEntry entry1 in entries)
      {
        INpmMetadataEntry entry = entry1;
        if (!entry.PackageIdentity.Name.Equals(packageName))
          throw new InvalidOperationException("UpdateEntriesWithTransientStateAsync can only handle entries from a single package name");
        if (entry.IsLocal)
          throw new InvalidOperationException("Do not send local entries to UpdateEntriesWithTransientStateAsync");
        INpmMetadataEntry npmMetadataEntry = registrationAsync.Entries.FirstOrDefault<INpmMetadataEntry>((Func<INpmMetadataEntry, bool>) (x => x.PackageIdentity.Version == entry.PackageIdentity.Version));
        if (npmMetadataEntry != null)
        {
          INpmMetadataEntryWriteable writeable = entry.CreateWriteable(fakeCommitHeader);
          bool flag = false;
          string deprecated1 = npmMetadataEntry.Deprecated;
          string deprecated2 = entry.Deprecated;
          if (deprecated1 != deprecated2)
          {
            writeable.Deprecated = deprecated1;
            flag = true;
          }
          if (flag)
          {
            writeable.SetPackageJsonBytes(npmMetadataEntry.PackageJsonContentBytes.Content, npmMetadataEntry.PackageJsonContentBytes.AreBytesCompressed);
            writeable.PackageJsonOptions = npmMetadataEntry.PackageJsonOptions;
            writeable.PackageManifest = npmMetadataEntry.PackageManifest;
            writeable.PackageSha1Sum = npmMetadataEntry.PackageSha1Sum;
            npmMetadataEntryList.Add((INpmMetadataEntry) writeable);
          }
        }
      }
      return (IEnumerable<INpmMetadataEntry>) npmMetadataEntryList;
    }

    public async Task<object> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      NpmPackageName name)
    {
      return (await this.upstreamNpmClient.GetPackageRegistrationAsync(name)).Properties.NameMetadata;
    }
  }
}
