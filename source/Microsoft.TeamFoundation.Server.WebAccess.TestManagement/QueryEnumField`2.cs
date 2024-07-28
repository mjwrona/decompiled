// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryEnumField`2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryEnumField<TQueryItem, TEnumType> : QueryIntField<TQueryItem>
  {
    public QueryEnumField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, int> getDataValueFunc,
      Dictionary<int, string> valuesMap)
      : base(testContext, referenceName, displayName, isQueryable, getDataValueFunc, valuesMap)
    {
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[4]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals,
      this.OperatorCollection.In,
      this.OperatorCollection.NotIn
    };

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryEnumField.ConvertDisplayValueToRawValue");
        if (!string.IsNullOrEmpty(displayValue) && this.valuesMap != null && this.valuesMap.Count > 0)
        {
          List<KeyValuePair<int, string>> list = this.valuesMap.Where<KeyValuePair<int, string>>((Func<KeyValuePair<int, string>, bool>) (m => string.Equals(m.Value, displayValue, StringComparison.CurrentCultureIgnoreCase))).ToList<KeyValuePair<int, string>>();
          if (list.Count > 0)
          {
            TestManagerRequestContext testContext = this.TestContext;
            object[] objArray = new object[2]
            {
              (object) displayValue,
              null
            };
            KeyValuePair<int, string> keyValuePair = list.First<KeyValuePair<int, string>>();
            objArray[1] = (object) keyValuePair.Value;
            testContext.TraceVerbose("BusinessLayer", "Display Value: {0}, Mapped Value: {1}", objArray);
            Type enumType = typeof (TEnumType);
            keyValuePair = list.First<KeyValuePair<int, string>>();
            int key = keyValuePair.Key;
            object obj = Enum.ToObject(enumType, key);
            if (obj != null)
              return (object) obj.ToString();
          }
        }
        return (object) displayValue;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryEnumField.ConvertDisplayValueToRawValue");
      }
    }
  }
}
