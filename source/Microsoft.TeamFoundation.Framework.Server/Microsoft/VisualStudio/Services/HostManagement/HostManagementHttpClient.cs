// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.HostManagementHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  [ResourceArea("{2522D64E-35A6-402D-A714-16B9D16F5BB9}")]
  [ClientCancellationTimeout(20)]
  [ClientCircuitBreakerSettings(10, 50)]
  public class HostManagementHttpClient : VssHttpClientBase
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion;
    private static readonly ApiResourceVersion s_apiVersion5_0_preview_2;

    static HostManagementHttpClient()
    {
      HostManagementHttpClient.s_translatedExceptions.Add("InvalidAccessException", typeof (InvalidAccessException));
      HostManagementHttpClient.s_translatedExceptions.Add("ServiceHostDoesNotExistException", typeof (ServiceHostDoesNotExistException));
      HostManagementHttpClient.s_translatedExceptions.Add("HostManagementArgumentException", typeof (HostManagementArgumentException));
      HostManagementHttpClient.s_translatedExceptions.Add("InvalidRegionCodeException", typeof (InvalidRegionCodeException));
      HostManagementHttpClient.s_currentApiVersion = new ApiResourceVersion("2.0-preview");
      HostManagementHttpClient.s_apiVersion5_0_preview_2 = new ApiResourceVersion("5.0-preview.2");
    }

    public HostManagementHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public HostManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public HostManagementHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<ServiceHostProperties> GetServiceHostPropertiesAsync(
      Guid hostId,
      object userState = null)
    {
      HostManagementHttpClient managementHttpClient = this;
      try
      {
        return await managementHttpClient.GetAsync<ServiceHostProperties>(HostManagementResourceIds.ServiceHostsLocationId, (object) new
        {
          hostId = hostId
        }, HostManagementHttpClient.s_currentApiVersion, userState: userState).ConfigureAwait(false);
      }
      catch (ServiceHostDoesNotExistException ex)
      {
        return (ServiceHostProperties) null;
      }
    }

    public async Task<ServiceHostProperties> GetServiceHostPropertiesAsync(object userState = null)
    {
      HostManagementHttpClient managementHttpClient = this;
      try
      {
        return await managementHttpClient.GetAsync<ServiceHostProperties>(HostManagementResourceIds.ServiceHostsLocationId, version: HostManagementHttpClient.s_apiVersion5_0_preview_2, userState: userState).ConfigureAwait(false);
      }
      catch (ServiceHostDoesNotExistException ex)
      {
        return (ServiceHostProperties) null;
      }
    }

    public virtual async Task<Region> GetRegionAsync(string regionCode, object userState = null)
    {
      HostManagementHttpClient managementHttpClient = this;
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (regionCode), regionCode);
      return await managementHttpClient.GetAsync<Region>(HostManagementResourceIds.RegionsLocationId, (object) new
      {
      }, new ApiResourceVersion("5.0-preview.2"), (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState).ConfigureAwait(false);
    }

    public virtual async Task<Geography> GetGeographyAsync(string code, object userState = null)
    {
      HostManagementHttpClient managementHttpClient = this;
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (code), code);
      return await managementHttpClient.GetAsync<Geography>(HostManagementResourceIds.GeographiesLocationId, (object) new
      {
      }, new ApiResourceVersion(7.1, 1), (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState).ConfigureAwait(false);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) HostManagementHttpClient.s_translatedExceptions;
  }
}
