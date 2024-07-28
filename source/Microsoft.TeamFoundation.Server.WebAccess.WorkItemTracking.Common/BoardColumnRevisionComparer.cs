// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnRevisionComparer
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardColumnRevisionComparer : IComparer<BoardColumnRevision>
  {
    public int Compare(BoardColumnRevision x, BoardColumnRevision y)
    {
      if (x.ColumnType != y.ColumnType)
        return x.ColumnType - y.ColumnType;
      if (!x.Deleted && !y.Deleted)
        return x.Order - y.Order;
      if (x.Deleted && y.Deleted)
        return x.RevisedDate.Value.CompareTo(y.RevisedDate.Value);
      return x.Deleted ? 1 : -1;
    }
  }
}
