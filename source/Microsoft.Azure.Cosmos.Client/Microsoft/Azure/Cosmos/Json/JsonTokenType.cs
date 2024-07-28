// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonTokenType
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.Json
{
  internal enum JsonTokenType
  {
    NotStarted,
    BeginArray,
    EndArray,
    BeginObject,
    EndObject,
    String,
    Number,
    True,
    False,
    Null,
    FieldName,
    Int8,
    Int16,
    Int32,
    Int64,
    UInt32,
    Float32,
    Float64,
    Guid,
    Binary,
  }
}
