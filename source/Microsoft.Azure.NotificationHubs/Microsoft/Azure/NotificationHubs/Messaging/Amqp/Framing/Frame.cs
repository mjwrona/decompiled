// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Frame
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Frame : IDisposable
  {
    public const int HeaderSize = 8;
    private const byte DefaultDataOffset = 2;

    public Frame()
      : this(FrameType.Amqp)
    {
    }

    public Frame(FrameType type)
    {
      this.Type = type;
      this.DataOffset = (byte) 2;
    }

    public int Size { get; private set; }

    public byte DataOffset { get; private set; }

    public FrameType Type { get; private set; }

    public ushort Channel { get; set; }

    public Performative Command { get; set; }

    public ArraySegment<byte> Payload { get; set; }

    public ByteBuffer RawByteBuffer { get; private set; }

    public static ByteBuffer EncodeCommand(
      FrameType type,
      ushort channel,
      Performative command,
      int payloadSize)
    {
      int num1 = 8;
      if (command != null)
        num1 += AmqpCodec.GetSerializableEncodeSize((IAmqpSerializable) command);
      int num2 = num1 + payloadSize;
      ByteBuffer buffer = new ByteBuffer(num2, false, false);
      AmqpBitConverter.WriteUInt(buffer, (uint) num2);
      AmqpBitConverter.WriteUByte(buffer, (byte) 2);
      AmqpBitConverter.WriteUByte(buffer, (byte) type);
      AmqpBitConverter.WriteUShort(buffer, channel);
      if (command != null)
        AmqpCodec.EncodeSerializable((IAmqpSerializable) command, buffer);
      return buffer;
    }

    public void Decode(ByteBuffer buffer)
    {
      this.RawByteBuffer = buffer;
      int offset = buffer.Offset;
      int length = buffer.Length;
      this.DecodeHeader(buffer);
      this.DecodeCommand(buffer);
      this.DecodePayload(buffer);
      buffer.AdjustPosition(offset, length);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "FRM({0:X4}|{1}|{2}|{3:X2}", (object) this.Size, (object) this.DataOffset, (object) (byte) this.Type, (object) this.Channel);
      if (this.Command != null)
        stringBuilder1.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "  {0}", new object[1]
        {
          (object) this.Command
        });
      ArraySegment<byte> payload = this.Payload;
      if (payload.Count > 0)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        object[] objArray = new object[1];
        payload = this.Payload;
        objArray[0] = (object) payload.Count;
        stringBuilder2.AppendFormat((IFormatProvider) invariantCulture, ",{0}", objArray);
      }
      stringBuilder1.Append(')');
      return stringBuilder1.ToString();
    }

    private void DecodeHeader(ByteBuffer buffer)
    {
      this.Size = (int) AmqpBitConverter.ReadUInt(buffer);
      this.DataOffset = AmqpBitConverter.ReadUByte(buffer);
      this.Type = (FrameType) AmqpBitConverter.ReadUByte(buffer);
      this.Channel = AmqpBitConverter.ReadUShort(buffer);
      buffer.Complete((int) this.DataOffset * 4 - 8);
    }

    private void DecodeCommand(ByteBuffer buffer)
    {
      if (buffer.Length <= 0)
        return;
      this.Command = (Performative) AmqpCodec.DecodeAmqpDescribed(buffer);
    }

    private void DecodePayload(ByteBuffer buffer)
    {
      if (buffer.Length <= 0)
        return;
      this.Payload = new ArraySegment<byte>(buffer.Buffer, buffer.Offset, buffer.Length);
    }

    public void Dispose()
    {
      if (this.RawByteBuffer == null)
        return;
      this.RawByteBuffer.Dispose();
    }
  }
}
