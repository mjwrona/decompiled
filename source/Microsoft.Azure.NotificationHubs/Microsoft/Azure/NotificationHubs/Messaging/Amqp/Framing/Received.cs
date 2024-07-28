// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Received
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Received : DeliveryState
  {
    public static readonly string Name = "amqp:received:list";
    public static readonly ulong Code = 35;
    private const int Fields = 2;

    public Received()
      : base((AmqpSymbol) Received.Name, Received.Code)
    {
    }

    public uint? SectionNumber { get; set; }

    public ulong? SectionOffset { get; set; }

    protected override int FieldCount => 2;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("received(");
      int count = 0;
      this.AddFieldToString(this.SectionNumber.HasValue, sb, "section-number", (object) this.SectionNumber, ref count);
      this.AddFieldToString(this.SectionOffset.HasValue, sb, "section-offset", (object) this.SectionOffset, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeUInt(this.SectionNumber, buffer);
      AmqpCodec.EncodeULong(this.SectionOffset, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.SectionNumber = AmqpCodec.DecodeUInt(buffer);
      if (count-- <= 0)
        return;
      this.SectionOffset = AmqpCodec.DecodeULong(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetUIntEncodeSize(this.SectionNumber) + AmqpCodec.GetULongEncodeSize(this.SectionOffset);
  }
}
