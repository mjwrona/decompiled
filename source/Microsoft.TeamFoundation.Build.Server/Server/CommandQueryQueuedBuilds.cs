// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CommandQueryQueuedBuilds
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class CommandQueryQueuedBuilds : CommandQueryBuildsBase
  {
    private bool m_hasMoreBuilds = true;
    private bool m_hasMoreInformation;
    private bool m_informationIncluded;
    private IEnumerator<BuildQueueSpec> m_specEnumerator;
    private BuildInformationMerger m_buildInformationMerger;
    private StreamingCollection<BuildQueueQueryResult> m_results;
    private IBuild2Converter m_build2Converter;
    private bool m_includeBuild2s;
    private List<BuildDetail> m_build2s;
    private BuildQueueComponent m_dbBuildQuery;
    private BuildQueueQueryResult m_currentResult;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<QueuedBuild> m_queuedBuildBinder;
    private ObjectBinder<BuildDetail> m_buildDetailBinder;
    private HashSet<string> m_buildUris = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public CommandQueryQueuedBuilds(
      IVssRequestContext requestContext,
      Guid projectId,
      IBuild2Converter build2Converter)
      : base(requestContext, projectId)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryQueuedBuilds constructed");
      this.m_build2Converter = build2Converter;
    }

    public StreamingCollection<BuildQueueQueryResult> Results => this.m_results;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_specEnumerator != null)
      {
        this.m_specEnumerator.Dispose();
        this.m_specEnumerator = (IEnumerator<BuildQueueSpec>) null;
      }
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

    public void Execute(IList<BuildQueueSpec> queueSpecs, bool includeBuild2s)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_specEnumerator = queueSpecs.GetEnumerator();
      this.m_results = new StreamingCollection<BuildQueueQueryResult>((Command) this);
      if (this.m_specEnumerator.MoveNext())
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Executing first BuildQueueSpec");
        this.m_state = CommandQueryBuildsBase.State.BuildDefinition;
        this.m_dbBuildQuery = this.RequestContext.CreateComponent<BuildQueueComponent>("Build");
        this.m_includeBuild2s = includeBuild2s;
        this.ContinueExecution();
        if (this.IsCacheFull)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "The cache is full, notifying partial results ready");
          this.RequestContext.PartialResultsReady();
        }
      }
      else
      {
        this.m_results.IsComplete = true;
        this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Command completed");
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (Execute));
    }

    public override void ContinueExecution()
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ContinueExecution));
      if (this.m_state == CommandQueryBuildsBase.State.BuildDefinition)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build definitions");
        if (this.m_dbBuildQueryResult == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Querying builds");
          this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryBuilds(this.RequestContext, this.m_specEnumerator.Current, this.m_specEnumerator.Current.QueryOptions | QueryOptions.Definitions);
          this.m_currentResult = new BuildQueueQueryResult();
          this.m_results.Enqueue(this.m_currentResult);
          this.m_informationIncluded = this.m_specEnumerator.Current.InformationTypes.Any<string>();
          if (this.m_informationIncluded)
          {
            this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger");
            this.m_hasMoreInformation = true;
            this.m_buildInformationMerger = new BuildInformationMerger(this.RequestContext, (IList<string>) this.m_specEnumerator.Current.InformationTypes);
          }
        }
        if (this.m_specEnumerator.Current.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
          this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_currentResult.Definitions, new Func<BuildDefinition, bool>((this.m_specEnumerator.Current.DefinitionFilter as BuildDefinitionSpec).Match), this.m_specEnumerator.Current.QueryOptions);
        else
          this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_currentResult.Definitions, (Func<BuildDefinition, bool>) (x => true), this.m_specEnumerator.Current.QueryOptions);
        this.m_state = CommandQueryBuildsBase.State.QueuedBuild;
      }
      if (this.m_state == CommandQueryBuildsBase.State.QueuedBuild)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading queued builds");
        if (this.m_queuedBuildBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting QueuedBuild query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_queuedBuildBinder = this.m_dbBuildQueryResult.GetCurrent<QueuedBuild>();
          this.m_currentResult.QueuedBuilds.BindCommand((Command) this);
        }
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_queuedBuildBinder.MoveNext()))
        {
          QueuedBuild current = this.m_queuedBuildBinder.Current;
          BuildDefinition definition;
          if (!this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out definition))
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped queued build '{0}' because definition '{1}' not found", (object) current.Id, (object) current.BuildDefinitionUri);
          else if (this.m_specEnumerator.Current.DefinitionFilterType == DefinitionFilterType.DefinitionSpec && !(this.m_specEnumerator.Current.DefinitionFilter as BuildDefinitionSpec).Match(definition))
          {
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped queued build '{0}' because definition '{1}' does not match spec", (object) current.Id, (object) current.BuildDefinitionUri);
          }
          else
          {
            current.TeamProject = definition.TeamProject.Name;
            current.ProjectId = definition.TeamProject.Id;
            if ((this.m_specEnumerator.Current.QueryOptions & QueryOptions.Definitions) == QueryOptions.Definitions && this.HasBuildPermission(this.RequestContext, definition.Uri, definition.SecurityToken, BuildPermissions.ViewBuildDefinition))
            {
              current.Definition = definition;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Associated definition '{0}' to queued build '{1}'", (object) definition.Uri, (object) current.Id);
            }
            this.m_buildUris.UnionWith((IEnumerable<string>) current.BuildUris);
            this.m_currentResult.QueuedBuilds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read queued build '{0}'", (object) current.Id);
          }
        }
        if (!this.m_hasMoreBuilds && this.m_includeBuild2s && this.m_build2Converter != null)
        {
          foreach (Tuple<QueuedBuild, BuildDetail> queuedBuild in this.m_build2Converter.GetQueuedBuilds(this.RequestContext, this.ProjectId, this.m_specEnumerator.Current))
          {
            this.m_currentResult.QueuedBuilds.Enqueue(queuedBuild.Item1);
            if (this.m_build2s == null)
              this.m_build2s = new List<BuildDetail>();
            this.m_build2s.Add(queuedBuild.Item2);
          }
        }
        if (!this.m_hasMoreBuilds)
        {
          this.m_hasMoreBuilds = true;
          this.m_state = CommandQueryBuildsBase.State.BuildDetail;
          this.m_currentResult.QueuedBuilds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading builds");
        if (this.m_informationIncluded)
          this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
        if (this.m_buildDetailBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail>();
          this.m_currentResult.Builds.BindCommand((Command) this);
          this.m_currentResult.Builds.IsComplete = false;
        }
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_buildDetailBinder.MoveNext()))
        {
          BuildDetail current = this.m_buildDetailBinder.Current;
          BuildDefinition buildDefinition;
          if (!this.m_buildUris.Contains(current.Uri) || !this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition) || !this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuilds))
          {
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped build '{0}'", (object) current.Uri);
          }
          else
          {
            this.m_buildUris.Remove(current.Uri);
            this.m_currentResult.Builds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
            this.m_controllerUris.Add(current.BuildControllerUri);
            current.TeamProject = buildDefinition.TeamProject.Name;
            current.ProjectId = buildDefinition.TeamProject.Id;
            if ((this.m_specEnumerator.Current.QueryOptions & QueryOptions.Definitions) == QueryOptions.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition))
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
              this.m_currentResult.Builds.Enqueue(build2);
          }
          this.m_state = CommandQueryBuildsBase.State.BuildServices;
          this.m_currentResult.Builds.IsComplete = true;
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
            this.m_currentResult.Builds.IsComplete = true;
          }
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildServices)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build services");
        this.m_dbBuildQueryResult.NextResult();
        this.ReadBuildServices(this.m_dbBuildQueryResult, this.m_specEnumerator.Current.QueryOptions, this.m_currentResult.Controllers, this.m_currentResult.Agents, this.m_currentResult.ServiceHosts);
        if (this.m_specEnumerator.MoveNext())
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Executing next BuildQueueSpec");
          this.m_hasMoreBuilds = true;
          this.m_buildDetailBinder = (ObjectBinder<BuildDetail>) null;
          this.m_queuedBuildBinder = (ObjectBinder<QueuedBuild>) null;
          this.m_controllerUris.Clear();
          this.m_allDefinitions.Clear();
          if (this.m_dbBuildQueryResult != null)
          {
            this.m_dbBuildQueryResult.Dispose();
            this.m_dbBuildQueryResult = (ResultCollection) null;
          }
          if (this.m_buildInformationMerger != null)
          {
            this.m_buildInformationMerger.Dispose();
            this.m_buildInformationMerger = (BuildInformationMerger) null;
          }
          this.m_state = CommandQueryBuildsBase.State.BuildDefinition;
        }
        else
        {
          this.m_results.IsComplete = true;
          this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Command completed");
        }
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ContinueExecution));
    }
  }
}
