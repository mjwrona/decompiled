// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent9
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
  internal class IdentityManagementComponent9 : IdentityManagementComponent8
  {
    private const string AadOidPrefix = "oid:";
    private const char IdentifierSeparator = '|';

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
        string str = IdentityHelper.CleanProviderDisplayName(update.ProviderDisplayName, update.Descriptor);
        if (!string.IsNullOrWhiteSpace(str))
          update.ProviderDisplayName = str;
        if (update.CustomDisplayName != null)
          update.CustomDisplayName = IdentityHelper.CleanCustomDisplayName(update.CustomDisplayName, update.Descriptor, false);
      }
      this.BindIdentityTable5("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindBoolean("@favorCurrentlyActive", favorCurrentlyActive);
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

    public override ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_ReadIdentities");
      this.BindOrderedDescriptorTable("@descriptors", descriptors, true);
      this.BindOrderedGuidTable("@ids", ids, true);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent9.IdentitiesColumns5());
      return resultCollection;
    }

    public override ResultCollection ReadIdentity(
      IdentitySearchFilter searchFactor,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      bool? isGroup)
    {
      this.PrepareStoredProcedure("prc_ReadIdentity");
      this.BindInt("@searchFactor", (int) searchFactor);
      this.BindString("@factorValue", factorValue, 515, false, SqlDbType.NVarChar);
      this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
      this.BindString("@account", account, 256, true, SqlDbType.NVarChar);
      this.BindInt("@uniqueUserId", uniqueUserId);
      if (isGroup.HasValue)
        this.BindBoolean("@isGroup", isGroup.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent9.IdentityColumns5());
      return resultCollection;
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

    public override int CleanupDescriptorChangeQueue(int beforeDays)
    {
      this.PrepareStoredProcedure("prc_DeleteFromDescriptorQueue");
      this.BindInt("@beforeDays", beforeDays);
      return (int) this.ExecuteScalar();
    }

    protected class IdentityColumns5 : IdentityManagementComponent5.IdentityColumns4
    {
      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind() => this.Bind(this.Reader);

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        object obj;
        if (identity.TryGetProperty("PUID", out obj))
        {
          string[] strArray;
          if (obj == null)
            strArray = (string[]) null;
          else
            strArray = obj.ToString().Split(new char[1]
            {
              '|'
            }, StringSplitOptions.RemoveEmptyEntries);
          string[] source = strArray;
          if (source != null && source.Length != 0)
          {
            int length = source.Length;
            identity.Properties.Remove("PUID");
            identity.Properties.Remove("http://schemas.microsoft.com/identity/claims/objectidentifier");
            foreach (string str in ((IEnumerable<string>) source).Take<string>(2))
            {
              if (str != null)
              {
                if (str.StartsWith("oid:"))
                  identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) str.Substring("oid:".Length));
                else
                  identity.SetProperty("PUID", (object) str);
              }
            }
          }
        }
        else if (identity.IsClaims && identity.GetProperty<string>("Domain", string.Empty).Equals("Windows Live ID", StringComparison.OrdinalIgnoreCase) && identity.Descriptor.Identifier.Length == 25 && identity.Descriptor.Identifier.Substring(16).Equals("@Live.com", StringComparison.OrdinalIgnoreCase))
          identity.SetProperty("PUID", (object) identity.Descriptor.Identifier.Substring(0, 16));
        return identity;
      }
    }

    protected class IdentitiesColumns5 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent9.IdentityColumns5 IdentityColumns = new IdentityManagementComponent9.IdentityColumns5();
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
