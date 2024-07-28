// Decompiled with JetBrains decompiler
// Type: Nest.Specification.MigrationApi.MigrationNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.MigrationApi
{
  public class MigrationNamespace : Nest.NamespacedClientProxy
  {
    internal MigrationNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeprecationInfoResponse DeprecationInfo(
      Func<DeprecationInfoDescriptor, IDeprecationInfoRequest> selector = null)
    {
      return this.DeprecationInfo(selector.InvokeOrDefault<DeprecationInfoDescriptor, IDeprecationInfoRequest>(new DeprecationInfoDescriptor()));
    }

    public Task<DeprecationInfoResponse> DeprecationInfoAsync(
      Func<DeprecationInfoDescriptor, IDeprecationInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeprecationInfoAsync(selector.InvokeOrDefault<DeprecationInfoDescriptor, IDeprecationInfoRequest>(new DeprecationInfoDescriptor()), ct);
    }

    public DeprecationInfoResponse DeprecationInfo(IDeprecationInfoRequest request) => this.DoRequest<IDeprecationInfoRequest, DeprecationInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeprecationInfoResponse> DeprecationInfoAsync(
      IDeprecationInfoRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeprecationInfoRequest, DeprecationInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
