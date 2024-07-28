// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyValuePairStringTable
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
  public class KeyValuePairStringTable : 
    TeamFoundationTableValueParameter<KeyValuePair<string, string>>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };

    public KeyValuePairStringTable(IEnumerable<KeyValuePair<string, string>> strings)
      : base(strings, "typ_KeyValuePairStringTable", KeyValuePairStringTable.s_metadata)
    {
    }

    public KeyValuePairStringTable(
      IEnumerable<KeyValuePair<string, string>> strings,
      bool dummyArgument)
      : base(strings, "typ_KeyValuePairStringTableNullable", KeyValuePairStringTable.s_metadata)
    {
    }

    public override void SetRecord(KeyValuePair<string, string> strings, SqlDataRecord record)
    {
      record.SetString(0, strings.Key);
      this.SetNullableString(record, 1, strings.Value);
    }
  }
}
