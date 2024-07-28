// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.FormatCode
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal struct FormatCode
  {
    public const byte Described = 0;
    public const byte Null = 64;
    public const byte Boolean = 86;
    public const byte BooleanTrue = 65;
    public const byte BooleanFalse = 66;
    public const byte UInt0 = 67;
    public const byte ULong0 = 68;
    public const byte UByte = 80;
    public const byte UShort = 96;
    public const byte UInt = 112;
    public const byte ULong = 128;
    public const byte Byte = 81;
    public const byte Short = 97;
    public const byte Int = 113;
    public const byte Long = 129;
    public const byte SmallUInt = 82;
    public const byte SmallULong = 83;
    public const byte SmallInt = 84;
    public const byte SmallLong = 85;
    public const byte Float = 114;
    public const byte Double = 130;
    public const byte Decimal32 = 116;
    public const byte Decimal64 = 132;
    public const byte Decimal128 = 148;
    public const byte Char = 115;
    public const byte TimeStamp = 131;
    public const byte Uuid = 152;
    public const byte Binary8 = 160;
    public const byte Binary32 = 176;
    public const byte String8Utf8 = 161;
    public const byte String32Utf8 = 177;
    public const byte Symbol8 = 163;
    public const byte Symbol32 = 179;
    public const byte List0 = 69;
    public const byte List8 = 192;
    public const byte List32 = 208;
    public const byte Map8 = 193;
    public const byte Map32 = 209;
    public const byte Array8 = 224;
    public const byte Array32 = 240;
    private byte type;
    private byte extType;

    public FormatCode(byte type)
      : this(type, (byte) 0)
    {
    }

    public FormatCode(byte type, byte extType)
    {
      this.type = type;
      this.extType = extType;
    }

    public byte Type => this.type;

    public byte SubType => (byte) ((uint) this.type & 15U);

    public byte SubCategory => (byte) (((int) this.type & 240) >> 4);

    public byte ExtType => this.extType;

    public static bool HasExtType(byte type) => ((int) type & 15) == 15;

    public static implicit operator FormatCode(byte value) => new FormatCode(value);

    public static implicit operator byte(FormatCode value) => value.Type;

    public static bool operator ==(FormatCode fc1, FormatCode fc2) => (int) fc1.Type == (int) fc2.Type;

    public static bool operator !=(FormatCode fc1, FormatCode fc2) => (int) fc1.Type != (int) fc2.Type;

    public bool HasExtType() => ((int) this.type & 15) == 15;

    public override bool Equals(object obj) => obj is FormatCode formatCode && this == formatCode;

    public override int GetHashCode() => this.type.GetHashCode();

    public override string ToString() => this.HasExtType() ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X2}.{1:X2}", new object[2]
    {
      (object) this.Type,
      (object) this.ExtType
    }) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X2}", new object[1]
    {
      (object) this.Type
    });
  }
}
