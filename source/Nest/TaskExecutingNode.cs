// Decompiled with JetBrains decompiler
// Type: Nest.TaskExecutingNode
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class TaskExecutingNode
  {
    [DataMember(Name = "attributes")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, string>))]
    public IReadOnlyDictionary<string, string> Attributes { get; internal set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "host")]
    public string Host { get; internal set; }

    [DataMember(Name = "ip")]
    public string Ip { get; internal set; }

    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "roles")]
    public IEnumerable<string> Roles { get; internal set; }

    [DataMember(Name = "tasks")]
    public IReadOnlyDictionary<TaskId, TaskState> Tasks { get; internal set; } = EmptyReadOnly<TaskId, TaskState>.Dictionary;

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; internal set; }
  }
}
