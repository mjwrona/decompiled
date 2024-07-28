// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.NuGet.ProxyNuGetVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.NuGet, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C6522AD-7299-43B3-A0C7-3C61C0ADE1EA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.NuGet.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Generated;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.NuGet
{
  public class ProxyNuGetVersionsService : INuGetVersionsService, IVssFrameworkService
  {
    public Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion)
    {
      NuGetHttpClient client = this.NuGetClient(requestContext);
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
      string packageName1 = packageName;
      string packageVersion1 = packageVersion;
      CancellationToken cancellationToken = new CancellationToken();
      return client.DeletePackageVersionAsync(project2, feedId, packageName1, packageVersion1, (object) null, cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      NuGetHttpClient client = this.NuGetClient(requestContext);
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
      string packageName1 = packageName;
      string packageVersion1 = packageVersion;
      bool? showDeleted1 = new bool?(showDeleted);
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetPackageVersionAsync(project2, feedId, packageName1, packageVersion1, showDeleted1, cancellationToken: cancellationToken);
    }

    public Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      NuGetHttpClient client = this.NuGetClient(requestContext);
      PackageVersionDetails packageVersionDetails1 = packageVersionDetails;
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
      string packageName1 = packageName;
      string packageVersion1 = packageVersion;
      CancellationToken cancellationToken = new CancellationToken();
      return client.UpdatePackageVersionAsync(packageVersionDetails1, project2, feedId, packageName1, packageVersion1, (object) null, cancellationToken);
    }

    public Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NuGetPackagesBatchRequest batchRequest)
    {
      NuGetHttpClient client = this.NuGetClient(requestContext);
      NuGetPackagesBatchRequest batchRequest1 = batchRequest;
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
      return client.UpdatePackageVersionsAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      NuGetPackagesBatchRequest batchRequest)
    {
      NuGetHttpClient client = this.NuGetClient(requestContext);
      NuGetPackagesBatchRequest batchRequest1 = batchRequest;
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
      return client.UpdateRecycleBinPackageVersionsAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private NuGetHttpClient NuGetClient(IVssRequestContext requestContext) => requestContext.GetClient<NuGetHttpClient>();
  }
}
