// Decompiled with JetBrains decompiler
// Type: Nest.AggregatedDanglingIndexInfo
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class AggregatedDanglingIndexInfo
  {
    private DateTimeOffset? _creationDate;

    [DataMember(Name = "index_name")]
    public string IndexName { get; internal set; }

    [DataMember(Name = "index_uuid")]
    public string IndexUUID { get; internal set; }

    [DataMember(Name = "creation_date_millis")]
    public long CreationDateInMilliseconds { get; internal set; }

    [DataMember(Name = "creation_date")]
    public DateTimeOffset CreationDate
    {
      get
      {
        this._creationDate.GetValueOrDefault();
        if (!this._creationDate.HasValue)
          this._creationDate = new DateTimeOffset?(DateTimeOffset.FromUnixTimeMilliseconds(this.CreationDateInMilliseconds));
        return this._creationDate.Value;
      }
      internal set => this._creationDate = new DateTimeOffset?(value);
    }

    [DataMember(Name = "node_ids")]
    public IReadOnlyCollection<string> NodeIds { get; internal set; }
  }
}
