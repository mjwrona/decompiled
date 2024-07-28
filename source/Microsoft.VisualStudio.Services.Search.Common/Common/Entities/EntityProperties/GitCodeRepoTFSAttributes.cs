// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.GitCodeRepoTFSAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (TFSEntityAttributes))]
  public class GitCodeRepoTFSAttributes : CodeRepoTFSAttributes
  {
    [DataMember(Order = 3)]
    public List<string> Branches { get; set; }

    [DataMember(Order = 4)]
    public string RemoteUrl { get; set; }

    [DataMember(Order = 4)]
    public string DefaultBranch { get; set; }

    public List<string> BranchesToIndex
    {
      get
      {
        HashSet<string> branchesToIndex = new HashSet<string>();
        if (!string.IsNullOrWhiteSpace(this.DefaultBranch))
          branchesToIndex.Add(this.DefaultBranch);
        if (this.Branches != null)
          this.Branches.ForEach((Action<string>) (x => branchesToIndex.Add(x)));
        branchesToIndex.RemoveWhere((Predicate<string>) (x => string.IsNullOrWhiteSpace(x)));
        return branchesToIndex.ToList<string>();
      }
    }

    public bool IdentifyBranchChanges(List<string> searchConfiguredBranches)
    {
      if (this.Branches == null)
        this.Branches = new List<string>();
      if (searchConfiguredBranches == null)
        searchConfiguredBranches = new List<string>();
      return this.Branches.Count != searchConfiguredBranches.Count || this.Branches.Except<string>((IEnumerable<string>) searchConfiguredBranches).Any<string>();
    }

    public List<string> GetBranchChanges(List<string> searchConfiguredBranches)
    {
      if (this.Branches == null)
        this.Branches = new List<string>();
      if (searchConfiguredBranches == null)
        searchConfiguredBranches = new List<string>();
      return searchConfiguredBranches.Except<string>((IEnumerable<string>) this.Branches).ToList<string>();
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext ctx)
    {
      if (this.Branches == null)
        this.Branches = new List<string>();
      if (!string.IsNullOrWhiteSpace(this.DefaultBranch))
        return;
      this.DefaultBranch = string.Empty;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext ctx)
    {
      if (!string.IsNullOrWhiteSpace(this.DefaultBranch))
        return;
      this.DefaultBranch = string.Empty;
    }
  }
}
