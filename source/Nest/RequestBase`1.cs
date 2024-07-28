// Decompiled with JetBrains decompiler
// Type: Nest.RequestBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public abstract class RequestBase<TParameters> : IRequest<TParameters>, IRequest where TParameters : class, IRequestParameters, new()
  {
    private readonly TParameters _parameters;

    protected RequestBase()
    {
      this._parameters = new TParameters();
      this.RequestDefaults(this._parameters);
    }

    protected RequestBase(Func<RouteValues, RouteValues> pathSelector)
    {
      RouteValues routeValues = pathSelector(this.RequestState.RouteValues);
      this._parameters = new TParameters();
      this.RequestDefaults(this._parameters);
    }

    protected virtual HttpMethod HttpMethod => this.RequestState.RequestParameters.DefaultHttpMethod;

    [IgnoreDataMember]
    protected IRequest<TParameters> RequestState => (IRequest<TParameters>) this;

    [IgnoreDataMember]
    HttpMethod IRequest.HttpMethod => this.HttpMethod;

    [IgnoreDataMember]
    string IRequest.ContentType => this.ContentType;

    protected virtual string ContentType { get; }

    [IgnoreDataMember]
    TParameters IRequest<TParameters>.RequestParameters => this._parameters;

    IRequestParameters IRequest.RequestParameters => (IRequestParameters) this._parameters;

    [IgnoreDataMember]
    RouteValues IRequest.RouteValues { get; } = new RouteValues();

    internal abstract ApiUrls ApiUrls { get; }

    string IRequest.GetUrl(IConnectionSettingsValues settings) => this.ResolveUrl(this.RequestState.RouteValues, settings);

    protected virtual string ResolveUrl(RouteValues routeValues, IConnectionSettingsValues settings) => this.ApiUrls.Resolve(routeValues, settings);

    protected virtual void RequestDefaults(TParameters parameters)
    {
    }

    protected TOut Q<TOut>(string name) => this.RequestState.RequestParameters.GetQueryStringValue<TOut>(name);

    protected void Q(string name, object value) => this.RequestState.RequestParameters.SetQueryString(name, value);

    protected void SetAcceptHeader(string format)
    {
      if (this.RequestState.RequestParameters.RequestConfiguration == null)
        this.RequestState.RequestParameters.RequestConfiguration = (IRequestConfiguration) new RequestConfiguration();
      this.RequestState.RequestParameters.RequestConfiguration.Accept = this.RequestState.RequestParameters.AcceptHeaderFromFormat(format);
    }
  }
}
