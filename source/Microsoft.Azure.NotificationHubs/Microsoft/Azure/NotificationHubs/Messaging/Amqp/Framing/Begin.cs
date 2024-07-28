// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Begin
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal class Begin : Performative
  {
    public static readonly string Name = "amqp:begin:list";
    public static readonly ulong Code = 17;
    private const int Fields = 8;

    public Begin()
      : base((AmqpSymbol) Begin.Name, Begin.Code)
    {
    }

    public ushort? RemoteChannel { get; set; }

    public uint? NextOutgoingId { get; set; }

    public uint? IncomingWindow { get; set; }

    public uint? OutgoingWindow { get; set; }

    public uint? HandleMax { get; set; }

    public Multiple<AmqpSymbol> OfferedCapabilities { get; set; }

    public Multiple<AmqpSymbol> DesiredCapabilities { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields Properties { get; set; }

    protected override int FieldCount => 8;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("begin(");
      int count = 0;
      this.AddFieldToString(this.RemoteChannel.HasValue, sb, "remote-channel", (object) this.RemoteChannel, ref count);
      this.AddFieldToString(this.NextOutgoingId.HasValue, sb, "next-outgoing-id", (object) this.NextOutgoingId, ref count);
      this.AddFieldToString(this.IncomingWindow.HasValue, sb, "incoming-window", (object) this.IncomingWindow, ref count);
      this.AddFieldToString(this.OutgoingWindow.HasValue, sb, "outgoing-window", (object) this.OutgoingWindow, ref count);
      this.AddFieldToString(this.HandleMax.HasValue, sb, "handle-max", (object) this.HandleMax, ref count);
      this.AddFieldToString(this.OfferedCapabilities != null, sb, "offered-capabilities", (object) this.OfferedCapabilities, ref count);
      this.AddFieldToString(this.DesiredCapabilities != null, sb, "desired-capabilities", (object) this.DesiredCapabilities, ref count);
      this.AddFieldToString(this.Properties != null, sb, "properties", (object) this.Properties, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (!this.NextOutgoingId.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "begin.next-outgoing-id");
      if (!this.IncomingWindow.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "begin.incoming-window");
      if (!this.OutgoingWindow.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "begin.outgoing-window");
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeUShort(this.RemoteChannel, buffer);
      AmqpCodec.EncodeUInt(this.NextOutgoingId, buffer);
      AmqpCodec.EncodeUInt(this.IncomingWindow, buffer);
      AmqpCodec.EncodeUInt(this.OutgoingWindow, buffer);
      AmqpCodec.EncodeUInt(this.HandleMax, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.OfferedCapabilities, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.DesiredCapabilities, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.RemoteChannel = AmqpCodec.DecodeUShort(buffer);
      if (count-- > 0)
        this.NextOutgoingId = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.IncomingWindow = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.OutgoingWindow = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.HandleMax = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.OfferedCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- > 0)
        this.DesiredCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- <= 0)
        return;
      this.Properties = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetUShortEncodeSize(this.RemoteChannel) + AmqpCodec.GetUIntEncodeSize(this.NextOutgoingId) + AmqpCodec.GetUIntEncodeSize(this.IncomingWindow) + AmqpCodec.GetUIntEncodeSize(this.OutgoingWindow) + AmqpCodec.GetUIntEncodeSize(this.HandleMax) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.OfferedCapabilities) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.DesiredCapabilities) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties);
  }
}
