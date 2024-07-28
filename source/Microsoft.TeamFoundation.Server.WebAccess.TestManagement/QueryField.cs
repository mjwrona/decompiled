// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryField
  {
    private QueryOperatorCollection m_queryOperatorCollection;

    public QueryField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      QueryFieldType fieldType,
      bool isQueryable)
    {
      this.TestContext = testContext;
      this.ReferenceName = referenceName;
      this.DisplayName = displayName;
      this.FieldType = fieldType;
      this.DefaultWidth = 100;
      this.CanSortBy = true;
      this.IsQueryable = isQueryable;
      this.AllowedOperators = (IEnumerable<QueryOperator>) new QueryOperator[2]
      {
        this.OperatorCollection.EqualTo,
        this.OperatorCollection.NotEquals
      };
    }

    protected TestManagerRequestContext TestContext { get; private set; }

    public QueryOperatorCollection OperatorCollection
    {
      get
      {
        if (this.m_queryOperatorCollection == null)
          this.m_queryOperatorCollection = new QueryOperatorCollection();
        return this.m_queryOperatorCollection;
      }
    }

    public string ReferenceName { get; private set; }

    public string DisplayName { get; private set; }

    public QueryFieldType FieldType { get; private set; }

    public int DefaultWidth { get; set; }

    public bool CanSortBy { get; set; }

    public bool IsQueryable { get; set; }

    public IEnumerable<QueryOperator> AllowedOperators { get; set; }

    public virtual IEnumerable<string> GetSuggestedValues() => (IEnumerable<string>) Array.Empty<string>();

    protected virtual string GetQueryValueString(object rawValue) => rawValue == null ? "''" : "'" + rawValue.ToString().Replace("'", "''") + "'";

    public virtual string ConvertRawValueToDisplayValue(object value) => value == null ? string.Empty : value.ToString();

    public virtual object ConvertDisplayValueToRawValue(string displayValue) => (object) displayValue;

    public virtual string GetConditionString(QueryOperator queryOperator, string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryField.GetConditionString");
        string queryValueString;
        if (string.Equals(queryOperator.RawValue, this.OperatorCollection.In.RawValue) || string.Equals(queryOperator.RawValue, this.OperatorCollection.NotIn.RawValue))
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append('(');
          if (!string.IsNullOrEmpty(displayValue))
          {
            bool flag = false;
            string str1 = displayValue;
            char[] separator = new char[1]{ ',' };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              if (flag)
                stringBuilder.Append(',');
              stringBuilder.Append(this.GetQueryValueString(this.ConvertDisplayValueToRawValue(str2.Trim())));
              flag = true;
            }
          }
          stringBuilder.Append(')');
          queryValueString = stringBuilder.ToString();
        }
        else
          queryValueString = this.GetQueryValueString(this.ConvertDisplayValueToRawValue(displayValue));
        return string.Format("[{0}] {1} {2}", (object) this.ReferenceName, (object) queryOperator.RawValue, (object) queryValueString);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryField.GetConditionString");
      }
    }

    public JsObject ToJson(bool includeAllowedOperatorsAndValues)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryField.ToJson");
        JsObject json = new JsObject();
        json["name"] = (object) this.DisplayName;
        json["type"] = (object) this.FieldType;
        if (includeAllowedOperatorsAndValues)
        {
          json["operators"] = (object) this.AllowedOperators.Select<QueryOperator, string>((Func<QueryOperator, string>) (op => op.DisplayName)).ToArray<string>();
          json["values"] = (object) this.GetSuggestedValues();
        }
        json["defaultWidth"] = (object) this.DefaultWidth;
        json["canSortBy"] = (object) this.CanSortBy;
        return json;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryField.ToJson");
      }
    }
  }
}
