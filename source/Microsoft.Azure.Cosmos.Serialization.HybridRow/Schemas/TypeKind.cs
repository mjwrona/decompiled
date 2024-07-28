// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.TypeKind
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [JsonConverter(typeof (StringEnumConverter), new object[] {true})]
  public enum TypeKind : byte
  {
    Invalid,
    Null,
    [EnumMember(Value = "bool")] Boolean,
    Int8,
    Int16,
    Int32,
    Int64,
    [EnumMember(Value = "uint8")] UInt8,
    [EnumMember(Value = "uint16")] UInt16,
    [EnumMember(Value = "uint32")] UInt32,
    [EnumMember(Value = "uint64")] UInt64,
    [EnumMember(Value = "varint")] VarInt,
    [EnumMember(Value = "varuint")] VarUInt,
    Float32,
    Float64,
    Float128,
    Decimal,
    [EnumMember(Value = "datetime")] DateTime,
    [EnumMember(Value = "unixdatetime")] UnixDateTime,
    Guid,
    [EnumMember(Value = "mongodbobjectid")] MongoDbObjectId,
    Utf8,
    Binary,
    Object,
    Array,
    Set,
    Map,
    Tuple,
    Tagged,
    Schema,
    Any,
    Enum,
  }
}
