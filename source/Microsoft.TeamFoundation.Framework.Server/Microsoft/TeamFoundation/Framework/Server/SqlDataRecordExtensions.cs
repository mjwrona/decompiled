// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlDataRecordExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlDataRecordExtensions
  {
    private const long c_ticksPerMicrosecond = 10;

    public static void SetString(
      this SqlDataRecord record,
      int ordinal,
      string value,
      BindStringBehavior bindBehavior)
    {
      if (!string.IsNullOrEmpty(value))
        record.SetString(ordinal, value);
      else if (bindBehavior == BindStringBehavior.NullToEmptyString)
        record.SetString(ordinal, string.Empty);
      else if (bindBehavior == BindStringBehavior.EmptyStringToNull)
        record.SetDBNull(ordinal);
      else if (value == null)
        record.SetDBNull(ordinal);
      else
        record.SetString(ordinal, string.Empty);
    }

    public static void SetNullableString(this SqlDataRecord record, int ordinal, string value)
    {
      if (value != null)
        record.SetString(ordinal, value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableStringAsEmpty(
      this SqlDataRecord record,
      int ordinal,
      string value)
    {
      if (value == null)
        value = string.Empty;
      record.SetNullableString(ordinal, value);
    }

    public static void SetNullableGuid(this SqlDataRecord record, int ordinal, Guid value)
    {
      if (value == Guid.Empty)
        record.SetDBNull(ordinal);
      else
        record.SetGuid(ordinal, value);
    }

    public static void SetNullableGuid(this SqlDataRecord record, int ordinal, Guid? value)
    {
      if (!value.HasValue)
        record.SetDBNull(ordinal);
      else
        record.SetGuid(ordinal, value.Value);
    }

    public static void SetNullableDateTime(this SqlDataRecord record, int ordinal, DateTime value)
    {
      if (value == DateTime.MinValue)
        record.SetDBNull(ordinal);
      else
        record.SetDateTime(ordinal, value);
    }

    public static void SetNullableDateTime(this SqlDataRecord record, int ordinal, DateTime? value)
    {
      if (!value.HasValue)
        record.SetDBNull(ordinal);
      else
        record.SetDateTime(ordinal, value.Value);
    }

    public static void SetNullableInt32(this SqlDataRecord record, int ordinal, int? value)
    {
      if (value.HasValue)
        record.SetInt32(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableInt64(this SqlDataRecord record, int ordinal, long? value)
    {
      if (value.HasValue)
        record.SetInt64(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableByte(this SqlDataRecord record, int ordinal, byte? value)
    {
      if (value.HasValue)
        record.SetByte(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableBinary(this SqlDataRecord record, int ordinal, byte[] value)
    {
      if (value == null)
        record.SetDBNull(ordinal);
      else
        record.SetBytes(ordinal, 0L, value, 0, value.Length);
    }

    public static void SetTimespanAsMicroseconds(
      this SqlDataRecord record,
      int ordinal,
      TimeSpan value)
    {
      record.SetInt64(ordinal, value.Ticks / 10L);
    }

    public static string SerializeClrAndSqlFieldValues(
      this SqlDataRecord record,
      Func<object, object> scrub,
      bool minimize = false)
    {
      if (record == null)
        return "null";
      string[] strArray = new string[record.FieldCount];
      for (int ordinal = 0; ordinal < record.FieldCount; ++ordinal)
        strArray[ordinal] = record.SafeSerialize(ordinal, scrub, minimize);
      return "[" + string.Join(",", strArray) + "]";
    }

    private static string SafeSerialize(
      this SqlDataRecord record,
      int ordinal,
      Func<object, object> scrub,
      bool minimize = false)
    {
      return "{\"clr\":" + record.SafeSerialize((Func<SqlDataRecord, object>) (r => scrub(r.GetValue(ordinal))), minimize) + ",\"sql\":" + record.SafeSerialize((Func<SqlDataRecord, object>) (r => scrub(r.GetSqlValue(ordinal))), minimize) + "}";
    }

    private static string SafeSerialize(
      this SqlDataRecord record,
      Func<SqlDataRecord, object> getValue,
      bool minimize = false)
    {
      object obj = (object) null;
      IList<Exception> exceptions = (IList<Exception>) null;
      try
      {
        obj = getValue(record);
        if (obj == null || obj is INullable nullable && nullable.IsNull)
          return "null";
        return minimize ? obj.ToString().Serialize<string>() : JsonUtilities.Serialize(obj);
      }
      catch (Exception ex)
      {
        SqlDataRecordExtensions.EnsureInitialized(ref exceptions).Add(ex);
      }
      if (obj == null | minimize)
        return SqlDataRecordExtensions.SerializeWithExceptions(exceptions);
      string str = (string) null;
      try
      {
        str = obj.ToString().Serialize<string>();
      }
      catch (Exception ex)
      {
        SqlDataRecordExtensions.EnsureInitialized(ref exceptions).Add(ex);
      }
      return SqlDataRecordExtensions.SerializeWithExceptions(exceptions, str);
    }

    private static IList<Exception> EnsureInitialized(ref IList<Exception> exceptions)
    {
      if (exceptions == null)
        exceptions = (IList<Exception>) new List<Exception>();
      return exceptions;
    }

    private static string SerializeWithExceptions(IList<Exception> exceptions, string value = null)
    {
      if (exceptions == null || exceptions.Count == 0)
        return value ?? "null";
      if (value == null)
        return "{\"exceptions\":[" + SqlDataRecordExtensions.SerializeExceptions(exceptions) + "]}";
      return "{\"value\":" + value + ",\"exceptions\":[" + SqlDataRecordExtensions.SerializeExceptions(exceptions) + "]}";
    }

    private static string SerializeExceptions(IList<Exception> exceptions) => string.Join(",", exceptions.Select<Exception, string>((Func<Exception, string>) (e => SqlDataRecordExtensions.SerializeException(e))));

    private static string SerializeException(Exception e)
    {
      if (e == null)
        return "null";
      return "{\"type\":\"" + e.GetType().FullName + "\",\"message\":\"" + e.Message + "\"}";
    }
  }
}
