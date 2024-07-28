// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryTimeSpanField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryTimeSpanField<TQueryItem> : QueryField<TQueryItem>
  {
    public QueryTimeSpanField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, long> getDataValueFunc)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.TimeSpan, (Func<TQueryItem, object>) (item => (object) getDataValueFunc(item)))
    {
      this.DefaultWidth = 80;
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

    public override string ConvertRawValueToDisplayValue(object value)
    {
      TimeSpan timeSpan = TimeSpan.FromTicks((long) value);
      return string.Format("{0:0}:{1:00}:{2:00}.{3:000}", (object) timeSpan.Hours, (object) timeSpan.Minutes, (object) timeSpan.Seconds, (object) timeSpan.Milliseconds);
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      TimeSpan result;
      return TimeSpan.TryParse(displayValue, out result) ? (object) result.Ticks : (object) 0L;
    }

    protected override string GetQueryValueString(object rawValue) => ((long) rawValue).ToString();
  }
}
