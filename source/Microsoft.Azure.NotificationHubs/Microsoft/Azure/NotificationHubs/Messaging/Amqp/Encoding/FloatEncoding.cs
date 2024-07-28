// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.FloatEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class FloatEncoding : EncodingBase
  {
    public FloatEncoding()
      : base((FormatCode) (byte) 114)
    {
    }

    public static int GetEncodeSize(float? value) => !value.HasValue ? 1 : 5;

    public static void Encode(float? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 114);
        AmqpBitConverter.WriteFloat(buffer, value.Value);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static float? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new float?() : new float?(AmqpBitConverter.ReadFloat(buffer));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 : FloatEncoding.GetEncodeSize(new float?((float) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteFloat(buffer, (float) value);
      else
        FloatEncoding.Encode(new float?((float) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) FloatEncoding.Decode(buffer, formatCode);
  }
}
