// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Int32Table
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
  public class Int32Table : TeamFoundationTableValueParameter<int>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };

    public Int32Table(IEnumerable<int> ints)
      : base(ints, "typ_Int32Table", Int32Table.s_metadata)
    {
    }

    public Int32Table(IEnumerable<int> ints, string tableName)
      : base(ints, tableName, Int32Table.s_metadata)
    {
    }

    public override void SetRecord(int val, SqlDataRecord record) => this.m_record.SetInt32(0, val);
  }
}
