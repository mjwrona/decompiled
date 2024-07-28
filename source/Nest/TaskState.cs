// Decompiled with JetBrains decompiler
// Type: Nest.TaskState
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class TaskState
  {
    [DataMember(Name = "action")]
    public string Action { get; internal set; }

    [DataMember(Name = "cancellable")]
    public bool Cancellable { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "headers")]
    public IReadOnlyDictionary<string, string> Headers { get; internal set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "id")]
    public long Id { get; internal set; }

    [DataMember(Name = "node")]
    public string Node { get; internal set; }

    [DataMember(Name = "parent_task_id")]
    public TaskId ParentTaskId { get; internal set; }

    [DataMember(Name = "running_time_in_nanos")]
    public long RunningTimeInNanoSeconds { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "status")]
    public TaskStatus Status { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}
