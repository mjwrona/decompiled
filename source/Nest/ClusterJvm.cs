// Decompiled with JetBrains decompiler
// Type: Nest.ClusterJvm
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterJvm
  {
    [DataMember(Name = "max_uptime_in_millis")]
    public long MaxUptimeInMilliseconds { get; internal set; }

    [DataMember(Name = "mem")]
    public ClusterJvmMemory Memory { get; internal set; }

    [DataMember(Name = "threads")]
    public long Threads { get; internal set; }

    [DataMember(Name = "versions")]
    public IReadOnlyCollection<ClusterJvmVersion> Versions { get; internal set; }
  }
}
