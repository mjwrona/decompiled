// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Wiql.WiqlQuery
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Azure.Boards.Charts.Wiql
{
  public class WiqlQuery
  {
    public WiqlQuery() => this.Select = new Collection<string>();

    public Collection<string> Select { get; set; }

    public Condition Where { get; set; }

    public DateTime? AsOf { get; set; }

    public override string ToString()
    {
      StringBuilder queryText = new StringBuilder();
      queryText.Append("SELECT ");
      this.VisitFields(queryText);
      queryText.Append(" FROM WorkItems");
      if (this.Where != null)
      {
        queryText.Append(" WHERE ");
        this.VisitCondition(queryText, this.Where);
      }
      if (this.AsOf.HasValue)
      {
        queryText.Append(" ASOF '");
        queryText.Append(this.AsOf.Value.ToUniversalTime().ToString("O"));
        queryText.Append('\'');
      }
      return queryText.ToString();
    }

    private void VisitFields(StringBuilder queryText)
    {
      for (int index = 0; index < this.Select.Count; ++index)
      {
        queryText.Append('[');
        queryText.Append(this.Select[index]);
        queryText.Append(']');
        if (index < this.Select.Count - 1)
          queryText.Append(',');
      }
    }

    private void VisitCondition(StringBuilder queryText, Condition condition)
    {
      switch (condition)
      {
        case GroupCondition _:
          this.VisitGroupCondition(queryText, condition as GroupCondition);
          break;
        case SingleValueCondition _:
          this.VisitSingleValueCondition(queryText, condition as SingleValueCondition);
          break;
        case MultiValueCondition _:
          this.VisitMultiValueCondition(queryText, condition as MultiValueCondition);
          break;
      }
    }

    private void VisitGroupCondition(StringBuilder queryText, GroupCondition groupCondition)
    {
      queryText.Append('(');
      for (int index = 0; index < groupCondition.Conditions.Count; ++index)
      {
        this.VisitCondition(queryText, groupCondition.Conditions[index]);
        if (index < groupCondition.Conditions.Count - 1)
        {
          switch (groupCondition.Operator)
          {
            case GroupOperator.And:
              queryText.Append(" AND ");
              continue;
            case GroupOperator.Or:
              queryText.Append(" OR ");
              continue;
            default:
              continue;
          }
        }
      }
      queryText.Append(')');
    }

    private void VisitSingleValueCondition(
      StringBuilder queryText,
      SingleValueCondition singleValueCondition)
    {
      queryText.Append('[');
      queryText.Append(singleValueCondition.Field);
      queryText.Append(']');
      switch (singleValueCondition.Operator)
      {
        case SingleValueOperator.Equals:
          queryText.Append(" = ");
          break;
        case SingleValueOperator.NotEquals:
          queryText.Append(" <> ");
          break;
        case SingleValueOperator.GreaterThan:
          queryText.Append(" > ");
          break;
        case SingleValueOperator.GreaterThanOrEqual:
          queryText.Append(" >= ");
          break;
        case SingleValueOperator.LessThan:
          queryText.Append(" < ");
          break;
        case SingleValueOperator.LessThanOrEqual:
          queryText.Append(" <= ");
          break;
        case SingleValueOperator.Under:
          queryText.Append(" UNDER ");
          break;
        case SingleValueOperator.Ever:
          queryText.Append(" EVER ");
          break;
      }
      if (singleValueCondition.Value is string || singleValueCondition.Value is DateTime)
        queryText.AppendFormat("'{0}'", (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(singleValueCondition.Value.ToString()));
      else
        queryText.Append(singleValueCondition.Value);
    }

    private void VisitMultiValueCondition(
      StringBuilder queryText,
      MultiValueCondition multiValueCondition)
    {
      queryText.Append('[');
      queryText.Append(multiValueCondition.Field);
      queryText.Append(']');
      switch (multiValueCondition.Operator)
      {
        case MultiValueOperator.In:
          queryText.Append(" IN ");
          break;
        case MultiValueOperator.NotIn:
          queryText.Append(" NOT IN ");
          break;
      }
      queryText.Append('(');
      for (int index = 0; index < multiValueCondition.Values.Count; ++index)
      {
        if (multiValueCondition.Values[index] is string || multiValueCondition.Values[index] is DateTime)
          queryText.AppendFormat("'{0}'", (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(multiValueCondition.Values[index].ToString()));
        else
          queryText.Append(multiValueCondition.Values[index]);
        if (index < multiValueCondition.Values.Count - 1)
          queryText.Append(',');
      }
      queryText.Append(')');
    }
  }
}
