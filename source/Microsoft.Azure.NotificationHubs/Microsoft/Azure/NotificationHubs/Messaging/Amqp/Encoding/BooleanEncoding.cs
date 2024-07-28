// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.BooleanEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class BooleanEncoding : EncodingBase
  {
    public BooleanEncoding()
      : base((FormatCode) (byte) 86)
    {
    }

    public static int GetEncodeSize(bool? value) => !value.HasValue ? 1 : 1;

    public static void Encode(bool? value, ByteBuffer buffer)
    {
      if (value.HasValue)
        AmqpBitConverter.WriteUByte(buffer, value.Value ? (byte) 65 : (byte) 66);
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static bool? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new bool?();
      EncodingBase.VerifyFormatCode(formatCode, buffer.Offset, (FormatCode) (byte) 86, (FormatCode) (byte) 66, (FormatCode) (byte) 65);
      return formatCode == (FormatCode) (byte) 86 ? new bool?(AmqpBitConverter.ReadUByte(buffer) > (byte) 0) : new bool?(formatCode == (FormatCode) (byte) 65);
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 1 : BooleanEncoding.GetEncodeSize(new bool?((bool) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteUByte(buffer, (bool) value ? (byte) 1 : (byte) 0);
      else
        BooleanEncoding.Encode(new bool?((bool) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) BooleanEncoding.Decode(buffer, formatCode);
  }
}
