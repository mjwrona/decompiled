// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssSoapMediaTypeFormatter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal sealed class VssSoapMediaTypeFormatter : MediaTypeFormatter
  {
    public VssSoapMediaTypeFormatter(
      string bodyName,
      string resultName,
      string soapAction,
      string soapNamespace)
    {
      this.BodyName = bodyName;
      this.ResultName = resultName;
      this.SoapAction = soapAction;
      this.SoapNamespace = soapNamespace;
      this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/soap+xml"));
    }

    private string BodyName { get; set; }

    private string ResultName { get; set; }

    private string SoapAction { get; set; }

    private string SoapNamespace { get; set; }

    public override bool CanReadType(Type type) => type.GetMethod("FromXml", BindingFlags.Static | BindingFlags.NonPublic, (Binder) null, new Type[2]
    {
      typeof (IServiceProvider),
      typeof (XmlReader)
    }, (ParameterModifier[]) null) != (MethodInfo) null;

    public override bool CanWriteType(Type type) => type.GetMethod("ToXml", BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[1]
    {
      typeof (XmlDictionaryWriter)
    }, (ParameterModifier[]) null) != (MethodInfo) null;

    public override Task<object> ReadFromStreamAsync(
      Type type,
      Stream readStream,
      HttpContent content,
      IFormatterLogger formatterLogger)
    {
      MethodInfo method = type.GetMethod("FromXml", BindingFlags.Static | BindingFlags.NonPublic, (Binder) null, new Type[2]
      {
        typeof (IServiceProvider),
        typeof (XmlReader)
      }, (ParameterModifier[]) null);
      using (XmlDictionaryReader textReader = XmlDictionaryReader.CreateTextReader(readStream, XmlDictionaryReaderQuotas.Max))
      {
        textReader.MoveToElement();
        textReader.ReadStartElement("Envelope", "http://www.w3.org/2003/05/soap-envelope");
        if (textReader.IsStartElement("Header", "http://www.w3.org/2003/05/soap-envelope"))
          textReader.ReadOuterXml();
        if (textReader.IsStartElement("Body", "http://www.w3.org/2003/05/soap-envelope"))
          textReader.Read();
        if (textReader.IsStartElement("Fault", "http://www.w3.org/2003/05/soap-envelope"))
        {
          textReader.Read();
          return Task.FromResult<object>((object) null);
        }
        if (textReader.NodeType == XmlNodeType.Element)
        {
          int num = textReader.IsEmptyElement ? 1 : 0;
          textReader.Read();
          if (num != 0 || string.IsNullOrEmpty(this.ResultName) || !string.Equals(textReader.LocalName, this.ResultName, StringComparison.Ordinal) || !string.Equals(textReader.NamespaceURI, this.SoapNamespace, StringComparison.Ordinal))
            return Task.FromResult<object>((object) null);
          return Task.FromResult<object>(method.Invoke((object) null, new object[2]
          {
            null,
            (object) textReader
          }));
        }
        textReader.Close();
        return Task.FromResult<object>((object) null);
      }
    }

    public override Task WriteToStreamAsync(
      Type type,
      object value,
      Stream writeStream,
      HttpContent content,
      TransportContext transportContext)
    {
      MethodInfo method = type.GetMethod("ToXml", BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[1]
      {
        typeof (XmlDictionaryWriter)
      }, (ParameterModifier[]) null);
      using (XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter(writeStream, VssHttpRequestSettings.Encoding, false))
      {
        textWriter.WriteStartElement("s", "Envelope", "http://www.w3.org/2003/05/soap-envelope");
        textWriter.WriteStartElement("s", "Body", "http://www.w3.org/2003/05/soap-envelope");
        textWriter.WriteStartElement(this.BodyName, this.SoapNamespace);
        method.Invoke(value, new object[1]
        {
          (object) textWriter
        });
        textWriter.WriteEndElement();
        textWriter.WriteEndElement();
        textWriter.WriteEndElement();
      }
      return (Task) Task.FromResult<object>((object) null);
    }
  }
}
