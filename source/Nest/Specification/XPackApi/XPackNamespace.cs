// Decompiled with JetBrains decompiler
// Type: Nest.Specification.XPackApi.XPackNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.XPackApi
{
  public class XPackNamespace : Nest.NamespacedClientProxy
  {
    internal XPackNamespace(ElasticClient client)
      : base(client)
    {
    }

    public XPackInfoResponse Info(
      Func<XPackInfoDescriptor, IXPackInfoRequest> selector = null)
    {
      return this.Info(selector.InvokeOrDefault<XPackInfoDescriptor, IXPackInfoRequest>(new XPackInfoDescriptor()));
    }

    public Task<XPackInfoResponse> InfoAsync(
      Func<XPackInfoDescriptor, IXPackInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.InfoAsync(selector.InvokeOrDefault<XPackInfoDescriptor, IXPackInfoRequest>(new XPackInfoDescriptor()), ct);
    }

    public XPackInfoResponse Info(IXPackInfoRequest request) => this.DoRequest<IXPackInfoRequest, XPackInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<XPackInfoResponse> InfoAsync(IXPackInfoRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IXPackInfoRequest, XPackInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public XPackUsageResponse Usage(
      Func<XPackUsageDescriptor, IXPackUsageRequest> selector = null)
    {
      return this.Usage(selector.InvokeOrDefault<XPackUsageDescriptor, IXPackUsageRequest>(new XPackUsageDescriptor()));
    }

    public Task<XPackUsageResponse> UsageAsync(
      Func<XPackUsageDescriptor, IXPackUsageRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UsageAsync(selector.InvokeOrDefault<XPackUsageDescriptor, IXPackUsageRequest>(new XPackUsageDescriptor()), ct);
    }

    public XPackUsageResponse Usage(IXPackUsageRequest request) => this.DoRequest<IXPackUsageRequest, XPackUsageResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<XPackUsageResponse> UsageAsync(IXPackUsageRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IXPackUsageRequest, XPackUsageResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
