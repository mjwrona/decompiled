// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OrderedStringTable
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete]
  public sealed class OrderedStringTable : TeamFoundationTableValueParameter<string>
  {
    private static readonly SqlMetaData[] s_metadataNvarchar = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_metadatavarchar = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.VarChar, -1L)
    };
    private int m_index;

    public OrderedStringTable(IEnumerable<string> rows)
      : this(rows, true)
    {
    }

    public OrderedStringTable(IEnumerable<string> rows, bool nvarchar)
      : this(rows, nvarchar, false)
    {
    }

    public OrderedStringTable(IEnumerable<string> rows, bool nvarchar, bool omitNullEntries)
      : base(rows, nvarchar ? "typ_KeyValuePairInt32StringTable" : "typ_KeyValuePairInt32StringVarcharTable", nvarchar ? OrderedStringTable.s_metadataNvarchar : OrderedStringTable.s_metadatavarchar, omitNullEntries)
    {
    }

    public override IEnumerator<SqlDataRecord> GetEnumerator()
    {
      OrderedStringTable orderedStringTable = this;
      foreach (string row in orderedStringTable.m_rows)
      {
        if (!orderedStringTable.m_omitNullEntries || row != null)
        {
          orderedStringTable.m_record.SetInt32(0, orderedStringTable.m_index);
          orderedStringTable.m_record.SetString(1, row);
          yield return orderedStringTable.m_record;
        }
        ++orderedStringTable.m_index;
      }
    }

    public override void Reset()
    {
      base.Reset();
      this.m_index = 0;
    }

    public override void SetRecord(string t, SqlDataRecord record) => throw new NotImplementedException();
  }
}
