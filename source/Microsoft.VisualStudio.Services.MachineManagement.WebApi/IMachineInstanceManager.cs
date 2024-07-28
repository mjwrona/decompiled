// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.IMachineInstanceManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public interface IMachineInstanceManager
  {
    Task<MachineInstance> CreateMachineAsync(MachineInstance machine);

    Task DeleteMachineAsync(string instanceName);

    Task<MachineInstance> GetMachineAsync(string instanceName, IEnumerable<string> propertyFilters = null);

    Task<MachineConfiguration> GetMachineConfigurationAsync(string instanceName);

    Task<List<MachineInstance>> GetMachinesAsync(IEnumerable<string> propertyFilters = null);

    Task<MachinePoolAndInstance> RegisterMachineAsync(
      string instanceName,
      string imageName,
      byte[] authorizationToken,
      bool provisioning = false);

    Task<MachineInstance> UpdateMachineAsync(
      string instanceName,
      string state = null,
      bool? enabled = null,
      string imageName = null,
      PropertiesCollection properties = null,
      bool? provisioned = null);

    Task<MachineInstanceMessage> GetMessageAsync(
      string instanceName,
      string queueName,
      string accessToken,
      long? lastMessageId = null);

    Task DeleteMessageAsync(
      string instanceName,
      string queueName,
      string accessToken,
      long messageId);
  }
}
