// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.DeploymentScriptsClient
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
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
  public class DeploymentScriptsClient : 
    ServiceClient<DeploymentScriptsClient>,
    IDeploymentScriptsClient,
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

    public virtual IDeploymentScriptsOperations DeploymentScripts { get; private set; }

    protected DeploymentScriptsClient(HttpClient httpClient, bool disposeHttpClient)
      : base(httpClient, disposeHttpClient)
    {
      this.Initialize();
    }

    protected DeploymentScriptsClient(params DelegatingHandler[] handlers)
      : base(handlers)
    {
      this.Initialize();
    }

    protected DeploymentScriptsClient(
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : base(rootHandler, handlers)
    {
      this.Initialize();
    }

    protected DeploymentScriptsClient(Uri baseUri, params DelegatingHandler[] handlers)
      : this(handlers)
    {
      this.BaseUri = !(baseUri == (Uri) null) ? baseUri : throw new ArgumentNullException(nameof (baseUri));
    }

    protected DeploymentScriptsClient(
      Uri baseUri,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      this.BaseUri = !(baseUri == (Uri) null) ? baseUri : throw new ArgumentNullException(nameof (baseUri));
    }

    public DeploymentScriptsClient(
      ServiceClientCredentials credentials,
      params DelegatingHandler[] handlers)
      : this(handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<DeploymentScriptsClient>((ServiceClient<DeploymentScriptsClient>) this);
    }

    public DeploymentScriptsClient(
      ServiceClientCredentials credentials,
      HttpClient httpClient,
      bool disposeHttpClient)
      : this(httpClient, disposeHttpClient)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<DeploymentScriptsClient>((ServiceClient<DeploymentScriptsClient>) this);
    }

    public DeploymentScriptsClient(
      ServiceClientCredentials credentials,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<DeploymentScriptsClient>((ServiceClient<DeploymentScriptsClient>) this);
    }

    public DeploymentScriptsClient(
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
      this.Credentials.InitializeServiceClient<DeploymentScriptsClient>((ServiceClient<DeploymentScriptsClient>) this);
    }

    public DeploymentScriptsClient(
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
      this.Credentials.InitializeServiceClient<DeploymentScriptsClient>((ServiceClient<DeploymentScriptsClient>) this);
    }

    private void Initialize()
    {
      this.DeploymentScripts = (IDeploymentScriptsOperations) new DeploymentScriptsOperations(this);
      this.BaseUri = new Uri("https://management.azure.com");
      this.ApiVersion = "2019-10-01-preview";
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
      this.SerializationSettings.Converters.Add((JsonConverter) new PolymorphicSerializeJsonConverter<DeploymentScript>("kind"));
      this.DeserializationSettings.Converters.Add((JsonConverter) new PolymorphicDeserializeJsonConverter<DeploymentScript>("kind"));
      this.DeserializationSettings.Converters.Add((JsonConverter) new TransformationJsonConverter());
      this.DeserializationSettings.Converters.Add((JsonConverter) new CloudErrorJsonConverter());
    }

    [SpecialName]
    HttpClient IAzureClient.get_HttpClient() => this.HttpClient;
  }
}
