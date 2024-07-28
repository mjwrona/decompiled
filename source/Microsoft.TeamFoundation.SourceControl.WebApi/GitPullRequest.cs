// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequest
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ServiceEventObject]
  [DataContract]
  public class GitPullRequest : VersionControlSecuredObject
  {
    [DataMember(Name = "repository", EmitDefaultValue = false)]
    public GitRepository Repository { get; set; }

    [DataMember(Name = "pullRequestId", EmitDefaultValue = false)]
    public int PullRequestId { get; set; }

    [DataMember(Name = "codeReviewId", EmitDefaultValue = false)]
    public int CodeReviewId { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public PullRequestStatus Status { get; set; }

    [DataMember(Name = "createdBy", EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(Name = "creationDate", EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(Name = "closedDate", EmitDefaultValue = false)]
    public DateTime ClosedDate { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "sourceRefName", EmitDefaultValue = false)]
    public string SourceRefName { get; set; }

    [DataMember(Name = "targetRefName", EmitDefaultValue = false)]
    public string TargetRefName { get; set; }

    [DataMember(Name = "mergeStatus", EmitDefaultValue = false)]
    public PullRequestAsyncStatus MergeStatus { get; set; }

    [DataMember(Name = "mergeFailureType", EmitDefaultValue = false)]
    public PullRequestMergeFailureType MergeFailureType { get; set; }

    [DataMember(Name = "mergeFailureMessage", EmitDefaultValue = false)]
    public string MergeFailureMessage { get; set; }

    [DataMember(Name = "isDraft", EmitDefaultValue = false)]
    public bool? IsDraft { get; set; }

    [DataMember(Name = "hasMultipleMergeBases", EmitDefaultValue = false)]
    public bool HasMultipleMergeBases { get; set; }

    [DataMember(Name = "mergeId", EmitDefaultValue = false)]
    public Guid MergeId { get; set; }

    [DataMember(Name = "lastMergeSourceCommit", EmitDefaultValue = false)]
    public GitCommitRef LastMergeSourceCommit { get; set; }

    [DataMember(Name = "lastMergeTargetCommit", EmitDefaultValue = false)]
    public GitCommitRef LastMergeTargetCommit { get; set; }

    [DataMember(Name = "lastMergeCommit", EmitDefaultValue = false)]
    public GitCommitRef LastMergeCommit { get; set; }

    [DataMember(Name = "reviewers", EmitDefaultValue = false)]
    public IdentityRefWithVote[] Reviewers { get; set; }

    [DataMember(Name = "labels", EmitDefaultValue = false)]
    public WebApiTagDefinition[] Labels { get; set; }

    [DataMember(Name = "commits", EmitDefaultValue = false)]
    public GitCommitRef[] Commits { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RemoteUrl { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    [DataMember(Name = "completionOptions", EmitDefaultValue = false)]
    public GitPullRequestCompletionOptions CompletionOptions { get; set; }

    [DataMember(Name = "mergeOptions", EmitDefaultValue = false)]
    public GitPullRequestMergeOptions MergeOptions { get; set; }

    [DataMember(Name = "supportsIterations", EmitDefaultValue = false)]
    public bool SupportsIterations { get; set; }

    [DataMember(Name = "workItemRefs", EmitDefaultValue = false)]
    public ResourceRef[] WorkItemRefs { get; set; }

    [DataMember(Name = "completionQueueTime", EmitDefaultValue = false)]
    public DateTime CompletionQueueTime { get; set; }

    [DataMember(Name = "closedBy", EmitDefaultValue = false)]
    public IdentityRef ClosedBy { get; set; }

    [DataMember(Name = "autoCompleteSetBy", EmitDefaultValue = false)]
    public IdentityRef AutoCompleteSetBy { get; set; }

    [DataMember(Name = "artifactId", EmitDefaultValue = false)]
    public string ArtifactId { get; set; }

    [DataMember(Name = "forkSource", EmitDefaultValue = false)]
    public GitForkRef ForkSource { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Repository?.SetSecuredObject(securedObject);
      this.LastMergeSourceCommit?.SetSecuredObject(securedObject);
      this.LastMergeTargetCommit?.SetSecuredObject(securedObject);
      this.LastMergeCommit?.SetSecuredObject(securedObject);
      GitCommitRef[] commits = this.Commits;
      if (commits != null)
        ((IEnumerable<GitCommitRef>) commits).SetSecuredObject<GitCommitRef>(securedObject);
      this.CompletionOptions?.SetSecuredObject(securedObject);
      this.MergeOptions?.SetSecuredObject(securedObject);
      this.ForkSource?.SetSecuredObject(securedObject);
      this.SetSecuredObjectOnCollections(securedObject);
    }

    private void SetSecuredObjectOnCollections(ISecuredObject securedObject)
    {
      if (this.Labels != null)
      {
        foreach (WebApiTagDefinition label in this.Labels)
          label?.SetSecuredObject(securedObject);
      }
      if (this.WorkItemRefs == null)
        return;
      foreach (ResourceRef workItemRef in this.WorkItemRefs)
        workItemRef?.SetSecuredObject(securedObject);
    }
  }
}
