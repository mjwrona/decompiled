// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.VirtualMachineGroupConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class VirtualMachineGroupConverter
  {
    public static VirtualMachineGroup ToResource(
      this VirtualMachineGroupCreateParameters createParameters,
      int environmentId)
    {
      VirtualMachineGroup resource = new VirtualMachineGroup();
      resource.Name = createParameters.Name;
      resource.Type = EnvironmentResourceType.VirtualMachine;
      resource.PoolId = 0;
      resource.EnvironmentReference.Id = environmentId;
      return resource;
    }
  }
}
