// Decompiled with JetBrains decompiler
// Type: Nest.FieldValueFactorModifier
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum FieldValueFactorModifier
  {
    [EnumMember(Value = "none")] None,
    [EnumMember(Value = "log")] Log,
    [EnumMember(Value = "log1p")] Log1P,
    [EnumMember(Value = "log2p")] Log2P,
    [EnumMember(Value = "ln")] Ln,
    [EnumMember(Value = "ln1p")] Ln1P,
    [EnumMember(Value = "ln2p")] Ln2P,
    [EnumMember(Value = "square")] Square,
    [EnumMember(Value = "sqrt")] SquareRoot,
    [EnumMember(Value = "reciprocal")] Reciprocal,
  }
}
