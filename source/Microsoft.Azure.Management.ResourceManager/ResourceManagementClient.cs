// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ResourceManagementClient
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Management.ResourceManager
{
  public class ResourceManagementClient : 
    ServiceClient<ResourceManagementClient>,
    IResourceManagementClient,
    IDisposable,
    IAzureClient
  {
    public Uri BaseUri { get; set; }

    public JsonSerializerSettings SerializationSettings { get; private set; }

    public JsonSerializerSettings DeserializationSettings { get; private set; }

    public ServiceClientCredentials Credentials { get; private set; }

    public string SubscriptionId { get; set; }

    public string ApiVersion { get; private set; }

    public string AcceptLanguage { get; set; }

    public int? LongRunningOperationRetryTimeout { get; set; }

    public bool? GenerateClientRequestId { get; set; }

    public virtual IOperations Operations { get; private set; }

    public virtual IDeploymentsOperations Deployments { get; private set; }

    public virtual IProvidersOperations Providers { get; private set; }

    public virtual IResourcesOperations Resources { get; private set; }

    public virtual IResourceGroupsOperations ResourceGroups { get; private set; }

    public virtual ITagsOperations Tags { get; private set; }

    public virtual IDeploymentOperations DeploymentOperations { get; private set; }

    protected ResourceManagementClient(HttpClient httpClient, bool disposeHttpClient)
      : base(httpClient, disposeHttpClient)
    {
      this.Initialize();
    }

    protected ResourceManagementClient(params DelegatingHandler[] handlers)
      : base(handlers)
    {
      this.Initialize();
    }

    protected ResourceManagementClient(
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : base(rootHandler, handlers)
    {
      this.Initialize();
    }

    protected ResourceManagementClient(Uri baseUri, params DelegatingHandler[] handlers)
      : this(handlers)
    {
      this.BaseUri = !(baseUri == (Uri) null) ? baseUri : throw new ArgumentNullException(nameof (baseUri));
    }

    protected ResourceManagementClient(
      Uri baseUri,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      this.BaseUri = !(baseUri == (Uri) null) ? baseUri : throw new ArgumentNullException(nameof (baseUri));
    }

    public ResourceManagementClient(
      ServiceClientCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<ResourceManagementClient>((ServiceClient<ResourceManagementClient>) this);
    }

    public ResourceManagementClient(
      ServiceClientCredentials credentials,
      HttpClient httpClient,
      bool disposeHttpClient)
      : this(httpClient, disposeHttpClient)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<ResourceManagementClient>((ServiceClient<ResourceManagementClient>) this);
    }

    public ResourceManagementClient(
      ServiceClientCredentials credentials,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<ResourceManagementClient>((ServiceClient<ResourceManagementClient>) this);
    }

    public ResourceManagementClient(
      Uri baseUri,
      ServiceClientCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(handlers)
    {
      if (baseUri == (Uri) null)
        throw new ArgumentNullException(nameof (baseUri));
      if (credentials == null)
        throw new ArgumentNullException(nameof (credentials));
      this.BaseUri = baseUri;
      this.Credentials = credentials;
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<ResourceManagementClient>((ServiceClient<ResourceManagementClient>) this);
    }

    public ResourceManagementClient(
      Uri baseUri,
      ServiceClientCredentials credentials,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      if (baseUri == (Uri) null)
        throw new ArgumentNullException(nameof (baseUri));
      if (credentials == null)
        throw new ArgumentNullException(nameof (credentials));
      this.BaseUri = baseUri;
      this.Credentials = credentials;
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<ResourceManagementClient>((ServiceClient<ResourceManagementClient>) this);
    }

    private void Initialize()
    {
      this.Operations = (IOperations) new Microsoft.Azure.Management.ResourceManager.Operations(this);
      this.Deployments = (IDeploymentsOperations) new DeploymentsOperations(this);
      this.Providers = (IProvidersOperations) new ProvidersOperations(this);
      this.Resources = (IResourcesOperations) new ResourcesOperations(this);
      this.ResourceGroups = (IResourceGroupsOperations) new ResourceGroupsOperations(this);
      this.Tags = (ITagsOperations) new TagsOperations(this);
      this.DeploymentOperations = (IDeploymentOperations) new Microsoft.Azure.Management.ResourceManager.DeploymentOperations(this);
      this.BaseUri = new Uri("https://management.azure.com");
      this.ApiVersion = "2019-10-01";
      this.AcceptLanguage = "en-US";
      this.LongRunningOperationRetryTimeout = new int?(30);
      this.GenerateClientRequestId = new bool?(true);
      this.SerializationSettings = new JsonSerializerSettings()
      {
        Formatting = Formatting.Indented,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ContractResolver = (IContractResolver) new ReadOnlyJsonContractResolver(),
        Converters = (IList<JsonConverter>) new List<JsonConverter>()
        {
          (JsonConverter) new Iso8601TimeSpanConverter()
        }
      };
      this.SerializationSettings.Converters.Add((JsonConverter) new TransformationJsonConverter());
      this.DeserializationSettings = new JsonSerializerSettings()
      {
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ContractResolver = (IContractResolver) new ReadOnlyJsonContractResolver(),
        Converters = (IList<JsonConverter>) new List<JsonConverter>()
        {
          (JsonConverter) new Iso8601TimeSpanConverter()
        }
      };
      this.DeserializationSettings.Converters.Add((JsonConverter) new TransformationJsonConverter());
      this.DeserializationSettings.Converters.Add((JsonConverter) new CloudErrorJsonConverter());
    }

    [SpecialName]
    HttpClient IAzureClient.get_HttpClient() => this.HttpClient;
  }
}
