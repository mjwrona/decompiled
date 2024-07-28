// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequestIteration
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPullRequestIteration : VersionControlSecuredObject
  {
    [DataMember]
    public int? Id { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<GitPullRequestChange> ChangeList { get; set; }

    [DataMember]
    public DateTime? CreatedDate { get; set; }

    [DataMember]
    public DateTime? UpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef SourceRefCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef TargetRefCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitCommitRef CommonRefCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<GitCommitRef> Commits { get; set; }

    [DataMember]
    public bool hasMoreCommits { get; set; }

    [DataMember]
    public IterationReason Reason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitPushRef Push { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string NewTargetRefName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OldTargetRefName { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IList<GitPullRequestChange> changeList = this.ChangeList;
      if (changeList != null)
        changeList.SetSecuredObject<GitPullRequestChange>(securedObject);
      this.SourceRefCommit?.SetSecuredObject(securedObject);
      this.TargetRefCommit?.SetSecuredObject(securedObject);
      this.CommonRefCommit?.SetSecuredObject(securedObject);
      IList<GitCommitRef> commits = this.Commits;
      if (commits != null)
        commits.SetSecuredObject<GitCommitRef>(securedObject);
      this.Push?.SetSecuredObject(securedObject);
    }
  }
}
