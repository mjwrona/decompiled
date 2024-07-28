// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.FileSummaryResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Server.Contracts;
using Microsoft.TeamFoundation.Core.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class FileSummaryResult
  {
    public FileSummaryResult()
      : this((IList<BranchSummaryResult>) new List<BranchSummaryResult>())
    {
    }

    public FileSummaryResult(IList<BranchSummaryResult> branchList)
    {
      this.ResourceVersion = "2.0";
      this.BranchList = branchList;
    }

    [JsonProperty]
    public string ResourceVersion { get; private set; }

    [JsonProperty]
    public IList<BranchSummaryResult> BranchList { get; private set; }

    public void Merge(FileSummaryResult otherResult)
    {
      foreach (BranchSummaryResult branch in (IEnumerable<BranchSummaryResult>) otherResult.BranchList)
      {
        BranchSummaryResult otherBranchListEntry = branch;
        BranchSummaryResult branchSummaryResult = this.BranchList.SingleOrDefault<BranchSummaryResult>((Func<BranchSummaryResult, bool>) (entry => TFStringComparer.VersionControlPath.Equals(entry.ServerPath, otherBranchListEntry.ServerPath)));
        if (branchSummaryResult != null)
          this.BranchList.Remove(branchSummaryResult);
        this.BranchList.Add(otherBranchListEntry);
      }
    }

    public void MergeSummaryResult(string serverPath, BranchSummaryResult newSummary)
    {
      BranchSummaryResult branchSummaryResult = this.BranchList.SingleOrDefault<BranchSummaryResult>((Func<BranchSummaryResult, bool>) (branch => TFStringComparer.VersionControlPath.Equals(branch.ServerPath, serverPath)));
      if (branchSummaryResult != null)
        this.BranchList.Remove(branchSummaryResult);
      this.BranchList.Add(newSummary);
    }
  }
}
