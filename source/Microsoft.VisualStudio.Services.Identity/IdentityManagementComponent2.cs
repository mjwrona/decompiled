// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent2
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
  internal class IdentityManagementComponent2 : IdentityManagementComponent
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
      try
      {
        this.TraceEnter(4701100, nameof (UpdateIdentities));
        this.PrepareUpdateIdentitiesStoredProcedure(updates);
        this.BindIdentityTable2("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
        this.BindGuid("@eventAuthor", this.Author);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IdentityChangedData>((ObjectBinder<IdentityChangedData>) new IdentityManagementComponent.UpdateIdentitySummaryColumns());
          resultCollection.AddBinder<Tuple<int, Guid, bool>>((ObjectBinder<Tuple<int, Guid, bool>>) new IdentityManagementComponent.UpdateIdentityColumns());
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
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
          if (identityChangedData.DescriptorChangeType == DescriptorChangeType.Minor)
          {
            resultCollection.NextResult();
            identityChangedData.DescriptorChanges = resultCollection.GetCurrent<DescriptorChange>().ToArray<DescriptorChange>();
          }
          identitiesToTransfer = new List<KeyValuePair<Guid, Guid>>();
          return identityList;
        }
      }
      finally
      {
        this.TraceLeave(4701109, nameof (UpdateIdentities));
      }
    }

    public override ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      try
      {
        this.TraceEnter(4701110, nameof (ReadIdentities));
        this.PrepareStoredProcedure("prc_ReadIdentities");
        this.BindOrderedDescriptorTable("@descriptors", descriptors, true);
        this.BindOrderedGuidTable("@ids", ids, true);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent2.IdentitiesColumns2());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4701119, nameof (ReadIdentities));
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
        this.TraceEnter(4701120, nameof (ReadIdentity));
        this.PrepareStoredProcedure("prc_ReadIdentity");
        this.BindInt("@searchFactor", (int) searchFactor);
        this.BindString("@factorValue", factorValue, 515, false, SqlDbType.NVarChar);
        this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
        this.BindString("@account", account, 256, true, SqlDbType.NVarChar);
        this.BindInt("@uniqueUserId", uniqueUserId);
        if (isGroup.HasValue)
          this.BindBoolean("@isGroup", isGroup.Value);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent2.IdentityColumns2());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4701129, nameof (ReadIdentity));
      }
    }

    protected class IdentityColumns2 : IdentityManagementComponent.IdentityColumns
    {
      private SqlColumnBinder Alias = new SqlColumnBinder(nameof (Alias));

      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind() => this.Bind(this.Reader);

      internal override Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = base.Bind(reader);
        string str = this.Alias.GetString((IDataReader) reader, true);
        if (!string.IsNullOrEmpty(str))
          identity.SetProperty("Alias", (object) str);
        return identity;
      }
    }

    protected class IdentitiesColumns2 : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent2.IdentityColumns2 IdentityColumns = new IdentityManagementComponent2.IdentityColumns2();
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
