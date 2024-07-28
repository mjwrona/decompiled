// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Int32StringTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class Int32StringTable : WorkItemTrackingTableValueParameter<KeyValuePair<int, string>>
  {
    private static readonly SqlMetaData[] typ_Int32StringTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_Int32StringVarcharTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.VarChar, -1L)
    };
    private bool m_treatNullAsEmpty;
    private int m_maxLength;

    public Int32StringTable(IEnumerable<KeyValuePair<int, string>> values)
      : this(values, false, int.MaxValue)
    {
    }

    public Int32StringTable(IEnumerable<KeyValuePair<int, string>> values, bool treatNullAsEmpty)
      : this(values, treatNullAsEmpty, int.MaxValue)
    {
    }

    public Int32StringTable(
      IEnumerable<KeyValuePair<int, string>> values,
      bool treatNullAsEmpty,
      int maxLength)
      : this(values, treatNullAsEmpty, maxLength, true)
    {
    }

    public Int32StringTable(
      IEnumerable<KeyValuePair<int, string>> values,
      bool treatNullAsEmpty,
      int maxLength,
      bool isnvarchar)
      : base(values, isnvarchar ? "typ_KeyValuePairInt32StringTable" : "typ_KeyValuePairInt32StringVarcharTable", isnvarchar ? Int32StringTable.typ_Int32StringTable : Int32StringTable.typ_Int32StringVarcharTable)
    {
      this.m_treatNullAsEmpty = treatNullAsEmpty;
      this.m_maxLength = maxLength;
    }

    public override void SetRecord(KeyValuePair<int, string> pair, SqlDataRecord record)
    {
      string str = string.IsNullOrEmpty(pair.Value) || pair.Value.Length <= this.m_maxLength ? pair.Value : pair.Value.Substring(0, this.m_maxLength);
      record.SetInt32(0, pair.Key);
      if (this.m_treatNullAsEmpty)
      {
        if (str != null)
          record.SetString(1, str);
        else
          record.SetString(1, string.Empty);
      }
      else if (str != null)
        record.SetString(1, str);
      else
        record.SetDBNull(1);
    }
  }
}
