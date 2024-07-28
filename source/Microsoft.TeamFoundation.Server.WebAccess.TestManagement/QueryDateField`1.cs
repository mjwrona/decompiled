// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryDateField`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryDateField<TQueryItem> : QueryField<TQueryItem>
  {
    private Regex s_todayMinusRegex;

    public QueryDateField(
      TestManagerRequestContext testContext,
      string referenceName,
      string displayName,
      bool isQueryable,
      Func<TQueryItem, DateTime> getDataValueFunc)
      : base(testContext, referenceName, displayName, isQueryable, QueryFieldType.DateTime, (Func<TQueryItem, object>) (item => (object) getDataValueFunc(item)))
    {
      this.DefaultWidth = 125;
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[4]
    {
      this.OperatorCollection.GT,
      this.OperatorCollection.LT,
      this.OperatorCollection.GTE,
      this.OperatorCollection.LTE
    };

    protected Regex TodayMinusRegEx
    {
      get
      {
        if (this.s_todayMinusRegex == null)
          this.s_todayMinusRegex = new Regex(string.Format("{0}\\s*(?<sign>[+-])\\s*(?<value>\\d+)?", (object) Regex.Escape(TestManagementServerResources.TodayMacro)), RegexOptions.IgnoreCase);
        return this.s_todayMinusRegex;
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryDateField<TQueryItem>.ConvertDisplayValueToRawValue");
        string format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
        Match match = this.TodayMinusRegEx.Match(displayValue);
        if (match.Success)
        {
          DateTime utcNow = DateTime.UtcNow;
          int int32 = Convert.ToInt32(match.Groups["value"].Value);
          if (match.Groups["sign"].Value.Equals("-"))
            int32 *= -1;
          return (object) DateTime.UtcNow.AddDays((double) int32).ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        }
        if (string.Equals(displayValue, TestManagementServerResources.TodayMacro, StringComparison.CurrentCultureIgnoreCase))
        {
          DateTime dateTime = DateTime.UtcNow;
          dateTime = dateTime.Date;
          return (object) dateTime.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        }
        DateTime result;
        if (DateTime.TryParse(displayValue, out result))
          return (object) result.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        this.TestContext.TraceError("BusinessLayer", string.Format(TestManagementServerResources.IncorrectDateFormat, (object) displayValue));
        throw new TeamFoundationServerException(string.Format(TestManagementServerResources.IncorrectDateFormat, (object) displayValue));
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryDateField<TQueryItem>.ConvertDisplayValueToRawValue");
      }
    }

    public override string ConvertRawValueToDisplayValue(object value) => value is DateTime dateTime && dateTime.Equals(DateTime.MinValue) ? string.Empty : base.ConvertRawValueToDisplayValue(value);
  }
}
