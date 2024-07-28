// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Data
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Data : AmqpDescribed
  {
    public static readonly string Name = "amqp:data:binary";
    public static readonly ulong Code = 117;

    public Data()
      : base((AmqpSymbol) Data.Name, Data.Code)
    {
    }

    public static ArraySegment<byte> GetEncodedPrefix(int valueLength)
    {
      byte[] numArray1 = new byte[8];
      numArray1[1] = (byte) 83;
      numArray1[2] = (byte) Data.Code;
      byte[] numArray2 = numArray1;
      int count;
      if (valueLength <= (int) byte.MaxValue)
      {
        numArray2[3] = (byte) 160;
        numArray2[4] = (byte) valueLength;
        count = 5;
      }
      else
      {
        numArray2[3] = (byte) 176;
        AmqpBitConverter.WriteUInt(numArray2, 4, (uint) valueLength);
        count = 8;
      }
      return new ArraySegment<byte>(numArray2, 0, count);
    }

    public override int GetValueEncodeSize() => BinaryEncoding.GetEncodeSize((ArraySegment<byte>) this.Value);

    public override void EncodeValue(ByteBuffer buffer) => BinaryEncoding.Encode((ArraySegment<byte>) this.Value, buffer);

    public override void DecodeValue(ByteBuffer buffer) => this.Value = (object) BinaryEncoding.Decode(buffer, (FormatCode) (byte) 0);

    public override string ToString() => "data()";
  }
}
