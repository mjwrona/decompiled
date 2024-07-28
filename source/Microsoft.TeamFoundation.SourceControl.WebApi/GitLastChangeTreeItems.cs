// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitLastChangeTreeItems
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
  public class GitLastChangeTreeItems : VersionControlSecuredObject
  {
    [DataMember(Name = "items", EmitDefaultValue = false)]
    public List<GitLastChangeItem> Items { get; set; }

    [DataMember(Name = "lastExploredTime", EmitDefaultValue = false)]
    public DateTime? LastExploredTime { get; set; }

    [DataMember(Name = "commits", EmitDefaultValue = false)]
    public List<GitCommitRef> Commits { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      List<GitLastChangeItem> items = this.Items;
      if (items != null)
        items.SetSecuredObject<GitLastChangeItem>(securedObject);
      List<GitCommitRef> commits = this.Commits;
      if (commits == null)
        return;
      commits.SetSecuredObject<GitCommitRef>(securedObject);
    }
  }
}
