// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.CrossCollectionUpstreamMavenClient
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Client.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class CrossCollectionUpstreamMavenClient : IUpstreamMavenClient
  {
    private readonly string feedId;
    private readonly Guid aadTenantId;
    private readonly string projectId;
    private readonly InternalMavenHttpClient internalMavenHttpClient;

    public CrossCollectionUpstreamMavenClient(
      Guid? projectId,
      string feedId,
      Guid aadTenantId,
      InternalMavenHttpClient internalMavenHttpClient)
    {
      this.projectId = projectId?.ToString();
      this.feedId = feedId;
      this.aadTenantId = aadTenantId;
      this.internalMavenHttpClient = internalMavenHttpClient;
    }

    public Task<Stream> GetFileAsync(IMavenArtifactFilePath filePath) => this.internalMavenHttpClient.GetPackageFileAsync(this.projectId, this.feedId, filePath.FullName, this.aadTenantId);

    public async Task<IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> GetPackageVersionsAsync(
      MavenPackageName packageName)
    {
      return (IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>) (await this.internalMavenHttpClient.GetPackageVersionsExposedToDownstreamsAsync(this.projectId, this.feedId, packageName.GroupId, packageName.ArtifactId, this.aadTenantId)).VersionInfo.Select<RawVersionWithSourceChain, VersionWithSourceChain<MavenPackageVersion>>((Func<RawVersionWithSourceChain, VersionWithSourceChain<MavenPackageVersion>>) (x => VersionWithSourceChain.FromInternalSource<MavenPackageVersion>(new MavenPackageVersion(x.NormalizedVersion), x.SourceChain))).ToList<VersionWithSourceChain<MavenPackageVersion>>();
    }

    public async Task<IEnumerable<UpstreamSourceInfo>> GetSourceChainAsync(
      IMavenFullyQualifiedFilePath filePath)
    {
      return (await this.internalMavenHttpClient.GetPackageVersionAsync(this.projectId, this.feedId, filePath.GroupId, filePath.ArtifactId, filePath.Version, this.aadTenantId, new bool?(true))).SourceChain;
    }
  }
}
