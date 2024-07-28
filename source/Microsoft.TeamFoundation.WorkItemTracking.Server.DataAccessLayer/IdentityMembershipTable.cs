// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IdentityMembershipTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class IdentityMembershipTable : WorkItemTrackingTableValueParameter<string>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
    {
      new SqlMetaData("sid", SqlDbType.VarChar, 256L)
    };

    public IdentityMembershipTable(IEnumerable<string> members)
      : base(members, "typ_WitIdentityMembershipTable", IdentityMembershipTable.s_metadata)
    {
    }

    public override void SetRecord(string sid, SqlDataRecord record) => record.SetString(0, sid);
  }
}
