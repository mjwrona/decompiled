// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyComponentType
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents.Routing
{
  internal enum PartitionKeyComponentType
  {
    Undefined = 0,
    Null = 1,
    False = 2,
    True = 3,
    MinNumber = 4,
    Number = 5,
    MaxNumber = 6,
    MinString = 7,
    String = 8,
    MaxString = 9,
    Int64 = 10, // 0x0000000A
    Int32 = 11, // 0x0000000B
    Int16 = 12, // 0x0000000C
    Int8 = 13, // 0x0000000D
    Uint64 = 14, // 0x0000000E
    Uint32 = 15, // 0x0000000F
    Uint16 = 16, // 0x00000010
    Uint8 = 17, // 0x00000011
    Binary = 18, // 0x00000012
    Guid = 19, // 0x00000013
    Float = 20, // 0x00000014
    Infinity = 255, // 0x000000FF
  }
}
