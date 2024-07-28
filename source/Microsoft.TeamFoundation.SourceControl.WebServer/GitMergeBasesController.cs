// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitMergeBasesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "mergeBases")]
  public class GitMergeBasesController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("7CF2ABB6-C964-4F7E-9872-F78C66E72E9C")]
    [ClientResponseType(typeof (IEnumerable<GitCommitRef>), null, null)]
    public HttpResponseMessage GetMergeBases(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      string commitId,
      string otherCommitId,
      Guid? otherCollectionId = null,
      Guid? otherRepositoryId = null)
    {
      Sha1Id sha1Id1 = GitCommitUtility.ParseSha1Id(commitId);
      Sha1Id sha1Id2 = GitCommitUtility.ParseSha1Id(otherCommitId);
      if (otherCollectionId.HasValue)
      {
        if (!otherRepositoryId.HasValue)
          throw new ArgumentException(nameof (otherRepositoryId)).Expected("git");
        if (otherCollectionId.Value != Guid.Empty && otherCollectionId.Value != this.TfsRequestContext.ServiceHost.InstanceId)
          throw new ArgumentException(Resources.Get("CrossCollectionMergeBaseUnsupported")).Expected("git");
      }
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryNameOrId, this.ProjectInfo?.Id.ToString()))
      {
        if (!otherRepositoryId.HasValue)
        {
          ITeamFoundationGitCommitService service = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>();
          if (this.TfsRequestContext.IsFeatureEnabled("Git.MergeBase.ReturnAllBases"))
          {
            IEnumerable<TfsGitCommit> mergeBases = service.GetMergeBases(this.TfsRequestContext, tfsGitRepository, sha1Id1, sha1Id2);
            if (mergeBases != null)
              return this.Request.CreateResponse<GitCommitRef[]>(HttpStatusCode.OK, mergeBases.Select<TfsGitCommit, GitCommitRef>((Func<TfsGitCommit, GitCommitRef>) (mergebase => new GitCommitRef()
              {
                CommitId = mergebase.ObjectId.ToString()
              })).ToArray<GitCommitRef>());
          }
          else
          {
            TfsGitCommit mergeBase = service.GetMergeBase(this.TfsRequestContext, tfsGitRepository, sha1Id1, sha1Id2);
            if (mergeBase != null)
              return this.Request.CreateResponse<GitCommitRef[]>(HttpStatusCode.OK, new GitCommitRef[1]
              {
                new GitCommitRef()
                {
                  CommitId = mergeBase.ObjectId.ToString()
                }
              });
          }
        }
        else
        {
          using (ITfsGitRepository repositoryById = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.TfsRequestContext, otherRepositoryId.Value))
          {
            if (this.TfsRequestContext.IsFeatureEnabled("Git.MergeBase.ReturnAllBases"))
            {
              IEnumerable<Sha1Id> maybeMergeBases;
              if (this.TfsRequestContext.GetService<IGitForkService>().TryCalculateMergeBases(tfsGitRepository, sha1Id1, repositoryById, sha1Id2, out maybeMergeBases))
                return this.Request.CreateResponse<GitCommitRef[]>(HttpStatusCode.OK, maybeMergeBases.Select<Sha1Id, GitCommitRef>((Func<Sha1Id, GitCommitRef>) (mergebase => new GitCommitRef()
                {
                  CommitId = mergebase.ToString()
                })).ToArray<GitCommitRef>());
            }
            else
            {
              Sha1Id maybeMergeBase;
              if (this.TfsRequestContext.GetService<IGitForkService>().TryCalculateMergeBase(tfsGitRepository, sha1Id1, repositoryById, sha1Id2, out maybeMergeBase))
                return this.Request.CreateResponse<GitCommitRef[]>(HttpStatusCode.OK, new GitCommitRef[1]
                {
                  new GitCommitRef() { CommitId = maybeMergeBase.ToString() }
                });
            }
          }
        }
        return this.Request.CreateResponse<IEnumerable<GitCommitRef>>(HttpStatusCode.OK, Enumerable.Empty<GitCommitRef>());
      }
    }
  }
}
