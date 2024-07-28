// Decompiled with JetBrains decompiler
// Type: Nest.ConvertProcessorType
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum ConvertProcessorType
  {
    [EnumMember(Value = "integer")] Integer,
    [EnumMember(Value = "long")] Long,
    [EnumMember(Value = "float")] Float,
    [EnumMember(Value = "double")] Double,
    [EnumMember(Value = "string")] String,
    [EnumMember(Value = "boolean")] Boolean,
    [EnumMember(Value = "ip")] Ip,
    [EnumMember(Value = "auto")] Auto,
  }
}
