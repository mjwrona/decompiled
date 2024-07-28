// Decompiled with JetBrains decompiler
// Type: Nest.ClusterGetSettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class ClusterGetSettingsDescriptor : 
    RequestDescriptorBase<ClusterGetSettingsDescriptor, ClusterGetSettingsRequestParameters, IClusterGetSettingsRequest>,
    IClusterGetSettingsRequest,
    IRequest<ClusterGetSettingsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterGetSettings;

    public ClusterGetSettingsDescriptor FlatSettings(bool? flatsettings = true) => this.Qs("flat_settings", (object) flatsettings);

    public ClusterGetSettingsDescriptor IncludeDefaults(bool? includedefaults = true) => this.Qs("include_defaults", (object) includedefaults);

    public ClusterGetSettingsDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public ClusterGetSettingsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
