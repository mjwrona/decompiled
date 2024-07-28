// Decompiled with JetBrains decompiler
// Type: Nest.AliasPointingToIndexExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nest
{
  public static class AliasPointingToIndexExtensions
  {
    public static IReadOnlyDictionary<string, AliasDefinition> GetAliasesPointingToIndex(
      this IElasticClient client,
      IndexName index)
    {
      GetAliasResponse alias = client.Indices.GetAlias((Indices) index, (Func<GetAliasDescriptor, IGetAliasRequest>) (a => (IGetAliasRequest) a.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions()))));
      return AliasPointingToIndexExtensions.AliasesPointingToIndex(index, alias);
    }

    public static async Task<IReadOnlyDictionary<string, AliasDefinition>> GetAliasesPointingToIndexAsync(
      this IElasticClient client,
      IndexName index)
    {
      return AliasPointingToIndexExtensions.AliasesPointingToIndex(index, await client.Indices.GetAliasAsync((Indices) index, (Func<GetAliasDescriptor, IGetAliasRequest>) (a => (IGetAliasRequest) a.RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions())))).ConfigureAwait(false));
    }

    private static IReadOnlyDictionary<string, AliasDefinition> AliasesPointingToIndex(
      IndexName index,
      GetAliasResponse response)
    {
      return !response.IsValid || !response.Indices.HasAny<KeyValuePair<IndexName, IndexAliases>>() ? EmptyReadOnly<string, AliasDefinition>.Dictionary : response.Indices[index].Aliases;
    }
  }
}
