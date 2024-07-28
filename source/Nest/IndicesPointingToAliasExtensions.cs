// Decompiled with JetBrains decompiler
// Type: Nest.IndicesPointingToAliasExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest
{
  public static class IndicesPointingToAliasExtensions
  {
    public static IReadOnlyCollection<string> GetIndicesPointingToAlias(
      this IElasticClient client,
      Names alias)
    {
      GetAliasResponse alias1 = client.Indices.GetAlias(Indices.All, (Func<GetAliasDescriptor, IGetAliasRequest>) (a => (IGetAliasRequest) a.Name(alias).RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions()))));
      return IndicesPointingToAliasExtensions.IndicesPointingToAlias(client.ConnectionSettings, (IUrlParameter) alias, alias1);
    }

    public static async Task<IReadOnlyCollection<string>> GetIndicesPointingToAliasAsync(
      this IElasticClient client,
      Names alias)
    {
      return IndicesPointingToAliasExtensions.IndicesPointingToAlias(client.ConnectionSettings, (IUrlParameter) alias, await client.Indices.GetAliasAsync(Indices.All, (Func<GetAliasDescriptor, IGetAliasRequest>) (a => (IGetAliasRequest) a.Name(alias).RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions())))).ConfigureAwait(false));
    }

    private static IReadOnlyCollection<string> IndicesPointingToAlias(
      IConnectionSettingsValues settings,
      IUrlParameter alias,
      GetAliasResponse aliasesResponse)
    {
      if (!aliasesResponse.IsValid || !aliasesResponse.Indices.HasAny<KeyValuePair<IndexName, IndexAliases>>())
        return EmptyReadOnly<string>.Collection;
      string[] aliases = alias.GetString((IConnectionConfigurationValues) settings).Split(',');
      return (IReadOnlyCollection<string>) aliasesResponse.Indices.Where<KeyValuePair<IndexName, IndexAliases>>((Func<KeyValuePair<IndexName, IndexAliases>, bool>) (i =>
      {
        IndexAliases indexAliases = i.Value;
        bool? nullable;
        if (indexAliases == null)
        {
          nullable = new bool?();
        }
        else
        {
          IReadOnlyDictionary<string, AliasDefinition> aliases1 = indexAliases.Aliases;
          if (aliases1 == null)
          {
            nullable = new bool?();
          }
          else
          {
            IEnumerable<string> keys = aliases1.Keys;
            nullable = keys != null ? new bool?(keys.Any<string>((Func<string, bool>) (key => ((IEnumerable<string>) aliases).Contains<string>(key)))) : new bool?();
          }
        }
        return nullable.GetValueOrDefault();
      })).Select<KeyValuePair<IndexName, IndexAliases>, string>((Func<KeyValuePair<IndexName, IndexAliases>, string>) (i => settings.Inferrer.IndexName(i.Key))).ToList<string>();
    }
  }
}
