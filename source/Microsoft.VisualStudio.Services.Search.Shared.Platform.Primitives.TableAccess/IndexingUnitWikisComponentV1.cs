// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitWikisComponentV1
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitWikisComponentV1 : IndexingUnitWikisComponent
  {
    public virtual void InsertIndexingUnitWikisEntry(IndexingUnitWikisEntry indexingUnitWikisEntry)
    {
      this.ValidateNotNull<IndexingUnitWikisEntry>(nameof (indexingUnitWikisEntry), indexingUnitWikisEntry);
      this.ValidateIndexingUnitWikisEntry(indexingUnitWikisEntry);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddIndexingUnitWikisEntry");
        IList<IndexingUnitWikisEntry> rows = (IList<IndexingUnitWikisEntry>) new List<IndexingUnitWikisEntry>();
        rows.Add(indexingUnitWikisEntry);
        this.BindRepositoryEntityLookupTable("@itemList", (IEnumerable<IndexingUnitWikisEntry>) rows);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Add IndexingUnitWikis entry with IndexingUnitId {0}", (object) indexingUnitWikisEntry.IndexingUnitId));
      }
    }

    public virtual void UpdateIndexingUnitWikisEntry(IndexingUnitWikisEntry indexingUnitWikisEntry)
    {
      this.ValidateNotNull<IndexingUnitWikisEntry>(nameof (indexingUnitWikisEntry), indexingUnitWikisEntry);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateIndexingUnitWikisEntry");
        this.BindInt("@indexingUnitId", indexingUnitWikisEntry.IndexingUnitId);
        this.BindString("@wikis", indexingUnitWikisEntry.Wikis.Serialize<List<WikiV2>>(), -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Update IndexingUnitWikis entry with IndexingUnitId {0}", (object) indexingUnitWikisEntry.IndexingUnitId));
      }
    }

    public virtual IndexingUnitWikisEntry RetrieveIndexingUnitWikisEntry(int indexingUnitId)
    {
      this.ValidateIndexingUnitId(indexingUnitId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitWikisEntry");
        this.BindInt("@indexingUnitId", indexingUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitWikisEntry>((ObjectBinder<IndexingUnitWikisEntry>) new ProjectionBinder<IndexingUnitWikisEntry>((System.Func<SqlDataReader, IndexingUnitWikisEntry>) (r => new IndexingUnitWikisEntry()
          {
            IndexingUnitId = this.m_columns.GetIndexingUnitId.GetInt32((IDataReader) r),
            Wikis = JsonUtilities.Deserialize<List<WikiV2>>(this.m_columns.GetWikis.GetString((IDataReader) r, false))
          })));
          ObjectBinder<IndexingUnitWikisEntry> current = resultCollection.GetCurrent<IndexingUnitWikisEntry>();
          return current == null || current.Items == null || current.Items.Count <= 0 ? (IndexingUnitWikisEntry) null : resultCollection.GetCurrent<IndexingUnitWikisEntry>().Items[0];
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Add IndexingUnitWikis entry with IndexingUnitId {0}", (object) indexingUnitId)));
      }
    }

    public virtual IDictionary<int, IndexingUnitWikisEntry> GetIndexingUnitWikiEntries()
    {
      IDictionary<int, IndexingUnitWikisEntry> indexingUnitWikiEntries = (IDictionary<int, IndexingUnitWikisEntry>) new Dictionary<int, IndexingUnitWikisEntry>();
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryIndexingUnitWikiEntries");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitWikisEntry>((ObjectBinder<IndexingUnitWikisEntry>) new ProjectionBinder<IndexingUnitWikisEntry>((System.Func<SqlDataReader, IndexingUnitWikisEntry>) (r => new IndexingUnitWikisEntry()
          {
            IndexingUnitId = this.m_columns.GetIndexingUnitId.GetInt32((IDataReader) r),
            Wikis = JsonUtilities.Deserialize<List<WikiV2>>(this.m_columns.GetWikis.GetString((IDataReader) r, false))
          })));
          ObjectBinder<IndexingUnitWikisEntry> current = resultCollection.GetCurrent<IndexingUnitWikisEntry>();
          if (current?.Items != null)
          {
            foreach (IndexingUnitWikisEntry indexingUnitWikisEntry in current.Items)
              indexingUnitWikiEntries.Add(indexingUnitWikisEntry.IndexingUnitId, indexingUnitWikisEntry);
          }
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Get IndexingUnitWikiEntries");
      }
      return indexingUnitWikiEntries;
    }
  }
}
