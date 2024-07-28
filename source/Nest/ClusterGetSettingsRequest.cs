// Decompiled with JetBrains decompiler
// Type: Nest.ClusterGetSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class ClusterGetSettingsRequest : 
    PlainRequestBase<ClusterGetSettingsRequestParameters>,
    IClusterGetSettingsRequest,
    IRequest<ClusterGetSettingsRequestParameters>,
    IRequest
  {
    protected IClusterGetSettingsRequest Self => (IClusterGetSettingsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterGetSettings;

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
    }

    public bool? IncludeDefaults
    {
      get => this.Q<bool?>("include_defaults");
      set => this.Q("include_defaults", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
