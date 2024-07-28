// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.PyPi.IPyPiVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.PyPi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF339F15-182F-4B87-9BC9-C042739E3DED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.PyPi.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.PyPi
{
  [DefaultServiceImplementation(typeof (ProxyPyPiVersionsService))]
  public interface IPyPiVersionsService : IVssFrameworkService
  {
    Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false);

    Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion);

    Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails);

    Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest);

    Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest);
  }
}
