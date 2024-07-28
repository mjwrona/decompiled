// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.ClrCanonicalFunctions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class ClrCanonicalFunctions
  {
    private static string _defaultString = (string) null;
    private static Enum _defaultEnum = (Enum) null;
    internal const string StartswithFunctionName = "startswith";
    internal const string EndswithFunctionName = "endswith";
    internal const string ContainsFunctionName = "contains";
    internal const string SubstringFunctionName = "substring";
    internal const string LengthFunctionName = "length";
    internal const string IndexofFunctionName = "indexof";
    internal const string TolowerFunctionName = "tolower";
    internal const string ToupperFunctionName = "toupper";
    internal const string TrimFunctionName = "trim";
    internal const string ConcatFunctionName = "concat";
    internal const string YearFunctionName = "year";
    internal const string MonthFunctionName = "month";
    internal const string DayFunctionName = "day";
    internal const string HourFunctionName = "hour";
    internal const string MinuteFunctionName = "minute";
    internal const string SecondFunctionName = "second";
    internal const string MillisecondFunctionName = "millisecond";
    internal const string FractionalSecondsFunctionName = "fractionalseconds";
    internal const string RoundFunctionName = "round";
    internal const string FloorFunctionName = "floor";
    internal const string CeilingFunctionName = "ceiling";
    internal const string CastFunctionName = "cast";
    internal const string IsofFunctionName = "isof";
    internal const string DateFunctionName = "date";
    internal const string TimeFunctionName = "time";
    internal const string NowFunctionName = "now";
    internal const string IifFunctionName = "iif";
    public static readonly MethodInfo StartsWith = ClrCanonicalFunctions.MethodOf<bool>((Expression<Func<object, bool>>) (_ => ClrCanonicalFunctions._defaultString.StartsWith(default (string))));
    public static readonly MethodInfo EndsWith = ClrCanonicalFunctions.MethodOf<bool>((Expression<Func<object, bool>>) (_ => ClrCanonicalFunctions._defaultString.EndsWith(default (string))));
    public static readonly MethodInfo Contains = ClrCanonicalFunctions.MethodOf<bool>((Expression<Func<object, bool>>) (_ => ClrCanonicalFunctions._defaultString.Contains(default (string))));
    public static readonly MethodInfo SubstringStart = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrCanonicalFunctions._defaultString.Substring(0)));
    public static readonly MethodInfo SubstringStartAndLength = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrCanonicalFunctions._defaultString.Substring(0, 0)));
    public static readonly MethodInfo SubstringStartNoThrow = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrSafeFunctions.SubstringStart(default (string), 0)));
    public static readonly MethodInfo SubstringStartAndLengthNoThrow = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrSafeFunctions.SubstringStartAndLength(default (string), 0, 0)));
    public static readonly MethodInfo IndexOf = ClrCanonicalFunctions.MethodOf<int>((Expression<Func<object, int>>) (_ => ClrCanonicalFunctions._defaultString.IndexOf(default (string))));
    public static readonly MethodInfo ToLower = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrCanonicalFunctions._defaultString.ToLower()));
    public static readonly MethodInfo ToUpper = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrCanonicalFunctions._defaultString.ToUpper()));
    public static readonly MethodInfo Trim = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => ClrCanonicalFunctions._defaultString.Trim()));
    public static readonly MethodInfo Concat = ClrCanonicalFunctions.MethodOf<string>((Expression<Func<object, string>>) (_ => string.Concat(default (string), default (string))));
    public static readonly MethodInfo CeilingOfDouble = ClrCanonicalFunctions.MethodOf<double>((Expression<Func<object, double>>) (_ => Math.Ceiling(0.0)));
    public static readonly MethodInfo RoundOfDouble = ClrCanonicalFunctions.MethodOf<double>((Expression<Func<object, double>>) (_ => Math.Round(0.0)));
    public static readonly MethodInfo FloorOfDouble = ClrCanonicalFunctions.MethodOf<double>((Expression<Func<object, double>>) (_ => Math.Floor(0.0)));
    public static readonly MethodInfo CeilingOfDecimal = ClrCanonicalFunctions.MethodOf<Decimal>((Expression<Func<object, Decimal>>) (_ => Math.Ceiling(0M)));
    public static readonly MethodInfo RoundOfDecimal = ClrCanonicalFunctions.MethodOf<Decimal>((Expression<Func<object, Decimal>>) (_ => Math.Round(0M)));
    public static readonly MethodInfo FloorOfDecimal = ClrCanonicalFunctions.MethodOf<Decimal>((Expression<Func<object, Decimal>>) (_ => Math.Floor(0M)));
    public static readonly MethodInfo HasFlag = ClrCanonicalFunctions.MethodOf<bool>((Expression<Func<object, bool>>) (_ => ClrCanonicalFunctions._defaultEnum.HasFlag(default (Enum))));
    public static readonly Dictionary<string, PropertyInfo> DateProperties = ((IEnumerable<KeyValuePair<string, PropertyInfo>>) new KeyValuePair<string, PropertyInfo>[3]
    {
      new KeyValuePair<string, PropertyInfo>("year", typeof (Date).GetProperty("Year")),
      new KeyValuePair<string, PropertyInfo>("month", typeof (Date).GetProperty("Month")),
      new KeyValuePair<string, PropertyInfo>("day", typeof (Date).GetProperty("Day"))
    }).ToDictionary<KeyValuePair<string, PropertyInfo>, string, PropertyInfo>((Func<KeyValuePair<string, PropertyInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, PropertyInfo>, PropertyInfo>) (kvp => kvp.Value));
    public static readonly Dictionary<string, PropertyInfo> DateTimeProperties = ((IEnumerable<KeyValuePair<string, PropertyInfo>>) new KeyValuePair<string, PropertyInfo>[7]
    {
      new KeyValuePair<string, PropertyInfo>("year", typeof (DateTime).GetProperty("Year")),
      new KeyValuePair<string, PropertyInfo>("month", typeof (DateTime).GetProperty("Month")),
      new KeyValuePair<string, PropertyInfo>("day", typeof (DateTime).GetProperty("Day")),
      new KeyValuePair<string, PropertyInfo>("hour", typeof (DateTime).GetProperty("Hour")),
      new KeyValuePair<string, PropertyInfo>("minute", typeof (DateTime).GetProperty("Minute")),
      new KeyValuePair<string, PropertyInfo>("second", typeof (DateTime).GetProperty("Second")),
      new KeyValuePair<string, PropertyInfo>("millisecond", typeof (DateTime).GetProperty("Millisecond"))
    }).ToDictionary<KeyValuePair<string, PropertyInfo>, string, PropertyInfo>((Func<KeyValuePair<string, PropertyInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, PropertyInfo>, PropertyInfo>) (kvp => kvp.Value));
    public static readonly Dictionary<string, PropertyInfo> DateTimeOffsetProperties = ((IEnumerable<KeyValuePair<string, PropertyInfo>>) new KeyValuePair<string, PropertyInfo>[7]
    {
      new KeyValuePair<string, PropertyInfo>("year", typeof (DateTimeOffset).GetProperty("Year")),
      new KeyValuePair<string, PropertyInfo>("month", typeof (DateTimeOffset).GetProperty("Month")),
      new KeyValuePair<string, PropertyInfo>("day", typeof (DateTimeOffset).GetProperty("Day")),
      new KeyValuePair<string, PropertyInfo>("hour", typeof (DateTimeOffset).GetProperty("Hour")),
      new KeyValuePair<string, PropertyInfo>("minute", typeof (DateTimeOffset).GetProperty("Minute")),
      new KeyValuePair<string, PropertyInfo>("second", typeof (DateTimeOffset).GetProperty("Second")),
      new KeyValuePair<string, PropertyInfo>("millisecond", typeof (DateTimeOffset).GetProperty("Millisecond"))
    }).ToDictionary<KeyValuePair<string, PropertyInfo>, string, PropertyInfo>((Func<KeyValuePair<string, PropertyInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, PropertyInfo>, PropertyInfo>) (kvp => kvp.Value));
    public static readonly Dictionary<string, PropertyInfo> TimeOfDayProperties = ((IEnumerable<KeyValuePair<string, PropertyInfo>>) new KeyValuePair<string, PropertyInfo>[4]
    {
      new KeyValuePair<string, PropertyInfo>("hour", typeof (TimeOfDay).GetProperty("Hours")),
      new KeyValuePair<string, PropertyInfo>("minute", typeof (TimeOfDay).GetProperty("Minutes")),
      new KeyValuePair<string, PropertyInfo>("second", typeof (TimeOfDay).GetProperty("Seconds")),
      new KeyValuePair<string, PropertyInfo>("millisecond", typeof (TimeOfDay).GetProperty("Milliseconds"))
    }).ToDictionary<KeyValuePair<string, PropertyInfo>, string, PropertyInfo>((Func<KeyValuePair<string, PropertyInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, PropertyInfo>, PropertyInfo>) (kvp => kvp.Value));
    public static readonly Dictionary<string, PropertyInfo> TimeSpanProperties = ((IEnumerable<KeyValuePair<string, PropertyInfo>>) new KeyValuePair<string, PropertyInfo>[4]
    {
      new KeyValuePair<string, PropertyInfo>("hour", typeof (TimeSpan).GetProperty("Hours")),
      new KeyValuePair<string, PropertyInfo>("minute", typeof (TimeSpan).GetProperty("Minutes")),
      new KeyValuePair<string, PropertyInfo>("second", typeof (TimeSpan).GetProperty("Seconds")),
      new KeyValuePair<string, PropertyInfo>("millisecond", typeof (TimeSpan).GetProperty("Milliseconds"))
    }).ToDictionary<KeyValuePair<string, PropertyInfo>, string, PropertyInfo>((Func<KeyValuePair<string, PropertyInfo>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, PropertyInfo>, PropertyInfo>) (kvp => kvp.Value));
    public static readonly PropertyInfo Length = typeof (string).GetProperty(nameof (Length));
    public static readonly PropertyInfo DateTimeKindPropertyInfo = typeof (DateTime).GetProperty("Kind");
    public static readonly MethodInfo ToUniversalTimeDateTime = typeof (DateTime).GetMethod("ToUniversalTime", BindingFlags.Instance | BindingFlags.Public);
    public static readonly MethodInfo ToUniversalTimeDateTimeOffset = typeof (DateTimeOffset).GetMethod("ToUniversalTime", BindingFlags.Instance | BindingFlags.Public);
    public static readonly MethodInfo ToOffsetFunction = typeof (DateTimeOffset).GetMethod("ToOffset", BindingFlags.Instance | BindingFlags.Public);
    public static readonly MethodInfo GetUtcOffset = typeof (TimeZoneInfo).GetMethod(nameof (GetUtcOffset), new Type[1]
    {
      typeof (DateTime)
    });

    private static MethodInfo MethodOf<TReturn>(Expression<Func<object, TReturn>> expression) => ClrCanonicalFunctions.MethodOf((Expression) expression);

    private static MethodInfo MethodOf(Expression expression) => ((expression as LambdaExpression).Body as MethodCallExpression).Method;
  }
}
