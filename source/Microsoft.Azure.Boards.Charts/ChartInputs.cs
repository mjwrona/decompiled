// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.ChartInputs
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.Azure.Boards.Charts.Wiql;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Boards.Charts
{
  public abstract class ChartInputs
  {
    private List<FieldValue> m_filterFieldValues;
    private string m_filterFieldName;

    public string FilterFieldName
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_filterFieldName))
          this.m_filterFieldName = CoreFieldReferenceNames.AreaPath;
        return this.m_filterFieldName;
      }
      set => this.m_filterFieldName = value;
    }

    public List<FieldValue> FilterFieldValues
    {
      get
      {
        if (this.m_filterFieldValues == null)
          this.m_filterFieldValues = new List<FieldValue>();
        return this.m_filterFieldValues;
      }
      set => this.m_filterFieldValues = value;
    }

    public GroupCondition GetFilterConditions()
    {
      Collection<Condition> collection = new Collection<Condition>();
      foreach (FieldValue filterFieldValue in this.FilterFieldValues)
        collection.Add((Condition) new SingleValueCondition()
        {
          Field = this.FilterFieldName,
          Operator = (filterFieldValue.IncludeHierarchy ? SingleValueOperator.Under : SingleValueOperator.Equals),
          Value = (object) filterFieldValue.Value
        });
      return new GroupCondition()
      {
        Operator = GroupOperator.Or,
        Conditions = collection
      };
    }
  }
}
