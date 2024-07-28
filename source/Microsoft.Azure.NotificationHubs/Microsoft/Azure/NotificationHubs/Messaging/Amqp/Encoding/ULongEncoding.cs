// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.ULongEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class ULongEncoding : EncodingBase
  {
    public ULongEncoding()
      : base((FormatCode) (byte) 128)
    {
    }

    public static int GetEncodeSize(ulong? value)
    {
      if (!value.HasValue || value.Value == 0UL)
        return 1;
      return value.Value > (ulong) byte.MaxValue ? 9 : 2;
    }

    public static void Encode(ulong? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        ulong? nullable = value;
        ulong num = 0;
        if (((long) nullable.GetValueOrDefault() == (long) num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 68);
        }
        else
        {
          nullable = value;
          ulong maxValue = (ulong) byte.MaxValue;
          if ((nullable.GetValueOrDefault() <= maxValue ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          {
            AmqpBitConverter.WriteUByte(buffer, (byte) 83);
            AmqpBitConverter.WriteUByte(buffer, (byte) value.Value);
          }
          else
          {
            AmqpBitConverter.WriteUByte(buffer, (byte) 128);
            AmqpBitConverter.WriteULong(buffer, value.Value);
          }
        }
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static ulong? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new ulong?();
      EncodingBase.VerifyFormatCode(formatCode, buffer.Offset, (FormatCode) (byte) 128, (FormatCode) (byte) 83, (FormatCode) (byte) 68);
      return formatCode == (FormatCode) (byte) 68 ? new ulong?(0UL) : new ulong?(formatCode == (FormatCode) (byte) 83 ? (ulong) AmqpBitConverter.ReadUByte(buffer) : AmqpBitConverter.ReadULong(buffer));
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 8 : ULongEncoding.GetEncodeSize(new ulong?((ulong) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteULong(buffer, (ulong) value);
      else
        ULongEncoding.Encode(new ulong?((ulong) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) ULongEncoding.Decode(buffer, formatCode);
  }
}
