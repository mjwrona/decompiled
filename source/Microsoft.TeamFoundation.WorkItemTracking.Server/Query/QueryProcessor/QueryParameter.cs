// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor.QueryParameter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Query.QueryProcessor
{
  internal static class QueryParameter
  {
    public static string DefineParameter(
      QueryProcessorContext context,
      QueryExpressionValue value,
      bool parameterizeString = false)
    {
      switch (value.ValueType)
      {
        case QueryExpressionValueType.Array:
          return QueryParameter.DefineTableValuedParameter(context, value);
        case QueryExpressionValueType.String:
          return !parameterizeString ? QueryProcessorCommon.EncodeSqlString(value.StringValue) : QueryParameter.DefineParameter(context, (object) value.StringValue);
        case QueryExpressionValueType.Number:
          return value.NumberValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        case QueryExpressionValueType.DateTime:
          return QueryParameter.DefineParameter(context, (object) value.DateValue);
        case QueryExpressionValueType.UniqueIdentifier:
          return "'" + value.GuidValue.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) + "'";
        case QueryExpressionValueType.Boolean:
          return !value.BoolValue ? "0" : "1";
        case QueryExpressionValueType.Double:
          return value.DoubleValue.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        default:
          throw new NotSupportedException();
      }
    }

    public static string DefineTableValuedParameter(
      QueryProcessorContext context,
      QueryExpressionValue value)
    {
      QueryExpressionValue[] arrayValue = value.GetArrayValue();
      switch (((IEnumerable<QueryExpressionValue>) arrayValue).First<QueryExpressionValue>().ValueType)
      {
        case QueryExpressionValueType.String:
          return QueryParameter.DefineTableValuedParameter<string>(context, (WorkItemTrackingTableValueParameter<string>) new StringTable((IEnumerable<string>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, string>((System.Func<QueryExpressionValue, string>) (x => x.StringValue)).ToArray<string>()));
        case QueryExpressionValueType.Number:
          return QueryParameter.DefineTableValuedParameter<int>(context, (WorkItemTrackingTableValueParameter<int>) new Int32Table((IEnumerable<int>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, int>((System.Func<QueryExpressionValue, int>) (x => x.NumberValue)).ToArray<int>()));
        case QueryExpressionValueType.DateTime:
          return QueryParameter.DefineTableValuedParameter<DateTime>(context, (WorkItemTrackingTableValueParameter<DateTime>) new DateTimeTable((IEnumerable<DateTime>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, DateTime>((System.Func<QueryExpressionValue, DateTime>) (x => x.DateValue)).ToArray<DateTime>()));
        case QueryExpressionValueType.UniqueIdentifier:
          return QueryParameter.DefineTableValuedParameter<Guid>(context, (WorkItemTrackingTableValueParameter<Guid>) new GuidTable((IEnumerable<Guid>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, Guid>((System.Func<QueryExpressionValue, Guid>) (x => x.GuidValue)).ToArray<Guid>()));
        case QueryExpressionValueType.Boolean:
          return QueryParameter.DefineTableValuedParameter<bool>(context, (WorkItemTrackingTableValueParameter<bool>) new BooleanTable((IEnumerable<bool>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, bool>((System.Func<QueryExpressionValue, bool>) (x => x.BoolValue)).ToArray<bool>()));
        case QueryExpressionValueType.Double:
          return QueryParameter.DefineTableValuedParameter<double>(context, (WorkItemTrackingTableValueParameter<double>) new DoubleTable((IEnumerable<double>) ((IEnumerable<QueryExpressionValue>) arrayValue).Where<QueryExpressionValue>((System.Func<QueryExpressionValue, bool>) (x => !x.IsNull)).Select<QueryExpressionValue, double>((System.Func<QueryExpressionValue, double>) (x => x.DoubleValue)).ToArray<double>()));
        default:
          throw new NotSupportedException();
      }
    }

    public static string DefineTableValuedParameter<T>(
      QueryProcessorContext context,
      WorkItemTrackingTableValueParameter<T> tvp)
    {
      string parameterName = "@p" + context.m_parameters.Count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      object obj = (object) null;
      if (tvp != null && !tvp.IsNullOrEmpty)
        obj = (object) tvp;
      context.m_parameters.Add(new SqlParameter(parameterName, obj)
      {
        TypeName = tvp.TypeName,
        SqlDbType = SqlDbType.Structured
      });
      return parameterName;
    }

    public static string DefineParameter(QueryProcessorContext context, object value)
    {
      string str;
      if (context.m_paramNames.TryGetValue(value, out str))
        return str;
      string parameterName = "@p" + context.m_parameters.Count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      context.m_paramNames[value] = parameterName;
      SqlParameter sqlParameter = new SqlParameter(parameterName, value);
      context.m_parameters.Add(sqlParameter);
      switch (value)
      {
        case string _:
          sqlParameter.SqlDbType = SqlDbType.NVarChar;
          if (((string) value).Length <= 4000)
          {
            sqlParameter.Size = 4000;
            break;
          }
          break;
        case int _:
          sqlParameter.SqlDbType = SqlDbType.Int;
          break;
        case double _:
          sqlParameter.SqlDbType = SqlDbType.Float;
          break;
        case DateTime _:
          sqlParameter.SqlDbType = SqlDbType.DateTime;
          break;
        case bool _:
          sqlParameter.SqlDbType = SqlDbType.Bit;
          break;
        case Guid _:
          sqlParameter.SqlDbType = SqlDbType.UniqueIdentifier;
          break;
        default:
          throw new NotSupportedException();
      }
      return parameterName;
    }
  }
}
