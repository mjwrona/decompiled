// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GuidStringTable
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
  public class GuidStringTable : TeamFoundationTableValueParameter<Tuple<Guid, string>>
  {
    private static readonly SqlMetaData[] s_metadataNvarchar = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_metadatavarchar = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.VarChar, -1L)
    };

    public GuidStringTable(IEnumerable<Tuple<Guid, string>> rows)
      : this(rows, true)
    {
    }

    public GuidStringTable(IEnumerable<Tuple<Guid, string>> rows, bool nvarchar)
      : base(rows, nvarchar ? "typ_GuidStringTable" : "typ_GuidStringVarcharTable", nvarchar ? GuidStringTable.s_metadataNvarchar : GuidStringTable.s_metadatavarchar)
    {
    }

    public override void SetRecord(Tuple<Guid, string> value, SqlDataRecord record)
    {
      record.SetGuid(0, value.Item1);
      record.SetString(1, value.Item2);
    }
  }
}
