// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Close
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Close : Performative
  {
    public static readonly string Name = "amqp:close:list";
    public static readonly ulong Code = 24;
    private const int Fields = 1;

    public Close()
      : base((AmqpSymbol) Close.Name, Close.Code)
    {
    }

    public Error Error { get; set; }

    protected override int FieldCount => 1;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("close(");
      int count = 0;
      this.AddFieldToString(this.Error != null, sb, "error", (object) this.Error, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    protected override void OnEncode(ByteBuffer buffer) => AmqpCodec.EncodeSerializable((IAmqpSerializable) this.Error, buffer);

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- <= 0)
        return;
      this.Error = AmqpCodec.DecodeKnownType<Error>(buffer);
    }

    protected override int OnValueSize() => 0 + AmqpCodec.GetSerializableEncodeSize((IAmqpSerializable) this.Error);
  }
}
