// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.DecimalEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class DecimalEncoding : EncodingBase
  {
    private const int Decimal32Bias = 101;
    private const int Decimal64Bias = 398;
    private const int Decimal128Bias = 6176;

    public DecimalEncoding()
      : base((FormatCode) (byte) 148)
    {
    }

    public static int GetEncodeSize(Decimal? value) => !value.HasValue ? 1 : 17;

    public static void Encode(Decimal? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 148);
        DecimalEncoding.EncodeValue(value.Value, buffer);
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static Decimal? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new Decimal?() : new Decimal?(DecimalEncoding.DecodeValue(buffer, formatCode));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 16 : 17;

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        DecimalEncoding.EncodeValue((Decimal) value, buffer);
      else
        DecimalEncoding.Encode(new Decimal?((Decimal) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) DecimalEncoding.Decode(buffer, formatCode);

    private static unsafe void EncodeValue(Decimal value, ByteBuffer buffer)
    {
      int[] bits = Decimal.GetBits(value);
      int num1 = bits[0];
      int num2 = bits[1];
      int num3 = bits[2];
      int num4 = bits[3];
      byte[] data = new byte[16];
      byte* numPtr1 = (byte*) &num4;
      int num5 = 6176 - (int) numPtr1[2];
      data[0] = numPtr1[3];
      data[0] |= (byte) (num5 >> 9);
      data[1] = (byte) ((num5 & (int) sbyte.MaxValue) << 1);
      data[2] = (byte) 0;
      data[3] = (byte) 0;
      byte* numPtr2 = (byte*) &num3;
      data[4] = numPtr2[3];
      data[5] = numPtr2[2];
      data[6] = numPtr2[1];
      data[7] = *numPtr2;
      byte* numPtr3 = (byte*) &num2;
      data[8] = numPtr3[3];
      data[9] = numPtr3[2];
      data[10] = numPtr3[1];
      data[11] = *numPtr3;
      byte* numPtr4 = (byte*) &num1;
      data[12] = numPtr4[3];
      data[13] = numPtr4[2];
      data[14] = numPtr4[1];
      data[15] = *numPtr4;
      AmqpBitConverter.WriteBytes(buffer, data, 0, data.Length);
    }

    private static Decimal DecodeValue(ByteBuffer buffer, FormatCode formatCode)
    {
      switch ((byte) formatCode)
      {
        case 116:
          return DecimalEncoding.DecodeDecimal32(buffer);
        case 132:
          return DecimalEncoding.DecodeDecimal64(buffer);
        case 148:
          return DecimalEncoding.DecodeDecimal128(buffer);
        default:
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
      }
    }

    private static Decimal DecodeDecimal32(ByteBuffer buffer)
    {
      byte[] numArray = new byte[4];
      AmqpBitConverter.ReadBytes(buffer, numArray, 0, numArray.Length);
      int num = 0;
      int sign = ((int) numArray[0] & 128) != 0 ? -1 : 1;
      if (((int) numArray[0] & 96) != 96)
      {
        num = ((int) numArray[0] & (int) sbyte.MaxValue) << 1 | ((int) numArray[1] & 128) >> 7;
        numArray[0] = (byte) 0;
        numArray[1] &= (byte) 127;
      }
      else if (((int) numArray[0] & 120) == 0)
      {
        num = ((int) numArray[0] & 31) << 3 | ((int) numArray[1] & 224) >> 5;
        numArray[0] = (byte) 0;
        numArray[1] &= (byte) 31;
        numArray[1] |= (byte) 128;
      }
      return DecimalEncoding.CreateDecimal((int) AmqpBitConverter.ReadUInt(numArray, 0, numArray.Length), 0, 0, sign, num - 101);
    }

    private static Decimal DecodeDecimal64(ByteBuffer buffer)
    {
      byte[] numArray = new byte[8];
      AmqpBitConverter.ReadBytes(buffer, numArray, 0, numArray.Length);
      int num = 0;
      int sign = ((int) numArray[0] & 128) != 0 ? -1 : 1;
      if (((int) numArray[0] & 96) != 96)
      {
        num = ((int) numArray[0] & (int) sbyte.MaxValue) << 3 | ((int) numArray[1] & 224) >> 5;
        numArray[0] = (byte) 0;
        numArray[1] &= (byte) 31;
      }
      else if (((int) numArray[0] & 120) == 0)
      {
        num = ((int) numArray[0] & 31) << 8 | ((int) numArray[1] & 248) >> 3;
        numArray[0] = (byte) 0;
        numArray[1] &= (byte) 7;
        numArray[1] |= (byte) 32;
      }
      int middle = (int) AmqpBitConverter.ReadUInt(numArray, 0, 4);
      return DecimalEncoding.CreateDecimal((int) AmqpBitConverter.ReadUInt(numArray, 4, 4), middle, 0, sign, num - 398);
    }

    private static Decimal DecodeDecimal128(ByteBuffer buffer)
    {
      byte[] numArray = new byte[16];
      AmqpBitConverter.ReadBytes(buffer, numArray, 0, numArray.Length);
      int num = 0;
      int sign = ((int) numArray[0] & 128) != 0 ? -1 : 1;
      if (((int) numArray[0] & 96) != 96)
      {
        num = ((int) numArray[0] & (int) sbyte.MaxValue) << 7 | ((int) numArray[1] & 254) >> 1;
        numArray[0] = (byte) 0;
        numArray[1] &= (byte) 1;
      }
      else if (((int) numArray[0] & 120) == 0)
        return 0M;
      int high = (int) AmqpBitConverter.ReadUInt(numArray, 4, 4);
      int middle = (int) AmqpBitConverter.ReadUInt(numArray, 8, 4);
      return DecimalEncoding.CreateDecimal((int) AmqpBitConverter.ReadUInt(numArray, 12, 4), middle, high, sign, num - 6176);
    }

    private static Decimal CreateDecimal(int low, int middle, int high, int sign, int exponent)
    {
      if (exponent <= 0)
        return new Decimal(low, middle, high, sign < 0, (byte) -exponent);
      Decimal num = new Decimal(low, middle, high, sign < 0, (byte) 0);
      for (int index = 0; index < exponent; ++index)
        num *= 10M;
      return num;
    }
  }
}
