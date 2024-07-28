// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ReindexingStatusTableV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ReindexingStatusTableV3 : ReindexingStatusTableV2
  {
    private static readonly SqlMetaData[] s_reindexingStatusSqlTableEntityLookUptable = new SqlMetaData[4]
    {
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("EntityType", SqlDbType.TinyInt),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Priority", SqlDbType.SmallInt)
    };

    public ReindexingStatusTableV3()
    {
    }

    internal ReindexingStatusTableV3(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override ReindexingStatusEntry GetReindexingStatusEntry(
      Guid collectionId,
      IEntityType entityType)
    {
      if (collectionId == Guid.Empty)
        throw new ArgumentException(nameof (collectionId));
      if (entityType.Name == "All")
        throw new ArgumentException("Name");
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryReindexingStatusTableById");
        this.BindGuid("@collectionId", collectionId);
        this.BindByte("@entityType", (byte) entityType.ID);
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
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve ReindexingStatus with SQL Azure Platform");
      }
    }

    public List<ReindexingStatusEntry> GetReindexingStatusEntries(
      List<ReindexingStatusEntry> reindexingStatusEntries)
    {
      this.ValidateNotNull<List<ReindexingStatusEntry>>(nameof (reindexingStatusEntries), reindexingStatusEntries);
      List<ReindexingStatusEntry> reindexingStatusEntries1 = new List<ReindexingStatusEntry>();
      try
      {
        this.PrepareStoredProcedure("Search.prc_GetReindexingStatusEntries");
        this.BindReindexingStatusEntryLookupTable("@itemList", (IEnumerable<ReindexingStatusEntry>) reindexingStatusEntries, false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.ReindexingStatusColumns(this.m_entityTypes));
          reindexingStatusEntries1.AddRange((IEnumerable<ReindexingStatusEntry>) resultCollection.GetCurrent<ReindexingStatusEntry>().Items);
        }
        return reindexingStatusEntries1;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Exception occured in Reading Entries:" + reindexingStatusEntries.Select<ReindexingStatusEntry, string>((System.Func<ReindexingStatusEntry, string>) (i => i.ToString())).Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)));
      }
    }

    public override List<ReindexingStatusEntry> AddOrUpdateReindexingStatusEntries(
      List<ReindexingStatusEntry> reindexingStatusEntries)
    {
      this.ValidateNotNull<List<ReindexingStatusEntry>>(nameof (reindexingStatusEntries), reindexingStatusEntries);
      List<ReindexingStatusEntry> reindexingStatusEntryList = new List<ReindexingStatusEntry>();
      try
      {
        int count1 = reindexingStatusEntries.Count;
        int val1 = count1;
        for (int index = 0; index < count1; index += 500)
        {
          int count2 = Math.Min(val1, 500);
          IList<ReindexingStatusEntry> range = (IList<ReindexingStatusEntry>) reindexingStatusEntries.GetRange(index, count2);
          val1 -= count2;
          this.PrepareStoredProcedure("Search.prc_AddOrUpdateReindexingStatusEntries");
          this.BindReindexingStatusEntryLookupTable("@itemList", (IEnumerable<ReindexingStatusEntry>) range);
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.ReindexingStatusColumns(this.m_entityTypes));
            reindexingStatusEntryList.AddRange((IEnumerable<ReindexingStatusEntry>) resultCollection.GetCurrent<ReindexingStatusEntry>().Items);
          }
        }
        return reindexingStatusEntryList;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Exception occured in inserting:" + reindexingStatusEntries.Select<ReindexingStatusEntry, string>((System.Func<ReindexingStatusEntry, string>) (i => i.ToString())).Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)));
      }
    }

    private SqlParameter BindReindexingStatusEntryLookupTable(
      string parameterName,
      IEnumerable<ReindexingStatusEntry> rows,
      bool bindStatusAndPriority = true)
    {
      rows = rows ?? Enumerable.Empty<ReindexingStatusEntry>();
      System.Func<ReindexingStatusEntry, SqlDataRecord> selector = (System.Func<ReindexingStatusEntry, SqlDataRecord>) (reindexingStatusEntry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ReindexingStatusTableV3.s_reindexingStatusSqlTableEntityLookUptable);
        sqlDataRecord.SetGuid(0, reindexingStatusEntry.CollectionId);
        sqlDataRecord.SetByte(1, (byte) reindexingStatusEntry.EntityType.ID);
        if (bindStatusAndPriority)
        {
          sqlDataRecord.SetByte(2, (byte) reindexingStatusEntry.Status);
          sqlDataRecord.SetInt16(3, reindexingStatusEntry.Priority);
        }
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_ReindexingStatusDescriptor", rows.Select<ReindexingStatusEntry, SqlDataRecord>(selector));
    }
  }
}
