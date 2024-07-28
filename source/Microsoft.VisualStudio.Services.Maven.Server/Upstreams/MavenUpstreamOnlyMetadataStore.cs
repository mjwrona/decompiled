// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.MavenUpstreamOnlyMetadataStore
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class MavenUpstreamOnlyMetadataStore : 
    IUpstreamMetadataService<MavenPackageName, MavenPackageVersion, MavenPackageIdentity, IMavenMetadataEntry>
  {
    private readonly UpstreamSource source;
    private readonly IUpstreamMavenClient upstreamMavenClient;
    private readonly ITimeProvider timeProvider;

    public MavenUpstreamOnlyMetadataStore(
      UpstreamSource source,
      IUpstreamMavenClient upstreamMavenClient,
      ITimeProvider timeProvider)
    {
      this.source = source;
      this.upstreamMavenClient = upstreamMavenClient;
      this.timeProvider = timeProvider;
    }

    public Task<IEnumerable<IMavenMetadataEntry>> UpdateEntriesWithTransientStateAsync(
      IFeedRequest downstreamFeedRequest,
      MavenPackageName packageName,
      IEnumerable<IMavenMetadataEntry> entries,
      ICommitLogEntryHeader fakeCommitHeader)
    {
      return Task.FromResult<IEnumerable<IMavenMetadataEntry>>(Enumerable.Empty<IMavenMetadataEntry>());
    }

    public Task<object> GetPackageNameMetadata(
      IFeedRequest downstreamFeedRequest,
      MavenPackageName name)
    {
      return (Task<object>) null;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> GetPackageVersionsAsync(
      IFeedRequest downstreamFeedRequest,
      MavenPackageName packageName)
    {
      return await this.upstreamMavenClient.GetPackageVersionsAsync(packageName);
    }

    public Task<IEnumerable<IMavenMetadataEntry>> GetPackageVersionStatesAsync(
      IFeedRequest downstreamFeedRequest,
      MavenPackageName name,
      IEnumerable<MavenPackageVersion> versions)
    {
      DateTime now = this.timeProvider.Now;
      List<UpstreamSourceInfo> sourceChain = new List<UpstreamSourceInfo>()
      {
        this.source.ToUpstreamSourceInfo()
      };
      return Task.FromResult<IEnumerable<IMavenMetadataEntry>>((IEnumerable<IMavenMetadataEntry>) versions.Select<MavenPackageVersion, IMavenMetadataEntry>((Func<MavenPackageVersion, IMavenMetadataEntry>) (version => this.CreateCachedUpstreamMetadataEntry(name, version, now, (IEnumerable<UpstreamSourceInfo>) sourceChain))).ToList<IMavenMetadataEntry>());
    }

    private IMavenMetadataEntry CreateCachedUpstreamMetadataEntry(
      MavenPackageName name,
      MavenPackageVersion version,
      DateTime now,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return (IMavenMetadataEntry) new MavenMetadataEntry(new MavenPackageIdentity(name, version), PackagingCommitId.Empty, now, now, Guid.Empty, Guid.Empty, sourceChain, (IEnumerable<MavenPackageFileNew>) null, (byte[]) null);
    }
  }
}
