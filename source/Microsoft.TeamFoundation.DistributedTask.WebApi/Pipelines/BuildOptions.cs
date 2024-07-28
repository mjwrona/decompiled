// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildOptions
  {
    public static BuildOptions None { get; } = new BuildOptions();

    public bool AllowEmptyQueueTarget { get; set; }

    public bool AllowHyphenNames { get; set; }

    public bool DemandLatestAgent { get; set; }

    public string MinimumAgentVersion { get; set; }

    public DemandSource MinimumAgentVersionDemandSource { get; set; }

    public bool EnableResourceExpressions { get; set; }

    public bool ResolveResourceVersions { get; set; }

    public bool ResolveTaskInputAliases { get; set; }

    public bool RollupStepDemands { get; set; }

    public bool ValidateExpressions { get; set; }

    public bool ValidateResources { get; set; }

    public bool ValidateStepNames { get; set; }

    public bool ValidateTaskInputs { get; set; }

    public bool ValidatePhaseExpressions { get; set; }

    public IList<int> RestrictedNodeVersions { get; set; }

    public bool ResolvePersistedStages { get; set; }
  }
}
