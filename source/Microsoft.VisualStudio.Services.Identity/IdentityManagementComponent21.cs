// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent21
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent21 : IdentityManagementComponent20
  {
    public override bool DeleteIdentities(IList<Guid> identityIds, bool sleepIfBusy = false)
    {
      this.TraceEnter(47011170, nameof (DeleteIdentities));
      try
      {
        this.PrepareStoredProcedure("prc_DeleteIdentities");
        this.BindGuidTable("@ids", (IEnumerable<Guid>) identityIds);
        this.BindBoolean("@sleepIfBusy", sleepIfBusy);
        this.ExecuteNonQuery();
        return true;
      }
      finally
      {
        this.TraceLeave(47011179, nameof (DeleteIdentities));
      }
    }

    public override bool DeleteHistoricalIdentities(IList<Guid> identityIds)
    {
      this.TraceEnter(47011180, nameof (DeleteHistoricalIdentities));
      try
      {
        this.PrepareStoredProcedure("prc_DeleteHistoricalIdentities");
        this.BindGuidTable("@identityIds", (IEnumerable<Guid>) identityIds);
        this.BindGuid("@eventAuthor", this.Author);
        this.ExecuteNonQuery();
        return true;
      }
      finally
      {
        this.TraceLeave(47011189, nameof (DeleteHistoricalIdentities));
      }
    }

    protected override void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates)
    {
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter> binder = (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter>) ((updateList, allowMetadataUpdate) => this.BindIdentityExtensionTableForIdentityComponent21OrLater("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, allowMetadataUpdates));
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, binder);
    }
  }
}
