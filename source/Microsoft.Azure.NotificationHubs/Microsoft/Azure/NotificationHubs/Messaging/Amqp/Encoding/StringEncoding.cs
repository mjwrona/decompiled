// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.StringEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class StringEncoding : EncodingBase
  {
    public StringEncoding()
      : base((FormatCode) (byte) 177)
    {
    }

    public static int GetEncodeSize(string value)
    {
      if (value == null)
        return 1;
      int byteCount = System.Text.Encoding.UTF8.GetByteCount(value);
      return 1 + AmqpEncoding.GetEncodeWidthBySize(byteCount) + byteCount;
    }

    public static void Encode(string value, ByteBuffer buffer)
    {
      if (value == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        int encodeWidthBySize = AmqpEncoding.GetEncodeWidthBySize(bytes.Length);
        AmqpBitConverter.WriteUByte(buffer, encodeWidthBySize == 1 ? (byte) 161 : (byte) 177);
        StringEncoding.Encode(bytes, encodeWidthBySize, buffer);
      }
    }

    public static string Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return (string) null;
      int num;
      System.Text.Encoding utF8;
      if (formatCode == (FormatCode) (byte) 161)
      {
        num = (int) AmqpBitConverter.ReadUByte(buffer);
        utF8 = System.Text.Encoding.UTF8;
      }
      else
      {
        if (!(formatCode == (FormatCode) (byte) 177))
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        num = (int) AmqpBitConverter.ReadUInt(buffer);
        utF8 = System.Text.Encoding.UTF8;
      }
      string str = utF8.GetString(buffer.Buffer, buffer.Offset, num);
      buffer.Complete(num);
      return str;
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 + System.Text.Encoding.UTF8.GetByteCount((string) value) : StringEncoding.GetEncodeSize((string) value);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        StringEncoding.Encode(System.Text.Encoding.UTF8.GetBytes((string) value), 4, buffer);
      else
        StringEncoding.Encode((string) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) StringEncoding.Decode(buffer, formatCode);

    private static void Encode(byte[] encodedData, int width, ByteBuffer buffer)
    {
      if (width == 1)
        AmqpBitConverter.WriteUByte(buffer, (byte) encodedData.Length);
      else
        AmqpBitConverter.WriteUInt(buffer, (uint) encodedData.Length);
      AmqpBitConverter.WriteBytes(buffer, encodedData, 0, encodedData.Length);
    }
  }
}
