// Decompiled with JetBrains decompiler
// Type: Nest.EmailPriority
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum EmailPriority
  {
    [EnumMember(Value = "lowest")] Lowest,
    [EnumMember(Value = "low")] Low,
    [EnumMember(Value = "normal")] Normal,
    [EnumMember(Value = "high")] High,
    [EnumMember(Value = "highest")] Highest,
  }
}
