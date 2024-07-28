// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityQueueTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityQueueTableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_IdentityQueueTable = new SqlMetaData[3]
    {
      new SqlMetaData("IdentityId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Recursive", SqlDbType.Bit),
      new SqlMetaData("Recursive", SqlDbType.Bit)
    };

    public static SqlParameter BindIdentityQueueTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<Guid, bool, bool>> rows)
    {
      return component.Bind<Tuple<Guid, bool, bool>>(parameterName, rows, IdentityQueueTableValuedParameterExtensions.typ_IdentityQueueTable, "typ_IdentityQueueTable", (Action<SqlDataRecord, Tuple<Guid, bool, bool>>) ((record, value) =>
      {
        record.SetGuid(0, value.Item1);
        record.SetBoolean(1, value.Item2);
        record.SetBoolean(2, value.Item3);
      }));
    }
  }
}
