// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsSoapMessageEncoder
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TfsSoapMessageEncoder : TfsMessageEncoder
  {
    private Encoding m_encoding;
    private bool m_useWsAddressing10;
    private XmlDictionaryReaderQuotas m_readerQuotas;
    internal const string SoapPrefix = "s";
    internal const string SoapCodeName = "Code";
    internal const string SoapBodyName = "Body";
    internal const string SoapTextName = "Text";
    internal const string SoapFaultName = "Fault";
    internal const string SoapValueName = "Value";
    internal const string SoapReasonName = "Reason";
    internal const string SoapDetailName = "Detail";
    internal const string SoapHeaderName = "Header";
    internal const string SoapSubCodeName = "Subcode";
    internal const string SoapEnvelopeName = "Envelope";
    internal const string WsAddressingPrefix = "w";
    internal const string WsAddressingToName = "To";
    internal const string WsAddressingActionName = "Action";
    internal const string MustUnderstand = "mustUnderstand";
    internal const string SoapNamespace = "http://www.w3.org/2003/05/soap-envelope";
    internal const string WsAddressing10Namespace = "http://www.w3.org/2005/08/addressing";

    public TfsSoapMessageEncoder(
      Encoding encoding,
      XmlDictionaryReaderQuotas readerQuotas,
      bool useWsAddressing10)
    {
      this.m_encoding = encoding;
      this.m_readerQuotas = readerQuotas;
      this.m_useWsAddressing10 = useWsAddressing10;
    }

    public override string ContentType => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "application/soap+xml; charset={0}", (object) this.m_encoding.WebName);

    public override bool IsContentTypeSupported(string contentType) => !string.IsNullOrEmpty(contentType) && contentType.IndexOf("application/soap+xml", StringComparison.Ordinal) >= 0;

    public override TfsMessage ReadMessage(Stream stream)
    {
      bool flag = true;
      TfsMessage tfsMessage = (TfsMessage) null;
      XmlDictionaryReader envelopeReader = (XmlDictionaryReader) null;
      try
      {
        envelopeReader = XmlDictionaryReader.CreateTextReader(stream, this.m_readerQuotas);
        tfsMessage = TfsSoapMessageEncoder.CreateMessage(envelopeReader);
        flag = false;
      }
      finally
      {
        if (flag && envelopeReader != null)
          envelopeReader.Close();
      }
      return tfsMessage;
    }

    public override TfsMessage ReadMessage(string messageString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(messageString, "message");
      bool flag = true;
      TfsMessage tfsMessage = (TfsMessage) null;
      XmlDictionaryReader envelopeReader = (XmlDictionaryReader) null;
      try
      {
        envelopeReader = XmlDictionaryReader.CreateTextReader(this.m_encoding.GetBytes(messageString), this.m_readerQuotas);
        tfsMessage = TfsSoapMessageEncoder.CreateMessage(envelopeReader);
        flag = false;
      }
      finally
      {
        if (flag && envelopeReader != null)
          envelopeReader.Close();
      }
      return tfsMessage;
    }

    public override void WriteMessage(TfsMessage message, Stream requestStream)
    {
      using (XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter(requestStream, this.m_encoding, false))
      {
        textWriter.WriteStartElement("s", "Envelope", "http://www.w3.org/2003/05/soap-envelope");
        if (this.m_useWsAddressing10 || message.Headers.Count > 0)
        {
          textWriter.WriteStartElement("s", "Header", "http://www.w3.org/2003/05/soap-envelope");
          if (this.m_useWsAddressing10)
          {
            textWriter.WriteAttributeString("xmlns", "w", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2005/08/addressing");
            textWriter.WriteStartElement("w", "Action", "http://www.w3.org/2005/08/addressing");
            textWriter.WriteString(message.Action);
            textWriter.WriteEndElement();
            textWriter.WriteStartElement("w", "To", "http://www.w3.org/2005/08/addressing");
            textWriter.WriteString(message.To.AbsoluteUri);
            textWriter.WriteEndElement();
          }
          for (int index = 0; index < message.Headers.Count; ++index)
            message.Headers[index].Write(textWriter);
          textWriter.WriteEndElement();
        }
        textWriter.WriteStartElement("s", "Body", "http://www.w3.org/2003/05/soap-envelope");
        message.WriteBodyContents(textWriter);
        textWriter.WriteEndElement();
        textWriter.WriteEndElement();
      }
    }

    private static TfsMessage CreateMessage(XmlDictionaryReader envelopeReader)
    {
      envelopeReader.MoveToElement();
      envelopeReader.ReadStartElement("Envelope", "http://www.w3.org/2003/05/soap-envelope");
      Uri to = (Uri) null;
      string action = (string) null;
      List<TfsMessageHeader> headers = new List<TfsMessageHeader>();
      if (envelopeReader.IsStartElement("Header", "http://www.w3.org/2003/05/soap-envelope"))
      {
        envelopeReader.Read();
        while (envelopeReader.IsStartElement())
        {
          if (envelopeReader.NamespaceURI == "http://www.w3.org/2005/08/addressing")
          {
            switch (envelopeReader.LocalName)
            {
              case "Action":
                action = envelopeReader.ReadElementContentAsString();
                continue;
              case "To":
                to = new Uri(envelopeReader.ReadElementContentAsString());
                continue;
              default:
                envelopeReader.Skip();
                continue;
            }
          }
          else
            headers.Add((TfsMessageHeader) new TfsSoapMessageEncoder.BufferedMessageHeader(envelopeReader.LocalName, envelopeReader.NamespaceURI, envelopeReader.ReadOuterXml()));
        }
        envelopeReader.ReadEndElement();
      }
      if (envelopeReader.IsStartElement("Body", "http://www.w3.org/2003/05/soap-envelope"))
        envelopeReader.Read();
      if (envelopeReader.IsStartElement("Fault", "http://www.w3.org/2003/05/soap-envelope"))
      {
        envelopeReader.Read();
        return TfsMessage.CreateMessage(to, action, (IList<TfsMessageHeader>) headers, (Exception) TfsSoapMessageEncoder.CreateException(envelopeReader));
      }
      if (envelopeReader.NodeType == XmlNodeType.Element)
        return TfsMessage.CreateMessage(to, action, (IList<TfsMessageHeader>) headers, envelopeReader);
      envelopeReader.Close();
      return TfsMessage.CreateMessage(to, action, (IList<TfsMessageHeader>) headers, (XmlDictionaryReader) null);
    }

    private static SoapException CreateException(XmlDictionaryReader bodyReader)
    {
      string message = (string) null;
      XmlNode detail = (XmlNode) null;
      string lang = (string) null;
      XmlQualifiedName code = (XmlQualifiedName) null;
      SoapFaultSubCode subCode = (SoapFaultSubCode) null;
      string localName = (string) null;
      string namespaceUri = (string) null;
      using (bodyReader)
      {
        while (bodyReader.NodeType == XmlNodeType.Element)
        {
          if (bodyReader.IsStartElement("Code", "http://www.w3.org/2003/05/soap-envelope"))
          {
            bodyReader.Read();
            while (bodyReader.NodeType == XmlNodeType.Element)
            {
              if (bodyReader.IsStartElement("Value", "http://www.w3.org/2003/05/soap-envelope"))
              {
                bodyReader.Read();
                bodyReader.ReadContentAsQualifiedName(out localName, out namespaceUri);
                code = new XmlQualifiedName(localName, namespaceUri);
                bodyReader.ReadEndElement();
              }
              else if (bodyReader.IsStartElement("Subcode", "http://www.w3.org/2003/05/soap-envelope"))
                subCode = TfsSoapMessageEncoder.ReadSubCode(bodyReader);
              else
                bodyReader.Skip();
            }
            bodyReader.ReadEndElement();
          }
          else if (bodyReader.IsStartElement("Reason", "http://www.w3.org/2003/05/soap-envelope"))
          {
            bodyReader.Read();
            while (bodyReader.IsStartElement("Text", "http://www.w3.org/2003/05/soap-envelope"))
            {
              while (bodyReader.MoveToNextAttribute())
              {
                if (bodyReader.LocalName == "lang")
                  lang = bodyReader.Value;
              }
              bodyReader.MoveToStartElement();
              message = bodyReader.ReadElementContentAsString();
            }
            bodyReader.ReadEndElement();
          }
          else if (TfsSoapMessageEncoder.IsDetailNode(bodyReader))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            using (XmlReader reader = bodyReader.ReadSubtree())
            {
              xmlDocument.Load(reader);
              detail = (XmlNode) xmlDocument.DocumentElement;
            }
            if (TfsSoapMessageEncoder.IsDetailNode(bodyReader))
              bodyReader.Skip();
          }
          else
            bodyReader.Skip();
        }
      }
      return new SoapException(message, code, string.Empty, string.Empty, lang, detail, subCode, (Exception) null);
    }

    private static SoapFaultSubCode ReadSubCode(XmlDictionaryReader bodyReader)
    {
      SoapFaultSubCode subCode = (SoapFaultSubCode) null;
      string localName = (string) null;
      string namespaceUri = (string) null;
      bodyReader.Read();
      while (bodyReader.IsStartElement())
      {
        switch (bodyReader.LocalName)
        {
          case "Value":
            bodyReader.Read();
            bodyReader.ReadContentAsQualifiedName(out localName, out namespaceUri);
            bodyReader.ReadEndElement();
            continue;
          case "Subcode":
            subCode = TfsSoapMessageEncoder.ReadSubCode(bodyReader);
            continue;
          default:
            bodyReader.Skip();
            continue;
        }
      }
      bodyReader.ReadEndElement();
      return !string.IsNullOrEmpty(localName) ? new SoapFaultSubCode(new XmlQualifiedName(localName, namespaceUri), subCode) : (SoapFaultSubCode) null;
    }

    private static bool IsDetailNode(XmlDictionaryReader reader)
    {
      if (reader.IsStartElement("Detail", "http://www.w3.org/2003/05/soap-envelope"))
        return true;
      return string.IsNullOrEmpty(reader.NamespaceURI) && reader.LocalName.Equals("detail", StringComparison.Ordinal);
    }

    internal sealed class BufferedMessageHeader : TfsMessageHeader
    {
      private string m_name;
      private string m_outerXml;
      private string m_namespace;

      public BufferedMessageHeader(string name, string ns, string outerXml)
      {
        this.m_name = name;
        this.m_namespace = ns;
        this.m_outerXml = outerXml;
      }

      public override string Name => this.m_name;

      public override string Namespace => this.m_namespace;

      public override XmlDictionaryReader GetReader()
      {
        byte[] bytes = TfsRequestSettings.RequestEncoding.GetBytes(this.m_outerXml);
        return XmlDictionaryReader.CreateTextReader(bytes, 0, bytes.Length, TfsRequestSettings.RequestEncoding, new XmlDictionaryReaderQuotas(), (OnXmlDictionaryReaderClose) null);
      }

      protected override void OnWriteEndHeader(XmlDictionaryWriter writer)
      {
      }

      protected override void OnWriteStartHeader(XmlDictionaryWriter writer)
      {
      }

      protected override void OnWriteHeaderContents(XmlDictionaryWriter writer) => writer.WriteRaw(this.m_outerXml);
    }
  }
}
