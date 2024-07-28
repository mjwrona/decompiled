// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ObjectBinder`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ObjectBinder<T> : IEnumerable<T>, IEnumerable
  {
    private string m_procedureName;
    private ObjectBinder<T>.ObjectEnumerator m_enumerator;

    protected ObjectBinder()
    {
    }

    protected ObjectBinder(SqlDataReader reader, string procedureName)
      : this()
    {
      this.m_enumerator = new ObjectBinder<T>.ObjectEnumerator(this, (IDataReader) reader, (ResultCollection) null);
      this.m_procedureName = procedureName;
    }

    public virtual IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.m_enumerator;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public bool MoveNext() => this.m_enumerator.MoveNext();

    public T Current => this.m_enumerator.Current;

    protected abstract T Bind();

    public virtual List<T> Items => this.m_enumerator.Items;

    internal void Initialize(
      IDataReader reader,
      string procedureName,
      ResultCollection resultCollection)
    {
      if (resultCollection == null)
        throw new ArgumentNullException("rc");
      this.m_enumerator = reader != null ? new ObjectBinder<T>.ObjectEnumerator(this, reader, resultCollection) : throw new ArgumentNullException(nameof (reader));
      this.m_procedureName = procedureName;
    }

    protected string ProcedureName => this.m_procedureName;

    protected SqlDataReader Reader => this.m_enumerator.Reader as SqlDataReader;

    protected IDataReader BaseReader => this.m_enumerator.Reader;

    private class ObjectEnumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private T m_current;
      private List<T> m_Items;
      private bool m_forwarded;
      private bool m_endOfReader;
      private ObjectBinder<T> m_binder;
      private IDataReader m_reader;
      private ResultCollection m_resultCollection;

      public ObjectEnumerator(
        ObjectBinder<T> binder,
        IDataReader reader,
        ResultCollection resultCollection)
      {
        this.m_binder = binder;
        this.m_reader = reader;
        this.m_resultCollection = resultCollection;
      }

      public void Dispose()
      {
      }

      public T Current => this.m_current;

      object IEnumerator.Current => (object) this.m_current;

      public bool MoveNext()
      {
        if (this.m_resultCollection != null && this.m_resultCollection.RequestContext != null && this.m_resultCollection.RequestContext is VssRequestContext)
          this.m_resultCollection.RequestContext.RequestContextInternal().CheckCanceled();
        if (!this.m_endOfReader)
        {
          try
          {
            this.m_endOfReader = !this.m_reader.Read();
            this.m_forwarded = true;
          }
          catch (SqlException ex)
          {
            bool flag = false;
            if (this.m_resultCollection != null)
              flag = this.m_resultCollection.MapException(ex);
            if (!flag)
              throw;
          }
          if (!this.m_endOfReader)
            this.m_current = this.m_binder.Bind();
        }
        else
          this.m_current = default (T);
        return !this.m_endOfReader;
      }

      public void Reset() => throw new NotImplementedException();

      internal List<T> Items
      {
        get
        {
          if (this.m_Items == null)
          {
            this.m_Items = new List<T>();
            while (this.MoveNext())
            {
              this.m_Items.Add(this.Current);
              if (this.m_resultCollection != null)
                this.m_resultCollection.IncrementRowCounter();
            }
          }
          return this.m_Items;
        }
      }

      internal IDataReader Reader => this.m_reader;
    }
  }
}
