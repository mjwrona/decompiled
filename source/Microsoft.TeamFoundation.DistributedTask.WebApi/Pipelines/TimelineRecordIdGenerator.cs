// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TimelineRecordIdGenerator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineRecordIdGenerator
  {
    private static readonly byte[] namespaceBytes = new byte[16]
    {
      (byte) 83,
      (byte) 55,
      (byte) 27,
      (byte) 127,
      (byte) 212,
      (byte) 97,
      (byte) 75,
      (byte) 93,
      (byte) 197,
      (byte) 226,
      (byte) 39,
      (byte) 51,
      (byte) 83,
      (byte) 35,
      (byte) 223,
      (byte) 36
    };

    public static Guid GetId(string refName)
    {
      byte[] bytes = Encoding.BigEndianUnicode.GetBytes(refName);
      TimelineRecordIdGenerator.Sha1ForNonSecretPurposes nonSecretPurposes = new TimelineRecordIdGenerator.Sha1ForNonSecretPurposes();
      nonSecretPurposes.Start();
      nonSecretPurposes.Append(TimelineRecordIdGenerator.namespaceBytes);
      nonSecretPurposes.Append(bytes);
      Array.Resize<byte>(ref bytes, 16);
      nonSecretPurposes.Finish(bytes);
      bytes[7] = (byte) ((int) bytes[7] & 15 | 80);
      return new Guid(bytes);
    }

    private struct Sha1ForNonSecretPurposes
    {
      private long length;
      private uint[] w;
      private int pos;

      public void Start()
      {
        if (this.w == null)
          this.w = new uint[85];
        this.length = 0L;
        this.pos = 0;
        this.w[80] = 1732584193U;
        this.w[81] = 4023233417U;
        this.w[82] = 2562383102U;
        this.w[83] = 271733878U;
        this.w[84] = 3285377520U;
      }

      public void Append(byte input)
      {
        this.w[this.pos / 4] = this.w[this.pos / 4] << 8 | (uint) input;
        if (64 != ++this.pos)
          return;
        this.Drain();
      }

      public void Append(byte[] input)
      {
        for (int index = 0; index < input.Length; ++index)
          this.Append(input[index]);
      }

      public void Finish(byte[] output)
      {
        long input = this.length + (long) (8 * this.pos);
        this.Append((byte) 128);
        while (this.pos != 56)
          this.Append((byte) 0);
        this.Append((byte) (input >> 56));
        this.Append((byte) (input >> 48));
        this.Append((byte) (input >> 40));
        this.Append((byte) (input >> 32));
        this.Append((byte) (input >> 24));
        this.Append((byte) (input >> 16));
        this.Append((byte) (input >> 8));
        this.Append((byte) input);
        int num1 = output.Length < 20 ? output.Length : 20;
        for (int index = 0; index != num1; ++index)
        {
          uint num2 = this.w[80 + index / 4];
          output[index] = (byte) (num2 >> 24);
          this.w[80 + index / 4] = num2 << 8;
        }
      }

      private void Drain()
      {
        for (int index = 16; index != 80; ++index)
          this.w[index] = TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol1(this.w[index - 3] ^ this.w[index - 8] ^ this.w[index - 14] ^ this.w[index - 16]);
        uint input1 = this.w[80];
        uint input2 = this.w[81];
        uint num1 = this.w[82];
        uint num2 = this.w[83];
        uint num3 = this.w[84];
        for (int index = 0; index != 20; ++index)
        {
          uint num4 = (uint) ((int) input2 & (int) num1 | ~(int) input2 & (int) num2);
          int num5 = (int) TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num4 + (int) num3 + 1518500249 + (int) this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = (uint) num5;
        }
        for (int index = 20; index != 40; ++index)
        {
          uint num6 = input2 ^ num1 ^ num2;
          int num7 = (int) TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num6 + (int) num3 + 1859775393 + (int) this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = (uint) num7;
        }
        for (int index = 40; index != 60; ++index)
        {
          uint num8 = (uint) ((int) input2 & (int) num1 | (int) input2 & (int) num2 | (int) num1 & (int) num2);
          int num9 = (int) TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num8 + (int) num3 - 1894007588 + (int) this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = (uint) num9;
        }
        for (int index = 60; index != 80; ++index)
        {
          uint num10 = input2 ^ num1 ^ num2;
          int num11 = (int) TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol5(input1) + (int) num10 + (int) num3 - 899497514 + (int) this.w[index];
          num3 = num2;
          num2 = num1;
          num1 = TimelineRecordIdGenerator.Sha1ForNonSecretPurposes.Rol30(input2);
          input2 = input1;
          input1 = (uint) num11;
        }
        this.w[80] += input1;
        this.w[81] += input2;
        this.w[82] += num1;
        this.w[83] += num2;
        this.w[84] += num3;
        this.length += 512L;
        this.pos = 0;
      }

      private static uint Rol1(uint input) => input << 1 | input >> 31;

      private static uint Rol5(uint input) => input << 5 | input >> 27;

      private static uint Rol30(uint input) => input << 30 | input >> 2;
    }
  }
}
