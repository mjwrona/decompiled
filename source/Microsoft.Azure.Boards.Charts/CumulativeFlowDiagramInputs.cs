// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.CumulativeFlowDiagramInputs
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Cache;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.Charts
{
  public class CumulativeFlowDiagramInputs : ChartInputs
  {
    private List<string> m_workItemTypes;
    private List<string> m_workItemStates;
    private IDictionary<Guid, Tuple<string, BoardColumnType, bool>> m_boardColumnMappings;
    private DateTime m_startDate = DateTime.MinValue;
    private DateTime m_endDate = DateTime.MaxValue;
    private TimeZoneInfo m_timeZoneInfo;
    private string m_doneStateName;
    private string m_newStateName;
    private IDictionary<string, BoardColumnType> m_stateColumnTypeMap;

    public void RemoveState(string state)
    {
      this.StateColumnTypeMap.Remove(state);
      this.WorkItemStates.Remove(state);
      foreach (Guid key in this.BoardColumnMappings.Where<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>>((Func<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>, bool>) (item => TFStringComparer.BoardColumnName.Equals(state, item.Value.Item1))).Select<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>, Guid>((Func<KeyValuePair<Guid, Tuple<string, BoardColumnType, bool>>, Guid>) (item => item.Key)).ToArray<Guid>())
        this.BoardColumnMappings.Remove(key);
    }

    public IDictionary<string, BoardColumnType> StateColumnTypeMap
    {
      get
      {
        if (this.m_stateColumnTypeMap == null)
          this.m_stateColumnTypeMap = (IDictionary<string, BoardColumnType>) new Dictionary<string, BoardColumnType>();
        return this.m_stateColumnTypeMap;
      }
      set => this.m_stateColumnTypeMap = value;
    }

    public List<string> WorkItemTypes
    {
      get
      {
        if (this.m_workItemTypes == null)
          this.m_workItemTypes = new List<string>();
        return this.m_workItemTypes;
      }
      set => this.m_workItemTypes = value;
    }

    public List<string> WorkItemStates
    {
      get
      {
        if (this.m_workItemStates == null)
          this.m_workItemStates = new List<string>();
        return this.m_workItemStates;
      }
      set => this.m_workItemStates = value;
    }

    public TimeZoneInfo TimeZone
    {
      get => this.m_timeZoneInfo == null ? TimeZoneInfo.Local : this.m_timeZoneInfo;
      set => this.m_timeZoneInfo = value;
    }

    public IdentityChartCache IdentityChartCache { get; set; }

    public DateTime StartDate
    {
      get => this.m_startDate == DateTime.MinValue ? DateTime.Today : this.m_startDate;
      set => this.m_startDate = value;
    }

    public DateTime EndDate
    {
      get => this.m_endDate == DateTime.MaxValue ? DateTime.Today : this.m_endDate;
      set => this.m_endDate = value;
    }

    public bool IsCustomStartDate { get; set; }

    public string DoneStateName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_doneStateName))
          this.m_doneStateName = this.WorkItemStates[this.WorkItemStates.Count - 1];
        return this.m_doneStateName;
      }
      set => this.m_doneStateName = value;
    }

    public string NewStateName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_newStateName))
          this.m_newStateName = this.WorkItemStates[0];
        return this.m_newStateName;
      }
      set => this.m_newStateName = value;
    }

    public DayOfWeek FirstWorkDay { get; set; }

    public bool UseKanbanColumns => this.BoardColumnRevisions != null && !string.IsNullOrEmpty(this.BoardColumnExtensionFieldName);

    public IEnumerable<BoardColumnRevision> BoardColumnRevisions { get; set; }

    public string BoardColumnExtensionFieldName { get; set; }

    public IDictionary<Guid, Tuple<string, BoardColumnType, bool>> BoardColumnMappings
    {
      get
      {
        if (this.m_boardColumnMappings == null)
          this.m_boardColumnMappings = (IDictionary<Guid, Tuple<string, BoardColumnType, bool>>) new Dictionary<Guid, Tuple<string, BoardColumnType, bool>>();
        return this.m_boardColumnMappings;
      }
      set => this.m_boardColumnMappings = value;
    }

    public bool UseActualEndDateTime { get; set; }
  }
}
