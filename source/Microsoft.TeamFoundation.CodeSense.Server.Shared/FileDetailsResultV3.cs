// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.FileDetailsResultV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class FileDetailsResultV3
  {
    public FileDetailsResultV3()
      : this(new List<BranchDetailsResultV3>())
    {
    }

    public FileDetailsResultV3(List<BranchDetailsResultV3> branchList) => this.BranchList = branchList;

    [JsonProperty]
    public List<BranchDetailsResultV3> BranchList { get; private set; }

    public void Merge(FileDetailsResultV3 toMerge)
    {
      foreach (BranchDetailsResultV3 branch1 in toMerge.BranchList)
      {
        BranchDetailsResultV3 branch = branch1;
        this.BranchList.RemoveAll((Predicate<BranchDetailsResultV3>) (existingBranch => TFStringComparer.VersionControlPath.Equals(existingBranch.ServerPath, branch.ServerPath)));
        this.BranchList.Add(branch);
      }
    }

    public void RemoveRestrictedBranches(IVssRequestContext requestContext)
    {
      Dictionary<string, bool> permissions = requestContext.GetService<TeamFoundationSecurityServiceProxy>().ObtainPermissions(requestContext, SecurityConstants.RepositorySecurity2NamespaceGuid, this.BranchList.Select<BranchDetailsResultV3, string>((Func<BranchDetailsResultV3, string>) (item => item.ServerPath)), 1);
      if (permissions == null || !permissions.Any<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (tokenPermissionsPair => !tokenPermissionsPair.Value)))
        return;
      this.BranchList = this.BranchList.Where<BranchDetailsResultV3>((Func<BranchDetailsResultV3, bool>) (branch => permissions.Single<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (permissionedItem => TFStringComparer.VersionControlPath.Equals(branch.ServerPath, permissionedItem.Key))).Value)).ToList<BranchDetailsResultV3>();
    }

    public void MergeCodeElements(
      string serverPath,
      IEnumerable<CodeElementDetailsResultV3> codeElements)
    {
      List<BranchDetailsResultV3> list = this.BranchList.Where<BranchDetailsResultV3>((Func<BranchDetailsResultV3, bool>) (branch => TFStringComparer.VersionControlPath.Equals(branch.ServerPath, serverPath))).ToList<BranchDetailsResultV3>();
      if (!list.Any<BranchDetailsResultV3>())
      {
        this.BranchList.Add(new BranchDetailsResultV3(serverPath, codeElements.ToList<CodeElementDetailsResultV3>()));
      }
      else
      {
        BranchDetailsResultV3 branchListEntry = list.Last<BranchDetailsResultV3>();
        FileDetailsResultV3.MergeCodeElements(branchListEntry, codeElements);
        list.Remove(branchListEntry);
        foreach (BranchDetailsResultV3 branchDetailsResultV3 in list)
        {
          FileDetailsResultV3.MergeCodeElements(branchListEntry, (IEnumerable<CodeElementDetailsResultV3>) branchDetailsResultV3.Details);
          this.BranchList.Remove(branchDetailsResultV3);
        }
      }
    }

    private static void MergeCodeElements(
      BranchDetailsResultV3 branchListEntry,
      IEnumerable<CodeElementDetailsResultV3> codeElements)
    {
      foreach (CodeElementDetailsResultV3 codeElement1 in codeElements)
      {
        CodeElementDetailsResultV3 codeElement = codeElement1;
        CodeElementDetailsResultV3 elementDetailsResultV3 = branchListEntry.Details.FirstOrDefault<CodeElementDetailsResultV3>((Func<CodeElementDetailsResultV3, bool>) (e => e.Id == codeElement.Id));
        if (elementDetailsResultV3 != null)
        {
          CodeElementChangeResultV3 data1 = codeElement.ElementDetails.First<CollectorResult>().GetData<CodeElementChangeResultV3>();
          CollectorResult collectorResult = elementDetailsResultV3.ElementDetails.First<CollectorResult>();
          CodeElementChangeResultV3 data2 = collectorResult.GetData<CodeElementChangeResultV3>();
          data2.Merge(data1);
          collectorResult.Data = (object) data2;
        }
        else
          branchListEntry.Details.Add(codeElement);
      }
    }
  }
}
