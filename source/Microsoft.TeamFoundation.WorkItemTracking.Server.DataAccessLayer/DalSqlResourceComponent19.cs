// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent19
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent19 : DalSqlResourceComponent18
  {
    public override void ResyncIdentities1(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<SyncBase.Membership> members,
      Tuple<int, int> seqId)
    {
      this.PrepareStoredProcedure(nameof (ResyncIdentities1), 3600);
      this.BindSyncIdentityTable("@identities", identities);
      this.BindSyncMembershipTable("@memberships", members);
      this.BindInt("@seqId", seqId.Item1);
      this.BindInt("@seqId2", seqId.Item2);
      this.ExecuteNonQuery();
    }
  }
}
