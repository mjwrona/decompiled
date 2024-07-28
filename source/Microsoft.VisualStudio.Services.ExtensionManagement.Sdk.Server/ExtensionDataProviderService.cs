// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionDataProviderService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionDataProviderService : IExtensionDataProviderService, IVssFrameworkService
  {
    private IDictionary<string, Type> m_dataProviderTypes;
    private Guid m_serviceInstanceType;
    private const string s_area = "ContributionService";
    private const string s_layer = "DataProviderService";
    private const string s_dataProviderRequestContextKey = "ExtensionDataProviders";
    private const string s_dataProviderPropertiesDataKey = "DataProviderQuery.Properties";
    private static readonly RegistryQuery s_remoteRequestTimeoutQuery = new RegistryQuery("/Configuration/ExtensionService/RemoteDataProviderTimeout");
    private const int s_defaultRemoteRequestTimeout = 3000;
    private const string s_resolveRemoteProvidersCommandGroupKey = "WebPlatform.DataProviders.ResolveRemoteProviders.{0}";
    private static readonly CommandPropertiesSetter s_commandPropertesForResolvingRemoteProviders = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(2.5)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10);
    private static readonly JsonSerializer s_securedSerializerFriendly = new ServerVssJsonMediaTypeFormatter(true).CreateJsonSerializer();
    private static readonly JsonSerializer s_serializerFriendly = new VssJsonMediaTypeFormatter(true).CreateJsonSerializer();
    private static readonly JsonSerializer s_securedSerializerRaw = new ServerVssJsonMediaTypeFormatter(true, true, true).CreateJsonSerializer();
    private static readonly JsonSerializer s_serializerRaw = new VssJsonMediaTypeFormatter(true, true, true).CreateJsonSerializer();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10013511, "ContributionService", "DataProviderService", nameof (ServiceStart));
      try
      {
        IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
        if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
          this.m_serviceInstanceType = vssRequestContext.ServiceInstanceType();
        this.m_dataProviderTypes = vssRequestContext.GetService<ExtensionDataProviderTypesService>().GetDataProviderTypes();
      }
      finally
      {
        systemRequestContext.TraceLeave(10013512, "ContributionService", "DataProviderService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public DataProviderResult GetDataProviderData(
      IVssRequestContext requestContext,
      DataProviderQuery query,
      bool remoteExecution = false,
      bool userFriendlySerialization = false,
      IDataProviderScope scope = null)
    {
      IEnumerable<Contribution> dataProviderContributions = requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) query.ContributionIds);
      return this.GetDataProviderData(requestContext, query.Context, dataProviderContributions, remoteExecution, userFriendlySerialization, (IDataProviderScope) null);
    }

    public DataProviderResult GetDataProviderData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      IEnumerable<Contribution> dataProviderContributions,
      bool remoteExecution = false,
      bool userFriendlySerialization = false,
      IDataProviderScope scope = null)
    {
      return this.GetDataProviderData(requestContext, providerContext, dataProviderContributions, scope, remoteExecution, !remoteExecution, userFriendlySerialization);
    }

    public DataProviderResult GetDataProviderData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      IEnumerable<Contribution> dataProviderContributions,
      IDataProviderScope scope,
      bool executeClientProviders = false,
      bool executeRemoteProviders = true,
      bool userFriendlySerialization = false)
    {
      Dictionary<string, ExtensionDataProviderService.DataProviderDetails> providers1 = new Dictionary<string, ExtensionDataProviderService.DataProviderDetails>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, ExtensionDataProviderService.DataProviderDetails> providers2 = new Dictionary<string, ExtensionDataProviderService.DataProviderDetails>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, DataProviderContext> dictionary1 = new Dictionary<string, DataProviderContext>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (providerContext == null)
        providerContext = new DataProviderContext();
      if (providerContext.SharedData == null)
        providerContext.SharedData = new DataProviderSharedData();
      string dataspaceId = requestContext.DataspaceIdentifier.ToString();
      DataProviderResult dataProviderData = new DataProviderResult(dataspaceId)
      {
        Data = new Dictionary<string, object>(),
        ResolvedProviders = new List<ResolvedDataProvider>(),
        SharedData = new Dictionary<string, object>(),
        ScopeName = scope?.Name,
        ScopeValue = scope?.Value
      };
      this.CheckPermission(requestContext, (ISecuredObject) dataProviderData);
      foreach (Contribution contribution in (IEnumerable<Contribution>) dataProviderContributions.OrderBy<Contribution, int>((Func<Contribution, int>) (c => c.GetProperty<int>("order"))))
      {
        requestContext.Trace(10013585, TraceLevel.Info, "ContributionService", "DataProviderService", "Loading data for provider: {0}", (object) contribution.Id);
        bool useSecureSerialization = this.UseSecureSerialization(contribution);
        JsonSerializer serializer = this.GetSerializer(useSecureSerialization, userFriendlySerialization);
        DataProviderContext providerContext1 = providerContext;
        bool flag1 = false;
        string propertyProviderName = string.Empty;
        ResolvedDataProvider resolvedDataProvider = new ResolvedDataProvider(dataspaceId)
        {
          Id = contribution.Id
        };
        if (executeRemoteProviders)
        {
          propertyProviderName = contribution.GetProperty<string>("propertyProvider", string.Empty);
          if (!string.IsNullOrEmpty(propertyProviderName))
          {
            if (!dictionary1.TryGetValue(propertyProviderName, out providerContext1))
            {
              IRemotePropertyProvider extension = requestContext.GetExtension<IRemotePropertyProvider>((Func<IRemotePropertyProvider, bool>) (filteredProvider => string.Equals(filteredProvider.Name, propertyProviderName, StringComparison.OrdinalIgnoreCase)));
              if (extension != null)
              {
                try
                {
                  Dictionary<string, DataProviderContext> dictionary2 = dictionary1;
                  string key = propertyProviderName;
                  DataProviderContext dataProviderContext1 = new DataProviderContext();
                  dataProviderContext1.Properties = extension.GetProviderProperties(requestContext);
                  dataProviderContext1.SharedData = providerContext.SharedData;
                  DataProviderContext dataProviderContext2 = dataProviderContext1;
                  dictionary2[key] = dataProviderContext1;
                  providerContext1 = dataProviderContext2;
                  if (providerContext.Properties != null)
                  {
                    if (providerContext1.Properties == null)
                    {
                      providerContext1.Properties = providerContext.Properties;
                    }
                    else
                    {
                      foreach (KeyValuePair<string, object> property in providerContext.Properties)
                        providerContext1.Properties[property.Key] = property.Value;
                    }
                  }
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(10013514, "ContributionService", "DataProviderService", ex);
                  resolvedDataProvider.Exception = ex;
                  resolvedDataProvider.Error = ex.Message;
                }
              }
              else
                resolvedDataProvider.Error = string.Format("Property provider '{0}' for contribution '{1}' does not exist.", (object) propertyProviderName, (object) contribution.Id);
            }
            if (providerContext1 == null && !string.IsNullOrEmpty(resolvedDataProvider.Error))
            {
              dataProviderData.ResolvedProviders.Add(resolvedDataProvider);
              continue;
            }
          }
        }
        Guid result1 = Guid.Empty;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          string property1 = contribution.GetProperty<string>("resourceAreaId");
          Guid result2;
          if (!string.IsNullOrEmpty(property1) && Guid.TryParse(property1, out result2))
          {
            ResourceArea resourceArea = requestContext.GetService<IResourceAreaService>().GetResourceArea(requestContext, result2);
            if (resourceArea != null)
              result1 = resourceArea.ParentService;
          }
          if (result1 == Guid.Empty)
          {
            string property2 = contribution.GetProperty<string>("serviceInstanceType");
            if (!string.IsNullOrEmpty(property2))
              Guid.TryParse(property2, out result1);
          }
        }
        if (result1 == Guid.Empty)
          result1 = this.m_serviceInstanceType;
        string property3 = contribution.GetProperty<string>("resolution", "Server");
        bool flag2;
        if ((((flag2 = string.Equals("ServerOnly", property3, StringComparison.OrdinalIgnoreCase)) ? 1 : ((flag1 = string.Equals("Server", property3)) ? 1 : 0)) | (executeClientProviders ? 1 : 0)) != 0)
        {
          if (flag2)
            stringSet.Add(contribution.Id);
          if (executeRemoteProviders && result1 != this.m_serviceInstanceType)
          {
            this.AddProviderDetails((IDictionary<string, ExtensionDataProviderService.DataProviderDetails>) providers1, contribution.Id, propertyProviderName, result1, providerContext1);
          }
          else
          {
            dataProviderData.ResolvedProviders.Add(resolvedDataProvider);
            string property4 = contribution.GetProperty<string>("name");
            if (string.IsNullOrEmpty(property4))
            {
              resolvedDataProvider.Error = string.Format("Data provider contribution '{0}' is missing property '{1}'.", (object) contribution.Id, (object) "name");
            }
            else
            {
              IExtensionDataProvider dataProvider = this.GetDataProvider(requestContext, property4);
              if (dataProvider != null)
              {
                using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "DataProvider"))
                {
                  performanceTimer.AddProperty("ProviderName", (object) property4);
                  performanceTimer.AddProperty("ContributionId", (object) contribution.Id);
                  try
                  {
                    object data = dataProvider.GetData(requestContext, providerContext1, contribution);
                    dataProviderData.Data[contribution.Id] = (object) ExtensionDataProviderService.GetJToken(requestContext, data, serializer, useSecureSerialization);
                  }
                  catch (Exception ex)
                  {
                    requestContext.TraceException(10013464, "ContributionService", "DataProviderService", ex);
                    resolvedDataProvider.Exception = ex;
                    resolvedDataProvider.Error = ex.Message;
                    if (executeRemoteProviders)
                    {
                      if (flag1)
                        this.AddProviderDetails((IDictionary<string, ExtensionDataProviderService.DataProviderDetails>) providers2, contribution.Id, propertyProviderName, result1, providerContext1);
                    }
                  }
                  resolvedDataProvider.Duration = (float) (performanceTimer.Duration / 10000L);
                }
              }
              else
                resolvedDataProvider.Error = string.Format("Data provider contribution '{0}' specifies data provider '{1}' which does not exist.", (object) contribution.Id, (object) property4);
            }
          }
        }
        else
          this.AddProviderDetails((IDictionary<string, ExtensionDataProviderService.DataProviderDetails>) providers2, contribution.Id, propertyProviderName, result1, providerContext1);
      }
      using (PerformanceTimer.StartMeasure(requestContext, "DataProvider.SharedData"))
      {
        foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) providerContext.SharedData)
        {
          try
          {
            dataProviderData.SharedData[keyValuePair.Key] = (object) ExtensionDataProviderService.GetJToken(requestContext, keyValuePair.Value, this.GetSerializer(true, userFriendlySerialization));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013464, "ContributionService", "DataProviderService", ex);
            if (dataProviderData.Exceptions == null)
              dataProviderData.Exceptions = new Dictionary<string, DataProviderExceptionDetails>();
            dataProviderData.Exceptions["sharedData." + keyValuePair.Key] = new DataProviderExceptionDetails(dataspaceId)
            {
              Message = ex.Message,
              ExceptionType = ex.GetType().FullName
            };
          }
        }
      }
      if (providers1.Count > 0)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        int remoteRequestTimeout = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in ExtensionDataProviderService.s_remoteRequestTimeoutQuery, 3000);
        JsonSerializer serializer = this.GetSerializer(true, userFriendlySerialization);
        foreach (KeyValuePair<string, ExtensionDataProviderService.DataProviderDetails> keyValuePair in providers1)
        {
          KeyValuePair<string, ExtensionDataProviderService.DataProviderDetails> remoteDataProvider = keyValuePair;
          using (PerformanceTimer performanceTimer = PerformanceTimer.StartMeasure(requestContext, "RemoteDataProviders"))
          {
            performanceTimer.AddProperty("ServiceInstanceType", (object) remoteDataProvider.Value.ServiceInstanceType);
            performanceTimer.AddProperty("ContributionIds", (object) remoteDataProvider.Value.ContributionIds);
            CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "OpenALM.").AndCommandKey((CommandKey) string.Format("WebPlatform.DataProviders.ResolveRemoteProviders.{0}", (object) remoteDataProvider.Key)).AndCommandPropertiesDefaults(ExtensionDataProviderService.s_commandPropertesForResolvingRemoteProviders);
            CommandService<DataProviderResult> commandService = new CommandService<DataProviderResult>(requestContext, setter, (Func<DataProviderResult>) (() => this.QueryRemoteDataProvider(requestContext, remoteDataProvider.Value.ServiceInstanceType, remoteDataProvider.Value.ProviderContext, (IEnumerable<string>) remoteDataProvider.Value.ContributionIds, remoteRequestTimeout, userFriendlySerialization, scope)), (Func<DataProviderResult>) (() => (DataProviderResult) null));
            try
            {
              DataProviderResult dataProviderResult = commandService.Execute();
              if (dataProviderResult != null)
              {
                foreach (ResolvedDataProvider resolvedProvider in dataProviderResult.ResolvedProviders)
                {
                  if (!string.IsNullOrEmpty(resolvedProvider.Error) && !stringSet.Contains(resolvedProvider.Id))
                    this.AddProviderDetails((IDictionary<string, ExtensionDataProviderService.DataProviderDetails>) providers2, resolvedProvider.Id, remoteDataProvider.Value.ProviderName, remoteDataProvider.Value.ServiceInstanceType, remoteDataProvider.Value.ProviderContext);
                }
                dataProviderData.ResolvedProviders.AddRange((IEnumerable<ResolvedDataProvider>) dataProviderResult.ResolvedProviders);
                ExtensionDataProviderService.MergeDataValues(dataProviderData.Data, dataProviderResult.Data, serializer);
                if (dataProviderResult.SharedData != null)
                  ExtensionDataProviderService.MergeDataValues(dataProviderData.SharedData, dataProviderResult.SharedData, serializer);
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(10013513, "ContributionService", "DataProviderService", ex);
              foreach (string contributionId in remoteDataProvider.Value.ContributionIds)
              {
                if (!stringSet.Contains(contributionId))
                  this.AddProviderDetails((IDictionary<string, ExtensionDataProviderService.DataProviderDetails>) providers2, contributionId, remoteDataProvider.Value.ProviderName, remoteDataProvider.Value.ServiceInstanceType, remoteDataProvider.Value.ProviderContext);
              }
            }
          }
        }
      }
      if (providers2.Count > 0)
      {
        dataProviderData.ClientProviders = new Dictionary<string, ClientDataProviderQuery>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, ExtensionDataProviderService.DataProviderDetails> keyValuePair in providers2)
        {
          Dictionary<string, ClientDataProviderQuery> clientProviders = dataProviderData.ClientProviders;
          string key = keyValuePair.Key;
          ClientDataProviderQuery dataProviderQuery = new ClientDataProviderQuery();
          dataProviderQuery.Context = keyValuePair.Value.ProviderContext;
          dataProviderQuery.ContributionIds = keyValuePair.Value.ContributionIds;
          dataProviderQuery.QueryServiceInstanceType = keyValuePair.Value.ServiceInstanceType;
          clientProviders.Add(key, dataProviderQuery);
        }
      }
      foreach (ResolvedDataProvider resolvedProvider in dataProviderData.ResolvedProviders)
      {
        if (!string.IsNullOrEmpty(resolvedProvider.Error))
        {
          if (dataProviderData.Exceptions == null)
            dataProviderData.Exceptions = new Dictionary<string, DataProviderExceptionDetails>();
          dataProviderData.Exceptions[resolvedProvider.Id] = new DataProviderExceptionDetails(dataspaceId)
          {
            Message = resolvedProvider.Error,
            ExceptionType = resolvedProvider.Exception != null ? resolvedProvider.Exception.GetType().Name : string.Empty
          };
        }
      }
      return dataProviderData;
    }

    private void CheckPermission(IVssRequestContext requestContext, ISecuredObject securedObject)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, securedObject.NamespaceId);
      if (securityNamespace == null)
        return;
      securityNamespace.CheckPermission(requestContext, securedObject.GetToken(), securedObject.RequiredPermissions);
    }

    private bool UseSecureSerialization(Contribution contribution)
    {
      if (contribution.RestrictedTo == null)
        return true;
      return ContributionRestriction.HasAnyClaim(contribution, "anonymous", "public");
    }

    private JsonSerializer GetSerializer(
      bool useSecureSerialization,
      bool userFriendlySerialization)
    {
      return useSecureSerialization ? (!userFriendlySerialization ? ExtensionDataProviderService.s_securedSerializerRaw : ExtensionDataProviderService.s_securedSerializerFriendly) : (!userFriendlySerialization ? ExtensionDataProviderService.s_serializerRaw : ExtensionDataProviderService.s_serializerFriendly);
    }

    private static JToken GetJToken(
      IVssRequestContext requestContext,
      object o,
      JsonSerializer serializer,
      bool useSecureSerialization = true)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "DataProvider.GetJToken"))
      {
        if (o == null)
          return (JToken) null;
        if (useSecureSerialization)
          ServerJsonSerializationHelper.EnsureValidRootType(o.GetType());
        return JToken.FromObject(o, serializer);
      }
    }

    private ExtensionDataProviderService.DataProviderDetails AddProviderDetails(
      IDictionary<string, ExtensionDataProviderService.DataProviderDetails> providers,
      string contributionId,
      string providerName,
      Guid instanceScopeGuid,
      DataProviderContext providerContext)
    {
      string key = string.Format("{0}.{1}", (object) providerName, (object) instanceScopeGuid);
      ExtensionDataProviderService.DataProviderDetails dataProviderDetails;
      if (!providers.TryGetValue(key, out dataProviderDetails))
      {
        dataProviderDetails = new ExtensionDataProviderService.DataProviderDetails()
        {
          ProviderName = providerName,
          ProviderContext = providerContext,
          ServiceInstanceType = instanceScopeGuid
        };
        providers.Add(key, dataProviderDetails);
      }
      dataProviderDetails.ContributionIds.Add(contributionId);
      return dataProviderDetails;
    }

    private IExtensionDataProvider GetDataProvider(
      IVssRequestContext requestContext,
      string dataProviderName)
    {
      Dictionary<string, IExtensionDataProvider> dictionary;
      if (!requestContext.Items.TryGetValue<Dictionary<string, IExtensionDataProvider>>("ExtensionDataProviders", out dictionary))
      {
        dictionary = new Dictionary<string, IExtensionDataProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["ExtensionDataProviders"] = (object) dictionary;
      }
      IExtensionDataProvider instance;
      Type type;
      if (!dictionary.TryGetValue(dataProviderName, out instance) && this.m_dataProviderTypes.TryGetValue(dataProviderName, out type))
      {
        instance = (IExtensionDataProvider) Activator.CreateInstance(type);
        if (instance != null)
          dictionary[dataProviderName] = instance;
      }
      return instance;
    }

    private DataProviderResult QueryRemoteDataProvider(
      IVssRequestContext requestContext,
      Guid serviceInstanceType,
      DataProviderContext providerContext,
      IEnumerable<string> contributionIds,
      int timeoutMs,
      bool userFriendlySerialization,
      IDataProviderScope scope)
    {
      ContributionsHttpClient contributionsHttpClient1;
      if (userFriendlySerialization)
      {
        contributionsHttpClient1 = requestContext.GetClient<ContributionsHttpClient>(serviceInstanceType);
      }
      else
      {
        ExtensionDataProviderService.ContributionsHttpClientWithRawSerialization client = requestContext.GetClient<ExtensionDataProviderService.ContributionsHttpClientWithRawSerialization>(serviceInstanceType);
        client.SetUseRegExForDateDeserialization(!requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.DonotUseRegExForCustomDateSerialization"));
        contributionsHttpClient1 = (ContributionsHttpClient) client;
      }
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(timeoutMs);
        ContributionsHttpClient contributionsHttpClient2 = contributionsHttpClient1;
        DataProviderQuery query = new DataProviderQuery();
        query.Context = providerContext;
        query.ContributionIds = contributionIds.ToList<string>();
        string name = scope?.Name;
        string scopeValue = scope?.Value;
        CancellationToken token = linkedTokenSource.Token;
        return contributionsHttpClient2.QueryDataProvidersAsync(query, name, scopeValue, cancellationToken: token).Result;
      }
    }

    private static void MergeDataValues(
      Dictionary<string, object> existingData,
      Dictionary<string, object> newData,
      JsonSerializer serializer)
    {
      foreach (string key in newData.Keys)
      {
        object obj1 = newData[key];
        object o;
        if (existingData.TryGetValue(key, out o) && o != null)
        {
          if (obj1 == null)
          {
            obj1 = o;
          }
          else
          {
            JToken jtoken = JToken.FromObject(o, serializer);
            if (jtoken is JArray && obj1 is IEnumerable)
            {
              JArray jarray = jtoken as JArray;
              foreach (object obj2 in (IEnumerable) obj1)
                jarray.Add(obj1);
              obj1 = (object) jarray;
            }
            else if (jtoken.Type == JTokenType.Object)
            {
              JObject jobject = jtoken as JObject;
              JObject content = JObject.FromObject(obj1, serializer);
              if (content.Type == JTokenType.Object)
              {
                jobject.Merge((object) content);
                obj1 = (object) jobject;
              }
            }
          }
        }
        existingData[key] = obj1;
      }
    }

    public T GetRequestDataProviderContextProperty<T>(
      IVssRequestContext requestContext,
      string propertyName)
    {
      T providerContextProperty = default (T);
      IDictionary<string, object> dictionary;
      object o;
      if (requestContext.Items.TryGetValue<IDictionary<string, object>>("DataProviderQuery.Properties", out dictionary) && dictionary.TryGetValue(propertyName, out o))
      {
        JsonSerializer jsonSerializer = new VssJsonMediaTypeFormatter().CreateJsonSerializer();
        providerContextProperty = JObject.FromObject(o, jsonSerializer).ToObject<T>();
      }
      return providerContextProperty;
    }

    public void SetRequestDataProviderContext(
      IVssRequestContext requestContext,
      IDictionary<string, object> providerContextProperties)
    {
      if (providerContextProperties == null)
        return;
      requestContext.Items["DataProviderQuery.Properties"] = (object) providerContextProperties;
    }

    private class DataProviderDetails
    {
      public List<string> ContributionIds = new List<string>();
      public DataProviderContext ProviderContext;
      public string ProviderName;
      public Guid ServiceInstanceType;
    }

    internal class ContributionsHttpClientWithRawSerialization : ContributionsHttpClient
    {
      private bool useRegExForDateDeserialization = true;

      public ContributionsHttpClientWithRawSerialization(Uri baseUrl, VssCredentials credentials)
        : base(baseUrl, credentials)
      {
      }

      public ContributionsHttpClientWithRawSerialization(
        Uri baseUrl,
        VssCredentials credentials,
        VssHttpRequestSettings settings)
        : base(baseUrl, credentials, settings)
      {
      }

      public ContributionsHttpClientWithRawSerialization(
        Uri baseUrl,
        VssCredentials credentials,
        params DelegatingHandler[] handlers)
        : base(baseUrl, credentials, handlers)
      {
      }

      public ContributionsHttpClientWithRawSerialization(
        Uri baseUrl,
        VssCredentials credentials,
        VssHttpRequestSettings settings,
        params DelegatingHandler[] handlers)
        : base(baseUrl, credentials, settings, handlers)
      {
      }

      public ContributionsHttpClientWithRawSerialization(
        Uri baseUrl,
        HttpMessageHandler pipeline,
        bool disposeHandler)
        : base(baseUrl, pipeline, disposeHandler)
      {
      }

      public void SetUseRegExForDateDeserialization(bool useRegEx) => this.useRegExForDateDeserialization = useRegEx;

      protected override MediaTypeWithQualityHeaderValue CreateAcceptHeader(
        ApiResourceVersion requestVersion,
        string mediaType)
      {
        MediaTypeWithQualityHeaderValue acceptHeader = base.CreateAcceptHeader(requestVersion, mediaType);
        acceptHeader.Parameters.Add(new NameValueHeaderValue("enumsAsNumbers", "true"));
        acceptHeader.Parameters.Add(new NameValueHeaderValue("msDateFormat", "true"));
        acceptHeader.Parameters.Add(new NameValueHeaderValue("noArrayWrap", "true"));
        return acceptHeader;
      }

      protected override async Task<T> ReadJsonContentAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default (CancellationToken))
      {
        string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return DataProviderSerializationUtil.DeserializeDataProviderResponse<T>(this.GetRequestContext(response), responseContent, this.useRegExForDateDeserialization);
      }

      private IVssRequestContext GetRequestContext(HttpResponseMessage response)
      {
        object obj;
        return response.RequestMessage.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj) ? obj as IVssRequestContext : (IVssRequestContext) null;
      }
    }
  }
}
