// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.NuGet.INuGetVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.NuGet, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C6522AD-7299-43B3-A0C7-3C61C0ADE1EA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.NuGet.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.NuGet
{
  [DefaultServiceImplementation(typeof (ProxyNuGetVersionsService))]
  public interface INuGetVersionsService : IVssFrameworkService
  {
    Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false);

    Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> DeletePackageVersion(
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
      NuGetPackagesBatchRequest batchRequest);

    Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NuGetPackagesBatchRequest batchRequest);
  }
}
