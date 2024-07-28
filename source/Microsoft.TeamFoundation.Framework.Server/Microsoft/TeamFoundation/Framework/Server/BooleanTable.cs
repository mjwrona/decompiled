// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BooleanTable
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
  public class BooleanTable : TeamFoundationTableValueParameter<bool>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
    {
      new SqlMetaData("Flag", SqlDbType.Bit)
    };

    public BooleanTable(IEnumerable<bool> flags)
      : base(flags, "typ_BooleanTable", BooleanTable.s_metadata)
    {
    }

    public override void SetRecord(bool flag, SqlDataRecord record) => record.SetBoolean(0, flag);
  }
}
