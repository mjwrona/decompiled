// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Contracts.IMavenPackageVersionService
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.Shared.Maven;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Contracts
{
  [DefaultServiceImplementation(typeof (MavenVersionsService))]
  public interface IMavenPackageVersionService : IMavenVersionsService, IVssFrameworkService
  {
    Task<MavenPackageFileResponse> GetPackageFile(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string path,
      bool requireContent,
      bool streamContent = true);

    Task<IMavenMetadataEntry> GetPackageVersionMetadataFromRecycleBin(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version);

    Task RestorePackageVersionToFeed(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      IRecycleBinPackageVersionDetails packageVersionDetails);

    Task PermanentDeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version);

    Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string version,
      PackageVersionDetails packageVersionDetails);
  }
}
