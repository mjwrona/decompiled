// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent30
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent30 : IdentityManagementComponent29
  {
    protected override IdentityManagementComponent20.IdentityColumns9 GetIdentityColumns() => (IdentityManagementComponent20.IdentityColumns9) new IdentityManagementComponent30.IdentityColumns11(this.GetIdentityBindingConfig());

    protected override IdentityManagementComponent20.IdentitiesColumns9 GetIdentitiesColumns() => (IdentityManagementComponent20.IdentitiesColumns9) new IdentityManagementComponent25.IdentitiesColumns10((IdentityManagementComponent25.IdentityColumns10) new IdentityManagementComponent30.IdentityColumns11(this.GetIdentityBindingConfig()));

    protected override void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates)
    {
      base.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates);
      this.BindBoolean("@resolveByOid", this.RequestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(PlatformIdentityStore.ResolveByOid));
    }

    public override bool SwapIdentity(string domain, string accountName, Guid id1, Guid id2)
    {
      this.PrepareStoredProcedure("prc_SwapIdentity");
      this.BindString("@domain", domain, 256, false, SqlDbType.NVarChar);
      this.BindString("@accountName", accountName, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@id1", id1);
      this.BindGuid("@id2", id2);
      this.BindGuid("@eventAuthor", this.Author);
      this.ExecuteNonQuery();
      return true;
    }

    protected class IdentityColumns11 : IdentityManagementComponent25.IdentityColumns10
    {
      private SqlColumnBinder DisambiguationDate = new SqlColumnBinder(nameof (DisambiguationDate));

      public IdentityColumns11(IdentityBindingConfig bindingConfig)
        : base(bindingConfig)
      {
      }

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        if (this.DisambiguationDate.GetDateTime((IDataReader) reader, DateTime.MinValue) > DateTime.MinValue)
          identity.SetProperty("IsDeletedInOrigin", (object) true);
        return identity;
      }
    }
  }
}
