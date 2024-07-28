// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CommandQueryBuildsByUri
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class CommandQueryBuildsByUri : CommandQueryBuildsBase
  {
    private int m_currentUriIndex;
    private QueryOptions m_queryOptions;
    private string[] m_informationTypes;
    private bool m_informationIncluded;
    private IList<string> m_uris;
    private bool m_hasMoreBuilds = true;
    private bool m_hasMoreInformation;
    private bool m_includeBuild2s;
    private IBuild2Converter m_build2Converter;
    private BuildQueryResult m_result;
    private BuildComponent m_dbBuildQuery;
    private ResultCollection m_dbBuildQueryResult;
    private ObjectBinder<BuildDetail> m_buildDetailBinder;
    private ObjectBinder<QueuedBuild> m_queuedBuildBinder;
    private BuildInformationMerger m_buildInformationMerger;
    private HashSet<int> m_queueIds = new HashSet<int>();

    internal CommandQueryBuildsByUri(
      IVssRequestContext requestContext,
      Guid projectId,
      IBuild2Converter build2Converter)
      : base(requestContext, projectId)
    {
      requestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "CommandQueryBuildsByUri constructed");
      this.m_build2Converter = build2Converter;
    }

    public BuildQueryResult Result => this.m_result;

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
      IList<string> uris,
      IList<string> informationTypes,
      QueryOptions options,
      QueryDeletedOption deletedOption,
      bool includeBuild2s)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (Execute));
      this.m_result = new BuildQueryResult();
      this.m_state = CommandQueryBuildsBase.State.BuildDefinition;
      this.m_includeBuild2s = includeBuild2s;
      this.m_uris = uris;
      this.m_queryOptions = options;
      this.m_informationIncluded = informationTypes.Any<string>();
      this.m_informationTypes = informationTypes.ToArray<string>();
      if (this.m_informationIncluded)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Creating BuildInformationMerger");
        this.m_hasMoreInformation = true;
        this.m_buildInformationMerger = new BuildInformationMerger(this.RequestContext, informationTypes);
      }
      this.m_dbBuildQuery = this.RequestContext.CreateComponent<BuildComponent>("Build");
      this.m_dbBuildQueryResult = this.m_dbBuildQuery.QueryBuildsByUri(uris, options | QueryOptions.Definitions, deletedOption);
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
      if (this.m_dbBuildQueryResult == null)
        throw new TeamFoundationServiceException("CommandQueryBuildsByUri was used after Dispose().");
      if (this.m_state == CommandQueryBuildsBase.State.BuildDefinition)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build definitions");
        this.ReadDefinitions(this.m_dbBuildQueryResult, this.m_result.Definitions, (Func<BuildDefinition, bool>) (x => true), this.m_queryOptions);
        this.m_state = CommandQueryBuildsBase.State.BuildDetail;
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildDetail)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading builds");
        this.m_hasMoreBuilds = this.ReadBuildsByUri();
        if (this.m_hasMoreInformation)
          this.m_state = CommandQueryBuildsBase.State.BuildInformation;
        else if (!this.m_hasMoreBuilds)
        {
          this.EnqueueNulls();
          this.m_state = CommandQueryBuildsBase.State.QueuedBuild;
          this.m_result.Builds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildInformation)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build information");
        this.m_hasMoreInformation = this.ReadBuildInformation(this.m_buildInformationMerger);
        if (!this.m_hasMoreInformation)
        {
          if (this.m_hasMoreBuilds)
          {
            this.m_state = CommandQueryBuildsBase.State.BuildDetail;
          }
          else
          {
            this.EnqueueNulls();
            this.m_state = CommandQueryBuildsBase.State.QueuedBuild;
            this.m_result.Builds.IsComplete = true;
          }
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.QueuedBuild)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading queued builds");
        this.m_hasMoreBuilds = this.ReadQueuedBuilds(this.m_dbBuildQueryResult, this.m_result.QueuedBuilds, this.m_queueIds, this.m_queryOptions, (IDictionary<string, BuildDefinition>) this.m_allDefinitions, ref this.m_queuedBuildBinder);
        if (!this.m_hasMoreBuilds)
        {
          this.m_state = CommandQueryBuildsBase.State.BuildServices;
          this.m_result.QueuedBuilds.IsComplete = true;
        }
      }
      if (this.m_state == CommandQueryBuildsBase.State.BuildServices)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Reading build services");
        this.m_dbBuildQueryResult.NextResult();
        this.ReadBuildServices(this.m_dbBuildQueryResult, this.m_queryOptions, this.m_result.Controllers, this.m_result.Agents, this.m_result.ServiceHosts);
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ContinueExecution));
    }

    private void EnqueueNulls()
    {
      while (this.m_currentUriIndex < this.m_uris.Count)
      {
        BuildDetail buildDetail = (BuildDetail) null;
        if (this.m_includeBuild2s && this.m_build2Converter != null)
          buildDetail = this.m_build2Converter.GetBuildByUri(this.RequestContext, this.m_uris[this.m_currentUriIndex], (IList<string>) this.m_informationTypes);
        ++this.m_currentUriIndex;
        this.m_result.Builds.Enqueue(buildDetail);
      }
    }

    private bool ReadBuildsByUri()
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadBuildsByUri));
      if (this.m_buildDetailBinder == null)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting BuildDetail query result");
        this.m_result.Builds.BindCommand((Command) this);
        this.m_result.Builds.IsComplete = false;
        this.m_dbBuildQueryResult.NextResult();
        this.m_buildDetailBinder = this.m_dbBuildQueryResult.GetCurrent<BuildDetail>();
      }
      if (this.m_informationIncluded)
        this.MaxCacheSize = Math.Max(1, this.MaxCacheSize >> 1);
      bool flag = false;
      while (!this.IsCacheFull && (flag = this.m_buildDetailBinder.MoveNext()))
      {
        BuildDetail current = this.m_buildDetailBinder.Current;
        BuildDefinition buildDefinition = (BuildDefinition) null;
        if (!this.m_allDefinitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition) || !buildDefinition.TeamProject.MatchesScope(this.ProjectId) || !this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuilds))
        {
          ++this.m_currentUriIndex;
          this.m_result.Builds.Enqueue((BuildDetail) null);
          this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Enqueued null because definition not found for build '{0}'", (object) current.Uri);
        }
        else
        {
          for (; this.m_currentUriIndex < this.m_uris.Count && !TFStringComparer.ArtiFactUrl.Equals(this.m_uris[this.m_currentUriIndex], current.Uri); ++this.m_currentUriIndex)
          {
            BuildDetail buildDetail = (BuildDetail) null;
            if (this.m_includeBuild2s && this.m_build2Converter != null)
              buildDetail = this.m_build2Converter.GetBuildByUri(this.RequestContext, this.m_uris[this.m_currentUriIndex], (IList<string>) this.m_informationTypes);
            else
              this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Skipping input uri '{0}' that does not match build '{1}'", (object) this.m_uris[this.m_currentUriIndex], (object) current.Uri);
            this.m_result.Builds.Enqueue(buildDetail);
          }
          ++this.m_currentUriIndex;
          this.m_result.Builds.Enqueue(current);
          this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read build '{0}'", (object) current.Uri);
          this.m_controllerUris.Add(current.BuildControllerUri);
          current.TeamProject = buildDefinition.TeamProject.Name;
          current.ProjectId = buildDefinition.TeamProject.Id;
          this.m_queueIds.UnionWith((IEnumerable<int>) current.QueueIds);
          if ((this.m_queryOptions & QueryOptions.Definitions) == QueryOptions.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition))
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
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadBuildsByUri));
      return flag;
    }
  }
}
