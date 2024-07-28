// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResultCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ResultCollection : IDisposable
  {
    private bool m_disposed;
    private bool m_endOfResults;
    protected int m_currentBinderIndex;
    private IDataReader m_reader;
    protected ArrayList m_binders = new ArrayList();
    private int m_maxItemsLimit;
    private int m_rowsReadCounter;
    private string m_storedProcedureName;
    private IVssRequestContext m_requestContext;

    public ResultCollection(
      IDataReader reader,
      string storedProcedureName,
      IVssRequestContext requestContext)
      : this(reader, 0, storedProcedureName, (SqlExceptionHandler) null, requestContext)
    {
    }

    public ResultCollection(
      IDataReader reader,
      int maximumRows,
      string storedProcedureName,
      SqlExceptionHandler exceptionHandler,
      IVssRequestContext requestContext)
    {
      if (maximumRows <= 0)
        maximumRows = int.MaxValue;
      this.m_maxItemsLimit = maximumRows;
      this.m_reader = reader;
      this.m_storedProcedureName = storedProcedureName;
      this.m_requestContext = requestContext;
      this.OnSqlException += exceptionHandler;
    }

    public void NextResult()
    {
      if (!this.TryNextResult())
        throw new UnexpectedDatabaseResultException(this.m_storedProcedureName);
    }

    public virtual bool TryNextResult()
    {
      if (!this.m_endOfResults)
      {
        try
        {
          this.m_endOfResults = !this.m_reader.NextResult();
        }
        catch (SqlException ex)
        {
          this.MapException(ex);
        }
        catch (InvalidOperationException ex)
        {
          this.m_endOfResults = true;
        }
        if (!this.m_endOfResults)
          ++this.m_currentBinderIndex;
      }
      return !this.m_endOfResults;
    }

    public virtual ObjectBinder<T> GetCurrent<T>() => (ObjectBinder<T>) this.m_binders[this.m_currentBinderIndex];

    public object GetCurrent() => this.m_binders[this.m_currentBinderIndex];

    internal bool MapException(SqlException ex)
    {
      if (this.OnSqlException == null)
        return false;
      this.OnSqlException(ex, QueryExecutionState.ReturningResults);
      return true;
    }

    public void AddBinder<T>(ObjectBinder<T> binder)
    {
      binder.Initialize(this.m_reader, this.m_storedProcedureName, this);
      this.m_binders.Add((object) binder);
    }

    public void IncrementRowCounter()
    {
      ++this.m_rowsReadCounter;
      if (this.m_rowsReadCounter >= this.m_maxItemsLimit)
        throw new TooManyItemsException(this.m_maxItemsLimit);
    }

    public IVssRequestContext RequestContext => this.m_requestContext;

    public void Dispose() => this.Dispose(true);

    private void Dispose(bool disposing)
    {
      if (!this.m_disposed && disposing && this.m_reader != null)
      {
        if (!this.m_reader.IsClosed)
        {
          while (this.TryNextResult())
            ;
        }
        this.m_reader.Dispose();
      }
      this.m_disposed = true;
    }

    internal event SqlExceptionHandler OnSqlException;
  }
}
