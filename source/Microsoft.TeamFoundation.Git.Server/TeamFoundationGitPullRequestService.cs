// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitPullRequestService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Git.Server.Policy;
using Microsoft.TeamFoundation.Git.Server.Services.PullRequest;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TeamFoundationGitPullRequestService : 
    ITeamFoundationGitPullRequestService,
    IVssFrameworkService
  {
    private static readonly List<string> c_templateLocations = new List<string>()
    {
      ".azuredevops/",
      ".vsts/",
      "docs/",
      (string) null
    };
    private const int c_maxTemplateFileNames = 100;
    private const int c_maxDescriptionLength = 4000;
    private const string c_defaultTemplateName = "pull_request_template";
    private const string c_templateFolderPath = "PULL_REQUEST_TEMPLATE/";
    private const string c_branchDefaultsFolder = "branches/";
    private static readonly Guid s_buildPolicyTypeId = new Guid("0609B952-1397-4640-95EC-E00A01B2C241");
    private static readonly int s_maxPoliciesAutoCompleteShouldRequeue = 5;
    internal const string AutoCompleteLayer = "PullRequestAutoComplete";
    internal const string CompletionQueueLayer = "PullRequestCompletionQueueJob";
    internal const string MergeQueueLayer = "PullRequestMergeQueueJob";
    private static readonly string TraceAreaAndLayer = "TeamFoundationGitPullRequestService.CodeReview";
    private const int c_defaultMaxReceiversCount = 100;
    private const int c_defaultMaxMessageLength = 1024;
    private const string c_Layer = "TfsGitPullRequest";
    private const int c_MaxReviewerUpdatesPerRequest = 100;
    internal const string PullRequestLinkType = "Pull Request";

    internal TfsGitPullRequest RebasePullRequestAndUpdateConflicts(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      Sha1Id sourceCommitId,
      Sha1Id targetCommitId,
      bool forCompletion,
      bool isFastForwardable,
      ClientTraceData ctData,
      out Sha1Id mergeCommitId,
      out Sha1Id rebasedSourceCommitId,
      out Sha1Id conflictResolutionHash,
      out IdentityDescriptor lastConflictResolver)
    {
      conflictResolutionHash = Sha1Id.Empty;
      mergeCommitId = Sha1Id.Empty;
      rebasedSourceCommitId = sourceCommitId;
      lastConflictResolver = (IdentityDescriptor) null;
      if (!requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.MergeStrategy.Rebase"))
      {
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        TfsGitPullRequest pullRequest1 = pullRequest;
        PullRequestAsyncStatus? nullable1 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Failure);
        Sha1Id? nullable2 = new Sha1Id?(sourceCommitId);
        Sha1Id? nullable3 = new Sha1Id?(targetCommitId);
        string str = Resources.Get("PullRequestUnsupportedMergeType");
        PullRequestMergeFailureType? nullable4 = new PullRequestMergeFailureType?(PullRequestMergeFailureType.Unknown);
        Sha1Id? nullable5 = new Sha1Id?(Sha1Id.Empty);
        PullRequestStatus? status = new PullRequestStatus?();
        PullRequestAsyncStatus? mergeStatus = nullable1;
        Sha1Id? lastMergeSourceCommit = nullable2;
        Sha1Id? lastMergeTargetCommit = nullable3;
        Sha1Id? lastMergeCommit = nullable5;
        Guid? completeWhenMergedAuthority = new Guid?();
        PullRequestMergeFailureType? mergeFailureType = nullable4;
        string mergeFailureMessage = str;
        DateTime? completionQueueTime = new DateTime?();
        Guid? autoCompleteAuthority = new Guid?();
        bool? isDraft = new bool?();
        return this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, mergeFailureMessage: mergeFailureMessage, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
      }
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (RebasePullRequestAndUpdateConflicts)))
      {
        MergeWithConflictsResult withConflictsResult = (MergeWithConflictsResult) null;
        GitPullRequestCompletionOptions completionOptions1 = pullRequest.CompletionOptions;
        GitPullRequestMergeStrategy? mergeStrategy;
        int num1;
        if (completionOptions1 == null)
        {
          num1 = 0;
        }
        else
        {
          mergeStrategy = completionOptions1.MergeStrategy;
          GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.Rebase;
          num1 = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
        }
        int num2 = isFastForwardable ? 1 : 0;
        if ((num1 & num2) != 0)
        {
          withConflictsResult = new MergeWithConflictsResult()
          {
            Status = PullRequestAsyncStatus.Succeeded,
            MergeCommitId = sourceCommitId
          };
        }
        else
        {
          bool disableRenames;
          int renameThreshold;
          int targetLimit;
          this.CoalesceMergeOptions(requestContext, repository, pullRequest, out bool _, out disableRenames, out bool _, out renameThreshold, out targetLimit);
          CommitDetails commitDetails = ITfsGitRepositoryExtensions.CreateCommitDetails(requestContext, pullRequest, pullRequest.CompletionOptions?.MergeCommitMessage);
          MergeWithConflictsOptions options = new MergeWithConflictsOptions()
          {
            CommitDetails = commitDetails,
            DisableRenames = disableRenames,
            RenameThreshold = renameThreshold,
            TargetLimit = targetLimit
          };
          using (LibGit2NativeLibrary git2NativeLibrary = new LibGit2NativeLibrary(requestContext, repository))
          {
            if (!isFastForwardable)
            {
              TfsGitPullRequest tfsGitPullRequest = pullRequest;
              IVssRequestContext requestContext2 = requestContext;
              ITfsGitRepository repository1 = repository;
              string str1 = sourceCommitId.ToString();
              string str2 = targetCommitId.ToString();
              int? top = new int?();
              int? skip = new int?();
              string sourceCommitId1 = str1;
              string targetCommitId1 = str2;
              List<Sha1Id> list = tfsGitPullRequest.GetCommits(requestContext2, repository1, top, skip, sourceCommitId1, targetCommitId1).Reverse<Sha1Id>().ToList<Sha1Id>();
              ctData?.Add("PullRequestRebaseCommitCount", (object) list.Count);
              int defaultValue = requestContext.ExecutionEnvironment.IsHostedDeployment ? 100 : 500;
              int num3 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MaxRebaseCommitCount", true, defaultValue);
              if (num3 > 0 && list.Count > num3)
              {
                IVssRequestContext requestContext3 = requestContext;
                Guid projectId = repository.Key.ProjectId;
                TfsGitPullRequest pullRequest2 = pullRequest;
                PullRequestAsyncStatus? nullable6 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Failure);
                Sha1Id? nullable7 = new Sha1Id?(sourceCommitId);
                Sha1Id? nullable8 = new Sha1Id?(targetCommitId);
                string str3 = Resources.Format("PullRequestRebaseTooManyCommits", (object) num3);
                PullRequestMergeFailureType? nullable9 = new PullRequestMergeFailureType?(PullRequestMergeFailureType.Unknown);
                Sha1Id? nullable10 = new Sha1Id?(Sha1Id.Empty);
                PullRequestStatus? status = new PullRequestStatus?();
                PullRequestAsyncStatus? mergeStatus = nullable6;
                Sha1Id? lastMergeSourceCommit = nullable7;
                Sha1Id? lastMergeTargetCommit = nullable8;
                Sha1Id? lastMergeCommit = nullable10;
                Guid? completeWhenMergedAuthority = new Guid?();
                PullRequestMergeFailureType? mergeFailureType = nullable9;
                string mergeFailureMessage = str3;
                DateTime? completionQueueTime = new DateTime?();
                Guid? autoCompleteAuthority = new Guid?();
                bool? isDraft = new bool?();
                return this.UpdatePullRequestInDatabase(requestContext3, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, mergeFailureMessage: mergeFailureMessage, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
              }
              Sha1Id ontoCommitId = targetCommitId;
              foreach (Sha1Id commitId in list)
              {
                using (requestContext.TimeRegion("LibGit2NativeLibrary", "RebaseCommitUsingIndex"))
                  withConflictsResult = git2NativeLibrary.RebaseCommitUsingIndex(commitId, ontoCommitId, options, ctData);
                if (withConflictsResult.Status == PullRequestAsyncStatus.Succeeded)
                  rebasedSourceCommitId = ontoCommitId = withConflictsResult.MergeCommitId;
                else
                  break;
              }
            }
            GitPullRequestCompletionOptions completionOptions2 = pullRequest.CompletionOptions;
            int num4;
            if (completionOptions2 == null)
            {
              num4 = 0;
            }
            else
            {
              mergeStrategy = completionOptions2.MergeStrategy;
              GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.RebaseMerge;
              num4 = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
            }
            if (num4 != 0)
            {
              if (withConflictsResult != null)
              {
                if (withConflictsResult.Status != PullRequestAsyncStatus.Succeeded)
                  goto label_35;
              }
              withConflictsResult = git2NativeLibrary.CreateSemiLinearMergeCommit(targetCommitId, rebasedSourceCommitId, options, ctData);
            }
          }
        }
label_35:
        mergeCommitId = withConflictsResult.MergeCommitId;
        IVssRequestContext requestContext4 = requestContext;
        Guid projectId1 = repository.Key.ProjectId;
        TfsGitPullRequest pullRequest3 = pullRequest;
        PullRequestAsyncStatus? nullable11 = new PullRequestAsyncStatus?(withConflictsResult.Status);
        Sha1Id? nullable12 = new Sha1Id?(sourceCommitId);
        Sha1Id? nullable13 = new Sha1Id?(targetCommitId);
        string failureMessage = withConflictsResult.FailureMessage;
        PullRequestMergeFailureType? nullable14 = new PullRequestMergeFailureType?(withConflictsResult.FailureType);
        Sha1Id? nullable15 = new Sha1Id?(mergeCommitId);
        PullRequestStatus? status1 = new PullRequestStatus?();
        PullRequestAsyncStatus? mergeStatus1 = nullable11;
        Sha1Id? lastMergeSourceCommit1 = nullable12;
        Sha1Id? lastMergeTargetCommit1 = nullable13;
        Sha1Id? lastMergeCommit1 = nullable15;
        Guid? completeWhenMergedAuthority1 = new Guid?();
        PullRequestMergeFailureType? mergeFailureType1 = nullable14;
        string mergeFailureMessage1 = failureMessage;
        DateTime? completionQueueTime1 = new DateTime?();
        Guid? autoCompleteAuthority1 = new Guid?();
        bool? isDraft1 = new bool?();
        pullRequest = this.UpdatePullRequestInDatabase(requestContext4, projectId1, pullRequest3, status1, mergeStatus1, lastMergeSourceCommit1, lastMergeTargetCommit1, lastMergeCommit1, completeWhenMergedAuthority1, mergeFailureType: mergeFailureType1, mergeFailureMessage: mergeFailureMessage1, completionQueueTime: completionQueueTime1, autoCompleteAuthority: autoCompleteAuthority1, isDraft: isDraft1);
        TeamFoundationGitPullRequestService.SetInternalPullRequestRefs(requestContext, pullRequest, new Sha1Id?(withConflictsResult.MergeCommitId), new Sha1Id?(), new Sha1Id?());
        return pullRequest;
      }
    }

    public string GetTemplateContent(
      ITfsGitRepository targetRepository,
      string targetRef,
      string templateName,
      out IEnumerable<string> path)
    {
      path = (IEnumerable<string>) new string[2];
      TfsGitTree templateBranchTree = this.GetTemplateBranchTree(targetRepository);
      if (templateBranchTree == null)
        return (string) null;
      string templateName1 = targetRef.Substring(0, targetRef.Contains<char>('/') ? targetRef.IndexOf('/') : targetRef.Length);
      bool flag = !string.IsNullOrEmpty(templateName);
      string path1 = (string) null;
      TfsGitBlob templateBlob1 = !flag ? this.GetTemplateBlob(targetRepository, templateBranchTree, "PULL_REQUEST_TEMPLATE/" + "branches/".ToUpper(), templateName1, true, out path1) : (TfsGitBlob) null;
      string pathSuffix = flag ? "PULL_REQUEST_TEMPLATE/" : (string) null;
      templateName = flag ? templateName : "pull_request_template";
      string path2;
      TfsGitBlob templateBlob2 = this.GetTemplateBlob(targetRepository, templateBranchTree, pathSuffix, templateName, !flag, out path2);
      path = (IEnumerable<string>) new string[2]
      {
        path1,
        path2
      };
      if (templateBlob1 == null && templateBlob2 == null)
        return (string) null;
      if (templateBlob1 != null)
      {
        using (Stream content = templateBlob1.GetContent())
          return this.ReadFileContent(content);
      }
      else
      {
        using (Stream content = templateBlob2.GetContent())
          return this.ReadFileContent(content);
      }
    }

    private TfsGitBlob GetTemplateBlob(
      ITfsGitRepository targetRepository,
      TfsGitTree branchTree,
      string pathSuffix,
      string templateName,
      bool checkExtensions,
      out string path)
    {
      string[] strArray;
      if (!checkExtensions)
        strArray = new string[1]{ templateName };
      else
        strArray = new string[3]
        {
          templateName,
          templateName + ".md",
          templateName + ".txt"
        };
      string[] candidateTemplateNames = strArray;
      for (int index = 0; index < TeamFoundationGitPullRequestService.c_templateLocations.Count; ++index)
      {
        string path1 = TeamFoundationGitPullRequestService.c_templateLocations[index] + pathSuffix;
        TfsGitObject tfsGitObject = string.IsNullOrEmpty(path1) ? (TfsGitObject) branchTree : PullRequestTemplateUtils.FindMember(branchTree, ref path1, out TfsGitTreeEntry _);
        if (tfsGitObject != null && tfsGitObject.ObjectType == GitObjectType.Tree)
        {
          for (int j = 0; j < candidateTemplateNames.Length; j++)
          {
            TfsGitTreeEntry tfsGitTreeEntry = ((TfsGitTree) tfsGitObject).GetTreeEntries().FirstOrDefault<TfsGitTreeEntry>((Func<TfsGitTreeEntry, bool>) (te => te.Name.Equals(candidateTemplateNames[j], StringComparison.OrdinalIgnoreCase) && te.ObjectType == GitObjectType.Blob));
            if (tfsGitTreeEntry != null)
            {
              path = path1 + "/" + tfsGitTreeEntry.Name;
              return (TfsGitBlob) targetRepository.LookupObject<TfsGitObject>(tfsGitTreeEntry.ObjectId);
            }
          }
        }
      }
      path = (string) null;
      return (TfsGitBlob) null;
    }

    public IEnumerable<string> GetTemplatesList(ITfsGitRepository targetRepository)
    {
      List<string> templatesList = new List<string>();
      TfsGitTree templateBranchTree = this.GetTemplateBranchTree(targetRepository);
      if (templateBranchTree == null)
        return (IEnumerable<string>) templatesList;
      string folderName1;
      IEnumerable<TfsGitTreeEntry> templateTreeEntries1 = this.GetTemplateTreeEntries(templateBranchTree, "PULL_REQUEST_TEMPLATE/", out folderName1);
      string folderName2;
      IEnumerable<TfsGitTreeEntry> templateTreeEntries2 = this.GetTemplateTreeEntries(templateBranchTree, "PULL_REQUEST_TEMPLATE/branches/", out folderName2);
      if (templateTreeEntries1 == null && templateTreeEntries2 == null)
        return (IEnumerable<string>) templatesList;
      if (templateTreeEntries1 != null)
      {
        foreach (TfsGitTreeEntry tfsGitTreeEntry in templateTreeEntries1)
          templatesList.Add(folderName1 + "/" + tfsGitTreeEntry.Name);
      }
      if (templateTreeEntries2 != null)
      {
        foreach (TfsGitTreeEntry tfsGitTreeEntry in templateTreeEntries2)
          templatesList.Add(folderName2 + "/" + tfsGitTreeEntry.Name);
      }
      return (IEnumerable<string>) templatesList;
    }

    private IEnumerable<TfsGitTreeEntry> GetTemplateTreeEntries(
      TfsGitTree branchTree,
      string templatesFolder,
      out string folderName)
    {
      for (int index = 0; index < TeamFoundationGitPullRequestService.c_templateLocations.Count; ++index)
      {
        string path = TeamFoundationGitPullRequestService.c_templateLocations[index] + templatesFolder;
        TfsGitObject member = PullRequestTemplateUtils.FindMember(branchTree, ref path, out TfsGitTreeEntry _);
        if ((member != null ? (member.ObjectType != GitObjectType.Tree ? 1 : 0) : 1) == 0)
        {
          folderName = path;
          return ((TfsGitTree) member).GetTreeEntries().Where<TfsGitTreeEntry>((Func<TfsGitTreeEntry, bool>) (te => te.ObjectType == GitObjectType.Blob)).Take<TfsGitTreeEntry>(100);
        }
      }
      folderName = (string) null;
      return (IEnumerable<TfsGitTreeEntry>) null;
    }

    private TfsGitTree GetTemplateBranchTree(ITfsGitRepository targetRepository)
    {
      TfsGitRef tfsGitRef = targetRepository.Refs.GetDefault();
      return tfsGitRef == null ? (TfsGitTree) null : targetRepository.LookupObject<TfsGitCommit>(tfsGitRef.ObjectId).GetTree();
    }

    private string ReadFileContent(Stream contentStream, int fileEncoding = 0)
    {
      char[] buffer = new char[4000];
      int length = new StreamReader(contentStream).Read(buffer, 0, 4000);
      return new string(buffer, 0, length);
    }

    public TagDefinition GetPullRequestLabel(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string labelIdOrName)
    {
      requestContext.Trace(1013737, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get Label {0} from pull request {1}", (object) labelIdOrName, (object) pullRequest.PullRequestId);
      IEnumerable<TagDefinition> pullRequestLabels = this.GetPullRequestLabels(requestContext, repository, pullRequest);
      Guid labelId;
      bool parseable = Guid.TryParse(labelIdOrName, out labelId);
      Func<TagDefinition, bool> predicate = (Func<TagDefinition, bool>) (label =>
      {
        if (string.Equals(label.Name, labelIdOrName, StringComparison.OrdinalIgnoreCase))
          return true;
        return parseable && label.TagId.Equals(labelId);
      });
      TagDefinition pullRequestLabel = pullRequestLabels.FirstOrDefault<TagDefinition>(predicate);
      if (pullRequestLabel == null)
      {
        requestContext.Trace(1013742, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Label {0} was not found for pull request {1}", (object) labelIdOrName, (object) pullRequest.PullRequestId);
        throw new GitPullRequestLabelNotFoundException(labelIdOrName);
      }
      requestContext.Trace(1013743, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Label {0} was found for pull request {1}", (object) labelIdOrName, (object) pullRequest.PullRequestId);
      return pullRequestLabel;
    }

    public IEnumerable<TagDefinition> GetPullRequestLabels(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      requestContext.Trace(1013738, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get all pull request Labels for pull request {0}", (object) pullRequest.PullRequestId);
      ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
      TagArtifact<int> tagArtifact = new TagArtifact<int>(repository.Key.ProjectId, pullRequest.PullRequestId);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid pullRequestLabelsId = GitWebApiConstants.PullRequestLabelsId;
      TagArtifact<int> artifact = tagArtifact;
      ArtifactTags<int> tagsForArtifact = service.GetTagsForArtifact<int>(requestContext1, pullRequestLabelsId, artifact);
      requestContext.Trace(1013741, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Successfully gathered labels for pull request {0}", (object) pullRequest.PullRequestId);
      return tagsForArtifact.Tags ?? Enumerable.Empty<TagDefinition>();
    }

    public TagDefinition AddLabelToPullRequest(
      IVssRequestContext requestContext,
      string label,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData = null)
    {
      requestContext.Trace(1013737, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to add label {0} to pull request", (object) pullRequest.PullRequestId);
      return this.AddLabelsToPullRequest(requestContext, new string[1]
      {
        label
      }, repository, pullRequest, ctData).FirstOrDefault<TagDefinition>();
    }

    public IEnumerable<TagDefinition> AddLabelsToPullRequest(
      IVssRequestContext requestContext,
      string[] labels,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData = null)
    {
      if (labels.Length < 1)
        return Enumerable.Empty<TagDefinition>();
      requestContext.Trace(1013738, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to add labels {0} to pull request {1}", (object) string.Join(", ", labels), (object) pullRequest.PullRequestId);
      repository.Permissions.CheckPullRequestContribute();
      ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
      IEnumerable<TagDefinition> source = service.EnsureTagDefinitions(requestContext, (IEnumerable<string>) labels, (IEnumerable<Guid>) new Guid[1]
      {
        GitWebApiConstants.PullRequestLabelsId
      }, repository.Key.ProjectId);
      service.UpdateTagsForArtifact<int>(requestContext.Elevate(), GitWebApiConstants.PullRequestLabelsId, new TagArtifact<int>(repository.Key.ProjectId, pullRequest.PullRequestId), (IEnumerable<Guid>) source.Select<TagDefinition, Guid>((Func<TagDefinition, Guid>) (tag => tag.TagId)).ToArray<Guid>(), (IEnumerable<Guid>) new Guid[0], requestContext.GetUserIdentity().Id, new int?(this.GetFixedVersion(requestContext)));
      requestContext.Trace(1013741, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "added labels labels {0} to pull request {1}", (object) string.Join(", ", labels), (object) pullRequest.PullRequestId);
      PullRequestLabelsNotification notificationEvent = new PullRequestLabelsNotification(repository.GetRepositoryFullUri(), repository.Key.RepoId, repository.Name, pullRequest.PullRequestId, labels);
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
      return source;
    }

    public void VerifyLabelsForPullRequests(
      IVssRequestContext requestContext,
      string[] labels,
      ITfsGitRepository repository)
    {
      requestContext.GetService<ITeamFoundationTaggingService>().EnsureTagDefinitions(requestContext, (IEnumerable<string>) labels, (IEnumerable<Guid>) new Guid[1]
      {
        GitWebApiConstants.PullRequestLabelsId
      }, repository.Key.ProjectId);
    }

    public void RemoveLabelFromPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string labelIdOrName,
      ClientTraceData ctData = null)
    {
      requestContext.Trace(1013737, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to delete label {0} from pull request {1}", (object) string.Join(", ", new string[1]
      {
        labelIdOrName
      }), (object) pullRequest.PullRequestId);
      repository.Permissions.CheckPullRequestContribute();
      try
      {
        ctData?.Add("Delete Label", (object) labelIdOrName);
      }
      catch
      {
      }
      ITeamFoundationTaggingService service = requestContext.GetService<ITeamFoundationTaggingService>();
      TagDefinition pullRequestLabel = this.GetPullRequestLabel(requestContext, repository, pullRequest, labelIdOrName);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid pullRequestLabelsId = GitWebApiConstants.PullRequestLabelsId;
      TagArtifact<int> artifact = new TagArtifact<int>(repository.Key.ProjectId, pullRequest.PullRequestId);
      Guid[] addedTagIds = new Guid[0];
      Guid[] removedTagIds = new Guid[1]
      {
        pullRequestLabel.TagId
      };
      Guid id = requestContext.GetUserIdentity().Id;
      int? version = new int?(this.GetFixedVersion(requestContext));
      service.UpdateTagsForArtifact<int>(requestContext1, pullRequestLabelsId, artifact, (IEnumerable<Guid>) addedTagIds, (IEnumerable<Guid>) removedTagIds, id, version);
      PullRequestLabelsNotification notificationEvent = new PullRequestLabelsNotification(repository.GetRepositoryFullUri(), repository.Key.RepoId, repository.Name, pullRequest.PullRequestId, new string[1]
      {
        labelIdOrName
      });
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
      requestContext.Trace(1013744, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "label {0} successfully deleted", (object) pullRequestLabel);
    }

    private int GetFixedVersion(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestLabelsFixedVersion", 1);

    public GitPullRequestIteration GetIteration(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int iterationId)
    {
      ArgumentUtility.CheckForOutOfRange(iterationId, nameof (iterationId), 1);
      requestContext.Trace(1013601, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request iteration {0} for pull request {1}", (object) iterationId, (object) pullRequest.PullRequestId);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      Iteration iteration;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        if (iterationId != 1)
          throw new GitPullRequestIterationNotFoundException(iterationId);
        requestContext.Trace(1013602, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not support iterations, build synthetic iteration", (object) pullRequest.PullRequestId);
        iteration = pullRequest.BuildSyntheticIteration(requestContext, repository, 1, addChangeEntries: false);
      }
      else
      {
        ICodeReviewIterationService service = requestContext.GetService<ICodeReviewIterationService>();
        requestContext.Trace(1013603, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} supports iterations, getting iteration", (object) pullRequest.PullRequestId);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        int codeReviewId = pullRequest.CodeReviewId;
        int iterationId1 = iterationId;
        iteration = service.GetIteration(requestContext1, projectId, codeReviewId, iterationId1);
        if (iteration == null)
          throw new GitPullRequestIterationNotFoundException(iterationId);
      }
      return iteration.ToGitPullRequestItem(requestContext, pullRequest, securedObject);
    }

    public IEnumerable<GitPullRequestIteration> GetIterations(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      requestContext.Trace(1013606, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get all pull request iterations for pull request {0}", (object) pullRequest.PullRequestId);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      IEnumerable<Iteration> source;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013607, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not support iterations, build synthetic iteration", (object) pullRequest.PullRequestId);
        source = (IEnumerable<Iteration>) new Iteration[1]
        {
          pullRequest.BuildSyntheticIteration(requestContext, repository, 1, addChangeEntries: false)
        };
      }
      else
      {
        requestContext.Trace(1013608, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} supports iterations, getting all iterations", (object) pullRequest.PullRequestId);
        source = (IEnumerable<Iteration>) requestContext.GetService<ICodeReviewIterationService>().GetIterations(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId);
      }
      return source == null ? (IEnumerable<GitPullRequestIteration>) null : source.Select<Iteration, GitPullRequestIteration>((Func<Iteration, GitPullRequestIteration>) (iter => iter.ToGitPullRequestItem(requestContext, pullRequest, securedObject)));
    }

    public GitPullRequestCommentThread GetThread(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int? iteration,
      int? baseIteration)
    {
      requestContext.Trace(1013611, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request thread {0} for pull request {1}", (object) threadId, (object) pullRequest.PullRequestId);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      GitPullRequestCommentThread gitPullRequestItem;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013612, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, getting thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        IVssRequestContext requestContext1 = requestContext;
        int discussionId = threadId;
        ref List<DiscussionComment> local = ref discussionCommentList;
        gitPullRequestItem = service.QueryDiscussionsById(requestContext1, discussionId, out local).ToGitPullRequestItem(requestContext, securedObject);
      }
      else
      {
        requestContext.Trace(1013613, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, getting thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        ICodeReviewCommentService service = requestContext.GetService<ICodeReviewCommentService>();
        Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria1 = (Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria) null;
        if (iteration.HasValue)
        {
          trackingCriteria1 = new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria();
          trackingCriteria1.FirstComparingIteration = baseIteration.HasValue ? baseIteration.Value : 0;
          trackingCriteria1.SecondComparingIteration = iteration.Value;
        }
        IVssRequestContext requestContext2 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        int codeReviewId = pullRequest.CodeReviewId;
        int threadId1 = threadId;
        Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria2 = trackingCriteria1;
        gitPullRequestItem = PullRequestCodeReviewConverter.ToGitPullRequestItem(service.GetCommentThread(requestContext2, projectId, codeReviewId, threadId1, trackingCriteria2, false), requestContext, securedObject);
      }
      this.EnsureCommentThreadScope(pullRequest, gitPullRequestItem, repository.Key.GetProjectUri(), threadId);
      return gitPullRequestItem;
    }

    public IEnumerable<GitPullRequestCommentThread> GetThreads(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int? iteration,
      int? baseIteration)
    {
      requestContext.Trace(1013616, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get all pull request threads for pull request {0}", (object) pullRequest.PullRequestId);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      Dictionary<Guid, IdentityRef> cachedIdentities = new Dictionary<Guid, IdentityRef>();
      IEnumerable<GitPullRequestCommentThread> source1;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013617, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, getting threads by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        List<DiscussionComment> discussionCommentList = (List<DiscussionComment>) null;
        IdentityRef[] identityRefArray = (IdentityRef[]) null;
        string str = pullRequest.BuildArtifactUriForDiscussions(repository.Key.GetProjectUri());
        IVssRequestContext requestContext1 = requestContext;
        string artifactUri = str;
        ref List<DiscussionComment> local1 = ref discussionCommentList;
        ref IdentityRef[] local2 = ref identityRefArray;
        List<DiscussionThread> source2 = service.QueryDiscussionsByArtifactUri(requestContext1, artifactUri, out local1, out local2);
        source1 = source2 != null ? source2.Select<DiscussionThread, GitPullRequestCommentThread>((Func<DiscussionThread, GitPullRequestCommentThread>) (thread => thread.ToGitPullRequestItem(requestContext, securedObject, (IDictionary<Guid, IdentityRef>) cachedIdentities))) : (IEnumerable<GitPullRequestCommentThread>) null;
      }
      else
      {
        requestContext.Trace(1013618, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, getting threads by using CR Comment Service", (object) pullRequest.PullRequestId);
        ICodeReviewCommentService service = requestContext.GetService<ICodeReviewCommentService>();
        Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria1 = (Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria) null;
        if (iteration.HasValue && baseIteration.HasValue)
        {
          trackingCriteria1 = new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria();
          trackingCriteria1.FirstComparingIteration = baseIteration.Value;
          trackingCriteria1.SecondComparingIteration = iteration.Value;
        }
        IVssRequestContext requestContext2 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        int codeReviewId = pullRequest.CodeReviewId;
        DateTime? modifiedSince = new DateTime?();
        Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria trackingCriteria2 = trackingCriteria1;
        List<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread> commentThreads = service.GetCommentThreads(requestContext2, projectId, codeReviewId, modifiedSince, trackingCriteria2, false);
        source1 = commentThreads != null ? commentThreads.Select<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread, GitPullRequestCommentThread>((Func<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread, GitPullRequestCommentThread>) (thread => PullRequestCodeReviewConverter.ToGitPullRequestItem(thread, requestContext, securedObject, (IDictionary<Guid, IdentityRef>) cachedIdentities))) : (IEnumerable<GitPullRequestCommentThread>) null;
      }
      IEnumerable<GitPullRequestCommentThread> array = source1 != null ? (IEnumerable<GitPullRequestCommentThread>) source1.ToArray<GitPullRequestCommentThread>() : (IEnumerable<GitPullRequestCommentThread>) (GitPullRequestCommentThread[]) null;
      this.PopulateCommentLikeIdentities(requestContext, array);
      return array;
    }

    public GitPullRequestCommentThread SaveThread(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestCommentThread prCommentThread)
    {
      requestContext.Trace(1013621, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to create (or update) a pull request thread for pull request {0}", (object) pullRequest.PullRequestId);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      if (prCommentThread.Id >= 1 && prCommentThread.Status == CommentThreadStatus.Active)
      {
        List<DiscussionComment> comments;
        requestContext.GetService<ITeamFoundationDiscussionService>().QueryDiscussionsById(requestContext, prCommentThread.Id, out comments);
        if (comments.Any<DiscussionComment>((Func<DiscussionComment, bool>) (x => x.CommentType == Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.System)))
          prCommentThread.Status = CommentThreadStatus.Unknown;
      }
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013622, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, saving thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        prCommentThread.ArtifactUri = pullRequest.BuildArtifactUriForDiscussions(repository.Key.GetProjectUri());
        Dictionary<Guid, IdentityRef> cachedIdentities = new Dictionary<Guid, IdentityRef>();
        if (prCommentThread.Id >= 1)
        {
          requestContext.Trace(1013623, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "We're updating the thread. Verify that the saved pull request and current pull request ids match");
          DiscussionThread discussionThread = service.QueryDiscussionsById(requestContext, prCommentThread.Id, out List<DiscussionComment> _);
          this.EnsureCommentThreadScope(pullRequest, discussionThread.ToGitPullRequestItem(requestContext, securedObject, (IDictionary<Guid, IdentityRef>) cachedIdentities), repository.Key.GetProjectUri(), prCommentThread.Id);
        }
        IVssRequestContext requestContext1 = requestContext;
        string traceArea = GitServerUtils.TraceArea;
        // ISSUE: variable of a boxed type
        __Boxed<int> pullRequestId = (ValueType) pullRequest.PullRequestId;
        IList<Comment> comments = prCommentThread.Comments;
        // ISSUE: variable of a boxed type
        __Boxed<int> count = (ValueType) (comments != null ? comments.Count : 0);
        requestContext1.Trace(1013624, TraceLevel.Verbose, traceArea, "TfsGitPullRequest", "Now, let's perform the saving operation proper. Pull request id: {0}, comments count: {1}", (object) pullRequestId, (object) count);
        return this.SaveLegacyThread(requestContext, repository.Key.ProjectId, prCommentThread).ToGitPullRequestItem(requestContext, securedObject, (IDictionary<Guid, IdentityRef>) cachedIdentities);
      }
      requestContext.Trace(1013625, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use the legacy format, saving thread by using CR Comment Service", (object) pullRequest.PullRequestId);
      Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread codeReviewItem = prCommentThread.ToCodeReviewItem(requestContext);
      if (!string.IsNullOrEmpty(codeReviewItem?.ThreadContext?.FilePath))
      {
        if (codeReviewItem.Properties == null)
          codeReviewItem.Properties = new PropertiesCollection();
        if (!codeReviewItem.Properties.ContainsKey("Microsoft.TeamFoundation.Discussion.UniqueID"))
          codeReviewItem.Properties.Add("Microsoft.TeamFoundation.Discussion.UniqueID", (object) Guid.NewGuid());
      }
      ICodeReviewCommentService service1 = requestContext.GetService<ICodeReviewCommentService>();
      try
      {
        return PullRequestCodeReviewConverter.ToGitPullRequestItem(service1.SaveCommentThreads(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, (IEnumerable<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread>) new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread[1]
        {
          codeReviewItem
        }, false).FirstOrDefault<Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread>(), requestContext, securedObject);
      }
      catch (MaxDiscussionThreadCountException ex)
      {
        throw new GitPullRequestMaxDiscussionThreadCountException(ex.Message);
      }
    }

    private DiscussionThread SaveLegacyThread(
      IVssRequestContext requestContext,
      Guid projectId,
      GitPullRequestCommentThread prCommentThread)
    {
      this.ValidateCommentThread(projectId, prCommentThread);
      DiscussionThread discussionItem = prCommentThread.ToDiscussionItem();
      return requestContext.GetService<ITeamFoundationDiscussionService>().PublishDiscussions(requestContext, new DiscussionThread[1]
      {
        discussionItem
      }, discussionItem.Comments, (CommentId[]) null).FirstOrDefault<DiscussionThread>();
    }

    private static DiscussionComment CreateNewComment(
      Comment comment,
      int threadId,
      short commentId = 0)
    {
      return new DiscussionComment()
      {
        Author = comment.Author,
        Content = comment.Content,
        DiscussionId = threadId,
        CommentId = commentId,
        ParentCommentId = comment.ParentCommentId,
        CommentType = comment.CommentType.ToCRCommentType()
      };
    }

    public Comment AddComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int inputThreadId,
      Comment comment)
    {
      requestContext.Trace(1013649, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to add a comment to thread {0} for pull request {1}", (object) inputThreadId, (object) pullRequest.PullRequestId);
      ArgumentUtility.CheckForNull<Comment>(comment, nameof (comment));
      int threadId = TeamFoundationGitPullRequestService.CheckThreadId(inputThreadId, comment);
      DiscussionComment newComment = TeamFoundationGitPullRequestService.CreateNewComment(comment, threadId);
      Comment comment1 = (Comment) null;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013650, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, saving thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        DiscussionThread thread;
        this.GetAndValidateThread(requestContext, service, threadId, out thread, out List<DiscussionComment> _);
        List<short> commentIds;
        DateTime lastUpdatedDate;
        service.PublishDiscussions(requestContext, new DiscussionThread[1]
        {
          thread
        }, new DiscussionComment[1]{ newComment }, (CommentId[]) null, out commentIds, out lastUpdatedDate);
        if (commentIds.Count > 0)
        {
          newComment.CommentId = commentIds[0];
          newComment.LastUpdatedDate = lastUpdatedDate;
          newComment.LastContentUpdatedDate = lastUpdatedDate;
          newComment.PublishedDate = lastUpdatedDate;
          newComment.CanDelete = true;
          comment1 = newComment.ToSourceControlItem();
        }
      }
      else
      {
        requestContext.Trace(1013651, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use the legacy format, saving thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        comment1 = requestContext.GetService<ICodeReviewCommentService>().SaveComment(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, newComment, false).ToSourceControlItem();
      }
      return comment1;
    }

    public Comment UpdateComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int inputThreadId,
      int inputCommentId,
      Comment comment)
    {
      requestContext.Trace(1013652, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to update comment {0} to thread {1} for pull request {2}", (object) inputCommentId, (object) inputThreadId, (object) pullRequest.PullRequestId);
      ArgumentUtility.CheckForNull<Comment>(comment, nameof (comment));
      int threadId = TeamFoundationGitPullRequestService.CheckThreadId(inputThreadId, comment);
      short commentId = TeamFoundationGitPullRequestService.CheckCommentId(inputCommentId, comment);
      DiscussionComment newComment = TeamFoundationGitPullRequestService.CreateNewComment(comment, threadId, commentId);
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013653, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, updating comment by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        try
        {
          ArtifactDiscussionThread discussionThread = new ArtifactDiscussionThread();
          discussionThread.DiscussionId = threadId;
          DiscussionThread thread = (DiscussionThread) discussionThread;
          return service.UpdateDiscussionComment(requestContext, thread, newComment).ToSourceControlItem();
        }
        catch (InvalidOperationException ex)
        {
          throw new UnauthorizedAccessException();
        }
      }
      else
      {
        requestContext.Trace(1013654, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use the legacy format, updating comment by using CR Comment Service", (object) pullRequest.PullRequestId);
        ICodeReviewCommentService service = requestContext.GetService<ICodeReviewCommentService>();
        if (newComment.Content == null)
          newComment.Content = (((IEnumerable<DiscussionComment>) service.GetCommentThread(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, addReferenceLinks: false).Comments).FirstOrDefault<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == (int) newComment.CommentId)) ?? throw new GitPullRequestCommentNotFoundException(threadId, (int) newComment.CommentId)).Content;
        try
        {
          return service.SaveComment(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, newComment).ToSourceControlItem();
        }
        catch (CommentCannotBeUpdatedException ex)
        {
          throw new GitPullRequestCommentCannotBeUpdatedException((int) commentId);
        }
        catch (Exception ex)
        {
          throw;
        }
      }
    }

    public void DeleteComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId)
    {
      requestContext.Trace(1013655, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to delete comment {0} from thread {1} for in pull request {2}", (object) commentId, (object) threadId, (object) pullRequest.PullRequestId);
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013656, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, deleting thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        DiscussionThread thread;
        DiscussionComment comment;
        this.GetAndValidateCommentThread(requestContext, service, threadId, commentId, out thread, out comment);
        if (comment.IsDeleted)
          return;
        service.PublishDiscussions(requestContext, new DiscussionThread[1]
        {
          thread
        }, (DiscussionComment[]) null, new CommentId[1]
        {
          new CommentId()
          {
            DiscussionId = threadId,
            Id = (short) commentId
          }
        }, out List<short> _, out DateTime _);
        List<DiscussionComment> comments = (List<DiscussionComment>) null;
        thread = service.QueryDiscussionsById(requestContext, threadId, out comments);
        if (thread != null && comments != null)
          comment = comments.FirstOrDefault<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == commentId));
        if (comment != null && !comment.IsDeleted)
          throw new UnauthorizedAccessException();
      }
      else
      {
        requestContext.Trace(1013657, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, deleting thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        requestContext.GetService<ICodeReviewCommentService>().DeleteComment(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, (short) commentId);
      }
    }

    public void LikeComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId)
    {
      requestContext.Trace(1013720, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to like comment {0} from thread {1} for in pull request {2}", (object) commentId, (object) threadId, (object) pullRequest.PullRequestId);
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013721, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, liking comment in thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        DiscussionThread thread;
        DiscussionComment comment;
        this.GetAndValidateCommentThread(requestContext, service, threadId, commentId, out thread, out comment);
        if (comment.IsDeleted)
          return;
        service.LikeComment(requestContext, thread, (short) commentId);
      }
      else
      {
        requestContext.Trace(1013722, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, liking comment in thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        requestContext.GetService<ICodeReviewCommentService>().LikeComment(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, (short) commentId);
      }
    }

    public List<IdentityRef> GetLikes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId)
    {
      requestContext.Trace(1013730, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get comment likes {0} from thread {1} for in pull request {2}", (object) commentId, (object) threadId, (object) pullRequest.PullRequestId);
      List<IdentityRef> likeIdentities;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013731, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, getting comment likes for thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        DiscussionThread thread;
        this.GetAndValidateCommentThread(requestContext, service, threadId, commentId, out thread, out DiscussionComment _);
        likeIdentities = service.QueryLikes(requestContext, thread, (short) commentId);
      }
      else
      {
        requestContext.Trace(1013732, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, getting comment likes for thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        likeIdentities = requestContext.GetService<ICodeReviewCommentService>().GetLikes(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, (short) commentId);
      }
      this.PopulateCommentLikeIdentities(requestContext, (IEnumerable<IdentityRef>) likeIdentities);
      return likeIdentities;
    }

    public void WithdrawLikeComment(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int threadId,
      int commentId)
    {
      requestContext.Trace(1013723, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to unlike comment {0} from thread {1} for in pull request {2}", (object) commentId, (object) threadId, (object) pullRequest.PullRequestId);
      IdentityRef crIdentityRef = Microsoft.VisualStudio.Services.CodeReview.Server.IdentityExtensions.ToCRIdentityRef(requestContext.GetUserIdentity(), requestContext);
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013724, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses the legacy format, unliking thread by using Discussion Service", (object) pullRequest.PullRequestId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        DiscussionThread thread;
        DiscussionComment comment;
        this.GetAndValidateCommentThread(requestContext, service, threadId, commentId, out thread, out comment);
        if (comment.IsDeleted)
          return;
        service.WithdrawLikeComment(requestContext, thread, (short) commentId, crIdentityRef);
      }
      else
      {
        requestContext.Trace(1013725, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} does not use legacy format, unliking thread by using CR Comment Service", (object) pullRequest.PullRequestId);
        requestContext.GetService<ICodeReviewCommentService>().WithdrawLikeComment(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, threadId, (short) commentId, crIdentityRef);
      }
    }

    private bool DoesNotSupportIterations(TfsGitPullRequest pullRequest) => !pullRequest.SupportsIterations || pullRequest.CodeReviewId <= 0;

    private void ValidateCommentThread(Guid projectId, GitPullRequestCommentThread thread)
    {
      if (thread.Id < 1 && thread.ThreadContext != null)
      {
        if (string.IsNullOrEmpty(thread.ThreadContext.FilePath))
          throw new ArgumentNullException("FilePath", Resources.Get("PullRequestPathCannotBeNullForThreadsNotAtReviewLevel"));
        GitPullRequestCommentThreadContext requestThreadContext = thread.PullRequestThreadContext;
        if (requestThreadContext.IterationContext != null)
        {
          ArgumentUtility.CheckForOutOfRange((int) requestThreadContext.IterationContext.FirstComparingIteration, "FirstComparingIteration", 1);
          ArgumentUtility.CheckForOutOfRange((int) requestThreadContext.IterationContext.SecondComparingIteration, "SecondComparingIteration", 1);
        }
      }
      if (thread.Id >= 1)
      {
        if (thread.Properties != null)
          throw new ArgumentException(Resources.Get("PullRequestCommentThreadPropertiesCannotBeFound"), "Properties");
        if (thread.ThreadContext != null)
          throw new ArgumentException(Resources.Get("PullRequestCommentThreadContextCannotBeUpdated"), "ThreadContext");
        thread.MarkForUpdate = true;
      }
      if (thread.Comments == null)
        return;
      foreach (Comment comment in (IEnumerable<Comment>) thread.Comments)
        this.ValidateComment(thread.Id, comment);
    }

    private void ValidateComment(int threadId, Comment comment)
    {
      ArgumentUtility.CheckForNull<Comment>(comment, nameof (comment));
      comment.ThreadId = threadId;
      if (comment.Id < (short) 1)
      {
        if (string.IsNullOrEmpty(comment.Content))
          throw new ArgumentNullException("content", Resources.Get("PullRequestCommentWithNoContent"));
      }
      else if (comment.Author != null)
        throw new ArgumentException(Resources.Get("PullRequestCommentAuthorCannotBeUpdated"), "author");
    }

    private void EnsureCommentThreadScope(
      TfsGitPullRequest pullRequest,
      GitPullRequestCommentThread prThread,
      string projectUri,
      int threadId)
    {
      string b = pullRequest.BuildArtifactUriForDiscussions(projectUri);
      if (prThread == null)
        throw new GitPullRequestCommentThreadNotFoundException(threadId);
      if (!string.Equals(prThread.ArtifactUri, b, StringComparison.OrdinalIgnoreCase))
        throw new GitPullRequestCommentThreadNotFoundException(threadId);
    }

    private static int CheckThreadId(int threadId, Comment comment)
    {
      if (comment.ThreadId != 0 && threadId != 0 && comment.ThreadId != threadId)
        throw new GitPullRequestInvalidParametersException();
      return threadId == 0 ? comment.ThreadId : threadId;
    }

    private static short CheckCommentId(int commentId, Comment comment)
    {
      if (commentId != 0 && comment.Id != (short) 0 && commentId != (int) comment.Id)
        throw new GitPullRequestInvalidParametersException();
      if (commentId == 0 && comment.Id == (short) 0)
        throw new GitPullRequestInvalidParametersException();
      return comment.Id == (short) 0 ? (short) commentId : comment.Id;
    }

    private void GetAndValidateThread(
      IVssRequestContext requestContext,
      ITeamFoundationDiscussionService discussionService,
      int threadId,
      out DiscussionThread thread,
      out List<DiscussionComment> comments)
    {
      thread = discussionService.QueryDiscussionsById(requestContext, threadId, out comments);
      if (thread == null || comments == null)
        throw new GitPullRequestCommentThreadNotFoundException(threadId);
    }

    private void GetAndValidateCommentThread(
      IVssRequestContext requestContext,
      ITeamFoundationDiscussionService discussionService,
      int threadId,
      int commentId,
      out DiscussionThread thread,
      out DiscussionComment comment)
    {
      List<DiscussionComment> comments = (List<DiscussionComment>) null;
      this.GetAndValidateThread(requestContext, discussionService, threadId, out thread, out comments);
      comment = comments.FirstOrDefault<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == commentId));
      if (comment == null)
        throw new GitPullRequestCommentNotFoundException(threadId, commentId);
    }

    private void PopulateCommentLikeIdentities(
      IVssRequestContext requestContext,
      IEnumerable<GitPullRequestCommentThread> commentThreads)
    {
      if (commentThreads == null || !commentThreads.Any<GitPullRequestCommentThread>())
        return;
      List<IdentityRef> likeIdentities = new List<IdentityRef>();
      foreach (GitPullRequestCommentThread commentThread in commentThreads)
      {
        if (commentThread.Comments != null && commentThread.Comments.Any<Comment>())
        {
          foreach (Comment comment in (IEnumerable<Comment>) commentThread.Comments)
          {
            if (comment.UsersLiked != null && comment.UsersLiked.Any<IdentityRef>())
            {
              foreach (IdentityRef identityRef in (IEnumerable<IdentityRef>) comment.UsersLiked)
                likeIdentities.Add(identityRef);
            }
          }
        }
      }
      this.PopulateCommentLikeIdentities(requestContext, (IEnumerable<IdentityRef>) likeIdentities);
    }

    private void PopulateCommentLikeIdentities(
      IVssRequestContext requestContext,
      IEnumerable<IdentityRef> likeIdentities)
    {
      if (likeIdentities == null || !likeIdentities.Any<IdentityRef>())
        return;
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      TeamFoundationIdentity[] foundationIdentityArray = IdentityHelper.Instance.GetUserIdentities(requestContext, service, likeIdentities.Select<IdentityRef, Guid>((Func<IdentityRef, Guid>) (i => Guid.Parse(i.Id))).Distinct<Guid>().ToArray<Guid>()) ?? Array.Empty<TeamFoundationIdentity>();
      Dictionary<string, IdentityRef> dictionary = new Dictionary<string, IdentityRef>();
      foreach (TeamFoundationIdentity identity in foundationIdentityArray)
      {
        IdentityRef identityRef = identity != null ? identity.ToIdentityRef(requestContext) : (IdentityRef) null;
        if (!string.IsNullOrEmpty(identityRef?.Id))
          dictionary[identityRef.Id] = identityRef;
      }
      foreach (IdentityRef likeIdentity in likeIdentities)
      {
        if (dictionary.ContainsKey(likeIdentity.Id))
        {
          IdentityRef identityRef = dictionary[likeIdentity.Id];
          likeIdentity.Id = identityRef.Id;
          likeIdentity.DisplayName = identityRef.DisplayName;
          likeIdentity.UniqueName = identityRef.UniqueName;
          likeIdentity.ImageUrl = identityRef.ImageUrl;
          likeIdentity.Url = identityRef.Url;
        }
      }
    }

    public GitPullRequestStatus GetStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int statusId,
      int? iterationId = null)
    {
      requestContext.Trace(1013631, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request status {0} for pull request {1}", (object) statusId, (object) pullRequest.PullRequestId);
      requestContext.Trace(1013632, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is backed by a code review, getting status", (object) pullRequest.PullRequestId);
      Status status;
      try
      {
        status = CodeReviewRequestContextCacheUtil.GetStatus(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, statusId, iterationId);
      }
      catch (CodeReviewStatusNotFoundException ex)
      {
        throw new GitPullRequestStatusNotFoundException(statusId);
      }
      catch (Exception ex)
      {
        throw;
      }
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      return status.ToGitPullRequestStatusItem(securedObject);
    }

    public IEnumerable<GitPullRequestStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int? iterationId = null,
      bool includeProperties = false)
    {
      requestContext.Trace(1013636, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get all pull request statuses for pull request {0} or iteration {1}", (object) pullRequest.PullRequestId, (object) iterationId);
      requestContext.Trace(1013637, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is backed by a code review, getting statuses", (object) pullRequest.PullRequestId);
      IEnumerable<Status> source;
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013638, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses no-iteration discussion format, Not able to get statuses for iteration {1}", (object) pullRequest.PullRequestId, iterationId.HasValue ? (object) iterationId.ToString() : (object) "null");
        source = (IEnumerable<Status>) new List<Status>();
      }
      else
        source = CodeReviewRequestContextCacheUtil.GetStatusesOrDefault(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, iterationId, includeProperties);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      return source == null ? (IEnumerable<GitPullRequestStatus>) null : source.Select<Status, GitPullRequestStatus>((Func<Status, GitPullRequestStatus>) (status => status.ToGitPullRequestStatusItem(securedObject)));
    }

    public ILookup<int, GitPullRequestStatus> GetStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      IEnumerable<TfsGitPullRequest> pullRequests)
    {
      requestContext.Trace(1013641, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get all pull request statuses for pull requests {0}", (object) this.GetStringForMultiplePullRequestIds(requestContext, pullRequests));
      ILookup<int, GitPullRequestStatus> statuses1 = (ILookup<int, GitPullRequestStatus>) null;
      requestContext.Trace(1013642, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is backed by a code review, getting statuses", (object) this.GetStringForMultiplePullRequestIds(requestContext, pullRequests));
      Dictionary<int, int> codeReviewPullRequestIdMapping = new Dictionary<int, int>();
      foreach (TfsGitPullRequest pullRequest in pullRequests)
      {
        if (!codeReviewPullRequestIdMapping.ContainsKey(pullRequest.CodeReviewId))
          codeReviewPullRequestIdMapping.Add(pullRequest.CodeReviewId, pullRequest.PullRequestId);
      }
      ILookup<int, Status> statuses2 = requestContext.GetService<ICodeReviewStatusService>().GetStatuses(requestContext, repository.Key.ProjectId, (IEnumerable<int>) codeReviewPullRequestIdMapping.Values);
      if (statuses2 != null && statuses2.Any<IGrouping<int, Status>>())
      {
        Dictionary<int, List<Status>> dictionary = statuses2.ToDictionary<IGrouping<int, Status>, int, List<Status>>((Func<IGrouping<int, Status>, int>) (item => item.Key), (Func<IGrouping<int, Status>, List<Status>>) (item => item.ToList<Status>()));
        ISecuredObject securedObject = this.GetSecuredObject(repository);
        statuses1 = dictionary.SelectMany(mapping => mapping.Value.Select(status => new
        {
          Key = mapping.Key,
          Value = status
        })).ToLookup(pair => codeReviewPullRequestIdMapping[pair.Key], pair => pair.Value.ToGitPullRequestStatusItem(securedObject));
      }
      return statuses1;
    }

    public IEnumerable<GitStatusContext> GetLatestStatusContexts(
      IVssRequestContext requestContext,
      ITfsGitRepository repository)
    {
      TracepointUtils.Tracepoint(requestContext, 1013816, GitServerUtils.TraceArea, "TfsGitPullRequest", (Func<object>) (() => (object) new
      {
        ProjectId = repository.Key.ProjectId,
        RepoId = repository.Key.RepoId
      }), caller: nameof (GetLatestStatusContexts));
      ICodeReviewStatusService service = requestContext.GetService<ICodeReviewStatusService>();
      string artifactUriPrefix = PullRequestArtifactId.GetPullRequestArtifactUriPrefix(repository.Key.GetProjectUri(), repository.Key.RepoId);
      IVssRequestContext requestContext1 = requestContext;
      Guid projectId = repository.Key.ProjectId;
      string reviewArtifactPrefix = artifactUriPrefix;
      IEnumerable<StatusContext> sourceArtifactPrefix = service.GetLatestStatusContextsBySourceArtifactPrefix(requestContext1, projectId, reviewArtifactPrefix);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      return sourceArtifactPrefix == null ? (IEnumerable<GitStatusContext>) null : sourceArtifactPrefix.Select<StatusContext, GitStatusContext>((Func<StatusContext, GitStatusContext>) (statusContext => statusContext.ToGitStatusContext(securedObject)));
    }

    public IEnumerable<GitPullRequestStatus> GetLatestStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int top = 500)
    {
      TracepointUtils.Tracepoint(requestContext, 1013735, GitServerUtils.TraceArea, "TfsGitPullRequest", (Func<object>) (() => (object) new
      {
        ProjectId = repository.Key.ProjectId,
        RepoId = repository.Key.RepoId,
        PullRequestId = pullRequest.PullRequestId,
        top = top
      }), caller: nameof (GetLatestStatuses));
      if (pullRequest.CodeReviewId <= 0)
      {
        requestContext.Trace(1013736, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is not backed by a code review, repositoryId={1}", (object) pullRequest.PullRequestId, (object) repository.Key.RepoId);
        return (IEnumerable<GitPullRequestStatus>) new List<GitPullRequestStatus>();
      }
      IEnumerable<Status> statusesByReviewId = requestContext.GetService<ICodeReviewStatusService>().GetLatestStatusesByReviewId(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, top);
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      return statusesByReviewId == null ? (IEnumerable<GitPullRequestStatus>) null : statusesByReviewId.Select<Status, GitPullRequestStatus>((Func<Status, GitPullRequestStatus>) (status => status.ToGitPullRequestStatusItem(securedObject)));
    }

    public GitPullRequestStatus SaveStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestStatus prStatus,
      int? iterationId = null)
    {
      requestContext.Trace(1013646, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to create (or update) a pull request status for pull request {0} or iteration {1}", (object) pullRequest.PullRequestId, (object) iterationId);
      bool updateStatus = prStatus.Id > 0;
      this.ValidateStatus(prStatus, updateStatus);
      int? iterationId1 = iterationId;
      if (!updateStatus && !iterationId1.HasValue)
      {
        iterationId1 = prStatus.IterationId;
        requestContext.Trace(1013741, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses iteration {1} from the status object.", (object) pullRequest.PullRequestId, (object) iterationId1);
      }
      if (this.DoesNotSupportIterations(pullRequest) && iterationId1.HasValue)
      {
        requestContext.Trace(1013648, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} uses no-iteration discussion format, Not able to save status for iteration {1}.", (object) pullRequest.PullRequestId, (object) iterationId1);
        throw new GitPullRequestStatusCannotBeCreatedException();
      }
      requestContext.Trace(1013647, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is backed by a code review, saving statuses for iteration {1}", (object) pullRequest.PullRequestId, (object) iterationId1);
      ICodeReviewStatusService service = requestContext.GetService<ICodeReviewStatusService>();
      Status status;
      try
      {
        status = service.SaveStatus(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, prStatus.ToCodeReviewItem(), iterationId1);
      }
      catch (CodeReviewIterationNotFoundException ex)
      {
        throw new GitPullRequestIterationNotFoundException(iterationId1.Value);
      }
      catch (CodeReviewActionRejectedByPolicyException ex)
      {
        if (ex.InnerException is ActionDeniedBySubscriberException)
          throw new GitPullRequestStatusRejectedByPolicyException(ex.InnerException.Message);
        throw new GitPullRequestStatusRejectedByPolicyException();
      }
      catch (Exception ex)
      {
        throw;
      }
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      return status == null ? (GitPullRequestStatus) null : status.ToGitPullRequestStatusItem(securedObject);
    }

    public void DeleteStatuses(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      IEnumerable<int> statusIds,
      int? iterationId = null)
    {
      ICodeReviewStatusService service = requestContext.GetService<ICodeReviewStatusService>();
      if (pullRequest.Status != PullRequestStatus.Active)
        throw new GitPullRequestStatusNotEditableException();
      try
      {
        service.DeleteStatuses(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, statusIds, iterationId);
      }
      catch (CodeReviewStatusNotFoundException ex)
      {
        throw new GitPullRequestStatusNotFoundException(ex.StatusId);
      }
      catch (CodeReviewNotActiveException ex)
      {
        throw new GitPullRequestStatusNotEditableException();
      }
      catch (CodeReviewIterationNotFoundException ex)
      {
        throw new GitPullRequestIterationNotFoundException(ex.IterationId);
      }
    }

    private void ValidateStatus(GitPullRequestStatus status, bool updateStatus)
    {
      if (updateStatus)
      {
        if (status.Context?.Name != null)
          throw new ArgumentException(Resources.Get("PullRequestStatusNameCannotBeUpdated"), "name").Expected("git");
        if (status.Context?.Genre != null)
          throw new ArgumentException(Resources.Get("PullRequestStatusGenreCannotBeUpdated"), "genre").Expected("git");
      }
      else
      {
        if (status.Context == null)
          throw new ArgumentException(Resources.Get("PullRequestStatusContextNotAvailable"), "context").Expected("git");
        ArgumentUtility.CheckStringForNullOrEmpty(status.Context.Name, "name");
      }
    }

    private string GetStringForMultiplePullRequestIds(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest> pullRequests)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (pullRequests != null)
      {
        foreach (TfsGitPullRequest pullRequest in pullRequests)
          stringBuilder.Append(string.Format("'{0}'", (object) pullRequest.PullRequestId));
      }
      return stringBuilder.ToString();
    }

    public PropertiesCollection GetPullRequestProperties(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      requestContext.Trace(1013770, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Get pull request {0} properties.", (object) pullRequest.PullRequestId);
      if (pullRequest.CodeReviewId <= 0)
      {
        requestContext.Trace(1013771, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is not backed by a code review. Properties not supported.", (object) pullRequest.PullRequestId);
        throw new GitPullRequestPropertiesNotSupportedException();
      }
      return requestContext.GetService<ICodeReviewService>().GetProperties(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId);
    }

    public PropertiesCollection UpdatePullRequestProperties(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      PropertiesCollection properties)
    {
      requestContext.Trace(1013772, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Update pull request {0} properties.", (object) pullRequest.PullRequestId);
      if (pullRequest.CodeReviewId <= 0)
      {
        requestContext.Trace(1013773, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} is not backed by a code review. Properties not supported.", (object) pullRequest.PullRequestId);
        throw new GitPullRequestPropertiesNotSupportedException();
      }
      ClientTraceData properties1 = new ClientTraceData();
      properties1.Add("pullRequestId", (object) pullRequest.PullRequestId);
      properties1.Add("repositoryId", (object) pullRequest.RepositoryId);
      properties1.Add(nameof (properties), (object) properties.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (prop => prop.Key), (Func<KeyValuePair<string, object>, object>) (prop => prop.Value)));
      ICodeReviewService service = requestContext.GetService<ICodeReviewService>();
      PropertiesCollection propertiesCollection;
      try
      {
        propertiesCollection = service.PatchProperties(requestContext, repository.Key.ProjectId, pullRequest.CodeReviewId, properties);
      }
      catch (CodeReviewNotActiveException ex)
      {
        throw new GitPullRequestNotEditableException();
      }
      try
      {
        requestContext.GetService<ClientTraceService>().Publish(requestContext, GitServerUtils.TraceArea, nameof (UpdatePullRequestProperties), properties1);
      }
      catch
      {
      }
      return propertiesCollection;
    }

    private static void SetNextTopAndSkip(
      GitPullRequestIterationChanges changes,
      int? top,
      int? skip,
      int totalChanges)
    {
      int val2 = Math.Min(!top.HasValue || top.Value <= 0 ? 100 : top.Value, 2000);
      int num1 = !skip.HasValue || skip.Value < 0 ? 0 : skip.Value;
      int num2 = val2 + num1;
      int val1 = totalChanges - num2;
      if (val1 > 0)
      {
        changes.NextTop = Math.Min(val1, val2);
        changes.NextSkip = num2;
      }
      else
      {
        changes.NextTop = 0;
        changes.NextSkip = 0;
      }
    }

    public GitPullRequestIterationChanges GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      int iterationId,
      out int totalChanges,
      int? top,
      int? skip,
      int? compareTo)
    {
      totalChanges = 0;
      ISecuredObject securedObject = this.GetSecuredObject(repository);
      requestContext.Trace(1013583, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request changes for pull request {0}, iteration {1}", (object) pullRequest.PullRequestId, (object) iterationId);
      if (this.DoesNotSupportIterations(pullRequest))
      {
        requestContext.Trace(1013584, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request changes for pull request {0}, iteration {1} using legacy data", (object) pullRequest.PullRequestId, (object) iterationId);
        if (iterationId != 1)
          throw new GitPullRequestIterationNotFoundException(iterationId);
        if (compareTo.HasValue)
        {
          if (compareTo.Value == 1)
            return new GitPullRequestIterationChanges(securedObject);
          if (compareTo.Value != 0)
            throw new GitPullRequestIterationNotFoundException(compareTo.Value);
        }
        IReadOnlyList<TfsGitCommitChange> changes1 = pullRequest.GetChanges(requestContext);
        if (changes1 != null)
        {
          requestContext.Trace(1013585, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Found {2} changes for pull request {0}, iteration {1} using legacy data", (object) pullRequest.PullRequestId, (object) iterationId, (object) changes1.Count<TfsGitCommitChange>());
          IList<ChangeEntry> changeEntryList = PullRequestCodeReviewSdkExtensions.ConvertChanges(changes1, true);
          if (changeEntryList != null)
          {
            requestContext.Trace(1013586, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Converted {2} changes for pull request {0}, iteration {1} using legacy data", (object) pullRequest.PullRequestId, (object) iterationId, (object) changeEntryList.Count<ChangeEntry>());
            totalChanges = changeEntryList.Count<ChangeEntry>();
            int val2 = Math.Min(!top.HasValue || top.Value <= 0 ? 100 : top.Value, 2000);
            int index = !skip.HasValue || skip.Value < 0 ? 0 : skip.Value;
            if (index > 0 || val2 < totalChanges)
            {
              if (index >= totalChanges)
              {
                changeEntryList = (IList<ChangeEntry>) new List<ChangeEntry>();
              }
              else
              {
                int count = Math.Min(totalChanges - index, val2);
                changeEntryList = (IList<ChangeEntry>) changeEntryList.ToList<ChangeEntry>().GetRange(index, count);
                int num = index + 1;
                foreach (ChangeEntry changeEntry in (IEnumerable<ChangeEntry>) changeEntryList)
                {
                  changeEntry.ChangeId = new int?(num);
                  ++num;
                }
              }
            }
            else
            {
              int num = 1;
              foreach (ChangeEntry changeEntry in (IEnumerable<ChangeEntry>) changeEntryList)
              {
                changeEntry.ChangeId = new int?(num);
                ++num;
              }
            }
            GitPullRequestIterationChanges changes2 = new GitPullRequestIterationChanges(securedObject);
            changes2.ChangeEntries = changeEntryList.ToGetPullRequestItem(false, securedObject);
            TeamFoundationGitPullRequestService.SetNextTopAndSkip(changes2, top, skip, totalChanges);
            return changes2;
          }
        }
      }
      else
      {
        Review codeReview = pullRequest.GetCodeReview(requestContext, repository.Key.GetProjectUri(), CodeReviewExtendedProperties.None);
        requestContext.Trace(1013587, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to get pull request changes for pull request {0}, iteration {1} using code review service", (object) pullRequest.PullRequestId, (object) iterationId);
        bool flag = false;
        foreach (Iteration iteration in (IEnumerable<Iteration>) codeReview.Iterations)
        {
          int? id = iteration.Id;
          int num = iterationId;
          if (id.GetValueOrDefault() == num & id.HasValue)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          throw new GitPullRequestIterationNotFoundException(iterationId);
        ICodeReviewIterationService service = requestContext.GetService<ICodeReviewIterationService>();
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        int id1 = codeReview.Id;
        List<int> iterationIds = new List<int>();
        iterationIds.Add(iterationId);
        int? top1 = top;
        int? skip1 = skip;
        int? baseIteration = compareTo;
        IEnumerable<ChangeEntry> changeList = service.GetChangeList(requestContext1, projectId, id1, iterationIds, top1, skip1, baseIteration);
        if (changeList != null)
        {
          int? nullable = compareTo;
          int num = 0;
          bool computeChangeType = nullable.GetValueOrDefault() > num & nullable.HasValue;
          if (changeList.Any<ChangeEntry>())
            totalChanges = changeList.First<ChangeEntry>().TotalChangesCount;
          GitPullRequestIterationChanges changes = new GitPullRequestIterationChanges(securedObject);
          changes.ChangeEntries = changeList.ToGetPullRequestItem(computeChangeType, securedObject);
          TeamFoundationGitPullRequestService.SetNextTopAndSkip(changes, top, skip, totalChanges);
          return changes;
        }
      }
      return new GitPullRequestIterationChanges(securedObject);
    }

    private ISecuredObject GetSecuredObject(ITfsGitRepository repository) => GitSecuredObjectFactory.CreateRepositoryReadOnly(repository.Key);

    internal string PerformAutoCompleteJob(
      IVssRequestContext requestContext,
      Guid jobId,
      XmlNode jobDataXml,
      ClientTraceData ctData)
    {
      Guid repositoryId;
      int pullRequestId;
      TeamFoundationGitPullRequestService.ParseAutoCompleteJobDataXml(jobDataXml, out repositoryId, out pullRequestId, out Guid _);
      ctData.Add("RepositoryId", (object) repositoryId);
      ctData.Add("PullRequestId", (object) pullRequestId);
      ITeamFoundationGitRepositoryService service1 = requestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repository = service1.FindRepositoryById(requestContext, repositoryId))
      {
        ctData.Add("RepositoryName", (object) repository.Name);
        TfsGitPullRequest pullRequest = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        if (!requestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.AutoComplete") || pullRequest.BelongsToCompletionJob || pullRequest.Status != PullRequestStatus.Active)
        {
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = repository.Key.ProjectId;
          TfsGitPullRequest pullRequest1 = pullRequest;
          Guid? nullable = new Guid?(Guid.Empty);
          PullRequestStatus? status = new PullRequestStatus?();
          PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
          Sha1Id? lastMergeSourceCommit = new Sha1Id?();
          Sha1Id? lastMergeTargetCommit = new Sha1Id?();
          Sha1Id? lastMergeCommit = new Sha1Id?();
          Guid? completeWhenMergedAuthority = new Guid?();
          PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime = new DateTime?();
          Guid? autoCompleteAuthority = nullable;
          bool? isDraft = new bool?();
          this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
          throw new GitPullRequestAutoCompleteFailedException(string.Format("Pull request {0} on repo {1} not autocompleted due to its state. Status: {2}, Belongs to completion job: {3}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.BelongsToCompletionJob), (string) null);
        }
        if (pullRequest.AutoCompleteAuthority == Guid.Empty || pullRequest.MergeStatus != PullRequestAsyncStatus.Succeeded || pullRequest.IsDraft)
          throw new GitPullRequestAutoCompleteFailedException(string.Format("Pull request {0} on repo {1} not autocompleted due to its state. Status: {2}, Belongs to completion job: {3}, Auto complete authority: {4}, Merge status: {5}, IsDraft: {6}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.BelongsToCompletionJob, (object) pullRequest.AutoCompleteAuthority, (object) pullRequest.MergeStatus, (object) pullRequest.IsDraft), (string) null);
        ITeamFoundationPolicyService service2 = requestContext.GetService<ITeamFoundationPolicyService>();
        string projectUri = ProjectInfo.GetProjectUri(repository.Key.ProjectId);
        ArtifactId artifactId = pullRequest.BuildLegacyArtifactId(projectUri);
        GitPullRequestTarget target = new GitPullRequestTarget(projectUri, pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequest));
        List<PolicyConfigurationRecord> blockingAutocompletePolicies = new List<PolicyConfigurationRecord>();
        List<int> policiesToRequeue = new List<int>();
        bool shouldRequeuePolicies = true;
        bool mergeStrategyPolicyChanged = false;
        HashSet<int> ignoreConfigIds = (HashSet<int>) null;
        if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.SelectiveAutoComplete") && pullRequest.CompletionOptions?.AutoCompleteIgnoreConfigIds != null)
          ignoreConfigIds = new HashSet<int>((IEnumerable<int>) pullRequest.CompletionOptions?.AutoCompleteIgnoreConfigIds);
        service2.NotifyPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) target, artifactId, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) =>
        {
          if (!policy.Configuration.IsBlocking)
          {
            HashSet<int> intSet = ignoreConfigIds;
            // ISSUE: explicit non-virtual call
            if ((intSet != null ? (!__nonvirtual (intSet.Contains(policy.Configuration.ConfigurationId)) ? 1 : 0) : 1) == 0)
              goto label_8;
          }
          AutoCompleteStatus autoCompleteStatus = policy.GetAutoCompleteStatus(requestContext, repository, pullRequest.CompletionOptions, pullRequest, existingStatus, existingContext);
          if (autoCompleteStatus != AutoCompleteStatus.NotBlocking)
          {
            if (policy is ITeamFoundationMergeStrategyRestrictionPolicy)
              mergeStrategyPolicyChanged = true;
            this.RecordBlockingPolicyInCtData(ctData, policy);
            blockingAutocompletePolicies.Add(policy.Configuration);
            if (autoCompleteStatus == AutoCompleteStatus.BlockingCanAutoRequeue)
              policiesToRequeue.Add(policy.Configuration.ConfigurationId);
            else
              shouldRequeuePolicies = false;
          }
label_8:
          return (PolicyNotificationResult) null;
        }), ctData);
        if (mergeStrategyPolicyChanged)
        {
          IVssRequestContext requestContext2 = requestContext;
          Guid projectId = repository.Key.ProjectId;
          TfsGitPullRequest pullRequest2 = pullRequest;
          Guid? nullable = new Guid?(Guid.Empty);
          PullRequestStatus? status = new PullRequestStatus?();
          PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
          Sha1Id? lastMergeSourceCommit = new Sha1Id?();
          Sha1Id? lastMergeTargetCommit = new Sha1Id?();
          Sha1Id? lastMergeCommit = new Sha1Id?();
          Guid? completeWhenMergedAuthority = new Guid?();
          PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime = new DateTime?();
          Guid? autoCompleteAuthority = nullable;
          bool? isDraft = new bool?();
          this.UpdatePullRequestInDatabase(requestContext2, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
          string str = string.Format("Pull request {0} on repo {1} not autocompleted because merge strategy policy changed. Autocomplete cancelled. Status: {2}, Belongs to completion job: {3}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.BelongsToCompletionJob);
          requestContext.TraceAlways(1013466, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", str);
          throw new GitPullRequestAutoCompleteFailedException(str, Resources.Get("MergeStrategyNotAllowed"));
        }
        int num = 0;
        if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.EnforceAdHocRequiredReviewers") && blockingAutocompletePolicies.Count == 0)
          num = pullRequest.Reviewers.Count<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => r.IsRequired && !r.Approves));
        if (blockingAutocompletePolicies.Count + num == 0)
        {
          pullRequest = this.ReadyToAutoComplete(requestContext, pullRequest);
          return string.Format("Autocomplete successfully attempted for PR: {0} on repository {1}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId);
        }
        shouldRequeuePolicies &= num == 0;
        if (shouldRequeuePolicies && policiesToRequeue.Count<int>() <= TeamFoundationGitPullRequestService.s_maxPoliciesAutoCompleteShouldRequeue)
        {
          foreach (int policyConfigurationId in policiesToRequeue)
            service2.NotifyPolicy<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) target, artifactId, policyConfigurationId, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) => policy.AutoRequeue(requestContext, existingStatus, existingContext, target)), ctData);
          ctData.Add("AutoCompleteRequeuedBuilds", (object) policiesToRequeue.Count);
          throw new GitPullRequestAutoCompleteFailedException(string.Format("{0} builds were automatically requeued for PR: {1} on repository {2}. Status: {3}, Auto complete authority: {4}", (object) policiesToRequeue.Count, (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.AutoCompleteAuthority), (string) null);
        }
        throw new GitPullRequestAutoCompleteFailedException(string.Format("One or more policies blocked autocompletion for PR: {0} on repository {1}. Status: {2}, Belongs to completion job: {3}, Auto complete authority: {4}, Merge status: {5}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.BelongsToCompletionJob, (object) pullRequest.AutoCompleteAuthority, (object) pullRequest.MergeStatus), (string) null);
      }
    }

    internal TfsGitPullRequest ReadyToAutoComplete(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return TracepointUtils.TraceBlock<TfsGitPullRequest>(requestContext, 1013513, GitServerUtils.TraceArea, "TfsGitPullRequest", (Func<TfsGitPullRequest>) (() =>
      {
        IdentityDescriptor identityDescriptor = IdentityHelper.Instance.GetIdentityDescriptor(requestContext, pullRequest.AutoCompleteAuthority);
        if (identityDescriptor == (IdentityDescriptor) null)
        {
          requestContext.Trace(1013511, TraceLevel.Error, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", "Can't find the user who authorized pull request completion");
          return pullRequest;
        }
        try
        {
          using (IVssRequestContext userContext = requestContext.CreateUserContext(identityDescriptor))
          {
            userContext.ProgressTimerJoin(requestContext);
            using (ITfsGitRepository repositoryById = userContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(userContext, pullRequest.RepositoryId))
              pullRequest = !requestContext.IsFeatureEnabled("Git.PullRequests.AtomicIterationSave") ? this.TryCommit(userContext, repositoryById, pullRequest.PullRequestId, Sha1Id.Empty, pullRequest.CompletionOptions, false, true) : this.TryCommit(userContext, repositoryById, pullRequest.PullRequestId, pullRequest.LastMergeSourceCommit ?? Sha1Id.Empty, pullRequest.CompletionOptions, false, true);
          }
        }
        catch (Exception ex)
        {
          throw new GitPullRequestAutoCompleteFailedException(string.Format("Pull request {0} on repo {1} not autocompleted due to an exception. Status: {2}, Belongs to completion job: {3}, Auto complete authority: {4}, Merge status: {5}, Exception: {6}", (object) pullRequest.PullRequestId, (object) pullRequest.RepositoryId, (object) pullRequest.Status, (object) pullRequest.BelongsToCompletionJob, (object) pullRequest.AutoCompleteAuthority, (object) pullRequest.MergeStatus, (object) ex.Message), TeamFoundationGitPullRequestService.GetAutoCompleteExpectedFailureMessage(requestContext, ex, pullRequest.AutoCompleteAuthority) ?? Resources.Get("PullRequestAutoCompleteFailedReason_UnexpectedError"));
        }
        finally
        {
          using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
          {
            IVssRequestContext requestContext1 = requestContext;
            Guid projectId = repositoryById.Key.ProjectId;
            TfsGitPullRequest pullRequest1 = pullRequest;
            Guid? nullable = new Guid?(Guid.Empty);
            PullRequestStatus? status = new PullRequestStatus?();
            PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
            Sha1Id? lastMergeSourceCommit = new Sha1Id?();
            Sha1Id? lastMergeTargetCommit = new Sha1Id?();
            Sha1Id? lastMergeCommit = new Sha1Id?();
            Guid? completeWhenMergedAuthority = new Guid?();
            PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
            DateTime? completionQueueTime = new DateTime?();
            Guid? autoCompleteAuthority = nullable;
            bool? isDraft = new bool?();
            pullRequest = this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
          }
        }
        return pullRequest;
      }), (Func<object>) (() => (object) new
      {
        PullRequestId = pullRequest.PullRequestId,
        CurrentUser = requestContext.GetUserId(),
        Status = pullRequest.Status
      }), (Func<object>) (() => (object) new
      {
        AutoCompleteAuthority = pullRequest.AutoCompleteAuthority,
        Status = pullRequest.Status
      }), caller: nameof (ReadyToAutoComplete));
    }

    internal void PublishAutoCompleteFailedNotification(
      IVssRequestContext requestContext,
      string failedReason,
      XmlNode jobDataXml)
    {
      if (string.IsNullOrEmpty(failedReason))
        return;
      Guid repositoryId;
      int pullRequestId;
      TeamFoundationGitPullRequestService.ParseAutoCompleteJobDataXml(jobDataXml, out repositoryId, out pullRequestId, out Guid _);
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId))
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new AutoCompleteUpdatedNotification(repositoryById.Key.GetProjectUri(), repositoryById.Key.RepoId, repositoryById.Name, pullRequestId, (TeamFoundationIdentity) null, (TeamFoundationIdentity) null, failedReason));
    }

    public List<PolicyConfigurationRecord> GetBlockingAutoCompletePolicies(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ITfsGitRepository repository,
      ClientTraceData ctData = null,
      IActivePolicyEvaluationCache policyEvaluationCacheService = null)
    {
      return this.CalculateBlockingAutoCompletePolicies(requestContext, pullRequest, repository, ctData, policyEvaluationCacheService);
    }

    private List<PolicyConfigurationRecord> CalculateBlockingAutoCompletePolicies(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ITfsGitRepository repository,
      ClientTraceData ctData = null,
      IActivePolicyEvaluationCache policyEvaluationCacheService = null)
    {
      string projectUri = ProjectInfo.GetProjectUri(repository.Key.ProjectId);
      ArtifactId artifactId1 = pullRequest.BuildLegacyArtifactId(projectUri);
      List<PolicyConfigurationRecord> blockingPolicies = new List<PolicyConfigurationRecord>();
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      HashSet<int> ignoreConfigIds = (HashSet<int>) null;
      if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.SelectiveAutoComplete") && pullRequest.CompletionOptions?.AutoCompleteIgnoreConfigIds != null)
        ignoreConfigIds = new HashSet<int>((IEnumerable<int>) pullRequest.CompletionOptions?.AutoCompleteIgnoreConfigIds);
      IVssRequestContext requestContext1 = requestContext;
      GitPullRequestTarget target = new GitPullRequestTarget(projectUri, pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequest));
      ArtifactId artifactId2 = artifactId1;
      PolicyEvaluationResult evaluationResult;
      ref PolicyEvaluationResult local = ref evaluationResult;
      Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult> action = (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult>) ((policy, existingStatus, existingContext) =>
      {
        HashSet<int> intSet = ignoreConfigIds;
        // ISSUE: explicit non-virtual call
        if ((intSet != null ? (!__nonvirtual (intSet.Contains(policy.Configuration.ConfigurationId)) ? 1 : 0) : 1) != 0 && this.GetAutoCompleteStatus(requestContext, pullRequest, repository, policy, existingStatus, existingContext, ctData) != AutoCompleteStatus.NotBlocking)
          blockingPolicies.Add(policy.Configuration);
        return (PolicyCheckResult) null;
      });
      IActivePolicyEvaluationCache policyEvaluationCacheService1 = policyEvaluationCacheService;
      using (PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy> evaluationTransaction = service.CheckPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext1, (ITeamFoundationPolicyTarget) target, artifactId2, out local, action, policyEvaluationCacheService1))
        evaluationTransaction.Discard();
      return blockingPolicies;
    }

    private AutoCompleteStatus GetAutoCompleteStatus(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ITfsGitRepository repository,
      ITeamFoundationGitPullRequestPolicy policy,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      ClientTraceData ctData)
    {
      int autoCompleteStatus = (int) policy.GetAutoCompleteStatus(requestContext, repository, pullRequest.CompletionOptions, pullRequest, existingStatus, existingContext);
      if (autoCompleteStatus == 0)
        return (AutoCompleteStatus) autoCompleteStatus;
      this.RecordBlockingPolicyInCtData(ctData, policy);
      return (AutoCompleteStatus) autoCompleteStatus;
    }

    private void RecordBlockingPolicyInCtData(
      ClientTraceData ctData,
      ITeamFoundationGitPullRequestPolicy policy)
    {
      if (ctData == null)
        return;
      string key = "AutoCompleteBlockedBy_" + policy.GetType().Name;
      int num;
      ctData.GetData().TryGetValue<int>(key, out num);
      ctData.GetData()[key] = (object) (num + 1);
    }

    internal void QueueAutoCompleteJobIfNeeded(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      if (pullRequest == null || !requestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.AutoComplete") || pullRequest.AutoCompleteAuthority == Guid.Empty || pullRequest.Status != PullRequestStatus.Active || pullRequest.MergeStatus != PullRequestAsyncStatus.Succeeded)
        return;
      this.QueueAutoCompleteJob(requestContext, pullRequest);
    }

    internal virtual void QueueAutoCompleteJob(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      Guid jobId = TeamFoundationGitPullRequestService.JobIdForAutoComplete(pullRequest.RepositoryId, pullRequest.PullRequestId);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobId);
      string name = string.Format("PR AutoComplete repo={0} pr={1}", (object) pullRequest.RepositoryId.ToString("N"), (object) pullRequest.PullRequestId);
      if (foundationJobDefinition == null || foundationJobDefinition.Name != name)
      {
        XmlNode completeJobDataXml = TeamFoundationGitPullRequestService.CreateAutoCompleteJobDataXml(pullRequest.RepositoryId, pullRequest.PullRequestId, pullRequest.AutoCompleteAuthority);
        foundationJobDefinition = new TeamFoundationJobDefinition(jobId, name, "Microsoft.TeamFoundation.Git.Server.Plugins.GitPullRequestAutoCompleteJob", completeJobDataXml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.High);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
      }
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      });
    }

    public static XmlNode CreateAutoCompleteJobDataXml(
      Guid repositoryId,
      int pullRequestId,
      Guid autoCompleteAuthority)
    {
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckBoundsInclusive(pullRequestId, 1, int.MaxValue, nameof (pullRequestId));
      return TeamFoundationSerializationUtility.SerializeToXml((object) new AutoCompleteJobData()
      {
        RepositoryId = repositoryId,
        PullRequestId = pullRequestId,
        AutoCompleteAuthority = autoCompleteAuthority
      });
    }

    public static void ParseAutoCompleteJobDataXml(
      XmlNode xmlData,
      out Guid repositoryId,
      out int pullRequestId,
      out Guid autoCompleteAuthority)
    {
      ArgumentUtility.CheckForNull<XmlNode>(xmlData, nameof (xmlData));
      AutoCompleteJobData autoCompleteJobData = TeamFoundationSerializationUtility.Deserialize<AutoCompleteJobData>(xmlData);
      repositoryId = autoCompleteJobData.RepositoryId;
      pullRequestId = autoCompleteJobData.PullRequestId;
      autoCompleteAuthority = autoCompleteJobData.AutoCompleteAuthority;
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckBoundsInclusive(pullRequestId, 1, int.MaxValue, "PullRequestId");
    }

    internal static Guid JobIdForAutoComplete(Guid repositoryId, int pullRequestId) => DeterministicGuid.ComputeFromDeprecatedSha1(repositoryId.ToString("N") + ":" + pullRequestId.ToString());

    internal static string GetAutoCompleteExpectedFailureMessage(
      IVssRequestContext requestContext,
      Exception exception,
      Guid identity)
    {
      string resourceName = (string) null;
      switch (exception)
      {
        case GitPullRequestNotEditableException _:
          resourceName = "PullRequestAutoCompleteFailedReason_NotActive";
          break;
        case GitPullRequestCannotBeActivated _:
          resourceName = "PullRequestAutoCompleteFailedReason_SourceOrTargetDeleted";
          break;
        case GitPullRequestUpdateRejectedByPolicyException _:
          if (!string.IsNullOrWhiteSpace(exception.Message))
            return exception.Message.EndsWith(".") ? exception.Message : exception.Message + ".";
          resourceName = "PullRequestAutoCompleteFailedReason_PoliciesBlocking";
          break;
        case GitRepositoryNotFoundException _:
          resourceName = "PullRequestAutoCompleteFailedReason_RepoNotFound";
          break;
        case GitNeedsPermissionException _:
          resourceName = identity != Guid.Empty ? "PullRequestAutoCompleteFailedReason_NoPermissionToComplete" : "";
          break;
        case UnauthorizedAccessException _:
          resourceName = identity != Guid.Empty ? "PullRequestAutoCompleteFailedReason_NoPermissionToComplete" : "";
          break;
        case GitPullRequestAutoCompleteFailedException _:
          resourceName = "";
          break;
      }
      if (string.IsNullOrEmpty(resourceName))
        return resourceName;
      return Resources.Format(resourceName, (object) IdentityHelper.Instance.GetDisplayName(requestContext, identity));
    }

    internal TfsGitPullRequest MergePullRequestAndUpdateConflicts(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      Sha1Id mergeSourceCommitId,
      Sha1Id mergeTargetCommitId,
      bool forCompletion,
      ClientTraceData ctData,
      out Sha1Id mergeCommitId,
      out Sha1Id conflictResolutionHash,
      out IdentityDescriptor lastConflictResolver)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (MergePullRequestAndUpdateConflicts)))
      {
        bool conflictAuthorshipCommits;
        bool disableRenames;
        bool detectRenameFalsePositives;
        int renameThreshold;
        int targetLimit;
        this.CoalesceMergeOptions(requestContext, repository, pullRequest, out conflictAuthorshipCommits, out disableRenames, out detectRenameFalsePositives, out renameThreshold, out targetLimit);
        conflictResolutionHash = Sha1Id.Empty;
        lastConflictResolver = (IdentityDescriptor) null;
        Sha1Id[] sha1IdArray = new Sha1Id[2]
        {
          Sha1Id.Empty,
          Sha1Id.Empty
        };
        string commitMessage = (string) null;
        if (forCompletion)
          commitMessage = pullRequest.CompletionOptions?.MergeCommitMessage;
        CommitDetails commitDetails = ITfsGitRepositoryExtensions.CreateCommitDetails(requestContext, pullRequest, commitMessage);
        int num;
        if (forCompletion)
        {
          GitPullRequestCompletionOptions completionOptions = pullRequest.CompletionOptions;
          num = completionOptions != null ? (completionOptions.SquashMerge ? 1 : 0) : 0;
        }
        else
          num = 0;
        bool flag = num != 0;
        IEnumerable<Sha1Id> parentIdsForMergeCommit;
        if (flag)
          parentIdsForMergeCommit = (IEnumerable<Sha1Id>) new Sha1Id[1]
          {
            mergeTargetCommitId
          };
        else
          parentIdsForMergeCommit = (IEnumerable<Sha1Id>) new Sha1Id[2]
          {
            mergeTargetCommitId,
            mergeSourceCommitId
          };
        MergeWithConflictsOptions mergeOptions = new MergeWithConflictsOptions()
        {
          CommitDetails = commitDetails,
          DisableRenames = disableRenames,
          RenameThreshold = renameThreshold,
          TargetLimit = targetLimit
        };
        PullRequestMergerBase.Result result = (((conflictAuthorshipCommits ? 1 : 0) & (flag ? 0 : (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.ConflictResolutionAuthorCommits") ? 1 : 0))) != 0 ? (PullRequestMergerBase) new ConflictAuthorshipPullRequestMerger(requestContext, repository, pullRequest, detectRenameFalsePositives, mergeOptions, mergeSourceCommitId, mergeTargetCommitId, parentIdsForMergeCommit, ctData) : (PullRequestMergerBase) new GitIndexPullRequestMerger(requestContext, repository, pullRequest, detectRenameFalsePositives, mergeOptions, mergeSourceCommitId, mergeTargetCommitId, parentIdsForMergeCommit, ctData)).MergeAndResolveConflicts();
        mergeCommitId = result.MergeCommitId;
        conflictResolutionHash = result.ConflictResolutionHash;
        lastConflictResolver = result.ConflictResolver;
        using (GitConflictComponent component = requestContext.CreateComponent<GitConflictComponent>())
        {
          using (requestContext.TimeRegion("GitConflictComponent", "UpdateGitPullRequestMergeStatusAndConflicts"))
          {
            GitConflictComponent conflictComponent = component;
            RepoKey key = repository.Key;
            int pullRequestId = pullRequest.PullRequestId;
            int mergeStatus = (int) result.MergeStatus;
            Sha1Id mergeSourceCommitId1 = mergeSourceCommitId;
            Sha1Id mergeTargetCommitId1 = mergeTargetCommitId;
            Sha1Id mergeCommitId1 = result.MergeCommitId;
            string failureMessage = result.FailureMessage;
            int failureType = (int) result.FailureType;
            string mergeFailureMessage = failureMessage;
            IEnumerable<GitConflict> conflicts = result.Conflicts;
            conflictComponent.UpdateGitPullRequestMergeStatusAndConflicts(key, pullRequestId, (PullRequestAsyncStatus) mergeStatus, mergeSourceCommitId1, mergeTargetCommitId1, mergeCommitId1, (PullRequestMergeFailureType) failureType, mergeFailureMessage, conflicts);
          }
        }
        TeamFoundationGitPullRequestService.SetInternalPullRequestRefs(requestContext, pullRequest, new Sha1Id?(result.MergeCommitId), new Sha1Id?(result.SourceFixupCommitId), new Sha1Id?(result.TargetFixupCommitId));
        return this.GetPullRequestDetails(requestContext, repository, pullRequest.PullRequestId);
      }
    }

    private void CoalesceMergeOptions(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      out bool conflictAuthorshipCommits,
      out bool disableRenames,
      out bool detectRenameFalsePositives,
      out int renameThreshold,
      out int targetLimit)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      conflictAuthorshipCommits = ((bool?) pullRequest.MergeOptions?.ConflictAuthorshipCommits).GetValueOrDefault();
      ref bool local1 = ref disableRenames;
      bool? nullable = (bool?) pullRequest.MergeOptions?.DisableRenames;
      int num1 = (int) nullable ?? (!service.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Git/Settings/MergeConflictFindRenames", true, true) ? 1 : 0);
      local1 = num1 != 0;
      ref bool local2 = ref detectRenameFalsePositives;
      nullable = (bool?) pullRequest.MergeOptions?.DetectRenameFalsePositives;
      int num2 = (int) nullable ?? (repository.Settings.DetectRenameFalsePositivesByDefault ? 1 : 0);
      local2 = num2 != 0;
      renameThreshold = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MergeConflictRenameThreshold", true, 100);
      targetLimit = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MergeConflictTargetLimit", true, 500);
    }

    internal static void SetInternalPullRequestRefs(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id? newMergeCommitId,
      Sha1Id? newSourceFixupCommitId,
      Sha1Id? newTargetFixupCommitId)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (SetInternalPullRequestRefs)))
      {
        string mergeRefName = TfsGitPullRequest.GetMergeRefName(pullRequest.PullRequestId);
        string sourceFixupRefName = TfsGitPullRequest.GetSourceFixupRefName(pullRequest.PullRequestId);
        string targetFixupRefName = TfsGitPullRequest.GetTargetFixupRefName(pullRequest.PullRequestId);
        string[] refNames = new string[3]
        {
          mergeRefName,
          sourceFixupRefName,
          targetFixupRefName
        };
        List<TfsGitRef> source;
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
          source = repositoryById.Refs.MatchingNames((IEnumerable<string>) refNames);
        TfsGitRef tfsGitRef1 = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == mergeRefName));
        Sha1Id oldObjectId1 = tfsGitRef1 != null ? tfsGitRef1.ObjectId : Sha1Id.Empty;
        TfsGitRef tfsGitRef2 = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == sourceFixupRefName));
        Sha1Id oldObjectId2 = tfsGitRef2 != null ? tfsGitRef2.ObjectId : Sha1Id.Empty;
        TfsGitRef tfsGitRef3 = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == targetFixupRefName));
        Sha1Id oldObjectId3 = tfsGitRef3 != null ? tfsGitRef3.ObjectId : Sha1Id.Empty;
        List<TfsGitRefUpdateRequest> refUpdates = new List<TfsGitRefUpdateRequest>(3);
        Sha1Id? nullable;
        if (newMergeCommitId.HasValue)
        {
          nullable = newMergeCommitId;
          Sha1Id sha1Id = oldObjectId1;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != sha1Id ? 1 : 0) : 0) : 1) != 0)
            refUpdates.Add(new TfsGitRefUpdateRequest(mergeRefName, oldObjectId1, newMergeCommitId.Value));
        }
        if (newSourceFixupCommitId.HasValue)
        {
          nullable = newSourceFixupCommitId;
          Sha1Id sha1Id = oldObjectId2;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != sha1Id ? 1 : 0) : 0) : 1) != 0)
            refUpdates.Add(new TfsGitRefUpdateRequest(sourceFixupRefName, oldObjectId2, newSourceFixupCommitId.Value));
        }
        if (newTargetFixupCommitId.HasValue)
        {
          nullable = newTargetFixupCommitId;
          Sha1Id sha1Id = oldObjectId3;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != sha1Id ? 1 : 0) : 0) : 1) != 0)
            refUpdates.Add(new TfsGitRefUpdateRequest(targetFixupRefName, oldObjectId3, newTargetFixupCommitId.Value));
        }
        if (refUpdates.Count <= 0)
          return;
        TfsGitRefUpdateResultSet refUpdateResultSet = requestContext.GetService<ITeamFoundationGitRefService>().UpdateRefs(requestContext.Elevate(), pullRequest.RepositoryId, refUpdates);
        if (refUpdateResultSet.CountFailed <= 0)
          return;
        GitPullRequestConflictResolutionRefUpdateFailure refUpdateFailure = new GitPullRequestConflictResolutionRefUpdateFailure();
        TracepointUtils.TraceException(requestContext, 1013026, GitServerUtils.TraceArea, "TfsGitPullRequest", (Exception) refUpdateFailure, (object) new
        {
          FailedRefs = refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (u => !u.Succeeded)).Select(u => new
          {
            Name = u.Name,
            RejectedBy = u.RejectedBy
          }).ToArray()
        }, caller: nameof (SetInternalPullRequestRefs));
      }
    }

    internal static void DeleteAllInternalPullRequestRefs(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (DeleteAllInternalPullRequestRefs)))
      {
        string[] refNames = new string[1]
        {
          string.Format("{0}{1}/", (object) "refs/pull/", (object) pullRequest.PullRequestId)
        };
        List<TfsGitRef> source;
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
          source = repositoryById.Refs.MatchingNames((IEnumerable<string>) refNames, GitRefSearchType.StartsWith);
        if (source.Count <= 0)
          return;
        List<TfsGitRefUpdateRequest> list = source.Select<TfsGitRef, TfsGitRefUpdateRequest>((Func<TfsGitRef, TfsGitRefUpdateRequest>) (r => new TfsGitRefUpdateRequest(r.Name, r.ObjectId, Sha1Id.Empty))).ToList<TfsGitRefUpdateRequest>();
        TfsGitRefUpdateResultSet refUpdateResultSet = requestContext.GetService<ITeamFoundationGitRefService>().UpdateRefs(requestContext.Elevate(), pullRequest.RepositoryId, list);
        if (refUpdateResultSet.CountFailed <= 0)
          return;
        GitPullRequestConflictResolutionRefUpdateFailure refUpdateFailure = new GitPullRequestConflictResolutionRefUpdateFailure();
        TracepointUtils.TraceException(requestContext, 1013047, GitServerUtils.TraceArea, "TfsGitPullRequest", (Exception) refUpdateFailure, (object) new
        {
          FailedRefs = refUpdateResultSet.Results.Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (u => !u.Succeeded)).Select(u => new
          {
            Name = u.Name,
            RejectedBy = u.RejectedBy
          }).ToArray()
        }, caller: nameof (DeleteAllInternalPullRequestRefs));
      }
    }

    public Sha1Id StoreMergedBlobForConflict(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      int conflictId,
      byte[] contents)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (StoreMergedBlobForConflict)))
      {
        TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        string sourceBranchName = pullRequestDetails.SourceBranchName;
        string mergedBlobBranchName = TfsGitPullRequest.GetMergedBlobRefName(pullRequestId, conflictId);
        List<TfsGitRef> source = repository.Refs.MatchingNames((IEnumerable<string>) new string[2]
        {
          sourceBranchName,
          mergedBlobBranchName
        });
        TfsGitRef tfsGitRef1 = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == sourceBranchName));
        Sha1Id sha1Id = tfsGitRef1 != null ? tfsGitRef1.ObjectId : Sha1Id.Empty;
        TfsGitRef tfsGitRef2 = source.FirstOrDefault<TfsGitRef>((Func<TfsGitRef, bool>) (r => r.Name == mergedBlobBranchName));
        Sha1Id baseCommitId = tfsGitRef2 != null ? tfsGitRef2.ObjectId : Sha1Id.Empty;
        if (sha1Id == Sha1Id.Empty)
          throw new GitRefNotFoundException(pullRequestDetails.SourceBranchName, repository.Key.RepoId.ToString());
        ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
        string displayName = requestContext.GetUserIdentity().DisplayName;
        IVssRequestContext requestContext1 = requestContext;
        Guid userId = requestContext.GetUserId();
        string str = service.GetPreferredEmailAddress(requestContext1, userId);
        if (string.IsNullOrEmpty(str))
          str = displayName;
        GitUserDate gitUserDate = new GitUserDate()
        {
          Name = displayName,
          Email = str,
          Date = DateTime.UtcNow
        };
        string base64String = Convert.ToBase64String(contents);
        GitChange[] gitChangeArray = new GitChange[1];
        GitChange gitChange = new GitChange();
        gitChange.ChangeType = VersionControlChangeType.Add;
        GitItem gitItem = new GitItem();
        gitItem.Path = "/" + Guid.NewGuid().ToString("N");
        gitChange.Item = gitItem;
        gitChange.NewContent = new ItemContent()
        {
          Content = base64String,
          ContentType = ItemContentType.Base64Encoded
        };
        gitChangeArray[0] = gitChange;
        GitChange[] changes = gitChangeArray;
        repository.ModifyPaths(mergedBlobBranchName, baseCommitId, "Merged blob commit", (IEnumerable<GitChange>) changes, gitUserDate, gitUserDate);
        byte[] bytes = Encoding.UTF8.GetBytes("blob " + contents.Length.ToString("d") + "\0");
        MemoryStream memoryStream = new MemoryStream(bytes.Length + contents.Length);
        memoryStream.Write(bytes, 0, bytes.Length);
        memoryStream.Write(contents, 0, contents.Length);
        memoryStream.Flush();
        return new Sha1Id(SHA1.Create().ComputeHash(memoryStream.GetBuffer()));
      }
    }

    public TfsGitPullRequest TryCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      Sha1Id lastSourceCommitSeen,
      GitPullRequestCompletionOptions completionOptions,
      bool useStrictBypass = false,
      bool triggeredByAutoComplete = false)
    {
      requestContext.Trace(1013218, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to commit pull request {0}", (object) pullRequestId);
      Stopwatch stopwatch = Stopwatch.StartNew();
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add("Action", (object) nameof (TryCommit));
      clientTraceData.Add("PullRequestId", (object) pullRequestId);
      try
      {
        TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        requestContext.TraceAlways(1013834, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", string.Format("PullRequest: {0}/{1}/{2}", (object) pullRequestDetails.RepositoryId, (object) pullRequestDetails.TargetBranchName, (object) pullRequestDetails.PullRequestId));
        if (completionOptions != null)
        {
          clientTraceData.Add("PullRequestCompletionBypassPolicy", (object) completionOptions?.BypassPolicy);
          clientTraceData.Add("PullRequestCompletionBypassPolicyReason", (object) completionOptions?.BypassReason);
          clientTraceData.Add("PullRequestCompletionDeleteSource", (object) completionOptions?.DeleteSourceBranch);
          clientTraceData.Add("PullRequestCompletionMergeStrategy", (object) (GitPullRequestMergeStrategy?) completionOptions?.MergeStrategy);
          clientTraceData.Add("PullRequestCompletionSquash", (object) completionOptions?.SquashMerge);
          clientTraceData.Add("PullRequestCompletionTransitionWorkItems", (object) completionOptions?.TransitionWorkItems);
          clientTraceData.Add("PullRequesCompletionTriggeredByAutoComplete", (object) completionOptions?.TriggeredByAutoComplete);
          clientTraceData.Add("PullRequestCompletionKey", (object) string.Format("PullRequest: {0}/{1}/{2}", (object) pullRequestDetails.RepositoryId, (object) pullRequestDetails.TargetBranchName, (object) pullRequestDetails.PullRequestId));
        }
        repository.Permissions.CheckPullRequestContribute();
        repository.Permissions.CheckWrite(pullRequestDetails.TargetBranchName);
        if (pullRequestDetails.Status == PullRequestStatus.Abandoned)
          throw new GitPullRequestNotEditableException();
        if (pullRequestDetails.IsDraft)
          throw new GitPullRequestDraftCannotCompleteException();
        if (!pullRequestDetails.BelongsToNoJob)
          return pullRequestDetails;
        TfsGitRef targetRef = ITfsGitRepositoryExtensions.VerifyLocalRef(repository, pullRequestDetails.TargetBranchName);
        TfsGitRef sourceRef = TeamFoundationGitPullRequestService.VerifySourceRef(repository, pullRequestDetails, true);
        TfsGitPullRequest pullRequest = this.CloseIfMergedExternally(requestContext, repository, pullRequestDetails, sourceRef, targetRef, clientTraceData);
        if (pullRequest.Status == PullRequestStatus.Completed)
        {
          requestContext.Trace(1013740, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", "PR {0} was already merged externally so it was closed.", (object) pullRequestId);
          return pullRequest;
        }
        if (sourceRef == null)
          throw new GitPullRequestCannotBeActivated();
        if (!lastSourceCommitSeen.IsEmpty && sourceRef.ObjectId != lastSourceCommitSeen)
          throw new GitPullRequestStaleException();
        return this.TryEnterCompletionQueue(requestContext, repository, pullRequest, completionOptions, useStrictBypass, sourceRef, targetRef, clientTraceData, triggeredByAutoComplete);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case GitPullRequestUpdateRejectedByPolicyException _:
          case GitPullRequestCannotBeActivated _:
          case GitPullRequestNotEditableException _:
          case GitNeedsPermissionException _:
          case RequestCanceledException _:
          case GitPullRequestStaleException _:
            requestContext.TraceCatch(1013392, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
            break;
          default:
            requestContext.TraceException(1013392, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
            break;
        }
        throw;
      }
      finally
      {
        try
        {
          clientTraceData.Add("PullRequestTryCommitForegroundElapsedMs", (object) stopwatch.ElapsedMilliseconds);
          requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", clientTraceData);
        }
        catch
        {
        }
      }
    }

    private TfsGitPullRequest CloseIfMergedExternally(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      TfsGitRef sourceRef,
      TfsGitRef targetRef,
      ClientTraceData ctData)
    {
      Sha1Id sha1Id = sourceRef == null ? pullRequest.LastMergeSourceCommit ?? Sha1Id.Empty : sourceRef.ObjectId;
      Sha1Id objectId = targetRef.ObjectId;
      bool flag = GitServerUtils.IsConnected(requestContext, repository, (IEnumerable<Sha1Id>) new Sha1Id[1]
      {
        sha1Id
      }, targetRef.ObjectId);
      ctData.Add("PullRequestMergedExternally", (object) flag);
      if (flag)
      {
        int num = (int) this.TryCompletePullRequest(requestContext, repository, ref pullRequest, sourceRef, targetRef, Sha1Id.Empty, true, ctData);
      }
      return pullRequest;
    }

    private TfsGitPullRequest TryEnterCompletionQueue(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestCompletionOptions completionOptions,
      bool useStrictBypass,
      TfsGitRef sourceRef,
      TfsGitRef targetRef,
      ClientTraceData ctData,
      bool triggeredByAutoComplete = false)
    {
      string localizedErrorMessage = (string) null;
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      if (completionOptions == null)
      {
        completionOptions = pullRequest.CompletionOptions == null ? new GitPullRequestCompletionOptions() : pullRequest.CompletionOptions;
        completionOptions.BypassPolicy = false;
      }
      completionOptions.TriggeredByAutoComplete = triggeredByAutoComplete;
      if (triggeredByAutoComplete)
      {
        useStrictBypass = true;
        completionOptions.BypassPolicy = false;
      }
      bool flag1;
      bool flag2;
      try
      {
        requestContext.Trace(1013395, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", "Checking policies before queueing background completion.");
        string projectUri = repository.Key.GetProjectUri();
        ArtifactId artifactId1 = pullRequest.BuildLegacyArtifactId(projectUri);
        ITeamFoundationPolicyTarget target = (ITeamFoundationPolicyTarget) new GitPullRequestTarget(projectUri, pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequest));
        if (!requestContext.IsFeatureEnabled("Git.PullRequests.UsePolicyPRTarget"))
          target = (ITeamFoundationPolicyTarget) new GitBranchNameTarget(projectUri, repository.Key.RepoId, pullRequest.TargetBranchName, DefaultBranchUtils.IsDefaultBranch(requestContext, repository.Key.RepoId, pullRequest.TargetBranchName));
        TfsGitPullRequest requestCopy = pullRequest;
        PolicyEvaluationResult result;
        using (PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy> evaluationTransaction = service.CheckPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, target, artifactId1, out result, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult>) ((policy, existingStatus, existingContext) => policy.CheckEnterCompletionQueue(requestContext, repository, completionOptions, requestCopy, existingStatus, existingContext))))
        {
          flag1 = result.IsPassed;
          flag2 = result.RequiresBypass;
          localizedErrorMessage = result.RejectionReason;
          if (requestContext.IsFeatureEnabled("Policy.EventBasedCacheEnabled.OnTargetUpdate") && !result.IsPassed)
          {
            ArtifactId artifactId2 = pullRequest.BuildLegacyArtifactId(projectUri);
            new ActivePolicyEvaluationCache().Invalidate(requestContext, LinkingUtilities.EncodeUri(artifactId2), nameof (TryEnterCompletionQueue));
          }
          evaluationTransaction.Save(requestContext, ctData);
        }
      }
      catch (PolicyImplementationException ex)
      {
        requestContext.TraceException(1013396, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Exception) ex);
        flag1 = false;
        flag2 = false;
      }
      if (flag1 && requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.EnforceAdHocRequiredReviewers") && pullRequest.Reviewers.Any<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => r.IsRequired && !r.Approves)))
        flag2 = true;
      if (!flag2)
        completionOptions.BypassPolicy = false;
      if (!flag1)
      {
        if (triggeredByAutoComplete)
        {
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId = repository.Key.ProjectId;
          TfsGitPullRequest pullRequest1 = pullRequest;
          Guid? nullable = new Guid?(Guid.Empty);
          PullRequestStatus? status = new PullRequestStatus?();
          PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
          Sha1Id? lastMergeSourceCommit = new Sha1Id?();
          Sha1Id? lastMergeTargetCommit = new Sha1Id?();
          Sha1Id? lastMergeCommit = new Sha1Id?();
          Guid? completeWhenMergedAuthority = new Guid?();
          PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime = new DateTime?();
          Guid? autoCompleteAuthority = nullable;
          bool? isDraft = new bool?();
          this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
        }
        pullRequest = this.GetPullRequestDetails(requestContext, repository.Key, pullRequest.PullRequestId);
        return pullRequest.BelongsToCompletionJob ? pullRequest : throw new GitPullRequestUpdateRejectedByPolicyException(localizedErrorMessage);
      }
      if (flag2 && !completionOptions.BypassPolicy)
      {
        if (useStrictBypass)
          throw new GitPullRequestUpdateRejectedByPolicyException(Resources.Get("ExplictBypassOptionRequired"));
        completionOptions.BypassPolicy = true;
      }
      pullRequest = this.EnterCompletionQueue(requestContext, repository, pullRequest.PullRequestId, completionOptions);
      if (pullRequest != null)
      {
        ctData.Add("PullRequestBelongsToCompletionJob", (object) pullRequest.BelongsToCompletionJob);
        ctData.Add("PullRequestBelongsToMergeJob", (object) pullRequest.BelongsToMergeJob);
      }
      return pullRequest;
    }

    internal TfsGitPullRequest EnterCompletionQueue(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      GitPullRequestCompletionOptions completionOptions)
    {
      TfsGitPullRequest pullRequest1 = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      if (pullRequest1.BelongsToCompletionJob)
        this.QueueCompletionJobToRun(requestContext, pullRequest1.RepositoryId, pullRequest1.TargetBranchName);
      else if (!pullRequest1.BelongsToMergeJob)
      {
        if (!pullRequest1.BelongsToNoJob)
          throw new GitPullRequestNotEditableException();
        DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        TfsGitPullRequest pullRequest2 = pullRequest1;
        PullRequestAsyncStatus? nullable1 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Queued);
        Sha1Id? nullable2 = new Sha1Id?(Sha1Id.Empty);
        GitPullRequestCompletionOptions completionOptions1 = completionOptions;
        Guid? nullable3 = new Guid?(requestContext.GetUserId());
        DateTime? nullable4 = new DateTime?(resolutionUtcNow);
        Guid? nullable5 = new Guid?(Guid.Empty);
        PullRequestStatus? status = new PullRequestStatus?();
        PullRequestAsyncStatus? mergeStatus = nullable1;
        Sha1Id? lastMergeSourceCommit = new Sha1Id?();
        Sha1Id? lastMergeTargetCommit = new Sha1Id?();
        Sha1Id? lastMergeCommit = nullable2;
        Guid? completeWhenMergedAuthority = nullable3;
        GitPullRequestCompletionOptions completionOptions2 = completionOptions1;
        PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
        DateTime? completionQueueTime = nullable4;
        Guid? autoCompleteAuthority = nullable5;
        bool? isDraft = new bool?();
        pullRequest1 = this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, completionOptions2, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
        Guid orchestrationId = TeamFoundationGitPullRequestService.OrchestrationIdForCompletionQueueJob(pullRequest1);
        requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogNewOrchestration(requestContext, orchestrationId, -1L, GitServerUtils.TraceArea, "PullRequestComplete");
        requestContext.TraceAlways(1013261, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", string.Format("PullRequest: {0}/{1}/{2}, ", (object) pullRequest1.RepositoryId, (object) pullRequest1.TargetBranchName, (object) pullRequest1.PullRequestId) + string.Format("CompleteWhenMergedAuthority: {0}", (object) pullRequest1.CompleteWhenMergedAuthority));
        this.QueueCompletionJobToRun(requestContext, pullRequest1.RepositoryId, pullRequest1.TargetBranchName);
      }
      return pullRequest1;
    }

    internal string PerformCompletionJob(
      IVssRequestContext requestContext,
      Guid jobId,
      XmlNode jobDataXml,
      ClientTraceData ctData)
    {
      Guid repositoryId = Guid.Empty;
      string targetBranchName = (string) null;
      int queueLength = -1;
      return TracepointUtils.TraceBlock<string>(requestContext, 1013480, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<string>) (() =>
      {
        TeamFoundationGitPullRequestService.ParseCompletionJobDataXml(jobDataXml, out repositoryId, out targetBranchName);
        ctData.Add("RepositoryId", (object) repositoryId);
        ctData.Add("PullRequestTargetBranch", (object) targetBranchName);
        List<TfsGitPullRequest> inCompletionQueue = this.GetPullRequestsInCompletionQueue(requestContext, repositoryId, targetBranchName);
        TfsGitPullRequest pullRequest1 = inCompletionQueue.OrderBy<TfsGitPullRequest, DateTime>((Func<TfsGitPullRequest, DateTime>) (pr => pr.CompletionQueueTime)).FirstOrDefault<TfsGitPullRequest>();
        if (pullRequest1 == null)
          return string.Format("Nothing in completion queue for repository {0} and branch {1}", (object) repositoryId, (object) targetBranchName);
        ex = (Exception) null;
        string str = (string) null;
        try
        {
          queueLength = inCompletionQueue.Count;
          ctData.Add("PullRequestCompletionQueueLength", (object) queueLength);
          if (queueLength > 1)
            this.QueueCompletionJobToRun(requestContext, repositoryId, targetBranchName);
          pullRequest1 = this.MergeAndCompletePullRequest(requestContext, pullRequest1, ctData);
          TfsGitPullRequest.CheckMergeStatus(pullRequest1);
          if (pullRequest1 != null)
            str = string.Format("Completion for PR: {0} on repository: {1} with PR status result: {2}", (object) pullRequest1.PullRequestId, (object) pullRequest1.RepositoryId, (object) pullRequest1.Status);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          ex = ex;
          if (ex == null)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
        }
        finally
        {
          try
          {
            ctData.Add("PullRequestStateTransition", (object) pullRequest1.Status);
            if (ex != null)
              ctData.Add("ExceptionType", (object) ex.GetType().FullName);
          }
          catch
          {
          }
          try
          {
            if (pullRequest1.Status == PullRequestStatus.Active)
            {
              using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, repositoryId))
              {
                IVssRequestContext requestContext1 = requestContext;
                Guid projectId = repositoryById.Key.ProjectId;
                TfsGitPullRequest pullRequest2 = pullRequest1;
                Guid? nullable1 = new Guid?(Guid.Empty);
                DateTime? nullable2 = new DateTime?(new DateTime());
                PullRequestStatus? status = new PullRequestStatus?();
                PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
                Sha1Id? lastMergeSourceCommit = new Sha1Id?();
                Sha1Id? lastMergeTargetCommit = new Sha1Id?();
                Sha1Id? lastMergeCommit = new Sha1Id?();
                Guid? completeWhenMergedAuthority = nullable1;
                PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
                DateTime? completionQueueTime = nullable2;
                Guid? autoCompleteAuthority = new Guid?();
                bool? isDraft = new bool?();
                this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
              }
            }
          }
          catch (Exception ex) when (TracepointUtils.TraceException(requestContext, 1013079, GitServerUtils.TraceArea, "TfsGitPullRequest", ex, caller: nameof (PerformCompletionJob)))
          {
          }
        }
        return str;
      }), (Func<object>) (() => (object) new
      {
        jobId = jobId,
        jobData = jobDataXml.OuterXml,
        repositoryId = repositoryId,
        targetBranchName = targetBranchName,
        queueLength = queueLength
      }), caller: nameof (PerformCompletionJob));
    }

    internal TfsGitPullRequest MergeAndCompletePullRequest(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData)
    {
      Guid orchestrationId = TeamFoundationGitPullRequestService.OrchestrationIdForCompletionQueueJob(pullRequest);
      using (ITimedOrchestrationRegion orchestration = requestContext.TimeOrchestration("TfsGitPullRequest", orchestrationId, -1L, "PullRequestComplete", true, nameof (MergeAndCompletePullRequest)))
      {
        PullRequestAsyncStatus mergeStatus = PullRequestAsyncStatus.NotSet;
        TeamFoundationGitPullRequestService.CompletePullRequestResult completionResult = TeamFoundationGitPullRequestService.CompletePullRequestResult.Success;
        int mergesPerformed = 0;
        Exception caughtException = (Exception) null;
        return TracepointUtils.TraceBlock<TfsGitPullRequest>(requestContext, 1013481, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<TfsGitPullRequest>) (() =>
        {
          ctData.Add("PullRequestId", (object) pullRequest.PullRequestId);
          ClientTraceData clientTraceData1 = ctData;
          GitPullRequestCompletionOptions completionOptions1 = pullRequest.CompletionOptions;
          // ISSUE: variable of a boxed type
          __Boxed<bool> local1 = (ValueType) (bool) (completionOptions1 != null ? (completionOptions1.SquashMerge ? 1 : 0) : 0);
          clientTraceData1.Add("PullRequestCompletionSquash", (object) local1);
          ClientTraceData clientTraceData2 = ctData;
          GitPullRequestMergeStrategy? mergeStrategy = (GitPullRequestMergeStrategy?) pullRequest.CompletionOptions?.MergeStrategy;
          // ISSUE: variable of a boxed type
          __Boxed<int> valueOrDefault1 = (ValueType) (mergeStrategy.HasValue ? new int?((int) mergeStrategy.GetValueOrDefault()) : new int?()).GetValueOrDefault();
          clientTraceData2.Add("PullRequestCompletionMergeStrategy", (object) valueOrDefault1);
          ClientTraceData clientTraceData3 = ctData;
          GitPullRequestCompletionOptions completionOptions2 = pullRequest.CompletionOptions;
          // ISSUE: variable of a boxed type
          __Boxed<bool> local2 = (ValueType) (bool) (completionOptions2 != null ? (completionOptions2.DeleteSourceBranch ? 1 : 0) : 0);
          clientTraceData3.Add("PullRequestCompletionDeleteSource", (object) local2);
          ClientTraceData clientTraceData4 = ctData;
          GitPullRequestCompletionOptions completionOptions3 = pullRequest.CompletionOptions;
          // ISSUE: variable of a boxed type
          __Boxed<bool> local3 = (ValueType) (bool) (completionOptions3 != null ? (completionOptions3.BypassPolicy ? 1 : 0) : 0);
          clientTraceData4.Add("PullRequestCompletionBypassPolicy", (object) local3);
          if (!pullRequest.BelongsToCompletionJob)
          {
            TracepointUtils.Tracepoint(requestContext, 1013482, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", closure_0 ?? (closure_0 = (Func<object>) (() => (object) pullRequest)), TraceLevel.Error, caller: nameof (MergeAndCompletePullRequest));
            return pullRequest;
          }
          ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
          TeamFoundationIdentity userIdentity = IdentityHelper.Instance.GetUserIdentity(requestContext, service1, pullRequest.CompleteWhenMergedAuthority);
          requestContext.TraceAlways(1013262, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", string.Format("PullRequest: {0}/{1}/{2}, ", (object) pullRequest.RepositoryId, (object) pullRequest.TargetBranchName, (object) pullRequest.PullRequestId) + string.Format("CompleteWhenMergedAuthority: {0}, ", (object) pullRequest.CompleteWhenMergedAuthority) + string.Format("IdentityTeamFoundationId: {0}, ", (object) userIdentity?.TeamFoundationId) + string.Format("IsActive: {0}", (object) userIdentity?.IsActive));
          if (userIdentity == null)
          {
            requestContext.Trace(1013260, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", "Can't find the user who authorized pull request completion");
            return pullRequest;
          }
          if (!userIdentity.IsActive)
          {
            requestContext.RootContext.Items[RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid] = (object) true;
            userIdentity = IdentityHelper.Instance.GetUserIdentity(requestContext, service1, pullRequest.CompleteWhenMergedAuthority);
            requestContext.TraceAlways(1013854, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", string.Format("PullRequest: {0}/{1}/{2}, ", (object) pullRequest.RepositoryId, (object) pullRequest.TargetBranchName, (object) pullRequest.PullRequestId) + string.Format("CompleteWhenMergedAuthority: {0}, ", (object) pullRequest.CompleteWhenMergedAuthority) + string.Format("IdentityTeamFoundationId: {0}, ", (object) userIdentity?.TeamFoundationId) + string.Format("IsActive: {0}", (object) userIdentity?.IsActive));
          }
          using (IVssRequestContext userContext = requestContext.CreateUserContext(userIdentity.Descriptor))
          {
            userContext.ProgressTimerJoin(requestContext);
            ITeamFoundationGitRepositoryService service2 = userContext.GetService<ITeamFoundationGitRepositoryService>();
            string teamProjectUri = (string) null;
            string repositoryName = (string) null;
            bool flag = false;
            IVssRegistryService registryService = (IVssRegistryService) null;
            try
            {
              do
              {
                ++mergesPerformed;
                using (ITfsGitRepository repositoryById = service2.FindRepositoryById(userContext, pullRequest.RepositoryId))
                {
                  teamProjectUri = repositoryById.Key.GetProjectUri();
                  repositoryName = repositoryById.Name;
                  TfsGitRef sourceRef;
                  TfsGitRef targetRef;
                  Sha1Id rebasedSourceCommitId;
                  pullRequest = this.CreateUpToDateMerge(userContext, repositoryById.Key.ProjectId, repositoryById, pullRequest, out sourceRef, out targetRef, out rebasedSourceCommitId, true, ctData);
                  mergeStatus = pullRequest.MergeStatus;
                  if (mergeStatus == PullRequestAsyncStatus.Succeeded)
                  {
                    Sha1Id? mergeTargetCommit = pullRequest.LastMergeTargetCommit;
                    Sha1Id? lastMergeCommit = pullRequest.LastMergeCommit;
                    bool mergedExternally = mergeTargetCommit.HasValue == lastMergeCommit.HasValue && (!mergeTargetCommit.HasValue || mergeTargetCommit.GetValueOrDefault() == lastMergeCommit.GetValueOrDefault());
                    completionResult = this.TryCompletePullRequest(userContext, repositoryById, ref pullRequest, sourceRef, targetRef, rebasedSourceCommitId, mergedExternally, ctData);
                  }
                  else
                    goto label_20;
                }
                if (completionResult == TeamFoundationGitPullRequestService.CompletePullRequestResult.MergeOutOfDate)
                {
                  if (registryService == null)
                    registryService = requestContext.GetService<IVssRegistryService>();
                }
                else
                  goto label_20;
              }
              while (mergesPerformed < registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/MaxCompletionMergeAttempts", true, 2));
              flag = true;
            }
            catch (Exception ex)
            {
              completionResult = TeamFoundationGitPullRequestService.CompletePullRequestResult.OtherError;
              caughtException = ex;
            }
label_20:
            if (pullRequest.Status != PullRequestStatus.Completed)
            {
              UnattendedCompletionFailedNotification.FailureReason reason;
              if (mergeStatus == PullRequestAsyncStatus.Conflicts)
                reason = UnattendedCompletionFailedNotification.FailureReason.MergeConflict;
              else if (mergeStatus == PullRequestAsyncStatus.RejectedByPolicy || completionResult == TeamFoundationGitPullRequestService.CompletePullRequestResult.RejectedByPolicy || caughtException is GitPullRequestUpdateRejectedByPolicyException)
                reason = UnattendedCompletionFailedNotification.FailureReason.RejectedByPolicy;
              else if (flag)
                reason = UnattendedCompletionFailedNotification.FailureReason.RetryLimitReached;
              else if (caughtException is GitRepositoryNotFoundException)
              {
                reason = UnattendedCompletionFailedNotification.FailureReason.RepositoryNotFound;
              }
              else
              {
                orchestration.SetOrchestrationException(caughtException);
                reason = UnattendedCompletionFailedNotification.FailureReason.OtherError;
              }
              UnattendedCompletionFailedNotification notification = new UnattendedCompletionFailedNotification(teamProjectUri, pullRequest.RepositoryId, repositoryName, pullRequest.PullRequestId, IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(userContext), mergesPerformed, reason, caughtException);
              this.SyncPublishNotification(userContext, (PullRequestNotification) notification, 1013494, ctData, "UnattendedCompletionFailedSyncPublishElapsedMs", (object) new
              {
                mergesPerformed = mergesPerformed,
                failureReason = reason
              });
            }
          }
          ctData.Add("PullRequestCompletionMergeCount", (object) mergesPerformed);
          if (caughtException != null)
          {
            string name = caughtException.GetType().Name;
            object valueOrDefault2 = ctData.GetData().GetValueOrDefault<string, object>("ExceptionType");
            if (valueOrDefault2 != null)
              ctData.GetData()["ExceptionType"] = (object) (valueOrDefault2?.ToString() + "," + name);
            else
              ctData.Add("ExceptionType", (object) name);
          }
          requestContext.TraceAlways(1013824, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", string.Format("PullRequest: {0}/{1}/{2}", (object) pullRequest.RepositoryId, (object) pullRequest.TargetBranchName, (object) pullRequest.PullRequestId));
          return pullRequest;
        }), (Func<TfsGitPullRequest, object>) (pr => (object) new
        {
          mergeStatus = mergeStatus,
          completionResult = completionResult,
          Status = pullRequest.Status,
          mergesPerformed = mergesPerformed,
          caughtExceptionType = caughtException?.GetType().Name,
          caughtExceptionAt = caughtException?.TargetSite.Name,
          caughtExceptionMsg = caughtException?.Message
        }), (Func<object>) (() => (object) new
        {
          PullRequestId = pullRequest.PullRequestId,
          CompleteWhenMergedAuthority = pullRequest.CompleteWhenMergedAuthority
        }), caller: nameof (MergeAndCompletePullRequest));
      }
    }

    private TeamFoundationGitPullRequestService.CompletePullRequestResult TryCompletePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ref TfsGitPullRequest pullRequest,
      TfsGitRef sourceRef,
      TfsGitRef targetRef,
      Sha1Id rebasedSourceCommitId,
      bool mergedExternally,
      ClientTraceData ctData)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (TryCompletePullRequest)))
      {
        TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        StatusUpdateNotification updateNotification = TeamFoundationGitPullRequestService.CreateStatusUpdateNotification(requestContext, repository, pullRequest.PullRequestId, PullRequestStatus.Completed, foundationIdentity, mergedExternally ? new Sha1Id?() : pullRequest.LastMergeCommit);
        try
        {
          using (requestContext.TimeRegion("PublishDecisionPoint", nameof (TryCompletePullRequest)))
            service1.PublishDecisionPoint(requestContext, (object) updateNotification);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          ctData.Add("PullRequestRejectedByPolicy", (object) true);
          throw new GitPullRequestUpdateRejectedByPolicyException((Exception) ex);
        }
        TeamFoundationGitPullRequestService.CompletePullRequestResult pullRequestResult;
        if (mergedExternally)
        {
          if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.Conflicts"))
            TeamFoundationGitPullRequestService.DeleteAllInternalPullRequestRefs(requestContext, pullRequest);
          else
            repository.UpdatePullRequestMergeRef(requestContext, pullRequest.PullRequestId, new Sha1Id?(Sha1Id.Empty));
          pullRequestResult = TeamFoundationGitPullRequestService.CompletePullRequestResult.Success;
          try
          {
            ref TfsGitPullRequest local = ref pullRequest;
            IVssRequestContext requestContext1 = requestContext;
            Guid projectId = repository.Key.ProjectId;
            TfsGitPullRequest pullRequest1 = pullRequest;
            PullRequestStatus? status = new PullRequestStatus?(PullRequestStatus.Completed);
            Sha1Id? nullable1 = new Sha1Id?(Sha1Id.Empty);
            Guid? nullable2 = new Guid?(foundationIdentity.TeamFoundationId);
            PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
            Sha1Id? lastMergeSourceCommit = new Sha1Id?();
            Sha1Id? lastMergeTargetCommit = new Sha1Id?();
            Sha1Id? lastMergeCommit = nullable1;
            Guid? completeWhenMergedAuthority = nullable2;
            PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
            DateTime? completionQueueTime = new DateTime?();
            Guid? autoCompleteAuthority = new Guid?();
            bool? isDraft = new bool?();
            TfsGitPullRequest tfsGitPullRequest = this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
            local = tfsGitPullRequest;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1013390, TraceLevel.Error, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex);
            throw;
          }
        }
        else
        {
          this.ThrowIfBranchLockedByOtherUser(requestContext, targetRef);
          IEnumerable<Sha1Id> pullRequestCommits;
          try
          {
            pullRequestCommits = this.GetPullRequestCommits(requestContext, pullRequest);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1013090, TraceLevel.Error, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex);
            throw;
          }
          pullRequestResult = this.UpdateTargetBranchToMergeCommit(requestContext, repository, pullRequest, ctData);
          if (pullRequestResult != TeamFoundationGitPullRequestService.CompletePullRequestResult.Success)
            return pullRequestResult;
          try
          {
            ref TfsGitPullRequest local = ref pullRequest;
            IVssRequestContext requestContext2 = requestContext;
            Guid projectId = repository.Key.ProjectId;
            TfsGitPullRequest pullRequest2 = pullRequest;
            PullRequestStatus? status = new PullRequestStatus?(PullRequestStatus.Completed);
            Sha1Id? mergeSourceCommit = pullRequest.LastMergeSourceCommit;
            Sha1Id? mergeTargetCommit = pullRequest.LastMergeTargetCommit;
            Sha1Id? lastMergeCommit1 = pullRequest.LastMergeCommit;
            Guid? nullable = new Guid?(foundationIdentity.TeamFoundationId);
            IEnumerable<Sha1Id> sha1Ids = pullRequestCommits;
            PullRequestAsyncStatus? mergeStatus = new PullRequestAsyncStatus?();
            Sha1Id? lastMergeSourceCommit = mergeSourceCommit;
            Sha1Id? lastMergeTargetCommit = mergeTargetCommit;
            Sha1Id? lastMergeCommit2 = lastMergeCommit1;
            Guid? completeWhenMergedAuthority = nullable;
            PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
            DateTime? completionQueueTime = new DateTime?();
            IEnumerable<Sha1Id> commits = sha1Ids;
            Guid? autoCompleteAuthority = new Guid?();
            bool? isDraft = new bool?();
            TfsGitPullRequest tfsGitPullRequest = this.UpdatePullRequestInDatabase(requestContext2, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit2, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, commits: commits, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
            local = tfsGitPullRequest;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1013389, TraceLevel.Error, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex);
            throw;
          }
        }
        TfsGitPullRequest prCopy = pullRequest;
        IEnumerable<int> workItemIds = pullRequest.GetAssociatedWorkItems(requestContext, repository);
        ctData.LogElapsedMs("LinkWorkItemsToCommitElapsedMs", (Action) (() =>
        {
          try
          {
            this.LinkWorkItemsToCommit(requestContext, repository, prCopy, workItemIds);
          }
          catch (Exception ex)
          {
            TracepointUtils.TraceException(requestContext, 1013496, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex, (object) new
            {
              PullRequestId = prCopy.PullRequestId,
              RepositoryId = prCopy.RepositoryId,
              LastMergeCommit = prCopy.LastMergeCommit
            }, caller: nameof (TryCompletePullRequest));
          }
        }));
        if (!mergedExternally && pullRequest.Status == PullRequestStatus.Completed)
        {
          IDictionary<int, string> workItemIdToStateMap = (IDictionary<int, string>) null;
          ITeamFoundationWorkItemService service2 = requestContext.GetService<ITeamFoundationWorkItemService>();
          if (pullRequest.CompletionOptions != null && pullRequest.CompletionOptions.TransitionWorkItems)
          {
            ctData.LogElapsedMs("TransitionWorkItemsElapsedMs", (Action) (() =>
            {
              try
              {
                this.TransitionWorkItems(requestContext, workItemIds, Resources.Format("PullRequestWorkItemsTransitionedMessage", (object) prCopy.PullRequestId));
              }
              catch (Exception ex)
              {
                TracepointUtils.TraceException(requestContext, 1013497, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex, (object) new
                {
                  PullRequestId = prCopy.PullRequestId,
                  RepositoryId = prCopy.RepositoryId
                }, caller: nameof (TryCompletePullRequest));
              }
            }));
            if (requestContext.IsFeatureEnabled(MentionFeatureFlags.GitCommitMessageWITTransition))
              requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, string.Format("GitCommitMentionPullRequestProcessorJob (repo={0}, pr={1})", (object) pullRequest.RepositoryId, (object) pullRequest.PullRequestId), "Microsoft.TeamFoundation.Mention.Server.Plugins.Jobs.GitCommitMentionPullRequestProcessorJob", CommitMentionProcessingData.CreateCommitMentionProcessingDataXml(pullRequest.PullRequestId), JobPriorityLevel.Normal, JobPriorityClass.Normal, TimeSpan.Zero);
          }
          else if (this.IsWITResolutionEnabled(requestContext, pullRequest.RepositoryId))
          {
            if (service2.TryUpdateWorkItemIdToStateMap(requestContext, pullRequest.Description, out workItemIdToStateMap))
            {
              try
              {
                this.TransitionWorkItems(requestContext, workItemIds, Resources.Format("PullRequestWorkItemsTransitionedUsingDescriptionMessage", (object) prCopy.PullRequestId), workItemIdToStateMap);
              }
              catch (Exception ex)
              {
                TracepointUtils.TraceException(requestContext, 1013497, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", ex, (object) new
                {
                  PullRequestId = prCopy.PullRequestId,
                  RepositoryId = prCopy.RepositoryId
                }, caller: nameof (TryCompletePullRequest));
              }
            }
          }
          if (pullRequest.CompletionOptions != null)
          {
            if (pullRequest.CompletionOptions.DeleteSourceBranch)
            {
              GitRefUpdateStatus status = this.UpdateSourceBranch(requestContext, repository, pullRequest, Sha1Id.Empty);
              if (status.IsSuccessful())
              {
                updateNotification.SourceBranchState = PullRequestCompletionSourceBranchState.Deleted;
              }
              else
              {
                switch (status)
                {
                  case GitRefUpdateStatus.ForcePushRequired:
                  case GitRefUpdateStatus.WritePermissionRequired:
                  case GitRefUpdateStatus.CreateBranchPermissionRequired:
                  case GitRefUpdateStatus.CreateTagPermissionRequired:
                    updateNotification.SourceBranchState = PullRequestCompletionSourceBranchState.DeleteFailedPermission;
                    break;
                  case GitRefUpdateStatus.Locked:
                    updateNotification.SourceBranchState = PullRequestCompletionSourceBranchState.DeleteFailedLocked;
                    break;
                  case GitRefUpdateStatus.RejectedByPolicy:
                    updateNotification.SourceBranchState = PullRequestCompletionSourceBranchState.DeleteFailedPolicy;
                    break;
                  default:
                    updateNotification.SourceBranchState = PullRequestCompletionSourceBranchState.DeleteFailed;
                    break;
                }
              }
            }
            else
            {
              GitPullRequestMergeStrategy? mergeStrategy = pullRequest.CompletionOptions.MergeStrategy;
              GitPullRequestMergeStrategy requestMergeStrategy1 = GitPullRequestMergeStrategy.Rebase;
              if (!(mergeStrategy.GetValueOrDefault() == requestMergeStrategy1 & mergeStrategy.HasValue))
              {
                mergeStrategy = pullRequest.CompletionOptions.MergeStrategy;
                GitPullRequestMergeStrategy requestMergeStrategy2 = GitPullRequestMergeStrategy.RebaseMerge;
                if (!(mergeStrategy.GetValueOrDefault() == requestMergeStrategy2 & mergeStrategy.HasValue))
                  goto label_42;
              }
              int num = (int) this.UpdateSourceBranch(requestContext, repository, pullRequest, rebasedSourceCommitId);
            }
          }
        }
label_42:
        if (mergedExternally)
          service1.PublishNotification(requestContext, (object) updateNotification);
        else
          this.SyncPublishNotification(requestContext, (PullRequestNotification) updateNotification, 1013495, ctData, "StatusUpdateSyncPublishElapsedMs", (object) new
          {
            LastMergeCommit = pullRequest.LastMergeCommit
          });
        return pullRequestResult;
      }
    }

    private bool IsWITResolutionEnabled(IVssRequestContext requestContext, Guid repositoryId)
    {
      string query = "/WebAccess" + string.Format("/VersionControl/Repositories/{0}/WitResolutionMentionsEnabled", (object) repositoryId);
      return requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) query, true);
    }

    private IEnumerable<Sha1Id> GetPullRequestCommits(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (GetPullRequestCommits)))
      {
        Sha1Id? nullable = pullRequest.LastMergeTargetCommit;
        Sha1Id sha1Id = nullable ?? Sha1Id.Empty;
        nullable = pullRequest.LastMergeCommit;
        Sha1Id mergeId = nullable ?? Sha1Id.Empty;
        if (!mergeId.IsEmpty && !sha1Id.IsEmpty)
        {
          GitPullRequestCompletionOptions completionOptions = pullRequest.CompletionOptions;
          int num;
          if (completionOptions == null)
          {
            num = 0;
          }
          else
          {
            GitPullRequestMergeStrategy? mergeStrategy = completionOptions.MergeStrategy;
            GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.Squash;
            num = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
          }
          if (num == 0)
          {
            using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
              return (IEnumerable<Sha1Id>) repositoryById.GetCommitHistory(requestContext, mergeId, new Sha1Id?(sha1Id)).Where<Sha1Id>((Func<Sha1Id, bool>) (id => id != mergeId)).ToList<Sha1Id>();
          }
        }
        return (IEnumerable<Sha1Id>) null;
      }
    }

    private TeamFoundationGitPullRequestService.CompletePullRequestResult UpdateTargetBranchToMergeCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ClientTraceData ctData)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (UpdateTargetBranchToMergeCommit)))
      {
        requestContext.Trace(1013387, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", "Trying to update Git refs for the {0} branch from {1} to {2}.", (object) pullRequest.TargetBranchName, (object) pullRequest.LastMergeTargetCommit, (object) pullRequest.LastMergeCommit);
        Sha1Id? nullable = pullRequest.MergeStatus == PullRequestAsyncStatus.Succeeded ? pullRequest.LastMergeCommit : throw new InvalidOperationException();
        if (nullable.HasValue)
        {
          nullable = pullRequest.LastMergeCommit;
          if (!nullable.Value.IsEmpty)
          {
            using (TfsGitPullRequestRefUpdateValidator refUpdateValidator = new TfsGitPullRequestRefUpdateValidator(pullRequest))
            {
              ITeamFoundationGitRefService service = requestContext.GetService<ITeamFoundationGitRefService>();
              string targetBranchName = pullRequest.TargetBranchName;
              nullable = pullRequest.LastMergeTargetCommit;
              Sha1Id oldObjectId = nullable.Value;
              nullable = pullRequest.LastMergeCommit;
              Sha1Id newObjectId = nullable.Value;
              List<TfsGitRefUpdateRequest> refUpdates = new List<TfsGitRefUpdateRequest>()
              {
                new TfsGitRefUpdateRequest(targetBranchName, oldObjectId, newObjectId)
              };
              TfsGitRefUpdateResultSet refUpdateResultSet;
              try
              {
                refUpdateResultSet = service.UpdateRefs(requestContext, repository.Key.RepoId, refUpdates, GitRefUpdateMode.AllOrNone, refUpdateValidator: (ITeamFoundationGitRefUpdateValidator) refUpdateValidator);
              }
              catch
              {
                if (refUpdateValidator.Transaction != null)
                  refUpdateValidator.Transaction.Discard();
                throw;
              }
              PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy> transaction = refUpdateValidator.Transaction;
              if (refUpdateResultSet.Results.Count == 1)
              {
                switch (refUpdateResultSet.Results[0].Status)
                {
                  case GitRefUpdateStatus.Succeeded:
                    if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.Conflicts"))
                      TeamFoundationGitPullRequestService.DeleteAllInternalPullRequestRefs(requestContext, pullRequest);
                    else
                      repository.UpdatePullRequestMergeRef(requestContext, pullRequest.PullRequestId, new Sha1Id?(Sha1Id.Empty));
                    if (transaction != null)
                    {
                      transaction.Finalize(requestContext, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus, TeamFoundationPolicyEvaluationRecordContext, TeamFoundationPolicyEvaluationRecordContext>) ((policy, status, context) => policy.OnCommitted(requestContext, pullRequest, status, context)), ctData);
                      transaction.Save(requestContext);
                    }
                    return TeamFoundationGitPullRequestService.CompletePullRequestResult.Success;
                  case GitRefUpdateStatus.StaleOldObjectId:
                    transaction?.Discard();
                    return TeamFoundationGitPullRequestService.CompletePullRequestResult.MergeOutOfDate;
                  case GitRefUpdateStatus.RejectedByPolicy:
                    transaction?.Save(requestContext, ctData);
                    throw new GitPullRequestUpdateRejectedByPolicyException(refUpdateResultSet.Results[0].CustomMessage);
                }
              }
              transaction?.Save(requestContext, ctData);
              return TeamFoundationGitPullRequestService.CompletePullRequestResult.OtherError;
            }
          }
        }
      }
    }

    private List<TfsGitPullRequest> GetPullRequestsInCompletionQueue(
      IVssRequestContext requestContext,
      Guid repositoryId,
      string targetName)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (GetPullRequestsInCompletionQueue)))
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<TfsGitPullRequest> list;
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          list = gitCoreComponent.QueryGitPullRequests((string) null, new Guid?(repositoryId), new Guid?(), (IEnumerable<string>) null, (IEnumerable<string>) new string[1]
          {
            targetName
          }, new bool?(false), new PullRequestStatus?(PullRequestStatus.Active), (IEnumerable<Guid>) null, (IEnumerable<Guid>) null, new DateTime?(), new Guid?(), true, 0, 0, true).ToList<TfsGitPullRequest>();
        this.AddToQueryPullRequestsPerformanceCounters("CompletionQueue", stopwatch.ElapsedMilliseconds);
        return list;
      }
    }

    private GitRefUpdateStatus UpdateSourceBranch(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      Sha1Id newObjectId)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (UpdateSourceBranch)))
      {
        ITeamFoundationGitRefService service = requestContext.GetService<ITeamFoundationGitRefService>();
        Guid sourceRepoId = pullRequest.SourceRepositoryId != Guid.Empty ? pullRequest.SourceRepositoryId : repository.Key.RepoId;
        string sourceBranchName = pullRequest.ForkSource?.RefName ?? pullRequest.SourceBranchName;
        if (newObjectId == Sha1Id.Empty && this.RefHasActivePullRequests(requestContext, repository, sourceBranchName, 1))
        {
          TracepointUtils.Tracepoint(requestContext, 1013848, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<object>) (() => (object) new
          {
            Message = "Source branch has active pull request and it will not be deleted",
            sourceRepoId = sourceRepoId,
            PullRequestId = pullRequest.PullRequestId,
            sourceBranchName = sourceBranchName,
            LastMergeSourceCommit = pullRequest.LastMergeSourceCommit
          }), TraceLevel.Info, true, nameof (UpdateSourceBranch));
          return GitRefUpdateStatus.Unprocessed;
        }
        List<TfsGitRefUpdateRequest> refUpdates = new List<TfsGitRefUpdateRequest>()
        {
          new TfsGitRefUpdateRequest(sourceBranchName, pullRequest.LastMergeSourceCommit.Value, newObjectId)
        };
        TfsGitRefUpdateResultSet refUpdateResultSet = service.UpdateRefs(requestContext, sourceRepoId, refUpdates);
        foreach (TfsGitRefUpdateResult gitRefUpdateResult in refUpdateResultSet.Results.DefaultIfEmpty<TfsGitRefUpdateResult>().Where<TfsGitRefUpdateResult>((Func<TfsGitRefUpdateResult, bool>) (r => r == null || !r.Succeeded)))
        {
          TfsGitRefUpdateResult failedResult = gitRefUpdateResult;
          TracepointUtils.Tracepoint(requestContext, 1013534, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<object>) (() => (object) new
          {
            sourceRepoId = sourceRepoId,
            PullRequestId = pullRequest.PullRequestId,
            sourceBranchName = sourceBranchName,
            LastMergeSourceCommit = pullRequest.LastMergeSourceCommit,
            Status = failedResult?.Status.ToString(),
            CustomMessage = failedResult?.CustomMessage,
            RejectedBy = failedResult?.RejectedBy,
            IsLockedById = ((Guid?) failedResult?.IsLockedById ?? Guid.Empty)
          }), TraceLevel.Error, caller: nameof (UpdateSourceBranch));
        }
        TfsGitRefUpdateResult gitRefUpdateResult1 = refUpdateResultSet.Results.FirstOrDefault<TfsGitRefUpdateResult>();
        return gitRefUpdateResult1 != null ? gitRefUpdateResult1.Status : GitRefUpdateStatus.Unprocessed;
      }
    }

    private void ThrowIfBranchLockedByOtherUser(IVssRequestContext requestContext, TfsGitRef branch)
    {
      if (branch == null || !branch.IsLockedById.HasValue)
        return;
      TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
      Guid? isLockedById = branch.IsLockedById;
      Guid teamFoundationId1 = foundationIdentity.TeamFoundationId;
      if ((isLockedById.HasValue ? (isLockedById.HasValue ? (isLockedById.GetValueOrDefault() != teamFoundationId1 ? 1 : 0) : 0) : 1) != 0)
      {
        IdentityHelper instance = IdentityHelper.Instance;
        IVssRequestContext requestContext1 = requestContext;
        isLockedById = branch.IsLockedById;
        Guid teamFoundationId2 = isLockedById.Value;
        string displayName = instance.GetDisplayName(requestContext1, teamFoundationId2);
        throw new GitPullRequestTargetLockedException(branch, displayName);
      }
    }

    private bool IsBranchLockedByOtherUser(IVssRequestContext requestContext, TfsGitRef branch)
    {
      if (branch != null && branch.IsLockedById.HasValue)
      {
        TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
        Guid? isLockedById = branch.IsLockedById;
        Guid teamFoundationId = foundationIdentity.TeamFoundationId;
        if ((isLockedById.HasValue ? (isLockedById.HasValue ? (isLockedById.GetValueOrDefault() != teamFoundationId ? 1 : 0) : 0) : 1) != 0)
          return true;
      }
      return false;
    }

    internal virtual void QueueCompletionJobToRun(
      IVssRequestContext requestContext,
      Guid repositoryId,
      string targetBranchName)
    {
      Guid jobId = TeamFoundationGitPullRequestService.JobIdForCompletionQueue(repositoryId, targetBranchName);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, jobId);
      string name = "PR Completion repo=" + repositoryId.ToString("N") + " target=" + GitUtils.GetFriendlyBranchName(targetBranchName);
      if (name.Length > 126)
        name = name.Substring(0, 123) + "...";
      if (foundationJobDefinition == null || foundationJobDefinition.PriorityClass != JobPriorityClass.High || foundationJobDefinition.Name != name)
      {
        XmlNode completionJobDataXml = TeamFoundationGitPullRequestService.CreateCompletionJobDataXml(repositoryId, targetBranchName);
        foundationJobDefinition = new TeamFoundationJobDefinition(jobId, name, "Microsoft.TeamFoundation.Git.Server.Plugins.GitPullRequestCompletionQueueJob", completionJobDataXml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.High);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
      }
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      }, JobPriorityLevel.Highest);
    }

    public static XmlNode CreateCompletionJobDataXml(Guid repositoryId, string targetBranchName)
    {
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetBranchName, nameof (targetBranchName));
      return TeamFoundationSerializationUtility.SerializeToXml((object) new CompletionQueueJobData()
      {
        RepositoryId = repositoryId,
        TargetBranchName = targetBranchName
      });
    }

    public static void ParseCompletionJobDataXml(
      XmlNode xmlData,
      out Guid repositoryId,
      out string targetBranchName)
    {
      ArgumentUtility.CheckForNull<XmlNode>(xmlData, nameof (xmlData));
      CompletionQueueJobData completionQueueJobData = TeamFoundationSerializationUtility.Deserialize<CompletionQueueJobData>(xmlData);
      repositoryId = completionQueueJobData.RepositoryId;
      targetBranchName = completionQueueJobData.TargetBranchName;
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckStringForNullOrEmpty(targetBranchName, nameof (targetBranchName));
    }

    internal static Guid OrchestrationIdForCompletionQueueJob(TfsGitPullRequest pullRequest) => DeterministicGuid.Compute(string.Format("CompletionQueueJob:{0}:{1}:{2}", (object) pullRequest.RepositoryId.ToString("N"), (object) pullRequest.PullRequestId, (object) pullRequest.CompletionQueueTime.Ticks));

    internal static Guid JobIdForCompletionQueue(Guid repositoryId, string targetBranchName) => DeterministicGuid.ComputeFromDeprecatedSha1(repositoryId.ToString("N") + ":" + targetBranchName);

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdateCurrentUserVote(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      ReviewerVote newVoteValue,
      bool? isFlagged = null,
      bool? hasDeclined = null,
      bool? isReapprove = null)
    {
      requestContext.Trace(1013332, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to update pull request {0} (reviewer vote update)", (object) pullRequestId);
      ArgumentUtility.CheckForDefinedEnum<ReviewerVote>(newVoteValue, nameof (newVoteValue));
      TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      if (pullRequestDetails.IsDraft && newVoteValue != ReviewerVote.None)
        throw new GitPullRequestDraftCannotVoteException();
      TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
      if (repository.Settings.StrictVoteMode)
        repository.Permissions.CheckWrite(pullRequestDetails.TargetBranchName);
      else
        repository.Permissions.CheckPullRequestContribute();
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers1 = pullRequestDetails.Reviewers;
      TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
      Guid thisRequestUser = foundationIdentity.TeamFoundationId;
      if (hasDeclined.HasValue && hasDeclined.Value && thisRequestUser == pullRequestDetails.Creator)
        throw new GitPullRequestReviewerCantBeFlaggedOrDeclinedException(Resources.Get("PullRequestCreatorCannotDecline"));
      IEnumerable<Guid> allReviewersIds = reviewers1.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (reviewer => reviewer.Reviewer));
      List<Guid> votedForList = TeamFoundationGitPullRequestService.GetVotedForList(requestContext, foundationIdentity, allReviewersIds);
      Guid reviewer1 = thisRequestUser;
      int vote = (int) (short) newVoteValue;
      List<Guid> votedFor = votedForList;
      bool flag1 = hasDeclined.HasValue && hasDeclined.Value;
      int? pullRequestId1 = new int?();
      int num = flag1 ? 1 : 0;
      TfsGitPullRequest.ReviewerWithVote reviewerWithVote1 = new TfsGitPullRequest.ReviewerWithVote(reviewer1, (short) vote, ReviewerVoteStatus.None, (IEnumerable<Guid>) votedFor, pullRequestId: pullRequestId1, hasDeclined: num != 0);
      requestContext.Trace(1013230, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Current user vote is {0}", (object) newVoteValue);
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Action", (object) "UpdatePullRequest");
      properties.Add("PullRequestId", (object) pullRequestId);
      properties.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
      properties.Add("RepositoryName", (object) repository.Name);
      ClientTraceService service1 = requestContext.GetService<ClientTraceService>();
      ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
      List<PullRequestNotification> requestNotificationList = new List<PullRequestNotification>();
      TfsGitPullRequest.ReviewerWithVote reviewerWithVote2 = reviewers1.FirstOrDefault<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.Reviewer == thisRequestUser));
      if (reviewerWithVote2 == null)
      {
        ReviewersUpdateNotification updateNotification = new ReviewersUpdateNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, reviewers1, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) new TfsGitPullRequest.ReviewerWithVote[1]
        {
          reviewerWithVote1
        }, true, (reviewerWithVote1.Vote == (short) 0 ? 1 : 0) != 0);
        try
        {
          service2.PublishDecisionPoint(requestContext, (object) updateNotification);
          requestNotificationList.Add((PullRequestNotification) new SyncReviewersUpdateNotification(updateNotification));
          requestNotificationList.Add((PullRequestNotification) updateNotification);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          properties.Add("PullRequestRejectedByPolicy", (object) true);
          service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
          throw new GitPullRequestUpdateRejectedByPolicyException((Exception) ex);
        }
      }
      bool flag2 = isFlagged.HasValue && reviewerWithVote2 != null && isFlagged.Value != reviewerWithVote2.IsFlagged;
      bool flag3 = hasDeclined.HasValue && reviewerWithVote2 != null && hasDeclined.Value != reviewerWithVote2.HasDeclined;
      if (reviewerWithVote2 != null && (int) reviewerWithVote2.Vote == (int) (short) newVoteValue && !flag2 && !flag3)
      {
        bool? nullable = isReapprove;
        bool flag4 = true;
        if (!(nullable.GetValueOrDefault() == flag4 & nullable.HasValue))
        {
          service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
          return pullRequestDetails.Reviewers is IReadOnlyList<TfsGitPullRequest.ReviewerWithVote> reviewers2 ? (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) reviewers2 : (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) pullRequestDetails.Reviewers.ToArray<TfsGitPullRequest.ReviewerWithVote>();
        }
      }
      if (reviewerWithVote2 != null || reviewerWithVote1.Vote != (short) 0)
      {
        ReviewerVoteNotification notification = new ReviewerVoteNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, reviewerWithVote1.Vote);
        requestNotificationList.Add((PullRequestNotification) new SyncReviewerVoteNotification(notification));
        requestNotificationList.Add((PullRequestNotification) notification);
      }
      if (!isFlagged.HasValue && newVoteValue != ReviewerVote.None)
        isFlagged = new bool?(false);
      if (!hasDeclined.HasValue && newVoteValue != ReviewerVote.None)
        hasDeclined = new bool?(false);
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> source;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        source = gitCoreComponent.UpdatePullRequestReviewerVote(repository.Key, pullRequestId, thisRequestUser, (short) newVoteValue, (IEnumerable<Guid>) votedForList, isFlagged, hasDeclined);
      requestContext.Trace(1013331, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Publishing {0} PullRequestNotifications for pull request {1} (reviewer vote update)", (object) requestNotificationList.Count, (object) pullRequestId);
      foreach (PullRequestNotification notificationEvent in requestNotificationList)
      {
        switch (notificationEvent)
        {
          case SyncReviewersUpdateNotification _:
          case SyncReviewerVoteNotification _:
            service2.SyncPublishNotification(requestContext, (object) notificationEvent);
            continue;
          default:
            service2.PublishNotification(requestContext, (object) notificationEvent);
            continue;
        }
      }
      properties.Add("PullRequestReviewerCount", (object) source.Count<TfsGitPullRequest.ReviewerWithVote>());
      properties.Add("PullRequestVoteCount", (object) source.Count<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.Vote != (short) 0)));
      service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
      try
      {
        pullRequestDetails.UpdateReviewerVote(requestContext, repository.Key.ProjectId, thisRequestUser, newVoteValue);
      }
      catch (CodeReviewNotActiveException ex)
      {
        throw new GitPullRequestNotEditableException();
      }
      return source;
    }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdateReviewer(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      Guid reviewerId,
      bool? isFlagged,
      bool? hasDeclined)
    {
      TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
      repository.Permissions.CheckPullRequestContribute();
      if (hasDeclined.HasValue && hasDeclined.Value && reviewerId == pullRequestDetails.Creator)
        throw new GitPullRequestReviewerCantBeFlaggedOrDeclinedException(Resources.Get("PullRequestCreatorCannotDecline"));
      if (hasDeclined.HasValue && hasDeclined.Value && reviewerId != requestContext.GetAuthenticatedId())
        throw new GitPullRequestReviewerCantBeFlaggedOrDeclinedException(Resources.Get("PullRequestOnlyCurrentUserCanDecline"));
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers1 = pullRequestDetails.Reviewers;
      TfsGitPullRequest.ReviewerWithVote reviewerWithVote1 = reviewers1.FirstOrDefault<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.Reviewer == reviewerId));
      if (reviewerWithVote1 == null)
        throw new GitPullRequestReviewerCantBeFlaggedOrDeclinedException(Resources.Get("PullRequestInvalidReviewerForFlagOrDecline"));
      if ((!isFlagged.HasValue || reviewerWithVote1.IsFlagged == isFlagged.Value) && (!hasDeclined.HasValue || reviewerWithVote1.HasDeclined == hasDeclined.Value))
        return pullRequestDetails.Reviewers is IReadOnlyList<TfsGitPullRequest.ReviewerWithVote> reviewers2 ? (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) reviewers2 : (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) pullRequestDetails.Reviewers.ToArray<TfsGitPullRequest.ReviewerWithVote>();
      TeamFoundationIdentity foundationIdentity1 = TeamFoundationGitPullRequestService.GetMappedIdentities(requestContext, new Guid[1]
      {
        reviewerWithVote1.Reviewer
      }).FirstOrDefault<TeamFoundationIdentity>();
      if (foundationIdentity1 == null || foundationIdentity1.IsContainer)
        throw new GitPullRequestReviewerCantBeFlaggedOrDeclinedException(Resources.Get("PullRequestInvalidReviewerForFlagOrDecline"));
      ClientTraceService service1 = requestContext.GetService<ClientTraceService>();
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Action", (object) "UpdatePullRequest");
      properties.Add("PullRequestId", (object) pullRequestId);
      properties.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
      properties.Add("RepositoryName", (object) repository.Name);
      if (isFlagged.HasValue && isFlagged.Value && !hasDeclined.HasValue)
        hasDeclined = new bool?(false);
      if (hasDeclined.HasValue && hasDeclined.Value && !isFlagged.HasValue)
        isFlagged = new bool?(false);
      TfsGitPullRequest.ReviewerWithVote reviewerWithVote2 = new TfsGitPullRequest.ReviewerWithVote(reviewerWithVote1.Reviewer, reviewerWithVote1.Vote, reviewerWithVote1.Status, (IEnumerable<Guid>) reviewerWithVote1.VotedFor, reviewerWithVote1.IsRequired, reviewerWithVote1.PullRequestId, isFlagged.HasValue ? isFlagged.Value : reviewerWithVote1.IsFlagged, hasDeclined.HasValue ? hasDeclined.Value : reviewerWithVote1.HasDeclined);
      TeamFoundationIdentity foundationIdentity2 = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
      ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
      ReviewersUpdateNotification updateNotification = new ReviewersUpdateNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity2, reviewers1, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) new TfsGitPullRequest.ReviewerWithVote[1]
      {
        reviewerWithVote2
      }, true, true);
      try
      {
        service2.PublishDecisionPoint(requestContext, (object) updateNotification);
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        properties.Add("PullRequestRejectedByPolicy", (object) true);
        service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
        throw new GitPullRequestUpdateRejectedByPolicyException((Exception) ex);
      }
      IEnumerable<Guid> allReviewersIds = reviewers1.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (reviewer => reviewer.Reviewer));
      List<Guid> votedForList = TeamFoundationGitPullRequestService.GetVotedForList(requestContext, foundationIdentity2, allReviewersIds);
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> source;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        source = gitCoreComponent.UpdatePullRequestReviewerVote(repository.Key, pullRequestId, reviewerWithVote1.Reviewer, !hasDeclined.HasValue || !hasDeclined.Value ? reviewerWithVote1.Vote : (short) 0, (IEnumerable<Guid>) votedForList, isFlagged, hasDeclined);
      service2.SyncPublishNotification(requestContext, (object) new SyncReviewersUpdateNotification(updateNotification));
      service2.PublishNotification(requestContext, (object) updateNotification);
      properties.Add("PullRequestFlaggedReviewersCount", (object) source.Count<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.IsFlagged)));
      service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
      return source;
    }

    public virtual IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetAllReviewerVotes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      bool userInitiated = false,
      string reason = null)
    {
      TracepointUtils.Tracepoint(requestContext, 1013080, GitServerUtils.TraceArea, "TfsGitPullRequest", (Func<object>) (() => (object) new
      {
        pullRequestId = pullRequestId,
        userInitiated = userInitiated
      }), caller: nameof (ResetAllReviewerVotes));
      TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
      if (userInitiated)
        repository.Permissions.CheckPullRequestContribute();
      ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
      TeamFoundationIdentity initiator = (TeamFoundationIdentity) null;
      if (userInitiated)
        initiator = service1.ReadRequestIdentity(requestContext);
      Guid[] reviewerGuidsToReset = pullRequestDetails.Reviewers.Where<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => !r.NotVoted)).Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (r => r.Reviewer)).ToArray<Guid>();
      reviewerGuidsToReset = ((IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext.Elevate(), reviewerGuidsToReset)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null && !id.IsContainer)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (r => r.TeamFoundationId)).ToArray<Guid>();
      if (!((IEnumerable<Guid>) reviewerGuidsToReset).Any<Guid>())
        return pullRequestDetails.Reviewers;
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> source1;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        source1 = gitCoreComponent.ResetAllPullRequestReviewerVotes(repository.Key, pullRequestId, (IEnumerable<Guid>) reviewerGuidsToReset);
      pullRequestDetails.ResetAllReviewerVotes(requestContext, repository.Key.ProjectId);
      ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
      IEnumerable<TeamFoundationIdentity> source2 = (IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext.Elevate(), source1.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (r => r.Reviewer)).ToArray<Guid>());
      foreach (TeamFoundationIdentity reviewer in source2)
      {
        SyncReviewerVoteNotification notificationEvent = new SyncReviewerVoteNotification(new ReviewerVoteNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, reviewer, (short) 0, false));
        service2.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      IEnumerable<TeamFoundationIdentity> reviewers = source2.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (r => ((IEnumerable<Guid>) reviewerGuidsToReset).Contains<Guid>(r.TeamFoundationId)));
      ResetMultipleVotesNotification notificationEvent1 = new ResetMultipleVotesNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, reviewers, true, initiator, reason);
      service2.PublishNotification(requestContext, (object) notificationEvent1);
      return source1;
    }

    public virtual IEnumerable<TfsGitPullRequest.ReviewerWithVote> ResetSomeReviewerVotes(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      IList<Guid> reviewerGuidsToReset,
      bool userInitiated = false,
      string reason = null)
    {
      TracepointUtils.Tracepoint(requestContext, 1013088, GitServerUtils.TraceArea, "TfsGitPullRequest", (Func<object>) (() => (object) new
      {
        pullRequestId = pullRequestId,
        reviewerGuidsToReset = reviewerGuidsToReset
      }), caller: nameof (ResetSomeReviewerVotes));
      TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
      ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
      TeamFoundationIdentity initiator = (TeamFoundationIdentity) null;
      if (userInitiated)
      {
        repository.Permissions.CheckPullRequestContribute();
        initiator = service1.ReadRequestIdentity(requestContext);
      }
      reviewerGuidsToReset = (IList<Guid>) pullRequestDetails.Reviewers.Where<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => !r.NotVoted)).Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (r => r.Reviewer)).Where<Guid>((Func<Guid, bool>) (r => reviewerGuidsToReset.Contains(r))).ToArray<Guid>();
      reviewerGuidsToReset = (IList<Guid>) ((IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext.Elevate(), (Guid[]) reviewerGuidsToReset)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null && !id.IsContainer)).Select<TeamFoundationIdentity, Guid>((Func<TeamFoundationIdentity, Guid>) (id => id.TeamFoundationId)).ToList<Guid>();
      if (!reviewerGuidsToReset.Any<Guid>())
        return pullRequestDetails.Reviewers;
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> source;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        source = gitCoreComponent.ResetSomePullRequestReviewerVotes(repository.Key, pullRequestId, (IEnumerable<Guid>) reviewerGuidsToReset);
      pullRequestDetails.ResetSomeReviewerVotes(requestContext, repository.Key.ProjectId, (IEnumerable<Guid>) reviewerGuidsToReset);
      ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
      foreach (TeamFoundationIdentity reviewer in ((IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext.Elevate(), source.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (r => r.Reviewer)).ToArray<Guid>())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (r => reviewerGuidsToReset.Contains(r.TeamFoundationId))))
      {
        SyncReviewerVoteNotification notificationEvent = new SyncReviewerVoteNotification(new ReviewerVoteNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, reviewer, (short) 0, false));
        service2.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      ResetMultipleVotesNotification notificationEvent1 = new ResetMultipleVotesNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, (IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext.Elevate(), reviewerGuidsToReset.ToArray<Guid>()), false, initiator, reason);
      service2.PublishNotification(requestContext, (object) notificationEvent1);
      return source;
    }

    public IEnumerable<TfsGitPullRequest.ReviewerWithVote> UpdatePullRequestReviewers(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete,
      bool createDiscussionMessage,
      bool sendEmailNotification,
      bool notifyPolicies,
      bool userInitiated = true)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (UpdatePullRequestReviewers)))
      {
        ArgumentUtility.CheckForNull<IEnumerable<TfsGitPullRequest.ReviewerBase>>(reviewersToUpdate, nameof (reviewersToUpdate));
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(reviewerIdsToDelete, nameof (reviewerIdsToDelete));
        requestContext.Trace(1013328, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to update pull request {0} (reviewer update)", (object) pullRequestId);
        TeamFoundationGitPullRequestService.VerifyReviewerIdentities(requestContext, reviewersToUpdate);
        TeamFoundationGitPullRequestService.VerifyReviewerIdentities(requestContext, reviewerIdsToDelete);
        ClientTraceData properties = new ClientTraceData();
        properties.Add("Action", (object) "UpdatePullRequest");
        properties.Add("PullRequestId", (object) pullRequestId);
        properties.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
        properties.Add("RepositoryName", (object) repository.Name);
        TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
        repository.Permissions.CheckPullRequestContribute();
        List<TfsGitPullRequest.ReviewerWithVote> combinedReviewerList = TeamFoundationGitPullRequestService.GetCombinedReviewerList(requestContext, reviewersToUpdate, reviewerIdsToDelete);
        IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers = pullRequestDetails.Reviewers;
        try
        {
          TeamFoundationGitPullRequestService.VerifyTotalCountOfReviewers(requestContext, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) combinedReviewerList, reviewers);
        }
        catch (GitPullRequestMaxReviewerCountException ex)
        {
          if (userInitiated)
            throw;
          else
            requestContext.TraceCatch(1013949, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", (Exception) ex);
        }
        TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
        ClientTraceService service1 = requestContext.GetService<ClientTraceService>();
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        ReviewersUpdateNotification updateNotification = new ReviewersUpdateNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, reviewers, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) combinedReviewerList, createDiscussionMessage, sendEmailNotification, notifyPolicies);
        try
        {
          service2.PublishDecisionPoint(requestContext, (object) updateNotification);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          properties.Add("PullRequestRejectedByPolicy", (object) true);
          service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
          throw new GitPullRequestUpdateRejectedByPolicyException((Exception) ex);
        }
        IEnumerable<Tuple<Guid, Guid>> groupMembershipTable = (IEnumerable<Tuple<Guid, Guid>>) TeamFoundationGitPullRequestService.CreateGroupMembershipTable(requestContext, reviewers, reviewersToUpdate, reviewerIdsToDelete);
        IEnumerable<TfsGitPullRequest.ReviewerWithVote> source;
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          source = gitCoreComponent.UpdatePullRequestReviewers(repository.Key, pullRequestId, reviewersToUpdate, reviewerIdsToDelete, groupMembershipTable);
        requestContext.Trace(1013329, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Publishing ReviewersUpdateNotification for pull request {0} (reviewer update)", (object) pullRequestId);
        service2.SyncPublishNotification(requestContext, (object) new SyncReviewersUpdateNotification(updateNotification));
        service2.PublishNotification(requestContext, (object) updateNotification);
        properties.Add("PullRequestReviewerCount", (object) source.Count<TfsGitPullRequest.ReviewerWithVote>());
        properties.Add("PullRequestVoteCount", (object) source.Count<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.Vote != (short) 0)));
        service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
        pullRequestDetails.UpdateReviewers(requestContext, repository.Key.ProjectId, reviewersToUpdate, reviewerIdsToDelete);
        return source;
      }
    }

    private static List<TfsGitPullRequest.ReviewerWithVote> GetCombinedReviewerList(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete)
    {
      HashSet<Guid> guidSet = new HashSet<Guid>();
      List<TfsGitPullRequest.ReviewerWithVote> combinedReviewerList = new List<TfsGitPullRequest.ReviewerWithVote>();
      foreach (TfsGitPullRequest.ReviewerBase other in reviewersToUpdate)
      {
        if (guidSet.Contains(other.Reviewer))
          throw new GitArgumentException(Resources.Format("ReviewerAlreadySeen", (object) other.Reviewer.ToString()));
        guidSet.Add(other.Reviewer);
        combinedReviewerList.Add(new TfsGitPullRequest.ReviewerWithVote(other));
      }
      foreach (Guid reviewer in reviewerIdsToDelete)
      {
        if (guidSet.Contains(reviewer))
          throw new GitArgumentException(Resources.Format("ReviewerAlreadySeen", (object) reviewer.ToString()));
        guidSet.Add(reviewer);
        combinedReviewerList.Add(new TfsGitPullRequest.ReviewerWithVote(reviewer, (short) 0, ReviewerVoteStatus.Removed));
      }
      return combinedReviewerList;
    }

    private static List<Tuple<Guid, Guid>> CreateGroupMembershipTable(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> existingReviewers,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewersToUpdate,
      IEnumerable<Guid> reviewerIdsToDelete)
    {
      HashSet<Guid> source = new HashSet<Guid>(existingReviewers.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (r => r.Reviewer)));
      source.UnionWith(reviewersToUpdate.Select<TfsGitPullRequest.ReviewerBase, Guid>((Func<TfsGitPullRequest.ReviewerBase, Guid>) (r => r.Reviewer)));
      source.ExceptWith(reviewerIdsToDelete);
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      IEnumerable<TeamFoundationIdentity> mappedIdentities = TeamFoundationGitPullRequestService.GetMappedIdentities(requestContext, source.ToArray<Guid>());
      List<TeamFoundationIdentity> list1 = mappedIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null && !id.IsContainer)).ToList<TeamFoundationIdentity>();
      List<TeamFoundationIdentity> list2 = mappedIdentities.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (id => id != null && id.IsContainer)).ToList<TeamFoundationIdentity>();
      List<Tuple<Guid, Guid>> groupMembershipTable = new List<Tuple<Guid, Guid>>();
      foreach (TfsGitPullRequest.ReviewerBase reviewerBase in reviewersToUpdate)
      {
        TfsGitPullRequest.ReviewerBase updatedReviewer = reviewerBase;
        TeamFoundationIdentity foundationIdentity1 = list2.FirstOrDefault<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (g => g.TeamFoundationId == updatedReviewer.Reviewer));
        if (foundationIdentity1 != null)
        {
          foreach (TeamFoundationIdentity foundationIdentity2 in list1)
          {
            if (service.IsMember(requestContext, foundationIdentity1.Descriptor, foundationIdentity2.Descriptor))
              groupMembershipTable.Add(new Tuple<Guid, Guid>(foundationIdentity2.TeamFoundationId, foundationIdentity1.TeamFoundationId));
          }
        }
      }
      return groupMembershipTable;
    }

    private static List<Guid> GetVotedForList(
      IVssRequestContext requestContext,
      TeamFoundationIdentity voterIdentity,
      IEnumerable<Guid> allReviewersIds)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      IEnumerable<TeamFoundationIdentity> foundationIdentities = TeamFoundationGitPullRequestService.GetMappedIdentities(requestContext, allReviewersIds.Distinct<Guid>().ToArray<Guid>()).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null));
      List<Guid> votedForList = new List<Guid>();
      foreach (TeamFoundationIdentity foundationIdentity in foundationIdentities)
      {
        if (service.IsMember(requestContext, foundationIdentity.Descriptor, voterIdentity.Descriptor))
          votedForList.Add(foundationIdentity.TeamFoundationId);
      }
      return votedForList;
    }

    private static IEnumerable<TeamFoundationIdentity> GetMappedIdentities(
      IVssRequestContext requestContext,
      Guid[] uniqueTeamFoundationIds)
    {
      TeamFoundationIdentity[] foundationIdentityArray = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, uniqueTeamFoundationIds);
      Dictionary<Guid, TeamFoundationIdentity> identitiesLookup = new Dictionary<Guid, TeamFoundationIdentity>();
      Dictionary<Guid, TeamFoundationIdentity> source = new Dictionary<Guid, TeamFoundationIdentity>();
      foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityArray)
      {
        if (foundationIdentity != null)
          identitiesLookup[foundationIdentity.TeamFoundationId] = foundationIdentity;
      }
      if (uniqueTeamFoundationIds.Length == foundationIdentityArray.Length)
      {
        for (int index = 0; index < uniqueTeamFoundationIds.Length; ++index)
        {
          Guid teamFoundationId = uniqueTeamFoundationIds[index];
          if (!identitiesLookup.ContainsKey(teamFoundationId))
          {
            identitiesLookup[teamFoundationId] = foundationIdentityArray[index];
            source[teamFoundationId] = foundationIdentityArray[index];
          }
        }
      }
      if (source.Any<KeyValuePair<Guid, TeamFoundationIdentity>>())
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("ReadIdentities returned different identities than requested (requested / returned):");
        foreach (KeyValuePair<Guid, TeamFoundationIdentity> keyValuePair in source)
        {
          stringBuilder.AppendFormat("{0} {1}", (object) keyValuePair.Key.ToString(), keyValuePair.Value == null ? (object) "null" : (object) keyValuePair.Value.TeamFoundationId.ToString());
          stringBuilder.AppendLine();
        }
        requestContext.Trace(1013333, TraceLevel.Error, TeamFoundationGitPullRequestService.TraceAreaAndLayer, TeamFoundationGitPullRequestService.TraceAreaAndLayer, stringBuilder.ToString());
      }
      return ((IEnumerable<Guid>) uniqueTeamFoundationIds).Select<Guid, TeamFoundationIdentity>((Func<Guid, TeamFoundationIdentity>) (id => identitiesLookup[id]));
    }

    public TfsGitPullRequest TryMerge(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      ClientTraceData properties = new ClientTraceData();
      try
      {
        properties.Add("Action", (object) nameof (TryMerge));
        properties.Add("PullRequestId", (object) pullRequestId);
        requestContext.Trace(1013214, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to merge pull request {0}", (object) pullRequestId);
        if (repository == null)
          throw new GitArgumentException(Resources.Get("InvalidRepository"));
        TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        if (pullRequestDetails.Status == PullRequestStatus.Completed)
          throw new GitPullRequestNotEditableException();
        repository.Permissions.CheckPullRequestContribute();
        TfsGitRef tfsGitRef1 = ITfsGitRepositoryExtensions.VerifyLocalRef(repository, pullRequestDetails.TargetBranchName);
        TfsGitRef tfsGitRef2 = TeamFoundationGitPullRequestService.VerifySourceRef(repository, pullRequestDetails);
        if (pullRequestDetails.MergeStatus == PullRequestAsyncStatus.Succeeded)
        {
          Sha1Id objectId1 = tfsGitRef2.ObjectId;
          Sha1Id? nullable = pullRequestDetails.LastMergeSourceCommit;
          if ((nullable.HasValue ? (objectId1 == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            Sha1Id objectId2 = tfsGitRef1.ObjectId;
            nullable = pullRequestDetails.LastMergeTargetCommit;
            if ((nullable.HasValue ? (objectId2 == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            {
              requestContext.Trace(1013215, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "No changes to the pull request");
              return pullRequestDetails;
            }
          }
        }
        requestContext.Trace(1013216, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to merge the pull request");
        return this.EnterMergeJob(requestContext, repository, pullRequestDetails.PullRequestId);
      }
      finally
      {
        try
        {
          requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
        }
        catch
        {
        }
      }
    }

    internal IEnumerable<TfsGitPullRequest> InternalUncheckedQueryPullRequestsSinceClosedTime(
      IVssRequestContext requestContext,
      DateTime? minClosedTime,
      PullRequestStatus statusFilter = PullRequestStatus.Active,
      int top = 1000,
      int skip = 0)
    {
      requestContext.Trace(1013244, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Query pull requests. top: {0}, skip {1}, minClosedTime {2}", (object) top, (object) skip, (object) (minClosedTime.HasValue ? minClosedTime.Value : DateTime.MinValue));
      Stopwatch stopwatch = Stopwatch.StartNew();
      IEnumerable<TfsGitPullRequest> tfsGitPullRequests;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        tfsGitPullRequests = gitCoreComponent.QueryGitPullRequests((string) null, new Guid?(), new Guid?(), (IEnumerable<string>) null, (IEnumerable<string>) null, new bool?(), new PullRequestStatus?(statusFilter), (IEnumerable<Guid>) null, (IEnumerable<Guid>) null, minClosedTime, new Guid?(), true, top, skip, false);
      this.AddToQueryPullRequestsPerformanceCounters("SinceClosedTime", stopwatch.ElapsedMilliseconds);
      return tfsGitPullRequests;
    }

    internal TfsGitPullRequest PerformMergeJob(
      IVssRequestContext requestContext,
      Guid mergeId,
      ClientTraceData ctData)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (PerformMergeJob)))
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        TfsGitPullRequest pullRequest;
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        {
          using (requestContext.TimeRegion("GitCoreComponent", "QueryGitPullRequests"))
            pullRequest = gitCoreComponent.QueryGitPullRequests((string) null, new Guid?(), new Guid?(), (IEnumerable<string>) null, (IEnumerable<string>) null, new bool?(), new PullRequestStatus?(PullRequestStatus.Active), (IEnumerable<Guid>) null, (IEnumerable<Guid>) null, new DateTime?(), new Guid?(mergeId), true, 1, 0, false).FirstOrDefault<TfsGitPullRequest>();
        }
        this.AddToQueryPullRequestsPerformanceCounters("MergeJob", stopwatch.ElapsedMilliseconds);
        if (pullRequest == null)
        {
          requestContext.Trace(1013241, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "No active pull request found with merge id {0}", (object) mergeId);
          return (TfsGitPullRequest) null;
        }
        ctData.Add("PullRequestId", (object) pullRequest.PullRequestId);
        ctData.Add("RepositoryId", (object) pullRequest.RepositoryId);
        ctData.Add("PullRequestTargetBranch", (object) pullRequest.TargetBranchName);
        if (pullRequest.BelongsToCompletionJob || pullRequest.Status != PullRequestStatus.Active)
        {
          TracepointUtils.Tracepoint(requestContext, 1013483, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<object>) (() => (object) pullRequest), TraceLevel.Error, caller: nameof (PerformMergeJob));
          return (TfsGitPullRequest) null;
        }
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, pullRequest.RepositoryId))
        {
          try
          {
            TfsGitPullRequest.CheckMergeStatus(this.CreateUpToDateMerge(requestContext, repositoryById.Key.ProjectId, repositoryById, pullRequest, out TfsGitRef _, out TfsGitRef _, out Sha1Id _, false, ctData));
          }
          catch (GitPullRequestCannotBeActivated ex)
          {
            IVssRequestContext requestContext1 = requestContext;
            Guid projectId = repositoryById.Key.ProjectId;
            TfsGitPullRequest pullRequest1 = pullRequest;
            PullRequestAsyncStatus? nullable1 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Failure);
            Sha1Id? nullable2 = new Sha1Id?(Sha1Id.Empty);
            PullRequestStatus? status = new PullRequestStatus?();
            PullRequestAsyncStatus? mergeStatus = nullable1;
            Sha1Id? lastMergeSourceCommit = new Sha1Id?();
            Sha1Id? lastMergeTargetCommit = new Sha1Id?();
            Sha1Id? lastMergeCommit = nullable2;
            Guid? completeWhenMergedAuthority = new Guid?();
            PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
            DateTime? completionQueueTime = new DateTime?();
            Guid? autoCompleteAuthority = new Guid?();
            bool? isDraft = new bool?();
            pullRequest = this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
          }
        }
        requestContext.TraceAlways(1013830, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestMergeQueueJob", string.Format("PullRequest: {0}/{1}/{2}/{3}", (object) pullRequest.RepositoryId, (object) pullRequest.TargetBranchName, (object) pullRequest.PullRequestId, (object) pullRequest.LastMergeSourceCommit));
        return pullRequest;
      }
    }

    internal TfsGitPullRequest EnterMergeJob(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      TfsGitPullRequest pullRequest1 = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
      requestContext.TraceAlways(1013825, TraceLevel.Verbose, GitServerUtils.TraceArea, "PullRequestMergeQueueJob", string.Format("PullRequest: {0}/{1}/{2}/{3}", (object) pullRequest1.RepositoryId, (object) pullRequest1.TargetBranchName, (object) pullRequest1.PullRequestId, (object) pullRequest1.LastMergeSourceCommit));
      if (pullRequest1.BelongsToCompletionJob)
        this.QueueCompletionJobToRun(requestContext, pullRequest1.RepositoryId, pullRequest1.TargetBranchName);
      else if (pullRequest1.BelongsToMergeJob)
      {
        this.QueueMergeJobToRun(requestContext, pullRequest1);
      }
      else
      {
        if (!pullRequest1.BelongsToNoJob)
          throw new GitPullRequestNotEditableException();
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId = repository.Key.ProjectId;
        TfsGitPullRequest pullRequest2 = pullRequest1;
        Guid? nullable1 = new Guid?(Guid.Empty);
        PullRequestAsyncStatus? nullable2 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Queued);
        Sha1Id? nullable3 = new Sha1Id?(Sha1Id.Empty);
        DateTime? nullable4 = new DateTime?(new DateTime());
        PullRequestStatus? status = new PullRequestStatus?();
        PullRequestAsyncStatus? mergeStatus = nullable2;
        Sha1Id? lastMergeSourceCommit = new Sha1Id?();
        Sha1Id? lastMergeTargetCommit = new Sha1Id?();
        Sha1Id? lastMergeCommit = nullable3;
        Guid? completeWhenMergedAuthority = nullable1;
        PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
        DateTime? completionQueueTime = nullable4;
        Guid? autoCompleteAuthority = new Guid?();
        bool? isDraft = new bool?();
        pullRequest1 = this.UpdatePullRequestInDatabase(requestContext1, projectId, pullRequest2, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
        this.QueueMergeJobToRun(requestContext, pullRequest1);
      }
      return pullRequest1;
    }

    internal virtual void QueueMergeJobToRun(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (QueueMergeJobToRun)))
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, pullRequest.MergeId);
        string name = string.Format("PR Merge repo={0} pr={1}", (object) pullRequest.RepositoryId.ToString("N"), (object) pullRequest.PullRequestId);
        if (foundationJobDefinition == null || foundationJobDefinition.PriorityClass != JobPriorityClass.High || foundationJobDefinition.Name != name)
        {
          XmlNode mergeJobDataXml = TeamFoundationGitPullRequestService.CreateMergeJobDataXml(Guid.NewGuid());
          foundationJobDefinition = new TeamFoundationJobDefinition(pullRequest.MergeId, name, "Microsoft.TeamFoundation.Git.Server.Plugins.GitNativeMergeJob", mergeJobDataXml, TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.High);
          service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
          {
            foundationJobDefinition
          });
        }
        service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
        {
          foundationJobDefinition.ToJobReference()
        });
      }
    }

    public static XmlNode CreateMergeJobDataXml(Guid orchestrationId) => orchestrationId == new Guid() ? (XmlNode) null : TeamFoundationSerializationUtility.SerializeToXml((object) orchestrationId);

    public static Guid ParseMergeJobDataXml(XmlNode xmlData) => xmlData == null ? new Guid() : TeamFoundationSerializationUtility.Deserialize<Guid>(xmlData);

    private TfsGitPullRequest CreateUpToDateMerge(
      IVssRequestContext requestContext,
      Guid projectId,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      out TfsGitRef sourceRef,
      out TfsGitRef targetRef,
      out Sha1Id rebasedSourceCommitId,
      bool forCompletion,
      ClientTraceData ctData)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (CreateUpToDateMerge)))
      {
        Sha1Id? nullable1 = pullRequest.LastMergeCommit;
        Sha1Id mergeCommitId = nullable1 ?? Sha1Id.Empty;
        Sha1Id conflictResolutionHash = Sha1Id.Empty;
        Guid mergeId = pullRequest.MergeId;
        targetRef = ITfsGitRepositoryExtensions.VerifyLocalRef(repository, pullRequest.TargetBranchName);
        sourceRef = TeamFoundationGitPullRequestService.VerifySourceRef(repository, pullRequest);
        rebasedSourceCommitId = Sha1Id.Empty;
        if (GitServerUtils.IsConnected(requestContext, repository, (IEnumerable<Sha1Id>) new Sha1Id[1]
        {
          sourceRef.ObjectId
        }, targetRef.ObjectId))
        {
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId1 = projectId;
          TfsGitPullRequest pullRequest1 = pullRequest;
          PullRequestAsyncStatus? nullable2 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Succeeded);
          nullable1 = new Sha1Id?(sourceRef.ObjectId);
          Sha1Id? nullable3 = new Sha1Id?(targetRef.ObjectId);
          Sha1Id? nullable4 = new Sha1Id?(targetRef.ObjectId);
          PullRequestStatus? status = new PullRequestStatus?();
          PullRequestAsyncStatus? mergeStatus = nullable2;
          Sha1Id? lastMergeSourceCommit = nullable1;
          Sha1Id? lastMergeTargetCommit = nullable3;
          Sha1Id? lastMergeCommit = nullable4;
          Guid? completeWhenMergedAuthority = new Guid?();
          PullRequestMergeFailureType? mergeFailureType = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime = new DateTime?();
          Guid? autoCompleteAuthority = new Guid?();
          bool? isDraft = new bool?();
          pullRequest = this.UpdatePullRequestInDatabase(requestContext1, projectId1, pullRequest1, status, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, mergeFailureType: mergeFailureType, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, isDraft: isDraft);
          ctData.Add("PullRequestMergedExternally", (object) true);
          ctData.Add("PullRequestMergeStatus", (object) PullRequestAsyncStatus.Succeeded);
          return pullRequest;
        }
        ctData.Add("PullRequestMergedExternally", (object) false);
        IVssRequestContext requestContext2 = requestContext;
        Guid projectId2 = projectId;
        TfsGitPullRequest pullRequest2 = pullRequest;
        PullRequestAsyncStatus? nullable5 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Queued);
        PullRequestStatus? status1 = new PullRequestStatus?();
        PullRequestAsyncStatus? mergeStatus1 = nullable5;
        Sha1Id? lastMergeSourceCommit1 = new Sha1Id?();
        Sha1Id? lastMergeTargetCommit1 = new Sha1Id?();
        Sha1Id? lastMergeCommit1 = new Sha1Id?();
        Guid? completeWhenMergedAuthority1 = new Guid?();
        PullRequestMergeFailureType? mergeFailureType1 = new PullRequestMergeFailureType?();
        DateTime? completionQueueTime1 = new DateTime?();
        Guid? autoCompleteAuthority1 = new Guid?();
        bool? isDraft1 = new bool?();
        pullRequest = this.UpdatePullRequestInDatabase(requestContext2, projectId2, pullRequest2, status1, mergeStatus1, lastMergeSourceCommit1, lastMergeTargetCommit1, lastMergeCommit1, completeWhenMergedAuthority1, mergeFailureType: mergeFailureType1, completionQueueTime: completionQueueTime1, autoCompleteAuthority: autoCompleteAuthority1, isDraft: isDraft1);
        IdentityDescriptor lastConflictResolver = (IdentityDescriptor) null;
        ConflictResolutionIterationParams conflictResolution = (ConflictResolutionIterationParams) null;
        PullRequestAsyncStatus mergeStatus2;
        try
        {
          bool isFastForwardable = false;
          int num1;
          if (forCompletion)
          {
            GitPullRequestCompletionOptions completionOptions1 = pullRequest.CompletionOptions;
            GitPullRequestMergeStrategy? mergeStrategy;
            int num2;
            if (completionOptions1 == null)
            {
              num2 = 0;
            }
            else
            {
              mergeStrategy = completionOptions1.MergeStrategy;
              GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.Rebase;
              num2 = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
            }
            if (num2 == 0)
            {
              GitPullRequestCompletionOptions completionOptions2 = pullRequest.CompletionOptions;
              if (completionOptions2 == null)
              {
                num1 = 0;
              }
              else
              {
                mergeStrategy = completionOptions2.MergeStrategy;
                GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.RebaseMerge;
                num1 = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue ? 1 : 0;
              }
            }
            else
              num1 = 1;
          }
          else
            num1 = 0;
          bool flag = num1 != 0;
          if (flag)
            isFastForwardable = GitServerUtils.IsConnected(requestContext, repository, (IEnumerable<Sha1Id>) new Sha1Id[1]
            {
              targetRef.ObjectId
            }, sourceRef.ObjectId);
          pullRequest = !flag ? this.MergePullRequestAndUpdateConflicts(requestContext, repository, pullRequest, sourceRef.ObjectId, targetRef.ObjectId, forCompletion, ctData, out mergeCommitId, out conflictResolutionHash, out lastConflictResolver) : this.RebasePullRequestAndUpdateConflicts(requestContext, repository, pullRequest, sourceRef.ObjectId, targetRef.ObjectId, forCompletion, isFastForwardable, ctData, out mergeCommitId, out rebasedSourceCommitId, out conflictResolutionHash, out lastConflictResolver);
          conflictResolution = new ConflictResolutionIterationParams(conflictResolutionHash);
          mergeStatus2 = pullRequest.MergeStatus;
          if (mergeStatus2 == PullRequestAsyncStatus.Succeeded)
          {
            if (conflictResolutionHash != Sha1Id.Empty)
              pullRequest.CreateNewIteration(requestContext, repository, lastConflictResolver, false, conflictResolution: conflictResolution);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013046, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
          IVssRequestContext requestContext3 = requestContext;
          Guid projectId3 = projectId;
          TfsGitPullRequest pullRequest3 = pullRequest;
          PullRequestAsyncStatus? nullable6 = new PullRequestAsyncStatus?(PullRequestAsyncStatus.Failure);
          Sha1Id? nullable7 = new Sha1Id?(sourceRef.ObjectId);
          Sha1Id? nullable8 = new Sha1Id?(targetRef.ObjectId);
          Sha1Id? nullable9 = new Sha1Id?(Sha1Id.Empty);
          PullRequestStatus? status2 = new PullRequestStatus?();
          PullRequestAsyncStatus? mergeStatus3 = nullable6;
          Sha1Id? lastMergeSourceCommit2 = nullable7;
          Sha1Id? lastMergeTargetCommit2 = nullable8;
          Sha1Id? lastMergeCommit2 = nullable9;
          Guid? completeWhenMergedAuthority2 = new Guid?();
          PullRequestMergeFailureType? mergeFailureType2 = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime2 = new DateTime?();
          Guid? autoCompleteAuthority2 = new Guid?();
          bool? isDraft2 = new bool?();
          pullRequest = this.UpdatePullRequestInDatabase(requestContext3, projectId3, pullRequest3, status2, mergeStatus3, lastMergeSourceCommit2, lastMergeTargetCommit2, lastMergeCommit2, completeWhenMergedAuthority2, mergeFailureType: mergeFailureType2, mergeFailureMessage: "Unknown error", completionQueueTime: completionQueueTime2, autoCompleteAuthority: autoCompleteAuthority2, isDraft: isDraft2);
          mergeStatus2 = PullRequestAsyncStatus.Failure;
          mergeCommitId = Sha1Id.Empty;
        }
        MergeCompletedNotification completedNotification = this.SendMergeCompletedNotification(requestContext, repository, pullRequest.PullRequestId, new Sha1Id?(targetRef.ObjectId), new Sha1Id?(sourceRef.ObjectId), pullRequest.SourceBranchName, pullRequest.TargetBranchName, new Sha1Id?(mergeCommitId), mergeStatus2, conflictResolutionHash, lastConflictResolver, forCompletion, conflictResolution != null && conflictResolution.IsCreated, ctData, pullRequest.CompleteWhenMergedAuthority);
        ctData.Add("NotificationType", (object) "MergeCompletedNotification");
        ctData.Add("GitNotificationGuid", (object) completedNotification.GitNotificationGuid);
        ctData.Add("NotificationPublishedToken", (object) mergeCommitId.ToString());
        ctData.Add("ConflictResolutionHash", (object) conflictResolutionHash.ToString());
        ctData.Add("NotificationPublishedTime", (object) DateTime.UtcNow.ToString("O"));
        nullable1 = pullRequest.LastMergeSourceCommit;
        Sha1Id objectId1 = sourceRef.ObjectId;
        if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != objectId1 ? 1 : 0) : 0) : 1) == 0)
        {
          nullable1 = pullRequest.LastMergeTargetCommit;
          Sha1Id objectId2 = targetRef.ObjectId;
          if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != objectId2 ? 1 : 0) : 0) : 1) == 0)
          {
            nullable1 = pullRequest.LastMergeCommit;
            Sha1Id sha1Id = mergeCommitId;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != sha1Id ? 1 : 0) : 0) : 1) == 0 && pullRequest.MergeStatus == mergeStatus2)
              goto label_24;
          }
        }
        requestContext.Trace(1013393, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull Request: UpdatePullRequest() returned, but updated fields have unexpected values.");
label_24:
        ctData.GetData()["PullRequestMergeStatus"] = (object) mergeStatus2;
        return pullRequest;
      }
    }

    internal void LinkWorkItemsToCommit(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      IEnumerable<int> workItemIds)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (LinkWorkItemsToCommit)))
      {
        Sha1Id? lastMergeCommit = pullRequest.LastMergeCommit;
        if (!lastMergeCommit.HasValue)
          return;
        Sha1Id? nullable = lastMergeCommit;
        Sha1Id empty = Sha1Id.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0 || workItemIds == null || workItemIds.Count<int>() <= 0)
          return;
        string artifactUri = LinkingUtilities.EncodeUri(GitCommitArtifactId.GetArtifactIdForCommit(repository.Key, lastMergeCommit.Value));
        this.LinkWorkItems(requestContext, artifactUri, workItemIds, ArtifactLinkIds.Commit);
      }
    }

    internal MergeCompletedNotification SendMergeCompletedNotification(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      Sha1Id? targetRefHead,
      Sha1Id? sourceCommit,
      string sourceRefName,
      string targetRefName,
      Sha1Id? mergeCommit,
      PullRequestAsyncStatus mergeStatus,
      Sha1Id conflictResolutionHash,
      IdentityDescriptor lastConflictResolver,
      bool unattendedCompletion,
      bool isConflictResolution,
      ClientTraceData ctData,
      Guid completedMergeRequestActor)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (SendMergeCompletedNotification)))
      {
        MergeCompletedNotification notification = new MergeCompletedNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, targetRefHead, sourceCommit, sourceRefName, targetRefName, mergeCommit, mergeStatus, conflictResolutionHash, lastConflictResolver, unattendedCompletion, isConflictResolution, completedMergeRequestActor);
        this.SyncPublishNotification(requestContext, (PullRequestNotification) notification, 1013237, ctData, "MergeCompletedSyncPublishElapsedMs", (object) new
        {
          sourceCommit = sourceCommit,
          targetRefName = targetRefName,
          targetRefHead = targetRefHead,
          mergeCommit = mergeCommit
        });
        return notification;
      }
    }

    public TfsGitPullRequest CreatePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string title,
      string description,
      string sourceBranchName,
      string targetBranchName,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers,
      bool tryMerge,
      bool supportsIterations = true,
      GitPullRequestMergeOptions mergeOptions = null,
      IEnumerable<int> workItemIds = null,
      bool linkBranchWorkItems = false,
      bool linkCommitWorkItems = false,
      GitRepositoryRef sourceForkRepositoryRef = null,
      WebApiTagDefinition[] labels = null,
      bool isDraft = false)
    {
      Guid orchestrationId = Guid.NewGuid();
      using (ITimedOrchestrationRegion orchestrationRegion = requestContext.TimeOrchestration("TfsGitPullRequest", orchestrationId, -1L, "PullRequestCreate", regionName: nameof (CreatePullRequest)))
      {
        try
        {
          repository.Permissions.CheckPullRequestContribute();
          ClientTraceData clientTraceData1 = new ClientTraceData();
          clientTraceData1.Add("Action", (object) nameof (CreatePullRequest));
          ClientTraceData clientTraceData2 = clientTraceData1;
          Guid guid = repository.Key.RepoId;
          string str = guid.ToString();
          clientTraceData2.Add("RepositoryId", (object) str);
          clientTraceData1.Add("RepositoryName", (object) repository.Name);
          Guid mergeId1 = Guid.NewGuid();
          TeamFoundationGitPullRequestService.VerifyReviewerIdentities(requestContext, reviewers);
          IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
          int maxCount = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestMaxReviewersCount", true, 1000);
          if (reviewers.Count<TfsGitPullRequest.ReviewerBase>() > maxCount)
            throw new GitPullRequestMaxReviewerCountException(maxCount);
          TfsGitPullRequest pullRequest = (TfsGitPullRequest) null;
          TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
          using (ITfsGitRepository tfsGitRepository = this.LoadSourceRepository(requestContext, sourceForkRepositoryRef?.Id))
          {
            if (tfsGitRepository != null)
            {
              clientTraceData1.Add("SourceRepositoryName", (object) tfsGitRepository.Name);
              clientTraceData1.Add("SourceRepositoryId", (object) tfsGitRepository.Key.RepoId);
            }
            TfsGitRef tfsGitRef1 = ITfsGitRepositoryExtensions.VerifyLocalRef(tfsGitRepository ?? repository, sourceBranchName);
            TfsGitRef tfsGitRef2 = ITfsGitRepositoryExtensions.VerifyLocalRef(repository, targetBranchName);
            if (requestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.Labels") && labels != null && labels.Length != 0)
              requestContext.GetService<ITeamFoundationGitPullRequestService>().VerifyLabelsForPullRequests(requestContext, ((IEnumerable<WebApiTagDefinition>) labels).Where<WebApiTagDefinition>((Func<WebApiTagDefinition, bool>) (label => label != null && !string.IsNullOrEmpty(label.Name))).Select<WebApiTagDefinition, string>((Func<WebApiTagDefinition, string>) (label => label.Name)).ToArray<string>(), repository);
            int? pullRequestId1 = new int?();
            if (tfsGitRepository != null)
            {
              ITeamFoundationCounterService service2 = requestContext.GetService<ITeamFoundationCounterService>();
              ref int? local = ref pullRequestId1;
              ITeamFoundationCounterService foundationCounterService = service2;
              IVssRequestContext requestContext1 = requestContext;
              guid = new Guid();
              Guid dataSpaceIdentifier = guid;
              int num = (int) foundationCounterService.ReserveCounterIds(requestContext1, "codeReviewId", 1L, dataSpaceIdentifier: dataSpaceIdentifier);
              local = new int?(num);
              ITfsGitRepositoryExtensions.SyncForkMirrorRef(requestContext, repository, pullRequestId1.Value, tfsGitRef1.ObjectId, clientTraceData1);
            }
            List<TfsGitPullRequest.ReviewerWithVote> reviewerWithVoteList = new List<TfsGitPullRequest.ReviewerWithVote>();
            foreach (TfsGitPullRequest.ReviewerBase reviewer in reviewers)
              reviewerWithVoteList.Add(new TfsGitPullRequest.ReviewerWithVote(reviewer));
            Guid repoId1 = repository.Key.RepoId;
            int pullRequestId2 = pullRequestId1 ?? -1;
            Guid teamFoundationId = foundationIdentity.TeamFoundationId;
            DateTime creationDate = new DateTime();
            DateTime closedDate = new DateTime();
            string title1 = title;
            string description1 = description;
            string sourceBranchName1 = sourceBranchName;
            string targetBranchName1 = targetBranchName;
            int mergeStatus = tryMerge ? 1 : 0;
            Guid mergeId2 = mergeId1;
            Sha1Id? lastMergeSourceCommit = tryMerge ? new Sha1Id?(tfsGitRef1.ObjectId) : new Sha1Id?();
            Sha1Id? lastMergeTargetCommit = tryMerge ? new Sha1Id?(tfsGitRef2.ObjectId) : new Sha1Id?();
            Sha1Id? lastMergeCommit = new Sha1Id?();
            Guid empty = Guid.Empty;
            List<TfsGitPullRequest.ReviewerWithVote> reviewers1 = reviewerWithVoteList;
            GitPullRequestMergeOptions mergeOptions1 = mergeOptions;
            Guid? repoId2 = tfsGitRepository?.Key?.RepoId;
            bool flag = isDraft;
            DateTime completionQueueTime = new DateTime();
            Guid autoCompleteAuthority = new Guid();
            Guid? sourceRepositoryId = repoId2;
            int num1 = flag ? 1 : 0;
            DateTime updatedTime = new DateTime();
            Iteration firstIterationObject = new TfsGitPullRequest(repoId1, pullRequestId2, PullRequestStatus.Active, teamFoundationId, creationDate, closedDate, title1, description1, sourceBranchName1, targetBranchName1, (PullRequestAsyncStatus) mergeStatus, mergeId2, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, empty, (IEnumerable<TfsGitPullRequest.ReviewerWithVote>) reviewers1, mergeOptions: mergeOptions1, completionQueueTime: completionQueueTime, autoCompleteAuthority: autoCompleteAuthority, sourceRepositoryId: sourceRepositoryId, isDraft: num1 != 0, updatedTime: updatedTime).CreateFirstIterationObject(requestContext, repository, supportsIterations);
            int iterationTimeoutSeconds = this.CreatePullRequestWithIterationTimeoutSeconds(requestContext);
            using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
            {
              gitCoreComponent.CommandTimeout = iterationTimeoutSeconds;
              pullRequest = gitCoreComponent.CreatePullRequestWithIteration(repository.Key, foundationIdentity.TeamFoundationId, title, description, sourceBranchName, targetBranchName, tryMerge ? PullRequestAsyncStatus.Queued : PullRequestAsyncStatus.NotSet, mergeId1, tryMerge ? new Sha1Id?(tfsGitRef1.ObjectId) : new Sha1Id?(), tryMerge ? new Sha1Id?(tfsGitRef2.ObjectId) : new Sha1Id?(), new Sha1Id?(), false, mergeOptions, reviewers, firstIterationObject, pullRequestId1, tfsGitRepository?.Key?.RepoId, isDraft);
            }
            try
            {
              if (firstIterationObject != null)
                ArtifactPropertyKinds.SaveIterationProperties(requestContext, firstIterationObject.Properties, repository.Key.ProjectId, pullRequest.CodeReviewId, 1);
              pullRequest.SaveReviewProperties(requestContext, repository.Key.ProjectId);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1013425, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
            }
          }
          using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
            pullRequest = gitCoreComponent.GetPullRequestDetails(repository.Key.RepoId, pullRequest.PullRequestId);
          requestContext.Trace(1013213, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Created pull request {0} with mergeId {1} and codeReview.Id {2}", (object) pullRequest.PullRequestId, (object) pullRequest.MergeId, (object) pullRequest.CodeReviewId);
          this.LinkWorkItemsToPR(requestContext, repository, pullRequest, workItemIds, linkBranchWorkItems, linkCommitWorkItems);
          if (requestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.Labels") && labels != null && labels.Length != 0)
          {
            ITeamFoundationGitPullRequestService service3 = requestContext.GetService<ITeamFoundationGitPullRequestService>();
            try
            {
              service3.AddLabelsToPullRequest(requestContext, ((IEnumerable<WebApiTagDefinition>) labels).Where<WebApiTagDefinition>((Func<WebApiTagDefinition, bool>) (label => label != null && !string.IsNullOrEmpty(label.Name))).Select<WebApiTagDefinition, string>((Func<WebApiTagDefinition, string>) (label => label.Name)).ToArray<string>(), repository, pullRequest, clientTraceData1);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1013761, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
            }
          }
          int num2 = service1.GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequest/MaxFilesSyncPrCreatePolicyNotification", true, 1000);
          bool notifyPoliciesAsync = pullRequest.GetChangesetForPolicyApplicability(requestContext).Count > num2;
          ITeamFoundationEventService service4 = requestContext.GetService<ITeamFoundationEventService>();
          PullRequestCreatedNotification notification = this.ConstructCreatedNotification(repository, foundationIdentity, pullRequest, notifyPoliciesAsync);
          this.SyncPublishNotification(requestContext, (PullRequestNotification) new SyncPullRequestCreatedNotification(notification), 1013789, clientTraceData1, "PullRequestCreatedSyncPublishElapsedMs", eventManager: service4);
          if (tryMerge)
            this.QueueMergeJobToRun(requestContext, pullRequest);
          using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
            pullRequest = gitCoreComponent.GetPullRequestDetails(repository.Key.RepoId, pullRequest.PullRequestId);
          PullRequestCreatedNotification notificationEvent = this.ConstructCreatedNotification(repository, foundationIdentity, pullRequest, notifyPoliciesAsync);
          service4.PublishNotification(requestContext, (object) notificationEvent);
          clientTraceData1.Add("PullRequestId", (object) pullRequest.PullRequestId);
          IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewers2 = pullRequest.Reviewers;
          if ((reviewers2 != null ? (reviewers2.Count<TfsGitPullRequest.ReviewerWithVote>() > 0 ? 1 : 0) : 0) != 0)
            clientTraceData1.Add("PullRequestReviewerCount", (object) pullRequest.Reviewers.Count<TfsGitPullRequest.ReviewerWithVote>());
          clientTraceData1.Add("PullRequestDescriptionLength", (object) pullRequest.Description?.Length);
          clientTraceData1.Add("PullRequestSupportsIterations", (object) pullRequest.SupportsIterations);
          requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", clientTraceData1);
          return pullRequest;
        }
        catch (Exception ex) when (orchestrationRegion.SetOrchestrationException(ex))
        {
          throw;
        }
      }
    }

    private int CreatePullRequestWithIterationTimeoutSeconds(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/CreatePullRequestWithIterationTimeoutSeconds", true, InternalDatabaseProperties.DefaultDatabaseRequestTimeout);

    private PullRequestCreatedNotification ConstructCreatedNotification(
      ITfsGitRepository repository,
      TeamFoundationIdentity identity,
      TfsGitPullRequest createdPullRequest,
      bool notifyPoliciesAsync)
    {
      return new PullRequestCreatedNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, createdPullRequest.PullRequestId, identity, createdPullRequest.Title, createdPullRequest.Description, createdPullRequest.CreationDate, createdPullRequest.SourceBranchName, createdPullRequest.TargetBranchName, (IEnumerable<TfsGitPullRequest.ReviewerBase>) createdPullRequest.Reviewers, notifyPoliciesAsync);
    }

    internal virtual void LinkWorkItemsToPR(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      IEnumerable<int> workItemIds,
      bool linkBranchWorkItems,
      bool linkCommitWorkItems)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (LinkWorkItemsToPR)))
      {
        if (linkBranchWorkItems | linkCommitWorkItems)
        {
          if (workItemIds == null)
            workItemIds = (IEnumerable<int>) Array.Empty<int>();
          workItemIds = workItemIds.Union<int>(pullRequest.GetBranchAndCommitsAssociatedWorkItems(requestContext, repository, linkBranchWorkItems, linkCommitWorkItems)).Distinct<int>();
        }
        if (workItemIds == null || workItemIds.Count<int>() <= 0)
          return;
        string artifactUri = LinkingUtilities.EncodeUri(PullRequestArtifactId.GetArtifactIdForPullRequest(repository.Key.GetProjectUri(), repository.Key.RepoId, pullRequest.PullRequestId));
        this.LinkWorkItems(requestContext, artifactUri, workItemIds, ArtifactLinkIds.PullRequest);
      }
    }

    internal virtual void LinkWorkItems(
      IVssRequestContext requestContext,
      string artifactUri,
      IEnumerable<int> workItemIds,
      string linkType,
      bool retryBatchFailed = true)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (LinkWorkItems)))
      {
        if (workItemIds == null || workItemIds.Count<int>() <= 0)
          return;
        ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
        IEnumerable<WorkItemUpdate> workItemUpdates = workItemIds.Select<int, WorkItemUpdate>((Func<int, WorkItemUpdate>) (workItemId =>
        {
          WorkItemUpdate workItemUpdate1 = new WorkItemUpdate();
          workItemUpdate1.Id = workItemId;
          WorkItemUpdate workItemUpdate2 = workItemUpdate1;
          WorkItemResourceLinkUpdate[] resourceLinkUpdateArray = new WorkItemResourceLinkUpdate[1]
          {
            new WorkItemResourceLinkUpdate()
            {
              UpdateType = LinkUpdateType.Add,
              Type = new ResourceLinkType?(ResourceLinkType.ArtifactLink),
              Location = artifactUri,
              Name = linkType
            }
          };
          workItemUpdate2.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) resourceLinkUpdateArray;
          return workItemUpdate1;
        }));
        if (!workItemUpdates.Any<WorkItemUpdate>())
          return;
        try
        {
          List<int> intList = new List<int>();
          foreach (WorkItemUpdateResult itemUpdateResult in service.UpdateWorkItems(requestContext, workItemUpdates, includeInRecentActivity: true, checkRevisionsLimit: true).Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (ur => ur.Exception != null)))
          {
            if (retryBatchFailed && itemUpdateResult.Exception is WorkItemsBatchSaveFailedException)
              intList.Add(itemUpdateResult.Id);
            else
              requestContext.Trace(1013836, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", new StringBuilder().AppendLine("Failed to link work item of type '" + linkType + "'.").AppendLine("ArtifactUri: '" + artifactUri + "'").AppendLine(string.Format("UpdateResult.Id: '{0}'", (object) itemUpdateResult.Id)).AppendLine(string.Format("UpdateResult.Rev: '{0}'", (object) itemUpdateResult.Rev)).AppendLine(string.Format("Exception: '{0}'", (object) itemUpdateResult.Exception)).ToString());
          }
          if (!intList.Any<int>())
            return;
          this.LinkWorkItems(requestContext, artifactUri, (IEnumerable<int>) intList, linkType, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013560, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
        }
      }
    }

    internal virtual void TransitionWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string commentOnSave = null,
      IDictionary<int, string> workItemIdToStateMap = null)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (TransitionWorkItems)))
      {
        if ((workItemIds == null || workItemIds.Count<int>() <= 0) && (workItemIdToStateMap == null || workItemIdToStateMap.Count<KeyValuePair<int, string>>() <= 0))
          return;
        ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
        try
        {
          foreach (WorkItemUpdateResult itemUpdateResult in (workItemIdToStateMap == null || workItemIdToStateMap.Count<KeyValuePair<int, string>>() <= 0 ? service.UpdateWorkItemsStateOnCheckin(requestContext, workItemIds, commentOnSave) : service.UpdateWorkItemsStateOnCheckin(requestContext, workItemIdToStateMap, commentOnSave)).Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (ur => ur.Exception != null)))
            requestContext.Trace(1013838, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", new StringBuilder().AppendLine("Failed to transition work item to the next logical state.").AppendLine(string.Format("UpdateResult.Id: '{0}'", (object) itemUpdateResult.Id)).AppendLine(string.Format("UpdateResult.Rev: '{0}'", (object) itemUpdateResult.Rev)).AppendLine(string.Format("Exception: '{0}'", (object) itemUpdateResult.Exception)).ToString());
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013498, TraceLevel.Error, GitServerUtils.TraceArea, "TfsGitPullRequest", ex);
        }
      }
    }

    private ITfsGitRepository LoadSourceRepository(
      IVssRequestContext requestContext,
      Guid? sourceRepositoryId)
    {
      return !sourceRepositoryId.HasValue ? (ITfsGitRepository) null : GitServerUtils.FindRepositoryByFilters(requestContext, sourceRepositoryId.ToString());
    }

    public TfsGitPullRequest UpdatePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      PullRequestStatus status,
      string title,
      string description,
      GitPullRequestCompletionOptions completionOptions,
      GitPullRequestMergeOptions mergeOptions,
      Guid? autoCompleteAuthority,
      bool? isDraft)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (UpdatePullRequest)))
      {
        requestContext.Trace(1013228, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Trying to update pull request {0} with status {1}", (object) pullRequestId, (object) status);
        List<PullRequestNotification> requestNotificationList = new List<PullRequestNotification>();
        TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
        ITeamFoundationEventService service1 = requestContext.GetService<ITeamFoundationEventService>();
        ClientTraceData clientTraceData = new ClientTraceData();
        clientTraceData.Add("Action", (object) nameof (UpdatePullRequest));
        clientTraceData.Add("PullRequestId", (object) pullRequestId);
        clientTraceData.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
        clientTraceData.Add("RepositoryName", (object) repository.Name);
        ClientTraceService service2 = requestContext.GetService<ClientTraceService>();
        if (status == PullRequestStatus.Completed)
          throw new GitArgumentException(Resources.Format("StatusNotValid", (object) status));
        if (title == "")
          throw new GitArgumentException("InvalidPullRequestTitle");
        TfsGitPullRequest pullRequestDetails = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        repository.Permissions.CheckPullRequestContribute();
        if (pullRequestDetails.BelongsToCompletionJob && status != PullRequestStatus.Abandoned)
          throw new GitPullRequestNotEditableException();
        if (status == PullRequestStatus.Abandoned || status == PullRequestStatus.Active)
        {
          clientTraceData.Add("PullRequestStateTransition", (object) status);
          StatusUpdateNotification updateNotification = TeamFoundationGitPullRequestService.CreateStatusUpdateNotification(requestContext, repository, pullRequestId, status, foundationIdentity);
          try
          {
            service1.PublishDecisionPoint(requestContext, (object) updateNotification);
          }
          catch (ActionDeniedBySubscriberException ex)
          {
            clientTraceData.Add("PullRequestRejectedByPolicy", (object) true);
            service2.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", clientTraceData);
            throw new GitPullRequestUpdateRejectedByPolicyException((Exception) ex);
          }
          requestNotificationList.Add((PullRequestNotification) updateNotification);
        }
        bool? nullable1;
        if (status != PullRequestStatus.Abandoned)
        {
          nullable1 = isDraft;
          bool flag = true;
          if (!(nullable1.GetValueOrDefault() == flag & nullable1.HasValue))
            goto label_15;
        }
        autoCompleteAuthority = new Guid?(Guid.Empty);
label_15:
        if (title != null || description != null || autoCompleteAuthority.HasValue)
          TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(pullRequestDetails);
        if (status == PullRequestStatus.Active && pullRequestDetails.IsFromFork)
        {
          using (ITfsGitRepository repo = this.LoadSourceRepository(requestContext, new Guid?(pullRequestDetails.ForkSource.RepositoryId)))
          {
            TfsGitRef tfsGitRef = ITfsGitRepositoryExtensions.VerifyLocalRef(repo, pullRequestDetails.ForkSource.RefName);
            ITfsGitRepositoryExtensions.SyncForkMirrorRef(requestContext, repository, pullRequestDetails.PullRequestId, tfsGitRef.ObjectId, clientTraceData);
          }
        }
        if ((title == null || title == pullRequestDetails.Title) && pullRequestDetails.IsDraft)
        {
          nullable1 = isDraft;
          bool flag = false;
          if (nullable1.GetValueOrDefault() == flag & nullable1.HasValue)
            title = new Regex("^(\\[WIP\\]|WIP\\s)\\s*", RegexOptions.IgnoreCase).Replace(pullRequestDetails.Title, "");
        }
        if (title != null && title != pullRequestDetails.Title || description != null && description != pullRequestDetails.Description)
          requestNotificationList.Add((PullRequestNotification) new TitleDescriptionUpdatedNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, pullRequestDetails.Title, title, pullRequestDetails.Description, description));
        Guid? nullable2;
        if (autoCompleteAuthority.HasValue)
        {
          nullable2 = autoCompleteAuthority;
          Guid completeAuthority = pullRequestDetails.AutoCompleteAuthority;
          if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != completeAuthority ? 1 : 0) : 0) : 1) != 0)
          {
            SecurityHelper.Instance.CheckWritePermission(requestContext, (RepoScope) repository.Key, pullRequestDetails.TargetBranchName);
            TeamFoundationIdentity autoCompleteAuthority1 = (TeamFoundationIdentity) null;
            nullable2 = autoCompleteAuthority;
            Guid empty = Guid.Empty;
            if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            {
              ITeamFoundationIdentityService service3 = requestContext.GetService<ITeamFoundationIdentityService>();
              autoCompleteAuthority1 = IdentityHelper.Instance.GetUserIdentity(requestContext, service3, autoCompleteAuthority.Value);
              string projectUri = repository.Key.GetProjectUri();
              ArtifactId artifactId1 = pullRequestDetails.BuildLegacyArtifactId(projectUri);
              GitPullRequestTarget pullRequestTarget = new GitPullRequestTarget(projectUri, pullRequestDetails, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequestDetails));
              ITeamFoundationPolicyService service4 = requestContext.GetService<ITeamFoundationPolicyService>();
              bool completionOptionsValid = true;
              IVssRequestContext requestContext1 = requestContext;
              GitPullRequestTarget target = pullRequestTarget;
              ArtifactId artifactId2 = artifactId1;
              Func<ITeamFoundationMergeStrategyRestrictionPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult> action = (Func<ITeamFoundationMergeStrategyRestrictionPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) =>
              {
                List<GitPullRequestMergeStrategy> allowedMergeStrategies = policy.GetAllowedMergeStrategies();
                if (completionOptions == null && !allowedMergeStrategies.Contains(GitPullRequestMergeStrategy.NoFastForward) || completionOptions != null && completionOptions.MergeStrategy.HasValue && !allowedMergeStrategies.Contains(completionOptions.MergeStrategy.Value) || completionOptions != null && completionOptions.SquashMerge && !allowedMergeStrategies.Contains(GitPullRequestMergeStrategy.Squash))
                  completionOptionsValid = false;
                return (PolicyNotificationResult) null;
              });
              ClientTraceData ctData = clientTraceData;
              service4.NotifyPolicies<ITeamFoundationMergeStrategyRestrictionPolicy>(requestContext1, (ITeamFoundationPolicyTarget) target, artifactId2, action, ctData);
              if (!completionOptionsValid)
                throw new GitArgumentException(Resources.Get("InvalidAutoCompleteMergeStrategy"));
            }
            requestNotificationList.Add((PullRequestNotification) new AutoCompleteUpdatedNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, autoCompleteAuthority1, (string) null));
          }
        }
        List<int> completeIgnoreConfigIds1 = pullRequestDetails.CompletionOptions?.AutoCompleteIgnoreConfigIds;
        TfsGitPullRequest pullRequest;
        using (GitCoreComponent gitCoreComponent1 = requestContext.CreateGitCoreComponent())
        {
          GitCoreComponent gitCoreComponent2 = gitCoreComponent1;
          RepoKey key = repository.Key;
          int pullRequestId1 = pullRequestId;
          int status1 = (int) status;
          string title1 = title;
          string description1 = description;
          PullRequestAsyncStatus? mergeStatus = status == PullRequestStatus.Abandoned ? new PullRequestAsyncStatus?(PullRequestAsyncStatus.NotSet) : new PullRequestAsyncStatus?();
          Sha1Id? lastMergeSourceCommit = new Sha1Id?();
          Sha1Id? lastMergeTargetCommit = new Sha1Id?();
          Sha1Id? lastMergeCommit = status == PullRequestStatus.Abandoned ? new Sha1Id?(Sha1Id.Empty) : new Sha1Id?();
          Guid? completeWhenMergedAuthority;
          if (status != PullRequestStatus.Abandoned)
          {
            nullable2 = new Guid?();
            completeWhenMergedAuthority = nullable2;
          }
          else
            completeWhenMergedAuthority = new Guid?(Guid.Empty);
          Sha1Id? lastMergeSourceCommitToVerify = new Sha1Id?();
          int? codeReviewId = new int?();
          nullable1 = new bool?();
          bool? upgraded = nullable1;
          GitPullRequestCompletionOptions completionOptions1 = completionOptions;
          GitPullRequestMergeOptions mergeOptions1 = mergeOptions;
          PullRequestMergeFailureType? nullable3 = new PullRequestMergeFailureType?();
          DateTime? completionQueueTime = status == PullRequestStatus.Abandoned ? new DateTime?(new DateTime()) : new DateTime?();
          PullRequestMergeFailureType? mergeFailureType = nullable3;
          Guid? autoCompleteAuthority2 = autoCompleteAuthority;
          bool? isDraft1 = isDraft;
          pullRequest = gitCoreComponent2.UpdatePullRequest(key, pullRequestId1, (PullRequestStatus) status1, title1, description1, mergeStatus, lastMergeSourceCommit, lastMergeTargetCommit, lastMergeCommit, completeWhenMergedAuthority, lastMergeSourceCommitToVerify, codeReviewId, upgraded, completionOptions1, mergeOptions1, completionQueueTime, mergeFailureType, (string) null, (IEnumerable<Sha1Id>) null, autoCompleteAuthority2, isDraft1);
        }
        if (completionOptions != null)
        {
          List<int> completeIgnoreConfigIds2 = completionOptions.AutoCompleteIgnoreConfigIds;
          HashSet<int> collection = new HashSet<int>();
          collection.AddRangeIfRangeNotNull<int, HashSet<int>>((IEnumerable<int>) completeIgnoreConfigIds1);
          HashSet<int> intSet = new HashSet<int>();
          intSet.AddRangeIfRangeNotNull<int, HashSet<int>>((IEnumerable<int>) completeIgnoreConfigIds2);
          if (!collection.SetEquals((IEnumerable<int>) intSet))
            this.QueueAutoCompleteJobIfNeeded(requestContext, pullRequest);
        }
        pullRequest.SaveReviewProperties(requestContext, repository.Key.ProjectId);
        if (status == PullRequestStatus.Active)
          pullRequest.CreateNewIteration(requestContext, repository, true);
        if (status == PullRequestStatus.Abandoned)
        {
          if (requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.Conflicts"))
            TeamFoundationGitPullRequestService.DeleteAllInternalPullRequestRefs(requestContext, pullRequest);
          else
            repository.UpdatePullRequestMergeRef(requestContext, pullRequest.PullRequestId, new Sha1Id?(Sha1Id.Empty));
        }
        else if (status == PullRequestStatus.Active || pullRequest.Status == PullRequestStatus.Active && pullRequestDetails.IsDraft && !pullRequest.IsDraft)
          pullRequest = this.TryMerge(requestContext, repository, pullRequestId);
        bool flag1 = false;
        if (pullRequest.Status == PullRequestStatus.Active && pullRequestDetails.IsDraft != pullRequest.IsDraft)
        {
          this.NotifyPoliciesOfPublication(requestContext, pullRequest, repository, clientTraceData);
          requestNotificationList.Add((PullRequestNotification) new IsDraftUpdatedNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, foundationIdentity, pullRequest.IsDraft));
          flag1 = pullRequest.IsDraft && pullRequestDetails.Reviewers.Any<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (r => r.Vote != (short) 0));
        }
        if (requestNotificationList.Count > 0)
        {
          requestContext.Trace(1013257, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Publishing {0} PullRequestNotifications for pull request {1}", (object) requestNotificationList.Count, (object) pullRequestId);
          foreach (PullRequestNotification notificationEvent in requestNotificationList)
            service1.PublishNotification(requestContext, (object) notificationEvent);
        }
        clientTraceData.Add("PullRequestReviewerCount", (object) pullRequest.Reviewers.Count<TfsGitPullRequest.ReviewerWithVote>());
        clientTraceData.Add("PullRequestVoteCount", (object) pullRequest.Reviewers.Where<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (x => x.Vote != (short) 0)).Count<TfsGitPullRequest.ReviewerWithVote>());
        clientTraceData.Add("PullRequestDescriptionLength", (object) pullRequest.Description?.Length);
        clientTraceData.Add("PullRequestSupportsIterations", (object) pullRequest.SupportsIterations);
        service2.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", clientTraceData);
        if (flag1)
          this.ResetAllReviewerVotes(requestContext, repository, pullRequestId, false, Resources.Get("PullRequestDraftVotesResetReason"));
        return pullRequest;
      }
    }

    public TfsGitPullRequest RetargetPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      string targetRefName)
    {
      using (requestContext.TimeRegion("TfsGitPullRequest", nameof (RetargetPullRequest)))
      {
        TracepointUtils.Tracepoint(requestContext, 1013229, GitServerUtils.TraceArea, "PullRequestCompletionQueueJob", (Func<object>) (() => (object) new
        {
          pullRequestId = pullRequestId,
          targetRefName = targetRefName
        }), caller: nameof (RetargetPullRequest));
        List<PullRequestNotification> requestNotificationList = new List<PullRequestNotification>();
        IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
        ClientTraceData properties = new ClientTraceData();
        properties.Add("Action", (object) nameof (RetargetPullRequest));
        properties.Add("PullRequestId", (object) pullRequestId);
        properties.Add("RepositoryId", (object) repository.Key.RepoId.ToString());
        properties.Add("RepositoryName", (object) repository.Name);
        properties.Add("TargetRefName", (object) targetRefName);
        ClientTraceService service1 = requestContext.GetService<ClientTraceService>();
        TfsGitPullRequest originalPullRequest = this.GetPullRequestDetails(requestContext, repository, pullRequestId);
        if (string.Equals(originalPullRequest.TargetBranchName, targetRefName, StringComparison.Ordinal))
          throw new GitArgumentException(Resources.Format("GitPullRequestRetargetAlreadyTargeted", (object) targetRefName));
        repository.Permissions.CheckPullRequestContribute();
        TeamFoundationGitPullRequestService.VerifyPullRequestIsActive(originalPullRequest);
        try
        {
          ITfsGitRepositoryExtensions.VerifyLocalRef(repository, targetRefName);
        }
        catch
        {
          throw new GitRefNotFoundException(targetRefName, repository.Name);
        }
        if (originalPullRequest.SourceRepositoryId == originalPullRequest.RepositoryId && string.Equals(originalPullRequest.SourceBranchName, targetRefName))
          throw new GitArgumentException("GitPullRequestRetargetSourceSameAsTarget");
        if (originalPullRequest.BelongsToCompletionJob)
          throw new GitPullRequestNotEditableException();
        if (originalPullRequest.AutoCompleteAuthority != Guid.Empty)
          this.UpdatePullRequest(requestContext, repository, pullRequestId, PullRequestStatus.NotSet, (string) null, (string) null, (GitPullRequestCompletionOptions) null, (GitPullRequestMergeOptions) null, new Guid?(Guid.Empty), new bool?());
        ITeamFoundationPolicyService service2 = requestContext.GetService<ITeamFoundationPolicyService>();
        string projectUri = repository.Key.GetProjectUri();
        service2.NotifyPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitPullRequestTarget(projectUri, originalPullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, originalPullRequest)), originalPullRequest.BuildLegacyArtifactId(projectUri), (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) => policy.OnTargetBranchWillChange(requestContext, originalPullRequest, targetRefName, existingStatus, existingContext)));
        TfsGitPullRequest pullRequest;
        using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
          pullRequest = gitCoreComponent.RetargetPullRequest(repository.Key, pullRequestId, originalPullRequest.SourceBranchName, targetRefName);
        pullRequest.CreateNewIteration(requestContext, repository, false, newTargetRefName: targetRefName, oldTargetRefName: originalPullRequest.TargetBranchName);
        pullRequest.SaveReviewProperties(requestContext, repository.Key.ProjectId);
        this.EnterMergeJob(requestContext, repository, pullRequest.PullRequestId);
        if (requestContext.IsFeatureEnabled("Policy.EventBasedCacheEnabled.OnTargetUpdate"))
        {
          ArtifactId artifactId = pullRequest.BuildLegacyArtifactId(projectUri);
          new ActivePolicyEvaluationCache().Invalidate(requestContext, LinkingUtilities.EncodeUri(artifactId), nameof (RetargetPullRequest));
        }
        service2.NotifyPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitPullRequestTarget(projectUri, pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequest)), pullRequest.BuildLegacyArtifactId(projectUri), (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) => policy.OnTargetBranchChanged(requestContext, pullRequest, targetRefName, existingStatus, existingContext)));
        PullRequestRetargetNotification notificationEvent = new PullRequestRetargetNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequest.PullRequestId, IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext), targetRefName);
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        service1.Publish(requestContext, "Microsoft.TeamFoundation.Git.Server", "PullRequest", properties);
        return pullRequest;
      }
    }

    public bool HasPullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      requestContext.Trace(1013226, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Does pull request for id {0} and repository {1} exist?", (object) pullRequestId, (object) repository?.Key?.RepoId);
      TfsGitPullRequest pullRequest;
      this.TryGetPullRequestDetails(requestContext, repository, pullRequestId, out pullRequest);
      if (pullRequest != null && pullRequest.ProjectUri == null)
        return true;
      return pullRequest != null && this.HasReadPermission(requestContext, repository, pullRequest);
    }

    public TfsGitPullRequest GetPullRequestDetails(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      TfsGitPullRequest pullRequest;
      this.TryGetPullRequestDetails(requestContext, repository, pullRequestId, out pullRequest);
      if (pullRequest != null && pullRequest.ProjectUri == null || pullRequest != null && this.HasReadPermission(requestContext, repository, pullRequest))
        return pullRequest;
      throw new GitPullRequestNotFoundException();
    }

    private bool HasReadPermission(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest)
    {
      return repository != null || SecurityHelper.Instance.HasReadPermission(requestContext, (RepoScope) new RepoKey(ProjectInfo.GetProjectId(pullRequest.ProjectUri), pullRequest.RepositoryId));
    }

    public TfsGitPullRequest GetPullRequestDetails(
      IVssRequestContext requestContext,
      int pullRequestId)
    {
      return this.GetPullRequestDetails(requestContext, (ITfsGitRepository) null, pullRequestId);
    }

    public TfsGitPullRequest GetPullRequestDetails(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoKey, repoKey.RepoId.ToString());
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetPullRequestDetails(repoKey.RepoId, pullRequestId);
    }

    public IList<TfsGitPullRequest> QueryGitPullRequestsToBackfill(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      int firstPullRequestId = 1,
      int pullRequestsToFetch = 1000)
    {
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return gitCoreComponent.QueryGitPullRequestsToBackfill(repoScope, firstPullRequestId, pullRequestsToFetch);
    }

    public virtual bool TryGetPullRequestDetails(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      out TfsGitPullRequest pullRequest)
    {
      requestContext.Trace(1013226, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Getting pull request details for id {0} and repository {1}", (object) pullRequestId, (object) (repository == null ? Guid.Empty : repository.Key.RepoId));
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        pullRequest = gitCoreComponent.GetPullRequestDetails(repository == null ? Guid.Empty : repository.Key.RepoId, pullRequestId);
      return pullRequest != null;
    }

    public IEnumerable<TfsGitPullRequest> QueryPullRequests(
      IVssRequestContext requestContext,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      Guid? sourceRepositoryId = null,
      IEnumerable<string> sourceBranchFilters = null,
      IEnumerable<string> targetBranchFilters = null,
      bool? treatBranchFiltersAsUnion = null,
      PullRequestStatus? statusFilter = null,
      Guid? creatorIdFilter = null,
      Guid? assignedToIdFilter = null,
      bool orderAscending = false,
      TimeFilter timeFilter = null,
      int top = 1000,
      int skip = 0,
      bool completionAuthorityIsSet = false)
    {
      string str = !assignedToIdFilter.HasValue ? "MultiPerson" : "MultiPersonAssignedTo";
      IVssRequestContext requestContext1 = requestContext;
      string teamProjectUri1 = teamProjectUri;
      Guid? repositoryId1 = repositoryId;
      Guid? sourceRepositoryId1 = sourceRepositoryId;
      IEnumerable<string> sourceBranchFilters1 = sourceBranchFilters;
      IEnumerable<string> targetBranchFilters1 = targetBranchFilters;
      bool? treatBranchFiltersAsUnion1 = treatBranchFiltersAsUnion;
      PullRequestStatus? statusFilter1 = statusFilter;
      Guid[] creatorIdFilter1;
      if (!creatorIdFilter.HasValue)
        creatorIdFilter1 = (Guid[]) null;
      else
        creatorIdFilter1 = new Guid[1]
        {
          creatorIdFilter.Value
        };
      Guid[] assignedToIdFilter1;
      if (!assignedToIdFilter.HasValue)
        assignedToIdFilter1 = (Guid[]) null;
      else
        assignedToIdFilter1 = new Guid[1]
        {
          assignedToIdFilter.Value
        };
      int num1 = orderAscending ? 1 : 0;
      int top1 = top;
      int skip1 = skip;
      int num2 = completionAuthorityIsSet ? 1 : 0;
      string perfCounterInstanceName = str;
      DateTime? creationMinTime = timeFilter?.GetCreationMinTime();
      DateTime? creationMaxTime = timeFilter?.GetCreationMaxTime();
      DateTime? closedMinTime = timeFilter?.GetClosedMinTime();
      DateTime? closedMaxTime = timeFilter?.GetClosedMaxTime();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, teamProjectUri1, repositoryId1, sourceRepositoryId1, sourceBranchFilters1, targetBranchFilters1, treatBranchFiltersAsUnion1, statusFilter1, (IEnumerable<Guid>) creatorIdFilter1, (IEnumerable<Guid>) assignedToIdFilter1, num1 != 0, top1, skip1, num2 != 0, perfCounterInstanceName, creationMinTime, creationMaxTime, closedMinTime, closedMaxTime, minUpdatedTime, maxUpdatedTime, draftFilter, true);
    }

    public IEnumerable<TfsGitPullRequest> QueryActiveTargetPullRequests(
      IVssRequestContext requestContext,
      Guid repositoryId,
      string branchFilter,
      int top)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid? repositoryId1 = new Guid?(repositoryId);
      IEnumerable<string> strings1 = (IEnumerable<string>) new string[1]
      {
        branchFilter
      };
      IEnumerable<string> strings2 = (IEnumerable<string>) new string[1]
      {
        branchFilter
      };
      bool? nullable1 = new bool?(true);
      PullRequestStatus? nullable2 = new PullRequestStatus?(PullRequestStatus.Active);
      int num = top;
      Guid? sourceRepositoryId = new Guid?();
      IEnumerable<string> sourceBranchFilters = strings1;
      IEnumerable<string> targetBranchFilters = strings2;
      bool? treatBranchFiltersAsUnion = nullable1;
      PullRequestStatus? statusFilter = nullable2;
      int top1 = num;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, repositoryId: repositoryId1, sourceRepositoryId: sourceRepositoryId, sourceBranchFilters: sourceBranchFilters, targetBranchFilters: targetBranchFilters, treatBranchFiltersAsUnion: treatBranchFiltersAsUnion, statusFilter: statusFilter, top: top1, perfCounterInstanceName: "ActiveTargetPRs", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    public IEnumerable<TfsGitPullRequest> QueryActiveSourcePullRequests(
      IVssRequestContext requestContext,
      Guid sourceRepositoryId,
      string sourceBranchFilter,
      int top)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid? nullable1 = new Guid?(sourceRepositoryId);
      IEnumerable<string> strings = (IEnumerable<string>) new string[1]
      {
        sourceBranchFilter
      };
      PullRequestStatus? nullable2 = new PullRequestStatus?(PullRequestStatus.Active);
      int num = top;
      Guid? repositoryId = new Guid?();
      Guid? sourceRepositoryId1 = nullable1;
      IEnumerable<string> sourceBranchFilters = strings;
      bool? treatBranchFiltersAsUnion = new bool?();
      PullRequestStatus? statusFilter = nullable2;
      int top1 = num;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, repositoryId: repositoryId, sourceRepositoryId: sourceRepositoryId1, sourceBranchFilters: sourceBranchFilters, treatBranchFiltersAsUnion: treatBranchFiltersAsUnion, statusFilter: statusFilter, top: top1, perfCounterInstanceName: "ActiveSourcePRs", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    public IEnumerable<TfsGitPullRequest> QueryCompletedSourcePullRequests(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> sourceBranchFilters)
    {
      IVssRequestContext requestContext1 = requestContext;
      string teamProjectUri1 = teamProjectUri;
      Guid? repositoryId1 = new Guid?(repositoryId);
      IEnumerable<string> strings = sourceBranchFilters;
      PullRequestStatus? nullable = new PullRequestStatus?(PullRequestStatus.Completed);
      Guid? sourceRepositoryId = new Guid?();
      IEnumerable<string> sourceBranchFilters1 = strings;
      bool? treatBranchFiltersAsUnion = new bool?();
      PullRequestStatus? statusFilter = nullable;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, teamProjectUri1, repositoryId1, sourceRepositoryId, sourceBranchFilters1, treatBranchFiltersAsUnion: treatBranchFiltersAsUnion, statusFilter: statusFilter, perfCounterInstanceName: "CompletedSourcePRs", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    public IEnumerable<TfsGitPullRequest> QueryPullRequestsBySourceRepositoryRefs(
      IVssRequestContext requestContext,
      Guid sourceRepositoryId,
      IEnumerable<string> sourceBranchFilters)
    {
      IVssRequestContext requestContext1 = requestContext;
      Guid? nullable1 = new Guid?(sourceRepositoryId);
      IEnumerable<string> strings = sourceBranchFilters;
      bool? nullable2 = new bool?(false);
      PullRequestStatus? nullable3 = new PullRequestStatus?(PullRequestStatus.Active);
      Guid? repositoryId = new Guid?();
      Guid? sourceRepositoryId1 = nullable1;
      IEnumerable<string> sourceBranchFilters1 = strings;
      bool? treatBranchFiltersAsUnion = nullable2;
      PullRequestStatus? statusFilter = nullable3;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, repositoryId: repositoryId, sourceRepositoryId: sourceRepositoryId1, sourceBranchFilters: sourceBranchFilters1, treatBranchFiltersAsUnion: treatBranchFiltersAsUnion, statusFilter: statusFilter, perfCounterInstanceName: "BySourceRepositoryRefs", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    public IEnumerable<TfsGitPullRequest> QueryActiveSourcePullRequestsForBranches(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> sourceBranchFilters)
    {
      IVssRequestContext requestContext1 = requestContext;
      string teamProjectUri1 = teamProjectUri;
      Guid? repositoryId1 = new Guid?(repositoryId);
      IEnumerable<string> strings = sourceBranchFilters;
      PullRequestStatus? nullable = new PullRequestStatus?(PullRequestStatus.Active);
      Guid? sourceRepositoryId = new Guid?();
      IEnumerable<string> sourceBranchFilters1 = strings;
      bool? treatBranchFiltersAsUnion = new bool?();
      PullRequestStatus? statusFilter = nullable;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, teamProjectUri1, repositoryId1, sourceRepositoryId, sourceBranchFilters1, treatBranchFiltersAsUnion: treatBranchFiltersAsUnion, statusFilter: statusFilter, perfCounterInstanceName: "ActiveSourcePRsForBranches", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    public IEnumerable<TfsGitPullRequest> QueryPullRequestsForSuggestions(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      Guid sourceRepositoryId,
      IEnumerable<string> sourceBranchFilters)
    {
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext, teamProjectUri, new Guid?(repositoryId), new Guid?(sourceRepositoryId), sourceBranchFilters, top: 5, perfCounterInstanceName: "ForSuggestions");
    }

    public IEnumerable<TfsGitPullRequest> QueryActivePullRequestsBySourceAndTargetBranchFilters(
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      IEnumerable<string> branchFilters)
    {
      IVssRequestContext requestContext1 = requestContext;
      string teamProjectUri1 = teamProjectUri;
      Guid? repositoryId1 = new Guid?(repositoryId);
      IEnumerable<string> strings1 = branchFilters;
      IEnumerable<string> strings2 = branchFilters;
      bool? nullable1 = new bool?(true);
      PullRequestStatus? nullable2 = new PullRequestStatus?(PullRequestStatus.Active);
      Guid? sourceRepositoryId = new Guid?();
      IEnumerable<string> sourceBranchFilters = strings2;
      IEnumerable<string> targetBranchFilters = strings1;
      bool? treatBranchFiltersAsUnion = nullable1;
      PullRequestStatus? statusFilter = nullable2;
      DateTime? minCreationTime = new DateTime?();
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime = new DateTime?();
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime = new DateTime?();
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter = new bool?();
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, teamProjectUri1, repositoryId1, sourceRepositoryId, sourceBranchFilters, targetBranchFilters, treatBranchFiltersAsUnion, statusFilter, perfCounterInstanceName: "BySourceAndTargetBranchFilters", minCreationTime: minCreationTime, maxCreationTime: maxCreationTime, minClosedTime: minClosedTime, maxClosedTime: maxClosedTime, minUpdatedTime: minUpdatedTime, maxUpdatedTime: maxUpdatedTime, draftFilter: draftFilter);
    }

    private IEnumerable<TfsGitPullRequest> QueryPullRequestsAndLogToPerfCounter(
      IVssRequestContext requestContext,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      Guid? sourceRepositoryId = null,
      IEnumerable<string> sourceBranchFilters = null,
      IEnumerable<string> targetBranchFilters = null,
      bool? treatBranchFiltersAsUnion = null,
      PullRequestStatus? statusFilter = null,
      IEnumerable<Guid> creatorIdFilter = null,
      IEnumerable<Guid> assignedToIdFilter = null,
      bool orderAscending = false,
      int top = 1000,
      int skip = 0,
      bool completionAuthorityIsSet = false,
      string perfCounterInstanceName = "QueryPullRequests",
      DateTime? minCreationTime = null,
      DateTime? maxCreationTime = null,
      DateTime? minClosedTime = null,
      DateTime? maxClosedTime = null,
      DateTime? minUpdatedTime = null,
      DateTime? maxUpdatedTime = null,
      bool? draftFilter = null,
      bool isSecondVersionUsed = false)
    {
      requestContext.Trace(1013227, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Query pull requests. TeamProject: {0}, top: {1}, skip: {2}, perfCounterInstanceName: {3}, isSecondVersionUsed {4}", (object) teamProjectUri, (object) top, (object) skip, (object) perfCounterInstanceName, (object) isSecondVersionUsed);
      Stopwatch stopwatch = Stopwatch.StartNew();
      IEnumerable<TfsGitPullRequest> pullRequests;
      using (GitCoreComponent gitCoreComponent1 = requestContext.CreateGitCoreComponent())
      {
        GitCoreComponent gitCoreComponent2 = gitCoreComponent1;
        string teamProjectUri1 = teamProjectUri;
        Guid? repositoryId1 = repositoryId;
        Guid? sourceRepositoryId1 = sourceRepositoryId;
        IEnumerable<string> sourceBranchFilters1 = sourceBranchFilters;
        IEnumerable<string> targetBranchFilters1 = targetBranchFilters;
        bool? treatBranchFiltersAsUnion1 = treatBranchFiltersAsUnion;
        PullRequestStatus? statusFilter1 = statusFilter;
        IEnumerable<Guid> creatorIdFilter1 = creatorIdFilter;
        IEnumerable<Guid> assignedToIdFilter1 = assignedToIdFilter;
        DateTime? minClosedTime1 = minClosedTime;
        DateTime? nullable = maxClosedTime;
        Guid? mergeId = new Guid?();
        int num1 = orderAscending ? 1 : 0;
        int top1 = top;
        int skip1 = skip;
        int num2 = completionAuthorityIsSet ? 1 : 0;
        DateTime? maxClosedTime1 = nullable;
        DateTime? minCreationTime1 = minCreationTime;
        DateTime? maxCreationTime1 = maxCreationTime;
        DateTime? minUpdatedTime1 = minUpdatedTime;
        DateTime? maxUpdatedTime1 = maxUpdatedTime;
        bool? draftFilter1 = draftFilter;
        int num3 = isSecondVersionUsed ? 1 : 0;
        pullRequests = gitCoreComponent2.QueryGitPullRequests(teamProjectUri1, repositoryId1, sourceRepositoryId1, sourceBranchFilters1, targetBranchFilters1, treatBranchFiltersAsUnion1, statusFilter1, creatorIdFilter1, assignedToIdFilter1, minClosedTime1, mergeId, num1 != 0, top1, skip1, num2 != 0, maxClosedTime1, minCreationTime1, maxCreationTime1, minUpdatedTime1, maxUpdatedTime1, draftFilter1, num3 != 0);
      }
      this.AddToQueryPullRequestsPerformanceCounters(perfCounterInstanceName, stopwatch.ElapsedMilliseconds);
      return this.FilterOutPullRequestsByPermissions(pullRequests, requestContext, teamProjectUri);
    }

    public IEnumerable<TfsGitPullRequest> QueryPullRequestsMultiPerson(
      IVssRequestContext requestContext,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      Guid? sourceRepositoryId = null,
      IEnumerable<string> sourceBranchFilters = null,
      IEnumerable<string> targetBranchFilters = null,
      bool? treatBranchFiltersAsUnion = null,
      PullRequestStatus? statusFilter = null,
      IEnumerable<Guid> creatorIdFilter = null,
      IEnumerable<Guid> assignedToIdFilter = null,
      bool orderAscending = false,
      int top = 1000,
      int skip = 0,
      bool completionAuthorityIsSet = false,
      DateTime? minCreationTime = null,
      DateTime? minClosedTime = null,
      DateTime? minUpdatedTime = null,
      bool? draftFilter = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      string teamProjectUri1 = teamProjectUri;
      Guid? repositoryId1 = repositoryId;
      Guid? sourceRepositoryId1 = sourceRepositoryId;
      IEnumerable<string> sourceBranchFilters1 = sourceBranchFilters;
      IEnumerable<string> targetBranchFilters1 = targetBranchFilters;
      bool? treatBranchFiltersAsUnion1 = treatBranchFiltersAsUnion;
      PullRequestStatus? statusFilter1 = statusFilter;
      IEnumerable<Guid> creatorIdFilter1 = creatorIdFilter;
      IEnumerable<Guid> assignedToIdFilter1 = assignedToIdFilter;
      int num1 = orderAscending ? 1 : 0;
      int top1 = top;
      int skip1 = skip;
      int num2 = completionAuthorityIsSet ? 1 : 0;
      DateTime? minCreationTime1 = minCreationTime;
      DateTime? nullable1 = minClosedTime;
      DateTime? nullable2 = minUpdatedTime;
      bool? nullable3 = draftFilter;
      DateTime? maxCreationTime = new DateTime?();
      DateTime? minClosedTime1 = nullable1;
      DateTime? maxClosedTime = new DateTime?();
      DateTime? minUpdatedTime1 = nullable2;
      DateTime? maxUpdatedTime = new DateTime?();
      bool? draftFilter1 = nullable3;
      return this.QueryPullRequestsAndLogToPerfCounter(requestContext1, teamProjectUri1, repositoryId1, sourceRepositoryId1, sourceBranchFilters1, targetBranchFilters1, treatBranchFiltersAsUnion1, statusFilter1, creatorIdFilter1, assignedToIdFilter1, num1 != 0, top1, skip1, num2 != 0, "MultiPerson", minCreationTime1, maxCreationTime, minClosedTime1, maxClosedTime, minUpdatedTime1, maxUpdatedTime, draftFilter1);
    }

    public IEnumerable<TfsGitPullRequest> QueryPullRequestsBulk(
      IVssRequestContext requestContext,
      IEnumerable<Guid> assignedToIdFilters,
      PullRequestStatus statusFilter,
      Guid? creatorIdFilter = null,
      string teamProjectUri = null,
      Guid? repositoryId = null,
      int top = 1000)
    {
      requestContext.Trace(1013685, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Query pull requests bulk. TeamProject: {0}, top: {1}", (object) teamProjectUri, (object) top);
      IEnumerable<TfsGitPullRequest> pullRequests;
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        pullRequests = gitCoreComponent.QueryGitPullRequestsBulk(teamProjectUri, repositoryId, new PullRequestStatus?(statusFilter), creatorIdFilter, assignedToIdFilters, top);
      return this.FilterOutPullRequestsByPermissions(pullRequests, requestContext, teamProjectUri);
    }

    public void SharePullRequest(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      ShareNotificationContext userMessage)
    {
      requestContext.Trace(1013800, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Pull request {0} will be shared", (object) pullRequest.PullRequestId);
      ArgumentUtility.CheckForNull<ShareNotificationContext>(userMessage, nameof (userMessage));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userMessage.Receivers, "Receivers");
      repository.Permissions.CheckPullRequestContribute();
      int maxReceivers = TeamFoundationGitPullRequestService.GetMaxReceivers(requestContext);
      if (userMessage.Receivers.Count > maxReceivers)
        throw new GitArgumentException(Resources.Format("TooManyReceiversSupplied", (object) maxReceivers));
      int maxMessageLength = TeamFoundationGitPullRequestService.GetMaxMessageLength(requestContext);
      if (userMessage.Message != null && userMessage.Message.Length > maxMessageLength)
        throw new GitArgumentException(Resources.Format("ShareMessageTooLong", (object) maxMessageLength));
      requestContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(requestContext, (object) new PullRequestShareNotification(repository.Key.GetProjectUri(), pullRequest.RepositoryId, pullRequest.RepositoryName, pullRequest.PullRequestId, requestContext.GetUserIdentity().Id, (IEnumerable<IdentityRef>) userMessage.Receivers, userMessage.Message));
    }

    private static int GetMaxReceivers(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestShareMaxReceivers", 100);

    private static int GetMaxMessageLength(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestShareMaxMessage", 1024);

    private IEnumerable<TfsGitPullRequest> FilterOutPullRequestsByPermissions(
      IEnumerable<TfsGitPullRequest> pullRequests,
      IVssRequestContext requestContext,
      string teamProjectUri)
    {
      Dictionary<Guid, bool> hasRepositoryPermission = new Dictionary<Guid, bool>();
      if (teamProjectUri == null)
      {
        Dictionary<string, bool> hasProjectPermission = pullRequests.GroupBy<TfsGitPullRequest, string>((Func<TfsGitPullRequest, string>) (pr => pr.ProjectUri)).Select<IGrouping<string, TfsGitPullRequest>, string>((Func<IGrouping<string, TfsGitPullRequest>, string>) (grp => grp.Key)).ToDictionary<string, string, bool>((Func<string, string>) (projUri => projUri), (Func<string, bool>) (projUri => SecurityHelper.Instance.HasReadProjectPermission(requestContext, projUri)));
        hasRepositoryPermission = pullRequests.GroupBy<TfsGitPullRequest, Guid>((Func<TfsGitPullRequest, Guid>) (pr => pr.RepositoryId)).Select<IGrouping<Guid, TfsGitPullRequest>, TfsGitPullRequest>((Func<IGrouping<Guid, TfsGitPullRequest>, TfsGitPullRequest>) (grp => grp.First<TfsGitPullRequest>())).ToDictionary<TfsGitPullRequest, Guid, bool>((Func<TfsGitPullRequest, Guid>) (pr => pr.RepositoryId), (Func<TfsGitPullRequest, bool>) (pr => hasProjectPermission.ContainsKey(pr.ProjectUri) && hasProjectPermission[pr.ProjectUri] && SecurityHelper.Instance.HasReadPermission(requestContext, new RepoScope(ProjectInfo.GetProjectId(pr.ProjectUri), pr.RepositoryId))));
      }
      else
        hasRepositoryPermission = pullRequests.Select<TfsGitPullRequest, Guid>((Func<TfsGitPullRequest, Guid>) (pr => pr.RepositoryId)).Distinct<Guid>().ToDictionary<Guid, Guid, bool>((Func<Guid, Guid>) (repoId => repoId), (Func<Guid, bool>) (repoId => SecurityHelper.Instance.HasReadPermission(requestContext, new RepoScope(ProjectInfo.GetProjectId(teamProjectUri), repoId))));
      return pullRequests.Where<TfsGitPullRequest>((Func<TfsGitPullRequest, bool>) (pr => hasRepositoryPermission.ContainsKey(pr.RepositoryId) && hasRepositoryPermission[pr.RepositoryId]));
    }

    private TfsGitPullRequest UpdatePullRequestInDatabase(
      IVssRequestContext requestContext,
      Guid projectId,
      TfsGitPullRequest pullRequest,
      PullRequestStatus? status = null,
      PullRequestAsyncStatus? mergeStatus = null,
      Sha1Id? lastMergeSourceCommit = null,
      Sha1Id? lastMergeTargetCommit = null,
      Sha1Id? lastMergeCommit = null,
      Guid? completeWhenMergedAuthority = null,
      GitPullRequestCompletionOptions completionOptions = null,
      GitPullRequestMergeOptions mergeOptions = null,
      PullRequestMergeFailureType? mergeFailureType = null,
      string mergeFailureMessage = null,
      DateTime? completionQueueTime = null,
      IEnumerable<Sha1Id> commits = null,
      Guid? autoCompleteAuthority = null,
      bool? isDraft = null)
    {
      Sha1Id? nullable1;
      if ((PullRequestStatus) ((int) status ?? (int) pullRequest.Status) == pullRequest.Status && (PullRequestAsyncStatus) ((int) mergeStatus ?? (int) pullRequest.MergeStatus) == pullRequest.MergeStatus)
      {
        Sha1Id? nullable2 = lastMergeSourceCommit ?? pullRequest.LastMergeSourceCommit;
        nullable1 = pullRequest.LastMergeSourceCommit;
        if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          nullable1 = lastMergeTargetCommit ?? pullRequest.LastMergeTargetCommit;
          nullable2 = pullRequest.LastMergeTargetCommit;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
          {
            nullable2 = lastMergeCommit ?? pullRequest.LastMergeCommit;
            nullable1 = pullRequest.LastMergeCommit;
            if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && (completeWhenMergedAuthority ?? pullRequest.CompleteWhenMergedAuthority) == pullRequest.CompleteWhenMergedAuthority && (completionOptions == null || completionOptions.Equals((object) pullRequest.CompletionOptions)) && (completionQueueTime ?? pullRequest.CompletionQueueTime) == pullRequest.CompletionQueueTime && commits == null && (autoCompleteAuthority ?? pullRequest.AutoCompleteAuthority) == pullRequest.AutoCompleteAuthority && ((int) isDraft ?? (pullRequest.IsDraft ? 1 : 0)) == (pullRequest.IsDraft ? 1 : 0))
              return pullRequest;
          }
        }
      }
      using (GitCoreComponent gitCoreComponent1 = requestContext.CreateGitCoreComponent())
      {
        GitCoreComponent gitCoreComponent2 = gitCoreComponent1;
        RepoKey repoKey = new RepoKey(projectId, pullRequest.RepositoryId);
        int pullRequestId = pullRequest.PullRequestId;
        int valueOrDefault = (int) status.GetValueOrDefault();
        PullRequestAsyncStatus? mergeStatus1 = mergeStatus;
        Sha1Id? lastMergeSourceCommit1 = lastMergeSourceCommit;
        Sha1Id? lastMergeTargetCommit1 = lastMergeTargetCommit;
        Sha1Id? lastMergeCommit1 = lastMergeCommit;
        Guid? completeWhenMergedAuthority1 = completeWhenMergedAuthority;
        nullable1 = new Sha1Id?();
        Sha1Id? lastMergeSourceCommitToVerify = nullable1;
        int? codeReviewId = new int?();
        bool? upgraded = new bool?();
        GitPullRequestCompletionOptions completionOptions1 = completionOptions;
        GitPullRequestMergeOptions mergeOptions1 = mergeOptions;
        DateTime? completionQueueTime1 = completionQueueTime;
        PullRequestMergeFailureType? mergeFailureType1 = mergeFailureType;
        string mergeFailureMessage1 = mergeFailureMessage;
        IEnumerable<Sha1Id> commits1 = commits;
        Guid? autoCompleteAuthority1 = autoCompleteAuthority;
        bool? isDraft1 = isDraft;
        return gitCoreComponent2.UpdatePullRequest(repoKey, pullRequestId, (PullRequestStatus) valueOrDefault, (string) null, (string) null, mergeStatus1, lastMergeSourceCommit1, lastMergeTargetCommit1, lastMergeCommit1, completeWhenMergedAuthority1, lastMergeSourceCommitToVerify, codeReviewId, upgraded, completionOptions1, mergeOptions1, completionQueueTime1, mergeFailureType1, mergeFailureMessage1, commits1, autoCompleteAuthority1, isDraft1);
      }
    }

    private static StatusUpdateNotification CreateStatusUpdateNotification(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId,
      PullRequestStatus newStatus,
      TeamFoundationIdentity updaterIdentity,
      Sha1Id? associatedCommit = null)
    {
      StatusUpdateNotification updateNotification = new StatusUpdateNotification(repository.Key.GetProjectUri(), repository.Key.RepoId, repository.Name, pullRequestId, updaterIdentity, newStatus, associatedCommit);
      requestContext.Trace(1013256, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitPullRequest", "Publishing a PullRequestStatusUpdateNotification for pull request {0} by {1} {2}", (object) pullRequestId, (object) updaterIdentity.DisplayName, (object) updaterIdentity.TeamFoundationId);
      return updateNotification;
    }

    private void SyncPublishNotification(
      IVssRequestContext requestContext,
      PullRequestNotification notification,
      int tracepoint,
      ClientTraceData ctData,
      string ciKey,
      object extraLogData = null,
      ITeamFoundationEventService eventManager = null)
    {
      ctData.LogElapsedMs(ciKey, (Action) (() =>
      {
        try
        {
          if (eventManager == null)
            eventManager = requestContext.GetService<ITeamFoundationEventService>();
          TracepointUtils.TraceBlock(requestContext, tracepoint, GitServerUtils.TraceArea, "TfsGitPullRequest", (Action) (() => eventManager.SyncPublishNotification(requestContext, (object) notification)), (Func<object>) (() => (object) new
          {
            notificationType = notification.GetType().Name,
            PullRequestId = notification.PullRequestId,
            RepositoryId = notification.RepositoryId,
            TeamProjectUri = notification.TeamProjectUri
          }), (Func<object>) (() => extraLogData), caller: nameof (SyncPublishNotification));
        }
        catch
        {
        }
      }));
    }

    private void NotifyPoliciesOfPublication(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      ITfsGitRepository repository,
      ClientTraceData ctData = null)
    {
      string projectUri = ProjectInfo.GetProjectUri(repository.Key.ProjectId);
      ArtifactId artifactId = pullRequest.BuildLegacyArtifactId(projectUri);
      requestContext.GetService<ITeamFoundationPolicyService>().NotifyPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitPullRequestTarget(projectUri, pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, pullRequest)), artifactId, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult>) ((policy, existingStatus, existingContext) => policy.OnPublished(requestContext, pullRequest, existingStatus, existingContext)), ctData);
    }

    public void CreateGitCommitToPullRequests(
      IVssRequestContext requestContext,
      RepoKey repoScope,
      IDictionary<int, IEnumerable<Sha1Id>> pullRequestCommits)
    {
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        gitCoreComponent.CreateGitCommitToPullRequests(repoScope, pullRequestCommits);
    }

    public ILookup<Sha1Id, TfsGitPullRequest> QueryPullRequestsByMergeCommits(
      IVssRequestContext requestContext,
      IEnumerable<Sha1Id> commits,
      RepoKey repoScope)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoScope, (string) null);
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return gitCoreComponent.QueryGitPullRequestsByMergeCommits(repoScope, commits);
    }

    public ILookup<Sha1Id, TfsGitPullRequest> QueryPullRequestsByCommits(
      IVssRequestContext requestContext,
      IEnumerable<Sha1Id> commits,
      RepoKey repoScope)
    {
      SecurityHelper.Instance.CheckReadPermission(requestContext, (RepoScope) repoScope, (string) null);
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return gitCoreComponent.QueryGitPullRequestsByCommits(repoScope, commits);
    }

    public bool RefHasActivePullRequests(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string refName,
      int threshold)
    {
      return this.QueryActiveTargetPullRequests(requestContext, repository.Key.RepoId, refName, threshold).Count<TfsGitPullRequest>() + this.QueryActiveSourcePullRequests(requestContext, repository.Key.RepoId, refName, threshold).Count<TfsGitPullRequest>() >= threshold;
    }

    public IEnumerable<TfsGitPullRequest> GetPullRequestsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime updatedTime,
      int pullRequestId,
      int batchSize)
    {
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetPullRequestsChangedSinceLastWatermark(requestContext, updatedTime, pullRequestId, batchSize);
    }

    private static void VerifyPullRequestIsActive(TfsGitPullRequest pullRequest)
    {
      if (pullRequest.Status != PullRequestStatus.Active)
        throw new GitPullRequestNotEditableException();
    }

    protected void AddToQueryPullRequestsPerformanceCounters(string instanceName, long elapsedMs)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Git.Server.PullRequestQueryPerfCounters.QueryPullRequestsPerSec", instanceName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Git.Server.PullRequestQueryPerfCounters.AverageQueryPullRequestsTime", instanceName).IncrementMilliseconds(elapsedMs);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Git.Server.PullRequestQueryPerfCounters.AverageQueryPullRequestsTimeBase", instanceName).Increment();
    }

    internal static TfsGitRef VerifySourceRef(
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      bool allowDeletion = false)
    {
      return ITfsGitRepositoryExtensions.VerifyLocalRef(repository, pullRequest.SourceBranchName, allowDeletion);
    }

    private static TeamFoundationIdentity[] VerifyReviewerIdentities(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest.ReviewerBase> reviewers)
    {
      if (reviewers == null)
        return (TeamFoundationIdentity[]) null;
      List<Guid> reviewerIds = new List<Guid>(reviewers.Select<TfsGitPullRequest.ReviewerBase, Guid>((Func<TfsGitPullRequest.ReviewerBase, Guid>) (reviewer => reviewer.Reviewer)));
      return TeamFoundationGitPullRequestService.VerifyReviewerIdentities(requestContext, (IEnumerable<Guid>) reviewerIds);
    }

    private static TeamFoundationIdentity[] VerifyReviewerIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Guid> reviewerIds)
    {
      if (reviewerIds == null)
        return (TeamFoundationIdentity[]) null;
      ITeamFoundationIdentityService service = requestContext.Elevate().GetService<ITeamFoundationIdentityService>();
      Guid[] array = reviewerIds.ToArray<Guid>();
      if (array.Length > 100)
        throw new GitArgumentException(Resources.Format("TooManyReviewersSupplied", (object) 100));
      TeamFoundationIdentity[] foundationIdentityArray = service.ReadIdentities(requestContext.Elevate(), array);
      for (int index = 0; index < array.Length; ++index)
      {
        if (index >= foundationIdentityArray.Length || foundationIdentityArray[index] == null || foundationIdentityArray[index].TeamFoundationId != array[index])
          throw new GitIdentityNotFoundException(array[index].ToString());
      }
      return foundationIdentityArray;
    }

    private static void VerifyTotalCountOfReviewers(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> reviewersToUpdate,
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> existingReviewers)
    {
      int maxCount = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/PullRequestMaxReviewersCount", true, 1000);
      IEnumerable<TfsGitPullRequest.ReviewerWithVote> source = reviewersToUpdate.Where<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (reviewer => reviewer.Status != ReviewerVoteStatus.Removed));
      int num1 = source.Count<TfsGitPullRequest.ReviewerWithVote>();
      int num2 = reviewersToUpdate.Count<TfsGitPullRequest.ReviewerWithVote>() - num1;
      HashSet<Guid> existingReviewersGuids = new HashSet<Guid>(existingReviewers.Select<TfsGitPullRequest.ReviewerWithVote, Guid>((Func<TfsGitPullRequest.ReviewerWithVote, Guid>) (reviewer => reviewer.Reviewer)));
      int num3 = source.Count<TfsGitPullRequest.ReviewerWithVote>((Func<TfsGitPullRequest.ReviewerWithVote, bool>) (reviewer => !existingReviewersGuids.Contains(reviewer.Reviewer)));
      if (existingReviewers.Count<TfsGitPullRequest.ReviewerWithVote>() + num3 - num2 > maxCount && num3 > num2)
        throw new GitPullRequestMaxReviewerCountException(maxCount);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private enum CompletePullRequestResult
    {
      Success,
      MergeOutOfDate,
      RejectedByPolicy,
      OtherError,
    }
  }
}
