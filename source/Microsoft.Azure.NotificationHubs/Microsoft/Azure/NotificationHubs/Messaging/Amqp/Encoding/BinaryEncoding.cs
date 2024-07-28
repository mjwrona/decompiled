// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.BinaryEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class BinaryEncoding : EncodingBase
  {
    public BinaryEncoding()
      : base((FormatCode) (byte) 176)
    {
    }

    public static int GetEncodeSize(ArraySegment<byte> value) => value.Array != null ? 1 + AmqpEncoding.GetEncodeWidthBySize(value.Count) + value.Count : 1;

    public static void Encode(ArraySegment<byte> value, ByteBuffer buffer)
    {
      if (value.Array == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        if (AmqpEncoding.GetEncodeWidthBySize(value.Count) == 1)
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 160);
          AmqpBitConverter.WriteUByte(buffer, (byte) value.Count);
        }
        else
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 176);
          AmqpBitConverter.WriteUInt(buffer, (uint) value.Count);
        }
        AmqpBitConverter.WriteBytes(buffer, value.Array, value.Offset, value.Count);
      }
    }

    public static ArraySegment<byte> Decode(ByteBuffer buffer, FormatCode formatCode) => BinaryEncoding.Decode(buffer, formatCode, true);

    public static ArraySegment<byte> Decode(ByteBuffer buffer, FormatCode formatCode, bool copy)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return AmqpConstants.NullBinary;
      int count;
      AmqpEncoding.ReadCount(buffer, formatCode, (FormatCode) (byte) 160, (FormatCode) (byte) 176, out count);
      if (count == 0)
        return AmqpConstants.EmptyBinary;
      ArraySegment<byte> arraySegment;
      if (copy)
      {
        byte[] numArray = new byte[count];
        Buffer.BlockCopy((Array) buffer.Buffer, buffer.Offset, (Array) numArray, 0, count);
        arraySegment = new ArraySegment<byte>(numArray, 0, count);
      }
      else
        arraySegment = new ArraySegment<byte>(buffer.Buffer, buffer.Offset, count);
      buffer.Complete(count);
      return arraySegment;
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 4 + ((ArraySegment<byte>) value).Count : BinaryEncoding.GetEncodeSize((ArraySegment<byte>) value);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
      {
        ArraySegment<byte> arraySegment = (ArraySegment<byte>) value;
        AmqpBitConverter.WriteUInt(buffer, (uint) arraySegment.Count);
        AmqpBitConverter.WriteBytes(buffer, arraySegment.Array, arraySegment.Offset, arraySegment.Count);
      }
      else
        BinaryEncoding.Encode((ArraySegment<byte>) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) BinaryEncoding.Decode(buffer, formatCode);
  }
}
