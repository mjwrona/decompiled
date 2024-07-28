// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class MachinePoolManager : VssHttpClientBase, IMachinePoolManager
  {
    private string m_poolType;
    private VssCredentials m_credentials;
    private Func<DelegatingHandler[]> m_handlerFactory;

    internal MachinePoolManager(
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

    public MachineInstanceManager GetMachineManager(
      string poolName,
      VssHttpRequestSettings settings = null)
    {
      return new MachineInstanceManager(this.BaseAddress, this.m_credentials, this.m_poolType, poolName, settings, this.m_handlerFactory());
    }

    public MachineRequestManager GetRequestManager(string poolName, VssHttpRequestSettings settings = null) => new MachineRequestManager(this.BaseAddress, this.m_credentials, this.m_poolType, poolName, settings, this.m_handlerFactory());

    public Task<HttpResponseMessage> CreatePoolAsync(MachinePool pool) => this.PostAsync<MachinePool>(pool, WebApiConstants.PoolsLocationId, (object) new
    {
      poolType = this.m_poolType
    });

    public Task<HttpResponseMessage> DeletePoolAsync(string poolName) => this.DeleteAsync(WebApiConstants.PoolsLocationId, (object) new
    {
      poolType = this.m_poolType,
      poolName = poolName
    });

    public Task<List<MachinePool>> GetPoolsAsync(IEnumerable<string> propertyFilters = null)
    {
      List<KeyValuePair<string, string>> queryParameters = (List<KeyValuePair<string, string>>) null;
      if (propertyFilters != null)
        queryParameters = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("properties", string.Join(",", propertyFilters))
        };
      return this.GetAsync<List<MachinePool>>(WebApiConstants.PoolsLocationId, (object) new
      {
        poolType = this.m_poolType
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters);
    }

    public Task<MachinePool> GetPoolAsync(string poolName, IEnumerable<string> propertyFilters = null)
    {
      List<KeyValuePair<string, string>> queryParameters = (List<KeyValuePair<string, string>>) null;
      if (propertyFilters != null)
        queryParameters = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("properties", string.Join(",", propertyFilters))
        };
      return this.GetAsync<MachinePool>(WebApiConstants.PoolsLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = poolName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters);
    }

    public Task PublishNotificationAsync(string poolName, MachinePoolEvent notification)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
      ArgumentUtility.CheckForNull<MachinePoolEvent>(notification, nameof (notification));
      return (Task) this.PostAsync<MachinePoolEvent>(notification, WebApiConstants.EventsLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = poolName
      });
    }

    public Task UpdatePoolAsync(
      string poolName,
      string state = null,
      int? machineCount = null,
      string imageName = null,
      PropertiesCollection properties = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(state))
        keyValuePairList.Add(nameof (state), state);
      if (machineCount.HasValue)
        keyValuePairList.Add(nameof (machineCount), machineCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(imageName))
        keyValuePairList.Add(nameof (imageName), imageName);
      return (Task) this.PatchAsync<PropertiesCollection>(properties, WebApiConstants.PoolsLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = poolName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }
  }
}
