// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsRequestContext
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Channels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsRequestContext : IDisposable
  {
    private bool m_disposed;
    private string m_methodName;
    private TfsHttpClientBase m_proxy;
    private TfsMessage m_message;
    private List<ITfsResult> m_resultSets;
    private IClientContext m_clientContext;
    private XmlDictionaryReader m_bodyReader;

    public TfsRequestContext(
      IClientContext clientContext,
      TfsHttpClientBase proxy,
      TfsMessage message,
      XmlDictionaryReader bodyReader,
      string methodName)
    {
      this.m_proxy = proxy;
      this.m_message = message;
      this.m_bodyReader = bodyReader;
      this.m_methodName = methodName;
      this.m_clientContext = clientContext;
    }

    ~TfsRequestContext() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing)
      {
        if (this.m_resultSets != null)
        {
          foreach (ITfsResult resultSet in this.m_resultSets)
            ((IDisposable) resultSet)?.Dispose();
        }
        if (this.m_message != null)
          this.m_message.Close();
      }
      this.m_disposed = true;
    }

    protected void CheckDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public XmlDictionaryReader BodyReader
    {
      get
      {
        this.CheckDisposed();
        return this.m_bodyReader;
      }
    }

    public IClientContext ClientContext
    {
      get
      {
        this.CheckDisposed();
        return this.m_clientContext;
      }
    }

    public TfsHttpClientBase Proxy
    {
      get
      {
        this.CheckDisposed();
        return this.m_proxy;
      }
    }

    public string MethodName
    {
      get
      {
        this.CheckDisposed();
        return this.m_methodName;
      }
    }

    public void AddResultSet(ITfsResult resultSet)
    {
      this.CheckDisposed();
      this.ResultSets.Add(resultSet);
    }

    public void RemoveResultSet(ITfsResult resultSet)
    {
      this.CheckDisposed();
      if (this.m_resultSets == null)
        return;
      this.m_resultSets.Remove(resultSet);
      if (this.m_resultSets.Count != 0)
        return;
      this.Dispose();
    }

    protected List<ITfsResult> ResultSets
    {
      get
      {
        if (this.m_resultSets == null)
          this.m_resultSets = new List<ITfsResult>();
        return this.m_resultSets;
      }
    }
  }
}
