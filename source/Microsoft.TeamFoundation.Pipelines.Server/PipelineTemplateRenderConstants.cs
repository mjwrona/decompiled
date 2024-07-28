// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelineTemplateRenderConstants
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class PipelineTemplateRenderConstants
  {
    public static readonly string EnvironmentNamePrefix = "configless_";
    public static readonly string ClusterId = "clusterId";
    public static readonly string Namespace = "namespace";
    public static readonly string EndpointKey = "endpoint:";
    public static readonly string KubernetesEndpointType = "endpoint:kubernetes";
    public static readonly string DockerRegistryEndpointType = "endpoint:containerRegistry";
    public const string EnvironmentType = "environment";
    public static readonly string KubernetesResourceType = "environmentResource:kubernetes";
    public static readonly string Branch = "main";
    public static readonly string DefaultBranchKey = "branch";
    public static readonly string DefaultPoolKey = "pool";
    public static readonly string DefaultHostedPool = "vmImage: ubuntu-latest";
    public static readonly string DefaultOnPremPool = "name: default";

    public static string DefaultPool(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment ? PipelineTemplateRenderConstants.DefaultOnPremPool : PipelineTemplateRenderConstants.DefaultHostedPool;
  }
}
