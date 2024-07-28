// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.CustomRepoCodeTFSAttributes
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [DataContract]
  [Export(typeof (TFSEntityAttributes))]
  public class CustomRepoCodeTFSAttributes : TFSEntityAttributes
  {
    [DataMember(Order = 0)]
    public string ProjectName { get; set; }

    [DataMember(Order = 1)]
    public string RepositoryName { get; set; }

    [DataMember(Order = 2)]
    public IEnumerable<SDBranchDetail> Branches { get; set; }

    [DataMember(Order = 3)]
    public string DefaultBranch { get; set; }

    [DataMember(Order = 4)]
    public string ScopePath { get; set; }

    public List<string> BranchesToIndex
    {
      get
      {
        HashSet<string> branchesToIndex = new HashSet<string>();
        if (!string.IsNullOrWhiteSpace(this.DefaultBranch))
          branchesToIndex.Add(this.DefaultBranch);
        if (this.Branches != null)
          this.Branches.ToList<SDBranchDetail>().ForEach((Action<SDBranchDetail>) (x => branchesToIndex.Add(x.BranchName)));
        branchesToIndex.RemoveWhere((Predicate<string>) (x => string.IsNullOrWhiteSpace(x)));
        return branchesToIndex.ToList<string>();
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext ctx)
    {
      if (this.Branches == null)
        this.Branches = (IEnumerable<SDBranchDetail>) new List<SDBranchDetail>();
      if (!string.IsNullOrWhiteSpace(this.DefaultBranch) || this.Branches == null || !this.Branches.Any<SDBranchDetail>())
        return;
      this.DefaultBranch = this.Branches.First<SDBranchDetail>().BranchName;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext ctx)
    {
      if (!string.IsNullOrWhiteSpace(this.DefaultBranch) || this.Branches == null || !this.Branches.Any<SDBranchDetail>())
        return;
      this.DefaultBranch = this.Branches.First<SDBranchDetail>().BranchName;
    }
  }
}
