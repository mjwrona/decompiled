// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildQueueComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildQueueComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[17]
    {
      (IComponentCreator) new ComponentCreator<BuildQueueComponent>(1),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent2>(2),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent3>(3),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent4>(4),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent5>(5),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent6>(6),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent7>(7),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent8>(8),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent9>(9),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent9>(10),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent9>(11),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent9>(12),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent9>(13),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent10>(14),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent10>(15),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent11>(16),
      (IComponentCreator) new ComponentCreator<BuildQueueComponent11>(17)
    }, "Build", "Build");

    protected override string TraceArea => "Build";

    internal virtual ResultCollection CancelBuilds(ICollection<QueuedBuild> queueIds)
    {
      this.TraceEnter(0, nameof (CancelBuilds));
      this.PrepareStoredProcedure("prc_CancelBuilds");
      this.BindTable<int>("@queuedBuildIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table(queueIds.Select<QueuedBuild, int>((System.Func<QueuedBuild, int>) (x => x.Id))));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (CancelBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueueBuilds(
      ICollection<BuildRequest> requests,
      QueueOptions options)
    {
      this.TraceEnter(0, nameof (QueueBuilds));
      this.PrepareStoredProcedure("prc_QueueBuilds");
      this.BindTable<BuildRequest>("@buildRequestTable", (TeamFoundationTableValueParameter<BuildRequest>) new BuildRequestTable(requests));
      this.BindBoolean("@preview", (options & QueueOptions.Preview) == QueueOptions.Preview);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (QueueBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuilds(
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
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildsById(IList<int> ids, QueryOptions options)
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
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildsById));
      return resultCollection;
    }

    internal virtual IDictionary<int, List<int>> GetQueueIdsByBuildIds(
      Guid projectId,
      IList<int> buildIds)
    {
      return (IDictionary<int, List<int>>) new Dictionary<int, List<int>>();
    }

    internal virtual ResultCollection UpdateBuilds(IList<QueuedBuildUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateQueuedBuilds");
      this.BindTable<QueuedBuildUpdateOptions>("@buildUpdateTable", (TeamFoundationTableValueParameter<QueuedBuildUpdateOptions>) new QueuedBuildUpdateTable((ICollection<QueuedBuildUpdateOptions>) updates));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    internal virtual ResultCollection UpdateBuilds(
      IList<QueuedBuildUpdateOptions> updates,
      bool resetQueueTime)
    {
      return this.UpdateBuilds(updates);
    }

    internal virtual ResultCollection StartQueuedBuildsNow(ICollection<QueuedBuild> builds)
    {
      this.TraceEnter(0, nameof (StartQueuedBuildsNow));
      this.PrepareStoredProcedure("prc_StartQueuedBuildsNow");
      this.BindTable<int>("@queuedBuildIdTable", (TeamFoundationTableValueParameter<int>) new Int32Table(builds.Select<QueuedBuild, int>((System.Func<QueuedBuild, int>) (x => x.Id))));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (StartQueuedBuildsNow));
      return resultCollection;
    }

    internal virtual ResultCollection StopBuildRequest(
      Guid projectId,
      int queuedId,
      TeamFoundationIdentity requestedBy,
      Guid writerId,
      string errorMessage)
    {
      return (ResultCollection) null;
    }

    internal virtual ResultCollection StartPendingBuilds(
      Guid projectId,
      string definitionUri,
      DefinitionTriggerType triggerType,
      TeamFoundationIdentity buildOwner,
      string changesetVersion)
    {
      this.TraceEnter(0, nameof (StartPendingBuilds));
      this.PrepareStoredProcedure("prc_StartPendingBuild");
      this.BindItemUriToInt32("@definitionId", definitionUri);
      this.BindInt("@triggerType", (int) triggerType);
      this.BindIdentity("@buildOwner", buildOwner);
      this.BindString("@changesetVersion", changesetVersion, 326, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (StartPendingBuilds));
      return resultCollection;
    }
  }
}
