// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent6 : WorkItemComponent5
  {
    public override IEnumerable<int> DestroyWorkItems(
      IVssIdentity userIdentity,
      IEnumerable<int> workItemIds,
      int batchSize = 200)
    {
      this.PrepareStoredProcedure("prc_DestroyWorkItems", 3600);
      this.BindIdentityColumn(userIdentity, "@userSid");
      this.BindInt32Table("@workItemIds", workItemIds);
      return (IEnumerable<int>) WorkItemTrackingResourceComponent.Bind<int>(this.ExecuteReader(), (System.Func<IDataReader, int>) (reader => reader.GetInt32(0))).ToArray<int>();
    }
  }
}
