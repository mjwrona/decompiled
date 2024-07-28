// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ObjectPool`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class ObjectPool<T> : IDisposable where T : new()
  {
    private Stack<T> m_pooledObjects;
    private readonly int c_maximumFreeObjects;

    public ObjectPool(int maximumFreeObjects = 50)
    {
      this.c_maximumFreeObjects = maximumFreeObjects;
      this.m_pooledObjects = new Stack<T>(this.c_maximumFreeObjects);
    }

    public void Dispose()
    {
      while (this.m_pooledObjects.Count > 0)
      {
        if (this.m_pooledObjects.Pop() is IDisposable disposable)
          disposable.Dispose();
      }
    }

    public T Next() => this.m_pooledObjects.Count <= 0 ? new T() : this.m_pooledObjects.Pop();

    public void ReturnToPool(T objectToReturn)
    {
      if (this.m_pooledObjects.Count < this.c_maximumFreeObjects)
      {
        this.m_pooledObjects.Push(objectToReturn);
      }
      else
      {
        if (!(objectToReturn is IDisposable disposable))
          return;
        disposable.Dispose();
      }
    }
  }
}
