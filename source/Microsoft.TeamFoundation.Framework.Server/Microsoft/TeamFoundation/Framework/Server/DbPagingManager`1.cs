// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DbPagingManager`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class DbPagingManager<T> : IEnumerable<T>, IEnumerable, IDisposable
  {
    private bool m_closed;
    protected XmlTextWriter m_xmlTextWriter;
    protected StringWriter m_serverItemWriter;
    protected int m_counter;
    protected int m_parameterId;
    protected LobParameterComponent m_db;
    protected List<T> m_firstPage;
    protected int m_totalCount;
    protected bool m_keepFirstPage;
    protected IVssRequestContext m_requestContext;
    protected string m_dataspaceCategory;
    protected int m_XmlParameterChunkThreshold;
    protected bool m_isPagedOverride;

    public DbPagingManager(IVssRequestContext requestContext, bool keepFirstPage)
    {
      this.m_requestContext = requestContext;
      this.m_firstPage = new List<T>();
      this.m_keepFirstPage = keepFirstPage;
      this.m_serverItemWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_xmlTextWriter = new XmlTextWriter((TextWriter) this.m_serverItemWriter);
      this.m_xmlTextWriter.WriteStartDocument();
      this.m_xmlTextWriter.WriteStartElement(this.RootElement);
      this.m_XmlParameterChunkThreshold = DbPagingManagerSettings.XmlParameterChunkThresholdSetting;
    }

    public DbPagingManager(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      bool keepFirstPage)
      : this(requestContext, keepFirstPage)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(dataspaceCategory, nameof (dataspaceCategory));
      this.m_dataspaceCategory = dataspaceCategory;
    }

    public void EnqueueAll(IEnumerable<T> items)
    {
      foreach (T obj in items)
        this.Enqueue(obj);
      this.Flush();
    }

    public virtual void Enqueue(T item)
    {
      this.WriteSingleElement(item);
      ++this.m_counter;
      ++this.m_totalCount;
      if (this.m_keepFirstPage && !this.IsPaged)
        this.m_firstPage.Add(item);
      if (this.m_counter <= this.m_XmlParameterChunkThreshold)
        return;
      this.m_xmlTextWriter.Flush();
      if (this.m_db == null)
      {
        this.m_db = this.m_requestContext.GetService<TeamFoundationResourceManagementService>().CreateComponent<LobParameterComponent>(this.m_requestContext, this.m_dataspaceCategory);
        this.m_db.ParameterId = this.m_parameterId;
      }
      this.m_db.Append(this.m_serverItemWriter.ToString());
      if (!this.IsPaged)
        this.m_parameterId = this.m_db.ParameterId;
      this.m_counter = 0;
      this.m_xmlTextWriter.Close();
      this.m_serverItemWriter.Dispose();
      this.m_serverItemWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      this.m_xmlTextWriter = new XmlTextWriter((TextWriter) this.m_serverItemWriter);
    }

    public int ParameterId => this.m_parameterId;

    public StringWriter ServerItemWriter => this.m_serverItemWriter;

    public void Flush() => this.Flush(true);

    public void Flush(bool isLastPage)
    {
      if (this.m_xmlTextWriter == null)
        return;
      if (this.IsPaged || this.m_isPagedOverride)
      {
        this.m_xmlTextWriter.Flush();
        if (isLastPage)
          this.m_serverItemWriter.Write(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "</{0}>", (object) this.RootElement));
        this.m_db.Append(this.m_serverItemWriter.ToString());
        if (!this.IsPaged)
          this.m_parameterId = this.m_db.ParameterId;
        this.m_db.Dispose();
        this.m_db = (LobParameterComponent) null;
      }
      else
      {
        this.m_xmlTextWriter.WriteEndElement();
        this.m_xmlTextWriter.WriteEndDocument();
        this.m_xmlTextWriter.Flush();
      }
      this.m_xmlTextWriter.Close();
      this.m_xmlTextWriter = (XmlTextWriter) null;
      this.m_serverItemWriter.Close();
      this.m_closed = true;
    }

    public virtual void Dispose()
    {
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (LobParameterComponent) null;
      }
      if (this.m_xmlTextWriter != null)
      {
        this.m_xmlTextWriter.Close();
        this.m_xmlTextWriter = (XmlTextWriter) null;
      }
      if (this.m_serverItemWriter != null)
      {
        this.m_serverItemWriter.Dispose();
        this.m_serverItemWriter = (StringWriter) null;
      }
      GC.SuppressFinalize((object) this);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
      if (!this.m_closed || !this.m_keepFirstPage && !this.IsPaged)
        throw new InvalidOperationException();
      return (IEnumerator<T>) new DbPagingManager<T>.PendingItemEnumerator<T>(this);
    }

    public bool IsPaged => this.m_parameterId != 0;

    public List<T> FirstPage => this.m_firstPage;

    public int TotalCount => this.m_totalCount;

    protected abstract string RootElement { get; }

    protected abstract void WriteSingleElement(T item);

    protected virtual T ReadSingleElement(XmlReader reader) => throw new NotImplementedException();

    private sealed class PendingItemEnumerator<Y> : IEnumerator<Y>, IDisposable, IEnumerator
    {
      private bool m_isValid;
      private Y m_currentItem;
      private int m_currentIndex;
      private Stream m_lobStream;
      private XmlReader m_xmlReader;
      private LobParameterComponent m_db;
      private DbPagingManager<Y> m_pagingManager;

      public PendingItemEnumerator(DbPagingManager<Y> pagingManager) => this.m_pagingManager = pagingManager;

      public void Dispose() => this.Dispose(true);

      private void Dispose(bool disposing)
      {
        if (this.m_lobStream != null)
        {
          this.m_lobStream.Dispose();
          this.m_lobStream = (Stream) null;
        }
        if (this.m_xmlReader != null)
        {
          this.m_xmlReader.Dispose();
          this.m_xmlReader = (XmlReader) null;
        }
        if (this.m_db != null)
        {
          this.m_db.Dispose();
          this.m_db = (LobParameterComponent) null;
        }
        this.m_currentIndex = 0;
      }

      public Y Current
      {
        get
        {
          if (!this.m_isValid)
            throw new InvalidOperationException();
          return this.m_currentItem;
        }
      }

      object IEnumerator.Current => (object) this.Current;

      public bool MoveNext()
      {
        this.m_isValid = true;
        if (!this.m_pagingManager.IsPaged)
        {
          if (this.m_currentIndex >= this.m_pagingManager.FirstPage.Count)
            return false;
          this.m_currentItem = this.m_pagingManager.FirstPage[this.m_currentIndex];
          ++this.m_currentIndex;
          return true;
        }
        if (this.m_db == null)
        {
          this.m_db = this.m_pagingManager.m_requestContext.GetService<TeamFoundationResourceManagementService>().CreateComponent<LobParameterComponent>(this.m_pagingManager.m_requestContext, this.m_pagingManager.m_dataspaceCategory);
          this.m_lobStream = this.m_db.GetLobParameter(this.m_pagingManager.ParameterId);
          if (this.m_lobStream == null)
            return false;
          this.m_xmlReader = XmlReader.Create(this.m_lobStream, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          });
          do
            ;
          while (this.m_xmlReader.Read() && this.m_xmlReader.NodeType == XmlNodeType.XmlDeclaration);
          if (!this.m_xmlReader.Name.Equals(this.m_pagingManager.RootElement, StringComparison.Ordinal) || this.m_xmlReader.IsEmptyElement || !this.m_xmlReader.Read())
            return false;
        }
        if (this.m_xmlReader.NodeType == XmlNodeType.None || this.m_xmlReader.NodeType == XmlNodeType.EndElement && this.m_xmlReader.Name.Equals(this.m_pagingManager.RootElement, StringComparison.Ordinal))
          return false;
        this.m_currentItem = this.m_pagingManager.ReadSingleElement(this.m_xmlReader);
        this.m_xmlReader.Read();
        return true;
      }

      public void Reset() => this.Dispose(false);
    }
  }
}
