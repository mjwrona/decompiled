// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.RequestResult
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage
{
  [Serializable]
  public class RequestResult
  {
    private volatile Exception exception;

    public int HttpStatusCode { get; set; }

    public string HttpStatusMessage { get; internal set; }

    public string ServiceRequestID { get; internal set; }

    public string ContentMd5 { get; internal set; }

    public string ContentCrc64 { get; internal set; }

    public string Etag { get; internal set; }

    public long IngressBytes { get; set; }

    public long EgressBytes { get; set; }

    public string RequestDate { get; internal set; }

    public StorageLocation TargetLocation { get; internal set; }

    public StorageExtendedErrorInformation ExtendedErrorInformation { get; internal set; }

    public string ErrorCode { get; internal set; }

    public bool IsRequestServerEncrypted { get; internal set; }

    public bool IsServiceEncrypted { get; internal set; }

    public string EncryptionKeySHA256 { get; internal set; }

    public string EncryptionScope { get; internal set; }

    public Exception Exception
    {
      get => this.exception;
      set => this.exception = value;
    }

    public DateTime StartTime { get; internal set; }

    public DateTime EndTime { get; internal set; }

    [Obsolete("This should be available only in Microsoft.Azure.Storage.WinMD and not in Microsoft.Azure.Storage.dll. Please use ReadXML to deserialize RequestResult when Microsoft.Azure.Storage.dll is used.")]
    public static RequestResult TranslateFromExceptionMessage(string message)
    {
      RequestResult requestResult = new RequestResult();
      StringReader input = new StringReader(message);
      using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
      {
        IgnoreWhitespace = true,
        Async = true
      }))
        requestResult.ReadXmlAsync(reader).GetAwaiter().GetResult();
      return requestResult;
    }

    internal string WriteAsXml()
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      StringBuilder output = new StringBuilder();
      try
      {
        using (XmlWriter writer = XmlWriter.Create(output, settings))
          this.WriteXml(writer);
        return output.ToString();
      }
      catch (XmlException ex)
      {
        return (string) null;
      }
    }

    public async Task ReadXmlAsync(XmlReader reader)
    {
      CommonUtility.AssertNotNull(nameof (reader), (object) reader);
      int num1 = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
      if (reader.NodeType == XmlNodeType.Comment)
      {
        int num2 = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
      }
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      this.HttpStatusCode = int.Parse(await CommonUtility.ReadElementAsStringAsync("HTTPStatusCode", reader).ConfigureAwait(false), (IFormatProvider) CultureInfo.InvariantCulture);
      this.HttpStatusMessage = await CommonUtility.ReadElementAsStringAsync("HttpStatusMessage", reader).ConfigureAwait(false);
      StorageLocation result;
      if (Enum.TryParse<StorageLocation>(await CommonUtility.ReadElementAsStringAsync("TargetLocation", reader).ConfigureAwait(false), out result))
        this.TargetLocation = result;
      ConfiguredTaskAwaitable<string> configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("ServiceRequestID", reader).ConfigureAwait(false);
      this.ServiceRequestID = await configuredTaskAwaitable1;
      configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("ContentMd5", reader).ConfigureAwait(false);
      this.ContentMd5 = await configuredTaskAwaitable1;
      configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("ContentCrc64", reader).ConfigureAwait(false);
      this.ContentCrc64 = await configuredTaskAwaitable1;
      configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("Etag", reader).ConfigureAwait(false);
      this.Etag = await configuredTaskAwaitable1;
      configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("RequestDate", reader).ConfigureAwait(false);
      this.RequestDate = await configuredTaskAwaitable1;
      try
      {
        configuredTaskAwaitable1 = CommonUtility.ReadElementAsStringAsync("ErrorCode", reader).ConfigureAwait(false);
        this.ErrorCode = await configuredTaskAwaitable1;
      }
      catch (XmlException ex)
      {
      }
      ConfiguredTaskAwaitable<string> configuredTaskAwaitable2 = CommonUtility.ReadElementAsStringAsync("StartTime", reader).ConfigureAwait(false);
      this.StartTime = DateTime.Parse(await configuredTaskAwaitable2, (IFormatProvider) CultureInfo.InvariantCulture);
      configuredTaskAwaitable2 = CommonUtility.ReadElementAsStringAsync("EndTime", reader).ConfigureAwait(false);
      this.EndTime = DateTime.Parse(await configuredTaskAwaitable2, (IFormatProvider) CultureInfo.InvariantCulture);
      this.ExtendedErrorInformation = new StorageExtendedErrorInformation();
      ConfiguredTaskAwaitable configuredTaskAwaitable3 = this.ExtendedErrorInformation.ReadXmlAsync(reader, CancellationToken.None).ConfigureAwait(false);
      await configuredTaskAwaitable3;
      configuredTaskAwaitable3 = reader.ReadEndElementAsync().ConfigureAwait(false);
      await configuredTaskAwaitable3;
    }

    public void WriteXml(XmlWriter writer)
    {
      CommonUtility.AssertNotNull(nameof (writer), (object) writer);
      writer.WriteComment("An exception has occurred. For more information please deserialize this message via RequestResult.TranslateFromExceptionMessage.");
      writer.WriteStartElement(nameof (RequestResult));
      writer.WriteElementString("HTTPStatusCode", Convert.ToString(this.HttpStatusCode, (IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteElementString("HttpStatusMessage", this.HttpStatusMessage);
      writer.WriteElementString("TargetLocation", this.TargetLocation.ToString());
      writer.WriteElementString("ServiceRequestID", this.ServiceRequestID);
      writer.WriteElementString("ContentMd5", this.ContentMd5);
      writer.WriteElementString("ContentCrc64", this.ContentCrc64);
      writer.WriteElementString("Etag", this.Etag);
      writer.WriteElementString("RequestDate", this.RequestDate);
      writer.WriteElementString("ErrorCode", this.ErrorCode);
      XmlWriter xmlWriter1 = writer;
      DateTime dateTime1 = this.StartTime;
      dateTime1 = dateTime1.ToUniversalTime();
      string str1 = dateTime1.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
      xmlWriter1.WriteElementString("StartTime", str1);
      XmlWriter xmlWriter2 = writer;
      DateTime dateTime2 = this.EndTime;
      dateTime2 = dateTime2.ToUniversalTime();
      string str2 = dateTime2.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
      xmlWriter2.WriteElementString("EndTime", str2);
      if (this.ExtendedErrorInformation != null)
      {
        this.ExtendedErrorInformation.WriteXml(writer);
      }
      else
      {
        writer.WriteStartElement("Error");
        writer.WriteFullEndElement();
      }
      writer.WriteEndElement();
    }
  }
}
