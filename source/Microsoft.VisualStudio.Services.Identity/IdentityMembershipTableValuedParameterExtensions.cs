// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMembershipTableValuedParameterExtensions
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
  internal static class IdentityMembershipTableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_IdentityMembershipTable = new SqlMetaData[2]
    {
      new SqlMetaData("ContainerSid", SqlDbType.VarChar, 256L),
      new SqlMetaData("MemberSid", SqlDbType.VarChar, 256L)
    };

    public static SqlParameter BindIdentityMembershipTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<string, string>> rows)
    {
      return component.Bind<Tuple<string, string>>(parameterName, rows, IdentityMembershipTableValuedParameterExtensions.typ_IdentityMembershipTable, "typ_IdentityMembershipTable", (Action<SqlDataRecord, Tuple<string, string>>) ((record, value) =>
      {
        record.SetString(0, value.Item1);
        record.SetString(1, value.Item2);
      }));
    }
  }
}
