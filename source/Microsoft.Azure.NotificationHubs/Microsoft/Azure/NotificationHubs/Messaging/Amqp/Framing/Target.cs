// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Target
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Target : DescribedList
  {
    public static readonly string Name = "amqp:target:list";
    public static readonly ulong Code = 41;
    private const int Fields = 7;

    public Target()
      : base((AmqpSymbol) Target.Name, Target.Code)
    {
    }

    public Target(Uri uri)
      : this()
    {
      this.Address = (Address) uri.AbsoluteUri;
    }

    public Address Address { get; set; }

    public uint? Durable { get; set; }

    public AmqpSymbol ExpiryPolicy { get; set; }

    public uint? Timeout { get; set; }

    public bool? Dynamic { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields DynamicNodeProperties { get; set; }

    public Multiple<AmqpSymbol> Capabilities { get; set; }

    protected override int FieldCount => 7;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("target(");
      int count = 0;
      this.AddFieldToString(this.Address != null, sb, "address", (object) this.Address, ref count);
      this.AddFieldToString(this.Durable.HasValue, sb, "durable", (object) this.Durable, ref count);
      this.AddFieldToString(this.ExpiryPolicy.Value != null, sb, "expiry-policy", (object) this.ExpiryPolicy, ref count);
      this.AddFieldToString(this.Timeout.HasValue, sb, "timeout", (object) this.Timeout, ref count);
      this.AddFieldToString(this.Dynamic.HasValue, sb, "dynamic", (object) this.Dynamic, ref count);
      this.AddFieldToString(this.DynamicNodeProperties != null, sb, "dynamic-node-properties", (object) this.DynamicNodeProperties, ref count);
      this.AddFieldToString(this.Capabilities != null, sb, "capabilities", (object) this.Capabilities, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      Address.Encode(buffer, this.Address);
      AmqpCodec.EncodeUInt(this.Durable, buffer);
      AmqpCodec.EncodeSymbol(this.ExpiryPolicy, buffer);
      AmqpCodec.EncodeUInt(this.Timeout, buffer);
      AmqpCodec.EncodeBoolean(this.Dynamic, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.DynamicNodeProperties, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.Capabilities, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Address = Address.Decode(buffer);
      if (count-- > 0)
        this.Durable = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.ExpiryPolicy = AmqpCodec.DecodeSymbol(buffer);
      if (count-- > 0)
        this.Timeout = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Dynamic = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.DynamicNodeProperties = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
      if (count-- <= 0)
        return;
      this.Capabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
    }

    protected override int OnValueSize() => 0 + Address.GetEncodeSize(this.Address) + AmqpCodec.GetUIntEncodeSize(this.Durable) + AmqpCodec.GetSymbolEncodeSize(this.ExpiryPolicy) + AmqpCodec.GetUIntEncodeSize(this.Timeout) + AmqpCodec.GetBooleanEncodeSize(this.Dynamic) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.DynamicNodeProperties) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.Capabilities);
  }
}
