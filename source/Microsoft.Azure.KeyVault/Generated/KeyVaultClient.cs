// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.KeyVaultClient
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Azure.KeyVault.Customized.Authentication;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.KeyVault
{
  public class KeyVaultClient : 
    ServiceClient<KeyVaultClient>,
    IKeyVaultClient,
    IDisposable,
    IAzureClient
  {
    public KeyVaultClient(
      KeyVaultClient.AuthenticationCallback authenticationCallback,
      params DelegatingHandler[] handlers)
      : this((ServiceClientCredentials) new KeyVaultCredential(authenticationCallback), handlers)
    {
    }

    public KeyVaultClient(
      KeyVaultClient.AuthenticationCallback authenticationCallback,
      HttpClient httpClient)
      : this(new KeyVaultCredential(authenticationCallback), httpClient)
    {
    }

    public KeyVaultClient(KeyVaultCredential credential, HttpClient httpClient)
      : this((ServiceClientCredentials) credential.Clone())
    {
      this.HttpClient = httpClient;
    }

    public async Task<AzureOperationResponse<string>> GetPendingCertificateSigningRequestWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetPendingCertificateSigningRequest", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/pending").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + "?" + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      _httpRequest.Headers.Add("Accept", "application/pkcs10");
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<string> _result = new AzureOperationResponse<string>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        AzureOperationResponse<string> operationResponse = _result;
        operationResponse.Body = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        operationResponse = (AzureOperationResponse<string>) null;
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    protected override DelegatingHandler CreateHttpHandlerPipeline(
      HttpClientHandler httpClientHandler,
      params DelegatingHandler[] handlers)
    {
      ChallengeCacheHandler httpHandlerPipeline = new ChallengeCacheHandler();
      httpHandlerPipeline.InnerHandler = (HttpMessageHandler) base.CreateHttpHandlerPipeline(httpClientHandler, handlers);
      return (DelegatingHandler) httpHandlerPipeline;
    }

    internal string BaseUri { get; set; }

    public JsonSerializerSettings SerializationSettings { get; private set; }

    public JsonSerializerSettings DeserializationSettings { get; private set; }

    public ServiceClientCredentials Credentials { get; private set; }

    public string ApiVersion { get; private set; }

    public string AcceptLanguage { get; set; }

    public int? LongRunningOperationRetryTimeout { get; set; }

    public bool? GenerateClientRequestId { get; set; }

    protected KeyVaultClient(params DelegatingHandler[] handlers)
      : base(handlers)
    {
      this.Initialize();
    }

    protected KeyVaultClient(HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
      : base(rootHandler, handlers)
    {
      this.Initialize();
    }

    public KeyVaultClient(ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
      : this(handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<KeyVaultClient>((ServiceClient<KeyVaultClient>) this);
    }

    public KeyVaultClient(
      ServiceClientCredentials credentials,
      HttpClientHandler rootHandler,
      params DelegatingHandler[] handlers)
      : this(rootHandler, handlers)
    {
      this.Credentials = credentials != null ? credentials : throw new ArgumentNullException(nameof (credentials));
      if (this.Credentials == null)
        return;
      this.Credentials.InitializeServiceClient<KeyVaultClient>((ServiceClient<KeyVaultClient>) this);
    }

    private void Initialize()
    {
      this.BaseUri = "{vaultBaseUrl}";
      this.ApiVersion = "7.0";
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
      this.DeserializationSettings.Converters.Add((JsonConverter) new CloudErrorJsonConverter());
    }

    public async Task<AzureOperationResponse<KeyBundle>> CreateKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string kty,
      int? keySize = null,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      string curve = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyName != null && !Regex.IsMatch(keyName, "^[0-9a-zA-Z-]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (keyName), (object) "^[0-9a-zA-Z-]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (kty == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (kty));
      if (kty != null && kty.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (kty), (object) 1);
      KeyCreateParameters createParameters = new KeyCreateParameters();
      if (kty != null || keySize.HasValue || keyOps != null || keyAttributes != null || tags != null || curve != null)
      {
        createParameters.Kty = kty;
        createParameters.KeySize = keySize;
        createParameters.KeyOps = keyOps;
        createParameters.KeyAttributes = keyAttributes;
        createParameters.Tags = tags;
        createParameters.Curve = curve;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "CreateKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            "parameters",
            (object) createParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/create").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (createParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) createParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyBundle>> ImportKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      JsonWebKey key,
      bool? hsm = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyName != null && !Regex.IsMatch(keyName, "^[0-9a-zA-Z-]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (keyName), (object) "^[0-9a-zA-Z-]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (key == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (key));
      KeyImportParameters importParameters = new KeyImportParameters();
      if (hsm.HasValue || key != null || keyAttributes != null || tags != null)
      {
        importParameters.Hsm = hsm;
        importParameters.Key = key;
        importParameters.KeyAttributes = keyAttributes;
        importParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "ImportKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            "parameters",
            (object) importParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (importParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) importParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedKeyBundle>> DeleteKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedKeyBundle> _result = new AzureOperationResponse<DeletedKeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedKeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyBundle>> UpdateKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      IList<string> keyOps = null,
      KeyAttributes keyAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      KeyUpdateParameters updateParameters = new KeyUpdateParameters();
      if (keyOps != null || keyAttributes != null || tags != null)
      {
        updateParameters.KeyOps = keyOps;
        updateParameters.KeyAttributes = keyAttributes;
        updateParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyBundle>> GetKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<KeyItem>>> GetKeyVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetKeyVersions", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/versions").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<KeyItem>> _result = new AzureOperationResponse<IPage<KeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<KeyItem>) SafeJsonConvert.DeserializeObject<Page<KeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<KeyItem>>> GetKeysWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetKeys", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<KeyItem>> _result = new AzureOperationResponse<IPage<KeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<KeyItem>) SafeJsonConvert.DeserializeObject<Page<KeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<BackupKeyResult>> BackupKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "BackupKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/backup").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<BackupKeyResult> _result = new AzureOperationResponse<BackupKeyResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<BackupKeyResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyBundle>> RestoreKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] keyBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (keyBundleBackup == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyBundleBackup));
      KeyRestoreParameters restoreParameters = new KeyRestoreParameters();
      if (keyBundleBackup != null)
        restoreParameters.KeyBundleBackup = keyBundleBackup;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RestoreKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            "parameters",
            (object) restoreParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/restore").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (restoreParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) restoreParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyOperationResult>> EncryptWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      KeyOperationsParameters operationsParameters = new KeyOperationsParameters();
      if (algorithm != null || value != null)
      {
        operationsParameters.Algorithm = algorithm;
        operationsParameters.Value = value;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "Encrypt", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) operationsParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/encrypt").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (operationsParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) operationsParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyOperationResult> _result = new AzureOperationResponse<KeyOperationResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyOperationResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyOperationResult>> DecryptWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      KeyOperationsParameters operationsParameters = new KeyOperationsParameters();
      if (algorithm != null || value != null)
      {
        operationsParameters.Algorithm = algorithm;
        operationsParameters.Value = value;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "Decrypt", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) operationsParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/decrypt").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (operationsParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) operationsParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyOperationResult> _result = new AzureOperationResponse<KeyOperationResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyOperationResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyOperationResult>> SignWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      KeySignParameters keySignParameters = new KeySignParameters();
      if (algorithm != null || value != null)
      {
        keySignParameters.Algorithm = algorithm;
        keySignParameters.Value = value;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "Sign", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) keySignParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/sign").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (keySignParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) keySignParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyOperationResult> _result = new AzureOperationResponse<KeyOperationResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyOperationResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyVerifyResult>> VerifyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] digest,
      byte[] signature,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (digest == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (digest));
      if (signature == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (signature));
      KeyVerifyParameters verifyParameters = new KeyVerifyParameters();
      if (algorithm != null || digest != null || signature != null)
      {
        verifyParameters.Algorithm = algorithm;
        verifyParameters.Digest = digest;
        verifyParameters.Signature = signature;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "Verify", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) verifyParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/verify").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (verifyParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) verifyParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyVerifyResult> _result = new AzureOperationResponse<KeyVerifyResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyVerifyResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyOperationResult>> WrapKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      KeyOperationsParameters operationsParameters = new KeyOperationsParameters();
      if (algorithm != null || value != null)
      {
        operationsParameters.Algorithm = algorithm;
        operationsParameters.Value = value;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "WrapKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) operationsParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/wrapkey").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (operationsParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) operationsParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyOperationResult> _result = new AzureOperationResponse<KeyOperationResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyOperationResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<KeyOperationResult>> UnwrapKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      string keyVersion,
      string algorithm,
      byte[] value,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      if (keyVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (algorithm == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (algorithm));
      if (algorithm != null && algorithm.Length < 1)
        throw new ValidationException(ValidationRules.MinLength, nameof (algorithm), (object) 1);
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      KeyOperationsParameters operationsParameters = new KeyOperationsParameters();
      if (algorithm != null || value != null)
      {
        operationsParameters.Algorithm = algorithm;
        operationsParameters.Value = value;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UnwrapKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (keyVersion),
            (object) keyVersion
          },
          {
            "parameters",
            (object) operationsParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "keys/{key-name}/{key-version}/unwrapkey").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName)).Replace("{key-version}", Uri.EscapeDataString(keyVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (operationsParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) operationsParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyOperationResult> _result = new AzureOperationResponse<KeyOperationResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyOperationResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedKeyItem>>> GetDeletedKeysWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedKeys", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedkeys").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedKeyItem>> _result = new AzureOperationResponse<IPage<DeletedKeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedKeyItem>) SafeJsonConvert.DeserializeObject<Page<DeletedKeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedKeyBundle>> GetDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedkeys/{key-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedKeyBundle> _result = new AzureOperationResponse<DeletedKeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedKeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse> PurgeDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "PurgeDeletedKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedkeys/{key-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.NoContent)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse result = new AzureOperationResponse();
      result.Request = _httpRequest;
      result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) result);
      return result;
    }

    public async Task<AzureOperationResponse<KeyBundle>> RecoverDeletedKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RecoverDeletedKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (keyName),
            (object) keyName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedkeys/{key-name}/recover").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{key-name}", Uri.EscapeDataString(keyName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<KeyBundle> _result = new AzureOperationResponse<KeyBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<KeyBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SecretBundle>> SetSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string value,
      IDictionary<string, string> tags = null,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      if (secretName != null && !Regex.IsMatch(secretName, "^[0-9a-zA-Z-]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (secretName), (object) "^[0-9a-zA-Z-]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (value == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (value));
      SecretSetParameters secretSetParameters = new SecretSetParameters();
      if (value != null || tags != null || contentType != null || secretAttributes != null)
      {
        secretSetParameters.Value = value;
        secretSetParameters.Tags = tags;
        secretSetParameters.ContentType = contentType;
        secretSetParameters.SecretAttributes = secretAttributes;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "SetSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            "parameters",
            (object) secretSetParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (secretSetParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) secretSetParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SecretBundle> _result = new AzureOperationResponse<SecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedSecretBundle>> DeleteSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedSecretBundle> _result = new AzureOperationResponse<DeletedSecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedSecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SecretBundle>> UpdateSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      string contentType = null,
      SecretAttributes secretAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      if (secretVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      SecretUpdateParameters updateParameters = new SecretUpdateParameters();
      if (contentType != null || secretAttributes != null || tags != null)
      {
        updateParameters.ContentType = contentType;
        updateParameters.SecretAttributes = secretAttributes;
        updateParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (secretVersion),
            (object) secretVersion
          },
          {
            "parameters",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}/{secret-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName)).Replace("{secret-version}", Uri.EscapeDataString(secretVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SecretBundle> _result = new AzureOperationResponse<SecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SecretBundle>> GetSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      string secretVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      if (secretVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (secretVersion),
            (object) secretVersion
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}/{secret-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName)).Replace("{secret-version}", Uri.EscapeDataString(secretVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SecretBundle> _result = new AzureOperationResponse<SecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSecrets", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SecretItem>> _result = new AzureOperationResponse<IPage<SecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SecretItem>) SafeJsonConvert.DeserializeObject<Page<SecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSecretVersions", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}/versions").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SecretItem>> _result = new AzureOperationResponse<IPage<SecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SecretItem>) SafeJsonConvert.DeserializeObject<Page<SecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedSecretItem>>> GetDeletedSecretsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSecrets", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedsecrets").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedSecretItem>> _result = new AzureOperationResponse<IPage<DeletedSecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedSecretItem>) SafeJsonConvert.DeserializeObject<Page<DeletedSecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedSecretBundle>> GetDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedsecrets/{secret-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedSecretBundle> _result = new AzureOperationResponse<DeletedSecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedSecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse> PurgeDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "PurgeDeletedSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedsecrets/{secret-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.NoContent)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse result = new AzureOperationResponse();
      result.Request = _httpRequest;
      result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) result);
      return result;
    }

    public async Task<AzureOperationResponse<SecretBundle>> RecoverDeletedSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RecoverDeletedSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedsecrets/{secret-name}/recover").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SecretBundle> _result = new AzureOperationResponse<SecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<BackupSecretResult>> BackupSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      string secretName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (secretName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "BackupSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (secretName),
            (object) secretName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/{secret-name}/backup").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{secret-name}", Uri.EscapeDataString(secretName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<BackupSecretResult> _result = new AzureOperationResponse<BackupSecretResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<BackupSecretResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SecretBundle>> RestoreSecretWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] secretBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (secretBundleBackup == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (secretBundleBackup));
      SecretRestoreParameters restoreParameters = new SecretRestoreParameters();
      if (secretBundleBackup != null)
        restoreParameters.SecretBundleBackup = secretBundleBackup;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RestoreSecret", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            "parameters",
            (object) restoreParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "secrets/restore").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (restoreParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) restoreParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SecretBundle> _result = new AzureOperationResponse<SecretBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SecretBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificatesWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificates", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (includePending),
            (object) includePending
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      if (includePending.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("includePending={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) includePending, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateItem>> _result = new AzureOperationResponse<IPage<CertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateItem>) SafeJsonConvert.DeserializeObject<Page<CertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedCertificateBundle>> DeleteCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedCertificateBundle> _result = new AzureOperationResponse<DeletedCertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedCertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<Contacts>> SetCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Contacts contacts,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (contacts == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (contacts));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "SetCertificateContacts", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (contacts),
            (object) contacts
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/contacts").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (contacts != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) contacts, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<Contacts> _result = new AzureOperationResponse<Contacts>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<Contacts>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<Contacts>> GetCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateContacts", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/contacts").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<Contacts> _result = new AzureOperationResponse<Contacts>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<Contacts>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<Contacts>> DeleteCertificateContactsWithHttpMessagesAsync(
      string vaultBaseUrl,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteCertificateContacts", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/contacts").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<Contacts> _result = new AzureOperationResponse<Contacts>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<Contacts>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateIssuerItem>>> GetCertificateIssuersWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateIssuers", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/issuers").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateIssuerItem>> _result = new AzureOperationResponse<IPage<CertificateIssuerItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateIssuerItem>) SafeJsonConvert.DeserializeObject<Page<CertificateIssuerItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IssuerBundle>> SetCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      string provider,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (issuerName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (issuerName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (provider == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (provider));
      CertificateIssuerSetParameters issuerSetParameters = new CertificateIssuerSetParameters();
      if (provider != null || credentials != null || organizationDetails != null || attributes != null)
      {
        issuerSetParameters.Provider = provider;
        issuerSetParameters.Credentials = credentials;
        issuerSetParameters.OrganizationDetails = organizationDetails;
        issuerSetParameters.Attributes = attributes;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "SetCertificateIssuer", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (issuerName),
            (object) issuerName
          },
          {
            "parameter",
            (object) issuerSetParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/issuers/{issuer-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{issuer-name}", Uri.EscapeDataString(issuerName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (issuerSetParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) issuerSetParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IssuerBundle> _result = new AzureOperationResponse<IssuerBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<IssuerBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IssuerBundle>> UpdateCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      string provider = null,
      IssuerCredentials credentials = null,
      OrganizationDetails organizationDetails = null,
      IssuerAttributes attributes = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (issuerName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (issuerName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      CertificateIssuerUpdateParameters updateParameters = new CertificateIssuerUpdateParameters();
      if (provider != null || credentials != null || organizationDetails != null || attributes != null)
      {
        updateParameters.Provider = provider;
        updateParameters.Credentials = credentials;
        updateParameters.OrganizationDetails = organizationDetails;
        updateParameters.Attributes = attributes;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateCertificateIssuer", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (issuerName),
            (object) issuerName
          },
          {
            "parameter",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/issuers/{issuer-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{issuer-name}", Uri.EscapeDataString(issuerName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IssuerBundle> _result = new AzureOperationResponse<IssuerBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<IssuerBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IssuerBundle>> GetCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (issuerName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (issuerName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateIssuer", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (issuerName),
            (object) issuerName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/issuers/{issuer-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{issuer-name}", Uri.EscapeDataString(issuerName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IssuerBundle> _result = new AzureOperationResponse<IssuerBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<IssuerBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IssuerBundle>> DeleteCertificateIssuerWithHttpMessagesAsync(
      string vaultBaseUrl,
      string issuerName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (issuerName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (issuerName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteCertificateIssuer", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (issuerName),
            (object) issuerName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/issuers/{issuer-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{issuer-name}", Uri.EscapeDataString(issuerName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IssuerBundle> _result = new AzureOperationResponse<IssuerBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<IssuerBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateOperation>> CreateCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      if (certificateName != null && !Regex.IsMatch(certificateName, "^[0-9a-zA-Z-]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (certificateName), (object) "^[0-9a-zA-Z-]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      certificatePolicy?.Validate();
      CertificateCreateParameters createParameters = new CertificateCreateParameters();
      if (certificatePolicy != null || certificateAttributes != null || tags != null)
      {
        createParameters.CertificatePolicy = certificatePolicy;
        createParameters.CertificateAttributes = certificateAttributes;
        createParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "CreateCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            "parameters",
            (object) createParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/create").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (createParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) createParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.Accepted)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateOperation> _result = new AzureOperationResponse<CertificateOperation>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.Accepted)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateOperation>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> ImportCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string base64EncodedCertificate,
      string password = null,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      if (certificateName != null && !Regex.IsMatch(certificateName, "^[0-9a-zA-Z-]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (certificateName), (object) "^[0-9a-zA-Z-]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (base64EncodedCertificate == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (base64EncodedCertificate));
      certificatePolicy?.Validate();
      CertificateImportParameters importParameters = new CertificateImportParameters();
      if (base64EncodedCertificate != null || password != null || certificatePolicy != null || certificateAttributes != null || tags != null)
      {
        importParameters.Base64EncodedCertificate = base64EncodedCertificate;
        importParameters.Password = password;
        importParameters.CertificatePolicy = certificatePolicy;
        importParameters.CertificateAttributes = certificateAttributes;
        importParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "ImportCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            "parameters",
            (object) importParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/import").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (importParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) importParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificateVersionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateVersions", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/versions").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateItem>> _result = new AzureOperationResponse<IPage<CertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateItem>) SafeJsonConvert.DeserializeObject<Page<CertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificatePolicy>> GetCertificatePolicyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificatePolicy", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/policy").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificatePolicy> _result = new AzureOperationResponse<CertificatePolicy>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificatePolicy>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificatePolicy>> UpdateCertificatePolicyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      CertificatePolicy certificatePolicy,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      if (certificatePolicy == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificatePolicy));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateCertificatePolicy", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (certificatePolicy),
            (object) certificatePolicy
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/policy").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (certificatePolicy != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) certificatePolicy, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificatePolicy> _result = new AzureOperationResponse<CertificatePolicy>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificatePolicy>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> UpdateCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      CertificatePolicy certificatePolicy = null,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      if (certificateVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      certificatePolicy?.Validate();
      CertificateUpdateParameters updateParameters = new CertificateUpdateParameters();
      if (certificatePolicy != null || certificateAttributes != null || tags != null)
      {
        updateParameters.CertificatePolicy = certificatePolicy;
        updateParameters.CertificateAttributes = certificateAttributes;
        updateParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (certificateVersion),
            (object) certificateVersion
          },
          {
            "parameters",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/{certificate-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName)).Replace("{certificate-version}", Uri.EscapeDataString(certificateVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> GetCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      string certificateVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      if (certificateVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateVersion));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (certificateVersion),
            (object) certificateVersion
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/{certificate-version}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName)).Replace("{certificate-version}", Uri.EscapeDataString(certificateVersion));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateOperation>> UpdateCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      bool cancellationRequested,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      CertificateOperationUpdateParameter operationUpdateParameter = new CertificateOperationUpdateParameter();
      operationUpdateParameter.CancellationRequested = cancellationRequested;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateCertificateOperation", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            "certificateOperation",
            (object) operationUpdateParameter
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/pending").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (operationUpdateParameter != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) operationUpdateParameter, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateOperation> _result = new AzureOperationResponse<CertificateOperation>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateOperation>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateOperation>> GetCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateOperation", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/pending").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateOperation> _result = new AzureOperationResponse<CertificateOperation>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateOperation>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateOperation>> DeleteCertificateOperationWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteCertificateOperation", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/pending").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateOperation> _result = new AzureOperationResponse<CertificateOperation>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateOperation>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> MergeCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      IList<byte[]> x509Certificates,
      CertificateAttributes certificateAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (x509Certificates == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (x509Certificates));
      CertificateMergeParameters certificateMergeParameters = new CertificateMergeParameters();
      if (x509Certificates != null || certificateAttributes != null || tags != null)
      {
        certificateMergeParameters.X509Certificates = x509Certificates;
        certificateMergeParameters.CertificateAttributes = certificateAttributes;
        certificateMergeParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "MergeCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            "parameters",
            (object) certificateMergeParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/pending/merge").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (certificateMergeParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) certificateMergeParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.Created)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.Created)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<BackupCertificateResult>> BackupCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "BackupCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/{certificate-name}/backup").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<BackupCertificateResult> _result = new AzureOperationResponse<BackupCertificateResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<BackupCertificateResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> RestoreCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] certificateBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (certificateBundleBackup == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateBundleBackup));
      CertificateRestoreParameters restoreParameters = new CertificateRestoreParameters();
      if (certificateBundleBackup != null)
        restoreParameters.CertificateBundleBackup = certificateBundleBackup;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RestoreCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            "parameters",
            (object) restoreParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "certificates/restore").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (restoreParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) restoreParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedCertificateItem>>> GetDeletedCertificatesWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      bool? includePending = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedCertificates", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (includePending),
            (object) includePending
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedcertificates").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      if (includePending.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("includePending={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) includePending, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedCertificateItem>> _result = new AzureOperationResponse<IPage<DeletedCertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedCertificateItem>) SafeJsonConvert.DeserializeObject<Page<DeletedCertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedCertificateBundle>> GetDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedcertificates/{certificate-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedCertificateBundle> _result = new AzureOperationResponse<DeletedCertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedCertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse> PurgeDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "PurgeDeletedCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedcertificates/{certificate-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.NoContent)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse result = new AzureOperationResponse();
      result.Request = _httpRequest;
      result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) result);
      return result;
    }

    public async Task<AzureOperationResponse<CertificateBundle>> RecoverDeletedCertificateWithHttpMessagesAsync(
      string vaultBaseUrl,
      string certificateName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (certificateName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (certificateName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RecoverDeletedCertificate", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (certificateName),
            (object) certificateName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedcertificates/{certificate-name}/recover").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{certificate-name}", Uri.EscapeDataString(certificateName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<CertificateBundle> _result = new AzureOperationResponse<CertificateBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<CertificateBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<StorageAccountItem>>> GetStorageAccountsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetStorageAccounts", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<StorageAccountItem>> _result = new AzureOperationResponse<IPage<StorageAccountItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<StorageAccountItem>) SafeJsonConvert.DeserializeObject<Page<StorageAccountItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedStorageAccountItem>>> GetDeletedStorageAccountsWithHttpMessagesAsync(
      string vaultBaseUrl,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedStorageAccounts", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      bool? generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
      if (generateClientRequestId.HasValue)
      {
        // ISSUE: explicit non-virtual call
        generateClientRequestId = __nonvirtual (instance.GenerateClientRequestId);
        if (generateClientRequestId.Value)
          _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedStorageAccountItem>> _result = new AzureOperationResponse<IPage<DeletedStorageAccountItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedStorageAccountItem>) SafeJsonConvert.DeserializeObject<Page<DeletedStorageAccountItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedStorageBundle>> GetDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedStorageBundle> _result = new AzureOperationResponse<DeletedStorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedStorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse> PurgeDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "PurgeDeletedStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.NoContent)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse result = new AzureOperationResponse();
      result.Request = _httpRequest;
      result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) result);
      return result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> RecoverDeletedStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RecoverDeletedStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}/recover").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<BackupStorageResult>> BackupStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "BackupStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/backup").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<BackupStorageResult> _result = new AzureOperationResponse<BackupStorageResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<BackupStorageResult>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> RestoreStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      byte[] storageBundleBackup,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (storageBundleBackup == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageBundleBackup));
      StorageRestoreParameters restoreParameters = new StorageRestoreParameters();
      if (storageBundleBackup != null)
        restoreParameters.StorageBundleBackup = storageBundleBackup;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RestoreStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            "parameters",
            (object) restoreParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/restore").Replace("{vaultBaseUrl}", vaultBaseUrl);
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (restoreParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) restoreParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedStorageBundle>> DeleteStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedStorageBundle> _result = new AzureOperationResponse<DeletedStorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedStorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> GetStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> SetStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string resourceId,
      string activeKeyName,
      bool autoRegenerateKey,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (resourceId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceId));
      if (activeKeyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (activeKeyName));
      StorageAccountCreateParameters createParameters = new StorageAccountCreateParameters();
      if (resourceId != null || activeKeyName != null || regenerationPeriod != null || storageAccountAttributes != null || tags != null)
      {
        createParameters.ResourceId = resourceId;
        createParameters.ActiveKeyName = activeKeyName;
        createParameters.AutoRegenerateKey = autoRegenerateKey;
        createParameters.RegenerationPeriod = regenerationPeriod;
        createParameters.StorageAccountAttributes = storageAccountAttributes;
        createParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "SetStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            "parameters",
            (object) createParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (createParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) createParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> UpdateStorageAccountWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string activeKeyName = null,
      bool? autoRegenerateKey = null,
      string regenerationPeriod = null,
      StorageAccountAttributes storageAccountAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      StorageAccountUpdateParameters updateParameters = new StorageAccountUpdateParameters();
      if (activeKeyName != null || autoRegenerateKey.HasValue || regenerationPeriod != null || storageAccountAttributes != null || tags != null)
      {
        updateParameters.ActiveKeyName = activeKeyName;
        updateParameters.AutoRegenerateKey = autoRegenerateKey;
        updateParameters.RegenerationPeriod = regenerationPeriod;
        updateParameters.StorageAccountAttributes = storageAccountAttributes;
        updateParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateStorageAccount", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            "parameters",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<StorageBundle>> RegenerateStorageAccountKeyWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string keyName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (keyName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (keyName));
      StorageAccountRegenerteKeyParameters regenerteKeyParameters = new StorageAccountRegenerteKeyParameters();
      if (keyName != null)
        regenerteKeyParameters.KeyName = keyName;
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RegenerateStorageAccountKey", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            "parameters",
            (object) regenerteKeyParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/regeneratekey").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (regenerteKeyParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) regenerteKeyParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<StorageBundle> _result = new AzureOperationResponse<StorageBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<StorageBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SasDefinitionItem>>> GetSasDefinitionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSasDefinitions", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/sas").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SasDefinitionItem>> _result = new AzureOperationResponse<IPage<SasDefinitionItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SasDefinitionItem>) SafeJsonConvert.DeserializeObject<Page<SasDefinitionItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedSasDefinitionItem>>> GetDeletedSasDefinitionsWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      int? maxresults = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      int? nullable = maxresults;
      int num1 = 25;
      if (nullable.GetValueOrDefault() > num1 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, nameof (maxresults), (object) 25);
      nullable = maxresults;
      int num2 = 1;
      if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, nameof (maxresults), (object) 1);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSasDefinitions", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (maxresults),
            (object) maxresults
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}/sas").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName));
      List<string> values = new List<string>();
      if (maxresults.HasValue)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("maxresults={0}", (object) Uri.EscapeDataString(SafeJsonConvert.SerializeObject((object) maxresults, __nonvirtual (instance.SerializationSettings)).Trim('"'))));
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedSasDefinitionItem>> _result = new AzureOperationResponse<IPage<DeletedSasDefinitionItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedSasDefinitionItem>) SafeJsonConvert.DeserializeObject<Page<DeletedSasDefinitionItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedSasDefinitionBundle>> GetDeletedSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}/sas/{sas-definition-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedSasDefinitionBundle> _result = new AzureOperationResponse<DeletedSasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedSasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SasDefinitionBundle>> RecoverDeletedSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "RecoverDeletedSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "deletedstorage/{storage-account-name}/sas/{sas-definition-name}/recover").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SasDefinitionBundle> _result = new AzureOperationResponse<SasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<DeletedSasDefinitionBundle>> DeleteSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "DeleteSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/sas/{sas-definition-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<DeletedSasDefinitionBundle> _result = new AzureOperationResponse<DeletedSasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<DeletedSasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SasDefinitionBundle>> GetSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/sas/{sas-definition-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SasDefinitionBundle> _result = new AzureOperationResponse<SasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SasDefinitionBundle>> SetSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri,
      string sasType,
      string validityPeriod,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      if (templateUri == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (templateUri));
      if (sasType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasType));
      if (validityPeriod == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (validityPeriod));
      SasDefinitionCreateParameters createParameters = new SasDefinitionCreateParameters();
      if (templateUri != null || sasType != null || validityPeriod != null || sasDefinitionAttributes != null || tags != null)
      {
        createParameters.TemplateUri = templateUri;
        createParameters.SasType = sasType;
        createParameters.ValidityPeriod = validityPeriod;
        createParameters.SasDefinitionAttributes = sasDefinitionAttributes;
        createParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "SetSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            "parameters",
            (object) createParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/sas/{sas-definition-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (createParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) createParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SasDefinitionBundle> _result = new AzureOperationResponse<SasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<SasDefinitionBundle>> UpdateSasDefinitionWithHttpMessagesAsync(
      string vaultBaseUrl,
      string storageAccountName,
      string sasDefinitionName,
      string templateUri = null,
      string sasType = null,
      string validityPeriod = null,
      SasDefinitionAttributes sasDefinitionAttributes = null,
      IDictionary<string, string> tags = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (vaultBaseUrl == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (vaultBaseUrl));
      if (storageAccountName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (storageAccountName));
      if (storageAccountName != null && !Regex.IsMatch(storageAccountName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (storageAccountName), (object) "^[0-9a-zA-Z]+$");
      if (sasDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (sasDefinitionName));
      if (sasDefinitionName != null && !Regex.IsMatch(sasDefinitionName, "^[0-9a-zA-Z]+$"))
        throw new ValidationException(ValidationRules.Pattern, nameof (sasDefinitionName), (object) "^[0-9a-zA-Z]+$");
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.ApiVersion");
      SasDefinitionUpdateParameters updateParameters = new SasDefinitionUpdateParameters();
      if (templateUri != null || sasType != null || validityPeriod != null || sasDefinitionAttributes != null || tags != null)
      {
        updateParameters.TemplateUri = templateUri;
        updateParameters.SasType = sasType;
        updateParameters.ValidityPeriod = validityPeriod;
        updateParameters.SasDefinitionAttributes = sasDefinitionAttributes;
        updateParameters.Tags = tags;
      }
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "UpdateSasDefinition", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (vaultBaseUrl),
            (object) vaultBaseUrl
          },
          {
            nameof (storageAccountName),
            (object) storageAccountName
          },
          {
            nameof (sasDefinitionName),
            (object) sasDefinitionName
          },
          {
            "parameters",
            (object) updateParameters
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string baseUri = instance.BaseUri;
      string uriString = (baseUri + (baseUri.EndsWith("/") ? "" : "/") + "storage/{storage-account-name}/sas/{sas-definition-name}").Replace("{vaultBaseUrl}", vaultBaseUrl).Replace("{storage-account-name}", Uri.EscapeDataString(storageAccountName)).Replace("{sas-definition-name}", Uri.EscapeDataString(sasDefinitionName));
      List<string> values = new List<string>();
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.ApiVersion) != null)
      {
        // ISSUE: explicit non-virtual call
        values.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(__nonvirtual (instance.ApiVersion))));
      }
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      if (updateParameters != null)
      {
        // ISSUE: explicit non-virtual call
        _requestContent = SafeJsonConvert.SerializeObject((object) updateParameters, __nonvirtual (instance.SerializationSettings));
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<SasDefinitionBundle> _result = new AzureOperationResponse<SasDefinitionBundle>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = SafeJsonConvert.DeserializeObject<SasDefinitionBundle>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<KeyItem>>> GetKeyVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetKeyVersionsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<KeyItem>> _result = new AzureOperationResponse<IPage<KeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<KeyItem>) SafeJsonConvert.DeserializeObject<Page<KeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<KeyItem>>> GetKeysNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetKeysNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<KeyItem>> _result = new AzureOperationResponse<IPage<KeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<KeyItem>) SafeJsonConvert.DeserializeObject<Page<KeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedKeyItem>>> GetDeletedKeysNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedKeysNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedKeyItem>> _result = new AzureOperationResponse<IPage<DeletedKeyItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedKeyItem>) SafeJsonConvert.DeserializeObject<Page<DeletedKeyItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSecretsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SecretItem>> _result = new AzureOperationResponse<IPage<SecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SecretItem>) SafeJsonConvert.DeserializeObject<Page<SecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SecretItem>>> GetSecretVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSecretVersionsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SecretItem>> _result = new AzureOperationResponse<IPage<SecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SecretItem>) SafeJsonConvert.DeserializeObject<Page<SecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedSecretItem>>> GetDeletedSecretsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSecretsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedSecretItem>> _result = new AzureOperationResponse<IPage<DeletedSecretItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedSecretItem>) SafeJsonConvert.DeserializeObject<Page<DeletedSecretItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificatesNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificatesNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateItem>> _result = new AzureOperationResponse<IPage<CertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateItem>) SafeJsonConvert.DeserializeObject<Page<CertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateIssuerItem>>> GetCertificateIssuersNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateIssuersNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateIssuerItem>> _result = new AzureOperationResponse<IPage<CertificateIssuerItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateIssuerItem>) SafeJsonConvert.DeserializeObject<Page<CertificateIssuerItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<CertificateItem>>> GetCertificateVersionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetCertificateVersionsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<CertificateItem>> _result = new AzureOperationResponse<IPage<CertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<CertificateItem>) SafeJsonConvert.DeserializeObject<Page<CertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedCertificateItem>>> GetDeletedCertificatesNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedCertificatesNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedCertificateItem>> _result = new AzureOperationResponse<IPage<DeletedCertificateItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedCertificateItem>) SafeJsonConvert.DeserializeObject<Page<DeletedCertificateItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<StorageAccountItem>>> GetStorageAccountsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetStorageAccountsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<StorageAccountItem>> _result = new AzureOperationResponse<IPage<StorageAccountItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<StorageAccountItem>) SafeJsonConvert.DeserializeObject<Page<StorageAccountItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedStorageAccountItem>>> GetDeletedStorageAccountsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedStorageAccountsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedStorageAccountItem>> _result = new AzureOperationResponse<IPage<DeletedStorageAccountItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedStorageAccountItem>) SafeJsonConvert.DeserializeObject<Page<DeletedStorageAccountItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<SasDefinitionItem>>> GetSasDefinitionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetSasDefinitionsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<SasDefinitionItem>> _result = new AzureOperationResponse<IPage<SasDefinitionItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<SasDefinitionItem>) SafeJsonConvert.DeserializeObject<Page<SasDefinitionItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeletedSasDefinitionItem>>> GetDeletedSasDefinitionsNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      KeyVaultClient instance = this;
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        ServiceClientTracing.Enter(_invocationId, (object) instance, "GetDeletedSasDefinitionsNext", (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nextPageLink),
            (object) nextPageLink
          },
          {
            nameof (cancellationToken),
            (object) cancellationToken
          }
        });
      }
      string uriString = "{nextLink}".Replace("{nextLink}", nextPageLink);
      List<string> values = new List<string>();
      if (values.Count > 0)
        uriString = uriString + (uriString.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) values);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(uriString);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.GenerateClientRequestId).HasValue && __nonvirtual (instance.GenerateClientRequestId).Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.AcceptLanguage) != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        // ISSUE: explicit non-virtual call
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", __nonvirtual (instance.AcceptLanguage));
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          if (_httpRequest.Headers.Contains(customHeader.Key))
            _httpRequest.Headers.Remove(customHeader.Key);
          _httpRequest.Headers.TryAddWithoutValidation(customHeader.Key, (IEnumerable<string>) customHeader.Value);
        }
      }
      string _requestContent = (string) null;
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (instance.Credentials) != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        // ISSUE: explicit non-virtual call
        await __nonvirtual (instance.Credentials).ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await instance.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (statusCode != HttpStatusCode.OK)
      {
        KeyVaultErrorException ex = new KeyVaultErrorException(string.Format("Operation returned an invalid status code '{0}'", (object) statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          // ISSUE: explicit non-virtual call
          KeyVaultError keyVaultError = SafeJsonConvert.DeserializeObject<KeyVaultError>(_responseContent, __nonvirtual (instance.DeserializationSettings));
          if (keyVaultError != null)
            ex.Body = keyVaultError;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<DeletedSasDefinitionItem>> _result = new AzureOperationResponse<IPage<DeletedSasDefinitionItem>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          // ISSUE: explicit non-virtual call
          _result.Body = (IPage<DeletedSasDefinitionItem>) SafeJsonConvert.DeserializeObject<Page<DeletedSasDefinitionItem>>(_responseContent, __nonvirtual (instance.DeserializationSettings));
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    [SpecialName]
    HttpClient IAzureClient.get_HttpClient() => this.HttpClient;

    public delegate Task<string> AuthenticationCallback(
      string authority,
      string resource,
      string scope);
  }
}
