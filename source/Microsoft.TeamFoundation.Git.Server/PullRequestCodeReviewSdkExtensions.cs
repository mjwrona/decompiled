// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestCodeReviewSdkExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Notifications.PullRequest;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class PullRequestCodeReviewSdkExtensions
  {
    private static readonly string s_SourceRefProperty = "Microsoft.Git.PullRequest.SourceRefName";
    private static readonly string s_TargetRefProperty = "Microsoft.Git.PullRequest.TargetRefName";
    private static readonly string s_IsDraftProperty = "Microsoft.Git.PullRequest.IsDraft";
    public static readonly string s_SourceRefCommit = "Microsoft.Git.PullRequest.SourceRefCommit";
    public static readonly string s_TargetRefCommit = "Microsoft.Git.PullRequest.TargetRefCommit";
    public static readonly string s_CommonRefCommit = "Microsoft.Git.PullRequest.CommonRefCommit";
    public static readonly string s_SourcePushId = "Microsoft.Git.PullRequest.SourcePushId";
    public static readonly string s_NewTargetRefName = "Microsoft.Git.PullRequest.NewTargetRefName";
    public static readonly string s_OldTargetRefName = "Microsoft.Git.PullRequest.OldTargetRefName";
    public static readonly string s_ConflictResolutionHash = "Microsoft.Git.PullRequest.ConflictResolutionHash";
    private static readonly string s_CalledByPullRequest = "CalledByPullRequest";
    public static readonly string s_SupportsIterationsMaxChangeEntriesRegPath = "/Service/Git/PullRequest/SupportsIterationsMaxChangeEntries";
    public const int c_SupportsIterationsMaxChangeEntriesDefaultValue = 100000;
    private static readonly string s_Layer = "TfsGitPullRequestCodeReview";

    public static void UpdateReviewers(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      Guid teamProjectId,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete)
    {
      if (pullRequest.CodeReviewId <= 0)
        return;
      try
      {
        requestContext.Trace(1013460, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Updating pull request reviewers with id: '{0}', project id: {1}", (object) pullRequest.PullRequestId, (object) teamProjectId);
        requestContext.SetPullRequestServiceAsCaller();
        ICodeReviewReviewerService service = requestContext.GetService<ICodeReviewReviewerService>();
        IEnumerable<Reviewer> list = (IEnumerable<Reviewer>) reviewersToUpdate.Select<TfsGitPullRequest.ReviewerBase, Reviewer>((Func<TfsGitPullRequest.ReviewerBase, Reviewer>) (revwr => new Reviewer()
        {
          Identity = new IdentityRef()
          {
            Id = revwr.Reviewer.ToString()
          },
          Kind = new ReviewerKind?(PullRequestCodeReviewSdkExtensions.ConvertToCodeReviewReviewerKind(revwr.IsRequired))
        })).ToList<Reviewer>();
        if (list.Any<Reviewer>())
        {
          if (requestContext.IsTracing(1013461, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
            requestContext.Trace(1013461, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Updating {0} reviewers from the pull request", (object) list.Count<Reviewer>());
          service.SaveReviewers(requestContext, teamProjectId, pullRequest.CodeReviewId, list);
        }
        if (!reviewerIdsToDelete.Any<Guid>())
          return;
        if (requestContext.IsTracing(1013462, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
          requestContext.Trace(1013462, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Removing {0} reviewers from the pull request", (object) reviewerIdsToDelete.Count<Guid>());
        service.RemoveReviewers(requestContext, teamProjectId, pullRequest.CodeReviewId, reviewerIdsToDelete);
      }
      catch (Exception ex)
      {
        if (requestContext.IsTracing(1013468, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
        {
          StringBuilder stringBuilder = new StringBuilder();
          if (reviewersToUpdate.Any<TfsGitPullRequest.ReviewerBase>())
            stringBuilder.Append("reviewers to update: ");
          foreach (TfsGitPullRequest.ReviewerBase reviewerBase in reviewersToUpdate)
            stringBuilder.Append(string.Format("'{0}', ", (object) reviewerBase.Reviewer.ToString()));
          if (reviewerIdsToDelete.Any<Guid>())
            stringBuilder.Append("reviewers to delete: ");
          foreach (Guid guid in reviewerIdsToDelete)
            stringBuilder.Append(string.Format("'{0}', ", (object) guid.ToString()));
          requestContext.Trace(1013468, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when updating reviewers for code review: {0}", (object) stringBuilder.ToString());
        }
        requestContext.Trace(1013469, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when updating reviewers for code review: PR id:'{0}', exception: '{1}'", (object) pullRequest.PullRequestId, (object) ex.ToString());
        throw;
      }
      finally
      {
        requestContext.UnsetPullRequestServiceAsCaller();
      }
    }

    public static void UpdateReviewerVote(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Guid reviewerId,
      ReviewerVote voteValue)
    {
      if (pullRequest.CodeReviewId <= 0)
        return;
      try
      {
        requestContext.Trace(1013470, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Updating pull reviewer vote with id: '{0}', project id: {1}", (object) pullRequest.PullRequestId, (object) teamProjectId);
        requestContext.SetPullRequestServiceAsCaller();
        ICodeReviewReviewerService service = requestContext.GetService<ICodeReviewReviewerService>();
        Reviewer reviewer1 = new Reviewer()
        {
          Identity = new IdentityRef()
          {
            Id = reviewerId.ToString()
          },
          ReviewerStateId = new short?((short) PullRequestCodeReviewSdkExtensions.ConvertToCodeReviewVoteState(new ReviewerVote?(voteValue)).Value)
        };
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = teamProjectId;
        int codeReviewId = pullRequest.CodeReviewId;
        Reviewer reviewer2 = reviewer1;
        service.SaveReviewer(requestContext1, projectId, codeReviewId, reviewer2);
        if (!requestContext.IsTracing(1013471, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
          return;
        requestContext.Trace(1013471, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "The reviewer to update: id: '{0}', state id: '{1}', kind: '{2}'", (object) reviewerId.ToString(), (object) reviewer1.ReviewerStateId, (object) reviewer1.Kind);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1013479, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when updating reviewer vote for code review: {0}", (object) ex.ToString());
        throw;
      }
      finally
      {
        requestContext.UnsetPullRequestServiceAsCaller();
      }
    }

    public static void ResetAllReviewerVotes(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      Guid teamProjectId)
    {
      if (pullRequest.CodeReviewId <= 0)
        return;
      try
      {
        requestContext.Trace(1013087, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Reseting all CodeReview votes for pr id: '{0}', project id: {1}", (object) pullRequest.PullRequestId, (object) teamProjectId);
        requestContext.SetPullRequestServiceAsCaller();
        requestContext.GetService<ICodeReviewReviewerService>().ResetAllReviewerStatuses(requestContext, teamProjectId, pullRequest.CodeReviewId);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1013479, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when updating reviewer vote for code review: {0}", (object) ex.ToString());
        throw;
      }
      finally
      {
        requestContext.UnsetPullRequestServiceAsCaller();
      }
    }

    public static void ResetSomeReviewerVotes(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      Guid teamProjectId,
      IEnumerable<Guid> reviewerIdsToReset)
    {
      if (pullRequest.CodeReviewId <= 0)
        return;
      try
      {
        requestContext.TraceConditionally(1013089, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, (Func<string>) (() => string.Format("Reseting some CodeReview votes for pr id: '{0}', project id: {1}, reviewer ids: {2}", (object) pullRequest.PullRequestId, (object) teamProjectId, (object) string.Join(",", reviewerIdsToReset.Select<Guid, string>((Func<Guid, string>) (r => r.ToString()))))));
        requestContext.SetPullRequestServiceAsCaller();
        requestContext.GetService<ICodeReviewReviewerService>().ResetSomeReviewerStatuses(requestContext, teamProjectId, pullRequest.CodeReviewId, reviewerIdsToReset);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1013479, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when updating reviewer vote for code review: {0}", (object) ex.ToString());
        throw;
      }
      finally
      {
        requestContext.UnsetPullRequestServiceAsCaller();
      }
    }

    public static DefaultReviewerStates? ConvertToCodeReviewVoteState(ReviewerVote? prVote)
    {
      if (!prVote.HasValue)
        return new DefaultReviewerStates?();
      if (prVote.HasValue)
      {
        switch (prVote.GetValueOrDefault())
        {
          case ReviewerVote.Rejected:
            return new DefaultReviewerStates?(DefaultReviewerStates.Rejected);
          case ReviewerVote.NotReady:
            return new DefaultReviewerStates?(DefaultReviewerStates.CodeNotReadyYet);
          case ReviewerVote.None:
            return new DefaultReviewerStates?(DefaultReviewerStates.NoResponse);
          case ReviewerVote.ApprovedWithComment:
            return new DefaultReviewerStates?(DefaultReviewerStates.ApprovedWithComments);
          case ReviewerVote.Approved:
            return new DefaultReviewerStates?(DefaultReviewerStates.Approved);
        }
      }
      return new DefaultReviewerStates?(DefaultReviewerStates.NoResponse);
    }

    public static ReviewerKind ConvertToCodeReviewReviewerKind(bool isReviewerRequired) => !isReviewerRequired ? ReviewerKind.Optional : ReviewerKind.Required;

    public static void CreateFirstIteration(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<TfsGitPullRequest.ReviewerBase> prReviewers,
      int dataspaceId,
      bool allowIterations)
    {
      using (requestContext.TimeRegion(PullRequestCodeReviewSdkExtensions.s_Layer, nameof (CreateFirstIteration)))
      {
        try
        {
          Iteration firstIterationObject = pullRequest.CreateFirstIterationObject(requestContext, repository, allowIterations);
          if (firstIterationObject == null)
            return;
          using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
            gitCoreComponent.CreateFirstPullRequestIteration(pullRequest.PullRequestId, dataspaceId, pullRequest.CodeReviewId, firstIterationObject);
          try
          {
            ArtifactPropertyKinds.SaveIterationProperties(requestContext, firstIterationObject.Properties, repository.Key.ProjectId, pullRequest.CodeReviewId, 1);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1013422, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, ex);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013423, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, ex);
        }
      }
    }

    public static Iteration CreateFirstIterationObject(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      bool allowIterations)
    {
      using (requestContext.TimeRegion(PullRequestCodeReviewSdkExtensions.s_Layer, nameof (CreateFirstIterationObject)))
      {
        bool flag = false;
        requestContext.Trace(1013421, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Creating the first iteration for pull request id: {0}", (object) pullRequest.PullRequestId);
        IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
        requestContext.GetService<ITeamFoundationGitPullRequestService>();
        requestContext.GetService<ICodeReviewService>();
        requestContext.GetService<IdentityService>();
        ICodeReviewIterationService service2 = requestContext.GetService<ICodeReviewIterationService>();
        Iteration iteration = (Iteration) null;
        if (allowIterations)
        {
          ClientTraceData properties = new ClientTraceData();
          TfsGitPullRequest pullRequest1 = pullRequest;
          IVssRequestContext requestContext1 = requestContext;
          ITfsGitRepository repository1 = repository;
          ClientTraceData clientTraceData = properties;
          Sha1Id conflictResolutionHash = new Sha1Id();
          ClientTraceData ctData = clientTraceData;
          iteration = pullRequest1.BuildIterationObject(requestContext1, repository1, 1, conflictResolutionHash: conflictResolutionHash, ctData: ctData);
          if (requestContext != null)
            requestContext.GetService<ClientTraceService>()?.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
          int trackingComputation = service2.GetMaxChangeEntriesForChangeTrackingComputation(requestContext);
          Microsoft.VisualStudio.Services.CodeReview.Server.ValidationHelper.ProcessIterationAndChanges(requestContext, iteration, trackingComputation);
          int num = service1.GetValue<int>(requestContext, (RegistryQuery) PullRequestCodeReviewSdkExtensions.s_SupportsIterationsMaxChangeEntriesRegPath, 100000);
          iteration.Author = new IdentityRef()
          {
            Id = pullRequest.Creator.ToString()
          };
          flag = iteration.ChangeList.Count <= num;
          if (!flag)
            requestContext.TraceAlways(1013424, TraceLevel.Warning, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Iteration support for pull request {0} was disabled due to exceeding the change limit threshold ({1} / {2})", (object) pullRequest.PullRequestId, (object) iteration.ChangeList.Count, (object) num);
        }
        return flag & allowIterations ? iteration : (Iteration) null;
      }
    }

    public static Review GetCodeReview(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      string teamProjectUri,
      CodeReviewExtendedProperties extendedProperties = CodeReviewExtendedProperties.All,
      int? maxChangesCount = null)
    {
      try
      {
        ITeamFoundationGitRepositoryService service1 = requestContext.GetService<ITeamFoundationGitRepositoryService>();
        ICodeReviewService service2 = requestContext.GetService<ICodeReviewService>();
        if (teamProjectUri == null)
        {
          using (ITfsGitRepository repositoryById = service1.FindRepositoryById(requestContext, pullRequest.RepositoryId))
            teamProjectUri = repositoryById.Key.GetProjectUri();
        }
        requestContext.Trace(1013410, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Query code reviews for pull request id: {0}", (object) pullRequest.PullRequestId);
        requestContext.SetPullRequestServiceAsCaller();
        Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId);
        return service2.GetReview(requestContext, projectId, pullRequest.CodeReviewId, extendedProperties, maxChangesCount: maxChangesCount);
      }
      catch (Exception ex) when (TracepointUtils.TraceException(requestContext, 1013413, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, ex, (object) new
      {
        PullRequestId = pullRequest.PullRequestId
      }, caller: nameof (GetCodeReview)))
      {
      }
      finally
      {
        requestContext.UnsetPullRequestServiceAsCaller();
      }
      return (Review) null;
    }

    public static void SaveReviewProperties(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (requestContext.TimeRegion(PullRequestCodeReviewSdkExtensions.s_Layer, nameof (SaveReviewProperties)))
      {
        requestContext.Trace(1013446, TraceLevel.Verbose, GitServerUtils.TraceArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Adding properties to associated code review: srcRef: {0}, tgtRef: {1}", (object) pullRequest.SourceBranchName, (object) pullRequest.TargetBranchName);
        try
        {
          PropertiesCollection reviewProperties = PullRequestCodeReviewSdkExtensions.BuildReviewPropertyCollection(requestContext, pullRequest.SourceBranchName, pullRequest.TargetBranchName, pullRequest.IsDraft);
          ArtifactPropertyKinds.SaveReviewProperties(requestContext, reviewProperties, projectId, pullRequest.CodeReviewId);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1013447, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when saving review properties for pull request: {0}. Properties may not have been saved. Exception details:\n{1}", (object) pullRequest.PullRequestId, (object) ex.ToString());
        }
      }
    }

    public static void CreateNewIteration(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IdentityDescriptor author,
      bool createOnlyOnSourceBranchUpdate,
      int pushId = 0,
      ConflictResolutionIterationParams conflictResolution = null)
    {
      if (pullRequest.CodeReviewId <= 0 || !pullRequest.SupportsIterations)
        return;
      requestContext.Trace(1013354, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Impersonating pusher/conflicts resolver to Create iteration for pull request id: '{0}', project id: {1}", (object) pullRequest.PullRequestId, (object) repository.Key.ProjectId);
      using (IVssRequestContext userContext = requestContext.CreateUserContext(author))
      {
        userContext.ProgressTimerJoin(requestContext);
        using (ITfsGitRepository repositoryById = userContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(userContext, repository.Key.RepoId))
          pullRequest.CreateNewIteration(userContext, repositoryById, createOnlyOnSourceBranchUpdate, pushId, conflictResolution: conflictResolution);
      }
    }

    public static void CreateNewIteration(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      bool createOnlyOnSourceBranchUpdate,
      int pushId = 0,
      string newTargetRefName = null,
      string oldTargetRefName = null,
      ConflictResolutionIterationParams conflictResolution = null)
    {
      if (pullRequest.CodeReviewId <= 0 || !pullRequest.SupportsIterations)
        return;
      using (requestContext.TimeRegion(PullRequestCodeReviewSdkExtensions.s_Layer, nameof (CreateNewIteration)))
      {
        try
        {
          ICodeReviewService service1 = requestContext.GetService<ICodeReviewService>();
          Guid guid = Guid.Parse(LinkingUtilities.DecodeUri(repository.Key.GetProjectUri()).ToolSpecificId);
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = guid;
          int codeReviewId = pullRequest.CodeReviewId;
          int? maxChangesCount = new int?();
          Review review = service1.GetReview(requestContext1, projectId, codeReviewId, maxChangesCount: maxChangesCount);
          requestContext.Trace(1013440, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Creating iteration for pull request id: '{0}', project id: {1}", (object) pullRequest.PullRequestId, (object) repository.Key.ProjectId);
          requestContext.SetPullRequestServiceAsCaller();
          if (review != null)
          {
            ICodeReviewIterationService service2 = requestContext.GetService<ICodeReviewIterationService>();
            IList<Iteration> rawIterations = service2.GetRawIterations(requestContext, repository.Key.ProjectId, review.Id);
            Iteration iteration1 = rawIterations.Aggregate<Iteration>((Func<Iteration, Iteration, Iteration>) ((iterA, iterB) =>
            {
              int? id1 = iterA.Id;
              int? id2 = iterB.Id;
              return !(id1.GetValueOrDefault() > id2.GetValueOrDefault() & id1.HasValue & id2.HasValue) ? iterB : iterA;
            }));
            if (requestContext.IsTracing(1013444, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
              requestContext.Trace(1013444, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Last iteration id for the review is {0}", (object) iteration1.Id);
            if (createOnlyOnSourceBranchUpdate)
            {
              TfsGitCommit branchCommit = PullRequestCodeReviewSdkExtensions.GetBranchCommit(repository, pullRequest.SourceBranchName);
              string sha1IdString = (string) null;
              Sha1Id sha1Id1 = branchCommit != null ? branchCommit.ObjectId : Sha1Id.Empty;
              ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, repository.Key.ProjectId, iteration1, PullRequestCodeReviewSdkExtensions.s_SourceRefCommit);
              if (iteration1.Properties != null && iteration1.Properties.TryGetValue<string>(PullRequestCodeReviewSdkExtensions.s_SourceRefCommit, out sha1IdString))
              {
                Sha1Id sha1Id2 = Sha1Id.Parse(sha1IdString);
                if (sha1Id1 == sha1Id2)
                {
                  requestContext.Trace(1013441, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Source ref commit {0} didn't change since last iteration with id {1}, don't create new one", (object) sha1IdString, (object) iteration1.Id);
                  return;
                }
                if (sha1Id1 == Sha1Id.Empty)
                {
                  requestContext.Trace(1013445, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Source ref branch was deleted, don't create iteration");
                  return;
                }
              }
            }
            else if (conflictResolution != null && conflictResolution.Hash != Sha1Id.Empty)
            {
              ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, repository.Key.ProjectId, rawIterations, PullRequestCodeReviewSdkExtensions.s_ConflictResolutionHash);
              foreach (Iteration iteration2 in rawIterations.Reverse<Iteration>())
              {
                string sha1IdString;
                if (iteration2.Properties != null && iteration2.Properties.TryGetValue<string>(PullRequestCodeReviewSdkExtensions.s_ConflictResolutionHash, out sha1IdString))
                {
                  Sha1Id sha1Id = new Sha1Id(sha1IdString);
                  if (conflictResolution.Hash == sha1Id)
                  {
                    requestContext.Trace(1013902, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Conflict resolution hash {0} didn't change since last iteration with resolved conflicts and id {1}, don't create new one", (object) sha1Id, (object) iteration2.Id);
                    return;
                  }
                }
              }
              conflictResolution.IsCreated = true;
            }
            ClientTraceData properties = new ClientTraceData();
            int num = rawIterations.Count + 1;
            TfsGitPullRequest pullRequest1 = pullRequest;
            IVssRequestContext requestContext2 = requestContext;
            ITfsGitRepository repository1 = repository;
            int iterationId = num;
            int id = review.Id;
            int pushId1 = pushId;
            Sha1Id sha1Id3 = conflictResolution != null ? conflictResolution.Hash : new Sha1Id();
            string newTargetRefName1 = newTargetRefName;
            string oldTargetRefName1 = oldTargetRefName;
            Sha1Id conflictResolutionHash = sha1Id3;
            ClientTraceData ctData = properties;
            Iteration iteration3 = pullRequest1.BuildIterationObject(requestContext2, repository1, iterationId, id, pushId: pushId1, newTargetRefName: newTargetRefName1, oldTargetRefName: oldTargetRefName1, conflictResolutionHash: conflictResolutionHash, ctData: ctData);
            if (requestContext != null)
              requestContext.GetService<ClientTraceService>()?.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
            service2.SaveIteration(requestContext, repository.Key.ProjectId, iteration3, false);
          }
          else
            requestContext.Trace(1013442, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "No code reviews were found with PR id: '{0}'. Skipping iteration creation", (object) pullRequest.PullRequestId);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1013443, TraceLevel.Error, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Exception thrown when creating iteration for code review for PR with id '{0}': {1}", (object) pullRequest.PullRequestId, (object) ex.ToString());
          if (requestContext.IsFeatureEnabled("Git.PullRequests.AtomicIterationSave"))
            PullRequestCodeReviewSdkExtensions.PublishBrokenStateNotification(requestContext, repository, pullRequest.PullRequestId);
          throw;
        }
        finally
        {
          requestContext.UnsetPullRequestServiceAsCaller();
        }
      }
    }

    private static void PublishBrokenStateNotification(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
      ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
      PullRequestBrokenStateNotification stateNotification = new PullRequestBrokenStateNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, Resources.Get("PullRequestBrokenStateFailedToCreateIteration"));
      IVssRequestContext requestContext1 = requestContext;
      PullRequestBrokenStateNotification notificationEvent = stateNotification;
      service.PublishNotification(requestContext1, (object) notificationEvent);
    }

    private static Iteration BuildIterationObject(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int iterationId,
      int reviewId = 0,
      bool addChangeEntries = true,
      int pushId = 0,
      string newTargetRefName = null,
      string oldTargetRefName = null,
      Sha1Id conflictResolutionHash = default (Sha1Id),
      ClientTraceData ctData = null)
    {
      requestContext.Trace(1013450, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Creating iteration object id: {0}", (object) iterationId);
      string str1 = (string) null;
      TfsGitCommit tfsGitCommit1 = (TfsGitCommit) null;
      TfsGitCommit tfsGitCommit2 = (TfsGitCommit) null;
      if (pullRequest.Status == PullRequestStatus.Active)
      {
        tfsGitCommit1 = PullRequestCodeReviewSdkExtensions.GetBranchCommit(repository, pullRequest.SourceBranchName);
        tfsGitCommit2 = PullRequestCodeReviewSdkExtensions.GetBranchCommit(repository, pullRequest.TargetBranchName);
      }
      string sourceBranchCommitSha1Id;
      string targetBranchCommitSha1Id;
      Sha1Id sha1Id;
      if (tfsGitCommit1 != null && tfsGitCommit2 != null)
      {
        str1 = tfsGitCommit1.GetComment();
        sourceBranchCommitSha1Id = tfsGitCommit1.ObjectId.ToString();
        targetBranchCommitSha1Id = tfsGitCommit2.ObjectId.ToString();
      }
      else
      {
        Sha1Id valueOrDefault = pullRequest.LastMergeSourceCommit.GetValueOrDefault();
        if (repository.TryLookupObject(valueOrDefault) is TfsGitCommit tfsGitCommit3)
          str1 = tfsGitCommit3.GetComment();
        sourceBranchCommitSha1Id = valueOrDefault.ToString();
        sha1Id = pullRequest.LastMergeTargetCommit.GetValueOrDefault();
        targetBranchCommitSha1Id = sha1Id.ToString();
      }
      if (conflictResolutionHash != Sha1Id.Empty)
        str1 = "Conflicts resolved";
      requestContext.Trace(1013451, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Iteration description: {0}", (object) (str1 ?? "{null}"));
      requestContext.Trace(1013452, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Finding diff between commits {0} and {1}", (object) sourceBranchCommitSha1Id, (object) targetBranchCommitSha1Id);
      TfsGitCommit commonCommit = (TfsGitCommit) null;
      IList<ChangeEntry> changeEntryList = (IList<ChangeEntry>) null;
      if (addChangeEntries)
      {
        changeEntryList = PullRequestCodeReviewSdkExtensions.ConvertChanges(pullRequest.GetChanges(requestContext, out commonCommit), true);
        if (requestContext.IsTracing(1013454, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer))
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (ChangeEntry changeEntry in (IEnumerable<ChangeEntry>) changeEntryList)
            stringBuilder.Append(string.Format("base file path: '{0}', modified file path: '{1}'\r\n", changeEntry.Base == null ? (object) "{null}" : (object) changeEntry.Base.Path, changeEntry.Modified == null ? (object) "{null}" : (object) changeEntry.Modified.Path));
          requestContext.Trace(1013454, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Changes for the new iteration: {0}", (object) stringBuilder.ToString());
        }
      }
      else
        commonCommit = pullRequest.GetCommonCommit(requestContext);
      string str2;
      if (commonCommit == null)
      {
        str2 = (string) null;
      }
      else
      {
        sha1Id = commonCommit.ObjectId;
        str2 = sha1Id.ToString();
      }
      string commonCommitSha1Id = str2;
      requestContext.Trace(1013453, TraceLevel.Verbose, GitServerUtils.GitCodeReviewArea, PullRequestCodeReviewSdkExtensions.s_Layer, "Adding properties to iteration: srcCommit: {0}, tgtCommit: {1}", (object) sourceBranchCommitSha1Id, (object) targetBranchCommitSha1Id);
      ctData?.Add("ProjectId", (object) repository?.Key?.ProjectId);
      ctData?.Add("RepositoryId", (object) repository?.Key?.RepoId);
      ctData?.Add("RepositoryName", (object) repository?.Name);
      ctData?.Add("PullRequestId", (object) pullRequest?.PullRequestId);
      ctData?.Add("CodeReviewId", (object) reviewId);
      ctData?.Add("KeyPullRequestIterationId", (object) iterationId);
      ctData?.Add("KeyPullRequestIterationChangesCount", (object) changeEntryList?.Count);
      PropertiesCollection propertiesCollection = PullRequestCodeReviewSdkExtensions.BuildIterationPropertyCollection(requestContext, sourceBranchCommitSha1Id, targetBranchCommitSha1Id, commonCommitSha1Id, pushId, newTargetRefName, oldTargetRefName, conflictResolutionHash);
      return new Iteration()
      {
        Id = new int?(iterationId),
        ReviewId = reviewId,
        Description = str1,
        Properties = propertiesCollection,
        IsUnpublished = false,
        ChangeList = changeEntryList
      };
    }

    public static Iteration BuildSyntheticIteration(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int iterationId,
      int reviewId = 0,
      bool addChangeEntries = true)
    {
      Iteration iteration = pullRequest.BuildIterationObject(requestContext, repository, iterationId, reviewId, addChangeEntries);
      IdentityService service = requestContext.GetService<IdentityService>();
      iteration.Author = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        pullRequest.Creator
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>().ToIdentityRef(requestContext);
      iteration.CreatedDate = new DateTime?(pullRequest.CreationDate);
      iteration.UpdatedDate = new DateTime?(pullRequest.CreationDate);
      return iteration;
    }

    public static IterationChanges GetIterationChanges(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string teamProjectUri,
      int iterationId,
      int? top = null,
      int? skip = null)
    {
      if (teamProjectUri == null)
        teamProjectUri = repository.Key.GetProjectUri();
      Guid guid = Guid.Parse(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId);
      ICodeReviewIterationService service = requestContext.GetService<ICodeReviewIterationService>();
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = guid;
      int codeReviewId = pullRequest.CodeReviewId;
      List<int> iterationIds = new List<int>();
      iterationIds.Add(iterationId);
      int? top1 = top;
      int? skip1 = skip;
      return service.GetIterationChanges(requestContext1, projectId, codeReviewId, iterationIds, top1, skip1);
    }

    public static List<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread> GetCommentThreads(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string teamProjectUri,
      DateTime? modifiedSince = null,
      Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true)
    {
      if (teamProjectUri == null)
        teamProjectUri = repository.Key.GetProjectUri();
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId);
      return requestContext.GetService<ICodeReviewCommentService>().GetCommentThreads(requestContext, projectId, pullRequest.CodeReviewId, modifiedSince, trackingCriteria, addReferenceLinks, includeExtendedProperties);
    }

    public static Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread GetCommentThread(
      this TfsGitPullRequest pullRequest,
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string teamProjectUri,
      int threadId,
      Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true)
    {
      if (teamProjectUri == null)
        teamProjectUri = repository.Key.GetProjectUri();
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(teamProjectUri).ToolSpecificId);
      return requestContext.GetService<ICodeReviewCommentService>().GetCommentThread(requestContext, projectId, pullRequest.CodeReviewId, threadId, trackingCriteria, addReferenceLinks, includeExtendedProperties);
    }

    public static ReviewStatus? ConvertToReviewStatus(PullRequestStatus? prStatus)
    {
      if (!prStatus.HasValue)
        return new ReviewStatus?();
      if (prStatus.HasValue)
      {
        switch (prStatus.GetValueOrDefault())
        {
          case PullRequestStatus.NotSet:
            return new ReviewStatus?();
          case PullRequestStatus.Active:
            return new ReviewStatus?(ReviewStatus.Active);
          case PullRequestStatus.Abandoned:
            return new ReviewStatus?(ReviewStatus.Abandoned);
          case PullRequestStatus.Completed:
            return new ReviewStatus?(ReviewStatus.Completed);
        }
      }
      return new ReviewStatus?();
    }

    public static IList<ChangeEntry> ConvertChanges(
      IReadOnlyList<TfsGitCommitChange> changes,
      bool includeSubmoduleEntries)
    {
      if (changes == null)
        return (IList<ChangeEntry>) new List<ChangeEntry>();
      List<ChangeEntry> changeEntryList = new List<ChangeEntry>(changes.Count<TfsGitCommitChange>());
      foreach (TfsGitCommitChange change in (IEnumerable<TfsGitCommitChange>) changes)
      {
        if (!change.ChangeType.HasFlag((Enum) TfsGitChangeType.SourceRename) || !change.ChangeType.HasFlag((Enum) TfsGitChangeType.Delete))
        {
          ChangeEntry changeEntry = PullRequestCodeReviewSdkExtensions.CreateChangeEntry(change, includeSubmoduleEntries);
          if (changeEntry != null)
            changeEntryList.Add(changeEntry);
        }
      }
      return (IList<ChangeEntry>) changeEntryList;
    }

    public static PropertiesCollection BuildReviewPropertyCollection(
      IVssRequestContext requestContext,
      string sourceBranchName,
      string targetBranchName,
      bool isDraft)
    {
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      Func<string, string> func = (Func<string, string>) (propString => ArgumentUtility.ReplaceIllegalCharacters(propString, '?'));
      propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_SourceRefProperty, (object) func(sourceBranchName));
      propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_TargetRefProperty, (object) func(targetBranchName));
      propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_IsDraftProperty, (object) isDraft);
      return propertiesCollection;
    }

    public static PropertiesCollection BuildIterationPropertyCollection(
      IVssRequestContext requestContext,
      string sourceBranchCommitSha1Id,
      string targetBranchCommitSha1Id,
      string commonCommitSha1Id,
      int pushId,
      string newTargetRefName = null,
      string oldTargetRefName = null,
      Sha1Id conflictResolutionHash = default (Sha1Id))
    {
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_SourceRefCommit, (object) sourceBranchCommitSha1Id);
      propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_TargetRefCommit, (object) targetBranchCommitSha1Id);
      if (pushId > 0)
        propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_SourcePushId, (object) pushId);
      if (commonCommitSha1Id != null)
        propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_CommonRefCommit, (object) commonCommitSha1Id);
      if (!string.IsNullOrEmpty(newTargetRefName))
        propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_NewTargetRefName, (object) newTargetRefName);
      if (!string.IsNullOrEmpty(oldTargetRefName))
        propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_OldTargetRefName, (object) oldTargetRefName);
      if (conflictResolutionHash != Sha1Id.Empty)
        propertiesCollection.Add(PullRequestCodeReviewSdkExtensions.s_ConflictResolutionHash, (object) conflictResolutionHash.ToString());
      return propertiesCollection;
    }

    private static ChangeEntry CreateChangeEntry(
      TfsGitCommitChange change,
      bool includeSubmoduleEntry)
    {
      if (change == null)
        return (ChangeEntry) null;
      if (change.ObjectType != GitObjectType.Blob && (!includeSubmoduleEntry || change.ObjectType != GitObjectType.Commit))
        return (ChangeEntry) null;
      string str1 = change.ParentPath + change.ChildItem;
      string str2 = change.RenameSourceItemPath != null ? change.RenameSourceItemPath : str1;
      ChangeEntryFileInfo changeEntryFileInfo1 = (ChangeEntryFileInfo) null;
      if (change.OriginalObjectId.HasValue)
      {
        ChangeEntryFileInfo changeEntryFileInfo2 = new ChangeEntryFileInfo();
        changeEntryFileInfo2.Path = str2;
        changeEntryFileInfo2.SHA1Hash = change.OriginalObjectId.ToString();
        changeEntryFileInfo2.Flags = (byte) (change.ObjectType == GitObjectType.Commit);
        changeEntryFileInfo1 = changeEntryFileInfo2;
      }
      ChangeEntryFileInfo changeEntryFileInfo3 = (ChangeEntryFileInfo) null;
      if (change.ChangedObjectId.HasValue)
      {
        ChangeEntryFileInfo changeEntryFileInfo4 = new ChangeEntryFileInfo();
        changeEntryFileInfo4.Path = str1;
        changeEntryFileInfo4.SHA1Hash = change.ChangedObjectId.ToString();
        changeEntryFileInfo4.Flags = (byte) (change.ObjectType == GitObjectType.Commit);
        changeEntryFileInfo3 = changeEntryFileInfo4;
      }
      return new ChangeEntry()
      {
        Base = changeEntryFileInfo1,
        Modified = changeEntryFileInfo3,
        Type = PullRequestCodeReviewSdkExtensions.ConvertType(change.ChangeType)
      };
    }

    private static ChangeType ConvertType(TfsGitChangeType type)
    {
      ChangeType changeType = ChangeType.None;
      if (type.HasFlag((Enum) TfsGitChangeType.Add))
        changeType |= ChangeType.Add;
      if (type.HasFlag((Enum) TfsGitChangeType.Delete))
        changeType |= ChangeType.Delete;
      if (type.HasFlag((Enum) TfsGitChangeType.Edit))
        changeType |= ChangeType.Edit;
      if (type.HasFlag((Enum) TfsGitChangeType.Rename))
        changeType |= ChangeType.Rename;
      return changeType;
    }

    private static TfsGitCommit GetBranchCommit(ITfsGitRepository repository, string branchName)
    {
      TfsGitRef tfsGitRef = repository.Refs.MatchingName(branchName);
      return tfsGitRef != null ? repository.TryLookupObject(tfsGitRef.ObjectId) as TfsGitCommit : (TfsGitCommit) null;
    }

    private static void SetPullRequestServiceAsCaller(this IVssRequestContext requestContext) => requestContext.Items[PullRequestCodeReviewSdkExtensions.s_CalledByPullRequest] = (object) true;

    private static void UnsetPullRequestServiceAsCaller(this IVssRequestContext requestContext)
    {
      if (!requestContext.Items.ContainsKey(PullRequestCodeReviewSdkExtensions.s_CalledByPullRequest))
        return;
      requestContext.Items.Remove(PullRequestCodeReviewSdkExtensions.s_CalledByPullRequest);
    }

    public static bool IsCalledByPullRequestService(this IVssRequestContext requestContext)
    {
      bool flag;
      return requestContext.TryGetItem<bool>(PullRequestCodeReviewSdkExtensions.s_CalledByPullRequest, out flag) && flag;
    }
  }
}
