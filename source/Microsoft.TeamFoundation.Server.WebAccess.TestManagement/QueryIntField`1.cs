// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryIntField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryIntField<TQueryItem> : QueryField<TQueryItem>
  {
    protected Dictionary<int, string> valuesMap;

    public QueryIntField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, int> getDataValueFunc,
      Dictionary<int, string> valuesMap = null)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.Integer, (Func<TQueryItem, object>) (item => (object) getDataValueFunc(item)))
    {
      this.valuesMap = valuesMap;
      this.DefaultWidth = 60;
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[8]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals,
      this.OperatorCollection.GT,
      this.OperatorCollection.LT,
      this.OperatorCollection.GTE,
      this.OperatorCollection.LTE,
      this.OperatorCollection.In,
      this.OperatorCollection.NotIn
    };

    public virtual Dictionary<int, string> GetIdToNameMap() => new Dictionary<int, string>();

    public override IEnumerable<string> GetSuggestedValues() => this.valuesMap != null ? (IEnumerable<string>) this.valuesMap.Values.ToArray<string>() : base.GetSuggestedValues();

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryIntField<TQueryItem>.ConvertRawValueToDisplayValue");
        string str;
        return this.valuesMap != null && this.valuesMap.TryGetValue((int) value, out str) ? str : base.ConvertRawValueToDisplayValue(value);
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryIntField<TQueryItem>.ConvertRawValueToDisplayValue");
      }
    }
  }
}
