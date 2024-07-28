// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CommandQueryQueuedBuildsById
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class CommandQueryQueuedBuildsById : CommandQueryBuildsBase
  {
    private int m_currentIndex;
    private QueryOptions m_options;
    private bool m_hasMoreBuilds;
    private IList<string> m_informationTypes;
    private bool m_hasMoreInformation;
    private bool m_informationIncluded;
    private IList<int> m_queueIds;
    private List<BuildDetail> m_build2s;
    private bool m_includeBuild2s;
    private IBuild2Converter m_build2Converter;
    private BuildQueueComponent m_dbBuildQuery;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<QueuedBuild> m_queuedBuildBinder;
    private ObjectBinder<BuildDetail> m_buildDetailBinder;
    private BuildInformationMerger m_buildInformationMerger;
    private BuildQueueQueryResult m_result = new BuildQueueQueryResult();
    private HashSet<string> m_buildUris = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public CommandQueryQueuedBuildsById(
      IVssRequestContext requestContext,
      Guid projectId,
      IBuild2Converter build2Converter)
      : base(requestContext, projectId)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryQueuedBuildsById constructed");
      this.m_build2Converter = build2Converter;
    }

    public BuildQueueQueryResult Result => this.m_result;

    protected override void Dispose(bool disposing)
    {
      if (this.m_dbBuildQueryResult != null)
      {
        this.m_dbBuildQueryResult.Dispose();
        this.m_dbBuildQueryResult = (ResultCollection) null;
      }
      if (this.m_dbBuildQuery != null)
      {
        this.m_dbBuildQuery.Dispose();
        this.m_dbBuildQuery = (BuildQueueComponent) null;
      }
      if (this.m_buildInformationMerger == null)
        return;
      this.m_buildInformationMerger.Dispose();
      this.m_buildInformationMerger = (BuildInformationMerger) null;
    }

    public void Execute(
      IList<int> queueIds,
      IList<string> informationTypes,
      QueryOptions options,
      bool includeBuild2s)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_options = options;
      this.m_queueIds = queueIds;
      this.m_informationTypes = informationTypes;
      this.m_informationIncluded = informationTypes.Count > 0;
      this.m_includeBuild2s = includeBuild2s;
      if (this.m_informationIncluded)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger");
        this.m_hasMoreInformation = true;
        this.m_buildInformationMerger = new BuildInformationMerger(this.RequestContext, informationTypes);
      }
      this.m_dbBuildQuery = this.RequestContext.CreateComponent<BuildQueueComponent>("Build");
      this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryBuildsById(queueIds, this.m_options | QueryOptions.Definitions);
      this.ContinueExecution();
      if (this.IsCacheFull)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "The cache is full, notifying partial results ready");
        this.RequestContext.PartialResultsReady();
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (Execute));
    }

    public override void ContinueExecution()
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ContinueExecution));
      if (this.m_state == CommandQueryBuildsBase.State.BuildDefinition)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build definitions");
        this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_result.Definitions, (Func<BuildDefinition, bool>) (x => true), this.m_options);
        this.m_state = CommandQueryBuildsBase.State.QueuedBuild;
      }
      if (this.m_state == CommandQueryBuildsBase.State.QueuedBuild)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading queued builds");
        if (this.m_queuedBuildBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting QueuedBuild query result");
          this.m_currentIndex = 0;
          this.m_dbBuildQueryResult.NextResult();
          this.m_result.QueuedBuilds.BindCommand((Command) this);
          this.m_queuedBuildBinder = this.m_dbBuildQueryResult.GetCurrent<QueuedBuild>();
        }
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_queuedBuildBinder.MoveNext()))
        {
          QueuedBuild current = this.m_queuedBuildBinder.Current;
          BuildDefinition buildDefinition;
          if (!this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition))
          {
            ++this.m_currentIndex;
            this.m_result.QueuedBuilds.Enqueue((QueuedBuild) null);
            this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Enqueued null because definition not found for build '{0}'", (object) current.Id);
          }
          else
          {
            while (this.m_currentIndex < this.m_queueIds.Count && this.m_queueIds[this.m_currentIndex] != current.Id)
            {
              QueuedBuild queuedBuild2 = this.GetQueuedBuild2(this.m_queueIds[this.m_currentIndex]);
              if (queuedBuild2 == null)
                this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Skipping input Id '{0}' that does not match queued build '{1}'", (object) this.m_queueIds[this.m_currentIndex], (object) current.Id);
              ++this.m_currentIndex;
              this.m_result.QueuedBuilds.Enqueue(queuedBuild2);
            }
            current.TeamProject = buildDefinition.TeamProject.Name;
            current.ProjectId = buildDefinition.TeamProject.Id;
            ++this.m_currentIndex;
            this.m_result.QueuedBuilds.Enqueue(current);
            this.m_buildUris.UnionWith((IEnumerable<string>) current.BuildUris);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read queued build '{0}'", (object) current.Id);
          }
        }
        if (!this.m_hasMoreBuilds)
        {
          while (this.m_currentIndex < this.m_queueIds.Count)
          {
            QueuedBuild queuedBuild2 = this.GetQueuedBuild2(this.m_queueIds[this.m_currentIndex]);
            ++this.m_currentIndex;
            this.m_result.QueuedBuilds.Enqueue(queuedBuild2);
          }
          this.m_hasMoreBuilds = true;
          this.m_state = CommandQueryBuildsBase.State.BuildDetail;
          this.m_result.QueuedBuilds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading queued builds");
        if (this.m_buildDetailBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_result.Builds.BindCommand((Command) this);
          this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail>();
        }
        if (this.m_informationIncluded)
          this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_buildDetailBinder.MoveNext()))
        {
          BuildDetail current = this.m_buildDetailBinder.Current;
          BuildDefinition buildDefinition;
          if (!this.m_buildUris.Contains(current.Uri) || !this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition) || !this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuilds))
          {
            this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Enqueued null because definition not found for build '{0}'", (object) current.Uri);
          }
          else
          {
            this.m_buildUris.Remove(current.Uri);
            this.m_result.Builds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
            this.m_controllerUris.Add(current.BuildControllerUri);
            current.TeamProject = buildDefinition.TeamProject.Name;
            current.ProjectId = buildDefinition.TeamProject.Id;
            if ((this.m_options & QueryOptions.Definitions) == QueryOptions.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition))
            {
              current.Definition = buildDefinition;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Associated definition '{0}' to build '{1}'", (object) buildDefinition.Uri, (object) current.Uri);
            }
            if (this.m_informationIncluded)
            {
              this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Enqueuing a build information merge for build '{0}'", (object) current.Uri);
              this.m_hasMoreInformation = true;
              current.Information.BindCommand((Command) this);
              this.m_buildInformationMerger.Enqueue(current, current.Information);
            }
          }
        }
        if (this.m_hasMoreInformation)
          this.m_state = CommandQueryBuildsBase.State.BuildInformation;
        else if (!this.m_hasMoreBuilds)
        {
          if (this.m_build2s != null && this.m_build2s.Count > 0)
          {
            foreach (BuildDetail build2 in this.m_build2s)
              this.m_result.Builds.Enqueue(build2);
          }
          this.m_state = CommandQueryBuildsBase.State.BuildServices;
          this.m_result.Builds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildInformation)
      {
        this.m_hasMoreInformation = this.ReadBuildInformation(this.m_buildInformationMerger);
        if (!this.m_hasMoreInformation)
        {
          if (this.m_hasMoreBuilds)
          {
            this.m_state = CommandQueryBuildsBase.State.BuildDetail;
          }
          else
          {
            this.m_state = CommandQueryBuildsBase.State.BuildServices;
            this.m_result.Builds.IsComplete = true;
          }
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildServices)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build services");
        this.m_dbBuildQueryResult.NextResult();
        this.ReadBuildServices(this.m_dbBuildQueryResult, this.m_options, this.m_result.Controllers, this.m_result.Agents, this.m_result.ServiceHosts);
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ContinueExecution));
    }

    private QueuedBuild GetQueuedBuild2(int id)
    {
      QueuedBuild queuedBuild2 = (QueuedBuild) null;
      if (this.m_includeBuild2s && this.m_build2Converter != null)
      {
        Tuple<QueuedBuild, BuildDetail> queuedBuildById = this.m_build2Converter.GetQueuedBuildById(this.RequestContext, id, this.m_informationTypes);
        if (queuedBuildById.Item1 != null)
        {
          queuedBuild2 = queuedBuildById.Item1;
          if (this.m_build2s == null)
            this.m_build2s = new List<BuildDetail>();
          this.m_build2s.Add(queuedBuildById.Item2);
        }
      }
      return queuedBuild2;
    }
  }
}
