// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.SystemTrackerHeader
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class SystemTrackerHeader : MessageHeader
  {
    public SystemTrackerHeader()
      : this(string.Empty)
    {
    }

    public SystemTrackerHeader(string tracker) => this.Tracker = tracker;

    public string Tracker { get; private set; }

    public override string Name => "SystemTracker";

    public override string Namespace => "http://schemas.microsoft.com/servicebus/2010/08/protocol/";

    public static SystemTrackerHeader Read(MessageHeaders messageHeaders)
    {
      SystemTrackerHeader systemTrackerHeader;
      if (!SystemTrackerHeader.TryRead(messageHeaders, out systemTrackerHeader))
        throw new ArgumentException(SRClient.SystemTrackerHeaderMissing, nameof (messageHeaders));
      return systemTrackerHeader;
    }

    public static bool Remove(MessageHeaders messageHeaders)
    {
      if (messageHeaders != null)
      {
        int header = messageHeaders.FindHeader("SystemTracker", "http://schemas.microsoft.com/servicebus/2010/08/protocol/");
        if (header >= 0)
        {
          messageHeaders.RemoveAt(header);
          return true;
        }
      }
      return false;
    }

    public static bool TryRead(
      MessageHeaders messageHeaders,
      out SystemTrackerHeader systemTrackerHeader)
    {
      systemTrackerHeader = (SystemTrackerHeader) null;
      if (messageHeaders == null)
        return false;
      int header = messageHeaders.FindHeader("SystemTracker", "http://schemas.microsoft.com/servicebus/2010/08/protocol/");
      if (header >= 0)
      {
        using (XmlDictionaryReader readerAtHeader = messageHeaders.GetReaderAtHeader(header))
        {
          readerAtHeader.ReadStartElement();
          string tracker = readerAtHeader.ReadString();
          systemTrackerHeader = new SystemTrackerHeader(tracker);
        }
      }
      return systemTrackerHeader != null;
    }

    protected override void OnWriteHeaderContents(
      XmlDictionaryWriter writer,
      MessageVersion messageVersion)
    {
      writer.WriteString(this.Tracker.ToString());
    }
  }
}
