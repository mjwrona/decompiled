// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildQueueComponent4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildQueueComponent4 : BuildQueueComponent3
  {
    public BuildQueueComponent4()
    {
      this.ServiceVersion = ServiceVersion.V4;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection QueueBuilds(
      ICollection<BuildRequest> requests,
      QueueOptions options)
    {
      this.TraceEnter(0, nameof (QueueBuilds));
      this.PrepareStoredProcedure("prc_QueueBuilds");
      this.BindTable<BuildRequest>("@buildRequestTable", (TeamFoundationTableValueParameter<BuildRequest>) new BuildRequestTable(requests));
      this.BindBoolean("@preview", (options & QueueOptions.Preview) == QueueOptions.Preview);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (QueueBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuilds(
      IVssRequestContext requestContext,
      BuildQueueSpec spec,
      QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuilds));
      this.PrepareStoredProcedure("prc_QueryQueuedBuilds");
      if (spec.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        this.BindTeamProject(requestContext, "@teamProject", ((BuildDefinitionSpec) spec.DefinitionFilter).TeamProject, false);
        this.BindItemPath("@definitionPath", ((BuildDefinitionSpec) spec.DefinitionFilter).Path, false);
      }
      else
        this.BindTable<int>("@definitionIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToDistinctInt32Table((IEnumerable<string>) spec.DefinitionFilter));
      this.BindItemName("@controllerName", spec.ControllerSpec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ControllerSpec.ServiceHostName, 256, false);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindByte("@statusFilter", (byte) spec.Status);
      this.BindInt("@completedAge", spec.CompletedAge);
      this.BindByte("@options", (byte) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildsById(IList<int> ids, QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuildsById));
      this.PrepareStoredProcedure("prc_QueryQueuedBuildsById");
      this.BindTable<KeyValuePair<int, int>>("@queuedBuildIdTable", (TeamFoundationTableValueParameter<KeyValuePair<int, int>>) BuildSqlResourceComponent.IntsToOrderedTable((IEnumerable<int>) ids));
      this.BindInt("@options", (int) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder());
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder());
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder());
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildsById));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuilds(IList<QueuedBuildUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateQueuedBuilds");
      this.BindTable<QueuedBuildUpdateOptions>("@buildUpdateTable", (TeamFoundationTableValueParameter<QueuedBuildUpdateOptions>) new QueuedBuildUpdateTable((ICollection<QueuedBuildUpdateOptions>) updates));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    internal override ResultCollection StartQueuedBuildsNow(ICollection<QueuedBuild> queuedBuilds)
    {
      this.TraceEnter(0, nameof (StartQueuedBuildsNow));
      this.PrepareStoredProcedure("prc_StartQueuedBuildsNow");
      this.BindTable<int>("@queuedBuildIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table(queuedBuilds.Select<QueuedBuild, int>((System.Func<QueuedBuild, int>) (x => x.Id))));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (StartQueuedBuildsNow));
      return resultCollection;
    }
  }
}
