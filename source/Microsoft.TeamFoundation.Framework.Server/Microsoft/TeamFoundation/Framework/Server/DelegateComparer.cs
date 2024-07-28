// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DelegateComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DelegateComparer : IEqualityComparer<Delegate>
  {
    public static readonly DelegateComparer Instance = new DelegateComparer();

    public bool Equals(Delegate x, Delegate y)
    {
      if ((object) x == (object) y)
        return true;
      return !((object) x == null ^ (object) y == null) && x.Target == y.Target && x.Method == y.Method;
    }

    public int GetHashCode(Delegate obj) => obj.Target != null ? obj.Method.GetHashCode() ^ obj.Target.GetHashCode() : obj.Method.GetHashCode();
  }
}
