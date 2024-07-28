// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceManager
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class MachineInstanceManager : VssHttpClientBase, IMachineInstanceManager
  {
    private string m_poolType;
    private string m_poolName;

    internal MachineInstanceManager(
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

    public Task<MachineInstance> CreateMachineAsync(MachineInstance machine) => this.PostAsync<MachineInstance, MachineInstance>(machine, WebApiConstants.MachinesLocationId, (object) new
    {
      poolType = this.m_poolType,
      m_poolName = this.m_poolName
    });

    public Task DeleteMachineAsync(string instanceName) => (Task) this.DeleteAsync(WebApiConstants.MachinesLocationId, (object) new
    {
      poolType = this.m_poolType,
      poolName = this.m_poolName,
      instanceName = instanceName
    });

    public Task<List<MachineInstance>> GetMachinesAsync(IEnumerable<string> propertyFilters = null)
    {
      List<KeyValuePair<string, string>> queryParameters = (List<KeyValuePair<string, string>>) null;
      if (propertyFilters != null)
        queryParameters = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("properties", string.Join(",", propertyFilters.ToArray<string>()))
        };
      return this.GetAsync<List<MachineInstance>>(WebApiConstants.MachinesLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters);
    }

    public Task<MachineInstance> GetMachineAsync(
      string instanceName,
      IEnumerable<string> propertyFilters = null)
    {
      List<KeyValuePair<string, string>> queryParameters = (List<KeyValuePair<string, string>>) null;
      if (propertyFilters != null)
        queryParameters = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("properties", string.Join(",", propertyFilters.ToArray<string>()))
        };
      return this.GetAsync<MachineInstance>(WebApiConstants.MachinesLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        instanceName = instanceName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters);
    }

    public Task<MachineConfiguration> GetMachineConfigurationAsync(string instanceName) => this.GetAsync<MachineConfiguration>(WebApiConstants.ConfigurationLocationId, (object) new
    {
      poolType = this.m_poolType,
      poolName = this.m_poolName,
      instanceName = instanceName
    });

    public Task<MachinePoolAndInstance> RegisterMachineAsync(
      string instanceName,
      string imageName,
      byte[] authorizationToken,
      bool provisioning = false)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>(nameof (imageName), imageName)
      };
      if (provisioning)
        keyValuePairList.Add("startProvisioning", "true");
      return this.PostAsync<byte[], MachinePoolAndInstance>(authorizationToken, WebApiConstants.AccessTokenLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        instanceName = instanceName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task<MachineInstance> UpdateMachineAsync(
      string instanceName,
      string state = null,
      bool? enabled = null,
      string imageName = null,
      PropertiesCollection properties = null,
      bool? provisioned = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(state))
        keyValuePairList.Add(nameof (state), state);
      if (enabled.HasValue)
        keyValuePairList.Add(nameof (enabled), enabled.ToString().ToLowerInvariant());
      if (!string.IsNullOrEmpty(imageName))
        keyValuePairList.Add(nameof (imageName), imageName);
      if (provisioned.HasValue)
        keyValuePairList.Add(nameof (provisioned), provisioned.ToString().ToLowerInvariant());
      return this.PatchAsync<PropertiesCollection, MachineInstance>(properties, WebApiConstants.MachinesLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        instanceName = instanceName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task<MachineInstanceMessage> GetMessageAsync(
      string instanceName,
      string queueName,
      string accessToken,
      long? lastMessageId = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(queueName))
        keyValuePairList.Add(nameof (queueName), queueName);
      if (!string.IsNullOrEmpty(accessToken))
        keyValuePairList.Add(nameof (accessToken), accessToken);
      if (lastMessageId.HasValue)
        keyValuePairList.Add("messageId", lastMessageId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.GetAsync<MachineInstanceMessage>(WebApiConstants.MessagesLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        instanceName = instanceName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task DeleteMessageAsync(
      string instanceName,
      string queueName,
      string accessToken,
      long messageId)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(queueName))
        keyValuePairList.Add(nameof (queueName), queueName);
      if (!string.IsNullOrEmpty(accessToken))
        keyValuePairList.Add(nameof (accessToken), accessToken);
      keyValuePairList.Add(nameof (messageId), messageId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return (Task) this.DeleteAsync(WebApiConstants.MessagesLocationId, (object) new
      {
        poolType = this.m_poolType,
        poolName = this.m_poolName,
        instanceName = instanceName
      }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }
  }
}
