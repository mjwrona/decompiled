// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitCommitDiffsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Diffs")]
  public class GitCommitDiffsController : GitApiController
  {
    private const int c_defaultMaxChanges = 100;

    [HttpGet]
    [ClientLocationId("615588D5-C0C7-4B88-88F8-E625306446E8")]
    [ClientExample("GET__git_repositories__repositoryId__diffs_commits_baseVersionType-_baseVersionType_.json", "Between commit IDs", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__diffs_commits_targetVersion-_targetVersion__baseVersion-_baseVersion_.json", "Between branches", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__diffs_commits_targetVersion-_targetVersion__baseVersion-_baseVersion___top-_top___skip-_skip_.json", "A page at a time", null, null)]
    [PublicProjectRequestRestrictions]
    public GitCommitDiffs GetCommitDiffs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      bool? diffCommonCommit = null,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder)), ClientParameterType(typeof (GitBaseVersionDescriptor), false), ClientFlattenModelQueryParameterNames] GitVersionDescriptor baseVersionDescriptor = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder)), ClientParameterType(typeof (GitTargetVersionDescriptor), false), ClientFlattenModelQueryParameterNames] GitVersionDescriptor targetVersionDescriptor = null)
    {
      baseVersionDescriptor = baseVersionDescriptor ?? new GitVersionDescriptor();
      targetVersionDescriptor = targetVersionDescriptor ?? new GitVersionDescriptor();
      bool valueOrDefault = diffCommonCommit.GetValueOrDefault(true);
      int maxNumberOfChanges = Math.Max(0, top ?? 100);
      int skipCount = Math.Max(0, skip.GetValueOrDefault());
      GitCommitDiffs commitDiffs = new GitCommitDiffs();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitCommitService service = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>();
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        TfsGitCommit commitFromVersion = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, baseVersionDescriptor);
        TfsGitCommit targetCommit = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, targetVersionDescriptor);
        commitDiffs.BaseCommit = commitFromVersion.ToString();
        commitDiffs.TargetCommit = targetCommit.ToString();
        TfsGitCommit mergeBase = service.GetMergeBase(this.TfsRequestContext, tfsGitRepository, commitFromVersion.ObjectId, targetCommit.ObjectId);
        if (mergeBase != null)
        {
          commitDiffs.CommonCommit = mergeBase.ObjectId.ToString();
          List<TfsGitCommitLineageDiff> list = service.DiffCommitLineagesAgainstCommits(this.TfsRequestContext, tfsGitRepository, commitFromVersion.ObjectId, (IEnumerable<Sha1Id>) new List<Sha1Id>()
          {
            targetCommit.ObjectId
          }).ToList<TfsGitCommitLineageDiff>();
          if (list.Count == 1)
          {
            commitDiffs.AheadCount = new int?(list[0].AheadCount);
            commitDiffs.BehindCount = new int?(list[0].BehindCount);
          }
        }
        if (maxNumberOfChanges > 0)
        {
          TfsGitCommit tfsGitCommit = !(mergeBase != null & valueOrDefault) ? commitFromVersion : mergeBase;
          bool allChangesIncluded;
          GitCommitChanges gitCommitChanges = TfsGitDiffHelper.DiffTrees(tfsGitRepository, tfsGitCommit.GetTree(), targetCommit.GetTree(), true).Select<TfsGitDiffEntry, TfsGitCommitChangeWithId>((Func<TfsGitDiffEntry, TfsGitCommitChangeWithId>) (x => new TfsGitCommitChangeWithId(targetCommit.ObjectId, x))).ToGitCommitChanges(this.TfsRequestContext, tfsGitRepository.Key, maxNumberOfChanges, skipCount, (ISecuredObject) null, out allChangesIncluded);
          commitDiffs.Changes = gitCommitChanges.Changes;
          commitDiffs.ChangeCounts = (Dictionary<VersionControlChangeType, int>) gitCommitChanges.ChangeCounts;
          commitDiffs.AllChangesIncluded = allChangesIncluded;
        }
        commitDiffs.SetSecuredObject(repositoryReadOnly);
      }
      return commitDiffs;
    }
  }
}
