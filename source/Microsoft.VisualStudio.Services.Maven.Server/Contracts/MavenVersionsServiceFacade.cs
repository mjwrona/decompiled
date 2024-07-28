// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Contracts.MavenVersionsServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Contracts
{
  internal class MavenVersionsServiceFacade : IMavenPackageVersionServiceFacade
  {
    private readonly IVssRequestContext requestContext;

    public MavenVersionsServiceFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    private IMavenPackageVersionService Service => this.requestContext.GetService<IMavenPackageVersionService>();

    public Task DeletePackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion)
    {
      return this.Service.DeletePackageVersion(this.requestContext, feed, groupId, artifactId, packageVersion);
    }

    public Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion,
      bool showDeleted)
    {
      return this.Service.GetPackageVersion(this.requestContext, feed, groupId, artifactId, packageVersion, showDeleted);
    }

    public Task UpdatePackageVersions(FeedCore feed, MavenPackagesBatchRequest batchRequest) => this.Service.UpdatePackageVersions(this.requestContext, feed, batchRequest);

    public Task<MavenPackageFileResponse> GetPackageFile(
      FeedCore feedCore,
      string path,
      bool requireContent)
    {
      return this.Service.GetPackageFile(this.requestContext, feedCore, path, requireContent);
    }

    public Task<IMavenMetadataEntry> GetPackageVersionMetadataFromRecycleBin(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version)
    {
      return this.Service.GetPackageVersionMetadataFromRecycleBin(this.requestContext, feed, groupId, artifactId, version);
    }

    public Task PermanentDeletePackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version)
    {
      return this.Service.PermanentDeletePackageVersion(this.requestContext, feed, groupId, artifactId, version);
    }

    public Task RestorePackageVersionToFeed(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      IRecycleBinPackageVersionDetails packageVersionDetails)
    {
      return this.Service.RestorePackageVersionToFeed(this.requestContext, feed, groupId, artifactId, version, packageVersionDetails);
    }
  }
}
