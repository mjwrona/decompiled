// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.SymbolEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class SymbolEncoding : EncodingBase
  {
    public SymbolEncoding()
      : base((FormatCode) (byte) 179)
    {
    }

    public static int GetValueSize(AmqpSymbol value) => value.Value != null ? System.Text.Encoding.ASCII.GetByteCount(value.Value) : 0;

    public static int GetEncodeSize(AmqpSymbol value) => value.Value != null ? 1 + AmqpEncoding.GetEncodeWidthBySize(value.ValueSize) + value.ValueSize : 1;

    public static void Encode(AmqpSymbol value, ByteBuffer buffer)
    {
      if (value.Value == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value.Value);
        int encodeWidthBySize = AmqpEncoding.GetEncodeWidthBySize(bytes.Length);
        AmqpBitConverter.WriteUByte(buffer, encodeWidthBySize == 1 ? (byte) 163 : (byte) 179);
        SymbolEncoding.Encode(bytes, encodeWidthBySize, buffer);
      }
    }

    public static AmqpSymbol Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return new AmqpSymbol();
      int count;
      AmqpEncoding.ReadCount(buffer, formatCode, (FormatCode) (byte) 163, (FormatCode) (byte) 179, out count);
      string str = System.Text.Encoding.ASCII.GetString(buffer.Buffer, buffer.Offset, count);
      buffer.Complete(count);
      return new AmqpSymbol(str);
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 + System.Text.Encoding.ASCII.GetByteCount(((AmqpSymbol) value).Value) : SymbolEncoding.GetEncodeSize((AmqpSymbol) value);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        SymbolEncoding.Encode(System.Text.Encoding.ASCII.GetBytes(((AmqpSymbol) value).Value), 4, buffer);
      else
        SymbolEncoding.Encode((AmqpSymbol) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) SymbolEncoding.Decode(buffer, formatCode);

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
