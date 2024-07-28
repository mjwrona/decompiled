// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.ResponseParsingBase`1
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal abstract class ResponseParsingBase<T> : IDisposable
  {
    protected IList<T> outstandingObjectsToParse = (IList<T>) new List<T>();
    protected bool allObjectsParsed;
    protected XmlReader reader;
    private IEnumerator<T> parser;
    private bool enumerableConsumed;

    protected ResponseParsingBase(Stream stream)
    {
      this.reader = XmlReader.Create(stream, new XmlReaderSettings()
      {
        IgnoreWhitespace = false
      });
      this.parser = this.ParseXmlAndClose().GetEnumerator();
    }

    protected IEnumerable<T> ObjectsToParse
    {
      get
      {
        this.enumerableConsumed = !this.enumerableConsumed ? true : throw new InvalidOperationException("Resource consumed");
        while (!this.allObjectsParsed && this.parser.MoveNext())
        {
          if ((object) this.parser.Current != null)
            yield return this.parser.Current;
        }
        foreach (T obj in (IEnumerable<T>) this.outstandingObjectsToParse)
          yield return obj;
        this.outstandingObjectsToParse = (IList<T>) null;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected abstract IEnumerable<T> ParseXml();

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.reader != null)
        this.reader.Close();
      this.reader = (XmlReader) null;
    }

    protected void Variable(ref bool consumable)
    {
      if (consumable)
        return;
      while (this.parser.MoveNext())
      {
        if ((object) this.parser.Current != null)
          this.outstandingObjectsToParse.Add(this.parser.Current);
        if (consumable)
          break;
      }
    }

    private IEnumerable<T> ParseXmlAndClose()
    {
      foreach (T obj in this.ParseXml())
        yield return obj;
      this.reader.Close();
      this.reader = (XmlReader) null;
    }
  }
}
