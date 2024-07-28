// Decompiled with JetBrains decompiler
// Type: Nest.FixedIndexSettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public static class FixedIndexSettings
  {
    public const string NumberOfRoutingShards = "index.number_of_routing_shards";
    public const string NumberOfShards = "index.number_of_shards";
    public const string RoutingPartitionSize = "index.routing_partition_size";
    public const string Hidden = "index.hidden";
    public const string PercolatorMapUnmappedFieldsAsText = "index.percolator.map_unmapped_fields_as_text";
  }
}
