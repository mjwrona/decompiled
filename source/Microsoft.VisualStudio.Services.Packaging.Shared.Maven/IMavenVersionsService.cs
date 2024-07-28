// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Maven.IMavenVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Maven, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 455B8A99-BF20-4E00-8A1B-3189F2B441BB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Maven.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Maven
{
  [DefaultServiceImplementation(typeof (ProxyMavenVersionsService))]
  public interface IMavenVersionsService : IVssFrameworkService
  {
    Task DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion);

    Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion,
      bool showDeleted);

    Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest);

    Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest);
  }
}
