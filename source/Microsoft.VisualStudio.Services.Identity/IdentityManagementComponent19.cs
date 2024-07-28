// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent19
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
  internal class IdentityManagementComponent19 : IdentityManagementComponent18
  {
    public virtual SqlParameter BindIdentityTableForUpdateOrInsert(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      HashSet<string> propertiesToUpdate)
    {
      return this.BindIdentityTable7(parameterName, identities, propertiesToUpdate);
    }

    public override List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> createdAndBoundIds,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      this.PrepareUpdateIdentitiesStoredProcedure(updates);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
      {
        this.ThrowIfIdentityTypeInvalid(update);
        this.PrepareIdentityForUpdate(update);
      }
      this.BindIdentityTableForUpdateOrInsert("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates);
      return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out createdAndBoundIds, out deletedIds, out identityChangedData, out identitiesToTransfer);
    }

    public override void InsertIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.TraceEnter(47011160, nameof (InsertIdentities));
      try
      {
        this.PrepareStoredProcedure("prc_InsertIdentities");
        this.BindIdentityTableForUpdateOrInsert("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, (HashSet<string>) null);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011169, nameof (InsertIdentities));
      }
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
        resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent19.IdentitiesColumns8());
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
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent19.IdentityColumns8());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(47011119, nameof (ReadIdentity));
      }
    }

    protected override void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit)
    {
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, false);
    }

    protected override void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates)
    {
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter> binder = (Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter>) ((updateList, allowMetadataUpdate) => this.BindIdentityExtensionTableForIdentityComponent19OrLater("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, allowMetadataUpdates));
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, binder);
    }

    protected void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      Func<IList<Microsoft.VisualStudio.Services.Identity.Identity>, bool, SqlParameter> binder)
    {
      this.BindGuid("@eventAuthor", this.Author);
      this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
      if (this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        this.BindBoolean("@updateIdentityExtendedProperties", false);
      else if (allowMetadataUpdates && updates.Any<Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.HasModifiedProperties)) || updates.Any<Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.GetModifiedProperties() != null && i.GetModifiedProperties().Any<string>())))
        this.BindBoolean("@updateIdentityExtendedProperties", true);
      if (updateIdentityAudit)
        this.BindBoolean("@updateIdentityAudit", true);
      if (!this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        SqlParameter sqlParameter = binder(updates, allowMetadataUpdates);
      }
      this.BindBoolean("@allowMetadataUpdates", allowMetadataUpdates);
    }

    protected class IdentityColumns8 : IdentityManagementComponent16.IdentityColumns7
    {
      protected override void BindComplianceValidatedIfNecessary(
        SqlDataReader reader,
        Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        identity.SetProperty("ComplianceValidated", (object) DateTime.UtcNow.Date);
      }
    }

    protected class IdentitiesColumns8 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent19.IdentityColumns8 IdentityColumns;
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      public IdentitiesColumns8() => this.IdentityColumns = new IdentityManagementComponent19.IdentityColumns8();

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
