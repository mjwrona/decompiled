// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.SqlDataRecordExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SqlDataRecordExtensions
  {
    public static void SetNullableByte(this SqlDataRecord record, int ordinal, byte? value)
    {
      if (value.HasValue)
        record.SetByte(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableGuid(this SqlDataRecord record, int ordinal, Guid? value)
    {
      if (value.HasValue)
        record.SetGuid(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableInt16(this SqlDataRecord record, int ordinal, short? value)
    {
      if (value.HasValue)
        record.SetInt16(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
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

    public static void SetNullableDateTime(this SqlDataRecord record, int ordinal, DateTime? value)
    {
      if (value.HasValue)
        record.SetDateTime(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }
  }
}
