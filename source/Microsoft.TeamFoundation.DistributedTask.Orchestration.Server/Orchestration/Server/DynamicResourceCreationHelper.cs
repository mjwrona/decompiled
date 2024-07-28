// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DynamicResourceCreationHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class DynamicResourceCreationHelper
  {
    public static readonly string dynamicResourceTask = "ReviewApp@0";

    public static bool NeedsDynamicResourceCreation(JToken deploymentStrategy) => deploymentStrategy.ToString().Contains(DynamicResourceCreationHelper.dynamicResourceTask);

    public static string GetDynamicResourceSourceName(JToken deploymentStrategy)
    {
      JToken jtoken1 = deploymentStrategy.SelectToken("$.*.*.*[?(@.task == 'ReviewApp@0')].inputs.resourceName");
      if (jtoken1 != null)
        return jtoken1.ToString();
      JToken jtoken2 = deploymentStrategy.SelectToken("$.*.*.*.*[?(@.task == 'ReviewApp@0')].inputs.resourceName");
      return jtoken2 != null ? jtoken2.ToString() : "";
    }
  }
}
