// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.AmqpDescribed
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal class AmqpDescribed : DescribedType, IAmqpSerializable
  {
    private AmqpSymbol name;
    private ulong code;

    public AmqpDescribed(AmqpSymbol name, ulong code)
      : base(name.Value == null ? (object) code : (object) name, (object) null)
    {
      this.name = name;
      this.code = code;
    }

    public AmqpSymbol DescriptorName => this.name;

    public ulong DescriptorCode => this.code;

    public int EncodeSize => 1 + ULongEncoding.GetEncodeSize(new ulong?(this.DescriptorCode)) + this.GetValueEncodeSize();

    public long Offset { get; set; }

    public long Length { get; set; }

    public static void DecodeDescriptor(ByteBuffer buffer, out AmqpSymbol name, out ulong code)
    {
      name = new AmqpSymbol();
      code = 0UL;
      FormatCode formatCode = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode == (FormatCode) (byte) 0)
        formatCode = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode == (FormatCode) (byte) 163 || formatCode == (FormatCode) (byte) 179)
      {
        name = SymbolEncoding.Decode(buffer, formatCode);
      }
      else
      {
        if (!(formatCode == (FormatCode) (byte) 128) && !(formatCode == (FormatCode) (byte) 68) && !(formatCode == (FormatCode) (byte) 83))
          throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) buffer.Offset));
        code = ULongEncoding.Decode(buffer, formatCode).Value;
      }
    }

    public override string ToString() => this.name.Value;

    public void Encode(ByteBuffer buffer)
    {
      AmqpBitConverter.WriteUByte(buffer, (byte) 0);
      ULongEncoding.Encode(new ulong?(this.DescriptorCode), buffer);
      this.EncodeValue(buffer);
    }

    public void Decode(ByteBuffer buffer)
    {
      AmqpDescribed.DecodeDescriptor(buffer, out this.name, out this.code);
      this.DecodeValue(buffer);
    }

    public virtual int GetValueEncodeSize() => AmqpEncoding.GetObjectEncodeSize(this.Value);

    public virtual void EncodeValue(ByteBuffer buffer) => AmqpEncoding.EncodeObject(this.Value, buffer);

    public virtual void DecodeValue(ByteBuffer buffer) => this.Value = AmqpEncoding.DecodeObject(buffer);

    protected void AddFieldToString(
      bool condition,
      StringBuilder sb,
      string fieldName,
      object value,
      ref int count)
    {
      if (!condition)
        return;
      if (count > 0)
        sb.Append(',');
      if (value is ArraySegment<byte> arraySegment)
      {
        sb.Append(fieldName);
        sb.Append(':');
        int num = Math.Min(arraySegment.Count, 64);
        for (int index = 0; index < num; ++index)
          sb.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:X2}", new object[1]
          {
            (object) arraySegment.Array[arraySegment.Offset + index]
          });
      }
      else
        sb.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
        {
          (object) fieldName,
          value
        });
      ++count;
    }
  }
}
