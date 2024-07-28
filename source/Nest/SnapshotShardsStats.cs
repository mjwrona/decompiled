// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotShardsStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class SnapshotShardsStats
  {
    [DataMember(Name = "done")]
    public long Done { get; internal set; }

    [DataMember(Name = "failed")]
    public long Failed { get; internal set; }

    [DataMember(Name = "finalizing")]
    public long Finalizing { get; internal set; }

    [DataMember(Name = "initializing")]
    public long Initializing { get; internal set; }

    [DataMember(Name = "started")]
    public long Started { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }
  }
}
