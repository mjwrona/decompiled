// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.AmqpConstants
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing;
using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal static class AmqpConstants
  {
    public const string Apache = "apache.org";
    public const string Vendor = "com.microsoft";
    public const string SchemeAmqp = "amqp";
    public const string SchemeAmqps = "amqps";
    public const string TimeSpanName = "com.microsoft:timespan";
    public const string UriName = "com.microsoft:uri";
    public const string DateTimeOffsetName = "com.microsoft:datetime-offset";
    public const string OpenErrorName = "com.microsoft:open-error";
    public const string BadCommand = "BadCommand";
    public const string AddRule = "AddRule";
    public const string DeleteRule = "DeleteRule";
    public const string Publish = "Publish";
    public const string Consume = "Consume";
    public const string Dispose = "Dispose";
    public static readonly AmqpSymbol SimpleWebTokenPropertyName = (AmqpSymbol) "com.microsoft:swt";
    public static readonly AmqpSymbol ContainerId = (AmqpSymbol) "container-id";
    public static readonly AmqpSymbol ConnectionId = (AmqpSymbol) "connection-id";
    public static readonly AmqpSymbol LinkName = (AmqpSymbol) "link-name";
    public static readonly AmqpSymbol ClientMaxFrameSize = (AmqpSymbol) "client-max-frame-size";
    public static readonly AmqpSymbol HostName = (AmqpSymbol) "hostname";
    public static readonly AmqpSymbol NetworkHost = (AmqpSymbol) "network-host";
    public static readonly AmqpSymbol Port = (AmqpSymbol) "port";
    public static readonly AmqpSymbol Address = (AmqpSymbol) "address";
    public static readonly AmqpSymbol PublisherId = (AmqpSymbol) "publisher-id";
    public static readonly ArraySegment<byte> NullBinary = new ArraySegment<byte>();
    public static readonly ArraySegment<byte> EmptyBinary = new ArraySegment<byte>(new byte[0]);
    public static readonly AmqpVersion DefaultProtocolVersion = new AmqpVersion((byte) 1, (byte) 0, (byte) 0);
    public static readonly DateTime StartOfEpoch = DateTime.Parse("1970-01-01T00:00:00.0000000Z", (IFormatProvider) CultureInfo.InvariantCulture).ToUniversalTime();
    public static readonly DateTime MaxAbsoluteExpiryTime = DateTime.MaxValue.ToUniversalTime() - TimeSpan.FromDays(1.0);
    public static readonly Accepted AcceptedOutcome = new Accepted();
    public static readonly Released ReleasedOutcome = new Released();
    public static readonly Rejected RejectedOutcome = new Rejected();
    public static readonly Rejected RejectedNotFoundOutcome = new Rejected()
    {
      Error = AmqpError.NotFound
    };
    public const int DefaultPort = 5672;
    public const int DefaultSecurePort = 5671;
    public const int ProtocolHeaderSize = 8;
    public const int TransportBufferSize = 65536;
    public const uint AmqpMessageFormat = 0;
    public const int MinMaxFrameSize = 512;
    public const uint DefaultMaxFrameSize = 65536;
    public const ushort DefaultMaxConcurrentChannels = 10000;
    public const ushort DefaultMaxLinkHandles = 255;
    public const uint DefaultHeartBeatInterval = 90000;
    public const uint MinimumHeartBeatIntervalMs = 60000;
    public const int DefaultTimeout = 60;
    public const int DefaultTryCloseTimeout = 15;
    public const uint DefaultWindowSize = 5000;
    public const uint DefaultLinkCredit = 1000;
    public const uint DefaultNextTransferId = 1;
    public const int DefaultDispositionTimeout = 20;
    public const int SegmentSize = 512;
    public const byte AmqpFormat = 1;
  }
}
