// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.IterationDictionaryComparer
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public sealed class IterationDictionaryComparer : IEqualityComparer<Iteration>
  {
    public static readonly IterationDictionaryComparer Instance = new IterationDictionaryComparer();

    private IterationDictionaryComparer()
    {
    }

    public bool Equals(Iteration obj1, Iteration obj2)
    {
      if (obj1 == null || obj2 == null)
        return obj1 == null && obj2 == null;
      int? id = obj1.Id;
      int num1 = id.HasValue ? 1 : 0;
      id = obj2.Id;
      int num2 = id.HasValue ? 1 : 0;
      if (num1 != num2)
        return false;
      id = obj1.Id;
      if (!id.HasValue)
        return obj1.ReviewId == obj2.ReviewId;
      id = obj1.Id;
      int num3 = id.Value;
      id = obj2.Id;
      int num4 = id.Value;
      return num3 == num4 && obj1.ReviewId == obj2.ReviewId;
    }

    public int GetHashCode(Iteration obj)
    {
      if (obj == null)
        return 0;
      int? id = obj.Id;
      int num;
      if (!id.HasValue)
      {
        num = -1;
      }
      else
      {
        id = obj.Id;
        num = id.Value + 1;
      }
      return num * 486187739 ^ obj.ReviewId * 426188617;
    }
  }
}
