// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.IntEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class IntEncoding : EncodingBase
  {
    public IntEncoding()
      : base((FormatCode) (byte) 113)
    {
    }

    public static int GetEncodeSize(int? value)
    {
      if (!value.HasValue)
        return 1;
      return value.Value >= (int) sbyte.MinValue && value.Value <= (int) sbyte.MaxValue ? 2 : 5;
    }

    public static void Encode(int? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        int? nullable = value;
        int minValue = (int) sbyte.MinValue;
        if ((nullable.GetValueOrDefault() < minValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
        {
          nullable = value;
          int maxValue = (int) sbyte.MaxValue;
          if ((nullable.GetValueOrDefault() > maxValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
          {
            AmqpBitConverter.WriteUByte(buffer, (byte) 84);
            AmqpBitConverter.WriteByte(buffer, (sbyte) value.Value);
            return;
          }
        }
        AmqpBitConverter.WriteUByte(buffer, (byte) 113);
        AmqpBitConverter.WriteInt(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static int? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new int?();
      EncodingBase.VerifyFormatCode(formatCode, buffer.Offset, (FormatCode) (byte) 113, (FormatCode) (byte) 84);
      return new int?(formatCode == (FormatCode) (byte) 84 ? (int) AmqpBitConverter.ReadByte(buffer) : AmqpBitConverter.ReadInt(buffer));
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 : IntEncoding.GetEncodeSize(new int?((int) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteInt(buffer, (int) value);
      else
        IntEncoding.Encode(new int?((int) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) IntEncoding.Decode(buffer, formatCode);
  }
}
