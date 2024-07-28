// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.MessageTransmitTraceRecord
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class MessageTransmitTraceRecord : MessageTraceRecord
  {
    private Uri address;
    private string addressElementName;

    private MessageTransmitTraceRecord(Message message)
      : base(message)
    {
    }

    private MessageTransmitTraceRecord(Message message, string addressElementName)
      : this(message)
    {
      this.addressElementName = addressElementName;
    }

    private MessageTransmitTraceRecord(
      Message message,
      string addressElementName,
      EndpointAddress address)
      : this(message, addressElementName)
    {
      if (!(address != (EndpointAddress) null))
        return;
      this.address = address.Uri;
    }

    private MessageTransmitTraceRecord(Message message, string addressElementName, Uri uri)
      : this(message, addressElementName)
    {
      this.address = uri;
    }

    internal override string EventId => "http://schemas.microsoft.com/2006/08/ServiceModel/MessageTransmitTraceRecord";

    internal static MessageTransmitTraceRecord CreateSendTraceRecord(
      Message message,
      EndpointAddress address)
    {
      return new MessageTransmitTraceRecord(message, "RemoteAddress", address);
    }

    internal static MessageTransmitTraceRecord CreateReceiveTraceRecord(Message message, Uri uri) => new MessageTransmitTraceRecord(message, "LocalAddress", uri);

    internal static MessageTransmitTraceRecord CreateReceiveTraceRecord(
      Message message,
      EndpointAddress address)
    {
      return new MessageTransmitTraceRecord(message, "LocalAddress", address);
    }

    internal static MessageTransmitTraceRecord CreateReceiveTraceRecord(Message message) => new MessageTransmitTraceRecord(message);

    internal override void WriteTo(XmlWriter xml)
    {
      base.WriteTo(xml);
      if (!(this.address != (Uri) null))
        return;
      xml.WriteElementString(this.addressElementName, this.address.AbsoluteUri);
    }
  }
}
