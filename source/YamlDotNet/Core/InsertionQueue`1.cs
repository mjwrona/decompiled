// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.InsertionQueue`1
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Core
{
  [Serializable]
  public class InsertionQueue<T>
  {
    private readonly IList<T> items = (IList<T>) new List<T>();

    public int Count => this.items.Count;

    public void Enqueue(T item) => this.items.Add(item);

    public T Dequeue()
    {
      if (this.Count == 0)
        throw new InvalidOperationException("The queue is empty");
      T obj = this.items[0];
      this.items.RemoveAt(0);
      return obj;
    }

    public void Insert(int index, T item) => this.items.Insert(index, item);
  }
}
