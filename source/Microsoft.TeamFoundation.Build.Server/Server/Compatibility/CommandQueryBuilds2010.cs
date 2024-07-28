// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CommandQueryBuilds2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class CommandQueryBuilds2010 : CommandQueryBuildsBase2010
  {
    private bool m_hasMoreBuilds = true;
    private bool m_hasMoreInformation;
    private bool m_informationIncluded;
    private IEnumerator<BuildDetailSpec2010> m_specEnumerator;
    private BuildInformationMerger2010 m_buildInformationMerger;
    private StreamingCollection<BuildQueryResult2010> m_results;
    private CompatibilityComponent m_dbBuildQuery;
    private BuildQueryResult2010 m_currentResult;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<BuildDetail2010> m_buildDetailBinder;

    public CommandQueryBuilds2010(IVssRequestContext requestContext)
      : base(requestContext)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryBuilds2010 constructed");
    }

    public StreamingCollection<BuildQueryResult2010> Results => this.m_results;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_specEnumerator != null)
      {
        this.m_specEnumerator.Dispose();
        this.m_specEnumerator = (IEnumerator<BuildDetailSpec2010>) null;
      }
      if (this.m_dbBuildQueryResult != null)
      {
        this.m_dbBuildQueryResult.Dispose();
        this.m_dbBuildQueryResult = (ResultCollection) null;
      }
      if (this.m_dbBuildQuery != null)
      {
        this.m_dbBuildQuery.Dispose();
        this.m_dbBuildQuery = (CompatibilityComponent) null;
      }
      if (this.m_buildInformationMerger == null)
        return;
      this.m_buildInformationMerger.Dispose();
      this.m_buildInformationMerger = (BuildInformationMerger2010) null;
    }

    public void Execute(IList<BuildDetailSpec2010> buildDetailSpecs)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_specEnumerator = buildDetailSpecs.GetEnumerator();
      this.m_results = new StreamingCollection<BuildQueryResult2010>((Command) this);
      if (this.m_specEnumerator.MoveNext())
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Executing first BuildDetailSpec2010");
        this.m_state = CommandQueryBuildsBase2010.State.BuildDefinition;
        this.m_dbBuildQuery = this.RequestContext.CreateComponent<CompatibilityComponent>("Build");
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
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildDefinition)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build definitions");
        if (this.m_dbBuildQueryResult == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Querying builds");
          this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryBuilds(this.m_specEnumerator.Current, this.m_specEnumerator.Current.QueryOptions | QueryOptions2010.Definitions);
          this.m_currentResult = new BuildQueryResult2010();
          this.m_results.Enqueue(this.m_currentResult);
          this.m_informationIncluded = this.m_specEnumerator.Current.InformationTypes.Any<string>();
          if (this.m_informationIncluded)
          {
            this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger2010");
            this.m_hasMoreInformation = true;
            this.m_buildInformationMerger = new BuildInformationMerger2010(this.RequestContext, (IList<string>) this.m_specEnumerator.Current.InformationTypes);
          }
        }
        this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_currentResult.Definitions, new Func<BuildDefinition2010, bool>(this.m_specEnumerator.Current.Match), this.m_specEnumerator.Current.QueryOptions);
        this.m_state = CommandQueryBuildsBase2010.State.BuildDetail;
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading builds");
        if (this.m_informationIncluded)
          this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
        if (this.m_buildDetailBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail2010>();
          this.m_currentResult.Builds.BindCommand((Command) this);
          this.m_currentResult.Builds.IsComplete = false;
        }
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_buildDetailBinder.MoveNext()))
        {
          BuildDetail2010 current = this.m_buildDetailBinder.Current;
          BuildDefinition2010 buildDefinition2010;
          if (this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition2010) && this.HasBuildPermission(this.RequestContext, buildDefinition2010.Uri, buildDefinition2010.SecurityToken, BuildPermissions.ViewBuilds))
          {
            this.m_currentResult.Builds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
            this.m_controllerUris.Add(current.BuildControllerUri);
            current.TeamProject = buildDefinition2010.TeamProject.Name;
            if ((this.m_specEnumerator.Current.QueryOptions & QueryOptions2010.Definitions) == QueryOptions2010.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition2010.Uri, buildDefinition2010.SecurityToken, BuildPermissions.ViewBuildDefinition))
            {
              current.Definition = buildDefinition2010;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Associated definition '{0}' to build '{1}'", (object) buildDefinition2010.Uri, (object) current.Uri);
            }
            if (this.m_informationIncluded)
            {
              this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Enqueuing a build information merge for build '{0}'", (object) current.Uri);
              this.m_hasMoreInformation = true;
              current.Information.BindCommand((Command) this);
              this.m_buildInformationMerger.Enqueue(buildDefinition2010.TeamProject.Id, current.DatabaseUri, current.QueueId, current.Information);
            }
          }
          else
            this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Build definition not found for build '{0}'", (object) current.Uri);
        }
        if (this.m_hasMoreInformation)
          this.m_state = CommandQueryBuildsBase2010.State.BuildInformation;
        else if (!this.m_hasMoreBuilds)
        {
          this.m_state = CommandQueryBuildsBase2010.State.BuildServices;
          this.m_currentResult.Builds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildInformation)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build information");
        this.MaxCacheSize = Command.CommandCacheLimit;
        do
          ;
        while (!this.IsCacheFull && (this.m_hasMoreInformation = this.m_buildInformationMerger.TryMergeNext()));
        if (!this.m_hasMoreInformation)
        {
          if (this.m_hasMoreBuilds)
          {
            this.m_state = CommandQueryBuildsBase2010.State.BuildDetail;
          }
          else
          {
            this.m_state = CommandQueryBuildsBase2010.State.BuildServices;
            this.m_currentResult.Builds.IsComplete = true;
          }
        }
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildServices)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build services");
        this.m_dbBuildQueryResult.NextResult();
        this.ReadBuildServices(this.m_dbBuildQueryResult, this.m_specEnumerator.Current.QueryOptions, this.m_currentResult.Controllers, this.m_currentResult.Agents, this.m_currentResult.ServiceHosts);
        if (this.m_specEnumerator.MoveNext())
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Executing next BuildDetailSpec2010");
          this.m_hasMoreBuilds = true;
          this.m_buildDetailBinder = (ObjectBinder<BuildDetail2010>) null;
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
            this.m_buildInformationMerger = (BuildInformationMerger2010) null;
          }
          this.m_state = CommandQueryBuildsBase2010.State.BuildDefinition;
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
