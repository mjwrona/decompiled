// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupUpdateTableValuedParameterExtensions
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
  internal static class GroupUpdateTableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_GroupUpdateTable = new SqlMetaData[3]
    {
      new SqlMetaData("GroupSid", SqlDbType.VarChar, 256L),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 1024L)
    };

    public static SqlParameter BindGroupUpdateTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<GroupComponent.GroupUpdate> rows)
    {
      return component.Bind<GroupComponent.GroupUpdate>(parameterName, rows, GroupUpdateTableValuedParameterExtensions.typ_GroupUpdateTable, "typ_GroupUpdateTable", (Action<SqlDataRecord, GroupComponent.GroupUpdate>) ((record, groupUpdate) =>
      {
        record.SetString(0, groupUpdate.GroupSid);
        record.SetString(1, groupUpdate.Name, BindStringBehavior.Unchanged);
        record.SetString(2, groupUpdate.Description, BindStringBehavior.Unchanged);
      }));
    }
  }
}
