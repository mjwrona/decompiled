// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component77
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
  internal class Build2Component77 : Build2Component76
  {
    public override async Task<RetentionLease> DeleteRetentionLease(
      Guid projectId,
      string owner,
      int runId,
      int definitionId)
    {
      Build2Component77 build2Component77 = this;
      RetentionLease retentionLease;
      using (build2Component77.TraceScope(method: nameof (DeleteRetentionLease)))
      {
        build2Component77.PrepareStoredProcedure("Build.prc_DeleteRetentionLease");
        build2Component77.BindInt("@dataspaceId", build2Component77.GetDataspaceId(projectId));
        build2Component77.BindString("@ownerId", owner, 400, false, SqlDbType.NVarChar);
        build2Component77.BindInt("@runId", runId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component77.ExecuteReaderAsync(), build2Component77.ProcedureName, build2Component77.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component77.GetRetentionLeaseBinder());
          retentionLease = resultCollection.GetCurrent<RetentionLease>().Items.SingleOrDefault<RetentionLease>();
        }
      }
      return retentionLease;
    }

    public override async Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeases(
      Guid projectId,
      HashSet<int> leaseIds)
    {
      Build2Component77 component = this;
      IReadOnlyList<RetentionLease> retentionLeaseList;
      using (component.TraceScope(method: nameof (DeleteRetentionLeases)))
      {
        component.PrepareStoredProcedure("Build.prc_DeleteRetentionLeases");
        component.BindInt("@dataspaceID", component.GetDataspaceId(projectId));
        component.BindUniqueInt32Table("@leaseIds", (IEnumerable<int>) leaseIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          retentionLeaseList = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeaseList;
    }

    public override async Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeasesByOwnerPrefix(
      Guid projectId,
      string ownerPrefix)
    {
      Build2Component77 build2Component77 = this;
      IReadOnlyList<RetentionLease> retentionLeaseList;
      using (build2Component77.TraceScope(method: nameof (DeleteRetentionLeasesByOwnerPrefix)))
      {
        build2Component77.PrepareStoredProcedure("Build.prc_DeleteRetentionLeasesByOwnerPrefix");
        build2Component77.BindInt("@dataspaceId", build2Component77.GetDataspaceId(projectId));
        build2Component77.BindString("@ownerPrefix", ownerPrefix, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component77.ExecuteReaderAsync(), build2Component77.ProcedureName, build2Component77.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component77.GetRetentionLeaseBinder());
          retentionLeaseList = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeaseList;
    }

    public override async Task<IList<RepositoryBranchReferences>> GetBranchesByIdAsync(
      Guid projectId,
      List<int> branchIds)
    {
      Build2Component77 component = this;
      IList<RepositoryBranchReferences> branchReferences;
      using (component.TraceScope(method: nameof (GetBranchesByIdAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_GetBranchesById");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindUniqueInt32Table("@branchIds", (IEnumerable<int>) branchIds);
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
