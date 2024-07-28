// Decompiled with JetBrains decompiler
// Type: Nest.IndexBlock
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public class IndexBlock : IUrlParameter
  {
    private IndexBlock(string value) => this.Value = value;

    public string Value { get; }

    public string GetString(IConnectionConfigurationValues settings) => this.Value;

    public static IndexBlock Metadata { get; } = new IndexBlock("metadata");

    public static IndexBlock Read { get; } = new IndexBlock("read");

    public static IndexBlock ReadOnly { get; } = new IndexBlock("read_only");

    public static IndexBlock Write { get; } = new IndexBlock("write");
  }
}
