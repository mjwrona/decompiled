// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TaskBoard.TaskboardColumnService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.Taskboard;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.TaskBoard
{
  public class TaskboardColumnService : ITaskboardColumnService, IVssFrameworkService
  {
    private static readonly char[] IllegalColumnNameChars = new char[23]
    {
      '\'',
      ',',
      ';',
      '~',
      ':',
      '\\',
      '*',
      '|',
      '?',
      '"',
      '&',
      '%',
      '$',
      '!',
      '+',
      '=',
      '[',
      ']',
      '{',
      '}',
      '<',
      '>',
      '์'
    };
    private const int MaxNameLength = 128;
    private const int MaxColumns = 50;
    private const int MinColumns = 2;
    private const string c_TaskboardColumnArea = "TaskboardColumn";
    private const string c_TaskboardTraceLayer = "Service";

    public TaskboardColumns GetColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      using (context.TraceBlock(290901, 290902, "TaskboardColumn", "Service", nameof (GetColumns)))
      {
        IReadOnlyCollection<TaskboardColumn> taskboardColumns;
        using (TaskboardComponent component = context.CreateComponent<TaskboardComponent>())
          taskboardColumns = component.GetColumns(project.Id, team.Id);
        bool isCustomized = taskboardColumns.Any<TaskboardColumn>();
        bool isValidMapping = true;
        Exception exception = (Exception) null;
        if (isCustomized)
        {
          HashSet<string> additionalWits;
          isValidMapping = this.IsValidMapping(context, project, team, (IReadOnlyCollection<ITaskboardColumn>) taskboardColumns, true, out additionalWits, out exception);
          taskboardColumns = this.FilterAdditionalWits(taskboardColumns, additionalWits);
        }
        return new TaskboardColumns(team, (IEnumerable<TaskboardColumn>) taskboardColumns, isCustomized, isValidMapping, exception);
      }
    }

    public TaskboardColumns UpdateColumns(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      IReadOnlyCollection<UpdateTaskboardColumn> updateColumns)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<UpdateTaskboardColumn>>(updateColumns, nameof (updateColumns));
      using (context.TraceBlock(290903, 290904, "TaskboardColumn", "Service", nameof (UpdateColumns)))
      {
        if (updateColumns.Count < 2)
          throw new TaskboardColumnMinColumnLimitException(2, updateColumns.Count);
        this.CheckMaxColumnLimit(updateColumns);
        this.CheckAdminPermission(context, team);
        TaskboardColumns columns1 = this.GetColumns(context, project, team);
        HashSet<string> columnNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        HashSet<Guid> columnIds = new HashSet<Guid>();
        foreach (UpdateTaskboardColumn updateColumn in (IEnumerable<UpdateTaskboardColumn>) updateColumns)
        {
          this.CheckName(updateColumn.Name);
          this.CheckUniqueName(updateColumn.Name, columnNames);
          this.CheckExistingColumnId(updateColumn, columns1);
          this.CheckMappingNotEmpty(updateColumn);
          this.CheckUniqueId(updateColumn.Id, columnIds);
          columnNames.Add(updateColumn.Name);
          Guid? id = updateColumn.Id;
          if (id.HasValue)
          {
            HashSet<Guid> guidSet = columnIds;
            id = updateColumn.Id;
            Guid guid = id.Value;
            guidSet.Add(guid);
          }
        }
        Exception exception;
        if (!this.IsValidMapping(context, project, team, (IReadOnlyCollection<ITaskboardColumn>) updateColumns, false, out HashSet<string> _, out exception))
          throw exception;
        this.FixColumnOrder(updateColumns);
        DateTime readTime = columns1.IsCustomized ? columns1.Columns.Max<TaskboardColumn, DateTime>((Func<TaskboardColumn, DateTime>) (c => c.ChangeDate)) : DateTime.MinValue;
        IReadOnlyCollection<TaskboardColumn> columns2;
        using (TaskboardComponent component = context.CreateComponent<TaskboardComponent>())
        {
          component.UpdateColumns(project.Id, team.Id, context.GetUserId(), readTime, updateColumns);
          columns2 = component.GetColumns(project.Id, team.Id);
        }
        this.RecordCustomColumnTelemetry(context, updateColumns.Count);
        TaskboardColumnOptionsChangedEvent notificationEvent = new TaskboardColumnOptionsChangedEvent()
        {
          ProjectId = project.Id,
          TeamId = team.Id
        };
        context.GetService<ITeamFoundationEventService>().PublishNotification(context, (object) notificationEvent);
        return new TaskboardColumns(team, (IEnumerable<TaskboardColumn>) columns2, columns2.Any<TaskboardColumn>(), true, (Exception) null);
      }
    }

    private IReadOnlyCollection<TaskboardColumn> FilterAdditionalWits(
      IReadOnlyCollection<TaskboardColumn> dbResult,
      HashSet<string> additionalWits)
    {
      if (additionalWits == null || !additionalWits.Any<string>())
        return dbResult;
      List<TaskboardColumn> taskboardColumnList = new List<TaskboardColumn>();
      foreach (TaskboardColumn taskboardColumn in (IEnumerable<TaskboardColumn>) dbResult)
      {
        List<TaskboardColumnMapping> list = taskboardColumn.Mappings.Where<ITaskboardColumnMapping>((Func<ITaskboardColumnMapping, bool>) (m => !additionalWits.Contains(m.WorkItemType))).Select<ITaskboardColumnMapping, TaskboardColumnMapping>((Func<ITaskboardColumnMapping, TaskboardColumnMapping>) (m => m as TaskboardColumnMapping)).ToList<TaskboardColumnMapping>();
        taskboardColumnList.Add(new TaskboardColumn(taskboardColumn.Id, taskboardColumn.Name, taskboardColumn.Order, taskboardColumn.ChangeDate, (IReadOnlyCollection<TaskboardColumnMapping>) list));
      }
      return (IReadOnlyCollection<TaskboardColumn>) taskboardColumnList;
    }

    private void FixColumnOrder(
      IReadOnlyCollection<UpdateTaskboardColumn> updateColumns)
    {
      List<UpdateTaskboardColumn> list = updateColumns.OrderBy<UpdateTaskboardColumn, int>((Func<UpdateTaskboardColumn, int>) (c => c.Order)).ToList<UpdateTaskboardColumn>();
      int num = 0;
      foreach (UpdateTaskboardColumn updateTaskboardColumn in list)
      {
        updateTaskboardColumn.Order = num;
        ++num;
      }
    }

    private bool IsValidMapping(
      IVssRequestContext context,
      ProjectInfo project,
      WebApiTeam team,
      IReadOnlyCollection<ITaskboardColumn> columns,
      bool skipAdditionalWits,
      out HashSet<string> additionalWits,
      out Exception exception)
    {
      exception = (Exception) null;
      additionalWits = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      Dictionary<string, HashSet<string>> dictionary = this.Convert(columns);
      using (context.TraceBlock(290905, 290906, "TaskboardColumn", "Service", nameof (IsValidMapping)))
      {
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>> itemTypeStateMap = new AgileSettings(context, CommonStructureProjectInfo.ConvertProjectInfo(project), team).BacklogConfiguration.GetTaskWorkItemTypeStateMap();
        foreach (KeyValuePair<string, IReadOnlyDictionary<string, WorkItemStateCategory>> keyValuePair in (IEnumerable<KeyValuePair<string, IReadOnlyDictionary<string, WorkItemStateCategory>>>) itemTypeStateMap)
        {
          string key = keyValuePair.Key;
          List<string> list = keyValuePair.Value.Select<KeyValuePair<string, WorkItemStateCategory>, string>((Func<KeyValuePair<string, WorkItemStateCategory>, string>) (s => s.Key)).ToList<string>();
          HashSet<string> stringSet;
          if (!dictionary.TryGetValue(key, out stringSet))
          {
            exception = (Exception) new TaskboardColumnInvalidMappingException(string.Format(AgileResources.TaskboardColumnWorkItemTypeMappingMissing, (object) key));
            return false;
          }
          foreach (string str in list)
          {
            if (!stringSet.Contains(str))
            {
              exception = (Exception) new TaskboardColumnInvalidMappingException(string.Format(AgileResources.TaskboardColumnWorkItemTypeStateMappingMissing, (object) key, (object) str));
              return false;
            }
          }
        }
        foreach (KeyValuePair<string, HashSet<string>> keyValuePair in dictionary)
        {
          string key1 = keyValuePair.Key;
          IReadOnlyDictionary<string, WorkItemStateCategory> readOnlyDictionary;
          if (!itemTypeStateMap.TryGetValue(keyValuePair.Key, out readOnlyDictionary))
          {
            if (skipAdditionalWits)
            {
              additionalWits.Add(key1);
            }
            else
            {
              exception = (Exception) new TaskboardColumnInvalidMappingException(string.Format(AgileResources.TaskboardColumnWorkItemTypeInvalid, (object) key1));
              return false;
            }
          }
          else
          {
            foreach (string key2 in keyValuePair.Value)
            {
              if (!readOnlyDictionary.ContainsKey(key2))
              {
                exception = (Exception) new TaskboardColumnInvalidMappingException(string.Format(AgileResources.TaskboardColumnWorkItemTypeStateInvalid, (object) key2, (object) key1));
                return false;
              }
            }
          }
        }
        ITaskboardColumn taskboardColumn1 = columns.First<ITaskboardColumn>();
        ITaskboardColumn taskboardColumn2 = columns.Last<ITaskboardColumn>();
        foreach (ITaskboardColumnMapping mapping in (IEnumerable<ITaskboardColumnMapping>) taskboardColumn1.Mappings)
        {
          IReadOnlyDictionary<string, WorkItemStateCategory> source;
          if ((!itemTypeStateMap.TryGetValue(mapping.WorkItemType, out source) || !TFStringComparer.WorkItemStateName.Equals(source.First<KeyValuePair<string, WorkItemStateCategory>>().Key, mapping.State)) && (!skipAdditionalWits || !additionalWits.Contains(mapping.WorkItemType)))
          {
            exception = (Exception) new TaskboardColumnInvalidMappingException(string.Format(AgileResources.TaskboardColumnFirstColumnToStateInvalid, (object) taskboardColumn1.Name, (object) mapping.WorkItemType, source != null ? (object) source.First<KeyValuePair<string, WorkItemStateCategory>>().Key : (object) (string) null));
            return false;
          }
        }
        foreach (ITaskboardColumnMapping mapping in (IEnumerable<ITaskboardColumnMapping>) taskboardColumn2.Mappings)
        {
          IReadOnlyDictionary<string, WorkItemStateCategory> source;
          KeyValuePair<string, WorkItemStateCategory> keyValuePair;
          if (itemTypeStateMap.TryGetValue(mapping.WorkItemType, out source))
          {
            VssStringComparer workItemStateName = TFStringComparer.WorkItemStateName;
            keyValuePair = source.First<KeyValuePair<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, bool>) (s => s.Value == WorkItemStateCategory.Completed));
            string key = keyValuePair.Key;
            string state = mapping.State;
            if (workItemStateName.Equals(key, state))
              continue;
          }
          if (!skipAdditionalWits || !additionalWits.Contains(mapping.WorkItemType))
          {
            ref Exception local = ref exception;
            string columnToStateInvalid = AgileResources.TaskboardColumnLastColumnToStateInvalid;
            string name = taskboardColumn2.Name;
            string workItemType = mapping.WorkItemType;
            keyValuePair = source.First<KeyValuePair<string, WorkItemStateCategory>>((Func<KeyValuePair<string, WorkItemStateCategory>, bool>) (s => s.Value == WorkItemStateCategory.Completed));
            string key = keyValuePair.Key;
            TaskboardColumnInvalidMappingException mappingException = new TaskboardColumnInvalidMappingException(string.Format(columnToStateInvalid, (object) name, (object) workItemType, (object) key));
            local = (Exception) mappingException;
            return false;
          }
        }
        return true;
      }
    }

    private Dictionary<string, HashSet<string>> Convert(
      IReadOnlyCollection<ITaskboardColumn> columns)
    {
      Dictionary<string, HashSet<string>> dictionary = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (ITaskboardColumn column in (IEnumerable<ITaskboardColumn>) columns)
      {
        foreach (ITaskboardColumnMapping mapping in (IEnumerable<ITaskboardColumnMapping>) column.Mappings)
        {
          HashSet<string> stringSet;
          if (!dictionary.TryGetValue(mapping.WorkItemType, out stringSet))
          {
            stringSet = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            dictionary[mapping.WorkItemType] = stringSet;
          }
          stringSet.Add(mapping.State);
        }
      }
      return dictionary;
    }

    private void CheckAdminPermission(IVssRequestContext context, WebApiTeam team)
    {
      if (!context.GetService<ITeamService>().UserIsTeamAdmin(context, team.Identity))
        throw new TaskboardColumnUpdateUserIsNotTeamAdminException();
    }

    private void CheckUniqueId(Guid? id, HashSet<Guid> columnIds)
    {
      if (id.HasValue && columnIds.Contains(id.Value))
        throw new TaskboardColumnDuplicateColumnIdException(id.Value);
    }

    private void CheckMaxColumnLimit(
      IReadOnlyCollection<UpdateTaskboardColumn> updateColumns)
    {
      if (updateColumns.Count > 50)
        throw new TaskboardColumnMaxLimitExceededException(updateColumns.Count, 50);
    }

    private void CheckName(string name)
    {
      if (string.IsNullOrWhiteSpace(name) || name.Length > 128 || name.IndexOfAny(TaskboardColumnService.IllegalColumnNameChars) != -1)
        throw new TaskboardColumnNameInvalidException(name, 128, string.Join<char>("", (IEnumerable<char>) TaskboardColumnService.IllegalColumnNameChars));
    }

    private void CheckExistingColumnId(UpdateTaskboardColumn col, TaskboardColumns existingColumns)
    {
      if (col.Id.HasValue && !existingColumns.Columns.Any<TaskboardColumn>((Func<TaskboardColumn, bool>) (c =>
      {
        Guid id1 = c.Id;
        Guid? id2 = col.Id;
        return id2.HasValue && id1 == id2.GetValueOrDefault();
      })))
        throw new TaskboardColumnIdInvalidException(col.Id.Value);
    }

    private void CheckMappingNotEmpty(UpdateTaskboardColumn col)
    {
      if (col.Mappings == null || !col.Mappings.Any<ITaskboardColumnMapping>())
        throw new TaskboardColumnMappingEmptyException(col.Name);
    }

    private void CheckUniqueName(string colName, HashSet<string> columnNames)
    {
      if (columnNames.Contains(colName))
        throw new TaskboardColumnNameDuplicateException(colName);
    }

    private void RecordCustomColumnTelemetry(IVssRequestContext context, int columnCount)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      CustomerIntelligenceService service = context.GetService<CustomerIntelligenceService>();
      intelligenceData.Add("columns", (double) columnCount);
      IVssRequestContext requestContext = context;
      string customColumnSave = AgileCustomerIntelligenceFeature.TaskBoardCustomColumnSave;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext, "Agile", customColumnSave, properties);
    }

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
