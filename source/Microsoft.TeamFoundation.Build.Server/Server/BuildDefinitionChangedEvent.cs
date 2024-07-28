// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionChangedEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-build.build-definition-changed-event")]
  [Serializable]
  public sealed class BuildDefinitionChangedEvent : ChangedEvent
  {
    private List<PropertyChange<RetentionPolicy>> m_retentionPolicyChanges = new List<PropertyChange<RetentionPolicy>>();
    private List<PropertyChange<Schedule>> m_scheduleChanges = new List<PropertyChange<Schedule>>();
    private List<PropertyChange<WorkspaceMapping>> m_workspaceMappingChanges = new List<PropertyChange<WorkspaceMapping>>();
    private HashSet<string> ScheduleProperties = new HashSet<string>((IEnumerable<string>) new string[3]
    {
      "TimeZoneId",
      "UtcDaysToBuild",
      "UtcStartTime"
    });
    private HashSet<string> RetentionPolicyProperties = new HashSet<string>((IEnumerable<string>) new string[2]
    {
      "DeleteOptions",
      "NumberToKeep"
    });
    private HashSet<string> WorkspaceMappingProperties = new HashSet<string>((IEnumerable<string>) new string[4]
    {
      "ServerItem",
      "LocalItem",
      "MappingType",
      "Depth"
    });
    private HashSet<string> PublishedProperties = new HashSet<string>((IEnumerable<string>) new string[13]
    {
      "BatchSize",
      "BuildControllerUri",
      "ContinuousIntegrationQuietPeriod",
      "DefaultDropLocation",
      "Description",
      "FullPath",
      "LastBuildUri",
      "LastGoodBuildLabel",
      "LastGoodBuildUri",
      "ProcessParameters",
      "ProcessTemplateId",
      "QueueStatus",
      "TriggerType"
    });

    public BuildDefinitionChangedEvent() => this.PropertyChanges = new List<PropertyChange>();

    internal BuildDefinitionChangedEvent(
      IVssRequestContext requestContext,
      BuildDefinition oldDefinition,
      BuildDefinition newDefinition)
      : this()
    {
      this.TeamProject = newDefinition.TeamProject.Name;
      this.TeamProjectId = newDefinition.TeamProject.Id;
      this.DefinitionId = newDefinition.Id;
      EventHelper.FillChangedEventFromDelta<BuildDefinition>(requestContext, (ChangedEvent) this, oldDefinition, newDefinition);
      foreach (PropertyChange propertiesDeltaFromObject in EventHelper.CommonPropertiesDeltaFromObjects<BuildDefinition>(oldDefinition, newDefinition, this.PublishedProperties))
      {
        this.PropertyChanges.Add(propertiesDeltaFromObject);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertiesDeltaFromObject.Name, (object) propertiesDeltaFromObject.OldValue, (object) propertiesDeltaFromObject.NewValue);
      }
      this.AddRetentionPolicies(oldDefinition, newDefinition);
      this.AddSchedules(oldDefinition, newDefinition);
      this.AddWorkspaceMappings(oldDefinition, newDefinition);
    }

    internal BuildDefinitionChangedEvent(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      ChangedType action)
      : this()
    {
      this.TeamProject = definition.TeamProject.Name;
      this.TeamProjectId = definition.TeamProject.Id;
      this.DefinitionId = definition.Id;
      EventHelper.FillAddedOrDeletedEvent<BuildDefinition>(requestContext, action, (ChangedEvent) this, definition, this.PublishedProperties);
      if (action == ChangedType.Added)
      {
        this.AddRetentionPolicies((BuildDefinition) null, definition);
        this.AddSchedules((BuildDefinition) null, definition);
        this.AddWorkspaceMappings((BuildDefinition) null, definition);
      }
      else
      {
        if (action != ChangedType.Deleted)
          return;
        this.AddRetentionPolicies(definition, (BuildDefinition) null);
        this.AddSchedules(definition, (BuildDefinition) null);
        this.AddWorkspaceMappings(definition, (BuildDefinition) null);
      }
    }

    private void AddRetentionPolicies(BuildDefinition oldObj, BuildDefinition newObj)
    {
      if (oldObj == null)
      {
        foreach (RetentionPolicy retentionPolicy in newObj.RetentionPolicies)
          this.RetentionPolicyChanges.Add(new PropertyChange<RetentionPolicy>()
          {
            OldValue = (RetentionPolicy) null,
            NewValue = retentionPolicy
          });
      }
      else if (newObj == null)
      {
        foreach (RetentionPolicy retentionPolicy in oldObj.RetentionPolicies)
          this.RetentionPolicyChanges.Add(new PropertyChange<RetentionPolicy>()
          {
            OldValue = retentionPolicy,
            NewValue = (RetentionPolicy) null
          });
      }
      else
      {
        Dictionary<string, RetentionPolicy> dictionary = newObj.RetentionPolicies.ToDictionary<RetentionPolicy, string, RetentionPolicy>((Func<RetentionPolicy, string>) (x => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) x.BuildReason, (object) x.BuildStatus)), (Func<RetentionPolicy, RetentionPolicy>) (x => x));
        foreach (RetentionPolicy retentionPolicy in oldObj.RetentionPolicies)
        {
          string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) retentionPolicy.BuildReason, (object) retentionPolicy.BuildStatus);
          RetentionPolicy newObj1;
          if (dictionary.TryGetValue(key, out newObj1))
          {
            dictionary.Remove(key);
            if ((retentionPolicy.DeleteOptions != newObj1.DeleteOptions || retentionPolicy.NumberToKeep != newObj1.NumberToKeep) && EventHelper.CommonPropertiesDeltaFromObjects<RetentionPolicy>(retentionPolicy, newObj1, this.RetentionPolicyProperties).Count<PropertyChange>() > 0)
              this.RetentionPolicyChanges.Add(new PropertyChange<RetentionPolicy>()
              {
                OldValue = retentionPolicy,
                NewValue = newObj1
              });
          }
          else
            this.RetentionPolicyChanges.Add(new PropertyChange<RetentionPolicy>()
            {
              OldValue = retentionPolicy,
              NewValue = (RetentionPolicy) null
            });
        }
        foreach (RetentionPolicy retentionPolicy in dictionary.Values)
          this.RetentionPolicyChanges.Add(new PropertyChange<RetentionPolicy>()
          {
            OldValue = (RetentionPolicy) null,
            NewValue = retentionPolicy
          });
      }
    }

    private void AddSchedules(BuildDefinition oldObj, BuildDefinition newObj)
    {
      if (oldObj != null && newObj != null && oldObj.Schedules.Count <= 0 && newObj.Schedules.Count <= 0)
        return;
      this.m_scheduleChanges.Add(new PropertyChange<Schedule>()
      {
        OldValue = oldObj == null || oldObj.Schedules.Count == 0 ? (Schedule) null : oldObj.Schedules[0],
        NewValue = newObj == null || newObj.Schedules.Count == 0 ? (Schedule) null : newObj.Schedules[0]
      });
    }

    private void AddWorkspaceMappings(BuildDefinition oldObj, BuildDefinition newObj)
    {
      if (oldObj == null || oldObj.WorkspaceTemplate == null)
      {
        if (newObj == null || newObj.WorkspaceTemplate == null)
          return;
        foreach (WorkspaceMapping mapping in newObj.WorkspaceTemplate.Mappings)
          this.WorkspaceMappingChanges.Add(new PropertyChange<WorkspaceMapping>()
          {
            OldValue = (WorkspaceMapping) null,
            NewValue = mapping
          });
      }
      else if (newObj == null || newObj.WorkspaceTemplate == null)
      {
        if (oldObj == null || oldObj.WorkspaceTemplate == null)
          return;
        foreach (WorkspaceMapping mapping in oldObj.WorkspaceTemplate.Mappings)
          this.WorkspaceMappingChanges.Add(new PropertyChange<WorkspaceMapping>()
          {
            OldValue = mapping,
            NewValue = (WorkspaceMapping) null
          });
      }
      else
      {
        Dictionary<string, WorkspaceMapping> dictionary = newObj.WorkspaceTemplate.Mappings.ToDictionary<WorkspaceMapping, string, WorkspaceMapping>((Func<WorkspaceMapping, string>) (x => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}", (object) x.ServerItem, (object) x.LocalItem, (object) x.MappingType, (object) x.Depth)), (Func<WorkspaceMapping, WorkspaceMapping>) (x => x));
        foreach (WorkspaceMapping mapping in oldObj.WorkspaceTemplate.Mappings)
        {
          string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}", (object) mapping.ServerItem, (object) mapping.LocalItem, (object) mapping.MappingType, (object) mapping.Depth);
          if (dictionary.TryGetValue(key, out WorkspaceMapping _))
            dictionary.Remove(key);
          else
            this.WorkspaceMappingChanges.Add(new PropertyChange<WorkspaceMapping>()
            {
              OldValue = mapping,
              NewValue = (WorkspaceMapping) null
            });
        }
        foreach (WorkspaceMapping workspaceMapping in dictionary.Values)
          this.WorkspaceMappingChanges.Add(new PropertyChange<WorkspaceMapping>()
          {
            OldValue = (WorkspaceMapping) null,
            NewValue = workspaceMapping
          });
      }
    }

    public List<PropertyChange<RetentionPolicy>> RetentionPolicyChanges => this.m_retentionPolicyChanges;

    public List<PropertyChange<Schedule>> ScheduleChanges => this.m_scheduleChanges;

    public List<PropertyChange<WorkspaceMapping>> WorkspaceMappingChanges => this.m_workspaceMappingChanges;

    public string TeamProject { get; set; }

    public Guid TeamProjectId { get; set; }

    public int DefinitionId { get; set; }
  }
}
