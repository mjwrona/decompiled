// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Client.LocationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Location.Client
{
  [ClientCircuitBreakerSettings(15, 80, MaxConcurrentRequests = 800)]
  [ClientCancellationTimeout(30)]
  public class LocationHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions;
    private const string connectSubUrl = "_apis/connectionData";
    protected static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(1.0);

    static LocationHttpClient()
    {
      LocationHttpClient.s_translatedExceptions = new Dictionary<string, Type>();
      LocationHttpClient.s_translatedExceptions.Add("ServiceDefinitionDoesNotExistException", typeof (ServiceDefinitionDoesNotExistException));
      LocationHttpClient.s_translatedExceptions.Add("InvalidAccessPointException", typeof (InvalidAccessPointException));
      LocationHttpClient.s_translatedExceptions.Add("InvalidServiceDefinitionException", typeof (InvalidServiceDefinitionException));
      LocationHttpClient.s_translatedExceptions.Add("ParentDefinitionNotFoundException", typeof (ParentDefinitionNotFoundException));
      LocationHttpClient.s_translatedExceptions.Add("CannotChangeParentDefinitionException", typeof (CannotChangeParentDefinitionException));
      LocationHttpClient.s_translatedExceptions.Add("ActionDeniedBySubscriberException", typeof (ActionDeniedBySubscriberException));
    }

    public LocationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public LocationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public LocationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public LocationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public LocationHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<ConnectionData> GetConnectionDataAsync(
      ConnectOptions connectOptions,
      long lastChangeId,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      LocationHttpClient locationHttpClient = this;
      ConnectionData connectionDataAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetConnectionData"))
      {
        // ISSUE: explicit non-virtual call
        // ISSUE: explicit non-virtual call
        connectionDataAsync = await locationHttpClient.SendAsync<ConnectionData>(new HttpRequestMessage(HttpMethod.Get, new UriBuilder(new Uri(PathUtility.Combine(__nonvirtual (locationHttpClient.BaseAddress).GetLeftPart(UriPartial.Path), "_apis/connectionData")))
        {
          Query = __nonvirtual (locationHttpClient.BaseAddress).Query
        }.Uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>(nameof (connectOptions), ((int) connectOptions).ToString((IFormatProvider) CultureInfo.InvariantCulture)),
          new KeyValuePair<string, string>(nameof (lastChangeId), ((int) lastChangeId).ToString((IFormatProvider) CultureInfo.InvariantCulture)),
          new KeyValuePair<string, string>("lastChangeId64", lastChangeId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
        }).ToString())
        {
          Headers = {
            Accept = {
              new MediaTypeWithQualityHeaderValue("application/json")
            }
          }
        }, userState, cancellationToken).ConfigureAwait(false);
      }
      return connectionDataAsync;
    }

    public virtual async Task UpdateServiceDefinitionsAsync(
      IEnumerable<ServiceDefinition> definitions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LocationHttpClient locationHttpClient = this;
      using (new VssHttpClientBase.OperationScope("Location", "UpdateServiceDefinitions"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) definitions, nameof (definitions));
        HttpContent content = (HttpContent) new ObjectContent<VssJsonCollectionWrapper<IEnumerable<ServiceDefinition>>>(new VssJsonCollectionWrapper<IEnumerable<ServiceDefinition>>((IEnumerable) definitions), locationHttpClient.Formatter);
        HttpResponseMessage httpResponseMessage = await locationHttpClient.SendAsync(new HttpMethod("PATCH"), LocationResourceIds.ServiceDefinitions, version: LocationHttpClient.s_currentApiVersion, content: content, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task<HttpResponseMessage> DeleteServiceDefinitionAsync(
      string serviceType,
      Guid identifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LocationHttpClient locationHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("Location", "DeleteServiceDefinitions"))
        httpResponseMessage = await locationHttpClient.SendAsync<HttpResponseMessage>(HttpMethod.Delete, LocationResourceIds.ServiceDefinitions, (object) new
        {
          serviceType = serviceType,
          identifier = identifier
        }, LocationHttpClient.s_currentApiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      return httpResponseMessage;
    }

    public virtual async Task<IEnumerable<ServiceDefinition>> GetServiceDefinitionsAsync()
    {
      LocationHttpClient locationHttpClient = this;
      IEnumerable<ServiceDefinition> definitionsAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetServiceDefinitions"))
        definitionsAsync = await locationHttpClient.SendAsync<IEnumerable<ServiceDefinition>>(HttpMethod.Get, LocationResourceIds.ServiceDefinitions, version: LocationHttpClient.s_currentApiVersion).ConfigureAwait(false);
      return definitionsAsync;
    }

    public virtual async Task<IEnumerable<ServiceDefinition>> GetServiceDefinitionsAsync(
      string serviceType)
    {
      LocationHttpClient locationHttpClient = this;
      IEnumerable<ServiceDefinition> definitionsAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetServiceDefinitions"))
        definitionsAsync = await locationHttpClient.SendAsync<IEnumerable<ServiceDefinition>>(HttpMethod.Get, LocationResourceIds.ServiceDefinitions, (object) new
        {
          serviceType = serviceType
        }, LocationHttpClient.s_currentApiVersion).ConfigureAwait(false);
      return definitionsAsync;
    }

    public virtual Task<ServiceDefinition> GetServiceDefinitionAsync(
      string serviceType,
      Guid identifier,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetServiceDefinitionAsync(serviceType, identifier, true, false, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<ServiceDefinition> GetServiceDefinitionAsync(
      string serviceType,
      Guid identifier,
      bool allowFaultIn,
      bool previewFaultIn,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LocationHttpClient locationHttpClient = this;
      ServiceDefinition serviceDefinitionAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetServiceDefinitions"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        if (!allowFaultIn)
          keyValuePairList.Add(nameof (allowFaultIn), bool.FalseString);
        if (previewFaultIn)
        {
          if (!allowFaultIn)
            throw new InvalidOperationException("Cannot preview a service definition fault in if we do not allow the fault in.");
          keyValuePairList.Add(nameof (previewFaultIn), bool.TrueString);
        }
        serviceDefinitionAsync = await locationHttpClient.SendAsync<ServiceDefinition>(HttpMethod.Get, LocationResourceIds.ServiceDefinitions, (object) new
        {
          serviceType = serviceType,
          identifier = identifier
        }, LocationHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return serviceDefinitionAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<HttpResponseMessage> FlushSpsServiceDefinitionAsync(
      Guid hostId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LocationHttpClient locationHttpClient = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("Location", "FlushSpsServiceDefinition"))
        httpResponseMessage = await locationHttpClient.SendAsync(HttpMethod.Put, LocationResourceIds.SpsServiceDefinition, (object) new
        {
          hostId = hostId
        }, LocationHttpClient.s_currentApiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      return httpResponseMessage;
    }

    public virtual async Task<IEnumerable<ResourceAreaInfo>> GetResourceAreasAsync()
    {
      LocationHttpClient locationHttpClient = this;
      IEnumerable<ResourceAreaInfo> resourceAreasAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetResourceAreas"))
        resourceAreasAsync = await locationHttpClient.SendAsync<IEnumerable<ResourceAreaInfo>>(HttpMethod.Get, LocationResourceIds.ResourceAreas, version: new ApiResourceVersion("3.2-preview.1")).ConfigureAwait(false);
      return resourceAreasAsync;
    }

    public virtual async Task<ResourceAreaInfo> GetResourceAreaAsync(Guid areaId)
    {
      LocationHttpClient locationHttpClient = this;
      ResourceAreaInfo resourceAreaAsync;
      using (new VssHttpClientBase.OperationScope("Location", "GetResourceAreas"))
        resourceAreaAsync = await locationHttpClient.SendAsync<ResourceAreaInfo>(HttpMethod.Get, LocationResourceIds.ResourceAreas, (object) new
        {
          areaId = areaId
        }, new ApiResourceVersion("3.2-preview.1")).ConfigureAwait(false);
      return resourceAreaAsync;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) LocationHttpClient.s_translatedExceptions;
  }
}
