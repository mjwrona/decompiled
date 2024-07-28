// Decompiled with JetBrains decompiler
// Type: Nest.PlainRequestBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public abstract class PlainRequestBase<TParameters> : RequestBase<TParameters> where TParameters : class, IRequestParameters, new()
  {
    protected PlainRequestBase()
    {
    }

    protected PlainRequestBase(Func<RouteValues, RouteValues> pathSelector)
      : base(pathSelector)
    {
    }

    public IRequestConfiguration RequestConfiguration
    {
      get => this.RequestState.RequestParameters.RequestConfiguration;
      set => this.RequestState.RequestParameters.RequestConfiguration = value;
    }

    public bool? ErrorTrace
    {
      get => this.Q<bool?>("error_trace");
      set => this.Q("error_trace", (object) value);
    }

    public string[] FilterPath
    {
      get => this.Q<string[]>("filter_path");
      set => this.Q("filter_path", (object) value);
    }

    public bool? Human
    {
      get => this.Q<bool?>("human");
      set => this.Q("human", (object) value);
    }

    public bool? Pretty
    {
      get => this.Q<bool?>("pretty");
      set => this.Q("pretty", (object) value);
    }

    public string SourceQueryString
    {
      get => this.Q<string>("source");
      set => this.Q("source", (object) value);
    }
  }
}
