// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.ListEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class ListEncoding : EncodingBase
  {
    public ListEncoding()
      : base((FormatCode) (byte) 208)
    {
    }

    public static int GetEncodeSize(IList value)
    {
      if (value == null || value.Count == 0)
        return 1;
      int valueSize = ListEncoding.GetValueSize(value);
      return 1 + AmqpEncoding.GetEncodeWidthByCountAndSize(value.Count, valueSize) * 2 + valueSize;
    }

    public static void Encode(IList value, ByteBuffer buffer)
    {
      if (value == null)
        AmqpEncoding.EncodeNull(buffer);
      else if (value.Count == 0)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 69);
      }
      else
      {
        int valueSize = ListEncoding.GetValueSize(value);
        int widthByCountAndSize = AmqpEncoding.GetEncodeWidthByCountAndSize(value.Count, valueSize);
        AmqpBitConverter.WriteUByte(buffer, widthByCountAndSize == 1 ? (byte) 192 : (byte) 208);
        int size = widthByCountAndSize + valueSize;
        ListEncoding.Encode(value, widthByCountAndSize, size, buffer);
      }
    }

    public static IList Decode(ByteBuffer buffer, FormatCode formatCode)
    {
      if (formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64)
        return (IList) null;
      IList list = (IList) new List<object>();
      if (formatCode == (FormatCode) (byte) 69)
        return list;
      int count;
      for (AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 192, (FormatCode) (byte) 208, out int _, out count); count > 0; --count)
      {
        object obj = AmqpEncoding.DecodeObject(buffer);
        list.Add(obj);
      }
      return list;
    }

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 8 + ListEncoding.GetValueSize((IList) value) : ListEncoding.GetEncodeSize((IList) value);

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
      {
        IList list = (IList) value;
        int size = 4 + ListEncoding.GetValueSize(list);
        ListEncoding.Encode(list, 4, size, buffer);
      }
      else
        ListEncoding.Encode((IList) value, buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) ListEncoding.Decode(buffer, formatCode);

    public static int GetValueSize(IList value)
    {
      int valueSize = 0;
      if (value.Count > 0)
      {
        foreach (object obj in (IEnumerable) value)
          valueSize += AmqpEncoding.GetObjectEncodeSize(obj);
      }
      return valueSize;
    }

    private static void Encode(IList value, int width, int size, ByteBuffer buffer)
    {
      if (width == 1)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) size);
        AmqpBitConverter.WriteUByte(buffer, (byte) value.Count);
      }
      else
      {
        AmqpBitConverter.WriteUInt(buffer, (uint) size);
        AmqpBitConverter.WriteUInt(buffer, (uint) value.Count);
      }
      if (value.Count <= 0)
        return;
      foreach (object obj in (IEnumerable) value)
        AmqpEncoding.EncodeObject(obj, buffer);
    }
  }
}
