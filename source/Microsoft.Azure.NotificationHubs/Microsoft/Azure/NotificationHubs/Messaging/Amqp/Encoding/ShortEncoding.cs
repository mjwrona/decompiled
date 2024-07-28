// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.ShortEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class ShortEncoding : EncodingBase
  {
    public ShortEncoding()
      : base((FormatCode) (byte) 97)
    {
    }

    public static int GetEncodeSize(short? value) => !value.HasValue ? 1 : 3;

    public static void Encode(short? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 97);
        AmqpBitConverter.WriteShort(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static short? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new short?() : new short?(AmqpBitConverter.ReadShort(buffer));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 2 : ShortEncoding.GetEncodeSize(new short?((short) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteShort(buffer, (short) value);
      else
        ShortEncoding.Encode(new short?((short) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) ShortEncoding.Decode(buffer, formatCode);
  }
}
