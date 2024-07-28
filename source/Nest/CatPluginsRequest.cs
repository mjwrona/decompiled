// Decompiled with JetBrains decompiler
// Type: Nest.CatPluginsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;

namespace Nest
{
  public class CatPluginsRequest : 
    PlainRequestBase<CatPluginsRequestParameters>,
    ICatPluginsRequest,
    IRequest<CatPluginsRequestParameters>,
    IRequest
  {
    protected ICatPluginsRequest Self => (ICatPluginsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatPlugins;

    public string Format
    {
      get => this.Q<string>("format");
      set
      {
        this.Q("format", (object) value);
        this.SetAcceptHeader(value);
      }
    }

    public string[] Headers
    {
      get => this.Q<string[]>("h");
      set => this.Q("h", (object) value);
    }

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
    }

    public bool? IncludeBootstrap
    {
      get => this.Q<bool?>("include_bootstrap");
      set => this.Q("include_bootstrap", (object) value);
    }

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("v");
      set => this.Q("v", (object) value);
    }
  }
}
