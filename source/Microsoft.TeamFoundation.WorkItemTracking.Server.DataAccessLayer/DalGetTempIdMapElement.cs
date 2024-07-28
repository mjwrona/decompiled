// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetTempIdMapElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetTempIdMapElement : DalSqlElement
  {
    private Dictionary<int, int> m_tempIdMap;

    public void JoinBatch(ElementGroup group)
    {
      this.SetOutputs(1);
      this.SetGroup(group);
      this.AppendSql("select TempId, Id from ");
      this.AppendSql("@tempIdMap");
      this.AppendSql(Environment.NewLine);
    }

    internal int GetOutputId(int inputId)
    {
      if (this.m_tempIdMap == null)
      {
        this.m_tempIdMap = new Dictionary<int, int>();
        PayloadTable resultTable = this.GetResultTable();
        if (resultTable != null)
        {
          foreach (PayloadTable.PayloadRow row in resultTable.Rows)
            this.m_tempIdMap[(int) row["TempId"]] = (int) row["Id"];
        }
      }
      int num;
      return this.m_tempIdMap.TryGetValue(inputId - 20000, out num) ? num : 0;
    }
  }
}
