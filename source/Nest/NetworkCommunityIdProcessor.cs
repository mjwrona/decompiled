// Decompiled with JetBrains decompiler
// Type: Nest.NetworkCommunityIdProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class NetworkCommunityIdProcessor : ProcessorBase, INetworkCommunityIdProcessor, IProcessor
  {
    protected override string Name => "community_id";

    public Field DestinationIp { get; set; }

    public Field DestinationPort { get; set; }

    public Field IanaNumber { get; set; }

    public Field IcmpType { get; set; }

    public Field IcmpCode { get; set; }

    public bool? IgnoreMissing { get; set; }

    public int? Seed { get; set; }

    public Field SourceIp { get; set; }

    public Field SourcePort { get; set; }

    public Field TargetField { get; set; }

    public Field Transport { get; set; }
  }
}
