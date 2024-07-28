// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.TrackingIdHeader
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class TrackingIdHeader : MessageHeader
  {
    private string id;

    public TrackingIdHeader()
      : this(Guid.NewGuid().ToString())
    {
    }

    public TrackingIdHeader(string id) => this.id = id;

    public string Id => this.id;

    public override string Name => "TrackingId";

    public override string Namespace => "http://schemas.microsoft.com/servicebus/2010/08/protocol/";

    public static TrackingIdHeader Read(MessageHeaders messageHeaders)
    {
      TrackingIdHeader trackingIdHeader;
      if (!TrackingIdHeader.TryRead(messageHeaders, out trackingIdHeader))
        throw new ArgumentException(SRClient.TrackingIDHeaderMissing, nameof (messageHeaders));
      return trackingIdHeader;
    }

    public static bool Remove(MessageHeaders messageHeaders)
    {
      if (messageHeaders != null)
      {
        int header = messageHeaders.FindHeader("TrackingId", "http://schemas.microsoft.com/servicebus/2010/08/protocol/");
        if (header >= 0)
        {
          messageHeaders.RemoveAt(header);
          return true;
        }
      }
      return false;
    }

    public static bool TryRead(MessageHeaders messageHeaders, out TrackingIdHeader trackingIdHeader)
    {
      trackingIdHeader = (TrackingIdHeader) null;
      if (messageHeaders == null)
        return false;
      int header = messageHeaders.FindHeader("TrackingId", "http://schemas.microsoft.com/servicebus/2010/08/protocol/");
      if (header >= 0)
      {
        using (XmlDictionaryReader readerAtHeader = messageHeaders.GetReaderAtHeader(header))
        {
          readerAtHeader.ReadStartElement();
          string id = readerAtHeader.ReadString();
          trackingIdHeader = new TrackingIdHeader(id);
        }
      }
      return trackingIdHeader != null;
    }

    public static bool TryAddOrUpdate(MessageHeaders messageHeaders, string trackingId)
    {
      if (messageHeaders == null)
        return false;
      TrackingIdHeader header1 = new TrackingIdHeader(trackingId);
      int header2 = messageHeaders.FindHeader("TrackingId", "http://schemas.microsoft.com/servicebus/2010/08/protocol/");
      if (header2 >= 0)
      {
        messageHeaders.RemoveAt(header2);
        messageHeaders.Add((MessageHeader) header1);
      }
      else
        messageHeaders.Add((MessageHeader) header1);
      return true;
    }

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteString(this.id);
    }
  }
}
