// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.LongEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class LongEncoding : EncodingBase
  {
    public LongEncoding()
      : base((FormatCode) (byte) 129)
    {
    }

    public static int GetEncodeSize(long? value)
    {
      if (!value.HasValue)
        return 1;
      long? nullable = value;
      long minValue = (long) sbyte.MinValue;
      if ((nullable.GetValueOrDefault() < minValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
      {
        nullable = value;
        long maxValue = (long) sbyte.MaxValue;
        if ((nullable.GetValueOrDefault() > maxValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
          return 2;
      }
      return 9;
    }

    public static void Encode(long? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        long? nullable = value;
        long minValue = (long) sbyte.MinValue;
        if ((nullable.GetValueOrDefault() < minValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
        {
          nullable = value;
          long maxValue = (long) sbyte.MaxValue;
          if ((nullable.GetValueOrDefault() > maxValue ? (nullable.HasValue ? 1 : 0) : 0) == 0)
          {
            AmqpBitConverter.WriteUByte(buffer, (byte) 85);
            AmqpBitConverter.WriteByte(buffer, (sbyte) value.Value);
            return;
          }
        }
        AmqpBitConverter.WriteUByte(buffer, (byte) 129);
        AmqpBitConverter.WriteLong(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static long? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new long?();
      EncodingBase.VerifyFormatCode(formatCode, buffer.Offset, (FormatCode) (byte) 129, (FormatCode) (byte) 85);
      return new long?(formatCode == (FormatCode) (byte) 85 ? (long) AmqpBitConverter.ReadByte(buffer) : AmqpBitConverter.ReadLong(buffer));
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 8 : LongEncoding.GetEncodeSize(new long?((long) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteLong(buffer, (long) value);
      else
        LongEncoding.Encode(new long?((long) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) LongEncoding.Decode(buffer, formatCode);
  }
}
