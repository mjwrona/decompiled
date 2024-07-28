// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionCache
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class BuildDefinitionCache
  {
    private DateTime m_lastErrorLogged = DateTime.MinValue;
    private bool m_initialized;
    private bool m_fullRefresh;
    private HashSet<string> m_definitionsToUpdate = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private bool m_disposed;
    private ReaderWriterLock m_cacheLock = new ReaderWriterLock();
    private Dictionary<string, BuildDefinition> m_definitionCache = new Dictionary<string, BuildDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private SparseTree<List<BuildDefinitionCache.GroupedWorkspaceMapping>> m_definitionMappings = new SparseTree<List<BuildDefinitionCache.GroupedWorkspaceMapping>>('/', StringComparison.OrdinalIgnoreCase);

    internal BuildDefinitionCache(
      TeamFoundationBuildService buildService,
      IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build", "Cache", nameof (BuildDefinitionCache));
      this.m_fullRefresh = true;
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "m_initializedEvent reset");
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        systemRequestContext.Trace(0, TraceLevel.Error, "Build", "Cache", "Parent service host is null");
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      }
      systemRequestContext.TraceLeave(0, "Build", "Cache", nameof (BuildDefinitionCache));
    }

    private TeamFoundationTaskService GetTaskService(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Get task service");
      return systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
    }

    public void Unload(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, "Build", "Cache", nameof (Unload));
      if (!requestContext.IsServicingContext)
      {
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Removing Reload task if non-servicing context");
        this.GetTaskService(requestContext).RemoveTask(requestContext.Elevate(), new TeamFoundationTaskCallback(this.EnsureCacheUpToDate));
      }
      this.m_disposed = true;
      requestContext.TraceLeave(0, "Build", "Cache", nameof (Unload));
    }

    public void Invalidate(IVssRequestContext requestContext, IEnumerable<string> uris)
    {
      requestContext.TraceEnter(0, "Build", "Cache", nameof (Invalidate));
      if (requestContext.IsServicingContext)
      {
        requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Exiting if servicing context");
      }
      else
      {
        if (!this.m_disposed)
        {
          lock (this.m_definitionsToUpdate)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_definitionsToUpdate");
            if (uris != null && !this.m_fullRefresh)
            {
              this.m_definitionsToUpdate.UnionWith(uris);
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Set to do a partial refresh");
            }
            else
            {
              this.m_fullRefresh = true;
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Set to do a full refresh");
            }
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_definitionsToUpdate");
          }
          try
          {
            this.m_cacheLock.AcquireWriterLock(-1);
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_cacheLock with Timeout.Infinite");
            if (this.m_initialized)
              this.GetTaskService(requestContext.Elevate()).AddTask(requestContext.Elevate(), new TeamFoundationTaskCallback(this.EnsureCacheUpToDate));
          }
          finally
          {
            if (this.m_cacheLock.IsWriterLockHeld)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_cacheLock");
              this.m_cacheLock.ReleaseWriterLock();
            }
          }
        }
        requestContext.TraceLeave(0, "Build", "Cache", nameof (Invalidate));
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
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_cacheLock with Timeout.Infinite");
        if (this.m_initialized)
          return false;
        this.EnsureCacheUpToDate(requestContext, (object) false);
        this.m_initialized = true;
        flag = this.m_initialized;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Build", "Cache", ex);
        DateTime utcNow = DateTime.UtcNow;
        if ((utcNow - this.m_lastErrorLogged).TotalMinutes > 1.0)
        {
          this.m_lastErrorLogged = utcNow;
          TeamFoundationEventLog.Default.LogException(ResourceStrings.BuildDefinitionCacheUpdateError(), ex);
        }
      }
      finally
      {
        if (this.m_cacheLock.IsWriterLockHeld)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_cacheLock");
          this.m_cacheLock.ReleaseWriterLock();
        }
      }
      return flag;
    }

    private void EnsureCacheUpToDate(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(0, "Build", "Cache", "Reload");
      try
      {
        bool flag = false;
        List<string> uris = new List<string>();
        lock (this.m_definitionsToUpdate)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_definitionsToUpdate");
          if (this.m_fullRefresh)
          {
            flag = true;
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Will do a full refresh");
          }
          else
          {
            uris.AddRange((IEnumerable<string>) this.m_definitionsToUpdate);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Will do a partial refresh");
          }
          this.m_fullRefresh = false;
          this.m_definitionsToUpdate.Clear();
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_definitionsToUpdate");
        }
        if (!flag && uris.Count == 0)
          return;
        List<BuildDefinitionCache.BuildDefinitionUpdate> definitionUpdateList = new List<BuildDefinitionCache.BuildDefinitionUpdate>();
        TeamFoundationBuildService service = requestContext.Elevate().GetService<TeamFoundationBuildService>();
        BuildDefinitionQueryResult definitionQueryResult;
        try
        {
          if (flag)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Getting all build definitions for a full refresh");
            BuildDefinitionSpec spec = new BuildDefinitionSpec()
            {
              TriggerType = DefinitionTriggerType.All,
              FullPath = BuildConstants.Star + BuildPath.PathSeparator + BuildConstants.Star,
              Options = QueryOptions.Workspaces
            };
            definitionQueryResult = service.QueryBuildDefinitions(requestContext.Elevate(), spec);
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Getting build definitions for a partial refresh");
            definitionQueryResult = service.QueryBuildDefinitionsByUri(requestContext.Elevate(), (IList<string>) uris, (IList<string>) null, QueryOptions.Workspaces, new Guid());
          }
        }
        catch (ProjectDoesNotExistException ex)
        {
          requestContext.TraceException(0, "Build", "Cache", (Exception) ex);
          service.EnsureBuildGroups(requestContext);
          if (flag)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Retrying getting all build definitions for a full refresh");
            BuildDefinitionSpec spec = new BuildDefinitionSpec()
            {
              TriggerType = DefinitionTriggerType.All,
              FullPath = BuildConstants.Star + BuildPath.PathSeparator + BuildConstants.Star,
              Options = QueryOptions.Workspaces
            };
            definitionQueryResult = service.QueryBuildDefinitions(requestContext.Elevate(), spec);
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Retrying getting build definitions for a partial refresh");
            definitionQueryResult = service.QueryBuildDefinitionsByUri(requestContext.Elevate(), (IList<string>) uris, (IList<string>) null, QueryOptions.Workspaces, new Guid());
          }
        }
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        int index = -1;
        foreach (BuildDefinition definition in definitionQueryResult.Definitions)
        {
          BuildDefinition existingDefinition = (BuildDefinition) null;
          ++index;
          if (flag)
          {
            stringSet.Add(definition.Uri);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to processedDefinitions", (object) definition.Uri);
          }
          else if (definition == null)
          {
            if (this.m_definitionCache.TryGetValue(uris[index], out existingDefinition))
            {
              definitionUpdateList.Add(new BuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to definitionUpdates for removing", (object) uris[index]);
            }
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "The returned definition for uri '{0}' is null, continue", (object) uris[index]);
            continue;
          }
          if (!this.m_definitionCache.TryGetValue(definition.Uri, out existingDefinition))
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Existing definition '{0}' not found", (object) definition.Uri);
            if (definition.QueueStatus == DefinitionQueueStatus.Enabled || definition.QueueStatus == DefinitionQueueStatus.Paused)
            {
              definitionUpdateList.Add(new BuildDefinitionCache.BuildDefinitionUpdate(definition, (BuildDefinition) null));
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to definitionUpdates for adding", (object) definition.Uri);
            }
          }
          else
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Existing definition '{0}' found", (object) definition.Uri);
            if (definition.QueueStatus == DefinitionQueueStatus.Disabled)
            {
              definitionUpdateList.Add(new BuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added disabled definition '{0}' to definitionUpdates for removing", (object) definition.Uri);
            }
            else
            {
              definitionUpdateList.Add(new BuildDefinitionCache.BuildDefinitionUpdate(definition, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to definitionUpdates for updating", (object) definition.Uri);
            }
          }
        }
        if (flag)
        {
          foreach (BuildDefinition existingDefinition in this.m_definitionCache.Values)
          {
            if (!stringSet.Contains(existingDefinition.Uri))
            {
              definitionUpdateList.Add(new BuildDefinitionCache.BuildDefinitionUpdate((BuildDefinition) null, existingDefinition));
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to definitionUpdates for removing", (object) existingDefinition.Uri);
            }
          }
        }
        try
        {
          this.m_cacheLock.AcquireWriterLock(-1);
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_cacheLock with Timeout.Infinite");
          foreach (BuildDefinitionCache.BuildDefinitionUpdate definitionUpdate in definitionUpdateList)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Executing update '{0}'", (object) definitionUpdate);
            definitionUpdate.Execute(this.m_definitionMappings, (IDictionary<string, BuildDefinition>) this.m_definitionCache);
          }
        }
        finally
        {
          if (this.m_cacheLock.IsWriterLockHeld)
          {
            requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_cacheLock");
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
          requestContext.TraceException(0, "Build", "Cache", ex);
          DateTime utcNow = DateTime.UtcNow;
          if ((utcNow - this.m_lastErrorLogged).TotalMinutes > 1.0)
          {
            this.m_lastErrorLogged = utcNow;
            TeamFoundationEventLog.Default.LogException(ResourceStrings.BuildDefinitionCacheUpdateError(), ex);
          }
          this.m_fullRefresh = true;
          if (!this.m_disposed)
          {
            if (taskArgs == null)
            {
              requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Requeuing Reload task in 10 seconds");
              this.GetTaskService(requestContext.Elevate()).AddTask(requestContext.Elevate(), new TeamFoundationTask(new TeamFoundationTaskCallback(this.EnsureCacheUpToDate), (object) null, DateTime.UtcNow.AddSeconds(10.0), 0));
            }
          }
        }
      }
      requestContext.TraceLeave(0, "Build", "Cache", "Reload");
    }

    internal Dictionary<string, BuildDefinition> GetAffectedBuildDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> serverItems,
      DefinitionTriggerType continuousIntegrationType,
      Func<BuildDefinition, bool> ignoreDefinition,
      bool readAllItems)
    {
      requestContext.TraceEnter(0, "Build", "Cache", nameof (GetAffectedBuildDefinitions));
      if (requestContext.IsServicingContext)
      {
        requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Exiting if servicing context");
        return new Dictionary<string, BuildDefinition>();
      }
      if (!this.LoadCache(requestContext))
        this.EnsureCacheUpToDate(requestContext, (object) false);
      Dictionary<string, BuildDefinition> buildDefinitions = new Dictionary<string, BuildDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      try
      {
        this.m_cacheLock.AcquireReaderLock(-1);
        requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Acquired lock m_cacheLock with Timeout.Infinite");
        foreach (string serverItem in serverItems)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Finding affected definitions for server item '{0}'", (object) serverItem);
          Dictionary<string, BuildDefinition> dictionary = new Dictionary<string, BuildDefinition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (List<BuildDefinitionCache.GroupedWorkspaceMapping> workspaceMappingList in this.m_definitionMappings.EnumParents(serverItem, EnumParentsOptions.None).Select<EnumeratedSparseTreeNode<List<BuildDefinitionCache.GroupedWorkspaceMapping>>, List<BuildDefinitionCache.GroupedWorkspaceMapping>>((Func<EnumeratedSparseTreeNode<List<BuildDefinitionCache.GroupedWorkspaceMapping>>, List<BuildDefinitionCache.GroupedWorkspaceMapping>>) (s => s.ReferencedObject)))
          {
            foreach (BuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping in workspaceMappingList)
            {
              if (workspaceMapping.Type == WorkspaceMappingType.Cloak)
              {
                foreach (BuildDefinition definition in workspaceMapping.Definitions)
                {
                  if ((definition.TriggerType & continuousIntegrationType) == (DefinitionTriggerType) 0)
                    requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Skipped non-continuous-integration definition '{0}'", (object) definition.Uri);
                  else if (!dictionary.ContainsKey(definition.Uri))
                  {
                    dictionary.Add(definition.Uri, (BuildDefinition) null);
                    requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to currentAffectedDefinitions", (object) definition.Uri);
                  }
                }
              }
              else if (workspaceMapping.Depth == 120 || workspaceMapping.Depth == 1 && VersionControlPath.IsImmediateChild(serverItem, workspaceMapping.Item))
              {
                foreach (BuildDefinition definition in workspaceMapping.Definitions)
                {
                  if ((definition.TriggerType & continuousIntegrationType) == (DefinitionTriggerType) 0)
                    requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Skipped non-continuous-integration definition '{0}'", (object) definition.Uri);
                  else if (!dictionary.ContainsKey(definition.Uri))
                  {
                    dictionary.Add(definition.Uri, definition);
                    requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to currentAffectedDefinitions", (object) definition.Uri);
                  }
                }
              }
              else
                requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Skipped grouped workspace mapping '{0}'", (object) workspaceMapping.Item);
            }
          }
          foreach (KeyValuePair<string, BuildDefinition> keyValuePair in dictionary)
          {
            if (keyValuePair.Value != null)
            {
              buildDefinitions[keyValuePair.Key] = keyValuePair.Value;
              requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Added definition '{0}' to affectedDefinitions", (object) keyValuePair.Key);
            }
          }
          if (!readAllItems && buildDefinitions.Count == this.m_definitionCache.Count)
          {
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Reached the point where all gated definitions have been found to be affected");
            break;
          }
        }
        BuildDefinition[] array = buildDefinitions.Values.ToArray<BuildDefinition>();
        for (int index = 0; index < array.Length; ++index)
        {
          if (ignoreDefinition(array[index]))
          {
            buildDefinitions.Remove(array[index].Uri);
            requestContext.Trace(0, TraceLevel.Info, "Build", "Cache", "Removed definition '{0}' from affectedDefinitions", (object) array[index].Uri);
          }
        }
      }
      finally
      {
        if (this.m_cacheLock.IsReaderLockHeld || this.m_cacheLock.IsWriterLockHeld)
        {
          requestContext.Trace(0, TraceLevel.Verbose, "Build", "Cache", "Releasing lock m_cacheLock");
          this.m_cacheLock.ReleaseReaderLock();
        }
      }
      requestContext.TraceLeave(0, "Build", "Cache", nameof (GetAffectedBuildDefinitions));
      return buildDefinitions;
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
        SparseTree<List<BuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        IDictionary<string, BuildDefinition> definitionCache)
      {
        if (this.m_newDefinition == null)
        {
          this.RemoveMappings(gatedWorkspaces, this.m_existingDefinition);
          definitionCache.Remove(this.m_existingDefinition.Uri);
        }
        else
        {
          if (this.m_existingDefinition != null)
          {
            if (this.m_existingDefinition.WorkspaceTemplate.LastModifiedDate == this.m_newDefinition.WorkspaceTemplate.LastModifiedDate)
            {
              this.CopyProperties(this.m_existingDefinition, this.m_newDefinition);
              return;
            }
            this.RemoveMappings(gatedWorkspaces, this.m_existingDefinition);
          }
          this.AddImplicitCloaks(this.m_newDefinition);
          this.AddMappings(gatedWorkspaces, this.m_newDefinition);
          definitionCache[this.m_newDefinition.Uri] = this.m_newDefinition;
        }
      }

      private void CopyProperties(BuildDefinition copyTo, BuildDefinition copyFrom)
      {
        copyTo.BuildControllerUri = copyFrom.BuildControllerUri;
        copyTo.ContinuousIntegrationQuietPeriod = copyFrom.ContinuousIntegrationQuietPeriod;
        copyTo.TriggerType = copyFrom.TriggerType;
        copyTo.DefaultDropLocation = copyFrom.DefaultDropLocation;
        copyTo.Description = copyFrom.Description;
        copyTo.QueueStatus = copyFrom.QueueStatus;
        copyTo.FullPath = copyFrom.FullPath;
        copyTo.LastBuildUri = copyFrom.LastBuildUri;
        copyTo.LastGoodBuildLabel = copyFrom.LastGoodBuildLabel;
        copyTo.LastGoodBuildUri = copyFrom.LastGoodBuildUri;
        copyTo.Process = copyFrom.Process;
        copyTo.ProcessParameters = copyFrom.ProcessParameters;
        copyTo.ProcessTemplateId = copyFrom.ProcessTemplateId;
        copyTo.RetentionPolicies.Clear();
        copyTo.RetentionPolicies.AddRange((IEnumerable<RetentionPolicy>) copyFrom.RetentionPolicies);
        copyTo.Schedules.Clear();
        copyTo.Schedules.AddRange((IEnumerable<Schedule>) copyFrom.Schedules);
      }

      private void AddImplicitCloaks(BuildDefinition definition)
      {
        List<WorkspaceMapping> workspaceMappingList = new List<WorkspaceMapping>();
        Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) VersionControlPath.StringComparer);
        foreach (WorkspaceMapping mapping in definition.WorkspaceTemplate.Mappings)
        {
          dictionary.Add(mapping.ServerItem, (object) null);
          if (mapping.MappingType == WorkspaceMappingType.Map)
            workspaceMappingList.Add(mapping);
        }
        if (workspaceMappingList.Count <= 1)
          return;
        workspaceMappingList.Sort((Comparison<WorkspaceMapping>) ((x, y) => FileSpec.CompareTopDown(x.LocalItem, y.LocalItem, '\\', StringComparison.OrdinalIgnoreCase)));
        for (int index1 = workspaceMappingList.Count - 1; index1 > 0; --index1)
        {
          WorkspaceMapping workspaceMapping1 = (WorkspaceMapping) null;
          WorkspaceMapping workspaceMapping2 = workspaceMappingList[index1];
          for (int index2 = index1 - 1; index2 >= 0; --index2)
          {
            if (FileSpec.IsSubItem(workspaceMapping2.LocalItem, workspaceMappingList[index2].LocalItem))
            {
              workspaceMapping1 = workspaceMappingList[index2];
              break;
            }
          }
          if (workspaceMapping1 != null)
          {
            string fullPath = VersionControlPath.GetFullPath(workspaceMapping1.ServerItem + workspaceMapping2.LocalItem.Remove(0, workspaceMapping1.LocalItem.Length));
            if (!dictionary.ContainsKey(fullPath))
            {
              dictionary.Add(fullPath, (object) null);
              definition.WorkspaceTemplate.Mappings.Add(new WorkspaceMapping()
              {
                ServerItem = fullPath,
                LocalItem = (string) null,
                MappingType = WorkspaceMappingType.Cloak,
                Depth = 120
              });
            }
          }
        }
      }

      private void AddMappings(
        SparseTree<List<BuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        BuildDefinition definition)
      {
        foreach (WorkspaceMapping mapping in definition.WorkspaceTemplate.Mappings)
        {
          WorkspaceMapping workspaceMapping = mapping;
          List<BuildDefinitionCache.GroupedWorkspaceMapping> referencedObject;
          if (gatedWorkspaces.TryGetValue(workspaceMapping.ServerItem, out referencedObject))
          {
            BuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping1 = referencedObject.Find((Predicate<BuildDefinitionCache.GroupedWorkspaceMapping>) (x => x.Type == workspaceMapping.MappingType && x.Depth == workspaceMapping.Depth));
            if (workspaceMapping1 != null)
              workspaceMapping1.Definitions.Add(definition);
            else
              referencedObject.Add(new BuildDefinitionCache.GroupedWorkspaceMapping(workspaceMapping, definition));
          }
          else
          {
            referencedObject = new List<BuildDefinitionCache.GroupedWorkspaceMapping>(1);
            referencedObject.Add(new BuildDefinitionCache.GroupedWorkspaceMapping(workspaceMapping, definition));
            gatedWorkspaces.Add(workspaceMapping.ServerItem, referencedObject);
          }
        }
      }

      private void RemoveMappings(
        SparseTree<List<BuildDefinitionCache.GroupedWorkspaceMapping>> gatedWorkspaces,
        BuildDefinition definition)
      {
        foreach (WorkspaceMapping mapping in definition.WorkspaceTemplate.Mappings)
        {
          WorkspaceMapping workspaceMapping = mapping;
          List<BuildDefinitionCache.GroupedWorkspaceMapping> referencedObject;
          if (gatedWorkspaces.TryGetValue(workspaceMapping.ServerItem, out referencedObject))
          {
            BuildDefinitionCache.GroupedWorkspaceMapping workspaceMapping1 = referencedObject.Find((Predicate<BuildDefinitionCache.GroupedWorkspaceMapping>) (x => x.Type == workspaceMapping.MappingType && x.Depth == workspaceMapping.Depth));
            if (workspaceMapping1 != null)
            {
              int index = workspaceMapping1.Definitions.FindIndex((Predicate<BuildDefinition>) (x => StringComparer.OrdinalIgnoreCase.Equals(x.Uri, definition.Uri)));
              if (index >= 0)
              {
                workspaceMapping1.Definitions.RemoveAt(index);
                if (workspaceMapping1.Definitions.Count == 0 && referencedObject.Remove(workspaceMapping1) && referencedObject.Count == 0)
                  gatedWorkspaces.Remove(workspaceMapping1.Item, false);
              }
            }
          }
        }
      }

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDefinitionUpdate ExistingDefinition={0} NewDefinition={1}]", this.m_existingDefinition == null ? (object) (string) null : (object) this.m_existingDefinition.Uri, this.m_newDefinition == null ? (object) (string) null : (object) this.m_newDefinition.Uri);
    }

    private sealed class GroupedWorkspaceMapping
    {
      public WorkspaceMapping m_mapping;
      public List<BuildDefinition> m_definitions;

      public GroupedWorkspaceMapping(WorkspaceMapping mapping, BuildDefinition definition)
      {
        this.m_mapping = mapping;
        this.m_definitions = new List<BuildDefinition>();
        this.m_definitions.Add(definition);
      }

      public string Item => this.m_mapping.ServerItem;

      public WorkspaceMappingType Type => this.m_mapping.MappingType;

      public int Depth => this.m_mapping.Depth;

      public List<BuildDefinition> Definitions => this.m_definitions;
    }
  }
}
