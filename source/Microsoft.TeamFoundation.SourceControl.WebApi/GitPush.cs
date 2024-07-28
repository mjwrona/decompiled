// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPush
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPush : GitPushRef
  {
    [DataMember(Name = "commits", EmitDefaultValue = false)]
    public IEnumerable<GitCommitRef> Commits { get; set; }

    [DataMember(Name = "refUpdates", EmitDefaultValue = false)]
    public IEnumerable<GitRefUpdate> RefUpdates { get; set; }

    [DataMember(Name = "repository", EmitDefaultValue = false)]
    public GitRepository Repository { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Repository?.SetSecuredObject(securedObject);
      IEnumerable<GitCommitRef> commits = this.Commits;
      if (commits != null)
        commits.SetSecuredObject<GitCommitRef>(securedObject);
      IEnumerable<GitRefUpdate> refUpdates = this.RefUpdates;
      if (refUpdates == null)
        return;
      refUpdates.SetSecuredObject<GitRefUpdate>(securedObject);
    }
  }
}
