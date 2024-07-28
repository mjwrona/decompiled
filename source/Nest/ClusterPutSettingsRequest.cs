// Decompiled with JetBrains decompiler
// Type: Nest.ClusterPutSettingsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using System.Collections.Generic;

namespace Nest
{
  public class ClusterPutSettingsRequest : 
    PlainRequestBase<ClusterPutSettingsRequestParameters>,
    IClusterPutSettingsRequest,
    IRequest<ClusterPutSettingsRequestParameters>,
    IRequest
  {
    public IDictionary<string, object> Persistent { get; set; }

    public IDictionary<string, object> Transient { get; set; }

    protected IClusterPutSettingsRequest Self => (IClusterPutSettingsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPutSettings;

    public bool? FlatSettings
    {
      get => this.Q<bool?>("flat_settings");
      set => this.Q("flat_settings", (object) value);
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
