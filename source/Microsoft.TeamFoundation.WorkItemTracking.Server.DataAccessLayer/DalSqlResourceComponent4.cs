// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent4 : DalSqlResourceComponent3
  {
    public override void DestroyAttachments(
      IVssIdentity userIdentity,
      IEnumerable<int> workItemIds,
      string comment,
      bool dualSave)
    {
      this.PrepareStoredProcedure(nameof (DestroyAttachments), 3600);
      this.BindIdentityColumn(userIdentity);
      this.BindInt32Table("@workItemIds", workItemIds);
      this.BindString("@comment", comment, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
