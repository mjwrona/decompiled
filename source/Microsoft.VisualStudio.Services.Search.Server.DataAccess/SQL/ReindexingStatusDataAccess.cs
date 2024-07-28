// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.ReindexingStatusDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class ReindexingStatusDataAccess : SqlAzureDataAccess, IReindexingStatusDataAccess
  {
    public ReindexingStatusDataAccess()
    {
    }

    protected ReindexingStatusDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    public void AddOrUpdateReindexingStatusEntry(
      IVssRequestContext requestContext,
      ReindexingStatusEntry entry)
    {
      this.ValidateNotNull<ReindexingStatusEntry>(nameof (entry), entry);
      using (ITable<ReindexingStatusEntry> table = this.m_tableAccessPlatform.GetTable<ReindexingStatusEntry>(requestContext))
        this.InvokeTableOperation<ReindexingStatusEntry>((Func<ReindexingStatusEntry>) (() => table.Insert(entry)));
    }

    public IEnumerable<ReindexingStatusEntry> GetReindexingStatusEntries(
      IVssRequestContext requestContext,
      int count,
      ReindexingStatus status)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("Status", "eq", status.ToString()));
      TableEntityFilterList filters = entityFilterList;
      using (ITable<ReindexingStatusEntry> table = this.m_tableAccessPlatform.GetTable<ReindexingStatusEntry>(requestContext))
        return (IEnumerable<ReindexingStatusEntry>) this.InvokeTableOperation<List<ReindexingStatusEntry>>((Func<List<ReindexingStatusEntry>>) (() => table.RetriveTableEntityList(count, filters)));
    }

    public virtual ReindexingStatusEntry GetReindexingStatusEntry(
      IVssRequestContext requestContext,
      Guid collectionId,
      IEntityType entityType)
    {
      this.ValidateNotNull<Guid>(nameof (collectionId), collectionId);
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      using (ReindexingStatusTable reindexingStatusTable = requestContext.CreateComponent<ReindexingStatusTable>())
      {
        if (reindexingStatusTable is ReindexingStatusTableV3 reindexingStatusTableV3)
          return reindexingStatusTableV3.GetReindexingStatusEntry(collectionId, entityType);
        TableEntityFilterList entityFilterList = new TableEntityFilterList();
        entityFilterList.Add(new TableEntityFilter("CollectionId", "eq", collectionId.ToString()));
        entityFilterList.Add(new TableEntityFilter("EntityType", "eq", entityType.Name));
        TableEntityFilterList filters = entityFilterList;
        return this.InvokeTableOperation<ReindexingStatusEntry>((Func<ReindexingStatusEntry>) (() => reindexingStatusTable.RetriveTableEntity(filters)));
      }
    }

    public void DeleteReindexingStatusEntry(
      IVssRequestContext requestContext,
      Guid collectionId,
      IEntityType entityType)
    {
      this.ValidateNotNull<IVssRequestContext>(nameof (requestContext), requestContext);
      this.ValidateNotEmptyGuid(nameof (collectionId), collectionId);
      try
      {
        using (ReindexingStatusTable table = requestContext.CreateComponent<ReindexingStatusTable>())
          this.InvokeTableOperation<bool>((Func<bool>) (() => table.DeleteTableEntity(collectionId, entityType)));
      }
      catch (Exception ex)
      {
        SqlAzureDataAccessUtility.WrapAndThrowException(ex);
      }
    }

    public void AddOrUpdateReindexingStatusEntries(
      IVssRequestContext requestContext,
      List<ReindexingStatusEntry> reindexingStatusEntries)
    {
      this.ValidateNotNull<IVssRequestContext>(nameof (requestContext), requestContext);
      this.ValidateNotNull<List<ReindexingStatusEntry>>(nameof (reindexingStatusEntries), reindexingStatusEntries);
      using (ReindexingStatusTable component = requestContext.CreateComponent<ReindexingStatusTable>())
        component.AddOrUpdateReindexingStatusEntries(reindexingStatusEntries);
    }

    public List<ReindexingStatusEntry> GetReindexingStatusEntries(
      IVssRequestContext requestContext,
      List<KeyValuePair<Guid, IEntityType>> collections)
    {
      this.ValidateNotNull<IVssRequestContext>(nameof (requestContext), requestContext);
      this.ValidateNotNull<List<KeyValuePair<Guid, IEntityType>>>(nameof (collections), collections);
      List<ReindexingStatusEntry> list = collections.Select<KeyValuePair<Guid, IEntityType>, ReindexingStatusEntry>((Func<KeyValuePair<Guid, IEntityType>, ReindexingStatusEntry>) (it => new ReindexingStatusEntry(it.Key, it.Value))).ToList<ReindexingStatusEntry>();
      using (ReindexingStatusTable component = requestContext.CreateComponent<ReindexingStatusTable>())
        return component is ReindexingStatusTableV3 reindexingStatusTableV3 ? reindexingStatusTableV3.GetReindexingStatusEntries(list) : (List<ReindexingStatusEntry>) null;
    }
  }
}
