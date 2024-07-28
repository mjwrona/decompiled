// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.PaginationOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal abstract class PaginationOptions
  {
    protected static readonly ImmutableHashSet<string> bannedAdditionalHeaders = new HashSet<string>()
    {
      "x-ms-max-item-count",
      "x-ms-documentdb-content-serialization-format"
    }.ToImmutableHashSet<string>();
    private static readonly ImmutableDictionary<string, string> EmptyDictionary = new Dictionary<string, string>().ToImmutableDictionary<string, string>();

    protected PaginationOptions(
      int? pageSizeLimit = null,
      Microsoft.Azure.Cosmos.Json.JsonSerializationFormat? jsonSerializationFormat = null,
      Dictionary<string, string> additionalHeaders = null)
    {
      this.PageSizeLimit = pageSizeLimit;
      this.JsonSerializationFormat = jsonSerializationFormat;
      this.AdditionalHeaders = additionalHeaders != null ? additionalHeaders.ToImmutableDictionary<string, string>() : PaginationOptions.EmptyDictionary;
      foreach (string key in this.AdditionalHeaders.Keys)
      {
        if (PaginationOptions.bannedAdditionalHeaders.Contains(key) || this.BannedAdditionalHeaders.Contains(key))
          throw new ArgumentOutOfRangeException("The following http header is not allowed: '" + key + "'");
      }
    }

    public int? PageSizeLimit { get; }

    public Microsoft.Azure.Cosmos.Json.JsonSerializationFormat? JsonSerializationFormat { get; }

    public ImmutableDictionary<string, string> AdditionalHeaders { get; }

    protected abstract ImmutableHashSet<string> BannedAdditionalHeaders { get; }
  }
}
