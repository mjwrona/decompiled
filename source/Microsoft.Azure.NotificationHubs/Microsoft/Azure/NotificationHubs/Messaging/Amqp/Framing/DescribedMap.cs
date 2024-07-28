// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.DescribedMap
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal abstract class DescribedMap : AmqpDescribed
  {
    private AmqpMap innerMap;

    public DescribedMap(AmqpSymbol name, ulong code)
      : base(name, code)
    {
      this.innerMap = new AmqpMap();
    }

    protected AmqpMap InnerMap => this.innerMap;

    public override int GetValueEncodeSize() => MapEncoding.GetEncodeSize(this.innerMap);

    public override void EncodeValue(ByteBuffer buffer) => MapEncoding.Encode(this.innerMap, buffer);

    public override void DecodeValue(ByteBuffer buffer) => this.innerMap = MapEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public void DecodeValue(ByteBuffer buffer, int size, int count) => MapEncoding.ReadMapValue(buffer, this.innerMap, size, count);
  }
}
