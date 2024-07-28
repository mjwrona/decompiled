// Decompiled with JetBrains decompiler
// Type: Nest.MigrateToDataTiersRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;

namespace Nest
{
  public class MigrateToDataTiersRequest : 
    PlainRequestBase<MigrateToDataTiersRequestParameters>,
    IMigrateToDataTiersRequest,
    IRequest<MigrateToDataTiersRequestParameters>,
    IRequest
  {
    protected IMigrateToDataTiersRequest Self => (IMigrateToDataTiersRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementMigrateToDataTiers;

    public bool? DryRun
    {
      get => this.Q<bool?>("dry_run");
      set => this.Q("dry_run", (object) value);
    }

    public string LegacyTemplateToDelete { get; set; }

    public string NodeAttribute { get; set; }
  }
}
