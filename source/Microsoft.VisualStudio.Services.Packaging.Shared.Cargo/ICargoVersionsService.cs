// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Cargo.ICargoVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Cargo, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1CE16B2F-3944-4B39-B1D1-CBF6C7BD5C32
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Cargo.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Cargo
{
  [DefaultServiceImplementation(typeof (ProxyCargoVersionsService))]
  public interface ICargoVersionsService : IVssFrameworkService
  {
    Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      CargoPackagesBatchRequest batchRequest);

    Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      CargoPackagesBatchRequest batchRequest);
  }
}
