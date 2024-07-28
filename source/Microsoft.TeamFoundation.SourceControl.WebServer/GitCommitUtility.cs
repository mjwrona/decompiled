// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCommitUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class GitCommitUtility
  {
    internal const int c_defaultTop = 100;

    internal static Sha1Id? ParseSearchObjectId(string value) => !string.IsNullOrEmpty(value) ? new Sha1Id?(GitCommitUtility.ParseSha1Id(value)) : new Sha1Id?();

    internal static Sha1Id ParseSha1Id(string sha1Id)
    {
      try
      {
        return new Sha1Id(sha1Id);
      }
      catch (ArgumentException ex) when (ex.ExpectedExceptionFilter("git"))
      {
        throw;
      }
    }

    public static GitCommitRef CreateMinimalGitCommitRef(
      IVssRequestContext requestContext,
      Sha1Id commitId,
      RepoKey repoKey,
      UrlHelper urlHelper = null)
    {
      string commitId1 = commitId.ToString();
      return new GitCommitRef()
      {
        CommitId = commitId1,
        Url = GitReferenceLinksUtility.GetCommitRefUrl(requestContext, repoKey, commitId1, urlHelper)
      };
    }
  }
}
