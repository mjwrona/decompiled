// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component76
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component76 : Build2Component75
  {
    public override async Task<IList<BranchesViewItem>> GetBranchesViewAsync(
      Guid projectId,
      int definitionId,
      int repositoryId,
      string defaultBranch,
      DateTime minQueueTime,
      int maxBranches,
      int buildsPerBranch)
    {
      Build2Component76 build2Component76 = this;
      IList<BranchesViewItem> branchesViewAsync;
      using (build2Component76.TraceScope(method: nameof (GetBranchesViewAsync)))
      {
        build2Component76.PrepareStoredProcedure("Build.prc_GetBranchesView");
        build2Component76.BindInt("@dataspaceId", build2Component76.GetDataspaceId(projectId));
        build2Component76.BindInt("@definitionId", definitionId);
        build2Component76.BindInt("@repositoryId", repositoryId);
        build2Component76.BindString("@defaultBranch", defaultBranch, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        build2Component76.BindDateTime2("@minQueueTime", minQueueTime);
        build2Component76.BindInt("@maxBranches", maxBranches);
        build2Component76.BindInt("@buildsPerBranch", buildsPerBranch);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component76.ExecuteReaderAsync(), build2Component76.ProcedureName, build2Component76.RequestContext))
        {
          resultCollection.AddBinder<BranchesViewItem>((ObjectBinder<BranchesViewItem>) new BranchesViewItemBinder());
          resultCollection.AddBinder<BranchesViewItemRepositoryBranch>((ObjectBinder<BranchesViewItemRepositoryBranch>) new BranchesViewItemRepositoryBranchBinder());
          List<BranchesViewItem> list = resultCollection.GetCurrent<BranchesViewItem>().Items.Distinct<BranchesViewItem>((IEqualityComparer<BranchesViewItem>) new BranchViewItemComparer()).ToList<BranchesViewItem>();
          Dictionary<int, BranchesViewItem> dictionary = list.ToDictionary<BranchesViewItem, int, BranchesViewItem>((System.Func<BranchesViewItem, int>) (b => b.BuildId), (System.Func<BranchesViewItem, BranchesViewItem>) (b => b));
          resultCollection.NextResult();
          foreach (BranchesViewItemRepositoryBranch repositoryBranch in resultCollection.GetCurrent<BranchesViewItemRepositoryBranch>().Items)
          {
            BranchesViewItem branchesViewItem;
            if (dictionary.TryGetValue(repositoryBranch.BuildId, out branchesViewItem))
              branchesViewItem.RepositoryBranches.Add(repositoryBranch);
          }
          branchesViewAsync = (IList<BranchesViewItem>) list;
        }
      }
      return branchesViewAsync;
    }
  }
}
