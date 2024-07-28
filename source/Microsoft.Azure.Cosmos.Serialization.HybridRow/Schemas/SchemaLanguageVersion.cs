// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaLanguageVersion
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [JsonConverter(typeof (StringEnumConverter), new object[] {true})]
  public enum SchemaLanguageVersion : byte
  {
    V1 = 0,
    Latest = 2,
    V2 = 2,
    Unspecified = 255, // 0xFF
  }
}
