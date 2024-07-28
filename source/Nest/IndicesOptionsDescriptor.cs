// Decompiled with JetBrains decompiler
// Type: Nest.IndicesOptionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IndicesOptionsDescriptor : 
    DescriptorBase<IndicesOptionsDescriptor, IIndicesOptions>,
    IIndicesOptions
  {
    bool? IIndicesOptions.AllowNoIndices { get; set; }

    IEnumerable<Elasticsearch.Net.ExpandWildcards> IIndicesOptions.ExpandWildcards { get; set; }

    bool? IIndicesOptions.IgnoreUnavailable { get; set; }

    public IndicesOptionsDescriptor ExpandWildcards(IEnumerable<Elasticsearch.Net.ExpandWildcards> expandWildcards) => this.Assign<IEnumerable<Elasticsearch.Net.ExpandWildcards>>(expandWildcards, (Action<IIndicesOptions, IEnumerable<Elasticsearch.Net.ExpandWildcards>>) ((a, v) => a.ExpandWildcards = v));

    public IndicesOptionsDescriptor ExpandWildcards(params Elasticsearch.Net.ExpandWildcards[] expandWildcards) => this.Assign<Elasticsearch.Net.ExpandWildcards[]>(expandWildcards, (Action<IIndicesOptions, Elasticsearch.Net.ExpandWildcards[]>) ((a, v) => a.ExpandWildcards = (IEnumerable<Elasticsearch.Net.ExpandWildcards>) v));

    public IndicesOptionsDescriptor IgnoreUnavailable(bool? ignoreUnavailable = true) => this.Assign<bool?>(ignoreUnavailable, (Action<IIndicesOptions, bool?>) ((a, v) => a.IgnoreUnavailable = v));

    public IndicesOptionsDescriptor AllowNoIndices(bool? allowNoIndices = true) => this.Assign<bool?>(allowNoIndices, (Action<IIndicesOptions, bool?>) ((a, v) => a.AllowNoIndices = v));
  }
}
