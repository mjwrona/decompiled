// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.VersioningList`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal abstract class VersioningList<TElement> : IEnumerable<TElement>, IEnumerable
  {
    public abstract int Count { get; }

    public TElement this[int index] => (long) (uint) index < (long) this.Count ? this.IndexedElement(index) : throw new IndexOutOfRangeException();

    public static VersioningList<TElement> Create() => (VersioningList<TElement>) new VersioningList<TElement>.EmptyVersioningList();

    public abstract VersioningList<TElement> Add(TElement value);

    public VersioningList<TElement> RemoveAt(int index) => (long) (uint) index < (long) this.Count ? this.RemoveIndexedElement(index) : throw new IndexOutOfRangeException();

    public abstract IEnumerator<TElement> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    protected abstract TElement IndexedElement(int index);

    protected abstract VersioningList<TElement> RemoveIndexedElement(int index);

    internal sealed class EmptyVersioningList : VersioningList<TElement>
    {
      public override int Count => 0;

      public override VersioningList<TElement> Add(TElement value) => (VersioningList<TElement>) new VersioningList<TElement>.LinkedVersioningList((VersioningList<TElement>) this, value);

      public override IEnumerator<TElement> GetEnumerator() => (IEnumerator<TElement>) new VersioningList<TElement>.EmptyListEnumerator();

      protected override TElement IndexedElement(int index) => throw new IndexOutOfRangeException();

      protected override VersioningList<TElement> RemoveIndexedElement(int index) => throw new IndexOutOfRangeException();
    }

    internal sealed class EmptyListEnumerator : IEnumerator<TElement>, IDisposable, IEnumerator
    {
      public TElement Current => default (TElement);

      object IEnumerator.Current => (object) this.Current;

      public void Dispose()
      {
      }

      public bool MoveNext() => false;

      public void Reset()
      {
      }
    }

    internal sealed class LinkedVersioningList : VersioningList<TElement>
    {
      private readonly VersioningList<TElement> preceding;
      private readonly TElement last;

      public LinkedVersioningList(VersioningList<TElement> preceding, TElement last)
      {
        this.preceding = preceding;
        this.last = last;
      }

      public override int Count => this.preceding.Count + 1;

      public VersioningList<TElement> Preceding => this.preceding;

      public TElement Last => this.last;

      private int Depth
      {
        get
        {
          int depth = 0;
          for (VersioningList<TElement>.LinkedVersioningList linkedVersioningList = this; linkedVersioningList != null; linkedVersioningList = linkedVersioningList.Preceding as VersioningList<TElement>.LinkedVersioningList)
            ++depth;
          return depth;
        }
      }

      public override VersioningList<TElement> Add(TElement value) => this.Depth < 5 ? (VersioningList<TElement>) new VersioningList<TElement>.LinkedVersioningList((VersioningList<TElement>) this, value) : (VersioningList<TElement>) new VersioningList<TElement>.ArrayVersioningList((VersioningList<TElement>) this, value);

      public override IEnumerator<TElement> GetEnumerator() => (IEnumerator<TElement>) new VersioningList<TElement>.LinkedListEnumerator(this);

      protected override TElement IndexedElement(int index) => index == this.Count - 1 ? this.last : this.preceding.IndexedElement(index);

      protected override VersioningList<TElement> RemoveIndexedElement(int index) => index == this.Count - 1 ? this.preceding : (VersioningList<TElement>) new VersioningList<TElement>.LinkedVersioningList(this.preceding.RemoveIndexedElement(index), this.last);
    }

    internal sealed class LinkedListEnumerator : IEnumerator<TElement>, IDisposable, IEnumerator
    {
      private readonly VersioningList<TElement>.LinkedVersioningList list;
      private IEnumerator<TElement> preceding;
      private bool complete;

      public LinkedListEnumerator(VersioningList<TElement>.LinkedVersioningList list)
      {
        this.list = list;
        this.preceding = list.Preceding.GetEnumerator();
      }

      public TElement Current => this.complete ? this.list.Last : this.preceding.Current;

      object IEnumerator.Current => (object) this.Current;

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (this.complete)
          return false;
        if (!this.preceding.MoveNext())
          this.complete = true;
        return true;
      }

      public void Reset()
      {
        this.preceding.Reset();
        this.complete = false;
      }
    }

    internal sealed class ArrayVersioningList : VersioningList<TElement>
    {
      private readonly TElement[] elements;

      public ArrayVersioningList(VersioningList<TElement> preceding, TElement last)
      {
        this.elements = new TElement[preceding.Count + 1];
        int index = 0;
        foreach (TElement element in preceding)
          this.elements[index++] = element;
        this.elements[index] = last;
      }

      private ArrayVersioningList(TElement[] elements) => this.elements = elements;

      public override int Count => this.elements.Length;

      public TElement ElementAt(int index) => this.elements[index];

      public override VersioningList<TElement> Add(TElement value) => (VersioningList<TElement>) new VersioningList<TElement>.LinkedVersioningList((VersioningList<TElement>) this, value);

      public override IEnumerator<TElement> GetEnumerator() => (IEnumerator<TElement>) new VersioningList<TElement>.ArrayListEnumerator(this);

      protected override TElement IndexedElement(int index) => this.elements[index];

      protected override VersioningList<TElement> RemoveIndexedElement(int index)
      {
        if (this.elements.Length == 1)
          return (VersioningList<TElement>) new VersioningList<TElement>.EmptyVersioningList();
        int num = 0;
        TElement[] elements = new TElement[this.elements.Length - 1];
        for (int index1 = 0; index1 < this.elements.Length; ++index1)
        {
          if (index1 != index)
            elements[num++] = this.elements[index1];
        }
        return (VersioningList<TElement>) new VersioningList<TElement>.ArrayVersioningList(elements);
      }
    }

    internal sealed class ArrayListEnumerator : IEnumerator<TElement>, IDisposable, IEnumerator
    {
      private readonly VersioningList<TElement>.ArrayVersioningList array;
      private int index;

      public ArrayListEnumerator(VersioningList<TElement>.ArrayVersioningList array) => this.array = array;

      public TElement Current => this.index <= this.array.Count ? this.array.ElementAt(this.index - 1) : default (TElement);

      object IEnumerator.Current => (object) this.Current;

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        int count = this.array.Count;
        if (this.index <= count)
          ++this.index;
        return this.index <= count;
      }

      public void Reset() => this.index = 0;
    }
  }
}
