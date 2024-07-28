// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.WebLinksUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class WebLinksUtility
  {
    public static string GetRepositoryUrl(
      IVssRequestContext requestContext,
      string repositoryName,
      string projectName)
    {
      string publicBaseUrl = GitServerUtils.GetPublicBaseUrl(requestContext);
      return GitServerUtils.GetRepositoryWebUrl(requestContext, publicBaseUrl, projectName, repositoryName);
    }

    public static string GetCommitRemoteUrl(
      IVssRequestContext requestContext,
      string repositoryName,
      string projectName,
      string commitId)
    {
      return GitUtils.AppendUrl(WebLinksUtility.GetRepositoryUrl(requestContext, repositoryName, projectName), "/commit/" + commitId);
    }
  }
}
