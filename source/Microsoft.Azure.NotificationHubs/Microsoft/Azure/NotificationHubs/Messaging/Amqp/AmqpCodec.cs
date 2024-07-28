// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.AmqpCodec
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal static class AmqpCodec
  {
    private static Dictionary<string, Func<AmqpDescribed>> knownTypesByName = new Dictionary<string, Func<AmqpDescribed>>()
    {
      {
        Open.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Open())
      },
      {
        Close.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Close())
      },
      {
        Begin.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Begin())
      },
      {
        End.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new End())
      },
      {
        Attach.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Attach())
      },
      {
        Detach.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Detach())
      },
      {
        Transfer.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Transfer())
      },
      {
        Disposition.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Disposition())
      },
      {
        Flow.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Flow())
      },
      {
        Error.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Error())
      },
      {
        Source.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Source())
      },
      {
        Target.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Target())
      },
      {
        Received.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Received())
      },
      {
        Accepted.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Accepted())
      },
      {
        Released.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Released())
      },
      {
        Rejected.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Rejected())
      },
      {
        Modified.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Modified())
      },
      {
        DeleteOnClose.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnClose())
      },
      {
        DeleteOnNoLinks.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoLinks())
      },
      {
        DeleteOnNoMessages.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoMessages())
      },
      {
        DeleteOnNoLinksOrMessages.Name,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoLinksOrMessages())
      }
    };
    private static Dictionary<ulong, Func<AmqpDescribed>> knownTypesByCode = new Dictionary<ulong, Func<AmqpDescribed>>()
    {
      {
        Open.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Open())
      },
      {
        Close.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Close())
      },
      {
        Begin.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Begin())
      },
      {
        End.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new End())
      },
      {
        Attach.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Attach())
      },
      {
        Detach.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Detach())
      },
      {
        Transfer.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Transfer())
      },
      {
        Disposition.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Disposition())
      },
      {
        Flow.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Flow())
      },
      {
        Error.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Error())
      },
      {
        Source.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Source())
      },
      {
        Target.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Target())
      },
      {
        Received.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Received())
      },
      {
        Accepted.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Accepted())
      },
      {
        Released.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Released())
      },
      {
        Rejected.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Rejected())
      },
      {
        Modified.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new Modified())
      },
      {
        DeleteOnClose.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnClose())
      },
      {
        DeleteOnNoLinks.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoLinks())
      },
      {
        DeleteOnNoMessages.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoMessages())
      },
      {
        DeleteOnNoLinksOrMessages.Code,
        (Func<AmqpDescribed>) (() => (AmqpDescribed) new DeleteOnNoLinksOrMessages())
      }
    };

    public static int MinimumFrameDecodeSize => 8;

    public static int GetFrameSize(ByteBuffer buffer) => (int) AmqpBitConverter.PeekUInt(buffer);

    public static void RegisterKnownTypes(string name, ulong code, Func<AmqpDescribed> ctor)
    {
      lock (AmqpCodec.knownTypesByCode)
      {
        if (AmqpCodec.knownTypesByName.ContainsKey(name))
          return;
        AmqpCodec.knownTypesByName.Add(name, ctor);
        AmqpCodec.knownTypesByCode.Add(code, ctor);
      }
    }

    public static int GetBooleanEncodeSize(bool? value) => BooleanEncoding.GetEncodeSize(value);

    public static int GetUByteEncodeSize(byte? value) => UByteEncoding.GetEncodeSize(value);

    public static int GetUShortEncodeSize(ushort? value) => UShortEncoding.GetEncodeSize(value);

    public static int GetUIntEncodeSize(uint? value) => UIntEncoding.GetEncodeSize(value);

    public static int GetULongEncodeSize(ulong? value) => ULongEncoding.GetEncodeSize(value);

    public static int GetByteEncodeSize(sbyte? value) => ByteEncoding.GetEncodeSize(value);

    public static int GetShortEncodeSize(short? value) => ShortEncoding.GetEncodeSize(value);

    public static int GetIntEncodeSize(int? value) => IntEncoding.GetEncodeSize(value);

    public static int GetLongEncodeSize(long? value) => LongEncoding.GetEncodeSize(value);

    public static int GetFloatEncodeSize(float? value) => FloatEncoding.GetEncodeSize(value);

    public static int GetDoubleEncodeSize(double? value) => DoubleEncoding.GetEncodeSize(value);

    public static int GetCharEncodeSize(char? value) => CharEncoding.GetEncodeSize(value);

    public static int GetTimeStampEncodeSize(DateTime? value) => TimeStampEncoding.GetEncodeSize(value);

    public static int GetUuidEncodeSize(Guid? value) => UuidEncoding.GetEncodeSize(value);

    public static int GetBinaryEncodeSize(ArraySegment<byte> value) => BinaryEncoding.GetEncodeSize(value);

    public static int GetSymbolEncodeSize(AmqpSymbol value) => SymbolEncoding.GetEncodeSize(value);

    public static int GetStringEncodeSize(string value) => StringEncoding.GetEncodeSize(value);

    public static int GetListEncodeSize(IList value) => ListEncoding.GetEncodeSize(value);

    public static int GetMapEncodeSize(AmqpMap value) => MapEncoding.GetEncodeSize(value);

    public static int GetArrayEncodeSize<T>(T[] value) => ArrayEncoding.GetEncodeSize<T>(value);

    public static int GetSerializableEncodeSize(IAmqpSerializable value) => value == null ? 1 : value.EncodeSize;

    public static int GetMultipleEncodeSize<T>(Multiple<T> value) => Multiple<T>.GetEncodeSize(value);

    public static int GetObjectEncodeSize(object value) => AmqpEncoding.GetObjectEncodeSize(value);

    public static void EncodeBoolean(bool? data, ByteBuffer buffer) => BooleanEncoding.Encode(data, buffer);

    public static void EncodeUByte(byte? data, ByteBuffer buffer) => UByteEncoding.Encode(data, buffer);

    public static void EncodeUShort(ushort? data, ByteBuffer buffer) => UShortEncoding.Encode(data, buffer);

    public static void EncodeUInt(uint? data, ByteBuffer buffer) => UIntEncoding.Encode(data, buffer);

    public static void EncodeULong(ulong? data, ByteBuffer buffer) => ULongEncoding.Encode(data, buffer);

    public static void EncodeByte(sbyte? data, ByteBuffer buffer) => ByteEncoding.Encode(data, buffer);

    public static void EncodeShort(short? data, ByteBuffer buffer) => ShortEncoding.Encode(data, buffer);

    public static void EncodeInt(int? data, ByteBuffer buffer) => IntEncoding.Encode(data, buffer);

    public static void EncodeLong(long? data, ByteBuffer buffer) => LongEncoding.Encode(data, buffer);

    public static void EncodeChar(char? data, ByteBuffer buffer) => CharEncoding.Encode(data, buffer);

    public static void EncodeFloat(float? data, ByteBuffer buffer) => FloatEncoding.Encode(data, buffer);

    public static void EncodeDouble(double? data, ByteBuffer buffer) => DoubleEncoding.Encode(data, buffer);

    public static void EncodeTimeStamp(DateTime? data, ByteBuffer buffer) => TimeStampEncoding.Encode(data, buffer);

    public static void EncodeUuid(Guid? data, ByteBuffer buffer) => UuidEncoding.Encode(data, buffer);

    public static void EncodeBinary(ArraySegment<byte> data, ByteBuffer buffer) => BinaryEncoding.Encode(data, buffer);

    public static void EncodeString(string data, ByteBuffer buffer) => StringEncoding.Encode(data, buffer);

    public static void EncodeSymbol(AmqpSymbol data, ByteBuffer buffer) => SymbolEncoding.Encode(data, buffer);

    public static void EncodeList(IList data, ByteBuffer buffer) => ListEncoding.Encode(data, buffer);

    public static void EncodeMap(AmqpMap data, ByteBuffer buffer) => MapEncoding.Encode(data, buffer);

    public static void EncodeArray<T>(T[] data, ByteBuffer buffer) => ArrayEncoding.Encode<T>(data, buffer);

    public static void EncodeSerializable(IAmqpSerializable data, ByteBuffer buffer)
    {
      if (data == null)
        AmqpEncoding.EncodeNull(buffer);
      else
        data.Encode(buffer);
    }

    public static void EncodeMultiple<T>(Multiple<T> data, ByteBuffer buffer) => Multiple<T>.Encode(data, buffer);

    public static void EncodeObject(object data, ByteBuffer buffer) => AmqpEncoding.EncodeObject(data, buffer);

    public static bool? DecodeBoolean(ByteBuffer buffer) => BooleanEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static byte? DecodeUByte(ByteBuffer buffer) => UByteEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static ushort? DecodeUShort(ByteBuffer buffer) => UShortEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static uint? DecodeUInt(ByteBuffer buffer) => UIntEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static ulong? DecodeULong(ByteBuffer buffer) => ULongEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static sbyte? DecodeByte(ByteBuffer buffer) => ByteEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static short? DecodeShort(ByteBuffer buffer) => ShortEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static int? DecodeInt(ByteBuffer buffer) => IntEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static long? DecodeLong(ByteBuffer buffer) => LongEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static float? DecodeFloat(ByteBuffer buffer) => FloatEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static double? DecodeDouble(ByteBuffer buffer) => DoubleEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static char? DecodeChar(ByteBuffer buffer) => CharEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static DateTime? DecodeTimeStamp(ByteBuffer buffer) => TimeStampEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static Guid? DecodeUuid(ByteBuffer buffer) => UuidEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static ArraySegment<byte> DecodeBinary(ByteBuffer buffer) => BinaryEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static string DecodeString(ByteBuffer buffer) => StringEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static AmqpSymbol DecodeSymbol(ByteBuffer buffer) => SymbolEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static IList DecodeList(ByteBuffer buffer) => ListEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static AmqpMap DecodeMap(ByteBuffer buffer) => MapEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public static T DecodeMap<T>(ByteBuffer buffer) where T : RestrictedMap, new()
    {
      AmqpMap map = MapEncoding.Decode(buffer, (FormatCode) (byte) 0);
      T obj = default (T);
      if (map != null)
      {
        obj = new T();
        obj.SetMap(map);
      }
      return obj;
    }

    public static T[] DecodeArray<T>(ByteBuffer buffer) => ArrayEncoding.Decode<T>(buffer, (FormatCode) (byte) 0);

    public static Multiple<T> DecodeMultiple<T>(ByteBuffer buffer) => Multiple<T>.Decode(buffer);

    public static object DecodeObject(ByteBuffer buffer)
    {
      FormatCode formatCode = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode == (FormatCode) (byte) 64)
        return (object) null;
      if (!(formatCode == (FormatCode) (byte) 0))
        return AmqpEncoding.DecodeObject(buffer, formatCode);
      object descriptor = AmqpCodec.DecodeObject(buffer);
      Func<AmqpDescribed> func = (Func<AmqpDescribed>) null;
      if (descriptor is AmqpSymbol amqpSymbol)
        AmqpCodec.knownTypesByName.TryGetValue(amqpSymbol.Value, out func);
      else if (descriptor is ulong key)
        AmqpCodec.knownTypesByCode.TryGetValue(key, out func);
      if (func != null)
      {
        AmqpDescribed amqpDescribed = func();
        amqpDescribed.DecodeValue(buffer);
        return (object) amqpDescribed;
      }
      object obj = AmqpCodec.DecodeObject(buffer);
      return (object) new DescribedType(descriptor, obj);
    }

    public static AmqpDescribed DecodeAmqpDescribed(ByteBuffer buffer) => AmqpCodec.DecodeAmqpDescribed(buffer, AmqpCodec.knownTypesByName, AmqpCodec.knownTypesByCode);

    public static AmqpDescribed DecodeAmqpDescribed(
      ByteBuffer buffer,
      Dictionary<string, Func<AmqpDescribed>> byName,
      Dictionary<ulong, Func<AmqpDescribed>> byCode)
    {
      AmqpDescribed amqpDescribed = AmqpCodec.CreateAmqpDescribed(buffer, byName, byCode);
      amqpDescribed?.DecodeValue(buffer);
      return amqpDescribed;
    }

    public static AmqpDescribed CreateAmqpDescribed(ByteBuffer buffer) => AmqpCodec.CreateAmqpDescribed(buffer, AmqpCodec.knownTypesByName, AmqpCodec.knownTypesByCode);

    public static AmqpDescribed CreateAmqpDescribed(
      ByteBuffer buffer,
      Dictionary<string, Func<AmqpDescribed>> byName,
      Dictionary<ulong, Func<AmqpDescribed>> byCode)
    {
      FormatCode formatCode1 = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode1 == (FormatCode) (byte) 64)
        return (AmqpDescribed) null;
      EncodingBase.VerifyFormatCode(formatCode1, (FormatCode) (byte) 0, buffer.Offset);
      Func<AmqpDescribed> func = (Func<AmqpDescribed>) null;
      FormatCode formatCode2 = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode2 == (FormatCode) (byte) 163 || formatCode2 == (FormatCode) (byte) 179)
      {
        AmqpSymbol amqpSymbol = SymbolEncoding.Decode(buffer, formatCode2);
        byName.TryGetValue(amqpSymbol.Value, out func);
      }
      else if (formatCode2 == (FormatCode) (byte) 68 || formatCode2 == (FormatCode) (byte) 128 || formatCode2 == (FormatCode) (byte) 83)
      {
        ulong key = ULongEncoding.Decode(buffer, formatCode2).Value;
        byCode.TryGetValue(key, out func);
      }
      if (func == null)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode2, (object) buffer.Offset));
      return func();
    }

    public static T DecodeKnownType<T>(ByteBuffer buffer) where T : class, IAmqpSerializable, new()
    {
      if (AmqpEncoding.ReadFormatCode(buffer) == (FormatCode) (byte) 64)
        return default (T);
      T obj = new T();
      obj.Decode(buffer);
      return obj;
    }
  }
}
