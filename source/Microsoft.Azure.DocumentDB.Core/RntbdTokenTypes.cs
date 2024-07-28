// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenTypes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents
{
  internal enum RntbdTokenTypes : byte
  {
    Byte = 0,
    UShort = 1,
    ULong = 2,
    Long = 3,
    ULongLong = 4,
    LongLong = 5,
    Guid = 6,
    SmallString = 7,
    String = 8,
    ULongString = 9,
    SmallBytes = 10, // 0x0A
    Bytes = 11, // 0x0B
    ULongBytes = 12, // 0x0C
    Float = 13, // 0x0D
    Double = 14, // 0x0E
    Invalid = 255, // 0xFF
  }
}
