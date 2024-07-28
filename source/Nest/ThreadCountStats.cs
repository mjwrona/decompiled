// Decompiled with JetBrains decompiler
// Type: Nest.ThreadCountStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ThreadCountStats
  {
    [DataMember(Name = "active")]
    public long Active { get; internal set; }

    [DataMember(Name = "completed")]
    public long Completed { get; internal set; }

    [DataMember(Name = "largest")]
    public long Largest { get; internal set; }

    [DataMember(Name = "queue")]
    public long Queue { get; internal set; }

    [DataMember(Name = "rejected")]
    public long Rejected { get; internal set; }

    [DataMember(Name = "threads")]
    public long Threads { get; internal set; }
  }
}
