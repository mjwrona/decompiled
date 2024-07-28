// Decompiled with JetBrains decompiler
// Type: Nest.ClusterPutSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class ClusterPutSettingsDescriptor : 
    RequestDescriptorBase<ClusterPutSettingsDescriptor, ClusterPutSettingsRequestParameters, IClusterPutSettingsRequest>,
    IClusterPutSettingsRequest,
    IRequest<ClusterPutSettingsRequestParameters>,
    IRequest
  {
    IDictionary<string, object> IClusterPutSettingsRequest.Persistent { get; set; }

    IDictionary<string, object> IClusterPutSettingsRequest.Transient { get; set; }

    public ClusterPutSettingsDescriptor Persistent(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IClusterPutSettingsRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Persistent = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public ClusterPutSettingsDescriptor Transient(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IClusterPutSettingsRequest, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Transient = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPutSettings;

    public ClusterPutSettingsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public ClusterPutSettingsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ClusterPutSettingsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
