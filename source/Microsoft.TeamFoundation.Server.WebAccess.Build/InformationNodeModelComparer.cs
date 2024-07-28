// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.InformationNodeModelComparer
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  internal sealed class InformationNodeModelComparer : IComparer<InformationNodeModel>
  {
    int IComparer<InformationNodeModel>.Compare(InformationNodeModel x, InformationNodeModel y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null && y != null)
        return -1;
      if (x != null && y == null)
        return 1;
      DateTime x1 = x.StartTime;
      if (x1 == DateTime.MinValue)
        x1 = x.TimeStamp;
      DateTime y1 = y.StartTime;
      if (y1 == DateTime.MinValue)
        y1 = y.TimeStamp;
      if (x1 != DateTime.MinValue && y1 != DateTime.MinValue)
      {
        int num = Comparer<DateTime>.Default.Compare(x1, y1);
        if (num != 0)
          return num;
      }
      return x.Node.NodeId - y.Node.NodeId;
    }
  }
}
