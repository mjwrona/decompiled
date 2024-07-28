// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent18
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
  internal class IdentityManagementComponent18 : IdentityManagementComponent17
  {
    public override sealed List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      List<Guid> createdAndBoundIds = (List<Guid>) null;
      return this.UpdateIdentities(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, out createdAndBoundIds, out deletedIds, out identityChangedData, out identitiesToTransfer);
    }

    protected override sealed List<Microsoft.VisualStudio.Services.Identity.Identity> GetResults(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      List<Guid> createdIds = (List<Guid>) null;
      return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out createdIds, out deletedIds, out identityChangedData, out identitiesToTransfer);
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
      this.BindIdentityTable6("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindAdditionalUpdateParameters(updates, favorCurrentlyActive, updateIdentityAudit);
      this.BindBoolean("@allowMetadataUpdates", allowMetadataUpdates);
      return this.GetResults(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, out createdAndBoundIds, out deletedIds, out identityChangedData, out identitiesToTransfer);
    }

    protected virtual List<Microsoft.VisualStudio.Services.Identity.Identity> GetResults(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      out List<Guid> createdIds,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityChangedData>((ObjectBinder<IdentityChangedData>) new IdentityManagementComponent.UpdateIdentitySummaryColumns());
        resultCollection.AddBinder<Tuple<int, Guid, bool>>((ObjectBinder<Tuple<int, Guid, bool>>) new IdentityManagementComponent.UpdateIdentityColumns());
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
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
        createdIds = resultCollection.GetCurrent<Guid>().Items;
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

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByTypeId(
      byte typeId)
    {
      this.TraceEnter(47011140, nameof (ReadIdentitiesByTypeId));
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      try
      {
        this.PrepareStoredProcedure("prc_ReadIdentitiesByTypeId");
        this.BindByte("@typeId", typeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent18.IdentityMigrationColumns());
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            if (identity != null)
              identityList.Add(identity);
          }
        }
      }
      finally
      {
        this.TraceLeave(47011149, nameof (ReadIdentitiesByTypeId));
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList;
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByTypeIdAndPartialSid(
      byte typeId,
      string partialSid)
    {
      this.TraceEnter(47011150, nameof (ReadIdentitiesByTypeIdAndPartialSid));
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      try
      {
        if (string.IsNullOrWhiteSpace(partialSid))
          throw new ArgumentNullException("Partial SID cannot be null");
        this.PrepareStoredProcedure("prc_ReadIdentitiesByTypeIdAndPartialSid");
        this.BindByte("@typeId", typeId);
        this.BindString("@partialSidPattern", partialSid + (object) '%', 256, false, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent18.IdentityMigrationColumns());
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            if (identity != null)
              identityList.Add(identity);
          }
        }
      }
      finally
      {
        this.TraceLeave(47011159, nameof (ReadIdentitiesByTypeIdAndPartialSid));
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList;
    }

    public override void InsertIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.TraceEnter(47011160, nameof (InsertIdentities));
      try
      {
        this.PrepareStoredProcedure("prc_InsertIdentities");
        this.BindIdentityTable6("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, (HashSet<string>) null);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011169, nameof (InsertIdentities));
      }
    }

    protected class IdentityMigrationColumns : IdentityManagementComponent9.IdentityColumns5
    {
      private SqlColumnBinder TypeId = new SqlColumnBinder(nameof (TypeId));

      protected override IdentityDescriptor BindIdentityDescriptor(SqlDataReader reader)
      {
        string identifier = this.Sid.GetString((IDataReader) reader, false);
        byte typeId = this.TypeId.GetByte((IDataReader) reader);
        return new IdentityDescriptor(IdentityTypeMapper.Instance.GetTypeNameFromId(typeId), identifier);
      }
    }
  }
}
