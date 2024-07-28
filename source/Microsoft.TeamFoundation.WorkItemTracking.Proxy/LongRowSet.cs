// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.LongRowSet
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.Collections;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public class LongRowSet
  {
    private Hashtable m_hash;

    public LongRowSet(IRowSet irowset)
    {
      RowSet rowSet = (RowSet) irowset;
      this.m_hash = new Hashtable((IEqualityComparer) new LongRowSetKeyComparator());
      int rowCount = rowSet.RowCount;
      for (int row = 0; row < rowCount; ++row)
      {
        int fldID = (int) rowSet[2, row];
        int itemID = (int) rowSet[1, row];
        DateTime dt = (DateTime) rowSet[0, row];
        string words = (string) rowSet[3, row];
        LongRowSetKey key = new LongRowSetKey(fldID, itemID);
        LongRowSetData longRowSetData1 = (LongRowSetData) this.m_hash[(object) key];
        if (longRowSetData1 == null)
        {
          LongRowSetData longRowSetData2 = new LongRowSetData(dt, words);
          this.m_hash[(object) key] = (object) longRowSetData2;
        }
        else if (dt > longRowSetData1.AddedDate)
        {
          LongRowSetData longRowSetData3 = new LongRowSetData(dt, words);
          this.m_hash[(object) key] = (object) longRowSetData3;
        }
      }
    }

    public string GetWords(LongRowSetKey key) => ((LongRowSetData) this.m_hash[(object) key])?.Words;
  }
}
