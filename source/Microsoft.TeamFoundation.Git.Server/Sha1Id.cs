// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Sha1Id
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  [JsonConverter(typeof (Sha1IdJsonSerializer))]
  [DebuggerDisplay("{ToString()}")]
  [StructLayout(LayoutKind.Explicit, Size = 20, Pack = 1)]
  public readonly struct Sha1Id : IComparable<Sha1Id>, IEquatable<Sha1Id>, IHashCount
  {
    public const int Length = 20;
    public const int AbbreviatedByteCount = 3;
    public const string EmptyString = "0000000000000000000000000000000000000000";
    public static readonly Sha1Id Empty = new Sha1Id();
    public static readonly Sha1Id Maximum = new Sha1Id(new byte[20]
    {
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue
    });
    private static readonly char[] s_hexChars = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f'
    };
    [FieldOffset(0)]
    private readonly byte m_byte1;
    [FieldOffset(1)]
    private readonly byte m_byte2;
    [FieldOffset(2)]
    private readonly byte m_byte3;
    [FieldOffset(3)]
    private readonly byte m_byte4;
    [FieldOffset(4)]
    private readonly byte m_byte5;
    [FieldOffset(5)]
    private readonly byte m_byte6;
    [FieldOffset(6)]
    private readonly byte m_byte7;
    [FieldOffset(7)]
    private readonly byte m_byte8;
    [FieldOffset(8)]
    private readonly byte m_byte9;
    [FieldOffset(9)]
    private readonly byte m_byte10;
    [FieldOffset(10)]
    private readonly byte m_byte11;
    [FieldOffset(11)]
    private readonly byte m_byte12;
    [FieldOffset(12)]
    private readonly byte m_byte13;
    [FieldOffset(13)]
    private readonly byte m_byte14;
    [FieldOffset(14)]
    private readonly byte m_byte15;
    [FieldOffset(15)]
    private readonly byte m_byte16;
    [FieldOffset(16)]
    private readonly byte m_byte17;
    [FieldOffset(17)]
    private readonly byte m_byte18;
    [FieldOffset(18)]
    private readonly byte m_byte19;
    [FieldOffset(19)]
    private readonly byte m_byte20;
    [FieldOffset(0)]
    private readonly int m_bytes1Through4;
    [FieldOffset(4)]
    private readonly long m_bytes5Through12;
    [FieldOffset(12)]
    private readonly long m_bytes13Through20;
    private static readonly sbyte[] s_hexToDecTable = new sbyte[128]
    {
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 0,
      (sbyte) 1,
      (sbyte) 2,
      (sbyte) 3,
      (sbyte) 4,
      (sbyte) 5,
      (sbyte) 6,
      (sbyte) 7,
      (sbyte) 8,
      (sbyte) 9,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 10,
      (sbyte) 11,
      (sbyte) 12,
      (sbyte) 13,
      (sbyte) 14,
      (sbyte) 15,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) 10,
      (sbyte) 11,
      (sbyte) 12,
      (sbyte) 13,
      (sbyte) 14,
      (sbyte) 15,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1,
      (sbyte) -1
    };

    public unsafe Sha1Id(byte[] sha1IdBytes, int sourceIndex = 0)
      : this()
    {
      if (sha1IdBytes == null)
        throw new ArgumentNullException(nameof (sha1IdBytes));
      if (sha1IdBytes.Length < 20)
        throw new ArgumentException(string.Format("{0}.{1} < {2}", (object) nameof (sha1IdBytes), (object) nameof (Length), (object) 20), nameof (sha1IdBytes));
      if (sourceIndex < 0 || sourceIndex > sha1IdBytes.Length - 20)
        throw new ArgumentOutOfRangeException(nameof (sourceIndex));
      fixed (byte* numPtr = &sha1IdBytes[sourceIndex])
      {
        this.m_bytes1Through4 = *(int*) numPtr;
        this.m_bytes5Through12 = *(long*) (numPtr + 4);
        this.m_bytes13Through20 = *(long*) (numPtr + 12);
      }
    }

    public Sha1Id(string sha1IdString)
    {
      if (sha1IdString == null)
        throw new ArgumentNullException(nameof (sha1IdString));
      if (!Sha1Id.TryParse(sha1IdString, out this))
        throw new ArgumentException(Resources.Format("InvalidObjectId", (object) sha1IdString));
    }

    public static Sha1Id Parse(string sha1IdString)
    {
      Sha1Id id;
      if (Sha1Id.TryParse(sha1IdString, out id))
        return id;
      throw new ArgumentException(Resources.Format("InvalidObjectId", (object) sha1IdString));
    }

    public static unsafe bool TryParse(string sha1IdString, out Sha1Id id)
    {
      id = new Sha1Id();
      if (sha1IdString == null || sha1IdString.Length != 40)
        return false;
      fixed (Sha1Id* sha1IdPtr = &id)
      {
        int num1 = 0;
        byte* numPtr1 = (byte*) sha1IdPtr;
        string str1 = sha1IdString;
        int index1 = num1;
        int num2 = index1 + 1;
        byte digit1;
        if (!Sha1Id.TryParseHexDigit(str1[index1], out digit1))
          return false;
        string str2 = sha1IdString;
        int index2 = num2;
        int num3 = index2 + 1;
        byte digit2;
        if (!Sha1Id.TryParseHexDigit(str2[index2], out digit2))
          return false;
        byte* numPtr2 = numPtr1;
        byte* numPtr3 = numPtr2 + 1;
        int num4 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr2 = (byte) num4;
        string str3 = sha1IdString;
        int index3 = num3;
        int num5 = index3 + 1;
        if (!Sha1Id.TryParseHexDigit(str3[index3], out digit1))
          return false;
        string str4 = sha1IdString;
        int index4 = num5;
        int num6 = index4 + 1;
        if (!Sha1Id.TryParseHexDigit(str4[index4], out digit2))
          return false;
        byte* numPtr4 = numPtr3;
        byte* numPtr5 = numPtr4 + 1;
        int num7 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr4 = (byte) num7;
        string str5 = sha1IdString;
        int index5 = num6;
        int num8 = index5 + 1;
        if (!Sha1Id.TryParseHexDigit(str5[index5], out digit1))
          return false;
        string str6 = sha1IdString;
        int index6 = num8;
        int num9 = index6 + 1;
        if (!Sha1Id.TryParseHexDigit(str6[index6], out digit2))
          return false;
        byte* numPtr6 = numPtr5;
        byte* numPtr7 = numPtr6 + 1;
        int num10 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr6 = (byte) num10;
        string str7 = sha1IdString;
        int index7 = num9;
        int num11 = index7 + 1;
        if (!Sha1Id.TryParseHexDigit(str7[index7], out digit1))
          return false;
        string str8 = sha1IdString;
        int index8 = num11;
        int num12 = index8 + 1;
        if (!Sha1Id.TryParseHexDigit(str8[index8], out digit2))
          return false;
        byte* numPtr8 = numPtr7;
        byte* numPtr9 = numPtr8 + 1;
        int num13 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr8 = (byte) num13;
        string str9 = sha1IdString;
        int index9 = num12;
        int num14 = index9 + 1;
        if (!Sha1Id.TryParseHexDigit(str9[index9], out digit1))
          return false;
        string str10 = sha1IdString;
        int index10 = num14;
        int num15 = index10 + 1;
        if (!Sha1Id.TryParseHexDigit(str10[index10], out digit2))
          return false;
        byte* numPtr10 = numPtr9;
        byte* numPtr11 = numPtr10 + 1;
        int num16 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr10 = (byte) num16;
        string str11 = sha1IdString;
        int index11 = num15;
        int num17 = index11 + 1;
        if (!Sha1Id.TryParseHexDigit(str11[index11], out digit1))
          return false;
        string str12 = sha1IdString;
        int index12 = num17;
        int num18 = index12 + 1;
        if (!Sha1Id.TryParseHexDigit(str12[index12], out digit2))
          return false;
        byte* numPtr12 = numPtr11;
        byte* numPtr13 = numPtr12 + 1;
        int num19 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr12 = (byte) num19;
        string str13 = sha1IdString;
        int index13 = num18;
        int num20 = index13 + 1;
        if (!Sha1Id.TryParseHexDigit(str13[index13], out digit1))
          return false;
        string str14 = sha1IdString;
        int index14 = num20;
        int num21 = index14 + 1;
        if (!Sha1Id.TryParseHexDigit(str14[index14], out digit2))
          return false;
        byte* numPtr14 = numPtr13;
        byte* numPtr15 = numPtr14 + 1;
        int num22 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr14 = (byte) num22;
        string str15 = sha1IdString;
        int index15 = num21;
        int num23 = index15 + 1;
        if (!Sha1Id.TryParseHexDigit(str15[index15], out digit1))
          return false;
        string str16 = sha1IdString;
        int index16 = num23;
        int num24 = index16 + 1;
        if (!Sha1Id.TryParseHexDigit(str16[index16], out digit2))
          return false;
        byte* numPtr16 = numPtr15;
        byte* numPtr17 = numPtr16 + 1;
        int num25 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr16 = (byte) num25;
        string str17 = sha1IdString;
        int index17 = num24;
        int num26 = index17 + 1;
        if (!Sha1Id.TryParseHexDigit(str17[index17], out digit1))
          return false;
        string str18 = sha1IdString;
        int index18 = num26;
        int num27 = index18 + 1;
        if (!Sha1Id.TryParseHexDigit(str18[index18], out digit2))
          return false;
        byte* numPtr18 = numPtr17;
        byte* numPtr19 = numPtr18 + 1;
        int num28 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr18 = (byte) num28;
        string str19 = sha1IdString;
        int index19 = num27;
        int num29 = index19 + 1;
        if (!Sha1Id.TryParseHexDigit(str19[index19], out digit1))
          return false;
        string str20 = sha1IdString;
        int index20 = num29;
        int num30 = index20 + 1;
        if (!Sha1Id.TryParseHexDigit(str20[index20], out digit2))
          return false;
        byte* numPtr20 = numPtr19;
        byte* numPtr21 = numPtr20 + 1;
        int num31 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr20 = (byte) num31;
        string str21 = sha1IdString;
        int index21 = num30;
        int num32 = index21 + 1;
        if (!Sha1Id.TryParseHexDigit(str21[index21], out digit1))
          return false;
        string str22 = sha1IdString;
        int index22 = num32;
        int num33 = index22 + 1;
        if (!Sha1Id.TryParseHexDigit(str22[index22], out digit2))
          return false;
        byte* numPtr22 = numPtr21;
        byte* numPtr23 = numPtr22 + 1;
        int num34 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr22 = (byte) num34;
        string str23 = sha1IdString;
        int index23 = num33;
        int num35 = index23 + 1;
        if (!Sha1Id.TryParseHexDigit(str23[index23], out digit1))
          return false;
        string str24 = sha1IdString;
        int index24 = num35;
        int num36 = index24 + 1;
        if (!Sha1Id.TryParseHexDigit(str24[index24], out digit2))
          return false;
        byte* numPtr24 = numPtr23;
        byte* numPtr25 = numPtr24 + 1;
        int num37 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr24 = (byte) num37;
        string str25 = sha1IdString;
        int index25 = num36;
        int num38 = index25 + 1;
        if (!Sha1Id.TryParseHexDigit(str25[index25], out digit1))
          return false;
        string str26 = sha1IdString;
        int index26 = num38;
        int num39 = index26 + 1;
        if (!Sha1Id.TryParseHexDigit(str26[index26], out digit2))
          return false;
        byte* numPtr26 = numPtr25;
        byte* numPtr27 = numPtr26 + 1;
        int num40 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr26 = (byte) num40;
        string str27 = sha1IdString;
        int index27 = num39;
        int num41 = index27 + 1;
        if (!Sha1Id.TryParseHexDigit(str27[index27], out digit1))
          return false;
        string str28 = sha1IdString;
        int index28 = num41;
        int num42 = index28 + 1;
        if (!Sha1Id.TryParseHexDigit(str28[index28], out digit2))
          return false;
        byte* numPtr28 = numPtr27;
        byte* numPtr29 = numPtr28 + 1;
        int num43 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr28 = (byte) num43;
        string str29 = sha1IdString;
        int index29 = num42;
        int num44 = index29 + 1;
        if (!Sha1Id.TryParseHexDigit(str29[index29], out digit1))
          return false;
        string str30 = sha1IdString;
        int index30 = num44;
        int num45 = index30 + 1;
        if (!Sha1Id.TryParseHexDigit(str30[index30], out digit2))
          return false;
        byte* numPtr30 = numPtr29;
        byte* numPtr31 = numPtr30 + 1;
        int num46 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr30 = (byte) num46;
        string str31 = sha1IdString;
        int index31 = num45;
        int num47 = index31 + 1;
        if (!Sha1Id.TryParseHexDigit(str31[index31], out digit1))
          return false;
        string str32 = sha1IdString;
        int index32 = num47;
        int num48 = index32 + 1;
        if (!Sha1Id.TryParseHexDigit(str32[index32], out digit2))
          return false;
        byte* numPtr32 = numPtr31;
        byte* numPtr33 = numPtr32 + 1;
        int num49 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr32 = (byte) num49;
        string str33 = sha1IdString;
        int index33 = num48;
        int num50 = index33 + 1;
        if (!Sha1Id.TryParseHexDigit(str33[index33], out digit1))
          return false;
        string str34 = sha1IdString;
        int index34 = num50;
        int num51 = index34 + 1;
        if (!Sha1Id.TryParseHexDigit(str34[index34], out digit2))
          return false;
        byte* numPtr34 = numPtr33;
        byte* numPtr35 = numPtr34 + 1;
        int num52 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr34 = (byte) num52;
        string str35 = sha1IdString;
        int index35 = num51;
        int num53 = index35 + 1;
        if (!Sha1Id.TryParseHexDigit(str35[index35], out digit1))
          return false;
        string str36 = sha1IdString;
        int index36 = num53;
        int num54 = index36 + 1;
        if (!Sha1Id.TryParseHexDigit(str36[index36], out digit2))
          return false;
        byte* numPtr36 = numPtr35;
        byte* numPtr37 = numPtr36 + 1;
        int num55 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr36 = (byte) num55;
        string str37 = sha1IdString;
        int index37 = num54;
        int num56 = index37 + 1;
        if (!Sha1Id.TryParseHexDigit(str37[index37], out digit1))
          return false;
        string str38 = sha1IdString;
        int index38 = num56;
        int num57 = index38 + 1;
        if (!Sha1Id.TryParseHexDigit(str38[index38], out digit2))
          return false;
        byte* numPtr38 = numPtr37;
        byte* numPtr39 = numPtr38 + 1;
        int num58 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr38 = (byte) num58;
        string str39 = sha1IdString;
        int index39 = num57;
        int num59 = index39 + 1;
        if (!Sha1Id.TryParseHexDigit(str39[index39], out digit1))
          return false;
        string str40 = sha1IdString;
        int index40 = num59;
        int num60 = index40 + 1;
        if (!Sha1Id.TryParseHexDigit(str40[index40], out digit2))
          return false;
        byte* numPtr40 = numPtr39;
        byte* numPtr41 = numPtr40 + 1;
        int num61 = (int) (byte) ((uint) digit1 << 4 | (uint) digit2);
        *numPtr40 = (byte) num61;
        return true;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHexDigit(char c, out byte digit)
    {
      if (c > '\u007F')
      {
        digit = (byte) 0;
        return false;
      }
      digit = (byte) Sha1Id.s_hexToDecTable[(int) c];
      return digit != byte.MaxValue;
    }

    public static Sha1Id FromStream(Stream stream)
    {
      byte[] numArray = new byte[20];
      try
      {
        GitStreamUtil.ReadGreedy(stream, numArray, 0, 20);
      }
      catch (Exception ex)
      {
        throw new Sha1IdStreamReadException(ex);
      }
      return new Sha1Id(numArray);
    }

    public static bool IsNullOrEmpty(string shaString) => string.IsNullOrEmpty(shaString) || shaString.Equals("0000000000000000000000000000000000000000", StringComparison.Ordinal);

    public static string ValidateSha(string shaString, string defaultSha) => !Sha1Id.IsNullOrEmpty(shaString) ? shaString : defaultSha;

    public unsafe byte this[int index]
    {
      get
      {
        if (index < 0 || index > 19)
          throw new IndexOutOfRangeException();
        fixed (Sha1Id* sha1IdPtr = &this)
          return *(byte*) ((IntPtr) sha1IdPtr + index);
      }
    }

    public bool IsEmpty => this.m_bytes1Through4 == 0 && this.m_bytes5Through12 == 0L && this.m_bytes13Through20 == 0L;

    public int CompareTo(Sha1Id other)
    {
      int num1 = (int) this.m_byte1 - (int) other.m_byte1;
      if (num1 != 0)
        return num1;
      int num2 = (int) this.m_byte2 - (int) other.m_byte2;
      if (num2 != 0)
        return num2;
      int num3 = (int) this.m_byte3 - (int) other.m_byte3;
      if (num3 != 0)
        return num3;
      int num4 = (int) this.m_byte4 - (int) other.m_byte4;
      if (num4 != 0)
        return num4;
      int num5 = (int) this.m_byte5 - (int) other.m_byte5;
      if (num5 != 0)
        return num5;
      int num6 = (int) this.m_byte6 - (int) other.m_byte6;
      if (num6 != 0)
        return num6;
      int num7 = (int) this.m_byte7 - (int) other.m_byte7;
      if (num7 != 0)
        return num7;
      int num8 = (int) this.m_byte8 - (int) other.m_byte8;
      if (num8 != 0)
        return num8;
      int num9 = (int) this.m_byte9 - (int) other.m_byte9;
      if (num9 != 0)
        return num9;
      int num10 = (int) this.m_byte10 - (int) other.m_byte10;
      if (num10 != 0)
        return num10;
      int num11 = (int) this.m_byte11 - (int) other.m_byte11;
      if (num11 != 0)
        return num11;
      int num12 = (int) this.m_byte12 - (int) other.m_byte12;
      if (num12 != 0)
        return num12;
      int num13 = (int) this.m_byte13 - (int) other.m_byte13;
      if (num13 != 0)
        return num13;
      int num14 = (int) this.m_byte14 - (int) other.m_byte14;
      if (num14 != 0)
        return num14;
      int num15 = (int) this.m_byte15 - (int) other.m_byte15;
      if (num15 != 0)
        return num15;
      int num16 = (int) this.m_byte16 - (int) other.m_byte16;
      if (num16 != 0)
        return num16;
      int num17 = (int) this.m_byte17 - (int) other.m_byte17;
      if (num17 != 0)
        return num17;
      int num18 = (int) this.m_byte18 - (int) other.m_byte18;
      if (num18 != 0)
        return num18;
      int num19 = (int) this.m_byte19 - (int) other.m_byte19;
      if (num19 != 0)
        return num19;
      int num20 = (int) this.m_byte20 - (int) other.m_byte20;
      return num20 != 0 ? num20 : 0;
    }

    public int CompareTo(byte[] otherBytes)
    {
      if (otherBytes == null || otherBytes.Length != 20)
        throw new InvalidOperationException();
      int num1 = (int) this.m_byte1 - (int) otherBytes[0];
      if (num1 != 0)
        return num1;
      int num2 = (int) this.m_byte2 - (int) otherBytes[1];
      if (num2 != 0)
        return num2;
      int num3 = (int) this.m_byte3 - (int) otherBytes[2];
      if (num3 != 0)
        return num3;
      int num4 = (int) this.m_byte4 - (int) otherBytes[3];
      if (num4 != 0)
        return num4;
      int num5 = (int) this.m_byte5 - (int) otherBytes[4];
      if (num5 != 0)
        return num5;
      int num6 = (int) this.m_byte6 - (int) otherBytes[5];
      if (num6 != 0)
        return num6;
      int num7 = (int) this.m_byte7 - (int) otherBytes[6];
      if (num7 != 0)
        return num7;
      int num8 = (int) this.m_byte8 - (int) otherBytes[7];
      if (num8 != 0)
        return num8;
      int num9 = (int) this.m_byte9 - (int) otherBytes[8];
      if (num9 != 0)
        return num9;
      int num10 = (int) this.m_byte10 - (int) otherBytes[9];
      if (num10 != 0)
        return num10;
      int num11 = (int) this.m_byte11 - (int) otherBytes[10];
      if (num11 != 0)
        return num11;
      int num12 = (int) this.m_byte12 - (int) otherBytes[11];
      if (num12 != 0)
        return num12;
      int num13 = (int) this.m_byte13 - (int) otherBytes[12];
      if (num13 != 0)
        return num13;
      int num14 = (int) this.m_byte14 - (int) otherBytes[13];
      if (num14 != 0)
        return num14;
      int num15 = (int) this.m_byte15 - (int) otherBytes[14];
      if (num15 != 0)
        return num15;
      int num16 = (int) this.m_byte16 - (int) otherBytes[15];
      if (num16 != 0)
        return num16;
      int num17 = (int) this.m_byte17 - (int) otherBytes[16];
      if (num17 != 0)
        return num17;
      int num18 = (int) this.m_byte18 - (int) otherBytes[17];
      if (num18 != 0)
        return num18;
      int num19 = (int) this.m_byte19 - (int) otherBytes[18];
      if (num19 != 0)
        return num19;
      int num20 = (int) this.m_byte20 - (int) otherBytes[19];
      return num20 != 0 ? num20 : 0;
    }

    public override bool Equals(object value) => value != null && value is Sha1Id sha1Id && this == sha1Id;

    public bool Equals(Sha1Id other) => this == other;

    public static bool operator ==(Sha1Id sha1Id1, Sha1Id sha1Id2) => sha1Id1.m_bytes1Through4 == sha1Id2.m_bytes1Through4 && sha1Id1.m_bytes5Through12 == sha1Id2.m_bytes5Through12 && sha1Id1.m_bytes13Through20 == sha1Id2.m_bytes13Through20;

    public static bool operator !=(Sha1Id sha1Id1, Sha1Id sha1Id2) => !(sha1Id1 == sha1Id2);

    public override int GetHashCode() => this.m_bytes1Through4;

    public byte[] ToByteArray() => new byte[20]
    {
      this.m_byte1,
      this.m_byte2,
      this.m_byte3,
      this.m_byte4,
      this.m_byte5,
      this.m_byte6,
      this.m_byte7,
      this.m_byte8,
      this.m_byte9,
      this.m_byte10,
      this.m_byte11,
      this.m_byte12,
      this.m_byte13,
      this.m_byte14,
      this.m_byte15,
      this.m_byte16,
      this.m_byte17,
      this.m_byte18,
      this.m_byte19,
      this.m_byte20
    };

    public void GetBytes(byte[] buf, int offset)
    {
      buf[offset] = this.m_byte1;
      buf[offset + 1] = this.m_byte2;
      buf[offset + 2] = this.m_byte3;
      buf[offset + 3] = this.m_byte4;
      buf[offset + 4] = this.m_byte5;
      buf[offset + 5] = this.m_byte6;
      buf[offset + 6] = this.m_byte7;
      buf[offset + 7] = this.m_byte8;
      buf[offset + 8] = this.m_byte9;
      buf[offset + 9] = this.m_byte10;
      buf[offset + 10] = this.m_byte11;
      buf[offset + 11] = this.m_byte12;
      buf[offset + 12] = this.m_byte13;
      buf[offset + 13] = this.m_byte14;
      buf[offset + 14] = this.m_byte15;
      buf[offset + 15] = this.m_byte16;
      buf[offset + 16] = this.m_byte17;
      buf[offset + 17] = this.m_byte18;
      buf[offset + 18] = this.m_byte19;
      buf[offset + 19] = this.m_byte20;
    }

    public override unsafe string ToString()
    {
      char* chPtr1 = stackalloc char[40];
      fixed (Sha1Id* sha1IdPtr = &this)
      {
        char* chPtr2 = chPtr1;
        char* chPtr3 = (char*) ((IntPtr) chPtr2 + 2);
        int hexChar1 = (int) Sha1Id.s_hexChars[(int) *(byte*) sha1IdPtr >> 4];
        *chPtr2 = (char) hexChar1;
        char* chPtr4 = chPtr3;
        char* chPtr5 = (char*) ((IntPtr) chPtr4 + 2);
        int hexChar2 = (int) Sha1Id.s_hexChars[(int) *(byte*) sha1IdPtr & 15];
        *chPtr4 = (char) hexChar2;
        byte* numPtr1 = (byte*) sha1IdPtr + 1;
        char* chPtr6 = chPtr5;
        char* chPtr7 = (char*) ((IntPtr) chPtr6 + 2);
        int hexChar3 = (int) Sha1Id.s_hexChars[(int) *numPtr1 >> 4];
        *chPtr6 = (char) hexChar3;
        char* chPtr8 = chPtr7;
        char* chPtr9 = (char*) ((IntPtr) chPtr8 + 2);
        int hexChar4 = (int) Sha1Id.s_hexChars[(int) *numPtr1 & 15];
        *chPtr8 = (char) hexChar4;
        byte* numPtr2 = numPtr1 + 1;
        char* chPtr10 = chPtr9;
        char* chPtr11 = (char*) ((IntPtr) chPtr10 + 2);
        int hexChar5 = (int) Sha1Id.s_hexChars[(int) *numPtr2 >> 4];
        *chPtr10 = (char) hexChar5;
        char* chPtr12 = chPtr11;
        char* chPtr13 = (char*) ((IntPtr) chPtr12 + 2);
        int hexChar6 = (int) Sha1Id.s_hexChars[(int) *numPtr2 & 15];
        *chPtr12 = (char) hexChar6;
        byte* numPtr3 = numPtr2 + 1;
        char* chPtr14 = chPtr13;
        char* chPtr15 = (char*) ((IntPtr) chPtr14 + 2);
        int hexChar7 = (int) Sha1Id.s_hexChars[(int) *numPtr3 >> 4];
        *chPtr14 = (char) hexChar7;
        char* chPtr16 = chPtr15;
        char* chPtr17 = (char*) ((IntPtr) chPtr16 + 2);
        int hexChar8 = (int) Sha1Id.s_hexChars[(int) *numPtr3 & 15];
        *chPtr16 = (char) hexChar8;
        byte* numPtr4 = numPtr3 + 1;
        char* chPtr18 = chPtr17;
        char* chPtr19 = (char*) ((IntPtr) chPtr18 + 2);
        int hexChar9 = (int) Sha1Id.s_hexChars[(int) *numPtr4 >> 4];
        *chPtr18 = (char) hexChar9;
        char* chPtr20 = chPtr19;
        char* chPtr21 = (char*) ((IntPtr) chPtr20 + 2);
        int hexChar10 = (int) Sha1Id.s_hexChars[(int) *numPtr4 & 15];
        *chPtr20 = (char) hexChar10;
        byte* numPtr5 = numPtr4 + 1;
        char* chPtr22 = chPtr21;
        char* chPtr23 = (char*) ((IntPtr) chPtr22 + 2);
        int hexChar11 = (int) Sha1Id.s_hexChars[(int) *numPtr5 >> 4];
        *chPtr22 = (char) hexChar11;
        char* chPtr24 = chPtr23;
        char* chPtr25 = (char*) ((IntPtr) chPtr24 + 2);
        int hexChar12 = (int) Sha1Id.s_hexChars[(int) *numPtr5 & 15];
        *chPtr24 = (char) hexChar12;
        byte* numPtr6 = numPtr5 + 1;
        char* chPtr26 = chPtr25;
        char* chPtr27 = (char*) ((IntPtr) chPtr26 + 2);
        int hexChar13 = (int) Sha1Id.s_hexChars[(int) *numPtr6 >> 4];
        *chPtr26 = (char) hexChar13;
        char* chPtr28 = chPtr27;
        char* chPtr29 = (char*) ((IntPtr) chPtr28 + 2);
        int hexChar14 = (int) Sha1Id.s_hexChars[(int) *numPtr6 & 15];
        *chPtr28 = (char) hexChar14;
        byte* numPtr7 = numPtr6 + 1;
        char* chPtr30 = chPtr29;
        char* chPtr31 = (char*) ((IntPtr) chPtr30 + 2);
        int hexChar15 = (int) Sha1Id.s_hexChars[(int) *numPtr7 >> 4];
        *chPtr30 = (char) hexChar15;
        char* chPtr32 = chPtr31;
        char* chPtr33 = (char*) ((IntPtr) chPtr32 + 2);
        int hexChar16 = (int) Sha1Id.s_hexChars[(int) *numPtr7 & 15];
        *chPtr32 = (char) hexChar16;
        byte* numPtr8 = numPtr7 + 1;
        char* chPtr34 = chPtr33;
        char* chPtr35 = (char*) ((IntPtr) chPtr34 + 2);
        int hexChar17 = (int) Sha1Id.s_hexChars[(int) *numPtr8 >> 4];
        *chPtr34 = (char) hexChar17;
        char* chPtr36 = chPtr35;
        char* chPtr37 = (char*) ((IntPtr) chPtr36 + 2);
        int hexChar18 = (int) Sha1Id.s_hexChars[(int) *numPtr8 & 15];
        *chPtr36 = (char) hexChar18;
        byte* numPtr9 = numPtr8 + 1;
        char* chPtr38 = chPtr37;
        char* chPtr39 = (char*) ((IntPtr) chPtr38 + 2);
        int hexChar19 = (int) Sha1Id.s_hexChars[(int) *numPtr9 >> 4];
        *chPtr38 = (char) hexChar19;
        char* chPtr40 = chPtr39;
        char* chPtr41 = (char*) ((IntPtr) chPtr40 + 2);
        int hexChar20 = (int) Sha1Id.s_hexChars[(int) *numPtr9 & 15];
        *chPtr40 = (char) hexChar20;
        byte* numPtr10 = numPtr9 + 1;
        char* chPtr42 = chPtr41;
        char* chPtr43 = (char*) ((IntPtr) chPtr42 + 2);
        int hexChar21 = (int) Sha1Id.s_hexChars[(int) *numPtr10 >> 4];
        *chPtr42 = (char) hexChar21;
        char* chPtr44 = chPtr43;
        char* chPtr45 = (char*) ((IntPtr) chPtr44 + 2);
        int hexChar22 = (int) Sha1Id.s_hexChars[(int) *numPtr10 & 15];
        *chPtr44 = (char) hexChar22;
        byte* numPtr11 = numPtr10 + 1;
        char* chPtr46 = chPtr45;
        char* chPtr47 = (char*) ((IntPtr) chPtr46 + 2);
        int hexChar23 = (int) Sha1Id.s_hexChars[(int) *numPtr11 >> 4];
        *chPtr46 = (char) hexChar23;
        char* chPtr48 = chPtr47;
        char* chPtr49 = (char*) ((IntPtr) chPtr48 + 2);
        int hexChar24 = (int) Sha1Id.s_hexChars[(int) *numPtr11 & 15];
        *chPtr48 = (char) hexChar24;
        byte* numPtr12 = numPtr11 + 1;
        char* chPtr50 = chPtr49;
        char* chPtr51 = (char*) ((IntPtr) chPtr50 + 2);
        int hexChar25 = (int) Sha1Id.s_hexChars[(int) *numPtr12 >> 4];
        *chPtr50 = (char) hexChar25;
        char* chPtr52 = chPtr51;
        char* chPtr53 = (char*) ((IntPtr) chPtr52 + 2);
        int hexChar26 = (int) Sha1Id.s_hexChars[(int) *numPtr12 & 15];
        *chPtr52 = (char) hexChar26;
        byte* numPtr13 = numPtr12 + 1;
        char* chPtr54 = chPtr53;
        char* chPtr55 = (char*) ((IntPtr) chPtr54 + 2);
        int hexChar27 = (int) Sha1Id.s_hexChars[(int) *numPtr13 >> 4];
        *chPtr54 = (char) hexChar27;
        char* chPtr56 = chPtr55;
        char* chPtr57 = (char*) ((IntPtr) chPtr56 + 2);
        int hexChar28 = (int) Sha1Id.s_hexChars[(int) *numPtr13 & 15];
        *chPtr56 = (char) hexChar28;
        byte* numPtr14 = numPtr13 + 1;
        char* chPtr58 = chPtr57;
        char* chPtr59 = (char*) ((IntPtr) chPtr58 + 2);
        int hexChar29 = (int) Sha1Id.s_hexChars[(int) *numPtr14 >> 4];
        *chPtr58 = (char) hexChar29;
        char* chPtr60 = chPtr59;
        char* chPtr61 = (char*) ((IntPtr) chPtr60 + 2);
        int hexChar30 = (int) Sha1Id.s_hexChars[(int) *numPtr14 & 15];
        *chPtr60 = (char) hexChar30;
        byte* numPtr15 = numPtr14 + 1;
        char* chPtr62 = chPtr61;
        char* chPtr63 = (char*) ((IntPtr) chPtr62 + 2);
        int hexChar31 = (int) Sha1Id.s_hexChars[(int) *numPtr15 >> 4];
        *chPtr62 = (char) hexChar31;
        char* chPtr64 = chPtr63;
        char* chPtr65 = (char*) ((IntPtr) chPtr64 + 2);
        int hexChar32 = (int) Sha1Id.s_hexChars[(int) *numPtr15 & 15];
        *chPtr64 = (char) hexChar32;
        byte* numPtr16 = numPtr15 + 1;
        char* chPtr66 = chPtr65;
        char* chPtr67 = (char*) ((IntPtr) chPtr66 + 2);
        int hexChar33 = (int) Sha1Id.s_hexChars[(int) *numPtr16 >> 4];
        *chPtr66 = (char) hexChar33;
        char* chPtr68 = chPtr67;
        char* chPtr69 = (char*) ((IntPtr) chPtr68 + 2);
        int hexChar34 = (int) Sha1Id.s_hexChars[(int) *numPtr16 & 15];
        *chPtr68 = (char) hexChar34;
        byte* numPtr17 = numPtr16 + 1;
        char* chPtr70 = chPtr69;
        char* chPtr71 = (char*) ((IntPtr) chPtr70 + 2);
        int hexChar35 = (int) Sha1Id.s_hexChars[(int) *numPtr17 >> 4];
        *chPtr70 = (char) hexChar35;
        char* chPtr72 = chPtr71;
        char* chPtr73 = (char*) ((IntPtr) chPtr72 + 2);
        int hexChar36 = (int) Sha1Id.s_hexChars[(int) *numPtr17 & 15];
        *chPtr72 = (char) hexChar36;
        byte* numPtr18 = numPtr17 + 1;
        char* chPtr74 = chPtr73;
        char* chPtr75 = (char*) ((IntPtr) chPtr74 + 2);
        int hexChar37 = (int) Sha1Id.s_hexChars[(int) *numPtr18 >> 4];
        *chPtr74 = (char) hexChar37;
        char* chPtr76 = chPtr75;
        char* chPtr77 = (char*) ((IntPtr) chPtr76 + 2);
        int hexChar38 = (int) Sha1Id.s_hexChars[(int) *numPtr18 & 15];
        *chPtr76 = (char) hexChar38;
        byte* numPtr19 = numPtr18 + 1;
        char* chPtr78 = chPtr77;
        char* chPtr79 = (char*) ((IntPtr) chPtr78 + 2);
        int hexChar39 = (int) Sha1Id.s_hexChars[(int) *numPtr19 >> 4];
        *chPtr78 = (char) hexChar39;
        *chPtr79 = Sha1Id.s_hexChars[(int) *numPtr19 & 15];
      }
      return new string(chPtr1, 0, 40);
    }

    public unsafe string ToAbbreviatedString()
    {
      char* chPtr1 = stackalloc char[6];
      fixed (Sha1Id* sha1IdPtr = &this)
      {
        char* chPtr2 = chPtr1;
        char* chPtr3 = (char*) ((IntPtr) chPtr2 + 2);
        int hexChar1 = (int) Sha1Id.s_hexChars[(int) *(byte*) sha1IdPtr >> 4];
        *chPtr2 = (char) hexChar1;
        char* chPtr4 = chPtr3;
        char* chPtr5 = (char*) ((IntPtr) chPtr4 + 2);
        int hexChar2 = (int) Sha1Id.s_hexChars[(int) *(byte*) sha1IdPtr & 15];
        *chPtr4 = (char) hexChar2;
        byte* numPtr1 = (byte*) sha1IdPtr + 1;
        char* chPtr6 = chPtr5;
        char* chPtr7 = (char*) ((IntPtr) chPtr6 + 2);
        int hexChar3 = (int) Sha1Id.s_hexChars[(int) *numPtr1 >> 4];
        *chPtr6 = (char) hexChar3;
        char* chPtr8 = chPtr7;
        char* chPtr9 = (char*) ((IntPtr) chPtr8 + 2);
        int hexChar4 = (int) Sha1Id.s_hexChars[(int) *numPtr1 & 15];
        *chPtr8 = (char) hexChar4;
        byte* numPtr2 = numPtr1 + 1;
        char* chPtr10 = chPtr9;
        char* chPtr11 = (char*) ((IntPtr) chPtr10 + 2);
        int hexChar5 = (int) Sha1Id.s_hexChars[(int) *numPtr2 >> 4];
        *chPtr10 = (char) hexChar5;
        *chPtr11 = Sha1Id.s_hexChars[(int) *numPtr2 & 15];
      }
      return new string(chPtr1, 0, 6);
    }

    public int GetByteCount() => 20;
  }
}
