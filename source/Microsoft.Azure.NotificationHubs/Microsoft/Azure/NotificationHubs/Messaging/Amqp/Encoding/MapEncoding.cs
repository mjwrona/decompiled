// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.MapEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class MapEncoding : EncodingBase
  {
    public MapEncoding()
      : base((FormatCode) (byte) 209)
    {
    }

    public static int GetValueSize(AmqpMap value)
    {
      int valueSize = 0;
      if (value.Count > 0)
      {
        foreach (KeyValuePair<MapKey, object> keyValuePair in (IEnumerable<KeyValuePair<MapKey, object>>) value)
        {
          valueSize += AmqpEncoding.GetObjectEncodeSize(keyValuePair.Key.Key);
          valueSize += AmqpEncoding.GetObjectEncodeSize(keyValuePair.Value);
        }
      }
      return valueSize;
    }

    public static int GetEncodeSize(AmqpMap value) => value != null ? 1 + MapEncoding.GetEncodeWidth(value) * 2 + value.ValueSize : 1;

    public static void Encode(AmqpMap value, ByteBuffer buffer)
    {
      if (value == null)
      {
        AmqpEncoding.EncodeNull(buffer);
      }
      else
      {
        int encodeWidth = MapEncoding.GetEncodeWidth(value);
        AmqpBitConverter.WriteUByte(buffer, encodeWidth == 1 ? (byte) 193 : (byte) 209);
        int size = encodeWidth + value.ValueSize;
        MapEncoding.Encode(value, encodeWidth, size, buffer);
      }
    }

    public static AmqpMap Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return (AmqpMap) null;
      int size;
      int count;
      AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 193, (FormatCode) (byte) 209, out size, out count);
      AmqpMap map = new AmqpMap();
      MapEncoding.ReadMapValue(buffer, map, size, count);
      return map;
    }

    public static void ReadMapValue(ByteBuffer buffer, AmqpMap map, int size, int count)
    {
      for (; count > 0; count -= 2)
      {
        object key = AmqpEncoding.DecodeObject(buffer);
        object obj = AmqpCodec.DecodeObject(buffer);
        map[new MapKey(key)] = obj;
      }
    }

    private static int GetEncodeWidth(AmqpMap value) => AmqpEncoding.GetEncodeWidthByCountAndSize(value.Count * 2, value.ValueSize);

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 8 + MapEncoding.GetValueSize((AmqpMap) value) : MapEncoding.GetEncodeSize((AmqpMap) value);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
      {
        AmqpMap amqpMap = (AmqpMap) value;
        int size = 4 + amqpMap.ValueSize;
        MapEncoding.Encode(amqpMap, 4, size, buffer);
      }
      else
        MapEncoding.Encode((AmqpMap) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) MapEncoding.Decode(buffer, formatCode);

    private static void Encode(AmqpMap value, int width, int size, ByteBuffer buffer)
    {
      if (width == 1)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) size);
        AmqpBitConverter.WriteUByte(buffer, (byte) (value.Count * 2));
      }
      else
      {
        AmqpBitConverter.WriteUInt(buffer, (uint) size);
        AmqpBitConverter.WriteUInt(buffer, (uint) (value.Count * 2));
      }
      if (value.Count <= 0)
        return;
      foreach (KeyValuePair<MapKey, object> keyValuePair in (IEnumerable<KeyValuePair<MapKey, object>>) value)
      {
        AmqpEncoding.EncodeObject(keyValuePair.Key.Key, buffer);
        AmqpEncoding.EncodeObject(keyValuePair.Value, buffer);
      }
    }
  }
}
