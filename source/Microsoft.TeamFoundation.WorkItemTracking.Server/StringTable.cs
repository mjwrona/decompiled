// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.StringTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class StringTable : WorkItemTrackingTableValueParameter<string>
  {
    private static readonly SqlMetaData[] s_metadataNvarchar = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] s_metadataVarchar = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };
    private bool m_treatNullAsEmpty;
    private int m_maxLength;

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
      {
        if (str != null)
          record.SetString(0, str);
        else
          record.SetString(0, string.Empty);
      }
      else if (str != null)
        record.SetString(0, str);
      else
        record.SetDBNull(0);
    }
  }
}
