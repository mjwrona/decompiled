// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent32
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent32 : IdentityManagementComponent31
  {
    public override SqlParameter BindIdentityTableForUpdateOrInsert(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      HashSet<string> propertiesToUpdate)
    {
      return this.BindIdentityTable12(parameterName, identities, propertiesToUpdate, this.GetIdentityBindingConfig());
    }

    protected override IdentityManagementComponent20.IdentityColumns9 GetIdentityColumns() => (IdentityManagementComponent20.IdentityColumns9) new IdentityManagementComponent32.IdentityColumns12(this.GetIdentityBindingConfig());

    protected override IdentityManagementComponent20.IdentitiesColumns9 GetIdentitiesColumns() => (IdentityManagementComponent20.IdentitiesColumns9) new IdentityManagementComponent32.IdentitiesColumns12(new IdentityManagementComponent32.IdentityColumns12(this.GetIdentityBindingConfig()));

    public override Microsoft.VisualStudio.Services.Identity.Identity ReadSocialIdentity(
      SocialDescriptor socialDescriptor)
    {
      try
      {
        this.TraceEnter(4701010, nameof (ReadSocialIdentity));
        this.PrepareStoredProcedure("prc_ReadSocialIdentity");
        this.BindByte("@socialType", SocialTypeMapper.Instance.GetSocialIdFromName(socialDescriptor.SocialType));
        this.BindString("@socialIdentifier", socialDescriptor.Identifier, 256, false, SqlDbType.NVarChar);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentityColumns());
        List<Microsoft.VisualStudio.Services.Identity.Identity> items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
        // ISSUE: explicit non-virtual call
        return items == null || __nonvirtual (items.Count) <= 0 ? (Microsoft.VisualStudio.Services.Identity.Identity) null : items[0];
      }
      finally
      {
        this.TraceLeave(4701019, nameof (ReadSocialIdentity));
      }
    }

    protected class IdentityColumns12 : IdentityManagementComponent30.IdentityColumns11
    {
      private SqlColumnBinder SocialIdentifier { get; }

      private SqlColumnBinder SocialType { get; }

      public IdentityColumns12(IdentityBindingConfig bindingConfig)
        : base(bindingConfig)
      {
        this.SocialIdentifier = new SqlColumnBinder(nameof (SocialIdentifier));
        this.SocialType = new SqlColumnBinder(nameof (SocialType));
      }

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        this.BindSocialIdentifier(reader, identity);
        return identity;
      }

      protected virtual void BindSocialIdentifier(SqlDataReader reader, Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        if (!this.BindingConfig.SocialIdentifierEnabled)
          return;
        SqlColumnBinder sqlColumnBinder = this.SocialIdentifier;
        string identifier = sqlColumnBinder.GetString((IDataReader) reader, (string) null);
        SocialTypeMapper instance = SocialTypeMapper.Instance;
        sqlColumnBinder = this.SocialType;
        int typeId = (int) sqlColumnBinder.GetByte((IDataReader) reader, byte.MaxValue);
        string socialNameFromId = instance.GetSocialNameFromId((byte) typeId);
        if (socialNameFromId == "ukn" || string.IsNullOrEmpty(identifier))
          identity.SocialDescriptor = new SocialDescriptor();
        else
          identity.SocialDescriptor = new SocialDescriptor(socialNameFromId, identifier);
      }
    }

    protected class IdentitiesColumns12 : IdentityManagementComponent25.IdentitiesColumns10
    {
      public IdentitiesColumns12(
        IdentityManagementComponent32.IdentityColumns12 identityColumns)
        : base((IdentityManagementComponent25.IdentityColumns10) identityColumns)
      {
      }
    }
  }
}
