// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.UShortEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class UShortEncoding : EncodingBase
  {
    public UShortEncoding()
      : base((FormatCode) (byte) 96)
    {
    }

    public static int GetEncodeSize(ushort? value) => !value.HasValue ? 1 : 3;

    public static void Encode(ushort? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 96);
        AmqpBitConverter.WriteUShort(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static ushort? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new ushort?() : new ushort?(AmqpBitConverter.ReadUShort(buffer));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 2 : UShortEncoding.GetEncodeSize(new ushort?((ushort) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteUShort(buffer, (ushort) value);
      else
        UShortEncoding.Encode(new ushort?((ushort) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) UShortEncoding.Decode(buffer, formatCode);
  }
}
