// Decompiled with JetBrains decompiler
// Type: Nest.ClusterOperatingSystemStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterOperatingSystemStats
  {
    [DataMember(Name = "allocated_processors")]
    public int AllocatedProcessors { get; internal set; }

    [DataMember(Name = "available_processors")]
    public int AvailableProcessors { get; internal set; }

    [DataMember(Name = "mem")]
    public OperatingSystemMemoryInfo Memory { get; internal set; }

    [DataMember(Name = "names")]
    public IReadOnlyCollection<ClusterOperatingSystemName> Names { get; internal set; }

    [DataMember(Name = "pretty_names")]
    public IReadOnlyCollection<ClusterOperatingSystemPrettyNane> PrettyNames { get; internal set; }

    [DataMember(Name = "architectures")]
    public IReadOnlyCollection<ArchitectureStats> Architectures { get; internal set; }
  }
}
