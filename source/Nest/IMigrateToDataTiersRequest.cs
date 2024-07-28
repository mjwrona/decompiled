// Decompiled with JetBrains decompiler
// Type: Nest.IMigrateToDataTiersRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ilm.migrate_to_data_tiers")]
  [ReadAs(typeof (MigrateToDataTiersRequest))]
  public interface IMigrateToDataTiersRequest : 
    IRequest<MigrateToDataTiersRequestParameters>,
    IRequest
  {
    [DataMember(Name = "legacy_template_to_delete")]
    string LegacyTemplateToDelete { get; set; }

    [DataMember(Name = "node_attribute")]
    string NodeAttribute { get; set; }
  }
}
