// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ResultCollection`2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ResultCollection<T, V> : 
    IResultCollection<V>,
    IEnumerable<V>,
    IEnumerable,
    IDisposable,
    ITfsResult,
    ITfsXmlSerializable
    where T : class, ITfsXmlSerializable, V, new()
  {
    private bool m_disposed;
    private ITfsResult m_previous;
    private bool m_complete;
    private TfsRequestContext m_context;
    private string m_xmlElement;
    private ResultCollection<T, V>.ResultSetEnumerator m_enumerator;
    private T m_current;
    private T m_last;
    private System.Collections.Generic.Queue<T> m_queue;
    private bool m_firstItem = true;

    public ResultCollection()
    {
    }

    public ResultCollection(TfsRequestContext context)
    {
      this.m_context = context;
      this.m_context.AddResultSet((ITfsResult) this);
    }

    public ResultCollection(TfsRequestContext context, string xmlElement)
      : this(context)
    {
      this.m_xmlElement = xmlElement;
    }

    public ResultCollection(TfsRequestContext context, string xmlElement, ITfsResult previous)
      : this(context, xmlElement)
    {
      this.m_previous = previous;
    }

    ~ResultCollection() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing && this.m_context != null)
        this.m_context.RemoveResultSet((ITfsResult) this);
      this.m_disposed = true;
    }

    protected void CheckDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public IEnumerator<V> GetEnumerator()
    {
      this.CheckDisposed();
      return this.Enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.CheckDisposed();
      return (IEnumerator) this.Enumerator;
    }

    protected bool MoveNext()
    {
      this.CheckDisposed();
      if (this.HasQueuedItems)
      {
        this.m_current = this.Queue.Dequeue();
      }
      else
      {
        if (!this.MoveToNextXmlElement())
          return false;
        this.m_current = this.CreateNext();
        this.m_last = this.m_current;
      }
      return true;
    }

    protected void Reset()
    {
      this.CheckDisposed();
      throw new InvalidOperationException();
    }

    protected V Current
    {
      get
      {
        this.CheckDisposed();
        return (V) this.m_current;
      }
    }

    public void ReadAll()
    {
      this.CheckDisposed();
      while (this.MoveToNextXmlElement())
      {
        T next = this.CreateNext();
        this.Queue.Enqueue(next);
        this.m_last = next;
      }
    }

    public ITfsResult Previous
    {
      get
      {
        this.CheckDisposed();
        return this.m_previous;
      }
      set
      {
        this.CheckDisposed();
        this.m_previous = value;
      }
    }

    public bool IsComplete
    {
      get
      {
        this.CheckDisposed();
        return this.m_complete;
      }
      set
      {
        this.CheckDisposed();
        this.m_complete = value;
      }
    }

    public TfsRequestContext Context
    {
      get
      {
        this.CheckDisposed();
        return this.m_context;
      }
      set
      {
        this.CheckDisposed();
        this.m_context = value;
      }
    }

    public string XmlElement
    {
      get
      {
        this.CheckDisposed();
        return this.m_xmlElement;
      }
      set
      {
        this.CheckDisposed();
        this.m_xmlElement = value;
      }
    }

    void ITfsXmlSerializable.WriteXml(XmlWriter writer, string element) => this.CheckDisposed();

    void ITfsXmlSerializable.ReadXml(XmlReader reader, string element) => this.CheckDisposed();

    private bool MoveToNextXmlElement()
    {
      if (this.IsComplete)
        return false;
      if (this.m_firstItem)
      {
        if (this.Previous != null && !this.Previous.IsComplete)
          this.Previous.ReadAll();
        if (this.Context.BodyReader.IsEmptyElement)
        {
          this.Context.BodyReader.Read();
          this.IsComplete = true;
        }
        else
        {
          if (string.IsNullOrEmpty(this.XmlElement))
            this.XmlElement = this.Context.BodyReader.Name;
          if (string.Equals(this.Context.BodyReader.Name, this.XmlElement, StringComparison.Ordinal))
            this.Context.BodyReader.Read();
          else
            this.IsComplete = true;
        }
        this.m_firstItem = false;
      }
      else
      {
        if (this.m_last is ITfsResult last && !last.IsComplete)
          last.ReadAll();
        if (this.Context.BodyReader.NodeType != XmlNodeType.Element)
        {
          this.Context.BodyReader.ReadEndElement();
          this.IsComplete = true;
        }
      }
      return !this.IsComplete;
    }

    private T CreateNext()
    {
      if (this.Context.BodyReader.HasAttributes && this.Context.BodyReader.GetAttribute("xsi:nil") == "true")
      {
        this.Context.BodyReader.Read();
        return default (T);
      }
      T next = new T();
      if (next is ITfsResult tfsResult)
        tfsResult.Context = this.Context;
      next.ReadXml((XmlReader) this.Context.BodyReader, this.Context.BodyReader.Name);
      return next;
    }

    protected System.Collections.Generic.Queue<T> Queue
    {
      get
      {
        if (this.m_queue == null)
          this.m_queue = new System.Collections.Generic.Queue<T>();
        return this.m_queue;
      }
    }

    protected bool HasQueuedItems => this.m_queue != null && this.m_queue.Count > 0;

    protected IEnumerator<V> Enumerator
    {
      get
      {
        if (this.m_enumerator == null)
          this.m_enumerator = new ResultCollection<T, V>.ResultSetEnumerator(this);
        return (IEnumerator<V>) this.m_enumerator;
      }
    }

    private class ResultSetEnumerator : IEnumerator<V>, IDisposable, IEnumerator
    {
      private ResultCollection<T, V> m_collection;

      public ResultSetEnumerator(ResultCollection<T, V> collection) => this.m_collection = collection;

      public void Dispose() => GC.SuppressFinalize((object) this);

      public bool MoveNext() => this.m_collection.MoveNext();

      public void Reset() => this.m_collection.Reset();

      public V Current => this.m_collection.Current;

      object IEnumerator.Current => (object) this.m_collection.Current;
    }
  }
}
