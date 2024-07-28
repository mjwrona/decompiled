// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.IndexingUnitWikisComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class IndexingUnitWikisComponent : SQLTable<IndexingUnitWikisEntry>
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<IndexingUnitWikisComponent>(0, true),
      (IComponentCreator) new ComponentCreator<IndexingUnitWikisComponentV1>(1),
      (IComponentCreator) new ComponentCreator<IndexingUnitWikisComponentV2>(2)
    }, "Search_IndexingUnitWikis");
    private static readonly SqlMetaData[] s_indexingUnitWikisEntryLookupTable = new SqlMetaData[2]
    {
      new SqlMetaData("IndexingUnitId", SqlDbType.Int),
      new SqlMetaData("Wikis", SqlDbType.NVarChar, -1L)
    };
    protected IndexingUnitWikisComponent.Columns m_columns = new IndexingUnitWikisComponent.Columns();
    private const string ServiceName = "Search_IndexingUnitWikis";

    public IndexingUnitWikisComponent()
      : base()
    {
    }

    public IndexingUnitWikisComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    protected SqlParameter BindRepositoryEntityLookupTable(
      string parameterName,
      IEnumerable<IndexingUnitWikisEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<IndexingUnitWikisEntry>();
      System.Func<IndexingUnitWikisEntry, SqlDataRecord> selector = (System.Func<IndexingUnitWikisEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IndexingUnitWikisComponent.s_indexingUnitWikisEntryLookupTable);
        sqlDataRecord.SetInt32(0, entry.IndexingUnitId);
        sqlDataRecord.SetString(1, entry.Wikis.Serialize<List<WikiV2>>());
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IndexingUnitWikisRecord", rows.Select<IndexingUnitWikisEntry, SqlDataRecord>(selector));
    }

    protected void ValidateIndexingUnitWikisEntry(IndexingUnitWikisEntry entry) => this.ValidateIndexingUnitId(entry.IndexingUnitId);

    protected void ValidateIndexingUnitId(int indexingUnitId)
    {
      if (indexingUnitId <= 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("IndexingUnitId should be greater than zero."));
    }

    protected class Columns : Dictionary<string, SqlColumnBinder>
    {
      public SqlColumnBinder GetIndexingUnitId = new SqlColumnBinder("IndexingUnitId");
      public SqlColumnBinder GetWikis = new SqlColumnBinder("Wikis");
    }
  }
}
