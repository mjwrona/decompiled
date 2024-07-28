// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent10
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent10 : DalSqlResourceComponent9
  {
    public virtual void SyncIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.PrepareStoredProcedure("prc_SyncIdentity");
      this.BindString("@domainName", identity.GetProperty<string>("Domain", string.Empty), 256, false, SqlDbType.NVarChar);
      this.BindString("@accountName", identity.GetProperty<string>("Account", string.Empty), 256, false, SqlDbType.NVarChar);
      this.BindString("@displayName", identity.DisplayName, 256, false, SqlDbType.NVarChar);
      this.BindString("@sid", identity.Descriptor.Identifier, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@tfId", identity.Id);
      this.BindBoolean("@isContainer", identity.IsContainer);
      this.BindStringTable("@directGroups", (IEnumerable<string>) ((IEnumerable<IdentityDescriptor>) identity.MemberOf ?? Enumerable.Empty<IdentityDescriptor>()).Select<IdentityDescriptor, string>((System.Func<IdentityDescriptor, string>) (x => x.Identifier)).ToArray<string>());
      this.BindIdentityCategory(identity);
      this.BindCollectionAndAccountHostIds();
      this.ExecuteNonQuery();
    }
  }
}
