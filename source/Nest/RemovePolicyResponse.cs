// Decompiled with JetBrains decompiler
// Type: Nest.RemovePolicyResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RemovePolicyResponse : ResponseBase
  {
    [DataMember(Name = "failed_indexes")]
    public IReadOnlyCollection<string> FailedIndexes { get; internal set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "has_failures")]
    public bool HasFailures { get; internal set; }
  }
}
