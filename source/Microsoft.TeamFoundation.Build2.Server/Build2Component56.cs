// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component56
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component56 : Build2Component55
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
      Build2Component56 build2Component56 = this;
      IList<BranchesViewItem> branchesViewAsync;
      using (build2Component56.TraceScope(method: nameof (GetBranchesViewAsync)))
      {
        build2Component56.PrepareStoredProcedure("Build.prc_GetBranchesView2");
        build2Component56.BindInt("@dataspaceId", build2Component56.GetDataspaceId(projectId));
        build2Component56.BindInt("@definitionId", definitionId);
        build2Component56.BindString("@defaultBranch", defaultBranch, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        build2Component56.BindDateTime2("@minQueueTime", minQueueTime);
        build2Component56.BindInt("@maxBranches", maxBranches);
        build2Component56.BindInt("@buildsPerBranch", buildsPerBranch);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component56.ExecuteReaderAsync(), build2Component56.ProcedureName, build2Component56.RequestContext))
        {
          resultCollection.AddBinder<BranchesViewItem>((ObjectBinder<BranchesViewItem>) new BranchesViewItemBinder());
          resultCollection.AddBinder<BranchesViewItem>((ObjectBinder<BranchesViewItem>) new BranchesViewItemBinder());
          List<BranchesViewItem> branchesViewItemList = new List<BranchesViewItem>();
          branchesViewItemList.AddRange((IEnumerable<BranchesViewItem>) resultCollection.GetCurrent<BranchesViewItem>().Items);
          branchesViewItemList.AddRange((IEnumerable<BranchesViewItem>) resultCollection.GetCurrent<BranchesViewItem>().Items);
          branchesViewAsync = (IList<BranchesViewItem>) branchesViewItemList;
        }
      }
      return branchesViewAsync;
    }
  }
}
