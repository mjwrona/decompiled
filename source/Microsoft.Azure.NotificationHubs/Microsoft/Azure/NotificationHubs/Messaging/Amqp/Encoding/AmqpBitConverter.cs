// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.AmqpBitConverter
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal static class AmqpBitConverter
  {
    public static sbyte ReadByte(ByteBuffer buffer)
    {
      buffer.Validate(false, 1);
      int num = (int) (sbyte) buffer.Buffer[buffer.Offset];
      buffer.Complete(1);
      return (sbyte) num;
    }

    public static byte ReadUByte(ByteBuffer buffer)
    {
      buffer.Validate(false, 1);
      int num = (int) buffer.Buffer[buffer.Offset];
      buffer.Complete(1);
      return (byte) num;
    }

    public static unsafe short ReadShort(ByteBuffer buffer)
    {
      buffer.Validate(false, 2);
      short num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[1];
        numPtr2[1] = *numPtr1;
      }
      buffer.Complete(2);
      return num;
    }

    public static unsafe ushort ReadUShort(ByteBuffer buffer)
    {
      buffer.Validate(false, 2);
      ushort num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[1];
        numPtr2[1] = *numPtr1;
      }
      buffer.Complete(2);
      return num;
    }

    public static unsafe int ReadInt(ByteBuffer buffer)
    {
      buffer.Validate(false, 4);
      int num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[3];
        numPtr2[1] = numPtr1[2];
        numPtr2[2] = numPtr1[1];
        numPtr2[3] = *numPtr1;
      }
      buffer.Complete(4);
      return num;
    }

    public static uint PeekUInt(ByteBuffer buffer)
    {
      buffer.Validate(false, 4);
      return AmqpBitConverter.ReadUInt(buffer.Buffer, buffer.Offset, buffer.Length);
    }

    public static uint ReadUInt(ByteBuffer buffer)
    {
      buffer.Validate(false, 4);
      int num = (int) AmqpBitConverter.ReadUInt(buffer.Buffer, buffer.Offset, buffer.Length);
      buffer.Complete(4);
      return (uint) num;
    }

    public static unsafe uint ReadUInt(byte[] buffer, int offset, int count)
    {
      AmqpBitConverter.Validate(count, 4);
      uint num;
      fixed (byte* numPtr1 = &buffer[offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[3];
        numPtr2[1] = numPtr1[2];
        numPtr2[2] = numPtr1[1];
        numPtr2[3] = *numPtr1;
      }
      return num;
    }

    public static unsafe long ReadLong(ByteBuffer buffer)
    {
      buffer.Validate(false, 8);
      long num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[7];
        numPtr2[1] = numPtr1[6];
        numPtr2[2] = numPtr1[5];
        numPtr2[3] = numPtr1[4];
        numPtr2[4] = numPtr1[3];
        numPtr2[5] = numPtr1[2];
        numPtr2[6] = numPtr1[1];
        numPtr2[7] = *numPtr1;
      }
      buffer.Complete(8);
      return num;
    }

    public static ulong ReadULong(ByteBuffer buffer)
    {
      buffer.Validate(false, 8);
      long num = (long) AmqpBitConverter.ReadULong(buffer.Buffer, buffer.Offset, buffer.Length);
      buffer.Complete(8);
      return (ulong) num;
    }

    public static unsafe ulong ReadULong(byte[] buffer, int offset, int count)
    {
      AmqpBitConverter.Validate(count, 8);
      ulong num;
      fixed (byte* numPtr1 = &buffer[offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[7];
        numPtr2[1] = numPtr1[6];
        numPtr2[2] = numPtr1[5];
        numPtr2[3] = numPtr1[4];
        numPtr2[4] = numPtr1[3];
        numPtr2[5] = numPtr1[2];
        numPtr2[6] = numPtr1[1];
        numPtr2[7] = *numPtr1;
      }
      return num;
    }

    public static unsafe float ReadFloat(ByteBuffer buffer)
    {
      buffer.Validate(false, 4);
      float num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[3];
        numPtr2[1] = numPtr1[2];
        numPtr2[2] = numPtr1[1];
        numPtr2[3] = *numPtr1;
      }
      buffer.Complete(4);
      return num;
    }

    public static unsafe double ReadDouble(ByteBuffer buffer)
    {
      buffer.Validate(false, 8);
      double num;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[7];
        numPtr2[1] = numPtr1[6];
        numPtr2[2] = numPtr1[5];
        numPtr2[3] = numPtr1[4];
        numPtr2[4] = numPtr1[3];
        numPtr2[5] = numPtr1[2];
        numPtr2[6] = numPtr1[1];
        numPtr2[7] = *numPtr1;
      }
      buffer.Complete(8);
      return num;
    }

    public static unsafe Guid ReadUuid(ByteBuffer buffer)
    {
      buffer.Validate(false, 16);
      Guid guid;
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.Offset])
      {
        byte* numPtr2 = (byte*) &guid;
        *numPtr2 = numPtr1[3];
        numPtr2[1] = numPtr1[2];
        numPtr2[2] = numPtr1[1];
        numPtr2[3] = *numPtr1;
        numPtr2[4] = numPtr1[5];
        numPtr2[5] = numPtr1[4];
        numPtr2[6] = numPtr1[7];
        numPtr2[7] = numPtr1[6];
        *(long*) (numPtr2 + 8) = *(long*) (numPtr1 + 8);
      }
      buffer.Complete(16);
      return guid;
    }

    public static void ReadBytes(ByteBuffer buffer, byte[] data, int offset, int count)
    {
      buffer.Validate(false, count);
      Buffer.BlockCopy((Array) buffer.Buffer, buffer.Offset, (Array) data, offset, count);
      buffer.Complete(count);
    }

    public static void WriteByte(ByteBuffer buffer, sbyte data)
    {
      buffer.Validate(true, 1);
      buffer.Buffer[buffer.WritePos] = (byte) data;
      buffer.Append(1);
    }

    public static void WriteUByte(ByteBuffer buffer, byte data)
    {
      buffer.Validate(true, 1);
      buffer.Buffer[buffer.WritePos] = data;
      buffer.Append(1);
    }

    public static void WriteUByte(byte[] buffer, int offset, byte data)
    {
      AmqpBitConverter.Validate(buffer.Length - offset, 1);
      buffer[offset] = data;
    }

    public static unsafe void WriteShort(ByteBuffer buffer, short data)
    {
      buffer.Validate(true, 2);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[1];
        numPtr1[1] = *numPtr2;
      }
      buffer.Append(2);
    }

    public static unsafe void WriteUShort(ByteBuffer buffer, ushort data)
    {
      buffer.Validate(true, 2);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[1];
        numPtr1[1] = *numPtr2;
      }
      buffer.Append(2);
    }

    public static unsafe void WriteUShort(byte[] buffer, int offset, ushort data)
    {
      AmqpBitConverter.Validate(buffer.Length - offset, 2);
      fixed (byte* numPtr1 = &buffer[offset])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[1];
        numPtr1[1] = *numPtr2;
      }
    }

    public static unsafe void WriteInt(ByteBuffer buffer, int data)
    {
      buffer.Validate(true, 4);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[3];
        numPtr1[1] = numPtr2[2];
        numPtr1[2] = numPtr2[1];
        numPtr1[3] = *numPtr2;
      }
      buffer.Append(4);
    }

    public static unsafe void WriteUInt(ByteBuffer buffer, uint data)
    {
      buffer.Validate(true, 4);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[3];
        numPtr1[1] = numPtr2[2];
        numPtr1[2] = numPtr2[1];
        numPtr1[3] = *numPtr2;
      }
      buffer.Append(4);
    }

    public static unsafe void WriteUInt(byte[] buffer, int offset, uint data)
    {
      AmqpBitConverter.Validate(buffer.Length - offset, 4);
      fixed (byte* numPtr1 = &buffer[offset])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[3];
        numPtr1[1] = numPtr2[2];
        numPtr1[2] = numPtr2[1];
        numPtr1[3] = *numPtr2;
      }
    }

    public static unsafe void WriteLong(ByteBuffer buffer, long data)
    {
      buffer.Validate(true, 8);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[7];
        numPtr1[1] = numPtr2[6];
        numPtr1[2] = numPtr2[5];
        numPtr1[3] = numPtr2[4];
        numPtr1[4] = numPtr2[3];
        numPtr1[5] = numPtr2[2];
        numPtr1[6] = numPtr2[1];
        numPtr1[7] = *numPtr2;
      }
      buffer.Append(8);
    }

    public static unsafe void WriteULong(ByteBuffer buffer, ulong data)
    {
      buffer.Validate(true, 8);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[7];
        numPtr1[1] = numPtr2[6];
        numPtr1[2] = numPtr2[5];
        numPtr1[3] = numPtr2[4];
        numPtr1[4] = numPtr2[3];
        numPtr1[5] = numPtr2[2];
        numPtr1[6] = numPtr2[1];
        numPtr1[7] = *numPtr2;
      }
      buffer.Append(8);
    }

    public static unsafe void WriteFloat(ByteBuffer buffer, float data)
    {
      buffer.Validate(true, 4);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[3];
        numPtr1[1] = numPtr2[2];
        numPtr1[2] = numPtr2[1];
        numPtr1[3] = *numPtr2;
      }
      buffer.Append(4);
    }

    public static unsafe void WriteDouble(ByteBuffer buffer, double data)
    {
      buffer.Validate(true, 8);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[7];
        numPtr1[1] = numPtr2[6];
        numPtr1[2] = numPtr2[5];
        numPtr1[3] = numPtr2[4];
        numPtr1[4] = numPtr2[3];
        numPtr1[5] = numPtr2[2];
        numPtr1[6] = numPtr2[1];
        numPtr1[7] = *numPtr2;
      }
      buffer.Append(8);
    }

    public static unsafe void WriteUuid(ByteBuffer buffer, Guid data)
    {
      buffer.Validate(true, 16);
      fixed (byte* numPtr1 = &buffer.Buffer[buffer.WritePos])
      {
        byte* numPtr2 = (byte*) &data;
        *numPtr1 = numPtr2[3];
        numPtr1[1] = numPtr2[2];
        numPtr1[2] = numPtr2[1];
        numPtr1[3] = *numPtr2;
        numPtr1[4] = numPtr2[5];
        numPtr1[5] = numPtr2[4];
        numPtr1[6] = numPtr2[7];
        numPtr1[7] = numPtr2[6];
        *(long*) (numPtr1 + 8) = *(long*) (numPtr2 + 8);
      }
      buffer.Append(16);
    }

    public static void WriteBytes(ByteBuffer buffer, byte[] data, int offset, int count)
    {
      buffer.Validate(true, count);
      Buffer.BlockCopy((Array) data, offset, (Array) buffer.Buffer, buffer.WritePos, count);
      buffer.Append(count);
    }

    private static void Validate(int bufferSize, int dataSize)
    {
      if (bufferSize < dataSize)
        throw new AmqpException(AmqpError.DecodeError, SRAmqp.AmqpInsufficientBufferSize((object) dataSize, (object) bufferSize));
    }
  }
}
