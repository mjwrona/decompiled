// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Int32StringTable
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
  public sealed class Int32StringTable : TeamFoundationTableValueParameter<Tuple<int, string>>
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

    public Int32StringTable(IEnumerable<Tuple<int, string>> rows)
      : this(rows, true)
    {
    }

    public Int32StringTable(IEnumerable<Tuple<int, string>> rows, bool nvarchar)
      : base(rows, nvarchar ? "typ_KeyValuePairInt32StringTable" : "typ_KeyValuePairInt32StringVarcharTable", nvarchar ? Int32StringTable.s_metadataNvarchar : Int32StringTable.s_metadatavarchar)
    {
    }

    public override void SetRecord(Tuple<int, string> data, SqlDataRecord record)
    {
      record.SetInt32(0, data.Item1);
      record.SetString(1, data.Item2);
    }
  }
}
