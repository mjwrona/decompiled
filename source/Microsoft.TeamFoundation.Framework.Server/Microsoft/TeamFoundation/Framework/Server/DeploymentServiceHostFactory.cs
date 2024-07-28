// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeploymentServiceHostFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DeploymentServiceHostFactory
  {
    public static IVssDeploymentServiceHost CreateDeploymentServiceHost(
      HostProperties hostProperties,
      ISqlConnectionInfo connectionInfo,
      IVssExceptionHandler exceptionHandler = null)
    {
      return DeploymentServiceHostFactory.CreateDeploymentServiceHost(hostProperties, connectionInfo, new DeploymentServiceHostOptions(), exceptionHandler);
    }

    public static IVssDeploymentServiceHost CreateDeploymentServiceHost(
      HostProperties hostProperties,
      ISqlConnectionInfo connectionInfo,
      DeploymentServiceHostOptions options,
      IVssExceptionHandler exceptionHandler = null)
    {
      return (IVssDeploymentServiceHost) new DeploymentServiceHost(hostProperties, connectionInfo, options, exceptionHandler);
    }
  }
}
