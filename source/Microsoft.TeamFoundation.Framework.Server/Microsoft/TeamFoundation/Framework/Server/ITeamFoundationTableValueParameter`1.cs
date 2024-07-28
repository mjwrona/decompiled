// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationTableValueParameter`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITeamFoundationTableValueParameter<T> : 
    IEnumerable<SqlDataRecord>,
    IEnumerable,
    IEnumerator<SqlDataRecord>,
    IDisposable,
    IEnumerator
  {
    bool IsNullOrEmpty { get; }

    string TypeName { get; }

    void SetRecord(T t, SqlDataRecord record);

    void SetString(
      SqlDataRecord record,
      int ordinal,
      string value,
      BindStringBehavior bindBehavior);

    void SetNullableString(SqlDataRecord record, int ordinal, string value);

    void SetNullableStringAsEmpty(SqlDataRecord record, int ordinal, string value);

    void SetNullableGuid(SqlDataRecord record, int ordinal, Guid value);

    void SetNullableDateTime(SqlDataRecord record, int ordinal, DateTime value);

    void SetNullableInt32(SqlDataRecord record, int ordinal, int? value);

    void SetNullableBinary(SqlDataRecord record, int ordinal, byte[] value);

    void SetTimespanAsMicroseconds(SqlDataRecord record, int ordinal, TimeSpan value);
  }
}
