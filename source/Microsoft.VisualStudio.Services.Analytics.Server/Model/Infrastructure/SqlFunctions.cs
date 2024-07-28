// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.SqlFunctions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public static class SqlFunctions
  {
    public static long ApproxCountDistinctImpl<T>(IEnumerable<T> values) => values.Distinct<T>().LongCount<T>();

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Boolean")]
    public static long ApproxCountDistinct(IEnumerable<bool?> values) => SqlFunctions.ApproxCountDistinctImpl<bool?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Boolean")]
    public static long ApproxCountDistinct(IEnumerable<bool> values) => SqlFunctions.ApproxCountDistinctImpl<bool>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Byte")]
    public static long ApproxCountDistinct(IEnumerable<byte?> values) => SqlFunctions.ApproxCountDistinctImpl<byte?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Byte")]
    public static long ApproxCountDistinct(IEnumerable<byte> values) => SqlFunctions.ApproxCountDistinctImpl<byte>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int16")]
    public static long ApproxCountDistinct(IEnumerable<short?> values) => SqlFunctions.ApproxCountDistinctImpl<short?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int16")]
    public static long ApproxCountDistinct(IEnumerable<short> values) => SqlFunctions.ApproxCountDistinctImpl<short>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int32")]
    public static long ApproxCountDistinct(IEnumerable<int?> values) => SqlFunctions.ApproxCountDistinctImpl<int?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int32")]
    public static long ApproxCountDistinct(IEnumerable<int> values) => SqlFunctions.ApproxCountDistinctImpl<int>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int64")]
    public static long ApproxCountDistinct(IEnumerable<long?> values) => SqlFunctions.ApproxCountDistinctImpl<long?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Int64")]
    public static long ApproxCountDistinct(IEnumerable<long> values) => SqlFunctions.ApproxCountDistinctImpl<long>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_String")]
    public static long ApproxCountDistinct(IEnumerable<string> values) => SqlFunctions.ApproxCountDistinctImpl<string>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Guid")]
    public static long ApproxCountDistinct(IEnumerable<Guid?> values) => SqlFunctions.ApproxCountDistinctImpl<Guid?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Guid")]
    public static long ApproxCountDistinct(IEnumerable<Guid> values) => SqlFunctions.ApproxCountDistinctImpl<Guid>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_DateTimeOffset")]
    public static long ApproxCountDistinct(IEnumerable<DateTimeOffset?> values) => SqlFunctions.ApproxCountDistinctImpl<DateTimeOffset?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_DateTimeOffset")]
    public static long ApproxCountDistinct(IEnumerable<DateTimeOffset> values) => SqlFunctions.ApproxCountDistinctImpl<DateTimeOffset>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Double")]
    public static long ApproxCountDistinct(IEnumerable<double?> values) => SqlFunctions.ApproxCountDistinctImpl<double?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Double")]
    public static long ApproxCountDistinct(IEnumerable<double> values) => SqlFunctions.ApproxCountDistinctImpl<double>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Decimal")]
    public static long ApproxCountDistinct(IEnumerable<Decimal?> values) => SqlFunctions.ApproxCountDistinctImpl<Decimal?>(values);

    [DbFunction("AX", "APPROX_COUNT_DISTINCT_Decimal")]
    public static long ApproxCountDistinct(IEnumerable<Decimal> values) => SqlFunctions.ApproxCountDistinctImpl<Decimal>(values);

    private static double PercentileContImpl<T>(T value, Decimal percentile, params object[] grp) => -17.5;

    private static double PercentileContImpl<T>(T value, Decimal? percentile, params object[] grp) => -17.5;

    private static T PercentileDiscImpl<T>(T value, Decimal percentile, params object[] grp) => (T) Convert.ChangeType((object) -3, typeof (T));

    private static T PercentileDiscImpl<T>(T value, Decimal? percentile, params object[] grp) => (T) Convert.ChangeType((object) -3, typeof (T));

    private static T? PercentileDiscImpl<T>(T? value, Decimal percentile, params object[] grp) where T : struct => (T?) Convert.ChangeType((object) -3, typeof (T));

    private static T? PercentileDiscImpl<T>(T? value, Decimal? percentile, params object[] grp) where T : struct => (T?) Convert.ChangeType((object) -3, typeof (T));

    [DbFunction("AnalyticsModel", "PredictWorkItemCompletedTime")]
    public static double? PredictWorkItemCompletedTime(
      int partitionId,
      int workItemId,
      int? revision,
      string state,
      string workItemType)
    {
      return new double?(-42.0);
    }

    public static Dictionary<Type, MethodInfo> GetAggregationCustomMethods(string methodName) => SqlFunctions.GetAggregationCustomMethods(typeof (SqlFunctions), methodName);

    public static Dictionary<Type, MethodInfo> GetAggregationCustomMethods(
      Type type,
      string methodName)
    {
      return ((IEnumerable<MethodInfo>) type.GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == methodName)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).Count<ParameterInfo>() == 1)).ToDictionary<MethodInfo, Type>((Func<MethodInfo, Type>) (m => ((IEnumerable<Type>) ((IEnumerable<ParameterInfo>) m.GetParameters()).First<ParameterInfo>().ParameterType.GetGenericArguments()).First<Type>()));
    }

    public static IEnumerable<MethodInfo> GeCustomMethods(string methodName) => SqlFunctions.GeCustomMethods(typeof (SqlFunctions), methodName);

    public static IEnumerable<MethodInfo> GeCustomMethods(Type type, string methodName) => ((IEnumerable<MethodInfo>) type.GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == methodName));

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal")]
    public static double PercentileCont(byte value, Decimal grp0) => SqlFunctions.PercentileContImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal")]
    public static byte PercentileDisc(byte value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal")]
    public static double PercentileCont(byte? value, Decimal grp0) => SqlFunctions.PercentileContImpl<byte?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal")]
    public static byte? PercentileDisc(byte? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal")]
    public static double PercentileCont(byte value, Decimal? grp0) => SqlFunctions.PercentileContImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal")]
    public static byte PercentileDisc(byte value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal")]
    public static double PercentileCont(byte? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<byte?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_String")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_String")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_String")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_String_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_String_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Int32_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Int32_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_String")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_String")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Int32")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Int32")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte PercentileDisc(byte value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Byte_Decimal_Guid_Guid")]
    public static double PercentileCont(byte? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<byte?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Byte_Decimal_Guid_Guid")]
    public static byte? PercentileDisc(byte? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<byte>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal")]
    public static double PercentileCont(short value, Decimal grp0) => SqlFunctions.PercentileContImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal")]
    public static short PercentileDisc(short value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal")]
    public static double PercentileCont(short? value, Decimal grp0) => SqlFunctions.PercentileContImpl<short?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal")]
    public static short? PercentileDisc(short? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal")]
    public static double PercentileCont(short value, Decimal? grp0) => SqlFunctions.PercentileContImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal")]
    public static short PercentileDisc(short value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal")]
    public static double PercentileCont(short? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<short?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal")]
    public static short? PercentileDisc(short? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<short>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String")]
    public static double PercentileCont(short value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_String")]
    public static double PercentileCont(short value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_String")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_String")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_String")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_String")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_String")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_String_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_String_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_String")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Int32_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Int32_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_String")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_String")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Int32")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Int32")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short PercentileDisc(short value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int16_Decimal_Guid_Guid")]
    public static double PercentileCont(short? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<short?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int16_Decimal_Guid_Guid")]
    public static short? PercentileDisc(short? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<short>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal")]
    public static double PercentileCont(int value, Decimal grp0) => SqlFunctions.PercentileContImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal")]
    public static int PercentileDisc(int value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal")]
    public static double PercentileCont(int? value, Decimal grp0) => SqlFunctions.PercentileContImpl<int?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal")]
    public static int? PercentileDisc(int? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal")]
    public static double PercentileCont(int value, Decimal? grp0) => SqlFunctions.PercentileContImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal")]
    public static int PercentileDisc(int value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal")]
    public static double PercentileCont(int? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<int?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal")]
    public static int? PercentileDisc(int? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<int>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String")]
    public static double PercentileCont(int value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_String")]
    public static double PercentileCont(int value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_String")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_String")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_String")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_String")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_String")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_String_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_String_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_String")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Int32_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Int32_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_String")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_String")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Int32")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Int32")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int PercentileDisc(int value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int32_Decimal_Guid_Guid")]
    public static double PercentileCont(int? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<int?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int32_Decimal_Guid_Guid")]
    public static int? PercentileDisc(int? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<int>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal")]
    public static double PercentileCont(long value, Decimal grp0) => SqlFunctions.PercentileContImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal")]
    public static long PercentileDisc(long value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal")]
    public static double PercentileCont(long? value, Decimal grp0) => SqlFunctions.PercentileContImpl<long?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal")]
    public static long? PercentileDisc(long? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal")]
    public static double PercentileCont(long value, Decimal? grp0) => SqlFunctions.PercentileContImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal")]
    public static long PercentileDisc(long value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal")]
    public static double PercentileCont(long? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<long?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal")]
    public static long? PercentileDisc(long? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<long>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String")]
    public static double PercentileCont(long value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_String")]
    public static double PercentileCont(long value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_String")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_String")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_String")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_String")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_String")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_String_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_String_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_String")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Int32_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Int32_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_String")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_String")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Int32")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Int32")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long PercentileDisc(long value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Int64_Decimal_Guid_Guid")]
    public static double PercentileCont(long? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<long?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Int64_Decimal_Guid_Guid")]
    public static long? PercentileDisc(long? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<long>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal")]
    public static double PercentileCont(Decimal value, Decimal grp0) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal")]
    public static double PercentileCont(Decimal? value, Decimal grp0) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal")]
    public static double PercentileCont(Decimal value, Decimal? grp0) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal")]
    public static double PercentileCont(Decimal? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Int32_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Int32_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_String")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_String")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Int32")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Int32")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal PercentileDisc(Decimal value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_Guid_Guid")]
    public static double PercentileCont(Decimal? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_Guid_Guid")]
    public static Decimal? PercentileDisc(Decimal? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal")]
    public static double PercentileCont(double value, Decimal grp0) => SqlFunctions.PercentileContImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal")]
    public static double PercentileDisc(double value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal")]
    public static double PercentileCont(double? value, Decimal grp0) => SqlFunctions.PercentileContImpl<double?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal")]
    public static double? PercentileDisc(double? value, Decimal grp0) => SqlFunctions.PercentileDiscImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal")]
    public static double PercentileCont(double value, Decimal? grp0) => SqlFunctions.PercentileContImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal")]
    public static double PercentileDisc(double value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal")]
    public static double PercentileCont(double? value, Decimal? grp0) => SqlFunctions.PercentileContImpl<double?>(value, grp0);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal")]
    public static double? PercentileDisc(double? value, Decimal? grp0) => SqlFunctions.PercentileDiscImpl<double>(value, grp0);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String")]
    public static double PercentileCont(double value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_String")]
    public static double PercentileCont(double value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_String")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_String")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_String")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_String")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_String")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_String_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_String_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, string grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_String")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Int32_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Int32_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, int? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_String")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_String")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1, string grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1, int grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Int32")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Int32")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1, int? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1, Guid grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double PercentileDisc(double value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Double_Decimal_Guid_Guid")]
    public static double PercentileCont(double? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileContImpl<double?>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_DISC_Double_Decimal_Guid_Guid")]
    public static double? PercentileDisc(double? value, Decimal? grp0, Guid? grp1, Guid? grp2) => SqlFunctions.PercentileDiscImpl<double>(value, grp0, (object) grp1, (object) grp2);

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal value,
      Decimal grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal PercentileDisc(
      Decimal value,
      Decimal grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal? value,
      Decimal grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal? PercentileDisc(
      Decimal? value,
      Decimal grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal PercentileDisc(
      Decimal value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal? value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal? PercentileDisc(
      Decimal? value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal value,
      Decimal grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal PercentileDisc(
      Decimal value,
      Decimal grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal? value,
      Decimal grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal? PercentileDisc(
      Decimal? value,
      Decimal grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal PercentileDisc(
      Decimal value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_CONT_Decimal_Decimal_String_String_Int32")]
    public static double PercentileCont(
      Decimal? value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileContImpl<Decimal?>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "PERCENTILE_DISC_Decimal_Decimal_String_String_Int32")]
    public static Decimal? PercentileDisc(
      Decimal? value,
      Decimal? grp0,
      string grp1,
      string grp2,
      int? grp3)
    {
      return SqlFunctions.PercentileDiscImpl<Decimal>(value, grp0, (object) grp1, (object) grp2, (object) grp3);
    }

    [DbFunction("AX", "LAG_String_Int32_String")]
    public static string Lag(string value, int grp0, string grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_String")]
    public static string Lead(string value, int grp0, string grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_String")]
    public static string Lag(string value, int? grp0, string grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_String")]
    public static string Lead(string value, int? grp0, string grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_String")]
    public static string Lag(string value, DateTimeOffset grp0, string grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_String")]
    public static string Lead(string value, DateTimeOffset grp0, string grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_String")]
    public static string Lag(string value, DateTimeOffset? grp0, string grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_String")]
    public static string Lead(string value, DateTimeOffset? grp0, string grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Int32")]
    public static string Lag(string value, int grp0, int grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Int32")]
    public static string Lead(string value, int grp0, int grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Int32")]
    public static string Lag(string value, int? grp0, int grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Int32")]
    public static string Lead(string value, int? grp0, int grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Int32")]
    public static string Lag(string value, int grp0, int? grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Int32")]
    public static string Lead(string value, int grp0, int? grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Int32")]
    public static string Lag(string value, int? grp0, int? grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Int32")]
    public static string Lead(string value, int? grp0, int? grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Int32")]
    public static string Lag(string value, DateTimeOffset grp0, int grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Int32")]
    public static string Lead(string value, DateTimeOffset grp0, int grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Int32")]
    public static string Lag(string value, DateTimeOffset? grp0, int grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Int32")]
    public static string Lead(string value, DateTimeOffset? grp0, int grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Int32")]
    public static string Lag(string value, DateTimeOffset grp0, int? grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Int32")]
    public static string Lead(string value, DateTimeOffset grp0, int? grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Int32")]
    public static string Lag(string value, DateTimeOffset? grp0, int? grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Int32")]
    public static string Lead(string value, DateTimeOffset? grp0, int? grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Guid")]
    public static string Lag(string value, int grp0, Guid grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Guid")]
    public static string Lead(string value, int grp0, Guid grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Guid")]
    public static string Lag(string value, int? grp0, Guid grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Guid")]
    public static string Lead(string value, int? grp0, Guid grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Guid")]
    public static string Lag(string value, int grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Guid")]
    public static string Lead(string value, int grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LAG_String_Int32_Guid")]
    public static string Lag(string value, int? grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LEAD_String_Int32_Guid")]
    public static string Lead(string value, int? grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Guid")]
    public static string Lag(string value, DateTimeOffset grp0, Guid grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Guid")]
    public static string Lead(string value, DateTimeOffset grp0, Guid grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Guid")]
    public static string Lag(string value, DateTimeOffset? grp0, Guid grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Guid")]
    public static string Lead(string value, DateTimeOffset? grp0, Guid grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Guid")]
    public static string Lag(string value, DateTimeOffset grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Guid")]
    public static string Lead(string value, DateTimeOffset grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LAG_String_DateTimeOffset_Guid")]
    public static string Lag(string value, DateTimeOffset? grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LEAD_String_DateTimeOffset_Guid")]
    public static string Lead(string value, DateTimeOffset? grp0, Guid? grp1) => "17";

    [DbFunction("AX", "LAG_Int32_Int32_String")]
    public static int Lag(int value, int grp0, string grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_String")]
    public static int Lead(int value, int grp0, string grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_String")]
    public static int? Lag(int? value, int grp0, string grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_String")]
    public static int? Lead(int? value, int grp0, string grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_String")]
    public static int Lag(int value, int? grp0, string grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_String")]
    public static int Lead(int value, int? grp0, string grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_String")]
    public static int? Lag(int? value, int? grp0, string grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_String")]
    public static int? Lead(int? value, int? grp0, string grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_String")]
    public static int Lag(int value, DateTimeOffset grp0, string grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_String")]
    public static int Lead(int value, DateTimeOffset grp0, string grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_String")]
    public static int? Lag(int? value, DateTimeOffset grp0, string grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_String")]
    public static int? Lead(int? value, DateTimeOffset grp0, string grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_String")]
    public static int Lag(int value, DateTimeOffset? grp0, string grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_String")]
    public static int Lead(int value, DateTimeOffset? grp0, string grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_String")]
    public static int? Lag(int? value, DateTimeOffset? grp0, string grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_String")]
    public static int? Lead(int? value, DateTimeOffset? grp0, string grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int Lag(int value, int grp0, int grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int Lead(int value, int grp0, int grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int? Lag(int? value, int grp0, int grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int? Lead(int? value, int grp0, int grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int Lag(int value, int? grp0, int grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int Lead(int value, int? grp0, int grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int? Lag(int? value, int? grp0, int grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int? Lead(int? value, int? grp0, int grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int Lag(int value, int grp0, int? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int Lead(int value, int grp0, int? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int? Lag(int? value, int grp0, int? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int? Lead(int? value, int grp0, int? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int Lag(int value, int? grp0, int? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int Lead(int value, int? grp0, int? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Int32")]
    public static int? Lag(int? value, int? grp0, int? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Int32")]
    public static int? Lead(int? value, int? grp0, int? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int Lag(int value, DateTimeOffset grp0, int grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int Lead(int value, DateTimeOffset grp0, int grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int? Lag(int? value, DateTimeOffset grp0, int grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int? Lead(int? value, DateTimeOffset grp0, int grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int Lag(int value, DateTimeOffset? grp0, int grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int Lead(int value, DateTimeOffset? grp0, int grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int? Lag(int? value, DateTimeOffset? grp0, int grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int? Lead(int? value, DateTimeOffset? grp0, int grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int Lag(int value, DateTimeOffset grp0, int? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int Lead(int value, DateTimeOffset grp0, int? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int? Lag(int? value, DateTimeOffset grp0, int? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int? Lead(int? value, DateTimeOffset grp0, int? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int Lag(int value, DateTimeOffset? grp0, int? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int Lead(int value, DateTimeOffset? grp0, int? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Int32")]
    public static int? Lag(int? value, DateTimeOffset? grp0, int? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Int32")]
    public static int? Lead(int? value, DateTimeOffset? grp0, int? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int Lag(int value, int grp0, Guid grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int Lead(int value, int grp0, Guid grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int? Lag(int? value, int grp0, Guid grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int? Lead(int? value, int grp0, Guid grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int Lag(int value, int? grp0, Guid grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int Lead(int value, int? grp0, Guid grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int? Lag(int? value, int? grp0, Guid grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int? Lead(int? value, int? grp0, Guid grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int Lag(int value, int grp0, Guid? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int Lead(int value, int grp0, Guid? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int? Lag(int? value, int grp0, Guid? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int? Lead(int? value, int grp0, Guid? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int Lag(int value, int? grp0, Guid? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int Lead(int value, int? grp0, Guid? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_Int32_Guid")]
    public static int? Lag(int? value, int? grp0, Guid? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_Int32_Guid")]
    public static int? Lead(int? value, int? grp0, Guid? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int Lag(int value, DateTimeOffset grp0, Guid grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int Lead(int value, DateTimeOffset grp0, Guid grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int? Lag(int? value, DateTimeOffset grp0, Guid grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int? Lead(int? value, DateTimeOffset grp0, Guid grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int Lag(int value, DateTimeOffset? grp0, Guid grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int Lead(int value, DateTimeOffset? grp0, Guid grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int? Lag(int? value, DateTimeOffset? grp0, Guid grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int? Lead(int? value, DateTimeOffset? grp0, Guid grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int Lag(int value, DateTimeOffset grp0, Guid? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int Lead(int value, DateTimeOffset grp0, Guid? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int? Lag(int? value, DateTimeOffset grp0, Guid? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int? Lead(int? value, DateTimeOffset grp0, Guid? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int Lag(int value, DateTimeOffset? grp0, Guid? grp1) => 17;

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int Lead(int value, DateTimeOffset? grp0, Guid? grp1) => -17;

    [DbFunction("AX", "LAG_Int32_DateTimeOffset_Guid")]
    public static int? Lag(int? value, DateTimeOffset? grp0, Guid? grp1) => new int?(17);

    [DbFunction("AX", "LEAD_Int32_DateTimeOffset_Guid")]
    public static int? Lead(int? value, DateTimeOffset? grp0, Guid? grp1) => new int?(-17);

    [DbFunction("AX", "LAG_Decimal_Int32_String")]
    public static Decimal Lag(Decimal value, int grp0, string grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_String")]
    public static Decimal Lead(Decimal value, int grp0, string grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_String")]
    public static Decimal? Lag(Decimal? value, int grp0, string grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_String")]
    public static Decimal? Lead(Decimal? value, int grp0, string grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_String")]
    public static Decimal Lag(Decimal value, int? grp0, string grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_String")]
    public static Decimal Lead(Decimal value, int? grp0, string grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_String")]
    public static Decimal? Lag(Decimal? value, int? grp0, string grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_String")]
    public static Decimal? Lead(Decimal? value, int? grp0, string grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_String")]
    public static Decimal Lag(Decimal value, DateTimeOffset grp0, string grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_String")]
    public static Decimal Lead(Decimal value, DateTimeOffset grp0, string grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_String")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset grp0, string grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_String")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset grp0, string grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_String")]
    public static Decimal Lag(Decimal value, DateTimeOffset? grp0, string grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_String")]
    public static Decimal Lead(Decimal value, DateTimeOffset? grp0, string grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_String")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset? grp0, string grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_String")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset? grp0, string grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal Lag(Decimal value, int grp0, int grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal Lead(Decimal value, int grp0, int grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal? Lag(Decimal? value, int grp0, int grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal? Lead(Decimal? value, int grp0, int grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal Lag(Decimal value, int? grp0, int grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal Lead(Decimal value, int? grp0, int grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal? Lag(Decimal? value, int? grp0, int grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal? Lead(Decimal? value, int? grp0, int grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal Lag(Decimal value, int grp0, int? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal Lead(Decimal value, int grp0, int? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal? Lag(Decimal? value, int grp0, int? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal? Lead(Decimal? value, int grp0, int? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal Lag(Decimal value, int? grp0, int? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal Lead(Decimal value, int? grp0, int? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Int32")]
    public static Decimal? Lag(Decimal? value, int? grp0, int? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Int32")]
    public static Decimal? Lead(Decimal? value, int? grp0, int? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lag(Decimal value, DateTimeOffset grp0, int grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lead(Decimal value, DateTimeOffset grp0, int grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset grp0, int grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset grp0, int grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lag(Decimal value, DateTimeOffset? grp0, int grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lead(Decimal value, DateTimeOffset? grp0, int grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset? grp0, int grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset? grp0, int grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lag(Decimal value, DateTimeOffset grp0, int? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lead(Decimal value, DateTimeOffset grp0, int? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset grp0, int? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset grp0, int? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lag(Decimal value, DateTimeOffset? grp0, int? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal Lead(Decimal value, DateTimeOffset? grp0, int? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset? grp0, int? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Int32")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset? grp0, int? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal Lag(Decimal value, int grp0, Guid grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal Lead(Decimal value, int grp0, Guid grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal? Lag(Decimal? value, int grp0, Guid grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal? Lead(Decimal? value, int grp0, Guid grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal Lag(Decimal value, int? grp0, Guid grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal Lead(Decimal value, int? grp0, Guid grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal? Lag(Decimal? value, int? grp0, Guid grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal? Lead(Decimal? value, int? grp0, Guid grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal Lag(Decimal value, int grp0, Guid? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal Lead(Decimal value, int grp0, Guid? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal? Lag(Decimal? value, int grp0, Guid? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal? Lead(Decimal? value, int grp0, Guid? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal Lag(Decimal value, int? grp0, Guid? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal Lead(Decimal value, int? grp0, Guid? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_Int32_Guid")]
    public static Decimal? Lag(Decimal? value, int? grp0, Guid? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_Int32_Guid")]
    public static Decimal? Lead(Decimal? value, int? grp0, Guid? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lag(Decimal value, DateTimeOffset grp0, Guid grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lead(Decimal value, DateTimeOffset grp0, Guid grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset grp0, Guid grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset grp0, Guid grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lag(Decimal value, DateTimeOffset? grp0, Guid grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lead(Decimal value, DateTimeOffset? grp0, Guid grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset? grp0, Guid grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset? grp0, Guid grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lag(Decimal value, DateTimeOffset grp0, Guid? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lead(Decimal value, DateTimeOffset grp0, Guid? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset grp0, Guid? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset grp0, Guid? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lag(Decimal value, DateTimeOffset? grp0, Guid? grp1) => 17M;

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal Lead(Decimal value, DateTimeOffset? grp0, Guid? grp1) => -17M;

    [DbFunction("AX", "LAG_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lag(Decimal? value, DateTimeOffset? grp0, Guid? grp1) => new Decimal?((Decimal) 17);

    [DbFunction("AX", "LEAD_Decimal_DateTimeOffset_Guid")]
    public static Decimal? Lead(Decimal? value, DateTimeOffset? grp0, Guid? grp1) => new Decimal?((Decimal) -17);

    [DbFunction("AX", "LAG_Double_Int32_String")]
    public static double Lag(double value, int grp0, string grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_String")]
    public static double Lead(double value, int grp0, string grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_String")]
    public static double? Lag(double? value, int grp0, string grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_String")]
    public static double? Lead(double? value, int grp0, string grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_String")]
    public static double Lag(double value, int? grp0, string grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_String")]
    public static double Lead(double value, int? grp0, string grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_String")]
    public static double? Lag(double? value, int? grp0, string grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_String")]
    public static double? Lead(double? value, int? grp0, string grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_String")]
    public static double Lag(double value, DateTimeOffset grp0, string grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_String")]
    public static double Lead(double value, DateTimeOffset grp0, string grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_String")]
    public static double? Lag(double? value, DateTimeOffset grp0, string grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_String")]
    public static double? Lead(double? value, DateTimeOffset grp0, string grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_String")]
    public static double Lag(double value, DateTimeOffset? grp0, string grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_String")]
    public static double Lead(double value, DateTimeOffset? grp0, string grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_String")]
    public static double? Lag(double? value, DateTimeOffset? grp0, string grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_String")]
    public static double? Lead(double? value, DateTimeOffset? grp0, string grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double Lag(double value, int grp0, int grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double Lead(double value, int grp0, int grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double? Lag(double? value, int grp0, int grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double? Lead(double? value, int grp0, int grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double Lag(double value, int? grp0, int grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double Lead(double value, int? grp0, int grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double? Lag(double? value, int? grp0, int grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double? Lead(double? value, int? grp0, int grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double Lag(double value, int grp0, int? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double Lead(double value, int grp0, int? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double? Lag(double? value, int grp0, int? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double? Lead(double? value, int grp0, int? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double Lag(double value, int? grp0, int? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double Lead(double value, int? grp0, int? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Int32")]
    public static double? Lag(double? value, int? grp0, int? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Int32")]
    public static double? Lead(double? value, int? grp0, int? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double Lag(double value, DateTimeOffset grp0, int grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double Lead(double value, DateTimeOffset grp0, int grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double? Lag(double? value, DateTimeOffset grp0, int grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double? Lead(double? value, DateTimeOffset grp0, int grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double Lag(double value, DateTimeOffset? grp0, int grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double Lead(double value, DateTimeOffset? grp0, int grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double? Lag(double? value, DateTimeOffset? grp0, int grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double? Lead(double? value, DateTimeOffset? grp0, int grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double Lag(double value, DateTimeOffset grp0, int? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double Lead(double value, DateTimeOffset grp0, int? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double? Lag(double? value, DateTimeOffset grp0, int? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double? Lead(double? value, DateTimeOffset grp0, int? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double Lag(double value, DateTimeOffset? grp0, int? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double Lead(double value, DateTimeOffset? grp0, int? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Int32")]
    public static double? Lag(double? value, DateTimeOffset? grp0, int? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Int32")]
    public static double? Lead(double? value, DateTimeOffset? grp0, int? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double Lag(double value, int grp0, Guid grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double Lead(double value, int grp0, Guid grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double? Lag(double? value, int grp0, Guid grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double? Lead(double? value, int grp0, Guid grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double Lag(double value, int? grp0, Guid grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double Lead(double value, int? grp0, Guid grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double? Lag(double? value, int? grp0, Guid grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double? Lead(double? value, int? grp0, Guid grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double Lag(double value, int grp0, Guid? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double Lead(double value, int grp0, Guid? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double? Lag(double? value, int grp0, Guid? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double? Lead(double? value, int grp0, Guid? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double Lag(double value, int? grp0, Guid? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double Lead(double value, int? grp0, Guid? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_Int32_Guid")]
    public static double? Lag(double? value, int? grp0, Guid? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_Int32_Guid")]
    public static double? Lead(double? value, int? grp0, Guid? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double Lag(double value, DateTimeOffset grp0, Guid grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double Lead(double value, DateTimeOffset grp0, Guid grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double? Lag(double? value, DateTimeOffset grp0, Guid grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double? Lead(double? value, DateTimeOffset grp0, Guid grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double Lag(double value, DateTimeOffset? grp0, Guid grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double Lead(double value, DateTimeOffset? grp0, Guid grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double? Lag(double? value, DateTimeOffset? grp0, Guid grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double? Lead(double? value, DateTimeOffset? grp0, Guid grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double Lag(double value, DateTimeOffset grp0, Guid? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double Lead(double value, DateTimeOffset grp0, Guid? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double? Lag(double? value, DateTimeOffset grp0, Guid? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double? Lead(double? value, DateTimeOffset grp0, Guid? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double Lag(double value, DateTimeOffset? grp0, Guid? grp1) => 17.0;

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double Lead(double value, DateTimeOffset? grp0, Guid? grp1) => -17.0;

    [DbFunction("AX", "LAG_Double_DateTimeOffset_Guid")]
    public static double? Lag(double? value, DateTimeOffset? grp0, Guid? grp1) => new double?(17.0);

    [DbFunction("AX", "LEAD_Double_DateTimeOffset_Guid")]
    public static double? Lead(double? value, DateTimeOffset? grp0, Guid? grp1) => new double?(-17.0);

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_String")]
    public static DateTimeOffset Lag(DateTimeOffset value, int grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_String")]
    public static DateTimeOffset Lead(DateTimeOffset value, int grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_String")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_String")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_String")]
    public static DateTimeOffset Lag(DateTimeOffset value, int? grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_String")]
    public static DateTimeOffset Lead(DateTimeOffset value, int? grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_String")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int? grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_String")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int? grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset? grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset? grp0, string grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset? grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_String")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset? grp0, string grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, int grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, int grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, int? grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, int? grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int? grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int? grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, int grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, int grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, int? grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, int? grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int? grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int? grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset? grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset? grp0, int grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset? grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset? grp0, int grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset? grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset? grp0, int? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset? grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Int32")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset? grp0, int? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, int grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, int grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, int? grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, int? grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int? grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int? grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, int grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, int grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, int? grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, int? grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, int? grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_Int32_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, int? grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset? grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset? grp0, Guid grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset? grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset? grp0, Guid grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lag(DateTimeOffset value, DateTimeOffset? grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset Lead(DateTimeOffset value, DateTimeOffset? grp0, Guid? grp1) => new DateTimeOffset(new DateTime(2019, 4, 1));

    [DbFunction("AX", "LAG_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lag(DateTimeOffset? value, DateTimeOffset? grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "LEAD_DateTimeOffset_DateTimeOffset_Guid")]
    public static DateTimeOffset? Lead(DateTimeOffset? value, DateTimeOffset? grp0, Guid? grp1) => new DateTimeOffset?(new DateTimeOffset(new DateTime(2019, 4, 1)));

    [DbFunction("AX", "ROW_NUMBER_Int32_String")]
    public static long RowNumber(int value, string grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_String")]
    public static long RowNumber(int? value, string grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_String")]
    public static long RowNumber(DateTimeOffset value, string grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_String")]
    public static long RowNumber(DateTimeOffset? value, string grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Int32")]
    public static long RowNumber(int value, int grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Int32")]
    public static long RowNumber(int? value, int grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Int32")]
    public static long RowNumber(int value, int? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Int32")]
    public static long RowNumber(int? value, int? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Int32")]
    public static long RowNumber(DateTimeOffset value, int grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Int32")]
    public static long RowNumber(DateTimeOffset? value, int grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Int32")]
    public static long RowNumber(DateTimeOffset value, int? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Int32")]
    public static long RowNumber(DateTimeOffset? value, int? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Guid")]
    public static long RowNumber(int value, Guid grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Guid")]
    public static long RowNumber(int? value, Guid grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Guid")]
    public static long RowNumber(int value, Guid? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_Int32_Guid")]
    public static long RowNumber(int? value, Guid? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Guid")]
    public static long RowNumber(DateTimeOffset value, Guid grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Guid")]
    public static long RowNumber(DateTimeOffset? value, Guid grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Guid")]
    public static long RowNumber(DateTimeOffset value, Guid? grp0) => 1;

    [DbFunction("AX", "ROW_NUMBER_DateTimeOffset_Guid")]
    public static long RowNumber(DateTimeOffset? value, Guid? grp0) => 1;
  }
}
