// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.PyPi.ProxyPyPiVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.PyPi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF339F15-182F-4B87-9BC9-C042739E3DED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.PyPi.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Generated.Api;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.PyPi
{
  public class ProxyPyPiVersionsService : IPyPiVersionsService, IVssFrameworkService
  {
    public Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion)
    {
      PyPiApiHttpClient pyPiApiHttpClient = this.PyPiApiClient(requestContext);
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
      return pyPiApiHttpClient.DeletePackageVersionAsync(project2, feedId, packageName1, packageVersion1, (object) null, cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      PyPiApiHttpClient pyPiApiHttpClient = this.PyPiApiClient(requestContext);
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
      return pyPiApiHttpClient.GetPackageVersionAsync(project2, feedId, packageName1, packageVersion1, showDeleted1, cancellationToken: cancellationToken);
    }

    public Task UpdatePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      PyPiApiHttpClient pyPiApiHttpClient = this.PyPiApiClient(requestContext);
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
      PackageVersionDetails packageVersionDetails1 = packageVersionDetails;
      CancellationToken cancellationToken = new CancellationToken();
      return pyPiApiHttpClient.UpdatePackageVersionAsync(project2, feedId, packageName1, packageVersion1, packageVersionDetails1, cancellationToken: cancellationToken);
    }

    public Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest)
    {
      PyPiApiHttpClient pyPiApiHttpClient = this.PyPiApiClient(requestContext);
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
      PyPiPackagesBatchRequest batchRequest1 = batchRequest;
      CancellationToken cancellationToken = new CancellationToken();
      return pyPiApiHttpClient.UpdatePackageVersionsAsync(project2, feedId, batchRequest1, cancellationToken: cancellationToken);
    }

    public Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      PyPiPackagesBatchRequest batchRequest)
    {
      PyPiApiHttpClient pyPiApiHttpClient = this.PyPiApiClient(requestContext);
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
      PyPiPackagesBatchRequest batchRequest1 = batchRequest;
      CancellationToken cancellationToken = new CancellationToken();
      return pyPiApiHttpClient.UpdateRecycleBinPackageVersionsAsync(project2, feedId, batchRequest1, cancellationToken: cancellationToken);
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private PyPiApiHttpClient PyPiApiClient(IVssRequestContext requestContext) => requestContext.GetClient<PyPiApiHttpClient>();
  }
}
