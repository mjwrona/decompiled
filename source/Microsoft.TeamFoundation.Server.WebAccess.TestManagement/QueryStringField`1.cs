// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryStringField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryStringField<TQueryItem> : QueryField<TQueryItem>
  {
    private Dictionary<string, string> m_valuesMap;

    public QueryStringField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, string> getDataValueFunc,
      Dictionary<string, string> valuesMap = null)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.String, (Func<TQueryItem, object>) getDataValueFunc)
    {
      this.m_valuesMap = valuesMap;
      this.DefaultWidth = 150;
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[6]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals,
      this.OperatorCollection.Contains,
      this.OperatorCollection.NotContains,
      this.OperatorCollection.In,
      this.OperatorCollection.NotIn
    };

    public override IEnumerable<string> GetSuggestedValues() => this.m_valuesMap != null ? (IEnumerable<string>) this.m_valuesMap.Values.ToArray<string>() : base.GetSuggestedValues();

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryStringField<TQueryItem>.ConvertRawValueToDisplayValue");
        string str;
        return this.m_valuesMap != null && this.m_valuesMap.TryGetValue((string) value, out str) ? str : base.ConvertRawValueToDisplayValue(value);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryStringField<TQueryItem>.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryStringField<TQueryItem>.ConvertDisplayValueToRawValue");
        if (this.m_valuesMap != null)
        {
          foreach (KeyValuePair<string, string> values in this.m_valuesMap)
          {
            if (string.Equals(values.Value, displayValue, StringComparison.CurrentCultureIgnoreCase))
              return (object) values.Key;
          }
          this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.ErrorFieldValueNotAllowedFormat, (object) displayValue, (object) this.DisplayName));
          throw new TeamFoundationServerException(string.Format(TestManagementServerResources.ErrorFieldValueNotAllowedFormat, (object) displayValue, (object) this.DisplayName));
        }
        return base.ConvertDisplayValueToRawValue(displayValue);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryStringField<TQueryItem>.ConvertDisplayValueToRawValue");
      }
    }
  }
}
