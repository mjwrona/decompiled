// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.PlainXmlWriter
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class PlainXmlWriter : XmlWriter
  {
    private TraceXPathNavigator navigator;
    private bool writingAttribute;
    private string currentAttributeName;
    private string currentAttributePrefix;
    private string currentAttributeNs;
    private string currentAttributeText = string.Empty;

    public PlainXmlWriter()
      : this(-1)
    {
    }

    public PlainXmlWriter(int maxSize) => this.navigator = new TraceXPathNavigator(maxSize);

    public TraceXPathNavigator Navigator => this.navigator;

    public override void WriteStartDocument()
    {
    }

    public override void WriteStartDocument(bool standalone)
    {
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
    }

    public override void WriteEndDocument()
    {
    }

    public override string LookupPrefix(string ns) => this.navigator.LookupPrefix(ns);

    public override WriteState WriteState => this.navigator.WriteState;

    public override XmlSpace XmlSpace => XmlSpace.Default;

    public override string XmlLang => string.Empty;

    public override void WriteValue(object value) => this.navigator.AddText(value.ToString());

    public override void WriteValue(string value) => this.navigator.AddText(value);

    public override void WriteBase64(byte[] buffer, int offset, int count)
    {
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (string.IsNullOrEmpty(localName))
        throw new ArgumentNullException(nameof (localName));
      this.navigator.AddElement(prefix, localName, ns);
    }

    public override void WriteFullEndElement() => this.WriteEndElement();

    public override void WriteEndElement() => this.navigator.CloseElement();

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (this.writingAttribute)
        throw new InvalidOperationException();
      this.currentAttributeName = localName;
      this.currentAttributePrefix = prefix;
      this.currentAttributeNs = ns;
      this.currentAttributeText = string.Empty;
      this.writingAttribute = true;
    }

    public override void WriteEndAttribute()
    {
      if (!this.writingAttribute)
        throw new InvalidOperationException();
      this.navigator.AddAttribute(this.currentAttributeName, this.currentAttributeText, this.currentAttributeNs, this.currentAttributePrefix);
      this.writingAttribute = false;
    }

    public override void WriteCData(string text) => this.WriteRaw("<![CDATA[" + text + "]]>");

    public override void WriteComment(string text) => this.navigator.AddComment(text);

    public override void WriteProcessingInstruction(string name, string text) => this.navigator.AddProcessingInstruction(name, text);

    public override void WriteEntityRef(string name)
    {
    }

    public override void WriteCharEntity(char ch)
    {
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
    }

    public override void WriteWhitespace(string ws)
    {
    }

    public override void WriteString(string text)
    {
      if (this.writingAttribute)
        this.currentAttributeText += text;
      else
        this.WriteValue(text);
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (buffer.Length - index < count)
        throw new ArgumentException(Microsoft.Azure.NotificationHubs.SR.GetString(Resources.WriteCharsInvalidContent));
      this.WriteString(new string(buffer, index, count));
    }

    public override void WriteRaw(string data) => this.WriteString(data);

    public override void WriteRaw(char[] buffer, int index, int count) => this.WriteChars(buffer, index, count);

    public override void Close()
    {
    }

    public override void Flush()
    {
    }

    [Serializable]
    internal class MaxSizeExceededException : Exception
    {
      public MaxSizeExceededException()
      {
      }

      public MaxSizeExceededException(string message)
        : base(message)
      {
      }

      public MaxSizeExceededException(string message, Exception inner)
        : base(message, inner)
      {
      }

      protected MaxSizeExceededException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }
  }
}
