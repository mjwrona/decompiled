// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.DescribedList
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal abstract class DescribedList : AmqpDescribed
  {
    public DescribedList(AmqpSymbol name, ulong code)
      : base(name, code)
    {
    }

    protected abstract int FieldCount { get; }

    public override int GetValueEncodeSize()
    {
      if (this.FieldCount == 0)
        return 1;
      int valueSize = this.OnValueSize();
      int widthByCountAndSize = AmqpEncoding.GetEncodeWidthByCountAndSize(this.FieldCount, valueSize);
      return 1 + widthByCountAndSize + widthByCountAndSize + valueSize;
    }

    public override void EncodeValue(ByteBuffer buffer)
    {
      if (this.FieldCount == 0)
      {
        AmqpBitConverter.WriteUByte(buffer, (byte) 69);
      }
      else
      {
        int widthByCountAndSize = AmqpEncoding.GetEncodeWidthByCountAndSize(this.FieldCount, this.OnValueSize());
        int length;
        if (widthByCountAndSize == 1)
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 192);
          length = buffer.Length;
          buffer.Append(1);
          AmqpBitConverter.WriteUByte(buffer, (byte) this.FieldCount);
        }
        else
        {
          AmqpBitConverter.WriteUByte(buffer, (byte) 208);
          length = buffer.Length;
          buffer.Append(4);
          AmqpBitConverter.WriteUInt(buffer, (uint) this.FieldCount);
        }
        this.OnEncode(buffer);
        int data = buffer.Length - length - widthByCountAndSize;
        if (widthByCountAndSize == 1)
          AmqpBitConverter.WriteUByte(buffer.Buffer, length, (byte) data);
        else
          AmqpBitConverter.WriteUInt(buffer.Buffer, length, (uint) data);
      }
    }

    public override void DecodeValue(ByteBuffer buffer)
    {
      FormatCode formatCode = AmqpEncoding.ReadFormatCode(buffer);
      if (formatCode == (FormatCode) (byte) 69)
        return;
      int size1 = 0;
      int count = 0;
      AmqpEncoding.ReadSizeAndCount(buffer, formatCode, (FormatCode) (byte) 192, (FormatCode) (byte) 208, out size1, out count);
      int offset = buffer.Offset;
      this.DecodeValue(buffer, size1, count);
      if (count - this.FieldCount <= 0)
        return;
      int size2 = size1 - (buffer.Offset - offset) - (formatCode == (FormatCode) (byte) 192 ? 1 : 4);
      buffer.Complete(size2);
    }

    public void DecodeValue(ByteBuffer buffer, int size, int count)
    {
      if (count <= 0)
        return;
      this.OnDecode(buffer, count);
      this.EnsureRequired();
    }

    protected virtual void EnsureRequired()
    {
    }

    protected abstract int OnValueSize();

    protected abstract void OnEncode(ByteBuffer buffer);

    protected abstract void OnDecode(ByteBuffer buffer, int count);
  }
}
