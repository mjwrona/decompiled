// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ReindexingStatusTableV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ReindexingStatusTableV2 : ReindexingStatusTable
  {
    public ReindexingStatusTableV2()
    {
    }

    internal ReindexingStatusTableV2(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override ReindexingStatusEntry Insert(ReindexingStatusEntry reindexingStatus)
    {
      this.ValidateNotNull<ReindexingStatusEntry>(nameof (reindexingStatus), reindexingStatus);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddOrUpdateEntryForReindexingStatusTable");
        this.BindGuid("@collectionId", reindexingStatus.CollectionId);
        this.BindByte("@entityType", (byte) reindexingStatus.EntityType.ID);
        this.BindByte("@status", (byte) reindexingStatus.Status);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Add entry with Collection ID {0}, Status {1} and EntityType {2} with SQL Azure platform", (object) reindexingStatus.CollectionId, (object) reindexingStatus.Status, (object) reindexingStatus.EntityType.Name));
      }
      return (ReindexingStatusEntry) null;
    }

    public override List<ReindexingStatusEntry> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryOnReindexingStatusTable");
        this.BindInt("@count", count);
        TableEntityFilter propertyFilter;
        if (filterList.TryRetrieveFilter("Status", out propertyFilter))
        {
          this.BindByte("@status", (byte) (ReindexingStatus) Enum.Parse(typeof (ReindexingStatus), propertyFilter.Value));
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.Columns(this.m_entityTypes));
            ObjectBinder<ReindexingStatusEntry> current = resultCollection.GetCurrent<ReindexingStatusEntry>();
            return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<ReindexingStatusEntry>();
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve ReindexingStatusSQLTableEntry list with SQL Azure Platform");
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
    }

    public override ReindexingStatusEntry RetriveTableEntity(TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryOnReindexingStatusTable");
        TableEntityFilter propertyFilter;
        if (filterList.TryRetrieveFilter("CollectionId", out propertyFilter))
        {
          this.BindGuid("@collectionId", new Guid(propertyFilter.Value));
          if (filterList.TryRetrieveFilter("EntityType", out propertyFilter))
          {
            this.BindByte("@entityType", (byte) EntityPluginsFactory.GetEntityType(this.m_entityTypes, propertyFilter.Value).ID);
            using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
            {
              resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.Columns(this.m_entityTypes));
              ObjectBinder<ReindexingStatusEntry> current = resultCollection.GetCurrent<ReindexingStatusEntry>();
              if (current?.Items == null || current.Items.Count <= 0)
                return (ReindexingStatusEntry) null;
              if (current.Items.Count == 1)
                return resultCollection.GetCurrent<ReindexingStatusEntry>().Items[0];
              throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "More than one matching ReindexingStatus found for the input filter list");
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve ReindexingStatus with SQL Azure Platform");
      }
      throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
    }

    public override bool DeleteTableEntity(Guid collectionId, IEntityType entityType)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteReindexingStatusEntry");
        this.BindGuid("@collectionId", collectionId);
        this.BindByte("@entityType", (byte) entityType.ID);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Delete Reindexing Status entry for Collection ID: {0} and EntityType: {1}", (object) collectionId, (object) entityType)));
      }
      return true;
    }
  }
}
