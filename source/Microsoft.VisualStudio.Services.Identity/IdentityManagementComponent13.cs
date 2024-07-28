// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent13
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
  internal class IdentityManagementComponent13 : IdentityManagementComponent12
  {
    public override int InvalidateIdentities(bool updateIdentityAudit, IList<Guid> identityIds = null)
    {
      try
      {
        this.TraceEnter(47011000, nameof (InvalidateIdentities));
        if (identityIds == null || identityIds.Count == 0)
          return -1;
        this.PrepareStoredProcedure("prc_InvalidateIdentities");
        this.BindGuid("@eventAuthor", this.Author);
        this.BindGuidTable("@ids", (IEnumerable<Guid>) identityIds);
        this.BindBoolean("@updateIdentityAudit", updateIdentityAudit);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        return sqlDataReader.Read() ? sqlDataReader.GetInt32(0) : -1;
      }
      finally
      {
        this.TraceLeave(47011009, nameof (InvalidateIdentities));
      }
    }

    public override ResultCollection GetChanges(
      int sequenceId,
      ref int lastSequenceId,
      long firstAuditSequenceId,
      bool useIdentityAudit,
      IEnumerable<Guid> scopedIdentityIds = null,
      bool includeDescriptorChanges = false)
    {
      this.PrepareStoredProcedure("prc_GetIdentityChangesById");
      this.BindInt("@sequenceId", sequenceId);
      if (lastSequenceId > 0)
        this.BindInt("@lastSequenceId", lastSequenceId);
      if (scopedIdentityIds == null)
        scopedIdentityIds = (IEnumerable<Guid>) new List<Guid>();
      this.BindGuidTable("@scopedIdentityIds", scopedIdentityIds);
      if (useIdentityAudit)
        this.BindBoolean("@useIdentityAudit", useIdentityAudit);
      this.BindBoolean("@includeDescriptorChanges", includeDescriptorChanges);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException(this.ProcedureName);
      lastSequenceId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException(this.ProcedureName);
      ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
      changes.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
      if (includeDescriptorChanges)
        changes.AddBinder<IdentityDescriptorChange>((ObjectBinder<IdentityDescriptorChange>) new IdentityDescriptorChangeBinder());
      return changes;
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
      this.PrepareUpdateIdentitiesStoredProcedure(updates);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
      {
        update.ResourceVersion = 1;
        if (update.Id == Guid.Empty)
          update.Id = Guid.NewGuid();
        string str = IdentityHelper.CleanProviderDisplayName(update.ProviderDisplayName, update.Descriptor);
        if (!string.IsNullOrWhiteSpace(str))
          update.ProviderDisplayName = str;
        if (update.CustomDisplayName != null)
          update.CustomDisplayName = IdentityHelper.CleanCustomDisplayName(update.CustomDisplayName, update.Descriptor, false);
      }
      this.BindIdentityTable5("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
      if (updates.Any<Microsoft.VisualStudio.Services.Identity.Identity>((System.Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i.GetModifiedProperties() != null && i.GetModifiedProperties().Any<string>())))
        this.BindBoolean("@updateIdentityExtendedProperties", true);
      if (updateIdentityAudit)
        this.BindBoolean("@updateIdentityAudit", true);
      this.BindIdentityExtensionTableForIdentityComponent11OrLater("@identityExtendedProperties", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates);
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
          Microsoft.VisualStudio.Services.Identity.Identity update = updates[tuple.Item1];
          update.Id = tuple.Item2;
          update.MasterId = tuple.Item2;
          if (tuple.Item3)
            identityList.Add(update);
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

    public override List<IdentityAuditRecord> GetIdentityAuditRecords(DateTime lastUpdatedOnOrBefore)
    {
      this.PrepareStoredProcedure("prc_GetIdentityAuditRecords");
      this.BindDateTime("@LastUpdatedOnOrBefore", lastUpdatedOnOrBefore);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityAuditRecord>((ObjectBinder<IdentityAuditRecord>) new IdentityManagementComponent.IdentityAuditRecordColumns());
        List<IdentityAuditRecord> identityAuditRecords = new List<IdentityAuditRecord>();
        foreach (IdentityAuditRecord identityAuditRecord in resultCollection.GetCurrent<IdentityAuditRecord>())
        {
          if (identityAuditRecord != null)
            identityAuditRecords.Add(identityAuditRecord);
        }
        return identityAuditRecords;
      }
    }

    public override void DeleteIdentityAuditRecords(long sequenceId)
    {
      this.PrepareStoredProcedure("prc_DeleteIdentityAuditRecords");
      this.BindLong("@sequenceId", sequenceId);
      this.ExecuteNonQuery();
    }
  }
}
