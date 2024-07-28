// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServerMD5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationServerMD5 : HashAlgorithm
  {
    public uint[] m_i;
    public uint[] m_scratchBuffer;
    public byte[] m_inputBuffer;
    public byte[] m_digest;
    private static readonly byte[] padding = new byte[64]
    {
      (byte) 128,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };

    public TeamFoundationServerMD5() => this.Initialize();

    public TeamFoundationServerMD5(byte[] serializedBuffer)
    {
      this.m_i = serializedBuffer.Length == 104 ? TeamFoundationServerMD5.ReadUInt32ArrayFromBytes(serializedBuffer, 0, 2) : throw new ArgumentOutOfRangeException();
      this.m_scratchBuffer = TeamFoundationServerMD5.ReadUInt32ArrayFromBytes(serializedBuffer, 8, 4);
      this.m_inputBuffer = new byte[64];
      Array.Copy((Array) serializedBuffer, 24, (Array) this.m_inputBuffer, 0, this.m_inputBuffer.Length);
      this.m_digest = new byte[16];
      Array.Copy((Array) serializedBuffer, 88, (Array) this.m_digest, 0, this.m_digest.Length);
    }

    public byte[] GetBytes()
    {
      int offset1 = 0;
      byte[] bytes = new byte[104];
      TeamFoundationServerMD5.WriteUInt32Array(bytes, offset1, this.m_i);
      int offset2 = offset1 + 8;
      TeamFoundationServerMD5.WriteUInt32Array(bytes, offset2, this.m_scratchBuffer);
      int destinationIndex1 = offset2 + 16;
      Array.Copy((Array) this.m_inputBuffer, 0, (Array) bytes, destinationIndex1, this.m_inputBuffer.Length);
      int destinationIndex2 = destinationIndex1 + this.m_inputBuffer.Length;
      Array.Copy((Array) this.m_digest, 0, (Array) bytes, destinationIndex2, this.m_digest.Length);
      int num = destinationIndex2 + this.m_digest.Length;
      return bytes;
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize) => this.MD5Update(array, ibStart, (uint) cbSize);

    protected override byte[] HashFinal()
    {
      this.MD5Final();
      return this.m_digest;
    }

    public override void Initialize()
    {
      this.m_i = new uint[2];
      this.m_scratchBuffer = new uint[4];
      this.m_inputBuffer = new byte[64];
      this.m_digest = new byte[16];
      this.m_scratchBuffer[0] = 1732584193U;
      this.m_scratchBuffer[1] = 4023233417U;
      this.m_scratchBuffer[2] = 2562383102U;
      this.m_scratchBuffer[3] = 271733878U;
    }

    private void MD5Update(byte[] chunk, int offset, uint inLen)
    {
      uint[] inb = new uint[16];
      int num1 = 0;
      int num2 = (int) (this.m_i[0] >> 3) & 63;
      if (this.m_i[0] + (inLen << 3) < this.m_i[0])
        ++this.m_i[1];
      this.m_i[0] += inLen << 3;
      this.m_i[1] += inLen >> 29;
      while (inLen-- > 0U)
      {
        this.m_inputBuffer[num2++] = chunk[offset + num1++];
        if (num2 == 64)
        {
          uint index1 = 0;
          uint index2 = 0;
          while (index1 < 16U)
          {
            inb[(int) index1] = (uint) ((int) this.m_inputBuffer[(int) index2 + 3] << 24 | (int) this.m_inputBuffer[(int) index2 + 2] << 16 | (int) this.m_inputBuffer[(int) index2 + 1] << 8) | (uint) this.m_inputBuffer[(int) index2];
            ++index1;
            index2 += 4U;
          }
          TeamFoundationServerMD5.TransformMD5(this.m_scratchBuffer, inb);
          num2 = 0;
        }
      }
    }

    private void MD5Final()
    {
      uint[] inb = new uint[16];
      inb[14] = this.m_i[0];
      inb[15] = this.m_i[1];
      int num = (int) (this.m_i[0] >> 3) & 63;
      uint inLen = num < 56 ? (uint) (56 - num) : (uint) (120 - num);
      this.MD5Update(TeamFoundationServerMD5.padding, 0, inLen);
      uint index1 = 0;
      uint index2 = 0;
      while (index1 < 14U)
      {
        inb[(int) index1] = (uint) ((int) this.m_inputBuffer[(int) index2 + 3] << 24 | (int) this.m_inputBuffer[(int) index2 + 2] << 16 | (int) this.m_inputBuffer[(int) index2 + 1] << 8) | (uint) this.m_inputBuffer[(int) index2];
        ++index1;
        index2 += 4U;
      }
      TeamFoundationServerMD5.TransformMD5(this.m_scratchBuffer, inb);
      uint index3 = 0;
      uint index4 = 0;
      while (index3 < 4U)
      {
        this.m_digest[(int) index4] = (byte) (this.m_scratchBuffer[(int) index3] & (uint) byte.MaxValue);
        this.m_digest[(int) index4 + 1] = (byte) (this.m_scratchBuffer[(int) index3] >> 8 & (uint) byte.MaxValue);
        this.m_digest[(int) index4 + 2] = (byte) (this.m_scratchBuffer[(int) index3] >> 16 & (uint) byte.MaxValue);
        this.m_digest[(int) index4 + 3] = (byte) (this.m_scratchBuffer[(int) index3] >> 24 & (uint) byte.MaxValue);
        ++index3;
        index4 += 4U;
      }
    }

    private static uint[] ReadUInt32ArrayFromBytes(byte[] array, int offset, int length)
    {
      uint[] numArray = new uint[length];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] += (uint) ((int) array[index * 4 + offset] + ((int) array[index * 4 + 1 + offset] << 8) + ((int) array[index * 4 + 2 + offset] << 16) + ((int) array[index * 4 + 3 + offset] << 24));
      return numArray;
    }

    private static int WriteUInt32Array(byte[] array, int offset, uint[] values)
    {
      for (int index = 0; index < values.Length; ++index)
      {
        array[offset++] = (byte) (values[index] & (uint) byte.MaxValue);
        array[offset++] = (byte) (values[index] >> 8 & (uint) byte.MaxValue);
        array[offset++] = (byte) (values[index] >> 16 & (uint) byte.MaxValue);
        array[offset++] = (byte) (values[index] >> 24 & (uint) byte.MaxValue);
      }
      return offset;
    }

    private static uint F(uint x, uint y, uint z) => (uint) ((int) x & (int) y | ~(int) x & (int) z);

    private static uint G(uint x, uint y, uint z) => (uint) ((int) x & (int) z | (int) y & ~(int) z);

    private static uint H(uint x, uint y, uint z) => x ^ y ^ z;

    private static uint I(uint x, uint y, uint z) => y ^ (x | ~z);

    private static uint RotateLeft(uint x, int n) => x << n | x >> 32 - n;

    private static uint FF(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
      a += TeamFoundationServerMD5.F(b, c, d) + x + ac;
      return TeamFoundationServerMD5.RotateLeft(a, s) + b;
    }

    private static uint GG(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
      a += TeamFoundationServerMD5.G(b, c, d) + x + ac;
      return TeamFoundationServerMD5.RotateLeft(a, s) + b;
    }

    private static uint HH(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
      a += TeamFoundationServerMD5.H(b, c, d) + x + ac;
      return TeamFoundationServerMD5.RotateLeft(a, s) + b;
    }

    private static uint II(uint a, uint b, uint c, uint d, uint x, int s, uint ac)
    {
      a += TeamFoundationServerMD5.I(b, c, d) + x + ac;
      return TeamFoundationServerMD5.RotateLeft(a, s) + b;
    }

    private static void TransformMD5(uint[] buf, uint[] inb)
    {
      uint a = buf[0];
      uint num1 = buf[1];
      uint num2 = buf[2];
      uint num3 = buf[3];
      uint num4 = TeamFoundationServerMD5.FF(a, num1, num2, num3, inb[0], 7, 3614090360U);
      uint num5 = TeamFoundationServerMD5.FF(num3, num4, num1, num2, inb[1], 12, 3905402710U);
      uint num6 = TeamFoundationServerMD5.FF(num2, num5, num4, num1, inb[2], 17, 606105819U);
      uint num7 = TeamFoundationServerMD5.FF(num1, num6, num5, num4, inb[3], 22, 3250441966U);
      uint num8 = TeamFoundationServerMD5.FF(num4, num7, num6, num5, inb[4], 7, 4118548399U);
      uint num9 = TeamFoundationServerMD5.FF(num5, num8, num7, num6, inb[5], 12, 1200080426U);
      uint num10 = TeamFoundationServerMD5.FF(num6, num9, num8, num7, inb[6], 17, 2821735955U);
      uint num11 = TeamFoundationServerMD5.FF(num7, num10, num9, num8, inb[7], 22, 4249261313U);
      uint num12 = TeamFoundationServerMD5.FF(num8, num11, num10, num9, inb[8], 7, 1770035416U);
      uint num13 = TeamFoundationServerMD5.FF(num9, num12, num11, num10, inb[9], 12, 2336552879U);
      uint num14 = TeamFoundationServerMD5.FF(num10, num13, num12, num11, inb[10], 17, 4294925233U);
      uint num15 = TeamFoundationServerMD5.FF(num11, num14, num13, num12, inb[11], 22, 2304563134U);
      uint num16 = TeamFoundationServerMD5.FF(num12, num15, num14, num13, inb[12], 7, 1804603682U);
      uint num17 = TeamFoundationServerMD5.FF(num13, num16, num15, num14, inb[13], 12, 4254626195U);
      uint num18 = TeamFoundationServerMD5.FF(num14, num17, num16, num15, inb[14], 17, 2792965006U);
      uint num19 = TeamFoundationServerMD5.FF(num15, num18, num17, num16, inb[15], 22, 1236535329U);
      uint num20 = TeamFoundationServerMD5.GG(num16, num19, num18, num17, inb[1], 5, 4129170786U);
      uint num21 = TeamFoundationServerMD5.GG(num17, num20, num19, num18, inb[6], 9, 3225465664U);
      uint num22 = TeamFoundationServerMD5.GG(num18, num21, num20, num19, inb[11], 14, 643717713U);
      uint num23 = TeamFoundationServerMD5.GG(num19, num22, num21, num20, inb[0], 20, 3921069994U);
      uint num24 = TeamFoundationServerMD5.GG(num20, num23, num22, num21, inb[5], 5, 3593408605U);
      uint num25 = TeamFoundationServerMD5.GG(num21, num24, num23, num22, inb[10], 9, 38016083U);
      uint num26 = TeamFoundationServerMD5.GG(num22, num25, num24, num23, inb[15], 14, 3634488961U);
      uint num27 = TeamFoundationServerMD5.GG(num23, num26, num25, num24, inb[4], 20, 3889429448U);
      uint num28 = TeamFoundationServerMD5.GG(num24, num27, num26, num25, inb[9], 5, 568446438U);
      uint num29 = TeamFoundationServerMD5.GG(num25, num28, num27, num26, inb[14], 9, 3275163606U);
      uint num30 = TeamFoundationServerMD5.GG(num26, num29, num28, num27, inb[3], 14, 4107603335U);
      uint num31 = TeamFoundationServerMD5.GG(num27, num30, num29, num28, inb[8], 20, 1163531501U);
      uint num32 = TeamFoundationServerMD5.GG(num28, num31, num30, num29, inb[13], 5, 2850285829U);
      uint num33 = TeamFoundationServerMD5.GG(num29, num32, num31, num30, inb[2], 9, 4243563512U);
      uint num34 = TeamFoundationServerMD5.GG(num30, num33, num32, num31, inb[7], 14, 1735328473U);
      uint num35 = TeamFoundationServerMD5.GG(num31, num34, num33, num32, inb[12], 20, 2368359562U);
      uint num36 = TeamFoundationServerMD5.HH(num32, num35, num34, num33, inb[5], 4, 4294588738U);
      uint num37 = TeamFoundationServerMD5.HH(num33, num36, num35, num34, inb[8], 11, 2272392833U);
      uint num38 = TeamFoundationServerMD5.HH(num34, num37, num36, num35, inb[11], 16, 1839030562U);
      uint num39 = TeamFoundationServerMD5.HH(num35, num38, num37, num36, inb[14], 23, 4259657740U);
      uint num40 = TeamFoundationServerMD5.HH(num36, num39, num38, num37, inb[1], 4, 2763975236U);
      uint num41 = TeamFoundationServerMD5.HH(num37, num40, num39, num38, inb[4], 11, 1272893353U);
      uint num42 = TeamFoundationServerMD5.HH(num38, num41, num40, num39, inb[7], 16, 4139469664U);
      uint num43 = TeamFoundationServerMD5.HH(num39, num42, num41, num40, inb[10], 23, 3200236656U);
      uint num44 = TeamFoundationServerMD5.HH(num40, num43, num42, num41, inb[13], 4, 681279174U);
      uint num45 = TeamFoundationServerMD5.HH(num41, num44, num43, num42, inb[0], 11, 3936430074U);
      uint num46 = TeamFoundationServerMD5.HH(num42, num45, num44, num43, inb[3], 16, 3572445317U);
      uint num47 = TeamFoundationServerMD5.HH(num43, num46, num45, num44, inb[6], 23, 76029189U);
      uint num48 = TeamFoundationServerMD5.HH(num44, num47, num46, num45, inb[9], 4, 3654602809U);
      uint num49 = TeamFoundationServerMD5.HH(num45, num48, num47, num46, inb[12], 11, 3873151461U);
      uint num50 = TeamFoundationServerMD5.HH(num46, num49, num48, num47, inb[15], 16, 530742520U);
      uint num51 = TeamFoundationServerMD5.HH(num47, num50, num49, num48, inb[2], 23, 3299628645U);
      uint num52 = TeamFoundationServerMD5.II(num48, num51, num50, num49, inb[0], 6, 4096336452U);
      uint num53 = TeamFoundationServerMD5.II(num49, num52, num51, num50, inb[7], 10, 1126891415U);
      uint num54 = TeamFoundationServerMD5.II(num50, num53, num52, num51, inb[14], 15, 2878612391U);
      uint num55 = TeamFoundationServerMD5.II(num51, num54, num53, num52, inb[5], 21, 4237533241U);
      uint num56 = TeamFoundationServerMD5.II(num52, num55, num54, num53, inb[12], 6, 1700485571U);
      uint num57 = TeamFoundationServerMD5.II(num53, num56, num55, num54, inb[3], 10, 2399980690U);
      uint num58 = TeamFoundationServerMD5.II(num54, num57, num56, num55, inb[10], 15, 4293915773U);
      uint num59 = TeamFoundationServerMD5.II(num55, num58, num57, num56, inb[1], 21, 2240044497U);
      uint num60 = TeamFoundationServerMD5.II(num56, num59, num58, num57, inb[8], 6, 1873313359U);
      uint num61 = TeamFoundationServerMD5.II(num57, num60, num59, num58, inb[15], 10, 4264355552U);
      uint num62 = TeamFoundationServerMD5.II(num58, num61, num60, num59, inb[6], 15, 2734768916U);
      uint num63 = TeamFoundationServerMD5.II(num59, num62, num61, num60, inb[13], 21, 1309151649U);
      uint num64 = TeamFoundationServerMD5.II(num60, num63, num62, num61, inb[4], 6, 4149444226U);
      uint num65 = TeamFoundationServerMD5.II(num61, num64, num63, num62, inb[11], 10, 3174756917U);
      uint b = TeamFoundationServerMD5.II(num62, num65, num64, num63, inb[2], 15, 718787259U);
      uint num66 = TeamFoundationServerMD5.II(num63, b, num65, num64, inb[9], 21, 3951481745U);
      buf[0] += num64;
      buf[1] += num66;
      buf[2] += b;
      buf[3] += num65;
    }

    private enum Transform
    {
      S31 = 4,
      S21 = 5,
      S41 = 6,
      S11 = 7,
      S22 = 9,
      S42 = 10, // 0x0000000A
      S32 = 11, // 0x0000000B
      S12 = 12, // 0x0000000C
      S23 = 14, // 0x0000000E
      S43 = 15, // 0x0000000F
      S33 = 16, // 0x00000010
      S13 = 17, // 0x00000011
      S24 = 20, // 0x00000014
      S44 = 21, // 0x00000015
      S14 = 22, // 0x00000016
      S34 = 23, // 0x00000017
    }
  }
}
