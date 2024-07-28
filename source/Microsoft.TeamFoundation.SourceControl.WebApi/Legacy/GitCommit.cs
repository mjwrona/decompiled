// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.GitCommit
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class GitCommit : ChangeList
  {
    [DataMember(Name = "commitId", EmitDefaultValue = false)]
    public GitObjectId CommitId { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public GitIdentityReference Author { get; set; }

    [DataMember(Name = "committer", EmitDefaultValue = false)]
    public GitIdentityReference Committer { get; set; }

    [DataMember(Name = "parents", EmitDefaultValue = false)]
    public IEnumerable<GitObjectReference> Parents { get; set; }

    [DataMember(Name = "tree", EmitDefaultValue = false)]
    public GitItem Tree { get; set; }

    [DataMember(Name = "commitTime", EmitDefaultValue = false)]
    public DateTime CommitTime { get; set; }

    [DataMember(Name = "pusher", EmitDefaultValue = false)]
    public string PushedByDisplayName { get; set; }

    [DataMember(Name = "pushTime", EmitDefaultValue = false)]
    public DateTime PushTime { get; set; }

    [DataMember(Name = "pushId", EmitDefaultValue = false)]
    public int PushId { get; set; }

    [Obsolete("This is unused as of Dev15 M108 and may be deleted in the future")]
    [DataMember(Name = "pushCorrelationId", EmitDefaultValue = false)]
    public Guid PushCorrelationId { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.CommitId?.SetSecuredObject(securedObject);
      this.Author?.SetSecuredObject(securedObject);
      this.Committer?.SetSecuredObject(securedObject);
      this.Tree?.SetSecuredObject(securedObject);
      IEnumerable<GitObjectReference> parents = this.Parents;
      if (parents == null)
        return;
      parents.SetSecuredObject<GitObjectReference>(securedObject);
    }
  }
}
