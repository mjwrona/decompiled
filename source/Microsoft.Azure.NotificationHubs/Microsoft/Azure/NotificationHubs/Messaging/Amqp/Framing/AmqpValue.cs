// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.AmqpValue
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class AmqpValue : AmqpDescribed
  {
    public static readonly string Name = "amqp:amqp-value:*";
    public static readonly ulong Code = 119;

    public AmqpValue()
      : base((AmqpSymbol) AmqpValue.Name, AmqpValue.Code)
    {
    }

    public override int GetValueEncodeSize() => this.Value is IAmqpSerializable amqpSerializable ? amqpSerializable.EncodeSize : base.GetValueEncodeSize();

    public override void EncodeValue(ByteBuffer buffer)
    {
      if (this.Value is IAmqpSerializable amqpSerializable)
        amqpSerializable.Encode(buffer);
      else
        base.EncodeValue(buffer);
    }

    public override void DecodeValue(ByteBuffer buffer) => this.Value = AmqpCodec.DecodeObject(buffer);

    public override string ToString() => "value()";
  }
}
