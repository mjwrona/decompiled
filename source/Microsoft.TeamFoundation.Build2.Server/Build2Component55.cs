// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component55
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
  internal class Build2Component55 : Build2Component54
  {
    protected override SqlParameter BindChangeDataTable(
      string parameterName,
      IEnumerable<ChangeData> changes)
    {
      changes = changes ?? Enumerable.Empty<ChangeData>();
      return this.BindTable(parameterName, "Build.typ_ChangeDataTable2", changes.Select<ChangeData, SqlDataRecord>(new System.Func<ChangeData, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(ChangeData changeData)
      {
        SqlDataRecord record = new SqlDataRecord(Build2Component.typ_ChangeDataTable2);
        record.SetBoolean(0, changeData.SourceChangeOnly);
        record.SetString(1, changeData.Descriptor, BindStringBehavior.Unchanged);
        record.SetString(2, changeData.ExternalData, BindStringBehavior.Unchanged);
        return record;
      }
    }

    public override async Task<List<BuildAnalyticsData>> GetBuildsByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      Build2Component55 build2Component55 = this;
      List<BuildAnalyticsData> items;
      using (build2Component55.TraceScope(method: nameof (GetBuildsByDateAsync)))
      {
        build2Component55.PrepareStoredProcedure("Build.prc_GetBuildsByDate");
        build2Component55.BindInt("@dataspaceId", dataspaceId);
        build2Component55.BindInt("@batchSize", batchSize);
        build2Component55.BindNullableDateTime("@fromDate", fromDate);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component55.ExecuteReaderAsync(), build2Component55.ProcedureName, build2Component55.RequestContext))
        {
          resultCollection.AddBinder<BuildAnalyticsData>(build2Component55.GetBuildAnalyticsDataBinder(build2Component55.GetDataspaceIdentifier(dataspaceId)));
          items = resultCollection.GetCurrent<BuildAnalyticsData>().Items;
        }
      }
      return items;
    }

    public override async Task<List<BuildDefinitionAnalyticsData>> GetBuildDefinitionsByDateAsync(
      int dataspaceId,
      int batchSize,
      DateTime? fromDate)
    {
      Build2Component55 build2Component55 = this;
      List<BuildDefinitionAnalyticsData> items;
      using (build2Component55.TraceScope(method: nameof (GetBuildDefinitionsByDateAsync)))
      {
        build2Component55.PrepareStoredProcedure("Build.prc_GetBuildDefinitionsByDate");
        build2Component55.BindInt("@dataspaceId", dataspaceId);
        build2Component55.BindInt("@batchSize", batchSize);
        build2Component55.BindNullableDateTime("@fromDate", fromDate);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component55.ExecuteReaderAsync(), build2Component55.ProcedureName, build2Component55.RequestContext))
        {
          resultCollection.AddBinder<BuildDefinitionAnalyticsData>(build2Component55.GetBuildDefinitionAnalyticsDataBinder(build2Component55.GetDataspaceIdentifier(dataspaceId)));
          items = resultCollection.GetCurrent<BuildDefinitionAnalyticsData>().Items;
        }
      }
      return items;
    }

    public override async Task<IList<BuildData>> GetDeletedBuilds(
      Guid projectId,
      int? definitionId,
      string folderPath,
      DateTime maxQueueTime,
      int maxBuilds,
      HashSet<int> excludedDefinitionIds)
    {
      Build2Component55 component = this;
      IList<BuildData> items;
      using (component.TraceScope(method: nameof (GetDeletedBuilds)))
      {
        component.PrepareStoredProcedure("Build.prc_GetDeletedBuilds");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId));
        component.BindNullableInt("@definitionId", definitionId);
        component.BindRecursiveFolderPath("@folderPath", folderPath);
        component.BindNullableUtcDateTime2("@maxQueueTime", new DateTime?(maxQueueTime));
        component.BindInt("@maxBuilds", maxBuilds);
        component.BindUniqueInt32Table("@excludedDefinitionIds", (IEnumerable<int>) excludedDefinitionIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(component.GetBuildDataBinder());
          items = (IList<BuildData>) resultCollection.GetCurrent<BuildData>().Items;
        }
      }
      return items;
    }

    public override async Task<BuildProcessResources> GetAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      ResourceType? resourceType,
      string resourceId)
    {
      Build2Component55 build2Component55_1 = this;
      build2Component55_1.TraceEnter(0, nameof (GetAuthorizedResourcesAsync));
      build2Component55_1.PrepareStoredProcedure("Build.prc_GetAuthorizedResources");
      build2Component55_1.BindInt("@dataspaceId", build2Component55_1.GetDataspaceId(projectId));
      build2Component55_1.BindNullableInt("@definitionId", definitionId);
      Build2Component55 build2Component55_2 = build2Component55_1;
      ResourceType? nullable = resourceType;
      int? parameterValue = nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?();
      build2Component55_2.BindNullableInt("@resourceType", parameterValue);
      build2Component55_1.BindString("@resourceId", resourceId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      BuildProcessResources authorizedResourcesAsync;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component55_1.ExecuteReaderAsync(), build2Component55_1.ProcedureName, build2Component55_1.RequestContext))
      {
        resultCollection.AddBinder<ResourceReference>(build2Component55_1.GetResourceReferenceBinder());
        BuildProcessResources processResources = new BuildProcessResources();
        foreach (ResourceReference resourceReference in resultCollection.GetCurrent<ResourceReference>())
        {
          if (resourceReference != null)
            processResources.Add(resourceReference);
        }
        build2Component55_1.TraceLeave(0, nameof (GetAuthorizedResourcesAsync));
        authorizedResourcesAsync = processResources;
      }
      return authorizedResourcesAsync;
    }

    public override async Task<BuildProcessResources> UpdateAuthorizedResourcesAsync(
      Guid projectId,
      int? definitionId,
      Guid authorizedBy,
      BuildProcessResources resources)
    {
      Build2Component55 build2Component55 = this;
      build2Component55.TraceEnter(0, nameof (UpdateAuthorizedResourcesAsync));
      build2Component55.PrepareStoredProcedure("Build.prc_UpdateAuthorizedResources");
      build2Component55.BindInt("@dataspaceId", build2Component55.GetDataspaceId(projectId));
      build2Component55.BindNullableInt("@definitionId", definitionId);
      build2Component55.BindGuid("@authorizedBy", authorizedBy);
      build2Component55.BindAuthorizedResourcesTable("@resourcesToAuthorize", resources.GetAuthorizedResources());
      build2Component55.BindAuthorizedResourcesTable("@resourcesToUnauthorize", resources.GetUnauthorizedResources());
      BuildProcessResources processResources1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component55.ExecuteReaderAsync(), build2Component55.ProcedureName, build2Component55.RequestContext))
      {
        resultCollection.AddBinder<ResourceReference>(build2Component55.GetResourceReferenceBinder());
        BuildProcessResources processResources2 = new BuildProcessResources();
        foreach (ResourceReference resourceReference in resultCollection.GetCurrent<ResourceReference>())
        {
          if (resourceReference != null)
            processResources2.Add(resourceReference);
        }
        build2Component55.TraceLeave(0, nameof (UpdateAuthorizedResourcesAsync));
        processResources1 = processResources2;
      }
      return processResources1;
    }

    public override async Task<BuildData> GetLatestCompletedBuildAsync(
      Guid projectId,
      string repositoryIdentifier,
      string repositoryType,
      string branchName)
    {
      Build2Component55 build2Component55 = this;
      BuildData completedBuildAsync;
      using (build2Component55.TraceScope(method: nameof (GetLatestCompletedBuildAsync)))
      {
        build2Component55.PrepareStoredProcedure("Build.prc_GetLatestCompletedBuild");
        build2Component55.BindInt("@dataspaceId", build2Component55.GetDataspaceId(projectId));
        build2Component55.BindString("@repositoryIdentifier", repositoryIdentifier, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        build2Component55.BindString("@repositoryType", repositoryType, 40, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        build2Component55.BindString("@branchName", branchName, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await build2Component55.ExecuteReaderAsync(), build2Component55.ProcedureName, build2Component55.RequestContext))
        {
          resultCollection.AddBinder<BuildData>(build2Component55.GetBuildDataBinder());
          completedBuildAsync = resultCollection.GetCurrent<BuildData>().SingleOrDefault<BuildData>();
        }
      }
      return completedBuildAsync;
    }

    public override List<ChangeData> GetChanges(
      Guid projectId,
      int buildId,
      int startId,
      bool includeSourceChange,
      int maxChanges)
    {
      this.TraceEnter(0, nameof (GetChanges));
      this.PrepareStoredProcedure("Build.prc_GetChanges");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@startId", startId);
      this.BindBoolean("@includeSourceChange", includeSourceChange);
      this.BindInt("@maxChanges", maxChanges);
      List<ChangeData> changes = (List<ChangeData>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ChangeData>(this.GetChangeDataBinder());
        changes = resultCollection.GetCurrent<ChangeData>().Items;
      }
      this.TraceLeave(0, nameof (GetChanges));
      return changes;
    }

    protected override ObjectBinder<ChangeData> GetChangeDataBinder() => (ObjectBinder<ChangeData>) new ChangeDataBinder3();

    protected virtual ObjectBinder<BuildAnalyticsData> GetBuildAnalyticsDataBinder(Guid projectId) => (ObjectBinder<BuildAnalyticsData>) new BuildAnalyticsDataBinder(this.RequestContext, projectId);

    protected virtual ObjectBinder<ShallowBuildAnalyticsData> GetShallowBuildAnalyticsDataBinder(
      Guid projectId)
    {
      return (ObjectBinder<ShallowBuildAnalyticsData>) new ShallowBuildAnalyticsDataBinder(this.RequestContext, projectId);
    }

    protected virtual ObjectBinder<BuildDefinitionAnalyticsData> GetBuildDefinitionAnalyticsDataBinder(
      Guid projectId)
    {
      return (ObjectBinder<BuildDefinitionAnalyticsData>) new BuildDefinitionAnalyticsDataBinder(this.RequestContext, projectId);
    }

    public override bool SupportsResourceAuthorizationForAllDefinitions() => true;

    protected override ObjectBinder<ResourceReference> GetResourceReferenceBinder() => (ObjectBinder<ResourceReference>) new ResourceReferenceBinder2(this.RequestContext);
  }
}
