// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.NumberPartitionKeyComponent
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class NumberPartitionKeyComponent : IPartitionKeyComponent
  {
    private readonly double value;
    public static readonly NumberPartitionKeyComponent Zero = new NumberPartitionKeyComponent(0.0);

    public NumberPartitionKeyComponent(double value) => this.value = value;

    public double Value => this.value;

    public int CompareTo(IPartitionKeyComponent other) => other is NumberPartitionKeyComponent partitionKeyComponent ? this.value.CompareTo(partitionKeyComponent.value) : throw new ArgumentException(nameof (other));

    public int GetTypeOrdinal() => 5;

    public override int GetHashCode() => this.value.GetHashCode();

    public void JsonEncode(JsonWriter writer) => writer.WriteValue(this.value);

    public object ToObject() => (object) this.value;

    public IPartitionKeyComponent Truncate() => (IPartitionKeyComponent) this;

    public void WriteForHashing(BinaryWriter writer)
    {
      writer.Write((byte) 5);
      writer.Write(this.value);
    }

    public void WriteForHashingV2(BinaryWriter writer)
    {
      writer.Write((byte) 5);
      writer.Write(this.value);
    }

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter)
    {
      binaryWriter.Write((byte) 5);
      ulong num1 = NumberPartitionKeyComponent.EncodeDoubleAsUInt64(this.value);
      binaryWriter.Write((byte) (num1 >> 56));
      ulong num2 = num1 << 8;
      byte num3 = 0;
      bool flag = true;
      do
      {
        if (!flag)
          binaryWriter.Write(num3);
        else
          flag = false;
        num3 = (byte) (num2 >> 56 | 1UL);
        num2 <<= 7;
      }
      while (num2 != 0UL);
      binaryWriter.Write((byte) ((uint) num3 & 254U));
    }

    public static IPartitionKeyComponent FromHexEncodedBinaryString(
      byte[] byteString,
      ref int byteStringOffset)
    {
      int num1 = 64;
      ulong num2 = 0;
      int num3 = num1 - 8;
      ulong num4 = num2 | Convert.ToUInt64(byteString[byteStringOffset++]) << num3;
      while (byteStringOffset < byteString.Length)
      {
        byte num5 = byteString[byteStringOffset++];
        num3 -= 7;
        num4 |= Convert.ToUInt64((int) num5 >> 1) << num3;
        if (((int) num5 & 1) == 0)
          return (IPartitionKeyComponent) new NumberPartitionKeyComponent(NumberPartitionKeyComponent.DecodeDoubleFromUInt64(num4));
      }
      throw new InvalidDataException("Incorrect byte string without termination");
    }

    private static ulong EncodeDoubleAsUInt64(double value)
    {
      ulong int64Bits = (ulong) BitConverter.DoubleToInt64Bits(value);
      ulong num = 9223372036854775808;
      return int64Bits >= num ? ~int64Bits + 1UL : int64Bits ^ num;
    }

    private static double DecodeDoubleFromUInt64(ulong value)
    {
      ulong num = 9223372036854775808;
      value = value < num ? (ulong) ~((long) value - 1L) : value ^ num;
      return BitConverter.Int64BitsToDouble((long) value);
    }
  }
}
