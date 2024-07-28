// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsMessage
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsMessage
  {
    private Exception m_fault;
    private TfsBodyWriter m_bodyWriter;
    private XmlDictionaryReader m_bodyReader;
    private static TfsMessage s_emptyMessage = new TfsMessage((Uri) null, string.Empty, (IList<TfsMessageHeader>) null, (Exception) null);

    private TfsMessage(
      Uri to,
      string action,
      IList<TfsMessageHeader> headers,
      XmlDictionaryReader bodyReader)
    {
      this.m_bodyReader = bodyReader;
      this.Action = action;
      this.IsEmpty = bodyReader == null;
      this.Headers = new Collection<TfsMessageHeader>(headers);
      this.To = to;
    }

    private TfsMessage(Uri to, string action, IList<TfsMessageHeader> headers, Exception fault)
    {
      this.m_fault = fault;
      this.Action = action;
      this.IsEmpty = fault == null;
      this.IsFault = fault != null;
      this.To = to;
      if (headers == null)
        this.Headers = new Collection<TfsMessageHeader>();
      else
        this.Headers = new Collection<TfsMessageHeader>(headers);
    }

    private TfsMessage(string action, TfsBodyWriter bodyWriter)
    {
      this.m_bodyWriter = bodyWriter;
      this.Action = action;
      this.Headers = new Collection<TfsMessageHeader>();
    }

    public string Action { get; private set; }

    internal TfsMessageEncoder Encoder { get; private set; }

    public Collection<TfsMessageHeader> Headers { get; private set; }

    public bool IsEmpty { get; private set; }

    public bool IsFault { get; private set; }

    public Uri To { get; internal set; }

    public static TfsMessage EmptyMessage => TfsMessage.s_emptyMessage;

    public static TfsMessage CreateMessage(
      Uri to,
      string action,
      IList<TfsMessageHeader> headers,
      XmlDictionaryReader bodyReader)
    {
      return new TfsMessage(to, action, headers, bodyReader);
    }

    public static TfsMessage CreateMessage(
      Uri to,
      string action,
      IList<TfsMessageHeader> headers,
      Exception fault)
    {
      return new TfsMessage(to, action, headers, fault);
    }

    public static TfsMessage CreateMessage(string action, TfsBodyWriter bodyWriter)
    {
      ArgumentUtility.CheckForNull<TfsBodyWriter>(bodyWriter, nameof (bodyWriter));
      return new TfsMessage(action, bodyWriter);
    }

    public void Close()
    {
      this.Headers.Clear();
      if (this.m_bodyReader != null)
      {
        this.m_bodyReader.Close();
        this.m_bodyReader = (XmlDictionaryReader) null;
      }
      if (this.m_bodyWriter == null)
        return;
      this.m_bodyWriter.Close();
      this.m_bodyWriter = (TfsBodyWriter) null;
    }

    public Exception CreateException()
    {
      if (!this.IsFault)
        throw new InvalidOperationException();
      return this.m_fault;
    }

    public XmlDictionaryReader GetBodyReader()
    {
      if (this.IsFault)
        throw new InvalidOperationException();
      return this.m_bodyReader;
    }

    public void WriteBodyContents(XmlDictionaryWriter writer) => this.m_bodyWriter.WriteBodyContents(writer);
  }
}
