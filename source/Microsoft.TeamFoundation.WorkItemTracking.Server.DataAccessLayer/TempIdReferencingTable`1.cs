// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TempIdReferencingTable`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal abstract class TempIdReferencingTable<T> : WorkItemTrackingTableValueParameter<T>
  {
    protected TempIdReferencingTable(IEnumerable<T> rows, string typeName, SqlMetaData[] metadata)
      : base(rows, typeName, metadata)
    {
    }

    protected int ShiftTempId(int id)
    {
      if (id < 0)
        id += -20000;
      return id;
    }

    protected int TempIdOrRealId(int tempId, int realId) => tempId != 0 ? -tempId - 20000 : realId;
  }
}
