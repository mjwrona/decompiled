// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryFilterModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class QueryFilterModel : FilterModel
  {
    internal QueryFilterModel(
      QueryAdapter adapter,
      string prefix,
      string filter,
      string commonTeamProject)
    {
      this.Initialize(adapter, filter, prefix, commonTeamProject);
    }

    internal void Initialize(
      QueryAdapter adapter,
      string filter,
      string prefix,
      string commonTeamProject)
    {
      string parsedWiql;
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node whereNode;
      if (!WiqlHelper.ParseFilter(filter, prefix, out parsedWiql, out whereNode))
        return;
      List<FilterGroup> groups1 = new List<FilterGroup>();
      List<FilterClause> clauses = WiqlHelper.ProcessFilterNode(whereNode, parsedWiql, adapter, groups1);
      if (!string.IsNullOrEmpty(commonTeamProject))
        QueryFilterModel.StripTopLevelTeamProjectClause(adapter, clauses, groups1, commonTeamProject);
      List<FilterGroup> groups2 = QueryFilterModel.CleanupGroups(groups1, (IEnumerable<FilterClause>) clauses);
      this.Clauses = (ICollection<FilterClause>) clauses;
      this.Groups = (ICollection<FilterGroup>) groups2;
      this.MaxGroupLevel = FilterGroup.AssignLevels((IEnumerable<FilterGroup>) groups2);
    }

    internal static void StripTopLevelTeamProjectClause(
      QueryAdapter adapter,
      List<FilterClause> clauses,
      List<FilterGroup> groups,
      string teamProject)
    {
      int count = clauses.Count;
      for (int index = 0; index < count; ++index)
      {
        FilterClause clause = clauses[index];
        if (adapter.IsTeamProjectField(clause.FieldName) && clause.Operator == adapter.GetLocalizedOperator("=") && QueryFilterModel.AreTeamProjectValuesEqual(adapter, teamProject, clause.Value) && !QueryFilterModel.IsGrouped(clause, groups) && QueryFilterModel.RemoveTopLevelClause(clause, clauses, groups))
        {
          --count;
          --index;
        }
      }
    }

    internal static bool RemoveTopLevelClause(
      FilterClause c,
      List<FilterClause> clauses,
      List<FilterGroup> groups)
    {
      if (!clauses.Remove(c))
        return false;
      foreach (FilterClause clause in clauses)
      {
        if (clause.Index >= c.Index)
          clause.Index = Math.Max(1, clause.Index - 1);
      }
      int index = c.Index;
      foreach (FilterGroup group in groups)
      {
        if (group.End >= index)
        {
          if (group.Start > index)
            group.Start = Math.Max(1, group.Start - 1);
          group.End = Math.Max(1, group.End - 1);
        }
      }
      return true;
    }

    internal static bool IsGrouped(FilterClause clause, List<FilterGroup> groups)
    {
      foreach (FilterGroup group in groups)
      {
        if (clause.Index >= group.Start && clause.Index <= group.End)
          return true;
      }
      return false;
    }

    private static List<FilterGroup> CleanupGroups(
      List<FilterGroup> groups,
      IEnumerable<FilterClause> clauses)
    {
      List<FilterGroup> filterGroupList1 = new List<FilterGroup>();
      if (!clauses.Any<FilterClause>())
        return filterGroupList1;
      int num1 = clauses.Min<FilterClause>((Func<FilterClause, int>) (c => c.Index));
      int num2 = clauses.Max<FilterClause>((Func<FilterClause, int>) (c => c.Index));
      foreach (FilterGroup group in groups)
      {
        if (group.Start >= 0 && group.End >= 0 && group.Start != group.End && (group.Start != num1 || group.End != num2))
        {
          List<FilterClause> filterClauseList = new List<FilterClause>();
          for (int i = group.Start; i <= group.End; i++)
          {
            FilterClause filterClause = clauses.FirstOrDefault<FilterClause>((Func<FilterClause, bool>) (c => c.Index == i));
            if (filterClause != null)
              filterClauseList.Add(filterClause);
          }
          if (filterClauseList.Count > 1)
          {
            group.Start = filterClauseList[0].Index;
            group.End = filterClauseList[filterClauseList.Count - 1].Index;
            filterGroupList1.Add(group);
          }
        }
      }
      groups = filterGroupList1;
      List<FilterGroup> filterGroupList2 = new List<FilterGroup>();
      foreach (FilterGroup group in groups)
      {
        bool flag = true;
        foreach (FilterGroup filterGroup in filterGroupList2)
        {
          if (group.Start == filterGroup.Start && group.End == filterGroup.End)
          {
            flag = false;
            break;
          }
        }
        if (flag)
          filterGroupList2.Add(group);
      }
      return filterGroupList2;
    }

    private static bool AreTeamProjectValuesEqual(
      QueryAdapter adapter,
      string currentProject,
      string teamProjectValue)
    {
      if (string.IsNullOrEmpty(currentProject) || string.IsNullOrEmpty(teamProjectValue))
        return false;
      return adapter.Operators.IsProjectMacro(currentProject, false) && adapter.Operators.IsProjectMacro(teamProjectValue, true) || TFStringComparer.TeamProjectName.Equals(currentProject, teamProjectValue);
    }
  }
}
