// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GatedBuildDefinitionCache
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class GatedBuildDefinitionCache : IVssFrameworkService
  {
    private DateTime m_lastErrorLogged = DateTime.MinValue;
    private bool m_initialized;
    private bool m_fullRefresh;
    private HashSet<Tuple<Guid, int>> m_definitionsToUpdate = new HashSet<Tuple<Guid, int>>();
    private bool m_disposed;
    private ReaderWriterLock m_cacheLock = new ReaderWriterLock();
    private Dictionary<int, BuildDefinition> m_definitionCache = new Dictionary<int, BuildDefinition>();
    private SparseTree<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>> m_definitionMappings = new SparseTree<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>>('/', StringComparison.OrdinalIgnoreCase);

    internal IEnumerable<BuildDefinition> GetAffectedBuildDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> serverItems,
      Func<string, string, bool> ignoreDefinition)
    {
      requestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", nameof (GetAffectedBuildDefinitions));
      if (requestContext.IsServicingContext)
      {
        requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Exiting if servicing context");
        return (IEnumerable<BuildDefinition>) Array.Empty<BuildDefinition>();
      }
      if (!this.LoadCache(requestContext))
        this.EnsureCacheUpToDate(requestContext, (object) false);
      Dictionary<int, BuildDefinition> dictionary1 = new Dictionary<int, BuildDefinition>();
      try
      {
        this.m_cacheLock.AcquireReaderLock(-1);
        requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_cacheLock with Timeout.Infinite");
        foreach (string serverItem in serverItems)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Finding affected definitions for server item '{0}'", (object) serverItem);
          Dictionary<int, BuildDefinition> dictionary2 = new Dictionary<int, BuildDefinition>();
          foreach (List<GatedBuildDefinitionCache.GroupedWorkspaceMapping> workspaceMappingList in this.m_definitionMappings.EnumParents(serverItem, EnumParentsOptions.None).Select<EnumeratedSparseTreeNode<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>>, List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>>((Func<EnumeratedSparseTreeNode<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>>, List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>>) (s => s.ReferencedObject)))
          {
            foreach (GatedBuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping in workspaceMappingList)
            {
              if (!workspaceMapping.Include)
              {
                foreach (BuildDefinition definition in workspaceMapping.Definitions)
                {
                  if (!dictionary2.ContainsKey(definition.Id))
                  {
                    dictionary2.Add(definition.Id, (BuildDefinition) null);
                    requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to currentAffectedDefinitions", (object) definition.Uri);
                  }
                }
              }
              else if (workspaceMapping.Depth == 120 || workspaceMapping.Depth == 1 && VersionControlPath.IsImmediateChild(serverItem, workspaceMapping.Item))
              {
                foreach (BuildDefinition definition in workspaceMapping.Definitions)
                {
                  if (!dictionary2.ContainsKey(definition.Id))
                  {
                    dictionary2.Add(definition.Id, definition);
                    requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to currentAffectedDefinitions", (object) definition.Uri);
                  }
                }
              }
              else
                requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Skipped grouped workspace mapping '{0}'", (object) workspaceMapping.Item);
            }
          }
          foreach (KeyValuePair<int, BuildDefinition> keyValuePair in dictionary2)
          {
            if (keyValuePair.Value != null)
            {
              dictionary1[keyValuePair.Key] = keyValuePair.Value;
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to affectedDefinitions", (object) keyValuePair.Key);
            }
          }
          if (dictionary1.Count == this.m_definitionCache.Count)
          {
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Reached the point where all gated definitions have been found to be affected");
            break;
          }
        }
        BuildDefinition[] array = dictionary1.Values.ToArray<BuildDefinition>();
        for (int index = 0; index < array.Length; ++index)
        {
          if (ignoreDefinition(array[index].Name, array[index].GetToken()))
          {
            dictionary1.Remove(array[index].Id);
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Removed definition '{0}' from affectedDefinitions", (object) array[index].Id);
          }
        }
      }
      finally
      {
        if (this.m_cacheLock.IsReaderLockHeld || this.m_cacheLock.IsWriterLockHeld)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_cacheLock");
          this.m_cacheLock.ReleaseReaderLock();
        }
      }
      requestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", nameof (GetAffectedBuildDefinitions));
      return (IEnumerable<BuildDefinition>) dictionary1.Values;
    }

    public void Invalidate(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<Guid, int>> definitionIds)
    {
      requestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", nameof (Invalidate));
      if (requestContext.IsServicingContext)
      {
        requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Exiting if servicing context");
      }
      else
      {
        if (!this.m_disposed)
        {
          lock (this.m_definitionsToUpdate)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_definitionsToUpdate");
            if (definitionIds != null && !this.m_fullRefresh)
            {
              this.m_definitionsToUpdate.UnionWith(definitionIds);
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Set to do a partial refresh");
            }
            else
            {
              this.m_fullRefresh = true;
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Set to do a full refresh");
            }
            requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_definitionsToUpdate");
          }
          try
          {
            this.m_cacheLock.AcquireWriterLock(-1);
            requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_cacheLock with Timeout.Infinite");
            if (this.m_initialized)
              this.GetTaskService(requestContext.Elevate()).AddTask(requestContext.Elevate(), new TeamFoundationTaskCallback(this.EnsureCacheUpToDate));
          }
          finally
          {
            if (this.m_cacheLock.IsWriterLockHeld)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_cacheLock");
              this.m_cacheLock.ReleaseWriterLock();
            }
          }
        }
        requestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", nameof (Invalidate));
      }
    }

    internal BuildDefinition GetDefinition(IVssRequestContext requestContext, string definitionUri)
    {
      requestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", nameof (GetDefinition));
      if (!this.LoadCache(requestContext))
        this.EnsureCacheUpToDate(requestContext, (object) false);
      try
      {
        this.m_cacheLock.AcquireReaderLock(-1);
        int result = 0;
        BuildDefinition definition = (BuildDefinition) null;
        if (!int.TryParse(LinkingUtilities.DecodeUri(definitionUri).ToolSpecificId, out result) || !this.m_definitionCache.TryGetValue(result, out definition))
          definition = (BuildDefinition) null;
        return definition;
      }
      finally
      {
        if (this.m_cacheLock.IsReaderLockHeld || this.m_cacheLock.IsWriterLockHeld)
          this.m_cacheLock.ReleaseReaderLock();
        requestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", nameof (GetDefinition));
      }
    }

    private bool LoadCache(IVssRequestContext requestContext)
    {
      if (this.m_initialized)
        return false;
      bool flag = false;
      try
      {
        this.m_cacheLock.AcquireWriterLock(-1);
        requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_cacheLock with Timeout.Infinite");
        if (this.m_initialized)
          return false;
        this.EnsureCacheUpToDate(requestContext, (object) false);
        this.m_initialized = true;
        flag = this.m_initialized;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build2", "GatedCheckInEvaluator", ex);
        DateTime utcNow = DateTime.UtcNow;
        if ((utcNow - this.m_lastErrorLogged).TotalMinutes > 1.0)
          this.m_lastErrorLogged = utcNow;
      }
      finally
      {
        if (this.m_cacheLock.IsWriterLockHeld)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_cacheLock");
          this.m_cacheLock.ReleaseWriterLock();
        }
      }
      return flag;
    }

    private void EnsureCacheUpToDate(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", "Reload");
      try
      {
        bool flag = false;
        List<Tuple<Guid, int>> tupleList = new List<Tuple<Guid, int>>();
        lock (this.m_definitionsToUpdate)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_definitionsToUpdate");
          if (this.m_fullRefresh)
          {
            flag = true;
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Will do a full refresh");
          }
          else
          {
            tupleList.AddRange((IEnumerable<Tuple<Guid, int>>) this.m_definitionsToUpdate);
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Will do a partial refresh");
          }
          this.m_fullRefresh = false;
          this.m_definitionsToUpdate.Clear();
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_definitionsToUpdate");
        }
        if (!flag && tupleList.Count == 0)
          return;
        IVssRequestContext requestContext1 = requestContext.Elevate();
        List<BuildDefinition> buildDefinitionList = new List<BuildDefinition>();
        List<GatedBuildDefinitionCache.BuildDefinitionUpdate> definitionUpdateList = new List<GatedBuildDefinitionCache.BuildDefinitionUpdate>();
        if (flag)
        {
          IBuildDefinitionService service = requestContext.GetService<IBuildDefinitionService>();
          foreach (ProjectInfo project in requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate(), ProjectState.WellFormed))
          {
            IEnumerable<BuildDefinition> definitionsForRepository = service.GetDefinitionsForRepository(requestContext1, project.Id, "TfsVersionControl", (string) null, triggers: DefinitionTriggerType.GatedCheckIn, options: DefinitionQueryOptions.Repository | DefinitionQueryOptions.Triggers | DefinitionQueryOptions.Variables);
            buildDefinitionList.AddRange(definitionsForRepository.Where<BuildDefinition>((Func<BuildDefinition, bool>) (x =>
            {
              DefinitionQuality? definitionQuality1 = x.DefinitionQuality;
              DefinitionQuality definitionQuality2 = DefinitionQuality.Definition;
              return definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue;
            })));
          }
        }
        int index = -1;
        HashSet<int> intSet = new HashSet<int>();
        foreach (BuildDefinition buildDefinition in buildDefinitionList)
        {
          buildDefinition.ConvertGatedTriggerPathFilters(requestContext1);
          BuildDefinition existingDefinition = (BuildDefinition) null;
          ++index;
          if (flag)
          {
            intSet.Add(buildDefinition.Id);
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to processedDefinitions", (object) buildDefinition.Id);
          }
          else if (buildDefinition == null)
          {
            if (this.m_definitionCache.TryGetValue(tupleList[index].Item2, out existingDefinition))
            {
              definitionUpdateList.Add(new GatedBuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to definitionUpdates for removing", (object) tupleList[index]);
            }
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "The returned definition for uri '{0}' is null, continue", (object) tupleList[index]);
            continue;
          }
          if (!this.m_definitionCache.TryGetValue(buildDefinition.Id, out existingDefinition))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Existing definition '{0}' not found", (object) buildDefinition.Uri);
            if (buildDefinition.QueueStatus == DefinitionQueueStatus.Enabled || buildDefinition.QueueStatus == DefinitionQueueStatus.Paused)
            {
              definitionUpdateList.Add(new GatedBuildDefinitionCache.BuildDefinitionUpdate(buildDefinition, (BuildDefinition) null));
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to definitionUpdates for adding", (object) buildDefinition.Uri);
            }
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Existing definition '{0}' found", (object) buildDefinition.Uri);
            if (buildDefinition.QueueStatus == DefinitionQueueStatus.Disabled)
            {
              definitionUpdateList.Add(new GatedBuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added disabled definition '{0}' to definitionUpdates for removing", (object) buildDefinition.Uri);
            }
            else
            {
              definitionUpdateList.Add(new GatedBuildDefinitionCache.BuildDefinitionUpdate(buildDefinition, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to definitionUpdates for updating", (object) buildDefinition.Uri);
            }
          }
        }
        if (flag)
        {
          foreach (BuildDefinition existingDefinition in this.m_definitionCache.Values)
          {
            if (!intSet.Contains(existingDefinition.Id))
            {
              definitionUpdateList.Add(new GatedBuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build2", "GatedCheckInEvaluator", "Added definition '{0}' to definitionUpdates for removing", (object) existingDefinition.Uri);
            }
          }
        }
        try
        {
          this.m_cacheLock.AcquireWriterLock(-1);
          requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Acquired lock m_cacheLock with Timeout.Infinite");
          foreach (GatedBuildDefinitionCache.BuildDefinitionUpdate definitionUpdate in definitionUpdateList)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Executing update '{0}'", (object) definitionUpdate);
            definitionUpdate.Execute(this.m_definitionMappings, (IDictionary<int, BuildDefinition>) this.m_definitionCache);
          }
        }
        finally
        {
          if (this.m_cacheLock.IsWriterLockHeld)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Releasing lock m_cacheLock");
            this.m_cacheLock.ReleaseWriterLock();
          }
        }
      }
      catch (Exception ex)
      {
        if (!this.m_initialized)
        {
          throw;
        }
        else
        {
          requestContext.TraceException(0, "Build2", "GatedCheckInEvaluator", ex);
          DateTime utcNow = DateTime.UtcNow;
          if ((utcNow - this.m_lastErrorLogged).TotalMinutes > 1.0)
            this.m_lastErrorLogged = utcNow;
          this.m_fullRefresh = true;
          if (!this.m_disposed)
          {
            if (taskArgs == null)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Requeuing Reload task in 10 seconds");
              this.GetTaskService(requestContext.Elevate()).AddTask(requestContext.Elevate(), new TeamFoundationTask(new TeamFoundationTaskCallback(this.EnsureCacheUpToDate), (object) null, DateTime.UtcNow.AddSeconds(10.0), 0));
            }
          }
        }
      }
      requestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", "Reload");
    }

    private TeamFoundationTaskService GetTaskService(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Get task service");
      return systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
    }

    private void OnGatedDefinitionChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      this.Invalidate(requestContext, (IEnumerable<Tuple<Guid, int>>) null);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", "BuildDefinitionCache");
      this.m_fullRefresh = true;
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "m_initializedEvent reset");
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        systemRequestContext.Trace(0, TraceLevel.Error, "Build2", "GatedCheckInEvaluator", "Parent service host is null");
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      }
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, DatabaseCategories.Build, SqlNotifications.GatedDefinitionChanged, new SqlNotificationHandler(this.OnGatedDefinitionChanged), true);
      systemRequestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", "BuildDefinitionCache");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build2", "GatedCheckInEvaluator", "Unload");
      if (!systemRequestContext.IsServicingContext)
      {
        systemRequestContext.Trace(0, TraceLevel.Verbose, "Build2", "GatedCheckInEvaluator", "Removing Reload task if non-servicing context");
        this.GetTaskService(systemRequestContext).RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.EnsureCacheUpToDate));
      }
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, DatabaseCategories.Build, SqlNotifications.GatedDefinitionChanged, new SqlNotificationHandler(this.OnGatedDefinitionChanged), false);
      this.m_disposed = true;
      systemRequestContext.TraceLeave(0, "Build2", "GatedCheckInEvaluator", "Unload");
    }

    private sealed class BuildDefinitionUpdate
    {
      private BuildDefinition m_newDefinition;
      private BuildDefinition m_existingDefinition;

      public BuildDefinitionUpdate(
        BuildDefinition newDefinition,
        BuildDefinition existingDefinition)
      {
        this.m_newDefinition = newDefinition;
        this.m_existingDefinition = existingDefinition;
      }

      public void Execute(
        SparseTree<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        IDictionary<int, BuildDefinition> definitionCache)
      {
        if (this.m_newDefinition == null)
        {
          this.RemoveMappings(gatedWorkspaces, this.m_existingDefinition);
          definitionCache.Remove(this.m_existingDefinition.Id);
        }
        else
        {
          if (this.m_existingDefinition != null)
            this.RemoveMappings(gatedWorkspaces, this.m_existingDefinition);
          this.AddMappings(gatedWorkspaces, this.m_newDefinition);
          definitionCache[this.m_newDefinition.Id] = this.m_newDefinition;
        }
      }

      private void AddMappings(
        SparseTree<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        BuildDefinition definition)
      {
        List<BuildTrigger> triggers = definition.Triggers;
        GatedCheckInTrigger gatedCheckInTrigger = triggers != null ? triggers.OfType<GatedCheckInTrigger>().FirstOrDefault<GatedCheckInTrigger>() : (GatedCheckInTrigger) null;
        if (gatedCheckInTrigger == null)
          return;
        foreach (string pathFilter in gatedCheckInTrigger.PathFilters)
        {
          string token;
          ref string local1 = ref token;
          bool include;
          ref bool local2 = ref include;
          int depth;
          ref int local3 = ref depth;
          if (GatedBuildDefinitionCache.BuildDefinitionUpdate.ParseFilter(pathFilter, out local1, out local2, out local3))
          {
            List<GatedBuildDefinitionCache.GroupedWorkspaceMapping> referencedObject;
            if (gatedWorkspaces.TryGetValue(token, out referencedObject))
            {
              GatedBuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping = referencedObject.Find((Predicate<GatedBuildDefinitionCache.GroupedWorkspaceMapping>) (x => x.Include == include && x.Depth == depth));
              if (workspaceMapping != null)
                workspaceMapping.Definitions.Add(definition);
              else
                referencedObject.Add(new GatedBuildDefinitionCache.GroupedWorkspaceMapping(token, include, depth, definition));
            }
            else
            {
              referencedObject = new List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>(1);
              referencedObject.Add(new GatedBuildDefinitionCache.GroupedWorkspaceMapping(token, include, depth, definition));
              gatedWorkspaces.Add(token, referencedObject);
            }
          }
        }
      }

      private void RemoveMappings(
        SparseTree<List<GatedBuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        BuildDefinition definition)
      {
        BuildDefinition buildDefinition = definition;
        GatedCheckInTrigger gatedCheckInTrigger1;
        if (buildDefinition == null)
        {
          gatedCheckInTrigger1 = (GatedCheckInTrigger) null;
        }
        else
        {
          List<BuildTrigger> triggers = buildDefinition.Triggers;
          if (triggers == null)
          {
            gatedCheckInTrigger1 = (GatedCheckInTrigger) null;
          }
          else
          {
            IEnumerable<GatedCheckInTrigger> source = triggers.OfType<GatedCheckInTrigger>();
            gatedCheckInTrigger1 = source != null ? source.FirstOrDefault<GatedCheckInTrigger>() : (GatedCheckInTrigger) null;
          }
        }
        GatedCheckInTrigger gatedCheckInTrigger2 = gatedCheckInTrigger1;
        if (gatedCheckInTrigger2 == null)
          return;
        foreach (string pathFilter in gatedCheckInTrigger2.PathFilters)
        {
          string token;
          ref string local1 = ref token;
          bool include;
          ref bool local2 = ref include;
          int depth;
          ref int local3 = ref depth;
          List<GatedBuildDefinitionCache.GroupedWorkspaceMapping> referencedObject;
          if (GatedBuildDefinitionCache.BuildDefinitionUpdate.ParseFilter(pathFilter, out local1, out local2, out local3) && gatedWorkspaces.TryGetValue(token, out referencedObject))
          {
            GatedBuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping = referencedObject.FirstOrDefault<GatedBuildDefinitionCache.GroupedWorkspaceMapping>((Func<GatedBuildDefinitionCache.GroupedWorkspaceMapping, bool>) (x => x.Include == include && x.Depth == depth));
            if (workspaceMapping != null)
            {
              int index = workspaceMapping.Definitions.FindIndex((Predicate<BuildDefinition>) (x => StringComparer.OrdinalIgnoreCase.Equals((object) x.Uri, (object) definition.Uri)));
              if (index >= 0)
              {
                workspaceMapping.Definitions.RemoveAt(index);
                if (workspaceMapping.Definitions.Count == 0 && referencedObject.Remove(workspaceMapping) && referencedObject.Count == 0)
                  gatedWorkspaces.Remove(workspaceMapping.Item, false);
              }
            }
          }
        }
      }

      private static bool ParseFilter(
        string pathFilter,
        out string serverPath,
        out bool include,
        out int depth)
      {
        bool excludeBranch;
        bool isPattern;
        BuildSourceProviders.GitProperties.ParseBranchSpec(pathFilter, out excludeBranch, out serverPath, out isPattern, true);
        include = !excludeBranch;
        depth = isPattern ? 1 : 120;
        return VersionControlPath.IsValidPath(serverPath);
      }

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinitionUpdate ExistingDefinition={0} NewDefinition={1}]", this.m_existingDefinition == null ? (object) (Uri) null : (object) this.m_existingDefinition.Uri, this.m_newDefinition == null ? (object) (Uri) null : (object) this.m_newDefinition.Uri);
    }

    private sealed class GroupedWorkspaceMapping
    {
      private int m_depth;
      private string m_item;
      private bool m_include;
      private List<BuildDefinition> m_definitions;

      public GroupedWorkspaceMapping(
        string item,
        bool include,
        int depth,
        BuildDefinition definition)
      {
        this.m_item = item;
        this.m_depth = depth;
        this.m_include = include;
        this.m_definitions = new List<BuildDefinition>()
        {
          definition
        };
      }

      public string Item => this.m_item;

      public bool Include => this.m_include;

      public int Depth => this.m_depth;

      public List<BuildDefinition> Definitions => this.m_definitions;
    }
  }
}
