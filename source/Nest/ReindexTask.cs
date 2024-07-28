// Decompiled with JetBrains decompiler
// Type: Nest.ReindexTask
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class ReindexTask
  {
    [DataMember(Name = "action")]
    public string Action { get; internal set; }

    [DataMember(Name = "cancellable")]
    public bool Cancellable { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "id")]
    public long Id { get; internal set; }

    [DataMember(Name = "node")]
    public string Node { get; internal set; }

    [DataMember(Name = "running_time_in_nanos")]
    public long RunningTimeInNanoseconds { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "status")]
    public ReindexStatus Status { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}
