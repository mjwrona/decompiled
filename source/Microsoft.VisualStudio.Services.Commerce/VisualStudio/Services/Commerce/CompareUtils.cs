// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CompareUtils
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CompareUtils
  {
    public static bool CompareSequence<T>(
      IEnumerable<T> x,
      IEnumerable<T> y,
      IEqualityComparer<T> comp)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.SequenceEqual<T>(y, comp);
    }
  }
}
