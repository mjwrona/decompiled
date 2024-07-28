// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoNameKey
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Routing;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public struct RepoNameKey
  {
    public readonly string ProjectName;
    public readonly string RepositoryName;

    public RepoNameKey(string projectName, string repositoryName)
    {
      this.ProjectName = projectName;
      this.RepositoryName = repositoryName;
    }

    public static RepoNameKey FromRequest(HttpRequestBase request) => new RepoNameKey(request.RequestContext.RouteData.GetRouteValue<string>("project"), request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName"));

    public bool IsValid => !string.IsNullOrEmpty(this.ProjectName) && !string.IsNullOrEmpty(this.RepositoryName);
  }
}
