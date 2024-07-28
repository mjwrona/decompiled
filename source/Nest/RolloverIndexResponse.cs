// Decompiled with JetBrains decompiler
// Type: Nest.RolloverIndexResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RolloverIndexResponse : AcknowledgedResponseBase
  {
    [DataMember(Name = "conditions")]
    public IReadOnlyDictionary<string, bool> Conditions { get; internal set; } = EmptyReadOnly<string, bool>.Dictionary;

    [DataMember(Name = "dry_run")]
    public bool DryRun { get; internal set; }

    [DataMember(Name = "new_index")]
    public string NewIndex { get; internal set; }

    [DataMember(Name = "old_index")]
    public string OldIndex { get; internal set; }

    [DataMember(Name = "rolled_over")]
    public bool RolledOver { get; internal set; }

    [DataMember(Name = "shards_acknowledged")]
    public bool ShardsAcknowledged { get; internal set; }
  }
}
