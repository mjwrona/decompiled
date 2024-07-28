// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent17
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent17 : IdentityManagementComponent16
  {
    public override List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      try
      {
        this.TraceEnter(47011120, nameof (UpdateIdentities));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
        {
          this.ThrowIfIdentityTypeInvalid(update);
          this.PrepareIdentityForUpdate(update);
        }
        this.PrepareUpdateIdentitiesStoredProcedure(updates);
        this.BindIdentityTable6("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
        this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates);
        return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out deletedIds, out identityChangedData, out identitiesToTransfer);
      }
      finally
      {
        this.TraceLeave(47011129, nameof (UpdateIdentities));
      }
    }

    protected virtual void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates)
    {
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit);
      this.BindBoolean("@allowMetadataUpdates", allowMetadataUpdates);
    }
  }
}
