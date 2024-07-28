// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupMembershipTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class GroupMembershipTableValuedParameterExtensions
  {
    private const string area = "Identity";
    private const string layer = "GroupMembershipTable";
    private static readonly SqlMetaData[] typ_GroupMembershipTable = new SqlMetaData[4]
    {
      new SqlMetaData("GroupSid", SqlDbType.VarChar, 256L),
      new SqlMetaData("GroupType", SqlDbType.VarChar, 64L),
      new SqlMetaData("MemberId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Active", SqlDbType.Bit)
    };

    public static SqlParameter BindGroupMembershipTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> rows)
    {
      return component.Bind<Tuple<IdentityDescriptor, Guid, bool>>(parameterName, rows, GroupMembershipTableValuedParameterExtensions.typ_GroupMembershipTable, "typ_GroupMembershipTable", (Action<SqlDataRecord, Tuple<IdentityDescriptor, Guid, bool>>) ((record, value) =>
      {
        record.SetString(0, value.Item1.Identifier);
        record.SetString(1, value.Item1.IdentityType);
        if (value.Item2 == Guid.Empty)
        {
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "Identity", "GroupMembershipTable", "An identity with no Id is being added to group Sid={0} and Type={1}.", (object) value.Item1.Identifier, (object) value.Item1.IdentityType);
          throw new ArgumentNullException(FrameworkResources.GroupMemberWithNoId((object) value.Item1.Identifier, (object) value.Item1.IdentityType));
        }
        record.SetGuid(2, value.Item2);
        record.SetBoolean(3, value.Item3);
      }));
    }
  }
}
