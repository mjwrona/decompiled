// Decompiled with JetBrains decompiler
// Type: Nest.NetworkDirectionProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class NetworkDirectionProcessor : ProcessorBase, INetworkDirectionProcessor, IProcessor
  {
    protected override string Name => "network_direction";

    public Field DestinationIp { get; set; }

    public bool? IgnoreMissing { get; set; }

    public IEnumerable<string> InternalNetworks { get; set; }

    public Field InternalNetworksField { get; set; }

    public Field SourceIp { get; set; }

    public Field TargetField { get; set; }
  }
}
