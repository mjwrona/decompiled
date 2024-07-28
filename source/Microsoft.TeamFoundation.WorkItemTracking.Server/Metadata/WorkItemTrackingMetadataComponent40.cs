// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent40
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent40 : WorkItemTrackingMetadataComponent39
  {
    public override void SyncIdentities(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.PrepareStoredProcedure("prc_SyncIdentities");
      this.BindCollectionAndAccountHostIds();
      this.BindWitSyncIdentityTable("@identities", (IEnumerable<IVssIdentity>) identities);
      this.BindWitSyncMembershipTable("@memberships", (IEnumerable<(Guid, string)>) identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.MemberOf != null)).SelectMany<Microsoft.VisualStudio.Services.Identity.Identity, (Guid, string)>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IEnumerable<(Guid, string)>>) (i => i.MemberOf.Select<IdentityDescriptor, (Guid, string)>((Func<IdentityDescriptor, (Guid, string)>) (x => (i.Id, x.Identifier))))).ToArray<(Guid, string)>());
      this.BindInt("@boxcarExtensionId", -1);
      this.ExecuteNonQuery();
    }
  }
}
