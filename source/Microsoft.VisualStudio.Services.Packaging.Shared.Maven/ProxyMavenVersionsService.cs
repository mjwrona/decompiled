// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.Maven.ProxyMavenVersionsService
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.Maven, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 455B8A99-BF20-4E00-8A1B-3189F2B441BB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.Maven.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Generated;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.Maven
{
  public class ProxyMavenVersionsService : IMavenVersionsService, IVssFrameworkService
  {
    public Task DeletePackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion)
    {
      MavenHttpClient mavenHttpClient = this.MavenClient(requestContext);
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
      string feed1 = id.ToString();
      string groupId1 = groupId;
      string artifactId1 = artifactId;
      string version = packageVersion;
      CancellationToken cancellationToken = new CancellationToken();
      return mavenHttpClient.PackageDeleteAsync(project2, feed1, groupId1, artifactId1, version, (object) null, cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.Maven.WebApi.Types.API.Package> GetPackageVersion(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      string artifactId,
      string packageVersion,
      bool showDeleted = false)
    {
      MavenHttpClient mavenHttpClient = this.MavenClient(requestContext);
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
      string feed1 = id.ToString();
      string groupId1 = groupId;
      string artifactId1 = artifactId;
      string version = packageVersion;
      bool? showDeleted1 = new bool?(showDeleted);
      CancellationToken cancellationToken = new CancellationToken();
      return mavenHttpClient.GetPackageVersionAsync(project2, feed1, groupId1, artifactId1, version, showDeleted1, cancellationToken: cancellationToken);
    }

    public Task UpdatePackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest)
    {
      MavenHttpClient mavenHttpClient = this.MavenClient(requestContext);
      MavenPackagesBatchRequest batchRequest1 = batchRequest;
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
      return mavenHttpClient.UpdatePackageVersionsAsync(batchRequest1, project2, feedId, (object) null, cancellationToken);
    }

    public Task UpdateRecycleBinPackageVersions(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPackagesBatchRequest batchRequest)
    {
      MavenHttpClient mavenHttpClient = this.MavenClient(requestContext);
      MavenPackagesBatchRequest batchRequest1 = batchRequest;
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
      string feed1 = id.ToString();
      CancellationToken cancellationToken = new CancellationToken();
      return mavenHttpClient.UpdateRecycleBinPackagesAsync(batchRequest1, project2, feed1, (object) null, cancellationToken);
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private MavenHttpClient MavenClient(IVssRequestContext requestContext) => requestContext.GetClient<MavenHttpClient>();
  }
}
