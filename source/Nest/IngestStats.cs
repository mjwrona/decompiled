// Decompiled with JetBrains decompiler
// Type: Nest.IngestStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IngestStats
  {
    [DataMember(Name = "count")]
    public long Count { get; set; }

    [DataMember(Name = "current")]
    public long Current { get; set; }

    [DataMember(Name = "failed")]
    public long Failed { get; set; }

    [DataMember(Name = "time_in_millis")]
    public long TimeInMilliseconds { get; set; }

    [DataMember(Name = "processors")]
    public IReadOnlyCollection<KeyedProcessorStats> Processors { get; internal set; } = EmptyReadOnly<KeyedProcessorStats>.Collection;
  }
}
