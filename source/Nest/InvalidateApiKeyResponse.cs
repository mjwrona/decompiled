// Decompiled with JetBrains decompiler
// Type: Nest.InvalidateApiKeyResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class InvalidateApiKeyResponse : ResponseBase
  {
    [DataMember(Name = "error_count")]
    public int? ErrorCount { get; internal set; }

    [DataMember(Name = "error_details")]
    public IReadOnlyCollection<ErrorCause> ErrorDetails { get; internal set; } = EmptyReadOnly<ErrorCause>.Collection;

    [DataMember(Name = "invalidated_api_keys")]
    public IReadOnlyCollection<string> InvalidatedApiKeys { get; internal set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "previously_invalidated_api_keys")]
    public IReadOnlyCollection<string> PreviouslyInvalidatedApiKeys { get; internal set; } = EmptyReadOnly<string>.Collection;
  }
}
