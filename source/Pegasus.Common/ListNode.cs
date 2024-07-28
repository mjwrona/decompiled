// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.ListNode
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pegasus.Common
{
  public static class ListNode
  {
    public static ListNode<T> Push<T>(this ListNode<T> @this, T value) => new ListNode<T>(value, @this);

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This does not expose an internal list, it is a utility method explicitly for the purpose of creating generic lists.")]
    public static List<T> ToList<T>(this ListNode<T> @this)
    {
      List<T> list = new List<T>();
      for (; @this != null; @this = @this.Tail)
        list.Add(@this.Head);
      return list;
    }
  }
}
