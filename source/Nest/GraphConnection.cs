// Decompiled with JetBrains decompiler
// Type: Nest.GraphConnection
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class GraphConnection
  {
    [DataMember(Name = "doc_count")]
    public long DocumentCount { get; internal set; }

    [DataMember(Name = "source")]
    public long Source { get; internal set; }

    [DataMember(Name = "target")]
    public long Target { get; internal set; }

    [DataMember(Name = "weight")]
    public double Weight { get; internal set; }
  }
}
