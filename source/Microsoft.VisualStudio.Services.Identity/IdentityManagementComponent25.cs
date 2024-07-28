// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent25
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent25 : IdentityManagementComponent24
  {
    public override SqlParameter BindIdentityTableForUpdateOrInsert(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      HashSet<string> propertiesToUpdate)
    {
      return this.BindIdentityTable10(parameterName, identities, propertiesToUpdate, this.GetIdentityBindingConfig());
    }

    protected override IdentityManagementComponent20.IdentityColumns9 GetIdentityColumns() => (IdentityManagementComponent20.IdentityColumns9) this.GetIdentityColumns10();

    protected override IdentityManagementComponent20.IdentitiesColumns9 GetIdentitiesColumns() => (IdentityManagementComponent20.IdentitiesColumns9) new IdentityManagementComponent25.IdentitiesColumns10(this.GetIdentityColumns10());

    private IdentityManagementComponent25.IdentityColumns10 GetIdentityColumns10() => new IdentityManagementComponent25.IdentityColumns10(this.GetIdentityBindingConfig());

    public override int FireMajorDescriptorChangeNotification()
    {
      this.TraceEnter(47011202, nameof (FireMajorDescriptorChangeNotification));
      try
      {
        this.PrepareStoredProcedure("prc_FireMajorDescriptorChangeForIdentityChanges");
        return (int) this.ExecuteNonQuery(true);
      }
      finally
      {
        this.TraceLeave(47011206, nameof (FireMajorDescriptorChangeNotification));
      }
    }

    protected class IdentityColumns10 : IdentityManagementComponent20.IdentityColumns9
    {
      private SqlColumnBinder DirectoryAlias = new SqlColumnBinder(nameof (DirectoryAlias));

      public IdentityColumns10(IdentityBindingConfig bindingConfig)
        : base(bindingConfig)
      {
      }

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        this.BindDirectoryAlias(reader, identity);
        return identity;
      }

      protected virtual void BindDirectoryAlias(SqlDataReader reader, Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        if (!this.BindingConfig.DirectoryAliasFeatureEnabled)
          return;
        if (this.BindingConfig.UseAccountNameAsDirectoryAlias)
        {
          string property = identity.GetProperty<string>("Account", (string) null);
          if (string.IsNullOrEmpty(property))
            return;
          identity.SetProperty("DirectoryAlias", (object) property);
        }
        else
        {
          string str = this.DirectoryAlias.GetString((IDataReader) reader, (string) null);
          if (string.IsNullOrEmpty(str))
            return;
          identity.SetProperty("DirectoryAlias", (object) str);
        }
      }
    }

    protected class IdentitiesColumns10 : IdentityManagementComponent20.IdentitiesColumns9
    {
      public IdentitiesColumns10(
        IdentityManagementComponent25.IdentityColumns10 identityColumns)
        : base((IdentityManagementComponent20.IdentityColumns9) identityColumns)
      {
      }
    }
  }
}
