// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Modified
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Modified : Outcome
  {
    public static readonly string Name = "amqp:modified:list";
    public static readonly ulong Code = 39;
    private const int Fields = 3;

    public Modified()
      : base((AmqpSymbol) Modified.Name, Modified.Code)
    {
    }

    public bool? DeliveryFailed { get; set; }

    public bool? UndeliverableHere { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields MessageAnnotations { get; set; }

    protected override int FieldCount => 3;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("modified(");
      int count = 0;
      this.AddFieldToString(this.DeliveryFailed.HasValue, sb, "delivery-failed", (object) this.DeliveryFailed, ref count);
      this.AddFieldToString(this.UndeliverableHere.HasValue, sb, "undeliverable-here", (object) this.UndeliverableHere, ref count);
      this.AddFieldToString(this.MessageAnnotations != null, sb, "message-annotations", (object) this.MessageAnnotations, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeBoolean(this.DeliveryFailed, buffer);
      AmqpCodec.EncodeBoolean(this.UndeliverableHere, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.MessageAnnotations, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.DeliveryFailed = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.UndeliverableHere = AmqpCodec.DecodeBoolean(buffer);
      if (count-- <= 0)
        return;
      this.MessageAnnotations = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetBooleanEncodeSize(this.DeliveryFailed) + AmqpCodec.GetBooleanEncodeSize(this.UndeliverableHere) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.MessageAnnotations);
  }
}
