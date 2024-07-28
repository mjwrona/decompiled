// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent35
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
  internal class IdentityManagementComponent35 : IdentityManagementComponent34
  {
    protected override IdentityManagementComponent20.IdentityColumns9 GetIdentityColumns() => (IdentityManagementComponent20.IdentityColumns9) new IdentityManagementComponent35.IdentityColumns13(this.GetIdentityBindingConfig());

    protected override IdentityManagementComponent20.IdentitiesColumns9 GetIdentitiesColumns() => (IdentityManagementComponent20.IdentitiesColumns9) new IdentityManagementComponent35.IdentitiesColumns13(new IdentityManagementComponent35.IdentityColumns13(this.GetIdentityBindingConfig()));

    protected override void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates)
    {
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter> binder = (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter>) ((updateList, allowMetadataUpdate) => this.BindIdentityExtensionTableForIdentityComponent35OrLater("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, allowMetadataUpdates));
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, binder);
      this.BindBoolean("@resolveByOid", this.RequestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(PlatformIdentityStore.ResolveByOid));
    }

    protected class IdentityColumns13 : IdentityManagementComponent32.IdentityColumns12
    {
      private SqlColumnBinder ApplicationId = new SqlColumnBinder(nameof (ApplicationId));

      public IdentityColumns13(IdentityBindingConfig bindingConfig)
        : base(bindingConfig)
      {
      }

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        Guid guid = this.ApplicationId.GetGuid((IDataReader) reader, true, Guid.Empty);
        if (guid != Guid.Empty)
          identity.SetProperty("ApplicationId", (object) guid.ToString());
        return identity;
      }
    }

    protected class IdentitiesColumns13 : IdentityManagementComponent32.IdentitiesColumns12
    {
      public IdentitiesColumns13(
        IdentityManagementComponent35.IdentityColumns13 identityColumns)
        : base((IdentityManagementComponent32.IdentityColumns12) identityColumns)
      {
      }
    }
  }
}
