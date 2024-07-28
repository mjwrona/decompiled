// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent16
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
  internal class IdentityManagementComponent16 : IdentityManagementComponent15
  {
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
        resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent16.IdentitiesColumns7());
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
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent16.IdentityColumns7());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(47011119, nameof (ReadIdentity));
      }
    }

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
        this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit);
        return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out deletedIds, out identityChangedData, out identitiesToTransfer);
      }
      finally
      {
        this.TraceLeave(47011129, nameof (UpdateIdentities));
      }
    }

    private List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentity(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      try
      {
        this.TraceEnter(47011130, nameof (UpdateIdentity));
        this.ThrowIfIdentityTypeInvalid(identity);
        this.PrepareIdentityForUpdate(identity);
        this.PrepareStoredProcedure("prc_UpdateIdentity");
        this.BindString("@sid", identity.Descriptor.Identifier, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindByte("@typeId", IdentityTypeMapper.Instance.GetTypeIdFromName(identity.Descriptor.IdentityType));
        this.BindNullableGuid("@id", identity.Id);
        this.BindString("@providerDisplayName", identity.ProviderDisplayName, 515, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@displayName", identity.CustomDisplayName, 515, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@description", identity.GetProperty<string>("Description", (string) null), 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@domain", identity.GetProperty<string>("Domain", (string) null), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@accountName", identity.GetProperty<string>("Account", (string) null), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@distinguishedName", identity.GetProperty<string>("DN", (string) null), 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@mailAddress", identity.GetProperty<string>("Mail", (string) null), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@isGroup", identity.IsContainer);
        this.BindString("@alias", identity.GetProperty<string>("Alias", (string) null), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@puid", identity.GetProperty<string>("PUID", (string) null), 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        DateTime property = identity.GetProperty<DateTime>("ComplianceValidated", DateTime.MinValue);
        this.BindNullableDateTime("@complianceValidated", property == DateTime.MinValue ? new DateTime?() : new DateTime?(property));
        this.BindGuid("@eventAuthor", this.Author);
        this.BindBoolean("@updateIdentityAudit", updateIdentityAudit);
        this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
        if (identity.GetModifiedProperties() != null && identity.GetModifiedProperties().Any<string>())
          this.BindBoolean("@updateIdentityExtendedProperties", true);
        if (identity.ResourceVersion > 0)
          this.BindByte("@resourceVersion", (byte) identity.ResourceVersion);
        if (identity.MetaTypeId >= 0 && identity.MetaTypeId < (int) byte.MaxValue)
          this.BindByte("@metaTypeId", (byte) identity.MetaTypeId);
        this.BindIdentityExtendedProperties(identity);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IdentityChangedData>((ObjectBinder<IdentityChangedData>) new IdentityManagementComponent.UpdateIdentitySummaryColumns());
          resultCollection.AddBinder<Tuple<int, Guid, bool>>((ObjectBinder<Tuple<int, Guid, bool>>) new IdentityManagementComponent.UpdateIdentityColumns());
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new IdentitiesToTransferColumns());
          resultCollection.AddBinder<DescriptorChange>((ObjectBinder<DescriptorChange>) new IdentityManagementComponent.DescriptorChangeColumns());
          identityChangedData = resultCollection.GetCurrent<IdentityChangedData>().FirstOrDefault<IdentityChangedData>();
          if (identityChangedData == null)
            throw new UnexpectedDatabaseResultException(this.ProcedureName);
          resultCollection.NextResult();
          List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
          foreach (Tuple<int, Guid, bool> tuple in resultCollection.GetCurrent<Tuple<int, Guid, bool>>())
          {
            identity.Id = tuple.Item2;
            identity.MasterId = tuple.Item2;
            if (tuple.Item3)
              identityList.Add(identity);
          }
          resultCollection.NextResult();
          deletedIds = resultCollection.GetCurrent<Guid>().Items;
          resultCollection.NextResult();
          identitiesToTransfer = resultCollection.GetCurrent<KeyValuePair<Guid, Guid>>().Items;
          if (identityChangedData.DescriptorChangeType == DescriptorChangeType.Minor)
          {
            resultCollection.NextResult();
            identityChangedData.DescriptorChanges = resultCollection.GetCurrent<DescriptorChange>().ToArray<DescriptorChange>();
          }
          return identityList;
        }
      }
      finally
      {
        this.TraceLeave(47011139, nameof (UpdateIdentity));
      }
    }

    protected void ThrowIfIdentityTypeInvalid(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!IdentityTypeMapper.Instance.IsValidTypeName(identity.Descriptor.IdentityType))
        throw new DynamicIdentityTypeCreationNotSupportedException();
    }

    protected class IdentityColumns7 : IdentityManagementComponent10.IdentityColumns6
    {
      private SqlColumnBinder TypeId = new SqlColumnBinder(nameof (TypeId));

      protected override IdentityDescriptor BindIdentityDescriptor(SqlDataReader reader)
      {
        string identifier = this.Sid.GetString((IDataReader) reader, false);
        byte typeId = this.TypeId.GetByte((IDataReader) reader);
        return new IdentityDescriptor(IdentityTypeMapper.Instance.GetTypeNameFromId(typeId), identifier);
      }

      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind()
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind();
        if (identity != null)
        {
          if (identity.GetProperty<DateTime>("ComplianceValidated", DateTime.MinValue).AddDays(90.0) <= DateTime.UtcNow)
            identity.SetProperty("ComplianceValidated", (object) DateTime.UtcNow.Date);
          DateTime? nullable = this.BindMetadataUpdateDate(this.Reader);
          if (nullable.HasValue)
            identity.SetProperty("MetadataUpdateDate", (object) nullable.Value);
        }
        return identity;
      }
    }

    protected class IdentitiesColumns7 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent16.IdentityColumns7 IdentityColumns;
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      public IdentitiesColumns7() => this.IdentityColumns = new IdentityManagementComponent16.IdentityColumns7();

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
