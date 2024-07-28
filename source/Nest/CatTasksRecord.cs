// Decompiled with JetBrains decompiler
// Type: Nest.CatTasksRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class CatTasksRecord : ICatRecord
  {
    [DataMember(Name = "action")]
    public string Action { get; internal set; }

    [DataMember(Name = "ip")]
    public string Ip { get; internal set; }

    [DataMember(Name = "node")]
    public string Node { get; internal set; }

    [DataMember(Name = "parent_task_id")]
    public string ParentTaskId { get; internal set; }

    [DataMember(Name = "running_time")]
    public string RunningTime { get; internal set; }

    [DataMember(Name = "start_time")]
    public string StartTime { get; internal set; }

    [DataMember(Name = "task_id")]
    public string TaskId { get; internal set; }

    [DataMember(Name = "timestamp")]
    public string Timestamp { get; internal set; }

    [DataMember(Name = "type")]
    public string Type { get; internal set; }
  }
}
