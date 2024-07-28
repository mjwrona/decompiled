// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.DescribedEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class DescribedEncoding : EncodingBase
  {
    public DescribedEncoding()
      : base((FormatCode) (byte) 0)
    {
    }

    public static int GetEncodeSize(DescribedType value) => value != null ? 1 + AmqpEncoding.GetObjectEncodeSize(value.Descriptor) + AmqpEncoding.GetObjectEncodeSize(value.Value) : 1;

    public static void Encode(DescribedType value, ByteBuffer buffer)
    {
      if (value.Value == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 0);
        AmqpEncoding.EncodeObject(value.Descriptor, buffer);
        AmqpEncoding.EncodeObject(value.Value, buffer);
      }
    }

    public static DescribedType Decode(ByteBuffer buffer)
    {
      FormatCode formatCode;
      return (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? (DescribedType) null : DescribedEncoding.Decode(buffer, formatCode);
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding)
    {
      if (!arrayEncoding)
        return DescribedEncoding.GetEncodeSize((DescribedType) value);
      object obj = ((DescribedType) value).Value;
      return AmqpEncoding.GetEncoding(obj).GetObjectEncodeSize(obj, true);
    }

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
      {
        object obj = ((DescribedType) value).Value;
        AmqpEncoding.GetEncoding(obj).EncodeObject(obj, true, buffer);
      }
      else
        DescribedEncoding.Encode((DescribedType) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 ? (object) DescribedEncoding.Decode(buffer, formatCode) : AmqpEncoding.DecodeObject(buffer, formatCode);

    private static DescribedType Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode != (FormatCode) (byte) 0)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
      return new DescribedType(AmqpEncoding.DecodeObject(buffer), AmqpEncoding.DecodeObject(buffer));
    }
  }
}
