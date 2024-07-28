// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent15
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent15 : IdentityManagementComponent14
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
      this.PrepareUpdateIdentitiesStoredProcedure(updates);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
        this.PrepareIdentityForUpdate(update);
      this.BindIdentityTable5("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit);
      return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out deletedIds, out identityChangedData, out identitiesToTransfer);
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
      this.PrepareStoredProcedure("prc_UpdateIdentity");
      this.PrepareIdentityForUpdate(identity);
      this.BindString("@sid", identity.Descriptor.Identifier, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@type", identity.Descriptor.IdentityType, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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

    protected virtual void BindAdditionalUpdateParameters(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      bool favorCurrentlyActive,
      bool updateIdentityAudit)
    {
      this.BindGuid("@eventAuthor", this.Author);
      this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
      if (this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        this.BindBoolean("@updateIdentityExtendedProperties", false);
      else if (updates.Any<Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.GetModifiedProperties() != null && i.GetModifiedProperties().Any<string>())))
        this.BindBoolean("@updateIdentityExtendedProperties", true);
      if (updateIdentityAudit)
        this.BindBoolean("@updateIdentityAudit", true);
      if (this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      this.BindIdentityExtensionTableForIdentityComponent11OrLater("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates);
    }

    protected virtual void BindIdentityExtendedProperties(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity.ResourceVersion <= 0)
        return;
      HashSet<string> modifiedProperties = identity.GetModifiedProperties();
      if (modifiedProperties == null || !modifiedProperties.Any<string>())
        return;
      if (modifiedProperties.Contains("http://schemas.microsoft.com/identity/claims/objectidentifier"))
        this.BindGuid("@externalId", identity.Properties.GetValue<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty));
      if (modifiedProperties.Contains("AuthenticationCredentialValidFrom"))
      {
        long ticks = identity.Properties.GetValue<long>("AuthenticationCredentialValidFrom", 0L);
        this.BindDateTime("@authenticationCredentialValidFrom", ticks == 0L ? new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc) : new DateTime(ticks, DateTimeKind.Utc));
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Id"))
      {
        byte[] b = identity.Properties.GetValue<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
        this.BindGuid("@imageId", b == null ? Guid.Empty : new Guid(b));
      }
      if (modifiedProperties.Contains("Microsoft.TeamFoundation.Identity.Image.Type"))
        this.BindString("@imageType", identity.Properties.GetValue<string>("Microsoft.TeamFoundation.Identity.Image.Type", string.Empty), 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      if (modifiedProperties.Contains("ConfirmedNotificationAddress"))
        this.BindString("@confirmedNotificationAddress", identity.Properties.GetValue<string>("ConfirmedNotificationAddress", string.Empty), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (!modifiedProperties.Contains("CustomNotificationAddresses"))
        return;
      this.BindString("@customNotificationAddresses", identity.Properties.GetValue<string>("CustomNotificationAddresses", string.Empty), 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }

    protected virtual void PrepareIdentityForUpdate(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity.Id == Guid.Empty)
        identity.Id = this.GenerateIdentityId(identity);
      string str = IdentityHelper.CleanProviderDisplayName(identity.ProviderDisplayName, identity.Descriptor);
      if (!string.IsNullOrWhiteSpace(str))
        identity.ProviderDisplayName = str;
      if (identity.CustomDisplayName == null)
        return;
      identity.CustomDisplayName = IdentityHelper.CleanCustomDisplayName(identity.CustomDisplayName, identity.Descriptor, false);
    }

    internal virtual Guid GenerateIdentityId(Microsoft.VisualStudio.Services.Identity.Identity identity) => Guid.NewGuid();

    protected virtual List<Microsoft.VisualStudio.Services.Identity.Identity> GetResults(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
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
        List<Microsoft.VisualStudio.Services.Identity.Identity> results = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        foreach (Tuple<int, Guid, bool> tuple in resultCollection.GetCurrent<Tuple<int, Guid, bool>>())
        {
          Microsoft.VisualStudio.Services.Identity.Identity update = updates[tuple.Item1];
          update.Id = tuple.Item2;
          update.MasterId = tuple.Item2;
          if (tuple.Item3)
            results.Add(update);
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
        return results;
      }
    }
  }
}
