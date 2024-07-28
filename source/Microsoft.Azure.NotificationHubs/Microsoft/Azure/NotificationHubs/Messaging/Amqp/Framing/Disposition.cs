// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Disposition
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Disposition : Performative
  {
    public static readonly string Name = "amqp:disposition:list";
    public static readonly ulong Code = 21;
    private const int Fields = 6;

    public Disposition()
      : base((AmqpSymbol) Disposition.Name, Disposition.Code)
    {
    }

    public bool? Role { get; set; }

    public uint? First { get; set; }

    public uint? Last { get; set; }

    public bool? Settled { get; set; }

    public DeliveryState State { get; set; }

    public bool? Batchable { get; set; }

    protected override int FieldCount => 6;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("disposition(");
      int count = 0;
      this.AddFieldToString(this.Role.HasValue, sb, "role", (object) this.Role, ref count);
      this.AddFieldToString(this.First.HasValue, sb, "first", (object) this.First, ref count);
      this.AddFieldToString(this.Last.HasValue, sb, "last", (object) this.Last, ref count);
      this.AddFieldToString(this.Settled.HasValue, sb, "settled", (object) this.Settled, ref count);
      this.AddFieldToString(this.State != null, sb, "state", (object) this.State, ref count);
      this.AddFieldToString(this.Batchable.HasValue, sb, "batchable", (object) this.Batchable, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (!this.Role.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "role", (object) Disposition.Name));
      if (!this.First.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "first", (object) Disposition.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeBoolean(this.Role, buffer);
      AmqpCodec.EncodeUInt(this.First, buffer);
      AmqpCodec.EncodeUInt(this.Last, buffer);
      AmqpCodec.EncodeBoolean(this.Settled, buffer);
      AmqpCodec.EncodeSerializable((IAmqpSerializable) this.State, buffer);
      AmqpCodec.EncodeBoolean(this.Batchable, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Role = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.First = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Last = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Settled = AmqpCodec.DecodeBoolean(buffer);
      if (count-- > 0)
        this.State = (DeliveryState) AmqpCodec.DecodeAmqpDescribed(buffer);
      if (count-- <= 0)
        return;
      this.Batchable = AmqpCodec.DecodeBoolean(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetBooleanEncodeSize(this.Role) + AmqpCodec.GetUIntEncodeSize(this.First) + AmqpCodec.GetUIntEncodeSize(this.Last) + AmqpCodec.GetBooleanEncodeSize(this.Settled) + AmqpCodec.GetSerializableEncodeSize((IAmqpSerializable) this.State) + AmqpCodec.GetBooleanEncodeSize(this.Batchable);
  }
}
