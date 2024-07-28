// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.ByteEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class ByteEncoding : EncodingBase
  {
    public ByteEncoding()
      : base((FormatCode) (byte) 81)
    {
    }

    public static int GetEncodeSize(sbyte? value) => !value.HasValue ? 1 : 2;

    public static void Encode(sbyte? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 81);
        AmqpBitConverter.WriteByte(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static sbyte? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new sbyte?() : new sbyte?(AmqpBitConverter.ReadByte(buffer));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 1 : ByteEncoding.GetEncodeSize(new sbyte?((sbyte) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteByte(buffer, (sbyte) value);
      else
        ByteEncoding.Encode(new sbyte?((sbyte) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) ByteEncoding.Decode(buffer, formatCode);
  }
}
