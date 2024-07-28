// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StringTable
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
  public class StringTable : TeamFoundationTableValueParameter<string>
  {
    private bool m_treatNullAsEmpty;
    private int m_maxLength;
    private static readonly SqlMetaData[] s_metadataNvarchar = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_metadataVarchar = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };

    public StringTable(IEnumerable<string> strings)
      : this(strings, false, int.MaxValue)
    {
    }

    public StringTable(IEnumerable<string> strings, bool treatNullAsEmpty)
      : this(strings, treatNullAsEmpty, int.MaxValue)
    {
    }

    public StringTable(IEnumerable<string> strings, bool treatNullAsEmpty, int maxLength)
      : this(strings, treatNullAsEmpty, maxLength, true)
    {
    }

    public StringTable(
      IEnumerable<string> strings,
      bool treatNullAsEmpty,
      int maxLength,
      bool isnvarchar)
      : base(strings, isnvarchar ? "typ_StringTable" : "typ_StringVarcharTable", isnvarchar ? StringTable.s_metadataNvarchar : StringTable.s_metadataVarchar)
    {
      this.m_treatNullAsEmpty = treatNullAsEmpty;
      this.m_maxLength = maxLength;
    }

    public override void SetRecord(string st, SqlDataRecord record)
    {
      string str = string.IsNullOrEmpty(st) || st.Length <= this.m_maxLength ? st : st.Substring(0, this.m_maxLength);
      if (this.m_treatNullAsEmpty)
        this.SetNullableStringAsEmpty(record, 0, str);
      else
        this.SetNullableString(record, 0, str);
    }
  }
}
