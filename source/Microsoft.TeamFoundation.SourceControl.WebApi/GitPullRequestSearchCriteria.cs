// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequestSearchCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class GitPullRequestSearchCriteria
  {
    [DataMember(Name = "repositoryId", EmitDefaultValue = false)]
    public Guid? RepositoryId { get; set; }

    [DataMember(Name = "creatorId", EmitDefaultValue = false)]
    public Guid? CreatorId { get; set; }

    [DataMember(Name = "reviewerId", EmitDefaultValue = false)]
    public Guid? ReviewerId { get; set; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public PullRequestStatus? Status { get; set; }

    [DataMember(Name = "targetRefName", EmitDefaultValue = false)]
    public string TargetRefName { get; set; }

    [DataMember(Name = "sourceRepositoryId", EmitDefaultValue = false)]
    public Guid? SourceRepositoryId { get; set; }

    [DataMember(Name = "sourceRefName", EmitDefaultValue = false)]
    public string SourceRefName { get; set; }

    [DataMember(Name = "includeLinks", EmitDefaultValue = false)]
    public bool IncludeLinks { get; set; }

    [DataMember(Name = "minTime", EmitDefaultValue = false)]
    public DateTime? MinTime { get; set; }

    [DataMember(Name = "maxTime", EmitDefaultValue = false)]
    public DateTime? MaxTime { get; set; }

    [DataMember(Name = "queryTimeRangeType", EmitDefaultValue = false)]
    public PullRequestTimeRangeType? QueryTimeRangeType { get; set; }
  }
}
