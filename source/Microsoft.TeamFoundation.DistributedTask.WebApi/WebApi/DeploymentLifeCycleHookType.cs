// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentLifeCycleHookType
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [Flags]
  public enum DeploymentLifeCycleHookType
  {
    Undefined = 0,
    Deploy = 1,
    PreDeploy = 2,
    RouteTraffic = 4,
    PostRouteTraffic = 8,
    OnSuccess = 16, // 0x00000010
    OnFailure = 32, // 0x00000020
  }
}
