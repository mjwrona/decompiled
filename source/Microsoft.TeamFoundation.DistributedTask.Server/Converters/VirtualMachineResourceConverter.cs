// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.VirtualMachineResourceConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class VirtualMachineResourceConverter
  {
    public static VirtualMachineResource ToResource(
      this VirtualMachineResourceCreateParameters createParameters,
      int environmentId)
    {
      TaskAgent taskAgent = createParameters.virtualMachineResource.Agent.Clone();
      taskAgent.Name = createParameters.virtualMachineResource.Name;
      VirtualMachineResource resource = new VirtualMachineResource();
      resource.Name = createParameters.virtualMachineResource.Name;
      resource.Type = EnvironmentResourceType.VirtualMachine;
      resource.Agent = taskAgent;
      resource.EnvironmentReference.Id = environmentId;
      resource.Tags = createParameters.virtualMachineResource.Tags;
      return resource;
    }
  }
}
