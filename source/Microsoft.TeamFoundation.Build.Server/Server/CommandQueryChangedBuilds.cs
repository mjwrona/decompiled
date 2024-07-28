// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CommandQueryChangedBuilds
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
  internal sealed class CommandQueryChangedBuilds : CommandQueryBuildsBase
  {
    private bool m_informationIncluded;
    private bool m_hasMoreBuilds = true;
    private bool m_hasMoreInformation;
    private BuildQueryResult m_queryResult;
    private BuildComponent m_dbBuildQuery;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<BuildDetail> m_buildDetailBinder;
    private BuildInformationMerger m_buildInformationMerger;

    internal CommandQueryChangedBuilds(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryChangedBuilds constructed");
    }

    public BuildQueryResult QueryResult => this.m_queryResult;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_dbBuildQueryResult != null)
      {
        this.m_dbBuildQueryResult.Dispose();
        this.m_dbBuildQueryResult = (ResultCollection) null;
      }
      if (this.m_dbBuildQuery != null)
      {
        this.m_dbBuildQuery.Dispose();
        this.m_dbBuildQuery = (BuildComponent) null;
      }
      if (this.m_buildInformationMerger == null)
        return;
      this.m_buildInformationMerger.Dispose();
      this.m_buildInformationMerger = (BuildInformationMerger) null;
    }

    public void Execute(
      IList<string> teamProjects,
      DateTime minChangedTime,
      BuildStatus statusFilter,
      IList<string> informationTypes,
      int batchSize)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_queryResult = new BuildQueryResult();
      this.m_queryResult.Builds.BindCommand((Command) this);
      this.m_queryResult.Builds.IsComplete = false;
      this.m_state = CommandQueryBuildsBase.State.BuildDefinition;
      this.m_informationIncluded = informationTypes.Any<string>();
      if (this.m_informationIncluded)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger");
        this.m_hasMoreInformation = true;
        this.m_buildInformationMerger = new BuildInformationMerger(this.RequestContext, informationTypes);
      }
      this.m_dbBuildQuery = this.RequestContext.CreateComponent<BuildComponent>("Build");
      this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryChangedBuilds(this.RequestContext, (ICollection<string>) teamProjects, minChangedTime, statusFilter, batchSize);
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
        this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_queryResult.Definitions, (Func<BuildDefinition, bool>) (x => true), QueryOptions.Definitions);
        this.m_state = CommandQueryBuildsBase.State.BuildDetail;
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading builds");
        if (this.m_buildDetailBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail>();
        }
        if (this.m_informationIncluded)
          this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_buildDetailBinder.MoveNext()))
        {
          BuildDetail current = this.m_buildDetailBinder.Current;
          BuildDefinition buildDefinition = (BuildDefinition) null;
          if (!this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition) || !this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuilds))
          {
            this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Access denied for build '{0}'", (object) current.Uri);
          }
          else
          {
            this.m_queryResult.Builds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
            current.TeamProject = buildDefinition.TeamProject.Name;
            current.ProjectId = buildDefinition.TeamProject.Id;
            if (this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition))
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
        if (this.m_informationIncluded)
          this.m_state = CommandQueryBuildsBase.State.BuildInformation;
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildInformation)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build information");
        this.MaxCacheSize = Command.CommandCacheLimit;
        do
          ;
        while (!this.IsCacheFull && (this.m_hasMoreInformation = this.m_buildInformationMerger.TryMergeNext()));
        if (!this.m_hasMoreInformation && this.m_hasMoreBuilds)
          this.m_state = CommandQueryBuildsBase.State.BuildDetail;
      }
      if (this.m_hasMoreBuilds || this.m_hasMoreInformation)
        return;
      this.m_queryResult.Builds.IsComplete = true;
      this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Command completed");
    }
  }
}
