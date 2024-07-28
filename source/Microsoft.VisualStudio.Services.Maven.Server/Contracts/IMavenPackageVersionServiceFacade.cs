// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Contracts.IMavenPackageVersionServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Contracts
{
  public interface IMavenPackageVersionServiceFacade
  {
    Task<MavenPackageFileResponse> GetPackageFile(
      FeedCore feedCore,
      string path,
      bool requireContent);

    Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion,
      bool showDeleted);

    Task<IMavenMetadataEntry> GetPackageVersionMetadataFromRecycleBin(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version);

    Task DeletePackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion);

    Task PermanentDeletePackageVersion(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version);

    Task RestorePackageVersionToFeed(
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      IRecycleBinPackageVersionDetails packageVersionDetails);

    Task UpdatePackageVersions(FeedCore feed, MavenPackagesBatchRequest batchRequest);
  }
}
