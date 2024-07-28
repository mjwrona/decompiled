// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.EnumerableUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class EnumerableUtility
  {
    public static IEnumerable<T> AsEnumerable<T>(
      Func<int, int, IEnumerable<T>> pagingExpression,
      int take)
    {
      int skip = 0;
label_2:
      int count = 0;
      foreach (T obj in pagingExpression(skip, take))
      {
        ++skip;
        ++count;
        yield return obj;
      }
      if (count == take)
        goto label_2;
    }
  }
}
