// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitCommitRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (GitCommit))]
  public class GitCommitRef : VersionControlSecuredObject
  {
    [DataMember(Name = "commitId", EmitDefaultValue = false)]
    public string CommitId { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public GitUserDate Author { get; set; }

    [DataMember(Name = "committer", EmitDefaultValue = false)]
    public GitUserDate Committer { get; set; }

    [DataMember(Name = "comment", EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(Name = "commentTruncated", EmitDefaultValue = false)]
    public bool CommentTruncated { get; set; }

    [DataMember(Name = "changeCounts", EmitDefaultValue = false)]
    public ChangeCountDictionary ChangeCounts { get; set; }

    [DataMember(Name = "changes", EmitDefaultValue = false)]
    public IEnumerable<GitChange> Changes { get; set; }

    [DataMember(Name = "parents", EmitDefaultValue = false)]
    public IEnumerable<string> Parents { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "remoteUrl", EmitDefaultValue = false)]
    public string RemoteUrl { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(Name = "statuses", EmitDefaultValue = false)]
    public IList<GitStatus> Statuses { get; set; }

    [DataMember(Name = "workItems", EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IList<ResourceRef> WorkItems { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitPushRef Push { get; set; }

    [DataMember(Name = "commitTooManyChanges", EmitDefaultValue = false)]
    public bool CommitTooManyChanges { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Author?.SetSecuredObject(securedObject);
      this.Committer?.SetSecuredObject(securedObject);
      IEnumerable<GitChange> changes = this.Changes;
      if (changes != null)
        changes.SetSecuredObject<GitChange>(securedObject);
      IList<GitStatus> statuses = this.Statuses;
      if (statuses != null)
        statuses.SetSecuredObject<GitStatus>(securedObject);
      this.Push?.SetSecuredObject(securedObject);
      if (this.WorkItems == null)
        return;
      foreach (ResourceRef workItem in (IEnumerable<ResourceRef>) this.WorkItems)
        workItem?.SetSecuredObject(securedObject);
    }
  }
}
