// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.DeploymentScriptsOperations
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
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

namespace Microsoft.Azure.Management.ResourceManager
{
  internal class DeploymentScriptsOperations : 
    IServiceOperations<DeploymentScriptsClient>,
    IDeploymentScriptsOperations
  {
    internal DeploymentScriptsOperations(DeploymentScriptsClient client) => this.Client = client != null ? client : throw new ArgumentNullException(nameof (client));

    public DeploymentScriptsClient Client { get; private set; }

    public async Task<AzureOperationResponse<DeploymentScript>> CreateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ConfiguredTaskAwaitable<AzureOperationResponse<DeploymentScript>> configuredTaskAwaitable = this.BeginCreateWithHttpMessagesAsync(resourceGroupName, scriptName, deploymentScript, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse<DeploymentScript> _response = await configuredTaskAwaitable;
      configuredTaskAwaitable = this.Client.GetPutOrPatchOperationResultAsync<DeploymentScript>(_response, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse<DeploymentScript> httpMessagesAsync = await configuredTaskAwaitable;
      return httpMessagesAsync;
    }

    public async Task<AzureOperationResponse<DeploymentScript>> UpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScriptUpdateParameter deploymentScript = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (deploymentScript), (object) deploymentScript);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Update", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PATCH");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (deploymentScript != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) deploymentScript, this.Client.SerializationSettings);
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<DeploymentScript> _result = new AzureOperationResponse<DeploymentScript>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<DeploymentScript>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<DeploymentScript>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Get", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<DeploymentScript> _result = new AzureOperationResponse<DeploymentScript>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<DeploymentScript>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Delete", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("DELETE");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK && _statusCode != HttpStatusCode.NoContent)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse _result = new AzureOperationResponse();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<IPage<DeploymentScript>>> ListBySubscriptionWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListBySubscription", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Resources/deploymentScripts").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      bool? generateClientRequestId = this.Client.GenerateClientRequestId;
      int num;
      if (generateClientRequestId.HasValue)
      {
        generateClientRequestId = this.Client.GenerateClientRequestId;
        num = generateClientRequestId.Value ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<IPage<DeploymentScript>> _result = new AzureOperationResponse<IPage<DeploymentScript>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<DeploymentScript>) SafeJsonConvert.DeserializeObject<Page<DeploymentScript>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ScriptLogsList>> GetLogsWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetLogs", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}/logs").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<ScriptLogsList> _result = new AzureOperationResponse<ScriptLogsList>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ScriptLogsList>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ScriptLog>> GetLogsDefaultWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetLogsDefault", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}/logs/default").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<ScriptLog> _result = new AzureOperationResponse<ScriptLog>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ScriptLog>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<DeploymentScript>>> ListByResourceGroupWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListByResourceGroup", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<IPage<DeploymentScript>> _result = new AzureOperationResponse<IPage<DeploymentScript>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<DeploymentScript>) SafeJsonConvert.DeserializeObject<Page<DeploymentScript>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<DeploymentScript>> BeginCreateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      if (resourceGroupName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceGroupName));
      if (resourceGroupName != null)
      {
        if (resourceGroupName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (resourceGroupName), (object) 90);
        if (resourceGroupName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (resourceGroupName), (object) 1);
        if (!Regex.IsMatch(resourceGroupName, "^[-\\w\\._\\(\\)]+$"))
          throw new ValidationException(ValidationRules.Pattern, nameof (resourceGroupName), (object) "^[-\\w\\._\\(\\)]+$");
      }
      if (scriptName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scriptName));
      if (scriptName != null)
      {
        if (scriptName.Length > 90)
          throw new ValidationException(ValidationRules.MaxLength, nameof (scriptName), (object) 90);
        if (scriptName.Length < 1)
          throw new ValidationException(ValidationRules.MinLength, nameof (scriptName), (object) 1);
      }
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (deploymentScript == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (deploymentScript));
      deploymentScript?.Validate();
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (scriptName), (object) scriptName);
        tracingParameters.Add(nameof (deploymentScript), (object) deploymentScript);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "BeginCreate", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/Microsoft.Resources/deploymentScripts/{scriptName}").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{scriptName}", Uri.EscapeDataString(scriptName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("PUT");
      _httpRequest.RequestUri = new Uri(_url);
      if (this.Client.GenerateClientRequestId.HasValue && this.Client.GenerateClientRequestId.Value)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (deploymentScript != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) deploymentScript, this.Client.SerializationSettings);
        _httpRequest.Content = (HttpContent) new StringContent(_requestContent, Encoding.UTF8);
        _httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
      }
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK && _statusCode != HttpStatusCode.Created)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<DeploymentScript> _result = new AzureOperationResponse<DeploymentScript>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<DeploymentScript>(_responseContent, this.Client.DeserializationSettings);
        }
        catch (JsonException ex)
        {
          _httpRequest.Dispose();
          _httpResponse?.Dispose();
          throw new SerializationException("Unable to deserialize the response.", _responseContent, (Exception) ex);
        }
      }
      if (_statusCode == HttpStatusCode.Created)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<DeploymentScript>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<DeploymentScript>>> ListBySubscriptionNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (nextPageLink), (object) nextPageLink);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListBySubscriptionNext", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _url = "{nextLink}";
      _url = _url.Replace("{nextLink}", nextPageLink);
      List<string> _queryParameters = new List<string>();
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      bool? generateClientRequestId = this.Client.GenerateClientRequestId;
      int num;
      if (generateClientRequestId.HasValue)
      {
        generateClientRequestId = this.Client.GenerateClientRequestId;
        num = generateClientRequestId.Value ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<IPage<DeploymentScript>> _result = new AzureOperationResponse<IPage<DeploymentScript>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<DeploymentScript>) SafeJsonConvert.DeserializeObject<Page<DeploymentScript>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<DeploymentScript>>> ListByResourceGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (nextPageLink == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (nextPageLink));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (nextPageLink), (object) nextPageLink);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListByResourceGroupNext", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _url = "{nextLink}";
      _url = _url.Replace("{nextLink}", nextPageLink);
      List<string> _queryParameters = new List<string>();
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("GET");
      _httpRequest.RequestUri = new Uri(_url);
      bool? generateClientRequestId = this.Client.GenerateClientRequestId;
      int num;
      if (generateClientRequestId.HasValue)
      {
        generateClientRequestId = this.Client.GenerateClientRequestId;
        num = generateClientRequestId.Value ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", Guid.NewGuid().ToString());
      if (this.Client.AcceptLanguage != null)
      {
        if (_httpRequest.Headers.Contains("accept-language"))
          _httpRequest.Headers.Remove("accept-language");
        _httpRequest.Headers.TryAddWithoutValidation("accept-language", this.Client.AcceptLanguage);
      }
      if (customHeaders != null)
      {
        foreach (KeyValuePair<string, List<string>> customHeader in customHeaders)
        {
          KeyValuePair<string, List<string>> _header = customHeader;
          if (_httpRequest.Headers.Contains(_header.Key))
            _httpRequest.Headers.Remove(_header.Key);
          _httpRequest.Headers.TryAddWithoutValidation(_header.Key, (IEnumerable<string>) _header.Value);
          _header = new KeyValuePair<string, List<string>>();
        }
      }
      string _requestContent = (string) null;
      if (this.Client.Credentials != null)
      {
        cancellationToken.ThrowIfCancellationRequested();
        await this.Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      }
      if (_shouldTrace)
        ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
      cancellationToken.ThrowIfCancellationRequested();
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.OK)
      {
        DefaultErrorResponseException ex = new DefaultErrorResponseException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          DefaultErrorResponse _errorBody = SafeJsonConvert.DeserializeObject<DefaultErrorResponse>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
            ex.Body = _errorBody;
          _errorBody = (DefaultErrorResponse) null;
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
      AzureOperationResponse<IPage<DeploymentScript>> _result = new AzureOperationResponse<IPage<DeploymentScript>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<DeploymentScript>) SafeJsonConvert.DeserializeObject<Page<DeploymentScript>>(_responseContent, this.Client.DeserializationSettings);
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
  }
}
