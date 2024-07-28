// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.AmqpEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal static class AmqpEncoding
  {
    private static Dictionary<Type, EncodingBase> encodingsByType;
    private static Dictionary<FormatCode, EncodingBase> encodingsByCode;
    private static BooleanEncoding booleanEncoding = new BooleanEncoding();
    private static UByteEncoding ubyteEncoding = new UByteEncoding();
    private static UShortEncoding ushortEncoding = new UShortEncoding();
    private static UIntEncoding uintEncoding = new UIntEncoding();
    private static ULongEncoding ulongEncoding = new ULongEncoding();
    private static ByteEncoding byteEncoding = new ByteEncoding();
    private static ShortEncoding shortEncoding = new ShortEncoding();
    private static IntEncoding intEncoding = new IntEncoding();
    private static LongEncoding longEncoding = new LongEncoding();
    private static FloatEncoding floatEncoding = new FloatEncoding();
    private static DoubleEncoding doubleEncoding = new DoubleEncoding();
    private static DecimalEncoding decimal128Encoding = new DecimalEncoding();
    private static CharEncoding charEncoding = new CharEncoding();
    private static TimeStampEncoding timeStampEncoding = new TimeStampEncoding();
    private static UuidEncoding uuidEncoding = new UuidEncoding();
    private static BinaryEncoding binaryEncoding = new BinaryEncoding();
    private static SymbolEncoding symbolEncoding = new SymbolEncoding();
    private static StringEncoding stringEncoding = new StringEncoding();
    private static ListEncoding listEncoding = new ListEncoding();
    private static MapEncoding mapEncoding = new MapEncoding();
    private static ArrayEncoding arrayEncoding = new ArrayEncoding();
    private static DescribedEncoding describedTypeEncoding = new DescribedEncoding();

    static AmqpEncoding()
    {
      AmqpEncoding.encodingsByType = new Dictionary<Type, EncodingBase>()
      {
        {
          typeof (bool),
          (EncodingBase) AmqpEncoding.booleanEncoding
        },
        {
          typeof (byte),
          (EncodingBase) AmqpEncoding.ubyteEncoding
        },
        {
          typeof (ushort),
          (EncodingBase) AmqpEncoding.ushortEncoding
        },
        {
          typeof (uint),
          (EncodingBase) AmqpEncoding.uintEncoding
        },
        {
          typeof (ulong),
          (EncodingBase) AmqpEncoding.ulongEncoding
        },
        {
          typeof (sbyte),
          (EncodingBase) AmqpEncoding.byteEncoding
        },
        {
          typeof (short),
          (EncodingBase) AmqpEncoding.shortEncoding
        },
        {
          typeof (int),
          (EncodingBase) AmqpEncoding.intEncoding
        },
        {
          typeof (long),
          (EncodingBase) AmqpEncoding.longEncoding
        },
        {
          typeof (float),
          (EncodingBase) AmqpEncoding.floatEncoding
        },
        {
          typeof (double),
          (EncodingBase) AmqpEncoding.doubleEncoding
        },
        {
          typeof (Decimal),
          (EncodingBase) AmqpEncoding.decimal128Encoding
        },
        {
          typeof (char),
          (EncodingBase) AmqpEncoding.charEncoding
        },
        {
          typeof (DateTime),
          (EncodingBase) AmqpEncoding.timeStampEncoding
        },
        {
          typeof (Guid),
          (EncodingBase) AmqpEncoding.uuidEncoding
        },
        {
          typeof (ArraySegment<byte>),
          (EncodingBase) AmqpEncoding.binaryEncoding
        },
        {
          typeof (AmqpSymbol),
          (EncodingBase) AmqpEncoding.symbolEncoding
        },
        {
          typeof (string),
          (EncodingBase) AmqpEncoding.stringEncoding
        },
        {
          typeof (AmqpMap),
          (EncodingBase) AmqpEncoding.mapEncoding
        }
      };
      AmqpEncoding.encodingsByCode = new Dictionary<FormatCode, EncodingBase>()
      {
        {
          (FormatCode) (byte) 66,
          (EncodingBase) AmqpEncoding.booleanEncoding
        },
        {
          (FormatCode) (byte) 65,
          (EncodingBase) AmqpEncoding.booleanEncoding
        },
        {
          (FormatCode) (byte) 86,
          (EncodingBase) AmqpEncoding.booleanEncoding
        },
        {
          (FormatCode) (byte) 80,
          (EncodingBase) AmqpEncoding.ubyteEncoding
        },
        {
          (FormatCode) (byte) 96,
          (EncodingBase) AmqpEncoding.ushortEncoding
        },
        {
          (FormatCode) (byte) 112,
          (EncodingBase) AmqpEncoding.uintEncoding
        },
        {
          (FormatCode) (byte) 82,
          (EncodingBase) AmqpEncoding.uintEncoding
        },
        {
          (FormatCode) (byte) 67,
          (EncodingBase) AmqpEncoding.uintEncoding
        },
        {
          (FormatCode) (byte) 128,
          (EncodingBase) AmqpEncoding.ulongEncoding
        },
        {
          (FormatCode) (byte) 83,
          (EncodingBase) AmqpEncoding.ulongEncoding
        },
        {
          (FormatCode) (byte) 68,
          (EncodingBase) AmqpEncoding.ulongEncoding
        },
        {
          (FormatCode) (byte) 81,
          (EncodingBase) AmqpEncoding.byteEncoding
        },
        {
          (FormatCode) (byte) 97,
          (EncodingBase) AmqpEncoding.shortEncoding
        },
        {
          (FormatCode) (byte) 113,
          (EncodingBase) AmqpEncoding.intEncoding
        },
        {
          (FormatCode) (byte) 84,
          (EncodingBase) AmqpEncoding.intEncoding
        },
        {
          (FormatCode) (byte) 129,
          (EncodingBase) AmqpEncoding.longEncoding
        },
        {
          (FormatCode) (byte) 85,
          (EncodingBase) AmqpEncoding.longEncoding
        },
        {
          (FormatCode) (byte) 114,
          (EncodingBase) AmqpEncoding.floatEncoding
        },
        {
          (FormatCode) (byte) 130,
          (EncodingBase) AmqpEncoding.doubleEncoding
        },
        {
          (FormatCode) (byte) 148,
          (EncodingBase) AmqpEncoding.decimal128Encoding
        },
        {
          (FormatCode) (byte) 115,
          (EncodingBase) AmqpEncoding.charEncoding
        },
        {
          (FormatCode) (byte) 131,
          (EncodingBase) AmqpEncoding.timeStampEncoding
        },
        {
          (FormatCode) (byte) 152,
          (EncodingBase) AmqpEncoding.uuidEncoding
        },
        {
          (FormatCode) (byte) 160,
          (EncodingBase) AmqpEncoding.binaryEncoding
        },
        {
          (FormatCode) (byte) 176,
          (EncodingBase) AmqpEncoding.binaryEncoding
        },
        {
          (FormatCode) (byte) 163,
          (EncodingBase) AmqpEncoding.symbolEncoding
        },
        {
          (FormatCode) (byte) 179,
          (EncodingBase) AmqpEncoding.symbolEncoding
        },
        {
          (FormatCode) (byte) 161,
          (EncodingBase) AmqpEncoding.stringEncoding
        },
        {
          (FormatCode) (byte) 177,
          (EncodingBase) AmqpEncoding.stringEncoding
        },
        {
          (FormatCode) (byte) 69,
          (EncodingBase) AmqpEncoding.listEncoding
        },
        {
          (FormatCode) (byte) 192,
          (EncodingBase) AmqpEncoding.listEncoding
        },
        {
          (FormatCode) (byte) 208,
          (EncodingBase) AmqpEncoding.listEncoding
        },
        {
          (FormatCode) (byte) 193,
          (EncodingBase) AmqpEncoding.mapEncoding
        },
        {
          (FormatCode) (byte) 209,
          (EncodingBase) AmqpEncoding.mapEncoding
        },
        {
          (FormatCode) (byte) 224,
          (EncodingBase) AmqpEncoding.arrayEncoding
        },
        {
          (FormatCode) (byte) 240,
          (EncodingBase) AmqpEncoding.arrayEncoding
        },
        {
          (FormatCode) (byte) 0,
          (EncodingBase) AmqpEncoding.describedTypeEncoding
        }
      };
    }

    public static EncodingBase GetEncoding(object value)
    {
      EncodingBase encoding = (EncodingBase) null;
      Type type = value.GetType();
      if (AmqpEncoding.encodingsByType.TryGetValue(type, out encoding))
        return encoding;
      if (type.IsArray)
        return (EncodingBase) AmqpEncoding.arrayEncoding;
      switch (value)
      {
        case IList _:
          return (EncodingBase) AmqpEncoding.listEncoding;
        case DescribedType _:
          return (EncodingBase) AmqpEncoding.describedTypeEncoding;
        default:
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidType((object) type.ToString()));
      }
    }

    public static EncodingBase GetEncoding(Type type)
    {
      EncodingBase encoding = (EncodingBase) null;
      if (AmqpEncoding.encodingsByType.TryGetValue(type, out encoding))
        return encoding;
      if (type.IsArray)
        return (EncodingBase) AmqpEncoding.arrayEncoding;
      if (typeof (IList).IsAssignableFrom(type))
        return (EncodingBase) AmqpEncoding.listEncoding;
      if (typeof (DescribedType).IsAssignableFrom(type))
        return (EncodingBase) AmqpEncoding.describedTypeEncoding;
      throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidType((object) type.ToString()));
    }

    public static EncodingBase GetEncoding(FormatCode formatCode)
    {
      EncodingBase encodingBase;
      return AmqpEncoding.encodingsByCode.TryGetValue(formatCode, out encodingBase) ? encodingBase : (EncodingBase) null;
    }

    public static int GetEncodeWidthBySize(int size) => size > (int) byte.MaxValue ? 4 : 1;

    public static int GetEncodeWidthByCountAndSize(int count, int valueSize) => count >= (int) byte.MaxValue || valueSize >= (int) byte.MaxValue ? 4 : 1;

    public static FormatCode ReadFormatCode(ByteBuffer buffer)
    {
      int type = (int) AmqpBitConverter.ReadUByte(buffer);
      byte extType = 0;
      if (FormatCode.HasExtType((byte) type))
        extType = AmqpBitConverter.ReadUByte(buffer);
      return new FormatCode((byte) type, extType);
    }

    public static void ReadCount(
      ByteBuffer buffer,
      FormatCode formatCode,
      FormatCode formatCode8,
      FormatCode formatCode32,
      out int count)
    {
      if (formatCode == formatCode8)
      {
        count = (int) AmqpBitConverter.ReadUByte(buffer);
      }
      else
      {
        if (!(formatCode == formatCode32))
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        count = (int) AmqpBitConverter.ReadUInt(buffer);
      }
    }

    public static void ReadSizeAndCount(
      ByteBuffer buffer,
      FormatCode formatCode,
      FormatCode formatCode8,
      FormatCode formatCode32,
      out int size,
      out int count)
    {
      if (formatCode == formatCode8)
      {
        size = (int) AmqpBitConverter.ReadUByte(buffer);
        count = (int) AmqpBitConverter.ReadUByte(buffer);
      }
      else
      {
        if (!(formatCode == formatCode32))
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        size = (int) AmqpBitConverter.ReadUInt(buffer);
        count = (int) AmqpBitConverter.ReadUInt(buffer);
      }
    }

    public static int GetObjectEncodeSize(object value)
    {
      if (value == null)
        return 1;
      return value is IAmqpSerializable amqpSerializable ? amqpSerializable.EncodeSize : AmqpEncoding.GetEncoding(value).GetObjectEncodeSize(value, false);
    }

    public static void EncodeNull(ByteBuffer buffer) => AmqpBitConverter.WriteUByte(buffer, (byte) 64);

    public static void EncodeObject(object value, ByteBuffer buffer)
    {
      if (value == null)
        AmqpEncoding.EncodeNull(buffer);
      else if (value is IAmqpSerializable amqpSerializable)
        amqpSerializable.Encode(buffer);
      else
        AmqpEncoding.GetEncoding(value).EncodeObject(value, false, buffer);
    }

    public static object DecodeObject(ByteBuffer buffer)
    {
      FormatCode formatCode = AmqpEncoding.ReadFormatCode(buffer);
      return formatCode == (FormatCode) (byte) 64 ? (object) null : AmqpEncoding.DecodeObject(buffer, formatCode);
    }

    public static object DecodeObject(ByteBuffer buffer, FormatCode formatCode)
    {
      EncodingBase encodingBase;
      if (AmqpEncoding.encodingsByCode.TryGetValue(formatCode, out encodingBase))
        return encodingBase.DecodeObject(buffer, formatCode);
      throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
    }

    public static AmqpException GetEncodingException(string message) => new AmqpException(AmqpError.InvalidField, message);
  }
}
