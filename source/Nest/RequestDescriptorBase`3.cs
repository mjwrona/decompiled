// Decompiled with JetBrains decompiler
// Type: Nest.RequestDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nest
{
  public abstract class RequestDescriptorBase<TDescriptor, TParameters, TInterface> : 
    RequestBase<TParameters>,
    IDescriptor
    where TDescriptor : RequestDescriptorBase<TDescriptor, TParameters, TInterface>, TInterface
    where TParameters : RequestParameters<TParameters>, new()
  {
    private readonly TDescriptor _descriptor;

    protected RequestDescriptorBase() => this._descriptor = (TDescriptor) this;

    protected RequestDescriptorBase(Func<RouteValues, RouteValues> pathSelector)
      : base(pathSelector)
    {
      this._descriptor = (TDescriptor) this;
    }

    protected TInterface Self => (TInterface) this._descriptor;

    protected TDescriptor Assign<TValue>(TValue value, Action<TInterface, TValue> assign) => Fluent.Assign<TDescriptor, TInterface, TValue>(this._descriptor, value, assign);

    protected TDescriptor Qs(string name, object value)
    {
      this.Q(name, value);
      return this._descriptor;
    }

    public TDescriptor RequestConfiguration(
      Func<RequestConfigurationDescriptor, IRequestConfiguration> configurationSelector)
    {
      IRequestConfiguration requestConfiguration = this.RequestState.RequestParameters.RequestConfiguration;
      this.RequestState.RequestParameters.RequestConfiguration = (configurationSelector != null ? configurationSelector(new RequestConfigurationDescriptor(requestConfiguration)) : (IRequestConfiguration) null) ?? requestConfiguration;
      return this._descriptor;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => base.Equals(obj);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => base.GetHashCode();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString() => base.ToString();

    public TDescriptor ErrorTrace(bool? errortrace = true) => this.Qs("error_trace", (object) errortrace);

    public TDescriptor FilterPath(params string[] filterpath) => this.Qs("filter_path", (object) filterpath);

    public TDescriptor FilterPath(IEnumerable<string> filterpath) => this.Qs("filter_path", (object) filterpath);

    public TDescriptor Human(bool? human = true) => this.Qs(nameof (human), (object) human);

    public TDescriptor Pretty(bool? pretty = true) => this.Qs(nameof (pretty), (object) pretty);

    public TDescriptor SourceQueryString(string sourcequerystring) => this.Qs("source", (object) sourcequerystring);
  }
}
