// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.UIntEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class UIntEncoding : EncodingBase
  {
    public UIntEncoding()
      : base((FormatCode) (byte) 112)
    {
    }

    public static int GetEncodeSize(uint? value)
    {
      if (!value.HasValue || value.Value == 0U)
        return 1;
      return value.Value > (uint) byte.MaxValue ? 5 : 2;
    }

    public static void Encode(uint? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        uint? nullable = value;
        uint num = 0;
        if (((int) nullable.GetValueOrDefault() == (int) num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          AmqpBitConverter.WriteUByte(buffer, (byte) 67);
        else if (value.Value <= (uint) byte.MaxValue)
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 82);
          AmqpBitConverter.WriteUByte(buffer, (byte) value.Value);
        }
        else
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 112);
          AmqpBitConverter.WriteUInt(buffer, value.Value);
        }
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static uint? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new uint?();
      EncodingBase.VerifyFormatCode(formatCode, buffer.Offset, (FormatCode) (byte) 112, (FormatCode) (byte) 82, (FormatCode) (byte) 67);
      return formatCode == (FormatCode) (byte) 67 ? new uint?(0U) : new uint?(formatCode == (FormatCode) (byte) 82 ? (uint) AmqpBitConverter.ReadUByte(buffer) : AmqpBitConverter.ReadUInt(buffer));
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 : UIntEncoding.GetEncodeSize(new uint?((uint) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteUInt(buffer, (uint) value);
      else
        UIntEncoding.Encode(new uint?((uint) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) UIntEncoding.Decode(buffer, formatCode);
  }
}
