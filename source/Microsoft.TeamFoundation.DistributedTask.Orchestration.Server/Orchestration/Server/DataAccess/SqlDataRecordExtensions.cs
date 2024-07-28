// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.SqlDataRecordExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
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
