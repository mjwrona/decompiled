// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component71
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component71 : Build2Component70
  {
    public override async Task<IReadOnlyList<RetentionLease>> DeleteRetentionLeasesByOwnerPrefix(
      Guid projectId,
      string ownerPrefix)
    {
      Build2Component71 build2Component71 = this;
      using (build2Component71.TraceScope(method: nameof (DeleteRetentionLeasesByOwnerPrefix)))
      {
        build2Component71.PrepareStoredProcedure("Build.prc_DeleteRetentionLeasesByOwnerPrefix");
        build2Component71.BindInt("@dataspaceId", build2Component71.GetDataspaceId(projectId));
        build2Component71.BindString("@ownerPrefix", ownerPrefix, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        int num = await build2Component71.ExecuteNonQueryAsync();
      }
      return (IReadOnlyList<RetentionLease>) Array.Empty<RetentionLease>();
    }

    public override async Task<PurgedBuildsResults> PurgeBuildsAsync(
      Guid projectId,
      int daysOld,
      int batchSize,
      string branchPrefix)
    {
      Build2Component71 component = this;
      PurgedBuildsResults purgedBuildsResults;
      using (component.TraceScope(method: nameof (PurgeBuildsAsync)))
      {
        component.PrepareStoredProcedure("Build.prc_PurgeBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt("@daysOld", daysOld);
        component.BindInt("@batchSize", batchSize);
        component.BindString("@branchPrefix", branchPrefix, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) new HashSet<int>());
        using (ResultCollection rc = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          rc.AddBinder<BuildArtifact>(component.GetBuildArtifactBinder());
          rc.AddBinder<BuildData>(component.GetBuildDataBinder());
          rc.AddBinder<BuildOrchestrationData>(component.GetBuildOrchestrationDataBinder());
          purgedBuildsResults = new PurgedBuildsResults(rc);
        }
      }
      return purgedBuildsResults;
    }

    public override IEnumerable<BuildDefinitionBranch> UpdateDefinitionBranches(
      Guid projectId,
      int definitionId,
      int definitionVersion,
      IEnumerable<BuildDefinitionBranch> branches,
      int maxConcurrentBuildsPerBranch,
      bool ignoreSourceIdCheck)
    {
      using (this.TraceScope(method: nameof (UpdateDefinitionBranches)))
      {
        this.PrepareStoredProcedure("Build.prc_UpdateDefinitionBranches");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@definitionId", definitionId);
        this.BindInt("@definitionVersion", definitionVersion);
        this.BindBuildDefinitionBranchTable3("@branches", branches);
        this.BindInt("@maxConcurrentBuildsPerBranch", maxConcurrentBuildsPerBranch);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildDefinitionBranch>(this.GetBuildDefinitionBranchBinder());
          return (IEnumerable<BuildDefinitionBranch>) resultCollection.GetCurrent<BuildDefinitionBranch>().Items;
        }
      }
    }

    protected SqlParameter BindBuildDefinitionBranchTable3(
      string parameterName,
      IEnumerable<BuildDefinitionBranch> branches)
    {
      branches = branches ?? Enumerable.Empty<BuildDefinitionBranch>();
      System.Func<BuildDefinitionBranch, SqlDataRecord> selector = (System.Func<BuildDefinitionBranch, SqlDataRecord>) (branch =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_DefinitionBranchTable2);
        record.SetString(0, branch.BranchName, BindStringBehavior.Unchanged);
        record.SetInt64(1, branch.SourceId);
        record.SetGuid(2, branch.PendingSourceOwner);
        record.SetString(3, branch.PendingSourceVersion, BindStringBehavior.Unchanged);
        return record;
      });
      return this.BindTable(parameterName, "Build.typ_DefinitionBranchTable3", branches.Select<BuildDefinitionBranch, SqlDataRecord>(selector));
    }
  }
}
