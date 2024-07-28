// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.EnvironmentDeploymentUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal static class EnvironmentDeploymentUtility
  {
    public static EnvironmentResourceType ConvertToEnvironmentResourceType(string type)
    {
      EnvironmentResourceType environmentResourceType = EnvironmentResourceType.Undefined;
      if (type.Equals("kubernetes", StringComparison.InvariantCultureIgnoreCase))
        environmentResourceType = EnvironmentResourceType.Kubernetes;
      if (type.Equals("virtualMachine", StringComparison.InvariantCultureIgnoreCase))
        environmentResourceType = EnvironmentResourceType.VirtualMachine;
      return environmentResourceType;
    }
  }
}
