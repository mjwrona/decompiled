// Decompiled with JetBrains decompiler
// Type: Nest.GetManyExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public static class GetManyExtensions
  {
    private static Func<MultiGetOperationDescriptor<T>, string, IMultiGetOperation> Lookup<T>(
      IndexName index)
      where T : class
    {
      return index == (IndexName) null ? (Func<MultiGetOperationDescriptor<T>, string, IMultiGetOperation>) null : (Func<MultiGetOperationDescriptor<T>, string, IMultiGetOperation>) ((d, id) => (IMultiGetOperation) d.Index(index));
    }

    public static IEnumerable<IMultiGetHit<T>> GetMany<T>(
      this IElasticClient client,
      IEnumerable<string> ids,
      IndexName index = null)
      where T : class
    {
      return client.MultiGet((Func<MultiGetDescriptor, IMultiGetRequest>) (s => (IMultiGetRequest) s.Index(index).RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions())).GetMany<T>(ids, GetManyExtensions.Lookup<T>(index)))).GetMany<T>(ids);
    }

    public static IEnumerable<IMultiGetHit<T>> GetMany<T>(
      this IElasticClient client,
      IEnumerable<long> ids,
      IndexName index = null)
      where T : class
    {
      return client.GetMany<T>(ids.Select<long, string>((Func<long, string>) (i => i.ToString((IFormatProvider) CultureInfo.InvariantCulture))), index);
    }

    public static async Task<IEnumerable<IMultiGetHit<T>>> GetManyAsync<T>(
      this IElasticClient client,
      IEnumerable<string> ids,
      IndexName index = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return (await client.MultiGetAsync((Func<MultiGetDescriptor, IMultiGetRequest>) (s => (IMultiGetRequest) s.Index(index).RequestConfiguration((Func<RequestConfigurationDescriptor, IRequestConfiguration>) (r => (IRequestConfiguration) r.ThrowExceptions())).GetMany<T>(ids, GetManyExtensions.Lookup<T>(index))), cancellationToken).ConfigureAwait(false)).GetMany<T>(ids);
    }

    public static Task<IEnumerable<IMultiGetHit<T>>> GetManyAsync<T>(
      this IElasticClient client,
      IEnumerable<long> ids,
      IndexName index = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return client.GetManyAsync<T>(ids.Select<long, string>((Func<long, string>) (i => i.ToString((IFormatProvider) CultureInfo.InvariantCulture))), index, cancellationToken);
    }
  }
}
