// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Transfer
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Transfer : LinkPerformative
  {
    public static readonly string Name = "amqp:transfer:list";
    public static readonly ulong Code = 20;
    private const int Fields = 11;

    public Transfer()
      : base((AmqpSymbol) Transfer.Name, Transfer.Code)
    {
    }

    public uint? DeliveryId { get; set; }

    public ArraySegment<byte> DeliveryTag { get; set; }

    public uint? MessageFormat { get; set; }

    public bool? Settled { get; set; }

    public bool? More { get; set; }

    public byte? RcvSettleMode { get; set; }

    public DeliveryState State { get; set; }

    public bool? Resume { get; set; }

    public bool? Aborted { get; set; }

    public bool? Batchable { get; set; }

    protected override int FieldCount => 11;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("transfer(");
      int count = 0;
      this.AddFieldToString(this.Handle.HasValue, sb, "handle", (object) this.Handle, ref count);
      this.AddFieldToString(this.DeliveryId.HasValue, sb, "delivery-id", (object) this.DeliveryId, ref count);
      this.AddFieldToString(this.DeliveryTag.Array != null, sb, "delivery-tag", (object) this.DeliveryTag, ref count);
      this.AddFieldToString(this.MessageFormat.HasValue, sb, "message-format", (object) this.MessageFormat, ref count);
      this.AddFieldToString(this.Settled.HasValue, sb, "settled", (object) this.Settled, ref count);
      this.AddFieldToString(this.More.HasValue, sb, "more", (object) this.More, ref count);
      this.AddFieldToString(this.RcvSettleMode.HasValue, sb, "rcv-settle-mode", (object) this.RcvSettleMode, ref count);
      this.AddFieldToString(this.State != null, sb, "state", (object) this.State, ref count);
      this.AddFieldToString(this.Resume.HasValue, sb, "resume", (object) this.Resume, ref count);
      this.AddFieldToString(this.Aborted.HasValue, sb, "aborted", (object) this.Aborted, ref count);
      this.AddFieldToString(this.Batchable.HasValue, sb, "batchable", (object) this.Batchable, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (!this.Handle.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "handle", (object) Transfer.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeUInt(this.Handle, buffer);
      AmqpCodec.EncodeUInt(this.DeliveryId, buffer);
      AmqpCodec.EncodeBinary(this.DeliveryTag, buffer);
      AmqpCodec.EncodeUInt(this.MessageFormat, buffer);
      AmqpCodec.EncodeBoolean(this.Settled, buffer);
      AmqpCodec.EncodeBoolean(this.More, buffer);
      AmqpCodec.EncodeUByte(this.RcvSettleMode, buffer);
      AmqpCodec.EncodeSerializable((IAmqpSerializable) this.State, buffer);
      AmqpCodec.EncodeBoolean(this.Resume, buffer);
      AmqpCodec.EncodeBoolean(this.Aborted, buffer);
      AmqpCodec.EncodeBoolean(this.Batchable, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Handle = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.DeliveryId = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.DeliveryTag = AmqpCodec.DecodeBinary(buffer);
      if (count-- > 0)
        this.MessageFormat = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Settled = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.More = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.RcvSettleMode = AmqpCodec.DecodeUByte(buffer);
      if (count-- > 0)
        this.State = (DeliveryState) AmqpCodec.DecodeAmqpDescribed(buffer);
      if (count-- > 0)
        this.Resume = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.Aborted = AmqpCodec.DecodeBoolean(buffer);
      if (count-- <= 0)
        return;
      this.Batchable = AmqpCodec.DecodeBoolean(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetUIntEncodeSize(this.Handle) + AmqpCodec.GetUIntEncodeSize(this.DeliveryId) + AmqpCodec.GetBinaryEncodeSize(this.DeliveryTag) + AmqpCodec.GetUIntEncodeSize(this.MessageFormat) + AmqpCodec.GetBooleanEncodeSize(this.Settled) + AmqpCodec.GetBooleanEncodeSize(this.More) + AmqpCodec.GetUByteEncodeSize(this.RcvSettleMode) + AmqpCodec.GetSerializableEncodeSize((IAmqpSerializable) this.State) + AmqpCodec.GetBooleanEncodeSize(this.Resume) + AmqpCodec.GetBooleanEncodeSize(this.Aborted) + AmqpCodec.GetBooleanEncodeSize(this.Batchable);
  }
}
