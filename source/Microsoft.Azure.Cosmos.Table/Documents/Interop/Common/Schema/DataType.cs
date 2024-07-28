// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Interop.Common.Schema.DataType
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Documents.Interop.Common.Schema
{
  internal enum DataType
  {
    Guid = 0,
    Double = 1,
    String = 2,
    Document = 3,
    Array = 4,
    Binary = 5,
    Undefined = 6,
    ObjectId = 7,
    Boolean = 8,
    DateTime = 9,
    Null = 10, // 0x0000000A
    RegularExpression = 11, // 0x0000000B
    JavaScript = 13, // 0x0000000D
    Symbol = 14, // 0x0000000E
    JavaScriptWithScope = 15, // 0x0000000F
    Int32 = 16, // 0x00000010
    Timestamp = 17, // 0x00000011
    Int64 = 18, // 0x00000012
    MaxKey = 127, // 0x0000007F
    MinKey = 255, // 0x000000FF
  }
}
