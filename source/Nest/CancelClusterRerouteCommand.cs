// Decompiled with JetBrains decompiler
// Type: Nest.CancelClusterRerouteCommand
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class CancelClusterRerouteCommand : ICancelClusterRerouteCommand, IClusterRerouteCommand
  {
    public bool? AllowPrimary { get; set; }

    public IndexName Index { get; set; }

    public string Name => "cancel";

    public string Node { get; set; }

    public int? Shard { get; set; }
  }
}
