// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPushUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitPushUtility
  {
    public static GitPush GetPush(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      UrlHelper urlHelper,
      int pushId,
      int includeCommits,
      bool includeRefUpdates)
    {
      GitPush gitPush1 = (GitPush) null;
      ITeamFoundationGitCommitService service1 = requestContext.GetService<ITeamFoundationGitCommitService>();
      TfsGitPushMetadata tfsGitPushMetadata = service1.GetPushDataForPushIds(requestContext, repository.Key, new int[1]
      {
        pushId
      }, (includeRefUpdates ? 1 : 0) != 0).FirstOrDefault<TfsGitPushMetadata>();
      if (tfsGitPushMetadata != null)
      {
        if (tfsGitPushMetadata.RepoId != repository.Key.RepoId)
          return (GitPush) null;
        ITeamFoundationIdentityService service2 = requestContext.GetService<ITeamFoundationIdentityService>();
        Guid pusherId = tfsGitPushMetadata.PusherId;
        IVssRequestContext requestContext1 = requestContext;
        Guid[] teamFoundationIds = new Guid[1]{ pusherId };
        TeamFoundationIdentity readIdentity = service2.ReadIdentities(requestContext1, teamFoundationIds)[0];
        GitPush gitPush2 = new GitPush();
        gitPush2.Date = tfsGitPushMetadata.PushTime;
        gitPush2.PushedBy = readIdentity.ToIdentityRef(requestContext);
        gitPush2.PushId = tfsGitPushMetadata.PushId;
        gitPush2.Repository = repository.ToWebApiItem(requestContext);
        gitPush2.Url = urlHelper.RestLink(requestContext, GitWebApiConstants.PushesLocationId, (object) new
        {
          repositoryId = repository.Key.RepoId,
          PushId = tfsGitPushMetadata.PushId
        });
        gitPush1 = gitPush2;
        if (tfsGitPushMetadata.RefLog != null)
          gitPush1.RefUpdates = (IEnumerable<GitRefUpdate>) tfsGitPushMetadata.RefLog.Select<TfsGitRefLogEntry, GitRefUpdate>((Func<TfsGitRefLogEntry, GitRefUpdate>) (x => x.ToRefUpdate())).ToList<GitRefUpdate>();
        if (includeCommits > 0)
        {
          List<GitCommitRef> gitCommitRefList = new List<GitCommitRef>();
          GitCommitTranslator commitTranslator = new GitCommitTranslator(requestContext, repository.Key, urlHelper);
          foreach (TfsGitCommitMetadata metadata in (IEnumerable<TfsGitCommitMetadata>) service1.GetPushCommitsByPushId(requestContext, repository, pushId, new int?(0), new int?(includeCommits)))
            gitCommitRefList.Add(commitTranslator.ToGitCommitShallow(metadata, false));
          gitPush1.Commits = (IEnumerable<GitCommitRef>) gitCommitRefList;
        }
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(repository.Key);
        gitPush1.Links = gitPush1.GetPushReferenceLinks(requestContext, urlHelper, repository.Key, repositoryReadOnly);
        gitPush1.SetSecuredObject(repositoryReadOnly);
      }
      return gitPush1;
    }
  }
}
