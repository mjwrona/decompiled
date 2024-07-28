// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.LayoutCode
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public enum LayoutCode : byte
  {
    Invalid = 0,
    Null = 1,
    BooleanFalse = 2,
    Boolean = 3,
    Int8 = 5,
    Int16 = 6,
    Int32 = 7,
    Int64 = 8,
    UInt8 = 9,
    UInt16 = 10, // 0x0A
    UInt32 = 11, // 0x0B
    UInt64 = 12, // 0x0C
    VarInt = 13, // 0x0D
    VarUInt = 14, // 0x0E
    Float32 = 15, // 0x0F
    Float64 = 16, // 0x10
    Decimal = 17, // 0x11
    DateTime = 18, // 0x12
    Guid = 19, // 0x13
    Utf8 = 20, // 0x14
    Binary = 21, // 0x15
    Float128 = 22, // 0x16
    UnixDateTime = 23, // 0x17
    MongoDbObjectId = 24, // 0x18
    ObjectScope = 30, // 0x1E
    ImmutableObjectScope = 31, // 0x1F
    ArrayScope = 32, // 0x20
    ImmutableArrayScope = 33, // 0x21
    TypedArrayScope = 34, // 0x22
    ImmutableTypedArrayScope = 35, // 0x23
    TupleScope = 36, // 0x24
    ImmutableTupleScope = 37, // 0x25
    TypedTupleScope = 38, // 0x26
    ImmutableTypedTupleScope = 39, // 0x27
    MapScope = 40, // 0x28
    ImmutableMapScope = 41, // 0x29
    TypedMapScope = 42, // 0x2A
    ImmutableTypedMapScope = 43, // 0x2B
    SetScope = 44, // 0x2C
    ImmutableSetScope = 45, // 0x2D
    TypedSetScope = 46, // 0x2E
    ImmutableTypedSetScope = 47, // 0x2F
    NullableScope = 48, // 0x30
    ImmutableNullableScope = 49, // 0x31
    TaggedScope = 50, // 0x32
    ImmutableTaggedScope = 51, // 0x33
    Tagged2Scope = 52, // 0x34
    ImmutableTagged2Scope = 53, // 0x35
    Schema = 68, // 0x44
    ImmutableSchema = 69, // 0x45
    EndScope = 70, // 0x46
  }
}
