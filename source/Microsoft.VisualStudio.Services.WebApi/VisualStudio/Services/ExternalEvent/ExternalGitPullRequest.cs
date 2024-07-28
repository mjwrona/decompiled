// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitPullRequest
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  [DataContract]
  public class ExternalGitPullRequest : IExternalArtifact, IExternalGitEvent, IAdditionalProperties
  {
    private const string c_contributorAssociation = "CONTRIBUTOR";
    [IgnoreDataMember]
    public static ApiResourceVersion CurrentVersion = new ApiResourceVersion(new Version(1, 0), 1);
    [DataMember]
    public string Id;
    [DataMember]
    public string Number;
    [DataMember]
    public string Title;
    [DataMember]
    public string Description;
    [DataMember]
    public string Url;
    [DataMember]
    public string WebUrl;
    [DataMember]
    public string MergeRef;
    [DataMember]
    public string TargetRef;
    [DataMember]
    public string TargetSha;
    [DataMember]
    public string SourceRef;
    [DataMember]
    public string SourceSha;
    [DataMember]
    public bool IsFork;
    [DataMember]
    public bool Draft;
    [DataMember]
    public string State;
    [DataMember]
    public bool? IsMergeable;
    [DataMember]
    public string ReviewDecision;
    [DataMember]
    public string MergeCommitSha;
    [DataMember]
    public string MergedAt;
    [DataMember]
    public string UpdatedAt;
    [DataMember]
    public string CreatedAt;
    [DataMember]
    public string ClosedAt;
    [DataMember]
    public ExternalGitRepo Repo;
    [DataMember]
    public string ProjectId;
    [DataMember]
    public ExternalGitUser Sender;
    [DataMember]
    public IDictionary<string, string> Properties;
    [DataMember]
    public bool IsFromComment;
    [DataMember]
    public string DefinitionToBuild;
    [DataMember]
    public string BuildToRetry;
    [DataMember]
    public ExternalGitCommit LastHeadRefCommit;
    [DataMember]
    public IEnumerable<ExternalGitCommit> Commits;
    [DataMember]
    public ICollection<ExternalGitUser> Assignees;
    [DataMember]
    public string AuthorAssociation;
    [DataMember]
    public bool DoesAuthorHaveWriteAccess;

    [DataMember]
    public string PipelineEventId { get; set; }

    public IDictionary<string, object> AdditionalProperties { get; set; }

    [DataMember]
    public IDictionary<string, IEnumerable<ExternalCheckRun>> StatusCheckResults { get; set; }

    [IgnoreDataMember]
    public bool AuthorIsRepoContributor => this.AuthorAssociation == "CONTRIBUTOR";
  }
}
