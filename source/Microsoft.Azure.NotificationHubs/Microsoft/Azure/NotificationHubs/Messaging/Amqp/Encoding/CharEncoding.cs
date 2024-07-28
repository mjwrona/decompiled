// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.CharEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class CharEncoding : EncodingBase
  {
    public CharEncoding()
      : base((FormatCode) (byte) 115)
    {
    }

    public static int GetEncodeSize(char? value) => !value.HasValue ? 1 : 5;

    public static void Encode(char? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 115);
        AmqpBitConverter.WriteInt(buffer, char.ConvertToUtf32(new string(value.Value, 1), 0));
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static char? Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new char?();
      string str = char.ConvertFromUtf32(AmqpBitConverter.ReadInt(buffer));
      return str.Length <= 1 ? new char?(str[0]) : throw new ArgumentOutOfRangeException(SRClient.ErroConvertingToChar);
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 : CharEncoding.GetEncodeSize(new char?((char) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteInt(buffer, char.ConvertToUtf32(new string((char) value, 1), 0));
      else
        CharEncoding.Encode(new char?((char) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) CharEncoding.Decode(buffer, formatCode);
  }
}
