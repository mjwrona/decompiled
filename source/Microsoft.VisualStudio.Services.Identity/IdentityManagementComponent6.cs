// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent6
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
  internal class IdentityManagementComponent6 : IdentityManagementComponent5
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
        this.TraceEnter(4701400, nameof (UpdateIdentities));
        this.PrepareUpdateIdentitiesStoredProcedure(updates);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity update in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates)
        {
          string str = IdentityHelper.CleanProviderDisplayName(update.ProviderDisplayName, update.Descriptor);
          if (!string.IsNullOrWhiteSpace(str))
            update.ProviderDisplayName = str;
          if (update.CustomDisplayName != null)
            update.CustomDisplayName = IdentityHelper.CleanCustomDisplayName(update.CustomDisplayName, update.Descriptor, false);
        }
        this.BindIdentityTable4("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
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
      finally
      {
        this.TraceLeave(4701409, nameof (UpdateIdentities));
      }
    }
  }
}
