// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Error
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  [Serializable]
  internal sealed class Error : DescribedList, ISerializable
  {
    public static readonly string Name = "amqp:error:list";
    public static readonly ulong Code = 29;
    private const int Fields = 3;

    public Error()
      : base((AmqpSymbol) Error.Name, Error.Code)
    {
    }

    private Error(SerializationInfo info, StreamingContext context)
      : base((AmqpSymbol) Error.Name, Error.Code)
    {
      this.Condition = (AmqpSymbol) (string) info.GetValue(nameof (Condition), typeof (string));
      this.Description = (string) info.GetValue(nameof (Description), typeof (string));
    }

    public AmqpSymbol Condition { get; set; }

    public string Description { get; set; }

    public Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields Info { get; set; }

    protected override int FieldCount => 3;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder("error(");
      int count = 0;
      this.AddFieldToString(this.Condition.Value != null, sb, "condition", (object) this.Condition, ref count);
      this.AddFieldToString(this.Description != null, sb, "description", (object) this.Description, ref count);
      this.AddFieldToString(this.Info != null, sb, "info", (object) this.Info, ref count);
      sb.Append(')');
      return sb.ToString();
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Condition", (object) this.Condition.Value);
      info.AddValue("Description", (object) this.Description);
    }

    protected override void EnsureRequired()
    {
      if (this.Condition.Value == null)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpRequiredFieldNotSet((object) "condition", (object) Error.Name));
    }

    protected override void OnEncode(ByteBuffer buffer)
    {
      AmqpCodec.EncodeSymbol(this.Condition, buffer);
      AmqpCodec.EncodeString(this.Description, buffer);
      AmqpCodec.EncodeMap((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Info, buffer);
    }

    protected override void OnDecode(ByteBuffer buffer, int count)
    {
      if (count-- > 0)
        this.Condition = AmqpCodec.DecodeSymbol(buffer);
      if (count-- > 0)
        this.Description = AmqpCodec.DecodeString(buffer);
      if (count-- <= 0)
        return;
      this.Info = AmqpCodec.DecodeMap<Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Fields>(buffer);
    }

    protected override int OnValueSize() => AmqpCodec.GetSymbolEncodeSize(this.Condition) + AmqpCodec.GetStringEncodeSize(this.Description) + AmqpCodec.GetMapEncodeSize((AmqpMap) (RestrictedMap<AmqpSymbol>) this.Info);
  }
}
