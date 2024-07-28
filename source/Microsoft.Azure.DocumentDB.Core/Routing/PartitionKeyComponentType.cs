// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyComponentType
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
