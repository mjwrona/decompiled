// Decompiled with JetBrains decompiler
// Type: Nest.PhaseExecution
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PhaseExecution
  {
    [DataMember(Name = "modified_date_in_millis")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset ModifiedDate { get; internal set; }

    [DataMember(Name = "phase_definition")]
    public IPhase PhaseDefinition { get; internal set; }

    [DataMember(Name = "policy")]
    public string Policy { get; internal set; }

    [DataMember(Name = "version")]
    public int Version { get; internal set; }
  }
}
