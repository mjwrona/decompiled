// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component64
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component64 : Build2Component63
  {
    public override List<BuildDefinition> GetRecentlyBuiltDefinitions(
      Guid projectId,
      int top,
      bool includeQueuedBuilds)
    {
      using (this.TraceScope(method: nameof (GetRecentlyBuiltDefinitions)))
      {
        this.PrepareStoredProcedure("Build.prc_GetRecentlyBuiltDefinitions");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@top", top);
        this.BindBoolean("@includeQueuedBuilds", includeQueuedBuilds);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
          rc.AddBinder<BuildData>(this.GetBuildDataBinder());
          rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
          rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
          List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
          Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
          this.BindLatestBuilds(rc, dictionary);
          return items;
        }
      }
    }

    public override int GetScheduledBuildsCount(
      Guid projectId,
      IEnumerable<int> definitionIds,
      IList<int> excludedDefinitionIds,
      bool includeDeleted,
      DateTime minQueueTime,
      DateTime? maxQueueTime,
      BuildStatus? statusFilter,
      BuildResult? resultFilter)
    {
      using (this.TraceScope(method: nameof (GetScheduledBuildsCount)))
      {
        this.TraceEnter(0, nameof (GetScheduledBuildsCount));
        this.PrepareStoredProcedure("Build.prc_GetScheduledBuildsCount");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindUniqueInt32Table("@definitionIdTable", definitionIds);
        this.BindUniqueInt32Table("@excludeDefinitionIdTable", (IEnumerable<int>) excludedDefinitionIds);
        this.BindBoolean("@includeDeleted", includeDeleted);
        this.BindNullableUtcDateTime2("@minQueueTime", new DateTime?(minQueueTime));
        this.BindNullableUtcDateTime2("@maxQueueTime", maxQueueTime);
        BuildStatus? nullable1 = statusFilter;
        this.BindNullableByte("@statusFilter", nullable1.HasValue ? new byte?((byte) nullable1.GetValueOrDefault()) : new byte?());
        BuildResult? nullable2 = resultFilter;
        this.BindNullableByte("@resultFilter", nullable2.HasValue ? new byte?((byte) nullable2.GetValueOrDefault()) : new byte?());
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          return resultCollection.GetCurrent<int>().First<int>();
        }
      }
    }

    public override BuildArtifact AddArtifact(Guid projectId, int buildId, BuildArtifact artifact)
    {
      using (this.TraceScope(method: nameof (AddArtifact)))
      {
        this.PrepareStoredProcedure("Build.prc_AddArtifact");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
        this.BindInt("@buildId", buildId);
        this.BindString("@artifactName", artifact.Name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@artifactType", artifact.Resource.Type, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@artifactSource", artifact.Source, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@data", artifact.Resource.Data, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        string parameterValue = "";
        if (artifact.Resource.Properties != null)
        {
          try
          {
            parameterValue = JsonUtility.ToString((object) artifact.Resource.Properties);
          }
          catch (JsonSerializationException ex)
          {
            parameterValue = "";
          }
        }
        this.BindString("@metadata", parameterValue, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildArtifact>(this.GetBuildArtifactBinder());
          return resultCollection.GetCurrent<BuildArtifact>().FirstOrDefault<BuildArtifact>();
        }
      }
    }

    public override async Task<IList<BuildArtifact>> GetArtifactsBySourceAsync(
      Guid projectId,
      int buildId,
      string source)
    {
      Build2Component64 build2Component64 = this;
      IList<BuildArtifact> uniqueArtifacts;
      using (build2Component64.TraceScope(method: nameof (GetArtifactsBySourceAsync)))
      {
        build2Component64.PrepareStoredProcedure("Build.prc_GetArtifactsBySource");
        build2Component64.BindInt("@dataspaceId", build2Component64.GetDataspaceId(projectId));
        build2Component64.BindInt("@buildId", buildId);
        build2Component64.BindString("@source", source, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component64.ExecuteReaderAsync(), build2Component64.ProcedureName, build2Component64.RequestContext))
        {
          resultCollection.AddBinder<BuildArtifact>(build2Component64.GetBuildArtifactBinder());
          uniqueArtifacts = build2Component64.GetUniqueArtifacts((IList<BuildArtifact>) resultCollection.GetCurrent<BuildArtifact>().Items);
        }
      }
      return uniqueArtifacts;
    }

    protected override ObjectBinder<BuildArtifact> GetBuildArtifactBinder() => (ObjectBinder<BuildArtifact>) new BuildArtifactBinder5(this.RequestContext);

    public override async Task UpdateDefinitionBuildOptionsAsync(BuildDefinition definition)
    {
      Build2Component64 build2Component64 = this;
      using (build2Component64.TraceScope(method: nameof (UpdateDefinitionBuildOptionsAsync)))
      {
        build2Component64.PrepareStoredProcedure("Build.prc_UpdateDefinitionBuildOptions");
        build2Component64.BindInt("@dataspaceId", build2Component64.GetDataspaceId(definition.ProjectId));
        build2Component64.BindInt("@definitionId", definition.Id);
        build2Component64.BindInt("@definitionVersion", definition.Revision.Value);
        build2Component64.BindString("@options", build2Component64.ToString<List<BuildOption>>(definition.Options), -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        int num = await build2Component64.ExecuteNonQueryAsync();
      }
    }
  }
}
