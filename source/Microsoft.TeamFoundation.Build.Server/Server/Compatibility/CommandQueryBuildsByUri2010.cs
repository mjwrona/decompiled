// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CommandQueryBuildsByUri2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class CommandQueryBuildsByUri2010 : CommandQueryBuildsBase2010
  {
    private int m_currentUriIndex;
    private QueryOptions2010 m_queryOptions;
    private bool m_informationIncluded;
    private IList<string> m_uris;
    private bool m_hasMoreBuilds = true;
    private bool m_hasMoreInformation;
    private BuildQueryResult2010 m_result;
    private CompatibilityComponent m_dbBuildQuery;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<BuildDetail2010> m_buildDetailBinder;
    private BuildInformationMerger2010 m_buildInformationMerger;
    private HashSet<string> m_buildsReturned = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal CommandQueryBuildsByUri2010(IVssRequestContext requestContext)
      : base(requestContext)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryBuildsByUri2010 constructed");
    }

    public BuildQueryResult2010 Result => this.m_result;

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
        this.m_dbBuildQuery = (CompatibilityComponent) null;
      }
      if (this.m_buildInformationMerger == null)
        return;
      this.m_buildInformationMerger.Dispose();
      this.m_buildInformationMerger = (BuildInformationMerger2010) null;
    }

    public void Execute(
      IList<string> uris,
      IList<string> informationTypes,
      QueryOptions2010 options,
      QueryDeletedOption2010 deletedOption)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_result = new BuildQueryResult2010();
      this.m_result.Builds.BindCommand((Command) this);
      this.m_result.Builds.IsComplete = false;
      this.m_state = CommandQueryBuildsBase2010.State.BuildDefinition;
      this.m_uris = uris;
      this.m_queryOptions = options;
      this.m_informationIncluded = informationTypes.Any<string>();
      if (this.m_informationIncluded)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger2010");
        this.m_hasMoreInformation = true;
        this.m_buildInformationMerger = new BuildInformationMerger2010(this.RequestContext, informationTypes);
      }
      this.m_dbBuildQuery = this.RequestContext.CreateComponent<CompatibilityComponent>("Build");
      this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryBuildsByUri(uris, options | QueryOptions2010.Definitions, deletedOption);
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
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildDefinition)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build definitions");
        this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_result.Definitions, (Func<BuildDefinition2010, bool>) (x => true), this.m_queryOptions);
        this.m_state = CommandQueryBuildsBase2010.State.BuildDetail;
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading builds");
        if (this.m_buildDetailBinder == null)
        {
          this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
          this.m_dbBuildQueryResult.NextResult();
          this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail2010>();
        }
        if (this.m_informationIncluded)
          this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
        while (!this.IsCacheFull && (this.m_hasMoreBuilds = this.m_buildDetailBinder.MoveNext()))
        {
          BuildDetail2010 current = this.m_buildDetailBinder.Current;
          BuildDefinition2010 buildDefinition2010 = (BuildDefinition2010) null;
          if (!this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition2010) || !this.HasBuildPermission(this.RequestContext, buildDefinition2010.Uri, buildDefinition2010.SecurityToken, BuildPermissions.ViewBuilds))
          {
            ++this.m_currentUriIndex;
            this.m_result.Builds.Enqueue((BuildDetail2010) null);
            this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Enqueued null because definition not found for build '{0}'", (object) current.Uri);
          }
          else
          {
            while (this.m_currentUriIndex < this.m_uris.Count && !TFStringComparer.ArtiFactUrl.Equals(this.m_uris[this.m_currentUriIndex], current.Uri))
            {
              this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Skipping input uri '{0}' that does not match build '{1}'", (object) this.m_uris[this.m_currentUriIndex], (object) current.Uri);
              ++this.m_currentUriIndex;
              this.m_result.Builds.Enqueue((BuildDetail2010) null);
            }
            ++this.m_currentUriIndex;
            RosarioHelper.FixUriForBuildMachine(this.RequestContext, current);
            this.m_result.Builds.Enqueue(current);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
            this.m_controllerUris.Add(current.BuildControllerUri);
            current.TeamProject = buildDefinition2010.TeamProject.Name;
            if ((this.m_queryOptions & QueryOptions2010.Definitions) == QueryOptions2010.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition2010.Uri, buildDefinition2010.SecurityToken, BuildPermissions.ViewBuildDefinition))
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
        }
        if (this.m_hasMoreInformation)
          this.m_state = CommandQueryBuildsBase2010.State.BuildInformation;
        else if (!this.m_hasMoreBuilds)
        {
          while (this.m_currentUriIndex < this.m_uris.Count)
          {
            ++this.m_currentUriIndex;
            this.m_result.Builds.Enqueue((BuildDetail2010) null);
          }
          this.m_state = CommandQueryBuildsBase2010.State.BuildServices;
          this.m_result.Builds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildInformation)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build information");
        this.MaxCacheSize = Command.CommandCacheLimit;
        do
          ;
        while (!this.IsCacheFull && (this.m_hasMoreInformation = this.m_buildInformationMerger.TryMergeNext()));
        if (!this.m_hasMoreInformation && this.m_hasMoreBuilds)
          this.m_state = CommandQueryBuildsBase2010.State.BuildDetail;
        else if (!this.m_hasMoreInformation && !this.m_hasMoreBuilds)
        {
          while (this.m_currentUriIndex < this.m_uris.Count)
          {
            ++this.m_currentUriIndex;
            this.m_result.Builds.Enqueue((BuildDetail2010) null);
          }
          this.m_state = CommandQueryBuildsBase2010.State.BuildServices;
          this.m_result.Builds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase2010.State.BuildServices)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading queued builds");
        this.m_dbBuildQueryResult.NextResult();
        List<BuildAgent2010> buildAgent2010List = new List<BuildAgent2010>();
        List<BuildController2010> buildController2010List = new List<BuildController2010>();
        List<BuildServiceHost2010> buildServiceHost2010List = new List<BuildServiceHost2010>();
        this.ReadBuildServices(this.m_dbBuildQueryResult, this.m_queryOptions, this.m_result.Controllers, this.m_result.Agents, this.m_result.ServiceHosts);
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ContinueExecution));
    }
  }
}
