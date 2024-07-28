// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ChangesExtension
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public static class ChangesExtension
  {
    public static List<ChangeEntry> GetSortedChangeList(List<ChangeEntry> changesToSort)
    {
      List<ChangeEntry> list = changesToSort.ToList<ChangeEntry>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      list.Sort(ChangesExtension.\u003C\u003EO.\u003C0\u003E__FirstAlphabeticalCompare ?? (ChangesExtension.\u003C\u003EO.\u003C0\u003E__FirstAlphabeticalCompare = new Comparison<ChangeEntry>(ChangesExtension.FirstAlphabeticalCompare)));
      return list;
    }

    private static int FirstAlphabeticalCompare(ChangeEntry x, ChangeEntry y)
    {
      if (x == null)
        return y == null ? 0 : -1;
      if (y == null)
        return 1;
      if (x.Modified == null && y.Modified != null)
        return -1;
      return x.Modified != null && y.Modified == null ? 1 : string.Compare(x.Modified.Path, y.Modified.Path, StringComparison.OrdinalIgnoreCase);
    }
  }
}
