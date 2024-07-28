// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.IMachinePoolManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public interface IMachinePoolManager
  {
    Task<HttpResponseMessage> CreatePoolAsync(MachinePool pool);

    Task<HttpResponseMessage> DeletePoolAsync(string poolName);

    MachineInstanceManager GetMachineManager(string poolName, VssHttpRequestSettings settings = null);

    Task<MachinePool> GetPoolAsync(string poolName, IEnumerable<string> propertyFilters = null);

    Task<List<MachinePool>> GetPoolsAsync(IEnumerable<string> propertyFilters = null);

    MachineRequestManager GetRequestManager(string poolName, VssHttpRequestSettings settings = null);

    Task PublishNotificationAsync(string poolName, MachinePoolEvent notification);

    Task UpdatePoolAsync(
      string poolName,
      string state = null,
      int? machineCount = null,
      string imageName = null,
      PropertiesCollection properties = null);
  }
}
