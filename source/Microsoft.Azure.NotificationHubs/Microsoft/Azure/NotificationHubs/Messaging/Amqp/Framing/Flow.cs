// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Flow
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Flow : LinkPerformative
  {
    public static readonly string Name = "amqp:flow:list";
    public static readonly ulong Code = 19;
    private const int Fields = 11;

    public Flow()
      : base((AmqpSymbol) Flow.Name, Flow.Code)
    {
    }

    public uint? NextIncomingId { get; set; }

    public uint? IncomingWindow { get; set; }

    public uint? NextOutgoingId { get; set; }

    public uint? OutgoingWindow { get; set; }

    public uint? DeliveryCount { get; set; }

    public uint? LinkCredit { get; set; }

    public uint? Available { get; set; }

    public bool? Drain { get; set; }

    public bool? Echo { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields Properties { get; set; }

    protected override int FieldCount => 11;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("flow(");
      int count = 0;
      this.AddFieldToString(this.NextIncomingId.HasValue, sb, "next-in-id", (object) this.NextIncomingId, ref count);
      this.AddFieldToString(this.IncomingWindow.HasValue, sb, "in-window", (object) this.IncomingWindow, ref count);
      this.AddFieldToString(this.NextOutgoingId.HasValue, sb, "next-out-id", (object) this.NextOutgoingId, ref count);
      this.AddFieldToString(this.OutgoingWindow.HasValue, sb, "out-window", (object) this.OutgoingWindow, ref count);
      this.AddFieldToString(this.Handle.HasValue, sb, "handle", (object) this.Handle, ref count);
      this.AddFieldToString(this.LinkCredit.HasValue, sb, "link-credit", (object) this.LinkCredit, ref count);
      this.AddFieldToString(this.DeliveryCount.HasValue, sb, "delivery-count", (object) this.DeliveryCount, ref count);
      this.AddFieldToString(this.Available.HasValue, sb, "available", (object) this.Available, ref count);
      this.AddFieldToString(this.Drain.HasValue, sb, "drain", (object) this.Drain, ref count);
      this.AddFieldToString(this.Echo.HasValue, sb, "echo", (object) this.Echo, ref count);
      this.AddFieldToString(this.Properties != null, sb, "properties", (object) this.Properties, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (!this.IncomingWindow.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "flow.incoming-window");
      if (!this.NextOutgoingId.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "flow.next-outgoing-id");
      if (!this.OutgoingWindow.HasValue)
        throw new AmqpException(AmqpError.InvalidField, "flow.outgoing-window");
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeUInt(this.NextIncomingId, buffer);
      AmqpCodec.EncodeUInt(this.IncomingWindow, buffer);
      AmqpCodec.EncodeUInt(this.NextOutgoingId, buffer);
      AmqpCodec.EncodeUInt(this.OutgoingWindow, buffer);
      AmqpCodec.EncodeUInt(this.Handle, buffer);
      AmqpCodec.EncodeUInt(this.DeliveryCount, buffer);
      AmqpCodec.EncodeUInt(this.LinkCredit, buffer);
      AmqpCodec.EncodeUInt(this.Available, buffer);
      AmqpCodec.EncodeBoolean(this.Drain, buffer);
      AmqpCodec.EncodeBoolean(this.Echo, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.NextIncomingId = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.IncomingWindow = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.NextOutgoingId = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.OutgoingWindow = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Handle = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.DeliveryCount = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.LinkCredit = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Available = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Drain = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.Echo = AmqpCodec.DecodeBoolean(buffer);
      if (count-- <= 0)
        return;
      this.Properties = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetUIntEncodeSize(this.NextIncomingId) + AmqpCodec.GetUIntEncodeSize(this.IncomingWindow) + AmqpCodec.GetUIntEncodeSize(this.NextOutgoingId) + AmqpCodec.GetUIntEncodeSize(this.OutgoingWindow) + AmqpCodec.GetUIntEncodeSize(this.Handle) + AmqpCodec.GetUIntEncodeSize(this.DeliveryCount) + AmqpCodec.GetUIntEncodeSize(this.LinkCredit) + AmqpCodec.GetUIntEncodeSize(this.Available) + AmqpCodec.GetBooleanEncodeSize(this.Drain) + AmqpCodec.GetBooleanEncodeSize(this.Echo) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties);
  }
}
