// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CommandQueryBuildsBase
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal abstract class CommandQueryBuildsBase : BuildCommand
  {
    protected CommandQueryBuildsBase.State m_state;
    protected BuildDefinitionDictionary m_allDefinitions = new BuildDefinitionDictionary();
    protected HashSet<string> m_controllerUris = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    protected CommandQueryBuildsBase(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext, projectId)
    {
    }

    protected void ReadDefinitions(
      ResultCollection dbResult,
      List<BuildDefinition> definitions,
      Func<BuildDefinition, bool> includeDefinition,
      QueryOptions queryOptions)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadDefinitions));
      bool flag = (queryOptions & QueryOptions.Definitions) == QueryOptions.Definitions;
      ObjectBinder<BuildDefinition> current1 = dbResult.GetCurrent<BuildDefinition>();
      while (current1.MoveNext())
      {
        BuildDefinition current2 = current1.Current;
        if (current2.TeamProject.MatchesScope(this.ProjectId))
        {
          if (this.HasBuildPermission(this.RequestContext, current2.Uri, current2.SecurityToken, BuildPermissions.ViewBuildDefinition) && flag && includeDefinition(current2))
          {
            definitions.Add(current2);
            this.m_controllerUris.Add(current2.BuildControllerUri);
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read definition '{0}'", (object) current2.Uri);
          }
          this.m_allDefinitions[current2.Uri] = current2;
        }
      }
      if (flag)
      {
        BuildDefinition.ReadDatabaseResults(this.RequestContext, definitions, dbResult);
      }
      else
      {
        this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped query results as definitions not to be included");
        this.NextResultAfter<BuildDefinition>(dbResult);
        this.NextResultAfter<RetentionPolicy>(dbResult);
        this.NextResultAfter<Schedule>(dbResult);
        this.NextResultAfter<BuildDefinitionSourceProvider>(dbResult, false);
        this.NextResultAfter<ProcessTemplate>(dbResult);
        this.NextResultAfter<WorkspaceTemplate>(dbResult);
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadDefinitions));
    }

    protected void NextResultAfter<T>(ResultCollection dbResult, bool throwIfIncorrectType = true)
    {
      if (typeof (ObjectBinder<T>).IsAssignableFrom(dbResult.GetCurrent().GetType()))
      {
        dbResult.NextResult();
      }
      else
      {
        if (!throwIfIncorrectType)
          return;
        dbResult.GetCurrent<T>();
      }
    }

    protected bool ReadBuildInformation(BuildInformationMerger buildInformationMerger)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadBuildInformation));
      this.MaxCacheSize = Command.CommandCacheLimit;
      bool flag = false;
      do
        ;
      while (!this.IsCacheFull && buildInformationMerger != null && (flag = buildInformationMerger.TryMergeNext()));
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadBuildInformation));
      return flag;
    }

    protected bool ReadQueuedBuilds(
      ResultCollection dbQueryResult,
      StreamingCollection<QueuedBuild> builds,
      HashSet<int> idsToReturn,
      QueryOptions options,
      IDictionary<string, BuildDefinition> definitions,
      ref ObjectBinder<QueuedBuild> binder)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadQueuedBuilds));
      this.MaxCacheSize = Command.CommandCacheLimit;
      if (binder == null)
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "Build", "Command", "Getting QueuedBuild query result");
        builds.BindCommand((Command) this);
        builds.IsComplete = false;
        dbQueryResult.NextResult();
        binder = dbQueryResult.GetCurrent<QueuedBuild>();
      }
      bool flag = false;
      while (!this.IsCacheFull && (flag = binder.MoveNext()))
      {
        QueuedBuild current = binder.Current;
        if (!idsToReturn.Contains(current.Id))
        {
          this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Skipped queued build '{0}'", (object) current.Id);
        }
        else
        {
          BuildDefinition buildDefinition;
          if (definitions.TryGetValue(current.BuildDefinitionUri, out buildDefinition))
          {
            current.TeamProject = buildDefinition.TeamProject.Name;
            current.ProjectId = buildDefinition.TeamProject.Id;
            if ((options & QueryOptions.Definitions) == QueryOptions.Definitions && this.HasBuildPermission(this.RequestContext, buildDefinition.Uri, buildDefinition.SecurityToken, BuildPermissions.ViewBuildDefinition))
            {
              current.Definition = buildDefinition;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Associated definition '{0}' with queued build '{1}'", (object) buildDefinition.Uri, (object) current.Id);
            }
          }
          builds.Enqueue(current);
          this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read queued build '{0}'", (object) current.Id);
        }
      }
      this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadQueuedBuilds));
      return flag;
    }

    protected void ReadBuildServices(
      ResultCollection dbResult,
      QueryOptions options,
      List<BuildController> controllers,
      List<BuildAgent> agents,
      List<BuildServiceHost> serviceHosts)
    {
      this.RequestContext.TraceEnter(0, "Build", "Command", nameof (ReadBuildServices));
      if ((options & QueryOptions.Controllers | QueryOptions.Agents) == QueryOptions.None || !this.BuildHost.SecurityManager.HasPrivilege(this.RequestContext, AdministrationPermissions.ViewBuildResources, false))
      {
        this.RequestContext.Trace(0, TraceLevel.Warning, "Build", "Command", "Exiting no build resources");
      }
      else
      {
        Dictionary<string, BuildServiceHost> dictionary = new Dictionary<string, BuildServiceHost>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if ((options & QueryOptions.Controllers) == QueryOptions.Controllers)
        {
          ObjectBinder<BuildController> current = dbResult.GetCurrent<BuildController>();
          while (current.MoveNext())
          {
            if (this.m_controllerUris.Contains(current.Current.Uri))
            {
              controllers.Add(current.Current);
              dictionary[current.Current.ServiceHostUri] = (BuildServiceHost) null;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read controller '{0}'", (object) current.Current.Uri);
            }
          }
        }
        dbResult.NextResult();
        if ((options & QueryOptions.Agents) == QueryOptions.Agents)
        {
          ObjectBinder<BuildAgent> current = dbResult.GetCurrent<BuildAgent>();
          while (current.MoveNext())
          {
            if (this.m_controllerUris.Contains(current.Current.ControllerUri))
            {
              agents.Add(current.Current);
              dictionary[current.Current.ServiceHostUri] = (BuildServiceHost) null;
              this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read agent '{0}'", (object) current.Current.Uri);
            }
          }
        }
        dbResult.NextResult();
        ObjectBinder<BuildServiceHost> current1 = dbResult.GetCurrent<BuildServiceHost>();
        while (current1.MoveNext())
        {
          if (dictionary.ContainsKey(current1.Current.Uri))
          {
            serviceHosts.Add(current1.Current);
            dictionary[current1.Current.Uri] = current1.Current;
            this.RequestContext.Trace(0, TraceLevel.Info, "Build", "Command", "Read service host '{0}'", (object) current1.Current.Uri);
          }
        }
        this.RequestContext.TraceLeave(0, "Build", "Command", nameof (ReadBuildServices));
      }
    }

    protected enum State
    {
      BuildDefinition,
      QueuedBuild,
      BuildDetail,
      BuildInformation,
      BuildServices,
    }
  }
}
