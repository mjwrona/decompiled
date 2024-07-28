// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component67
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
  internal class Build2Component67 : Build2Component66
  {
    public override async Task<IList<RepositoryBranchReferences>> GetBranchesByNameForDefinition(
      Guid projectId,
      int maxCount,
      int definitionId,
      string nameLike,
      HashSet<int> excludedBranchIds)
    {
      Build2Component67 component = this;
      IList<RepositoryBranchReferences> branchReferences;
      using (component.TraceScope(method: nameof (GetBranchesByNameForDefinition)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBranchesByNameForDefinition");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@definitionId", definitionId);
        component.BindInt("@maxCount", maxCount);
        component.BindString("@nameLike", nameLike, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindUniqueInt32Table("@excludedBranchIds", (IEnumerable<int>) excludedBranchIds);
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<RepositoryReference>(component.GetRepositoryReferenceBinder());
          rc.AddBinder<BranchReference>(component.GetBranchReferenceBinder());
          branchReferences = (IList<RepositoryBranchReferences>) component.GetRepositoryBranchReferences(rc);
        }
      }
      return branchReferences;
    }
  }
}
