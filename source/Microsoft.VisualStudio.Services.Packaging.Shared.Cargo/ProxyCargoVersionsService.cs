// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Cargo.ProxyCargoVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Cargo, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1CE16B2F-3944-4B39-B1D1-CBF6C7BD5C32
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Cargo.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Generated;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Cargo
{
  public class ProxyCargoVersionsService : ICargoVersionsService, IVssFrameworkService
  {
    public async Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      CargoPackagesBatchRequest batchRequest)
    {
      CargoApiHttpClient client = requestContext.GetClient<CargoApiHttpClient>();
      CargoPackagesBatchRequest batchRequest1 = batchRequest;
      ProjectReference project1 = feed.Project;
      Guid id;
      string project2;
      if ((object) project1 == null)
      {
        project2 = (string) null;
      }
      else
      {
        id = project1.Id;
        project2 = id.ToString();
      }
      id = feed.Id;
      string feedId = id.ToString();
      CancellationToken cancellationToken = new CancellationToken();
      await client.UpdatePackageVersionsAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public async Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      CargoPackagesBatchRequest batchRequest)
    {
      CargoApiHttpClient client = requestContext.GetClient<CargoApiHttpClient>();
      CargoPackagesBatchRequest batchRequest1 = batchRequest;
      ProjectReference project1 = feed.Project;
      Guid id;
      string project2;
      if ((object) project1 == null)
      {
        project2 = (string) null;
      }
      else
      {
        id = project1.Id;
        project2 = id.ToString();
      }
      id = feed.Id;
      string feedId = id.ToString();
      CancellationToken cancellationToken = new CancellationToken();
      await client.UpdateRecycleBinPackageVersionsAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
