// Decompiled with JetBrains decompiler
// Type: Nest.MigrateToDataTiersResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class MigrateToDataTiersResponse : ResponseBase
  {
    [DataMember(Name = "dry_run")]
    public bool DryRun { get; internal set; }

    [DataMember(Name = "migrated_ilm_policies")]
    public IEnumerable<string> MigratedIlmPolicies { get; internal set; }

    [DataMember(Name = "migrated_indices")]
    public IEnumerable<string> MigratedIndices { get; internal set; }

    [DataMember(Name = "removed_legacy_template")]
    public string RemovedLegacyTemplate { get; internal set; }
  }
}
