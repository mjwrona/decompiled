// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.ArrayEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class ArrayEncoding : EncodingBase
  {
    public ArrayEncoding()
      : base((FormatCode) (byte) 240)
    {
    }

    public static int GetEncodeSize<T>(T[] value) => value != null ? ArrayEncoding.GetEncodeSize((Array) value, false) : 1;

    public static void Encode<T>(T[] value, ByteBuffer buffer)
    {
      if (value == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        int width;
        int encodeSize = ArrayEncoding.GetEncodeSize((Array) value, false, out width);
        AmqpBitConverter.WriteUByte(buffer, width == 1 ? (byte) 224 : (byte) 240);
        ArrayEncoding.Encode((Array) value, width, encodeSize, buffer);
      }
    }

    public static T[] Decode<T>(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return (T[]) null;
      int size;
      int count;
      AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 224, (FormatCode) (byte) 240, out size, out count);
      formatCode = AmqpEncoding.ReadFormatCode(buffer);
      return ArrayEncoding.Decode<T>(buffer, size, count, formatCode);
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => ArrayEncoding.GetEncodeSize((Array) value, arrayEncoding);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      Array array = (Array) value;
      int width;
      int encodeSize = ArrayEncoding.GetEncodeSize(array, arrayEncoding, out width);
      AmqpBitConverter.WriteUByte(buffer, width == 1 ? (byte) 224 : (byte) 240);
      ArrayEncoding.Encode(array, width, encodeSize, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return (object) null;
      int size = 0;
      int count = 0;
      AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 224, (FormatCode) (byte) 240, out size, out count);
      formatCode = AmqpEncoding.ReadFormatCode(buffer);
      switch ((byte) formatCode)
      {
        case 80:
          return (object) ArrayEncoding.Decode<byte>(buffer, size, count, formatCode);
        case 81:
          return (object) ArrayEncoding.Decode<sbyte>(buffer, size, count, formatCode);
        case 82:
        case 112:
          return (object) ArrayEncoding.Decode<uint>(buffer, size, count, formatCode);
        case 83:
        case 128:
          return (object) ArrayEncoding.Decode<ulong>(buffer, size, count, formatCode);
        case 84:
        case 113:
          return (object) ArrayEncoding.Decode<int>(buffer, size, count, formatCode);
        case 85:
        case 129:
          return (object) ArrayEncoding.Decode<long>(buffer, size, count, formatCode);
        case 86:
          return (object) ArrayEncoding.Decode<bool>(buffer, size, count, formatCode);
        case 96:
          return (object) ArrayEncoding.Decode<ushort>(buffer, size, count, formatCode);
        case 97:
          return (object) ArrayEncoding.Decode<short>(buffer, size, count, formatCode);
        case 114:
          return (object) ArrayEncoding.Decode<float>(buffer, size, count, formatCode);
        case 115:
          return (object) ArrayEncoding.Decode<char>(buffer, size, count, formatCode);
        case 130:
          return (object) ArrayEncoding.Decode<double>(buffer, size, count, formatCode);
        case 131:
          return (object) ArrayEncoding.Decode<DateTime>(buffer, size, count, formatCode);
        case 152:
          return (object) ArrayEncoding.Decode<Guid>(buffer, size, count, formatCode);
        case 160:
        case 176:
          return (object) ArrayEncoding.Decode<ArraySegment<byte>>(buffer, size, count, formatCode);
        case 161:
        case 177:
          return (object) ArrayEncoding.Decode<string>(buffer, size, count, formatCode);
        case 163:
        case 179:
          return (object) ArrayEncoding.Decode<AmqpSymbol>(buffer, size, count, formatCode);
        case 192:
        case 208:
          return (object) ArrayEncoding.Decode<IList>(buffer, size, count, formatCode);
        case 193:
        case 209:
          return (object) ArrayEncoding.Decode<AmqpMap>(buffer, size, count, formatCode);
        case 224:
        case 240:
          return (object) ArrayEncoding.Decode<Array>(buffer, size, count, formatCode);
        default:
          throw new NotSupportedException(SRClient.NotSupportFrameCode((object) formatCode));
      }
    }

    private static int GetEncodeSize(Array array, bool arrayEncoding) => ArrayEncoding.GetEncodeSize(array, arrayEncoding, out int _);

    private static int GetEncodeSize(Array array, bool arrayEncoding, out int width)
    {
      int valueSize = 1 + ArrayEncoding.GetValueSize(array, (Type) null);
      width = arrayEncoding ? 4 : AmqpEncoding.GetEncodeWidthByCountAndSize(array.Length, valueSize);
      return valueSize + (1 + width + width);
    }

    private static int GetValueSize(Array value, Type type)
    {
      if (value.Length == 0)
        return 0;
      if (type == (Type) null)
        type = value.GetValue(0).GetType();
      EncodingBase encoding = AmqpEncoding.GetEncoding(type);
      int valueSize = 0;
      foreach (object obj in value)
      {
        bool arrayEncoding = true;
        if (encoding.FormatCode == (FormatCode) (byte) 0 && valueSize == 0)
          arrayEncoding = false;
        valueSize += encoding.GetObjectEncodeSize(obj, arrayEncoding);
      }
      return valueSize;
    }

    private static void Encode(Array value, int width, int encodeSize, ByteBuffer buffer)
    {
      encodeSize -= 1 + width;
      if (width == 1)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) encodeSize);
        AmqpBitConverter.WriteUByte(buffer, (byte) value.Length);
      }
      else
      {
        AmqpBitConverter.WriteUInt(buffer, (uint) encodeSize);
        AmqpBitConverter.WriteUInt(buffer, (uint) value.Length);
      }
      if (value.Length <= 0)
        return;
      object obj1 = value.GetValue(0);
      EncodingBase encoding = AmqpEncoding.GetEncoding(obj1);
      AmqpBitConverter.WriteUByte(buffer, (byte) encoding.FormatCode);
      if (encoding.FormatCode == (FormatCode) (byte) 0)
      {
        DescribedType describedType = (DescribedType) obj1;
        AmqpEncoding.EncodeObject(describedType.Descriptor, buffer);
        AmqpBitConverter.WriteUByte(buffer, (byte) AmqpEncoding.GetEncoding(describedType.Value).FormatCode);
      }
      foreach (object obj2 in value)
        encoding.EncodeObject(obj2, true, buffer);
    }

    private static T[] Decode<T>(ByteBuffer buffer, int size, int count, FormatCode formatCode)
    {
      T[] objArray = new T[count];
      EncodingBase encoding = AmqpEncoding.GetEncoding(formatCode);
      object descriptor = (object) null;
      if (formatCode == (FormatCode) (byte) 0)
      {
        descriptor = AmqpEncoding.DecodeObject(buffer);
        formatCode = AmqpEncoding.ReadFormatCode(buffer);
      }
      for (int index = 0; index < count; ++index)
      {
        object obj = encoding.DecodeObject(buffer, formatCode);
        if (descriptor != null)
          obj = (object) new DescribedType(descriptor, obj);
        objArray[index] = (T) obj;
      }
      return objArray;
    }
  }
}
