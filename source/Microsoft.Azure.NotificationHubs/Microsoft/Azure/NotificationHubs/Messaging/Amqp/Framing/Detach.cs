// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Detach
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Detach : LinkPerformative
  {
    public static readonly string Name = "amqp:detach:list";
    public static readonly ulong Code = 22;
    private const int Fields = 3;

    public Detach()
      : base((AmqpSymbol) Detach.Name, Detach.Code)
    {
    }

    public bool? Closed { get; set; }

    public Error Error { get; set; }

    protected override int FieldCount => 3;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("detach(");
      int count = 0;
      this.AddFieldToString(this.Handle.HasValue, sb, "handle", (object) this.Handle, ref count);
      this.AddFieldToString(this.Closed.HasValue, sb, "closed", (object) this.Closed, ref count);
      this.AddFieldToString(this.Error != null, sb, "error", (object) this.Error, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void EnsureRequired()
    {
      if (!this.Handle.HasValue)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "handle", (object) Detach.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeUInt(this.Handle, buffer);
      AmqpCodec.EncodeBoolean(this.Closed, buffer);
      AmqpCodec.EncodeSerializable((IAmqpSerializable) this.Error, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Handle = AmqpCodec.DecodeUInt(buffer);
      if (count-- > 0)
        this.Closed = AmqpCodec.DecodeBoolean(buffer);
      if (count-- <= 0)
        return;
      this.Error = AmqpCodec.DecodeKnownType<Error>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetUIntEncodeSize(this.Handle) + AmqpCodec.GetBooleanEncodeSize(this.Closed) + AmqpCodec.GetObjectEncodeSize((object) this.Error);
  }
}
