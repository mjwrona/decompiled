// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.FakeList`1
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
  public class FakeList<T>
  {
    private readonly IEnumerator<T> collection;
    private int currentIndex = -1;

    public FakeList(IEnumerator<T> collection) => this.collection = collection;

    public FakeList(IEnumerable<T> collection)
      : this(collection.GetEnumerator())
    {
    }

    public T this[int index]
    {
      get
      {
        if (index < this.currentIndex)
        {
          this.collection.Reset();
          this.currentIndex = -1;
        }
        for (; this.currentIndex < index; ++this.currentIndex)
        {
          if (!this.collection.MoveNext())
            throw new ArgumentOutOfRangeException(nameof (index));
        }
        return this.collection.Current;
      }
    }
  }
}
