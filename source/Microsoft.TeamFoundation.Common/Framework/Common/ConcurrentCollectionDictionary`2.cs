// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ConcurrentCollectionDictionary`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ConcurrentCollectionDictionary<TKey, TElement> : ICollectionDictionary<TKey, TElement>
  {
    private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
    private ConcurrentCollectionDictionary<TKey, TElement>.CreateCollectionDelegate m_createCollectionHandler;
    private ConcurrentCollectionDictionary<TKey, TElement>.CopyCollectionDelegate m_copyCollectionHandler;
    private bool m_removeKeyOnEmpty;
    private Dictionary<TKey, ICollection<TElement>> m_innerDictionary;

    public ConcurrentCollectionDictionary(
      ConcurrentCollectionDictionary<TKey, TElement>.CreateCollectionDelegate createCollectionHandler,
      ConcurrentCollectionDictionary<TKey, TElement>.CopyCollectionDelegate copyCollectionHandler = null,
      IEqualityComparer<TKey> comparer = null,
      bool removeKeyOnEmpty = true)
    {
      this.m_innerDictionary = new Dictionary<TKey, ICollection<TElement>>(comparer);
      this.m_createCollectionHandler = createCollectionHandler;
      this.m_copyCollectionHandler = copyCollectionHandler;
      this.m_removeKeyOnEmpty = removeKeyOnEmpty;
    }

    public void AddElement(TKey key, TElement element)
    {
      this.m_lock.EnterWriteLock();
      try
      {
        ICollection<TElement> elements;
        if (!this.m_innerDictionary.TryGetValue(key, out elements))
        {
          elements = this.m_createCollectionHandler();
          this.m_innerDictionary.Add(key, elements);
        }
        if (this.m_copyCollectionHandler != null)
        {
          elements.Add(element);
        }
        else
        {
          lock (elements)
            elements.Add(element);
        }
      }
      finally
      {
        this.m_lock.ExitWriteLock();
      }
    }

    public bool RemoveElement(TKey key, TElement element)
    {
      this.m_lock.EnterWriteLock();
      try
      {
        ICollection<TElement> elements;
        if (!this.m_innerDictionary.TryGetValue(key, out elements))
          return false;
        int num = -1;
        bool flag;
        if (this.m_copyCollectionHandler != null)
        {
          flag = elements.Remove(element);
          if (flag)
            num = elements.Count;
        }
        else
        {
          lock (elements)
          {
            flag = elements.Remove(element);
            if (flag)
              num = elements.Count;
          }
        }
        if (this.m_removeKeyOnEmpty & flag && num == 0)
          this.m_innerDictionary.Remove(key);
        return flag;
      }
      finally
      {
        this.m_lock.ExitWriteLock();
      }
    }

    public bool Remove(TKey key)
    {
      this.m_lock.EnterWriteLock();
      try
      {
        return this.m_innerDictionary.Remove(key);
      }
      finally
      {
        this.m_lock.ExitWriteLock();
      }
    }

    public void Clear()
    {
      this.m_lock.EnterWriteLock();
      try
      {
        this.m_innerDictionary.Clear();
      }
      finally
      {
        this.m_lock.ExitWriteLock();
      }
    }

    public bool TryGetValue(TKey key, out ICollection<TElement> collection)
    {
      this.m_lock.EnterReadLock();
      try
      {
        ICollection<TElement> collection1;
        if (!this.m_innerDictionary.TryGetValue(key, out collection1))
        {
          collection = (ICollection<TElement>) null;
          return false;
        }
        collection = this.m_copyCollectionHandler == null ? collection1 : this.m_copyCollectionHandler(collection1);
        return true;
      }
      finally
      {
        this.m_lock.ExitReadLock();
      }
    }

    public bool TryGetValue<TCollection>(TKey key, out TCollection collection) where TCollection : ICollection<TElement>
    {
      ICollection<TElement> collection1;
      int num = this.TryGetValue(key, out collection1) ? 1 : 0;
      collection = (TCollection) collection1;
      return num != 0;
    }

    public bool ContainsElement(TKey key, TElement element)
    {
      this.m_lock.EnterReadLock();
      try
      {
        ICollection<TElement> elements;
        if (!this.m_innerDictionary.TryGetValue(key, out elements))
          return false;
        if (this.m_copyCollectionHandler != null)
          return elements.Contains(element);
        lock (elements)
          return elements.Contains(element);
      }
      finally
      {
        this.m_lock.ExitReadLock();
      }
    }

    public bool ContainsKey(TKey key)
    {
      this.m_lock.EnterReadLock();
      try
      {
        return this.m_innerDictionary.ContainsKey(key);
      }
      finally
      {
        this.m_lock.ExitReadLock();
      }
    }

    public int Count
    {
      get
      {
        this.m_lock.EnterReadLock();
        try
        {
          return this.m_innerDictionary.Count;
        }
        finally
        {
          this.m_lock.ExitReadLock();
        }
      }
    }

    public ICollection<TKey> Keys
    {
      get
      {
        this.m_lock.EnterReadLock();
        try
        {
          return (ICollection<TKey>) new List<TKey>((IEnumerable<TKey>) this.m_innerDictionary.Keys);
        }
        finally
        {
          this.m_lock.ExitReadLock();
        }
      }
    }

    public delegate ICollection<TElement> CreateCollectionDelegate();

    public delegate ICollection<TElement> CopyCollectionDelegate(ICollection<TElement> collection);
  }
}
