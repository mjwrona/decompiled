// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Header
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Header : DescribedList
  {
    public static readonly string Name = "amqp:header:list";
    public static readonly ulong Code = 112;
    private const int Fields = 5;

    public Header()
      : base((AmqpSymbol) Header.Name, Header.Code)
    {
    }

    public bool? Durable { get; set; }

    public byte? Priority { get; set; }

    public uint? Ttl { get; set; }

    public bool? FirstAcquirer { get; set; }

    public uint? DeliveryCount { get; set; }

    protected override int FieldCount => 5;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("header(");
      int count = 0;
      this.AddFieldToString(this.Durable.HasValue, sb, "durable", (object) this.Durable, ref count);
      this.AddFieldToString(this.Priority.HasValue, sb, "priority", (object) this.Priority, ref count);
      this.AddFieldToString(this.Ttl.HasValue, sb, "ttl", (object) this.Ttl, ref count);
      this.AddFieldToString(this.FirstAcquirer.HasValue, sb, "first-acquirer", (object) this.FirstAcquirer, ref count);
      this.AddFieldToString(this.DeliveryCount.HasValue, sb, "delivery-count", (object) this.DeliveryCount, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeBoolean(this.Durable, buffer);
      AmqpCodec.EncodeUByte(this.Priority, buffer);
      AmqpCodec.EncodeUInt(this.Ttl, buffer);
      AmqpCodec.EncodeBoolean(this.FirstAcquirer, buffer);
      AmqpCodec.EncodeUInt(this.DeliveryCount, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Durable = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.Priority = AmqpCodec.DecodeUByte(buffer);
      if (count-- > 0)
        this.Ttl = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.FirstAcquirer = AmqpCodec.DecodeBoolean(buffer);
      if (count-- <= 0)
        return;
      this.DeliveryCount = AmqpCodec.DecodeUInt(buffer);
    }

    protected override int OnValueSize() => AmqpCodec.GetBooleanEncodeSize(this.Durable) + AmqpCodec.GetUByteEncodeSize(this.Priority) + AmqpCodec.GetUIntEncodeSize(this.Ttl) + AmqpCodec.GetBooleanEncodeSize(this.FirstAcquirer) + AmqpCodec.GetUIntEncodeSize(this.DeliveryCount);
  }
}
