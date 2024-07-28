// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Attach
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal class Attach : LinkPerformative
  {
    public static readonly string Name = "amqp:attach:list";
    public static readonly ulong Code = 18;
    private const int Fields = 14;

    public Attach()
      : base((AmqpSymbol) Attach.Name, Attach.Code)
    {
    }

    public string LinkName { get; set; }

    public bool? Role { get; set; }

    public byte? SndSettleMode { get; set; }

    public byte? RcvSettleMode { get; set; }

    public object Source { get; set; }

    public object Target { get; set; }

    public AmqpMap Unsettled { get; set; }

    public bool? IncompleteUnsettled { get; set; }

    public uint? InitialDeliveryCount { get; set; }

    public ulong? MaxMessageSize { get; set; }

    public Multiple<AmqpSymbol> OfferedCapabilities { get; set; }

    public Multiple<AmqpSymbol> DesiredCapabilities { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields Properties { get; set; }

    protected override int FieldCount => 14;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("attach(");
      int count = 0;
      this.AddFieldToString(this.LinkName != null, sb, "name", (object) this.LinkName, ref count);
      this.AddFieldToString(this.Handle.HasValue, sb, "handle", (object) this.Handle, ref count);
      this.AddFieldToString(this.Role.HasValue, sb, "role", (object) this.Role, ref count);
      this.AddFieldToString(this.SndSettleMode.HasValue, sb, "snd-settle-mode", (object) this.SndSettleMode, ref count);
      this.AddFieldToString(this.RcvSettleMode.HasValue, sb, "rcv-settle-mode", (object) this.RcvSettleMode, ref count);
      this.AddFieldToString(this.Source != null, sb, "source", this.Source, ref count);
      this.AddFieldToString(this.Target != null, sb, "target", this.Target, ref count);
      this.AddFieldToString(this.IncompleteUnsettled.HasValue, sb, "incomplete-unsettled", (object) this.IncompleteUnsettled, ref count);
      this.AddFieldToString(this.InitialDeliveryCount.HasValue, sb, "initial-delivery-count", (object) this.InitialDeliveryCount, ref count);
      this.AddFieldToString(this.MaxMessageSize.HasValue, sb, "max-message-size", (object) this.MaxMessageSize, ref count);
      this.AddFieldToString(this.OfferedCapabilities != null, sb, "offered-capabilities", (object) this.OfferedCapabilities, ref count);
      this.AddFieldToString(this.DesiredCapabilities != null, sb, "desired-capabilities", (object) this.DesiredCapabilities, ref count);
      this.AddFieldToString(this.Properties != null, sb, "properties", (object) this.Properties, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (this.LinkName == null)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "name", (object) Attach.Name));
      if (!this.Handle.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "handle", (object) Attach.Name));
      if (!this.Role.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "role", (object) Attach.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeString(this.LinkName, buffer);
      AmqpCodec.EncodeUInt(this.Handle, buffer);
      AmqpCodec.EncodeBoolean(this.Role, buffer);
      AmqpCodec.EncodeUByte(this.SndSettleMode, buffer);
      AmqpCodec.EncodeUByte(this.RcvSettleMode, buffer);
      AmqpCodec.EncodeObject(this.Source, buffer);
      AmqpCodec.EncodeObject(this.Target, buffer);
      AmqpCodec.EncodeMap(this.Unsettled, buffer);
      AmqpCodec.EncodeBoolean(this.IncompleteUnsettled, buffer);
      AmqpCodec.EncodeUInt(this.InitialDeliveryCount, buffer);
      AmqpCodec.EncodeULong(this.MaxMessageSize, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.OfferedCapabilities, buffer);
      AmqpCodec.EncodeMultiple<AmqpSymbol>(this.DesiredCapabilities, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.LinkName = AmqpCodec.DecodeString(buffer);
      if (count-- > 0)
        this.Handle = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Role = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.SndSettleMode = AmqpCodec.DecodeUByte(buffer);
      if (count-- > 0)
        this.RcvSettleMode = AmqpCodec.DecodeUByte(buffer);
      if (count-- > 0)
        this.Source = AmqpCodec.DecodeObject(buffer);
      if (count-- > 0)
        this.Target = AmqpCodec.DecodeObject(buffer);
      if (count-- > 0)
        this.Unsettled = AmqpCodec.DecodeMap(buffer);
      if (count-- > 0)
        this.IncompleteUnsettled = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.InitialDeliveryCount = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.MaxMessageSize = AmqpCodec.DecodeULong(buffer);
      if (count-- > 0)
        this.OfferedCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- > 0)
        this.DesiredCapabilities = AmqpCodec.DecodeMultiple<AmqpSymbol>(buffer);
      if (count-- <= 0)
        return;
      this.Properties = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetStringEncodeSize(this.LinkName) + AmqpCodec.GetUIntEncodeSize(this.Handle) + AmqpCodec.GetBooleanEncodeSize(this.Role) + AmqpCodec.GetUByteEncodeSize(this.SndSettleMode) + AmqpCodec.GetUByteEncodeSize(this.RcvSettleMode) + AmqpCodec.GetObjectEncodeSize(this.Source) + AmqpCodec.GetObjectEncodeSize(this.Target) + AmqpCodec.GetMapEncodeSize(this.Unsettled) + AmqpCodec.GetBooleanEncodeSize(this.IncompleteUnsettled) + AmqpCodec.GetUIntEncodeSize(this.InitialDeliveryCount) + AmqpCodec.GetULongEncodeSize(this.MaxMessageSize) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.OfferedCapabilities) + AmqpCodec.GetMultipleEncodeSize<AmqpSymbol>(this.DesiredCapabilities) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Properties);
  }
}
