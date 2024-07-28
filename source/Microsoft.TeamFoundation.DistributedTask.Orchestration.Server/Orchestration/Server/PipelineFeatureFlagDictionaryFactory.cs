// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PipelineFeatureFlagDictionaryFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class PipelineFeatureFlagDictionaryFactory
  {
    internal static IDictionary<string, bool> Create(IVssRequestContext requestContext) => (IDictionary<string, bool>) new Dictionary<string, bool>()
    {
      {
        "DistributedTask.SetSecretDependenciesOutputs",
        requestContext.IsFeatureEnabled("DistributedTask.SetSecretDependenciesOutputs")
      },
      {
        "DistributedTask.DoNotAddDefaultVmImageToExistingAgentSpecifcations",
        requestContext.IsFeatureEnabled("DistributedTask.DoNotAddDefaultVmImageToExistingAgentSpecifcations")
      },
      {
        "DistributedTask.DoNotAddDefaultVmImageForNonYamlPipelines",
        requestContext.IsFeatureEnabled("DistributedTask.DoNotAddDefaultVmImageForNonYamlPipelines")
      },
      {
        "DistributedTask.DeploymentJobAllowTaskMinorVersion",
        requestContext.IsFeatureEnabled("DistributedTask.DeploymentJobAllowTaskMinorVersion")
      },
      {
        "Pipelines.PersistedStages.EnableAutoCreation",
        requestContext.IsFeatureEnabled("Pipelines.PersistedStages.EnableAutoCreation")
      },
      {
        "DistributedTask.FixForDownloadBuildTaskNotResolved",
        requestContext.IsFeatureEnabled("DistributedTask.FixForDownloadBuildTaskNotResolved")
      }
    };
  }
}
