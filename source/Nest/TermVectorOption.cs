// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorOption
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum TermVectorOption
  {
    [EnumMember(Value = "no")] No,
    [EnumMember(Value = "yes")] Yes,
    [EnumMember(Value = "with_offsets")] WithOffsets,
    [EnumMember(Value = "with_positions")] WithPositions,
    [EnumMember(Value = "with_positions_offsets")] WithPositionsOffsets,
    [EnumMember(Value = "with_positions_payloads")] WithPositionsPayloads,
    [EnumMember(Value = "with_positions_offsets_payloads")] WithPositionsOffsetsPayloads,
  }
}
