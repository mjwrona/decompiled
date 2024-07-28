// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ServiceBusErrorData
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "Error", Namespace = "")]
  public class ServiceBusErrorData : IExtensibleDataObject
  {
    public const string RootTag = "Error";
    public const string HttpStatusCodeTag = "Code";
    public const string DetailTag = "Detail";
    private ExtensionDataObject extensionDataObject;

    [DataMember(Name = "Code", Order = 101, IsRequired = false, EmitDefaultValue = false)]
    public int Code { get; set; }

    [DataMember(Name = "Detail", Order = 102, IsRequired = false, EmitDefaultValue = false)]
    public string Detail { get; set; }

    public static ServiceBusErrorData GetServiceBusErrorData(HttpWebResponse webResponse)
    {
      Stream input = (Stream) null;
      ServiceBusErrorData serviceBusErrorData = new ServiceBusErrorData();
      try
      {
        input = webResponse.GetResponseStream();
      }
      catch (ProtocolViolationException ex)
      {
      }
      if (input != null)
      {
        if (input.CanSeek)
          input.Position = 0L;
        XmlReader xmlReader = XmlReader.Create(input);
        try
        {
          xmlReader.Read();
          xmlReader.ReadStartElement("Error");
          xmlReader.ReadStartElement("Code");
          serviceBusErrorData.Code = Convert.ToInt32(xmlReader.ReadString(), (IFormatProvider) CultureInfo.InvariantCulture);
          xmlReader.ReadEndElement();
          xmlReader.ReadStartElement("Detail");
          serviceBusErrorData.Detail = xmlReader.ReadString();
        }
        catch (XmlException ex)
        {
          serviceBusErrorData.Code = (int) Convert.ToInt16((object) webResponse.StatusCode, (IFormatProvider) CultureInfo.InvariantCulture);
          serviceBusErrorData.Detail = webResponse.StatusDescription;
        }
        finally
        {
          xmlReader.Close();
        }
      }
      return serviceBusErrorData;
    }

    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataObject;
      set => this.extensionDataObject = value;
    }
  }
}
