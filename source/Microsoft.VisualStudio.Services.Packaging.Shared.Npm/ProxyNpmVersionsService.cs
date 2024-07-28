// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Npm.ProxyNpmVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Npm, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CEBE98F3-C321-41E5-B439-3F9CCC0A6151
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Npm.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi.Generated;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Npm
{
  public class ProxyNpmVersionsService : INpmVersionsService, IVssFrameworkService
  {
    public Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest)
    {
      NpmHttpClient npmHttpClient = this.NpmClient(requestContext);
      NpmPackagesBatchRequest batchRequest1 = batchRequest;
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
      return npmHttpClient.UpdatePackagesAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NpmPackagesBatchRequest batchRequest)
    {
      NpmHttpClient npmHttpClient = this.NpmClient(requestContext);
      NpmPackagesBatchRequest batchRequest1 = batchRequest;
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
      return npmHttpClient.UpdateRecycleBinPackagesAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private NpmHttpClient NpmClient(IVssRequestContext requestContext) => requestContext.GetClient<NpmHttpClient>();
  }
}
