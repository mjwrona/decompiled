// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Source
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Source : DescribedList
  {
    public static readonly string Name = "amqp:source:list";
    public static readonly ulong Code = 40;
    private const int Fields = 11;

    public Source()
      : base((AmqpSymbol) Source.Name, Source.Code)
    {
    }

    public Source(Uri uri)
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

    public AmqpSymbol DistributionMode { get; set; }

    public FilterSet FilterSet { get; set; }

    public Outcome DefaultOutcome { get; set; }

    public Multiple<AmqpSymbol> Outcomes { get; set; }

    public Multiple<AmqpSymbol> Capabilities { get; set; }

    protected override int FieldCount => 11;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("source(");
      int count = 0;
      this.AddFieldToString(this.Address != null, sb, "address", (object) this.Address, ref count);
      this.AddFieldToString(this.Durable.HasValue, sb, "durable", (object) this.Durable, ref count);
      this.AddFieldToString(this.ExpiryPolicy.Value != null, sb, "expiry-policy", (object) this.ExpiryPolicy, ref count);
      this.AddFieldToString(this.Timeout.HasValue, sb, "timeout", (object) this.Timeout, ref count);
      this.AddFieldToString(this.Dynamic.HasValue, sb, "dynamic", (object) this.Dynamic, ref count);
      this.AddFieldToString(this.DynamicNodeProperties != null, sb, "dynamic-node-properties", (object) this.DynamicNodeProperties, ref count);
      this.AddFieldToString(this.DistributionMode.Value != null, sb, "distribution-mode", (object) this.DistributionMode, ref count);
      this.AddFieldToString(this.FilterSet != null, sb, "filter", (object) this.FilterSet, ref count);
      this.AddFieldToString(this.DefaultOutcome != null, sb, "default-outcome", (object) this.DefaultOutcome, ref count);
      this.AddFieldToString(this.Outcomes != null, sb, "outcomes", (object) this.Outcomes, ref count);
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
      AmqpCodec.EncodeSymbol(this.DistributionMode, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.FilterSet, buffer);
      AmqpCodec.EncodeSerializable((IAmqpSerializable) this.DefaultOutcome, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.Outcomes, buffer);
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
      if (count-- > 0)
        this.DistributionMode = AmqpCodec.DecodeSymbol(buffer);
      if (count-- > 0)
        this.FilterSet = AmqpCodec.DecodeMap<FilterSet>(buffer);
      if (count-- > 0)
        this.DefaultOutcome = (Outcome) AmqpCodec.DecodeAmqpDescribed(buffer);
      if (count-- > 0)
        this.Outcomes = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- <= 0)
        return;
      this.Capabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
    }

    protected override int OnValueSize() => 0 + Address.GetEncodeSize(this.Address) + AmqpCodec.GetUIntEncodeSize(this.Durable) + AmqpCodec.GetSymbolEncodeSize(this.ExpiryPolicy) + AmqpCodec.GetUIntEncodeSize(this.Timeout) + AmqpCodec.GetBooleanEncodeSize(this.Dynamic) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.DynamicNodeProperties) + AmqpCodec.GetSymbolEncodeSize(this.DistributionMode) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.FilterSet) + AmqpCodec.GetSerializableEncodeSize((IAmqpSerializable) this.DefaultOutcome) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.Outcomes) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.Capabilities);
  }
}
