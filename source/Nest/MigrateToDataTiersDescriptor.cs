// Decompiled with JetBrains decompiler
// Type: Nest.MigrateToDataTiersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndexLifecycleManagementApi;
using System;

namespace Nest
{
  public class MigrateToDataTiersDescriptor : 
    RequestDescriptorBase<MigrateToDataTiersDescriptor, MigrateToDataTiersRequestParameters, IMigrateToDataTiersRequest>,
    IMigrateToDataTiersRequest,
    IRequest<MigrateToDataTiersRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndexLifecycleManagementMigrateToDataTiers;

    public MigrateToDataTiersDescriptor DryRun(bool? dryrun = true) => this.Qs("dry_run", (object) dryrun);

    string IMigrateToDataTiersRequest.LegacyTemplateToDelete { get; set; }

    string IMigrateToDataTiersRequest.NodeAttribute { get; set; }

    public MigrateToDataTiersDescriptor LegacyTemplateToDelete(string legacyTemplateToDelete) => this.Assign<string>(legacyTemplateToDelete, (Action<IMigrateToDataTiersRequest, string>) ((a, v) => a.LegacyTemplateToDelete = v));

    public MigrateToDataTiersDescriptor NodeAttribute(string nodeAttribute) => this.Assign<string>(nodeAttribute, (Action<IMigrateToDataTiersRequest, string>) ((a, v) => a.NodeAttribute = v));
  }
}
