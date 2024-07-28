// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.GatewayServiceConfigurationReader
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Client
{
  internal sealed class GatewayServiceConfigurationReader : IServiceConfigurationReader
  {
    private ReplicationPolicy userReplicationPolicy;
    private ReplicationPolicy systemReplicationPolicy;
    private ConsistencyLevel consistencyLevel;
    private ReadPolicy readPolicy;
    private bool initialized;
    private Uri serviceEndpoint;
    private ApiType apiType;
    private readonly ConnectionPolicy connectionPolicy;
    private IDictionary<string, object> queryEngineConfiguration;
    private readonly IComputeHash authKeyHashFunction;
    private readonly bool hasAuthKeyResourceToken;
    private readonly string authKeyResourceToken = string.Empty;
    private readonly HttpMessageHandler messageHandler;

    public GatewayServiceConfigurationReader(
      Uri serviceEndpoint,
      IComputeHash stringHMACSHA256Helper,
      bool hasResourceToken,
      string resourceToken,
      ConnectionPolicy connectionPolicy,
      ApiType apiType,
      HttpMessageHandler messageHandler = null)
    {
      this.serviceEndpoint = serviceEndpoint;
      this.authKeyHashFunction = stringHMACSHA256Helper;
      this.hasAuthKeyResourceToken = hasResourceToken;
      this.authKeyResourceToken = resourceToken;
      this.connectionPolicy = connectionPolicy;
      this.messageHandler = messageHandler;
      this.apiType = apiType;
    }

    public string DatabaseAccountId => throw new NotImplementedException();

    public Uri DatabaseAccountApiEndpoint => throw new NotImplementedException();

    public ReplicationPolicy UserReplicationPolicy
    {
      get
      {
        this.ThrowIfNotInitialized();
        return this.userReplicationPolicy;
      }
    }

    public ReplicationPolicy SystemReplicationPolicy
    {
      get
      {
        this.ThrowIfNotInitialized();
        return this.systemReplicationPolicy;
      }
    }

    public ReadPolicy ReadPolicy
    {
      get
      {
        this.ThrowIfNotInitialized();
        return this.readPolicy;
      }
    }

    public string PrimaryMasterKey => throw new NotImplementedException();

    public string SecondaryMasterKey => throw new NotImplementedException();

    public string PrimaryReadonlyMasterKey => throw new NotImplementedException();

    public string SecondaryReadonlyMasterKey => throw new NotImplementedException();

    public string ResourceSeedKey => throw new NotImplementedException();

    public bool EnableAuthorization => true;

    public ConsistencyLevel DefaultConsistencyLevel
    {
      get
      {
        this.ThrowIfNotInitialized();
        return this.consistencyLevel;
      }
      set
      {
        this.ThrowIfNotInitialized();
        this.consistencyLevel = value;
      }
    }

    public string SubscriptionId => throw new NotImplementedException();

    public IDictionary<string, object> QueryEngineConfiguration
    {
      get
      {
        this.ThrowIfNotInitialized();
        return this.queryEngineConfiguration;
      }
    }

    private async Task<DatabaseAccount> GetDatabaseAccountAsync(Uri serviceEndpoint)
    {
      HttpClient httpClient = this.messageHandler == null ? new HttpClient() : new HttpClient(this.messageHandler);
      httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      httpClient.AddUserAgentHeader(this.connectionPolicy.UserAgentContainer);
      httpClient.AddApiTypeHeader(this.apiType);
      string empty = string.Empty;
      string str1;
      if (this.hasAuthKeyResourceToken)
      {
        str1 = HttpUtility.UrlEncode(this.authKeyResourceToken);
      }
      else
      {
        string str2 = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        httpClient.DefaultRequestHeaders.Add("x-ms-date", str2);
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        headers.Add("x-ms-date", str2);
        str1 = AuthorizationHelper.GenerateKeyAuthorizationSignature("GET", serviceEndpoint, headers, this.authKeyHashFunction);
      }
      httpClient.DefaultRequestHeaders.Add("authorization", str1);
      DatabaseAccount internalResource;
      using (HttpResponseMessage responseMessage = await httpClient.GetHttpAsync(serviceEndpoint))
      {
        using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(responseMessage))
          internalResource = responseAsync.GetInternalResource<DatabaseAccount>(new Func<DatabaseAccount>(DatabaseAccount.CreateNewInstance));
      }
      return internalResource;
    }

    public async Task InitializeAsync()
    {
      if (this.initialized)
        return;
      DatabaseAccount databaseAccount = await this.InitializeReaderAsync();
    }

    public async Task<DatabaseAccount> InitializeReaderAsync()
    {
      GatewayServiceConfigurationReader configurationReader = this;
      DatabaseAccount anyLocationsAsync = await GlobalEndpointManager.GetDatabaseAccountFromAnyLocationsAsync(configurationReader.serviceEndpoint, (IList<string>) configurationReader.connectionPolicy.PreferredLocations, new Func<Uri, Task<DatabaseAccount>>(configurationReader.GetDatabaseAccountAsync));
      configurationReader.userReplicationPolicy = anyLocationsAsync.ReplicationPolicy;
      configurationReader.systemReplicationPolicy = anyLocationsAsync.SystemReplicationPolicy;
      configurationReader.consistencyLevel = anyLocationsAsync.ConsistencyPolicy.DefaultConsistencyLevel;
      configurationReader.readPolicy = anyLocationsAsync.ReadPolicy;
      configurationReader.queryEngineConfiguration = anyLocationsAsync.QueryEngineConfiuration;
      configurationReader.initialized = true;
      return anyLocationsAsync;
    }

    private void ThrowIfNotInitialized()
    {
      if (!this.initialized)
        throw new InvalidProgramException();
    }

    public IList<Uri> DatabaseServices => throw new NotImplementedException();

    public IList<Uri> CollectionServices => throw new NotImplementedException();

    public IList<Uri> UserServices => throw new NotImplementedException();

    public IList<Uri> PermissionServices => throw new NotImplementedException();

    public IList<Uri> ServerServices => throw new NotImplementedException();
  }
}
