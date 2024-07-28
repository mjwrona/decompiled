// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SqlDataRecordExtensions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.SqlServer.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class SqlDataRecordExtensions
  {
    public static void SetStringPreserveNull(this SqlDataRecord record, int ordinal, string data)
    {
      if (data == null)
        record.SetDBNull(ordinal);
      else
        record.SetString(ordinal, data);
    }

    public static void SetDateTimePreserveNull(
      this SqlDataRecord record,
      int ordinal,
      DateTime data)
    {
      if (data == DateTime.MinValue)
        record.SetDBNull(ordinal);
      else
        record.SetDateTime(ordinal, data.ToUniversalTime());
    }

    public static void SetGuidPreserveNull(this SqlDataRecord record, int ordinal, Guid data)
    {
      if (data == Guid.Empty)
        record.SetDBNull(ordinal);
      else
        record.SetGuid(ordinal, data);
    }

    public static void SetGuid(
      this SqlDataRecord record,
      int ordinal,
      Guid data,
      bool preserveNull)
    {
      if (preserveNull && data == Guid.Empty)
        record.SetDBNull(ordinal);
      else
        record.SetGuid(ordinal, data);
    }

    public static void SetBytePreserveNull(
      this SqlDataRecord record,
      int ordinal,
      byte data,
      byte nullValue)
    {
      if ((int) data == (int) nullValue)
        record.SetDBNull(ordinal);
      else
        record.SetByte(ordinal, data);
    }

    public static void SetIntPreserveNull(
      this SqlDataRecord record,
      int ordinal,
      int data,
      int nullValue)
    {
      if (data == nullValue)
        record.SetDBNull(ordinal);
      else
        record.SetInt32(ordinal, data);
    }

    public static void SetLongPreserveNull(
      this SqlDataRecord record,
      int ordinal,
      long data,
      long nullValue)
    {
      if (data == nullValue)
        record.SetDBNull(ordinal);
      else
        record.SetInt64(ordinal, data);
    }

    public static void SetBytesPreserveNull(this SqlDataRecord record, int ordinal, byte[] data)
    {
      if (data == null)
        record.SetDBNull(ordinal);
      else
        record.SetBytes(ordinal, 0L, data, 0, data.Length);
    }

    public static void SetNullableDouble(this SqlDataRecord record, int ordinal, double? data)
    {
      if (data.HasValue)
        record.SetDouble(ordinal, data.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableBoolean(this SqlDataRecord record, int ordinal, bool? data)
    {
      if (data.HasValue)
        record.SetBoolean(ordinal, data.Value);
      else
        record.SetDBNull(ordinal);
    }
  }
}
