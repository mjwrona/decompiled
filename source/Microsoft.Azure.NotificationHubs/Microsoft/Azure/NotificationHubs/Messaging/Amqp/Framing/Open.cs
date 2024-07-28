// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Open
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal class Open : Performative
  {
    public static readonly string Name = "amqp:open:list";
    public static readonly ulong Code = 16;
    private const int Fields = 10;

    public Open()
      : base((AmqpSymbol) Open.Name, Open.Code)
    {
    }

    public string ContainerId { get; set; }

    public string HostName { get; set; }

    public uint? MaxFrameSize { get; set; }

    public ushort? ChannelMax { get; set; }

    public uint? IdleTimeOut { get; set; }

    public Multiple<AmqpSymbol> OutgoingLocales { get; set; }

    public Multiple<AmqpSymbol> IncomingLocales { get; set; }

    public Multiple<AmqpSymbol> OfferedCapabilities { get; set; }

    public Multiple<AmqpSymbol> DesiredCapabilities { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields Properties { get; set; }

    protected override int FieldCount => 10;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("open(");
      int count = 0;
      this.AddFieldToString(this.ContainerId != null, sb, "container-id", (object) this.ContainerId, ref count);
      this.AddFieldToString(this.HostName != null, sb, "host-name", (object) this.HostName, ref count);
      this.AddFieldToString(this.MaxFrameSize.HasValue, sb, "max-frame-size", (object) this.MaxFrameSize, ref count);
      this.AddFieldToString(this.ChannelMax.HasValue, sb, "channel-max", (object) this.ChannelMax, ref count);
      this.AddFieldToString(this.IdleTimeOut.HasValue, sb, "idle-time-out", (object) this.IdleTimeOut, ref count);
      this.AddFieldToString(this.OutgoingLocales != null, sb, "outgoing-locales", (object) this.OutgoingLocales, ref count);
      this.AddFieldToString(this.IncomingLocales != null, sb, "incoming-locales", (object) this.IncomingLocales, ref count);
      this.AddFieldToString(this.OfferedCapabilities != null, sb, "offered-capabilities", (object) this.OfferedCapabilities, ref count);
      this.AddFieldToString(this.DesiredCapabilities != null, sb, "desired-capabilities", (object) this.DesiredCapabilities, ref count);
      this.AddFieldToString(this.Properties != null, sb, "properties", (object) this.Properties, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (this.ContainerId == null)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "container-id", (object) Open.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeString(this.ContainerId, buffer);
      AmqpCodec.EncodeString(this.HostName, buffer);
      AmqpCodec.EncodeUInt(this.MaxFrameSize, buffer);
      AmqpCodec.EncodeUShort(this.ChannelMax, buffer);
      AmqpCodec.EncodeUInt(this.IdleTimeOut, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.OutgoingLocales, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.IncomingLocales, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.OfferedCapabilities, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.DesiredCapabilities, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.ContainerId = AmqpCodec.DecodeString(buffer);
      if (count-- > 0)
        this.HostName = AmqpCodec.DecodeString(buffer);
      if (count-- > 0)
        this.MaxFrameSize = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.ChannelMax = AmqpCodec.DecodeUShort(buffer);
      if (count-- > 0)
        this.IdleTimeOut = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.OutgoingLocales = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- > 0)
        this.IncomingLocales = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- > 0)
        this.OfferedCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- > 0)
        this.DesiredCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- <= 0)
        return;
      this.Properties = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetStringEncodeSize(this.ContainerId) + AmqpCodec.GetStringEncodeSize(this.HostName) + AmqpCodec.GetUIntEncodeSize(this.MaxFrameSize) + AmqpCodec.GetUShortEncodeSize(this.ChannelMax) + AmqpCodec.GetUIntEncodeSize(this.IdleTimeOut) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.OutgoingLocales) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.IncomingLocales) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.OfferedCapabilities) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.DesiredCapabilities) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties);
  }
}
