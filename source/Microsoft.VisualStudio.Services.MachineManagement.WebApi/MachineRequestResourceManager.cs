// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestResourceManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class MachineRequestResourceManager : 
    VssHttpClientBase,
    IMachineRequestResourceManager
  {
    private string m_poolType;
    private VssCredentials m_credentials;
    private Func<DelegatingHandler[]> m_handlerFactory;

    internal MachineRequestResourceManager(
      Uri url,
      string poolType,
      VssCredentials credentials,
      Func<DelegatingHandler[]> handlerFactory)
      : base(url, credentials, handlerFactory())
    {
      this.m_poolType = poolType;
      this.m_credentials = credentials;
      this.m_handlerFactory = handlerFactory;
    }

    public Task<MachineRequestResource> GetResourceAsync(string resourceVersion) => this.GetAsync<MachineRequestResource>(WebApiConstants.RequestResourcesLocationId, (object) new
    {
      poolType = this.m_poolType,
      version = resourceVersion
    });

    public Task<List<MachineRequestResource>> GetResourcesAsync() => this.GetAsync<List<MachineRequestResource>>(WebApiConstants.RequestResourcesLocationId, (object) new
    {
      poolType = this.m_poolType
    });
  }
}
