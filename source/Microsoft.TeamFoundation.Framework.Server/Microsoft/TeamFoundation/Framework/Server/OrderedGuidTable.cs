// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OrderedGuidTable
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
  public sealed class OrderedGuidTable : TeamFoundationTableValueParameter<Guid>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.Int)
    };
    private int m_index;

    public OrderedGuidTable(IEnumerable<Guid> rows)
      : this(rows, false)
    {
    }

    public OrderedGuidTable(IEnumerable<Guid> rows, bool omitEmptyEntries)
      : base(rows, "typ_GuidInt32Table", OrderedGuidTable.s_metadata, omitEmptyEntries)
    {
    }

    public override IEnumerator<SqlDataRecord> GetEnumerator()
    {
      OrderedGuidTable orderedGuidTable = this;
      foreach (Guid row in orderedGuidTable.m_rows)
      {
        if (!orderedGuidTable.m_omitNullEntries || row != Guid.Empty)
        {
          orderedGuidTable.m_record.SetGuid(0, row);
          orderedGuidTable.m_record.SetInt32(1, orderedGuidTable.m_index);
          yield return orderedGuidTable.m_record;
        }
        ++orderedGuidTable.m_index;
      }
    }

    public override void Reset()
    {
      base.Reset();
      this.m_index = 0;
    }

    public override void SetRecord(Guid t, SqlDataRecord record) => throw new NotImplementedException();
  }
}
