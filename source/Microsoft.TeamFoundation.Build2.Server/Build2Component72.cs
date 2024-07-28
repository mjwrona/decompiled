// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component72
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component72 : Build2Component71
  {
    protected static readonly SqlMetaData[] typ_RepositoryAddTable = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("RepoType", SqlDbType.NVarChar, 40L),
      new SqlMetaData("RepoIdentifier", SqlDbType.NVarChar, 400L),
      new SqlMetaData("DefaultBranch", SqlDbType.NVarChar, 400L)
    };

    public override async Task<IReadOnlyList<RetentionLease>> GetRetentionLeasesForRuns(
      Guid projectId,
      HashSet<int> runIds)
    {
      Build2Component72 component = this;
      IReadOnlyList<RetentionLease> retentionLeasesForRuns;
      using (component.TraceScope(method: nameof (GetRetentionLeasesForRuns)))
      {
        component.PrepareStoredProcedure("Build.prc_GetRetentionLeasesForRuns");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindInt32Table("@runIds", (IEnumerable<int>) runIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<RetentionLease>(component.GetRetentionLeaseBinder());
          retentionLeasesForRuns = (IReadOnlyList<RetentionLease>) resultCollection.GetCurrent<RetentionLease>().Items.AsReadOnly();
        }
      }
      return retentionLeasesForRuns;
    }

    public override void UpdateDefinitionRepositoryMappings(
      Dictionary<BuildDefinition, List<BuildRepository>> definitionRepositoryMaps,
      out List<BuildDefinitionRepositoryMap> deletedDefRepoPairs,
      out List<BuildDefinitionRepositoryMap> addedDefRepoPairs)
    {
      using (this.TraceScope(method: nameof (UpdateDefinitionRepositoryMappings)))
      {
        this.PrepareStoredProcedure("Build.prc_UpdateDefinitionRepositoryMappings");
        this.BindDefinitionRepositoryMapTable("@defRepoMapTable", definitionRepositoryMaps);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildDefinitionRepositoryMap>(this.GetBuildDefinitionRepositoryBinder());
          resultCollection.AddBinder<BuildDefinitionRepositoryMap>(this.GetBuildDefinitionRepositoryBinder());
          deletedDefRepoPairs = resultCollection.GetCurrent<BuildDefinitionRepositoryMap>().Items.ToList<BuildDefinitionRepositoryMap>();
          resultCollection.NextResult();
          addedDefRepoPairs = resultCollection.GetCurrent<BuildDefinitionRepositoryMap>().Items.ToList<BuildDefinitionRepositoryMap>();
        }
      }
    }

    protected virtual ObjectBinder<BuildDefinitionRepositoryMap> GetBuildDefinitionRepositoryBinder() => (ObjectBinder<BuildDefinitionRepositoryMap>) new BuildDefinitionRepositoryBinder(this.RequestContext, (BuildSqlComponentBase) this);

    protected void BindDefinitionRepositoryMapTable(
      string parameterName,
      Dictionary<BuildDefinition, List<BuildRepository>> definitionRepositoryMaps)
    {
      List<KeyValuePair<BuildDefinition, BuildRepository>> source = new List<KeyValuePair<BuildDefinition, BuildRepository>>();
      foreach (KeyValuePair<BuildDefinition, List<BuildRepository>> definitionRepositoryMap in definitionRepositoryMaps)
      {
        foreach (BuildRepository buildRepository in definitionRepositoryMap.Value)
          source.Add(new KeyValuePair<BuildDefinition, BuildRepository>(definitionRepositoryMap.Key, buildRepository));
      }
      System.Func<KeyValuePair<BuildDefinition, BuildRepository>, SqlDataRecord> selector = (System.Func<KeyValuePair<BuildDefinition, BuildRepository>, SqlDataRecord>) (pair =>
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_DefinitionRepositoryMapTable);
        int id = pair.Key.Id;
        this.GetDataspaceId(pair.Key.ProjectId);
        record.SetInt32(0, this.GetDataspaceId(pair.Key.ProjectId));
        record.SetInt32(1, pair.Key.Id);
        record.SetString(2, pair.Value.Type, BindStringBehavior.Unchanged);
        record.SetString(3, pair.Value.Id, BindStringBehavior.Unchanged);
        record.SetString(4, pair.Value.DefaultBranch, BindStringBehavior.Unchanged);
        return record;
      });
      this.BindTable(parameterName, "Build.typ_DefinitionRepositoryMapTable", source.Select<KeyValuePair<BuildDefinition, BuildRepository>, SqlDataRecord>(selector));
    }
  }
}
