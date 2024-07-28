// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitCommitDiffs
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitCommitDiffs : VersionControlSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public bool AllChangesIncluded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<VersionControlChangeType, int> ChangeCounts { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<GitChange> Changes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CommonCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string BaseCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string TargetCommit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? AheadCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? BehindCount { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<GitChange> changes = this.Changes;
      if (changes == null)
        return;
      changes.SetSecuredObject<GitChange>(securedObject);
    }
  }
}
