// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent20
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent20 : IdentityManagementComponent19
  {
    public override SqlParameter BindIdentityTableForUpdateOrInsert(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      HashSet<string> propertiesToUpdate)
    {
      return this.BindIdentityTable9(parameterName, identities, propertiesToUpdate);
    }

    public override ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      try
      {
        this.TraceEnter(47011100, nameof (ReadIdentities));
        Guid? nullable1 = new Guid?();
        IdentityDescriptor identityDescriptor;
        if (descriptors != null && descriptors.Count<IdentityDescriptor>() == 1 && (identityDescriptor = descriptors.FirstOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null)
        {
          this.PrepareStoredProcedure("prc_ReadIdentityByIdentifier");
          this.BindString("@sid", identityDescriptor.Identifier, 256, true, SqlDbType.VarChar);
          this.BindByte("@typeId", IdentityTypeMapper.Instance.GetTypeIdFromName(identityDescriptor.IdentityType));
        }
        else
        {
          if (ids != null && ids.Count<Guid>() == 1)
          {
            nullable1 = new Guid?(ids.FirstOrDefault<Guid>());
            if (nullable1.HasValue && nullable1.HasValue)
            {
              Guid? nullable2 = nullable1;
              Guid empty = Guid.Empty;
              if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
              {
                this.PrepareStoredProcedure("prc_ReadIdentityByIdentifier");
                this.BindGuid("@id", nullable1.Value);
                goto label_7;
              }
            }
          }
          this.PrepareStoredProcedure("prc_ReadIdentities");
          this.BindOrderedDescriptorTable2ForIdentityComponent16OrLater("@descriptors", descriptors, true);
          this.BindOrderedGuidTable("@ids", ids, true);
        }
label_7:
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) this.GetIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(47011109, nameof (ReadIdentities));
      }
    }

    public override ResultCollection ReadIdentity(
      IdentitySearchFilter searchFactor,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      bool? isGroup)
    {
      try
      {
        this.TraceEnter(47011110, nameof (ReadIdentity));
        this.PrepareStoredProcedure("prc_ReadIdentity");
        this.BindInt("@searchFactor", (int) searchFactor);
        this.BindString("@factorValue", factorValue, 515, false, SqlDbType.NVarChar);
        this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
        this.BindString("@account", account, 256, true, SqlDbType.NVarChar);
        this.BindInt("@uniqueUserId", uniqueUserId);
        if (isGroup.HasValue)
          this.BindBoolean("@isGroup", isGroup.Value);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentityColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(47011119, nameof (ReadIdentity));
      }
    }

    protected virtual IdentityManagementComponent20.IdentityColumns9 GetIdentityColumns() => new IdentityManagementComponent20.IdentityColumns9(this.GetIdentityBindingConfig());

    protected virtual IdentityManagementComponent20.IdentitiesColumns9 GetIdentitiesColumns() => new IdentityManagementComponent20.IdentitiesColumns9(this.GetIdentityBindingConfig());

    protected IdentityBindingConfig GetIdentityBindingConfig()
    {
      if (this.RequestContext?.RootContext?.Items == null)
        return new IdentityBindingConfig(false, -1, true, false, true);
      int castedValueOrDefault = this.RequestContext.RootContext.Items.GetCastedValueOrDefault<string, int>("IdentityMinimumResourceVersion", -1);
      int num1 = this.RequestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(PlatformIdentityStore.s_featureNameUseProviderDisplayName) ? 1 : 0;
      bool premisesDeployment = this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      int minResourceVersion = castedValueOrDefault;
      int num2 = premisesDeployment ? 1 : 0;
      return new IdentityBindingConfig(num1 != 0, minResourceVersion, num2 != 0, true, true);
    }

    protected class IdentityColumns9 : IdentityManagementComponent19.IdentityColumns8
    {
      private SqlColumnBinder MetadataUpdateDate = new SqlColumnBinder(nameof (MetadataUpdateDate));
      protected readonly IdentityBindingConfig BindingConfig;

      public IdentityColumns9(IdentityBindingConfig bindingConfig) => this.BindingConfig = bindingConfig;

      protected override DateTime? BindMetadataUpdateDate(SqlDataReader reader)
      {
        DateTime dateTime = this.MetadataUpdateDate.GetDateTime((IDataReader) reader);
        return !(dateTime == DateTime.MinValue) ? new DateTime?(dateTime) : new DateTime?();
      }

      protected override int BindResourceVersion(SqlDataReader reader) => Math.Max(base.BindResourceVersion(reader), this.BindingConfig.MinResourceVersion);

      protected override string BindCustomDisplayName(
        SqlDataReader reader,
        string providerDisplayName)
      {
        return !this.BindingConfig.UseProviderDisplayName ? base.BindCustomDisplayName(reader, providerDisplayName) : (string) null;
      }
    }

    protected class IdentitiesColumns9 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent20.IdentityColumns9 IdentityColumns;
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      public IdentitiesColumns9(IdentityBindingConfig bindingConfig) => this.IdentityColumns = new IdentityManagementComponent20.IdentityColumns9(bindingConfig);

      protected IdentitiesColumns9(
        IdentityManagementComponent20.IdentityColumns9 identityColumns)
      {
        this.IdentityColumns = identityColumns;
      }

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
