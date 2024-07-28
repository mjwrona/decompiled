// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.UuidEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class UuidEncoding : EncodingBase
  {
    public UuidEncoding()
      : base((FormatCode) (byte) 152)
    {
    }

    public static int GetEncodeSize(Guid? value) => !value.HasValue ? 1 : 17;

    public static void Encode(Guid? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 152);
        AmqpBitConverter.WriteUuid(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static Guid? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new Guid?() : new Guid?(AmqpBitConverter.ReadUuid(buffer));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 16 : UuidEncoding.GetEncodeSize(new Guid?((Guid) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteUuid(buffer, (Guid) value);
      else
        UuidEncoding.Encode(new Guid?((Guid) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) UuidEncoding.Decode(buffer, formatCode);
  }
}
