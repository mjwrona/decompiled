// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component68
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
  internal class Build2Component68 : Build2Component67
  {
    public override async Task<RetentionLease> AddRetentionLease(
      Guid projectId,
      string owner,
      int buildId,
      int definitionId,
      DateTime validUntil,
      bool highPriority,
      int maxLeases)
    {
      Build2Component68 build2Component68 = this;
      RetentionLease retentionLeaseById;
      using (build2Component68.TraceScope(method: nameof (AddRetentionLease)))
      {
        build2Component68.PrepareStoredProcedure("Build.prc_AddRetentionLease");
        build2Component68.BindInt("@dataspaceId", build2Component68.GetDataspaceId(projectId));
        build2Component68.BindString("@ownerId", owner, 400, false, SqlDbType.NVarChar);
        build2Component68.BindInt("@runId", buildId);
        build2Component68.BindInt("@definitionId", definitionId);
        build2Component68.BindDate("@validUntil", validUntil);
        build2Component68.BindBoolean("@highPriority", highPriority);
        build2Component68.BindInt("@maxLeases", maxLeases);
        int leaseId = (int) await build2Component68.ExecuteNonQueryAsync(true);
        retentionLeaseById = await build2Component68.GetRetentionLeaseById(projectId, leaseId);
      }
      return retentionLeaseById;
    }

    public override async Task<RetentionLease> DeleteRetentionLease(
      Guid projectId,
      string owner,
      int runId,
      int definitionId)
    {
      Build2Component68 build2Component68 = this;
      IReadOnlyList<RetentionLease> lease = await build2Component68.GetRetentionLeases(projectId, (IEnumerable<MinimalRetentionLease>) new MinimalRetentionLease[1]
      {
        new MinimalRetentionLease(owner, new int?(runId), new int?(definitionId))
      });
      using (build2Component68.TraceScope(method: nameof (DeleteRetentionLease)))
      {
        build2Component68.PrepareStoredProcedure("Build.prc_DeleteRetentionLease");
        build2Component68.BindInt("@dataspaceId", build2Component68.GetDataspaceId(projectId));
        build2Component68.BindString("@ownerId", owner, 400, false, SqlDbType.NVarChar);
        build2Component68.BindInt("@runId", runId);
        int num = await build2Component68.ExecuteNonQueryAsync();
      }
      RetentionLease retentionLease = lease.SingleOrDefault<RetentionLease>();
      lease = (IReadOnlyList<RetentionLease>) null;
      return retentionLease;
    }

    public override async Task<IList<RepositoryBranchReferences>> GetRecentlyBuiltBranchesForRepositories(
      Guid projectId,
      int maxBranches,
      IEnumerable<string> repositoryIdentifiers,
      HashSet<int> excludedRepositoryIds)
    {
      Build2Component68 component = this;
      IList<RepositoryBranchReferences> branchReferences;
      using (component.TraceScope(method: nameof (GetRecentlyBuiltBranchesForRepositories)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltBranchesForRepositories");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@topBranches", maxBranches);
        component.BindStringTable("@repositoryIdentifiers", repositoryIdentifiers);
        component.BindUniqueInt32Table("@excludedRepositoryIds", (IEnumerable<int>) excludedRepositoryIds);
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<RepositoryReference>(component.GetRepositoryReferenceBinder());
          rc.AddBinder<BranchReference>(component.GetBranchReferenceBinder());
          branchReferences = (IList<RepositoryBranchReferences>) component.GetRepositoryBranchReferences(rc);
        }
      }
      return branchReferences;
    }

    public override async Task<int> DeleteDefinitionCronSchedules(BuildDefinition definition)
    {
      Build2Component68 build2Component68 = this;
      int num;
      using (build2Component68.TraceScope(method: nameof (DeleteDefinitionCronSchedules)))
      {
        build2Component68.PrepareStoredProcedure("Build.prc_DeleteDefinitionCronSchedules");
        build2Component68.BindInt("@dataspaceId", build2Component68.GetDataspaceId(definition.ProjectId));
        build2Component68.BindInt("@definitionId", definition.Id);
        num = (int) await build2Component68.ExecuteScalarAsync();
      }
      return num;
    }

    public override async Task<RetentionLease> GetRetentionLeaseById(Guid projectId, int leaseId)
    {
      Build2Component68 build2Component68 = this;
      RetentionLease retentionLeaseById;
      using (build2Component68.TraceScope(method: nameof (GetRetentionLeaseById)))
      {
        build2Component68.PrepareStoredProcedure("Build.prc_GetRetentionLeaseById");
        build2Component68.BindInt("@dataspaceId", build2Component68.GetDataspaceId(projectId));
        build2Component68.BindInt("@leaseId", leaseId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component68.ExecuteReaderAsync(), build2Component68.ProcedureName, build2Component68.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(build2Component68.GetRetentionLeaseBinder());
          retentionLeaseById = resultCollection.GetCurrent<RetentionLease>().SingleOrDefault<RetentionLease>();
        }
      }
      return retentionLeaseById;
    }

    protected virtual ObjectBinder<RetentionLease> GetRetentionLeaseBinder() => (ObjectBinder<RetentionLease>) new RetentionLeaseBinder(this.RequestContext, (BuildSqlComponentBase) this);
  }
}
