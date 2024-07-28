// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.SameCollectionUpstreamMavenClient
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class SameCollectionUpstreamMavenClient : IUpstreamMavenClient
  {
    private readonly IFeedService feedServiceFacade;
    private readonly IMavenPackageVersionServiceFacade packageVersionService;
    private readonly UpstreamSource upstreamSource;
    private readonly IConverter<Stream, IList<string>> xmlMetadataToVersionListConverter;
    private readonly IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> versionsToProvideDownstreamsHandler;

    public SameCollectionUpstreamMavenClient(
      IFeedService feedServiceFacade,
      IMavenPackageVersionServiceFacade packageVersionService,
      UpstreamSource upstreamSource,
      IConverter<Stream, IList<string>> xmlMetadataToVersionListConverter,
      IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> versionsToProvideDownstreamsHandler)
    {
      this.feedServiceFacade = feedServiceFacade;
      this.packageVersionService = packageVersionService;
      this.upstreamSource = upstreamSource;
      this.xmlMetadataToVersionListConverter = xmlMetadataToVersionListConverter;
      this.versionsToProvideDownstreamsHandler = versionsToProvideDownstreamsHandler;
    }

    private FeedCore Feed => this.feedServiceFacade.GetFeed(this.upstreamSource.GetProjectId() ?? Guid.Empty, this.upstreamSource.GetFullyQualifiedFeedId());

    public async Task<Stream> GetFileAsync(IMavenArtifactFilePath filePath) => (await this.packageVersionService.GetPackageFile(this.Feed, filePath.FullName, true)).Content;

    public async Task<IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> GetPackageVersionsAsync(
      MavenPackageName packageName)
    {
      return await this.versionsToProvideDownstreamsHandler.Handle((IPackageNameRequest<MavenPackageName>) new PackageNameRequest<MavenPackageName>(this.Feed, packageName));
    }

    public async Task<IEnumerable<UpstreamSourceInfo>> GetSourceChainAsync(
      IMavenFullyQualifiedFilePath filePath)
    {
      return (await this.packageVersionService.GetPackageVersion(this.Feed, filePath.GroupId, filePath.ArtifactId, filePath.Version, true)).SourceChain;
    }
  }
}
