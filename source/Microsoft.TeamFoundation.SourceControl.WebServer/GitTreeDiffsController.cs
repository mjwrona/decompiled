// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitTreeDiffsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(true)]
  public sealed class GitTreeDiffsController : GitApiController
  {
    private const int c_defaultMaxChanges = 100;
    private const string TraceLayer = "GitTreeDiffsController";

    [HttpGet]
    [ClientLocationId("E264EF02-4E92-4CFC-A4B1-5E71894D7B31")]
    [ClientResponseType(typeof (GitTreeDiffResponse), null, null)]
    public HttpResponseMessage GetTreeDiffs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      string baseId = null,
      string targetId = null,
      [FromUri(Name = "$top")] int? top = null,
      string continuationToken = null)
    {
      Sha1Id treeish = !string.IsNullOrWhiteSpace(baseId) && !string.IsNullOrWhiteSpace(targetId) ? GitCommitUtility.ParseSha1Id(baseId) : throw new ArgumentException(Resources.Get("TreeDiffBaseTargetNotSpecified")).Expected("git");
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(targetId);
      Sha1Id objectId1;
      Sha1Id objectId2;
      DiffContinuationToken nextToken;
      IEnumerable<TfsGitDiffEntry> diffEntries;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitTree gitTree1 = this.ResolveToGitTree(tfsGitRepository, treeish);
        objectId1 = gitTree1.ObjectId;
        TfsGitTree gitTree2 = this.ResolveToGitTree(tfsGitRepository, sha1Id);
        objectId2 = gitTree2.ObjectId;
        DiffContinuationToken token = (DiffContinuationToken) null;
        string parseFailureReason;
        if (continuationToken != null && !DiffContinuationToken.TryParseContinuationToken(objectId1, objectId2, continuationToken, out token, out parseFailureReason))
          throw new ArgumentException(Resources.Format("InvalidContinuationTokenWithReason", (object) continuationToken, (object) parseFailureReason)).Expected("git");
        int top1 = Math.Max(0, top ?? 100);
        try
        {
          diffEntries = TfsGitDiffHelper.DiffTreesSegmented(gitTree1, gitTree2, token, top1, out nextToken);
        }
        catch (GitInvalidDiffContinuationTokenException ex)
        {
          throw new ArgumentException(Resources.Format("InvalidContinuationTokenWithReason", (object) continuationToken, (object) ex.Message)).Expected("git");
        }
      }
      GitTreeDiff gitTreeDiff = new GitTreeDiff()
      {
        BaseTreeId = objectId1.ToString(),
        TargetTreeId = objectId2.ToString(),
        DiffEntries = diffEntries.ToGitTreeDiffEntries()
      };
      if (!MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request))
        gitTreeDiff.Url = this.Url.RestLink(this.TfsRequestContext, GitWebApiConstants.TreeDiffsLocationId, (object) new
        {
          repositoryId = repositoryId,
          baseTreeId = gitTreeDiff.BaseTreeId,
          targetTreeId = gitTreeDiff.TargetTreeId
        });
      HttpResponseMessage response = this.Request.CreateResponse<GitTreeDiff>(HttpStatusCode.OK, gitTreeDiff);
      if (nextToken != null)
        response.Headers.Add("x-ms-continuationtoken", nextToken.ToString());
      return response;
    }

    private TfsGitTree ResolveToGitTree(ITfsGitRepository repository, Sha1Id treeish)
    {
      switch (repository.LookupObject(treeish))
      {
        case TfsGitTree gitTree:
          return gitTree;
        case TfsGitCommit tfsGitCommit:
          return tfsGitCommit.GetTree();
        default:
          throw new ArgumentException(Resources.Format("UnresolvableToTree", (object) treeish)).Expected("git");
      }
    }
  }
}
