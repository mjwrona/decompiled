// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FilterConditionUtil
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class FilterConditionUtil
  {
    public static IFilterCondition GetFilterConditionFromFilterModel(ExpressionFilterModel filter)
    {
      if (filter == null || filter.Clauses.Count == 0)
        return (IFilterCondition) new EmptyFilterCondition();
      Dictionary<int, IFilterCondition> clauses = new Dictionary<int, IFilterCondition>();
      foreach (ExpressionFilterClause clause in (IEnumerable<ExpressionFilterClause>) filter.Clauses)
        clauses[clause.Index] = (IFilterCondition) new FilterCondition(clause);
      if (filter.Groups.Count > 0)
        FilterConditionUtil.CollapseGroups(filter, clauses);
      IFilterCondition current = (IFilterCondition) null;
      foreach (KeyValuePair<int, IFilterCondition> keyValuePair in clauses)
        current = FilterConditionUtil.MergeClauses(current, keyValuePair.Value);
      return current;
    }

    private static void CollapseGroups(
      ExpressionFilterModel filter,
      Dictionary<int, IFilterCondition> clauses)
    {
      List<ExpressionFilterGroup> list = filter.Groups.ToList<ExpressionFilterGroup>();
      list.OrderBy<ExpressionFilterGroup, int>((Func<ExpressionFilterGroup, int>) (x => x.Level)).ThenBy<ExpressionFilterGroup, int>((Func<ExpressionFilterGroup, int>) (x => x.Start)).ThenBy<ExpressionFilterGroup, int>((Func<ExpressionFilterGroup, int>) (x => x.End));
      foreach (ExpressionFilterGroup expressionFilterGroup in list)
      {
        IFilterCondition current = (IFilterCondition) null;
        if (!clauses.TryGetValue(expressionFilterGroup.Start - 1, out current))
          throw new InvalidOperationException(string.Format("Failed to parse FilterClause, groups cannot overlap. Error happened while processing group with start: {0} and end: {1}", (object) expressionFilterGroup.Start, (object) expressionFilterGroup.End));
        for (int start = expressionFilterGroup.Start; start < expressionFilterGroup.End; ++start)
        {
          IFilterCondition next = (IFilterCondition) null;
          if (clauses.TryGetValue(start, out next))
          {
            current = FilterConditionUtil.MergeClauses(current, next);
            clauses.Remove(start);
          }
          else if (expressionFilterGroup.Level == 0)
            throw new InvalidOperationException(string.Format("Failed to parse FilterClause, group has start: {0} and end: {1}, but there is no clause with index {2} ", (object) expressionFilterGroup.Start, (object) expressionFilterGroup.End, (object) start));
        }
        clauses[expressionFilterGroup.Start - 1] = current;
      }
    }

    private static IFilterCondition MergeClauses(IFilterCondition current, IFilterCondition next)
    {
      if (current == null)
        return next;
      if (next.LogicalOperator.Equals("and", StringComparison.OrdinalIgnoreCase))
        return (IFilterCondition) new AndFilterCondition(current, next, current.LogicalOperator);
      if (next.LogicalOperator.Equals("or", StringComparison.OrdinalIgnoreCase))
        return (IFilterCondition) new OrFilterCondition(current, next, current.LogicalOperator);
      throw new InvalidOperationException("Operation " + next.LogicalOperator + " is not recognized");
    }
  }
}
