// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component89
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
  internal class Build2Component89 : Build2Component88
  {
    public override List<BuildDefinition> GetDefinitions(
      Guid projectId,
      string name,
      DefinitionTriggerType triggerFilter,
      string repositoryId,
      string repositoryType,
      DefinitionQueryOrder queryOrder,
      int maxDefinitions,
      DateTime? minLastModifiedTime,
      DateTime? maxLastModifiedTime,
      string lastDefinitionName,
      DateTime? minMetricsTime,
      string path,
      DateTime? builtAfter,
      DateTime? notBuiltAfter,
      DefinitionQueryOptions options,
      IEnumerable<string> tagFilters,
      bool includeLatestBuilds,
      Guid? taskIdFilter = null,
      int? processType = null)
    {
      this.TraceEnter(0, nameof (GetDefinitions));
      this.PrepareStoredProcedure("Build.prc_GetDefinitions");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindString("@definitionName", DBHelper.ServerPathToDBPath(name), 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@triggerFilter", (int) triggerFilter);
      this.BindString("@repositoryId", repositoryId, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@queryOrder", (int) queryOrder);
      this.BindInt("@maxDefinitions", maxDefinitions);
      this.BindNullableUtcDateTime2("@minLastModifiedTime", minLastModifiedTime);
      this.BindNullableUtcDateTime2("@maxLastModifiedTime", maxLastModifiedTime);
      this.BindString("@lastDefinitionName", DBHelper.ServerPathToDBPath(lastDefinitionName), 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      this.BindString("@path", DBHelper.UserToDBPath(path), 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableUtcDateTime2("@builtAfter", builtAfter);
      this.BindNullableUtcDateTime2("@notBuiltAfter", notBuiltAfter);
      this.BindInt("@queryOptions", (int) options);
      this.BindStringTable("@tagFilters", tagFilters);
      this.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      this.BindNullableGuid("@taskIdFilter", taskIdFilter);
      this.BindNullableInt32("@processType", processType);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(this.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(this.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(this.GetDefinitionTagDataBinder());
        rc.AddBinder<DesignerSchedule>(this.GetDesignerScheduleBinder());
        rc.AddBinder<BuildData>(this.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(this.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(this.GetBuildOrchestrationDataBinder());
        List<BuildDefinition> items = rc.GetCurrent<BuildDefinition>().Items;
        Dictionary<int, BuildDefinition> dictionary = items.ToDictionary<BuildDefinition, int>((System.Func<BuildDefinition, int>) (x => x.Id));
        foreach (BuildDefinition buildDefinition in items.Where<BuildDefinition>((System.Func<BuildDefinition, bool>) (bd => bd.ParentDefinition != null)))
        {
          if (dictionary.ContainsKey(buildDefinition.ParentDefinition.Id))
            dictionary[buildDefinition.ParentDefinition.Id].Drafts.Add(buildDefinition);
        }
        rc.NextResult();
        foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionMetric.DefinitionId, out buildDefinition))
            buildDefinition.Metrics.Add(definitionMetric.Metric);
        }
        rc.NextResult();
        foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
        {
          BuildDefinition buildDefinition;
          if (dictionary.TryGetValue(definitionTagData.DefinitionId, out buildDefinition))
            buildDefinition.Tags.Add(definitionTagData.Tag);
        }
        this.UpdateScheduleTriggers(rc, dictionary);
        if (includeLatestBuilds)
          this.BindLatestBuilds(rc, dictionary);
        this.TraceLeave(0, nameof (GetDefinitions));
        return items;
      }
    }

    public override async Task<BuildDefinition> GetDefinitionAsync(
      Guid projectId,
      int definitionId,
      int? definitionVersion,
      bool includeDeleted = false,
      DateTime? minMetricsTime = null,
      bool includeLatestBuilds = false)
    {
      Build2Component89 build2Component89 = this;
      build2Component89.TraceEnter(0, nameof (GetDefinitionAsync));
      build2Component89.PrepareStoredProcedure("Build.prc_GetDefinition");
      build2Component89.BindInt("@dataspaceId", build2Component89.GetDataspaceId(projectId));
      build2Component89.BindInt("@definitionId", definitionId);
      build2Component89.BindNullableInt32("@definitionVersion", definitionVersion);
      build2Component89.BindBoolean("@includeDeleted", includeDeleted);
      build2Component89.BindNullableUtcDateTime2("@minMetricsTime", minMetricsTime);
      build2Component89.BindBoolean("@includeLatestBuilds", includeLatestBuilds);
      build2Component89.BindBoolean("@includeDrafts", true);
      BuildDefinition definitionAsync;
      using (ResultCollection rc = new ResultCollection((IDataReader) await build2Component89.ExecuteReaderAsync(), build2Component89.ProcedureName, build2Component89.RequestContext))
      {
        rc.AddBinder<BuildDefinition>(build2Component89.GetBuildDefinitionBinder());
        rc.AddBinder<BuildDefinitionMetric>(build2Component89.GetBuildDefinitionMetricBinder());
        rc.AddBinder<DefinitionTagData>(build2Component89.GetDefinitionTagDataBinder());
        if (!definitionVersion.HasValue)
          rc.AddBinder<DesignerSchedule>(build2Component89.GetDesignerScheduleBinder());
        rc.AddBinder<BuildDefinition>(build2Component89.GetBuildDefinitionBinder());
        rc.AddBinder<BuildData>(build2Component89.GetBuildDataBinder());
        rc.AddBinder<BuildTagData>(build2Component89.GetBuildTagDataBinder());
        rc.AddBinder<BuildOrchestrationData>(build2Component89.GetBuildOrchestrationDataBinder());
        BuildDefinition definition = rc.GetCurrent<BuildDefinition>().SingleOrDefault<BuildDefinition>();
        if (definition != null)
        {
          rc.NextResult();
          foreach (BuildDefinitionMetric definitionMetric in rc.GetCurrent<BuildDefinitionMetric>())
            definition.Metrics.Add(definitionMetric.Metric);
          rc.NextResult();
          foreach (DefinitionTagData definitionTagData in rc.GetCurrent<DefinitionTagData>())
            definition.Tags.Add(definitionTagData.Tag);
          if (!definitionVersion.HasValue)
            build2Component89.UpdateScheduleTrigger(rc, definition);
          rc.NextResult();
          foreach (BuildDefinition buildDefinition in rc.GetCurrent<BuildDefinition>())
            definition.Drafts.Add(buildDefinition);
          if (includeLatestBuilds)
            build2Component89.BindLatestBuilds(rc, definition);
        }
        build2Component89.TraceLeave(0, nameof (GetDefinitionAsync));
        definitionAsync = definition;
      }
      return definitionAsync;
    }
  }
}
