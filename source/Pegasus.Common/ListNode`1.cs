// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.ListNode`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

namespace Pegasus.Common
{
  public class ListNode<T>
  {
    public ListNode(T head, ListNode<T> tail = null)
    {
      this.Head = head;
      this.Tail = tail;
    }

    public T Head { get; }

    public ListNode<T> Tail { get; }
  }
}
