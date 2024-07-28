// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MethodTimeComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MethodTimeComparer : IComparer<MethodTime>
  {
    public int Compare(MethodTime a, MethodTime b)
    {
      int num = b.Time.CompareTo(a.Time);
      if (num == 0)
        num = b.LogicalReads.CompareTo(a.LogicalReads);
      if (num == 0)
        num = a.Name.CompareTo(b.Name);
      return num;
    }
  }
}
