// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Npm.INpmVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Npm, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CEBE98F3-C321-41E5-B439-3F9CCC0A6151
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Npm.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Npm
{
  [DefaultServiceImplementation(typeof (ProxyNpmVersionsService))]
  public interface INpmVersionsService : IVssFrameworkService
  {
    Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest);

    Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest);
  }
}
