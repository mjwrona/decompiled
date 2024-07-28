// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenTypes
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
