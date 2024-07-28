// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDefinitionCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemStateDefinitionCacheService : 
    VssMemoryCacheService<Guid, IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private const int c_maxCacheSize = 200;

    public WorkItemStateDefinitionCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>>().WithMaxElements(200).WithCleanupInterval(WorkItemStateDefinitionCacheService.s_cacheCleanupInterval))
    {
      this.InactivityInterval.Value = WorkItemStateDefinitionCacheService.s_maxCacheInactivityAge;
    }

    public virtual IReadOnlyCollection<WorkItemStateDefinition> GetWorkItemStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      ProcessWorkItemType wit)
    {
      IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> dictionary;
      if (!this.TryGetValue(requestContext, processId, out dictionary) || !dictionary.ContainsKey(wit.Id))
        dictionary = this.RefreshProcess(requestContext, processId);
      IReadOnlyCollection<WorkItemStateDefinitionRecord> source;
      if (!dictionary.TryGetValue(wit.Id, out source))
        return (IReadOnlyCollection<WorkItemStateDefinition>) new List<WorkItemStateDefinition>();
      ReadOnlyDictionary<string, WorkItemOobState>.ValueCollection oobStates = requestContext.GetService<WorkItemTrackingOutOfBoxStatesCache>().GetOutOfBoxStates(requestContext)?.Values;
      return (IReadOnlyCollection<WorkItemStateDefinition>) source.Select<WorkItemStateDefinitionRecord, WorkItemStateDefinition>((Func<WorkItemStateDefinitionRecord, WorkItemStateDefinition>) (sr => WorkItemStateDefinition.Create(sr, wit.ReferenceName, (IEnumerable<WorkItemOobState>) oobStates))).ToList<WorkItemStateDefinition>();
    }

    public virtual IReadOnlyCollection<WorkItemStateDefinitionRecord> GetWorkItemStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId)
    {
      IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> dictionary;
      if (!this.TryGetValue(requestContext, processId, out dictionary))
        dictionary = this.RefreshProcess(requestContext, processId);
      List<WorkItemStateDefinitionRecord> stateDefinitions = new List<WorkItemStateDefinitionRecord>();
      foreach (KeyValuePair<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> keyValuePair in (IEnumerable<KeyValuePair<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>>) dictionary)
        stateDefinitions.AddRange((IEnumerable<WorkItemStateDefinitionRecord>) keyValuePair.Value);
      return (IReadOnlyCollection<WorkItemStateDefinitionRecord>) stateDefinitions;
    }

    public void OnInvalidateProcess(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        Guid key = new Guid(eventData);
        this.Remove(requestContext, key);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910508, "WorkItemStateDefinition", "WorkItemStateDefinitionCacheService.InvalidateProcess", ex);
      }
    }

    private IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> RefreshProcess(
      IVssRequestContext requestContext,
      Guid processId)
    {
      IReadOnlyCollection<WorkItemStateDefinitionRecord> stateDefinitions;
      using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
        stateDefinitions = component.GetProcessStateDefinitions(processId);
      requestContext.GetService<WorkItemStateDefinitionCacheService>();
      ConcurrentDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> concurrentDictionary = new ConcurrentDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>((IEnumerable<KeyValuePair<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>>) stateDefinitions.GroupBy<WorkItemStateDefinitionRecord, Guid>((Func<WorkItemStateDefinitionRecord, Guid>) (s => s.WorkItemTypeId)).ToDictionary<IGrouping<Guid, WorkItemStateDefinitionRecord>, Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>((Func<IGrouping<Guid, WorkItemStateDefinitionRecord>, Guid>) (g => g.Key), (Func<IGrouping<Guid, WorkItemStateDefinitionRecord>, IReadOnlyCollection<WorkItemStateDefinitionRecord>>) (g => (IReadOnlyCollection<WorkItemStateDefinitionRecord>) g.ToList<WorkItemStateDefinitionRecord>())));
      this.Set(requestContext, processId, (IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>) concurrentDictionary);
      return (IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>) concurrentDictionary;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(910509, "WorkItemStateDefinition", "WorkItemStateDefinitionCacheService.ServiceStart", nameof (ServiceStart));
      try
      {
        base.ServiceStart(requestContext);
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        service.RegisterNotification(requestContext, "Default", DBNotificationIds.ProcessWorkItemMetadataDeleted, new SqlNotificationCallback(this.OnInvalidateProcess), true);
        service.RegisterNotification(requestContext, "Default", DBNotificationIds.WorkItemStateDefinitionModified, new SqlNotificationCallback(this.OnInvalidateProcess), true);
      }
      finally
      {
        requestContext.TraceLeave(910510, "WorkItemStateDefinition", "WorkItemStateDefinitionCacheService.ServiceStart", nameof (ServiceStart));
      }
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(910511, "WorkItemStateDefinition", "WorkItemStateDefinitionCacheService.ServiceEnd", nameof (ServiceEnd));
      try
      {
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        service.UnregisterNotification(requestContext, "Default", DBNotificationIds.ProcessWorkItemMetadataDeleted, new SqlNotificationCallback(this.OnInvalidateProcess), true);
        service.UnregisterNotification(requestContext, "Default", DBNotificationIds.WorkItemStateDefinitionModified, new SqlNotificationCallback(this.OnInvalidateProcess), true);
        base.ServiceEnd(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(910512, "WorkItemStateDefinition", "WorkItemStateDefinitionCacheService.ServiceEnd", nameof (ServiceEnd));
      }
    }
  }
}
