// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.TimeStampEncoding
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal sealed class TimeStampEncoding : EncodingBase
  {
    private static readonly long MaxMilliseconds = (long) (DateTime.MaxValue.ToUniversalTime() - AmqpConstants.StartOfEpoch).TotalMilliseconds;

    public TimeStampEncoding()
      : base((FormatCode) (byte) 131)
    {
    }

    public static int GetEncodeSize(DateTime? value) => !value.HasValue ? 1 : 9;

    public static void Encode(DateTime? value, ByteBuffer buffer)
    {
      if (value.HasValue)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 131);
        AmqpBitConverter.WriteLong(buffer, TimeStampEncoding.GetMilliseconds(value.Value));
      }
      else
        AmqpEncoding.EncodeNull(buffer);
    }

    public static DateTime? Decode(ByteBuffer buffer, FormatCode formatCode) => formatCode == (FormatCode) (byte) 0 && (formatCode = AmqpEncoding.ReadFormatCode(buffer)) == (FormatCode) (byte) 64 ? new DateTime?() : new DateTime?(TimeStampEncoding.ToDateTime(AmqpBitConverter.ReadLong(buffer)));

    public override int GetObjectEncodeSize(object value, bool arrayEncoding) => arrayEncoding ? 8 : TimeStampEncoding.GetEncodeSize(new DateTime?((DateTime) value));

    public override void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer)
    {
      if (arrayEncoding)
        AmqpBitConverter.WriteLong(buffer, TimeStampEncoding.GetMilliseconds((DateTime) value));
      else
        TimeStampEncoding.Encode(new DateTime?((DateTime) value), buffer);
    }

    public override object DecodeObject(ByteBuffer buffer, FormatCode formatCode) => (object) TimeStampEncoding.Decode(buffer, formatCode);

    public static long GetMilliseconds(DateTime value) => (long) (value.ToUniversalTime() - AmqpConstants.StartOfEpoch).TotalMilliseconds;

    public static DateTime ToDateTime(long milliseconds)
    {
      milliseconds = milliseconds < 0L ? 0L : milliseconds;
      return milliseconds >= TimeStampEncoding.MaxMilliseconds ? DateTime.MaxValue : AmqpConstants.StartOfEpoch.AddMilliseconds((double) milliseconds);
    }
  }
}
