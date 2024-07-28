// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitBranchStatsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Stats")]
  public class GitBranchStatsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches.json", "For all branches", null, null)]
    [PublicProjectRequestRestrictions]
    public virtual IList<GitBranchStats> GetBranches(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor baseVersionDescriptor = null)
    {
      IList<GitBranchStats> branchStatistics = this.GetBranchStatistics(repositoryId, projectId, versionDescriptor: baseVersionDescriptor);
      GitStatusStateMapper.MapGitEntity<IList<GitBranchStats>>(branchStatistics, this.TfsRequestContext);
      return branchStatistics;
    }

    [HttpGet]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches__name_.json", "For a single branch", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches__name__baseVersionType-_baseVersionType__baseVersion-_baseVersion_.json", "For a tag or commit", null, null)]
    [PublicProjectRequestRestrictions]
    public virtual GitBranchStats GetBranch(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      string name,
      [ClientIgnore] string projectId = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor baseVersionDescriptor = null)
    {
      if (name == null)
        return this.GetBranches(repositoryId, projectId)[0];
      GitBranchStats branchStatistic = this.GetBranchStatistics(repositoryId, projectId, name, baseVersionDescriptor)[0];
      GitStatusStateMapper.MapGitEntity<GitBranchStats>(branchStatistic, this.TfsRequestContext);
      return branchStatistic;
    }

    protected IList<GitBranchStats> GetBranchStatistics(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      string name = null,
      GitVersionDescriptor versionDescriptor = null)
    {
      IList<GitBranchStats> branchStatistics = (IList<GitBranchStats>) new List<GitBranchStats>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        Sha1Id objectId = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, versionDescriptor).ObjectId;
        TfsGitRef tfsGitRef = (TfsGitRef) null;
        if (name != null)
        {
          tfsGitRef = tfsGitRepository.Refs.MatchingName("refs/heads/" + name);
          if (tfsGitRef == null)
            throw new ArgumentException(Resources.Format("ErrorBranchNotFound", (object) name, (object) repositoryId)).Expected("git");
        }
        ITeamFoundationGitCommitService service = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        ITfsGitRepository repository = tfsGitRepository;
        Sha1Id baseCommitId = objectId;
        IReadOnlyList<TfsGitRef> refs;
        if (tfsGitRef == null)
          refs = tfsGitRepository.Refs.AllRefHeads();
        else
          refs = (IReadOnlyList<TfsGitRef>) new List<TfsGitRef>()
          {
            tfsGitRef
          };
        IEnumerable<TfsGitCommitLineageDiff> commitLineageDiffs = service.DiffCommitLineages(tfsRequestContext, repository, baseCommitId, (IEnumerable<TfsGitRef>) refs);
        ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        GitCommitTranslator translator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key, this.Url);
        foreach (TfsGitCommitLineageDiff lineageDiff in commitLineageDiffs)
        {
          if (lineageDiff.AheadCount == 0 && lineageDiff.BehindCount == 0)
            branchStatistics.Add(new GitBranchStats((GitCommitRef) translator.ToGitCommit(tfsGitRepository.LookupObject<TfsGitCommit>(objectId)), lineageDiff.RefName, repositoryReadOnly)
            {
              IsBaseVersion = true
            });
          else
            branchStatistics.Add(lineageDiff.ToBranchStats(this.TfsRequestContext, tfsGitRepository.Key, translator, repositoryReadOnly));
        }
      }
      return branchStatistics;
    }

    [HttpPost]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientResponseType(typeof (IList<GitBranchStats>), null, null)]
    [ClientInternalUseOnly(false)]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetBranchStatsBatch(
      GitQueryBranchStatsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      IList<GitBranchStats> statsBatchInternal = this.GetBranchStatsBatchInternal(searchCriteria, repositoryId, projectId);
      GitStatusStateMapper.MapGitEntity<IList<GitBranchStats>>(statsBatchInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<IList<GitBranchStats>>(HttpStatusCode.OK, statsBatchInternal);
    }

    protected IList<GitBranchStats> GetBranchStatsBatchInternal(
      GitQueryBranchStatsCriteria searchCriteria,
      string repositoryId,
      string projectId)
    {
      ArgumentUtility.CheckForNull<GitQueryBranchStatsCriteria>(searchCriteria, nameof (searchCriteria), "git");
      ArgumentUtility.CheckForNull<GitVersionDescriptor>(searchCriteria.baseVersionDescriptor, "baseVersionDescriptor", "git");
      ArgumentUtility.CheckForNull<GitVersionDescriptor[]>(searchCriteria.targetVersionDescriptors, "targetVersionDescriptors", "git");
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) searchCriteria.targetVersionDescriptors, "targetVersionDescriptors", "git");
      IList<GitBranchStats> gitBranchStatsList = (IList<GitBranchStats>) new List<GitBranchStats>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitCommit commitFromVersion = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, searchCriteria.baseVersionDescriptor);
        IList<Sha1Id> commitIdsToDiff = (IList<Sha1Id>) new List<Sha1Id>();
        IDictionary<Sha1Id, string> targetCommitToVersion = (IDictionary<Sha1Id, string>) new Dictionary<Sha1Id, string>();
        foreach (GitVersionDescriptor versionDescriptor in searchCriteria.targetVersionDescriptors)
        {
          Sha1Id objectId = GitVersionParser.GetCommitFromVersion(this.TfsRequestContext, tfsGitRepository, versionDescriptor).ObjectId;
          commitIdsToDiff.Add(objectId);
          targetCommitToVersion[objectId] = versionDescriptor.Version;
        }
        IEnumerable<TfsGitCommitLineageDiff> source = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().DiffCommitLineagesAgainstCommits(this.TfsRequestContext, tfsGitRepository, commitFromVersion.ObjectId, (IEnumerable<Sha1Id>) commitIdsToDiff);
        ISecuredObject securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
        GitCommitTranslator translator = new GitCommitTranslator(this.TfsRequestContext, tfsGitRepository.Key);
        Func<TfsGitCommitLineageDiff, GitBranchStats> selector = (Func<TfsGitCommitLineageDiff, GitBranchStats>) (diff => new GitBranchStats(translator.ToGitCommitShallow(diff.Metadata, false), targetCommitToVersion[diff.Metadata.CommitId], diff.AheadCount, diff.BehindCount, securedObject));
        return (IList<GitBranchStats>) source.Select<TfsGitCommitLineageDiff, GitBranchStats>(selector).ToList<GitBranchStats>();
      }
    }
  }
}
