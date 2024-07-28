// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class MachineRequestManager : VssHttpClientBase
  {
    private string m_poolType;
    private string m_poolName;

    internal MachineRequestManager(
      Uri baseUrl,
      VssCredentials credentials,
      string poolType,
      string poolName,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.m_poolType = poolType;
      this.m_poolName = poolName;
    }

    public Task FinishRequestAsync(
      long requestId,
      string instanceName,
      byte[] accessToken,
      MachineRequestResult result)
    {
      return (Task) this.PostAsync<RequestStateData>(new RequestStateData()
      {
        AccessToken = accessToken,
        InstanceName = instanceName,
        Result = result,
        State = RequestState.Finish
      }, WebApiConstants.RequestsLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        requestId = requestId
      });
    }

    public Task StartRequestAsync(long requestId, string instanceName, byte[] accessToken) => (Task) this.PostAsync<RequestStateData>(new RequestStateData()
    {
      AccessToken = accessToken,
      InstanceName = instanceName,
      State = RequestState.Start
    }, WebApiConstants.RequestsLocationId, (object) new
    {
      poolType = this.m_poolType,
      poolName = this.m_poolName,
      requestId = requestId
    });
  }
}
