// Decompiled with JetBrains decompiler
// Type: Nest.MoveClusterRerouteCommand
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MoveClusterRerouteCommand : IMoveClusterRerouteCommand, IClusterRerouteCommand
  {
    public string FromNode { get; set; }

    public IndexName Index { get; set; }

    public string Name => "move";

    public int? Shard { get; set; }

    public string ToNode { get; set; }
  }
}
