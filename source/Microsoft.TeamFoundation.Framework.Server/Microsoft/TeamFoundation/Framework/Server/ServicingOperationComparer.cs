// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingOperationComparer : IComparer<ServicingOperation>
  {
    public int Compare(ServicingOperation x, ServicingOperation y)
    {
      if (x == null)
        return y != null ? -1 : 0;
      if (y == null)
        return 1;
      int num = x.Target.CompareTo((object) y.Target);
      if (num == 0)
        num = StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name);
      return num;
    }
  }
}
