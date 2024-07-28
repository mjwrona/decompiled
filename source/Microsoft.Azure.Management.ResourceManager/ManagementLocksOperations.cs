// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ManagementLocksOperations
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.OData;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  internal class ManagementLocksOperations : 
    IServiceOperations<ManagementLockClient>,
    IManagementLocksOperations
  {
    internal ManagementLocksOperations(ManagementLockClient client) => this.Client = client != null ? client : throw new ArgumentNullException(nameof (client));

    public ManagementLockClient Client { get; private set; }

    public async Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if ((object) parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      parameters?.Validate();
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdateAtResourceGroupLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
      if (parameters != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) parameters, this.Client.SerializationSettings);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "DeleteAtResourceGroupLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
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

    public async Task<AzureOperationResponse<ManagementLockObject>> GetAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetAtResourceGroupLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (scope == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scope));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if ((object) parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      parameters?.Validate();
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (scope), (object) scope);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdateByScope", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "{scope}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{scope}", Uri.EscapeDataString(scope));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
      if (parameters != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) parameters, this.Client.SerializationSettings);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (scope == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scope));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (scope), (object) scope);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "DeleteByScope", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "{scope}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{scope}", Uri.EscapeDataString(scope));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
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

    public async Task<AzureOperationResponse<ManagementLockObject>> GetByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (scope == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (scope));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (scope), (object) scope);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetByScope", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "{scope}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{scope}", Uri.EscapeDataString(scope));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (resourceProviderNamespace == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceProviderNamespace));
      if (parentResourcePath == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parentResourcePath));
      if (resourceType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceType));
      if (resourceName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceName));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if ((object) parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      parameters?.Validate();
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (resourceProviderNamespace), (object) resourceProviderNamespace);
        tracingParameters.Add(nameof (parentResourcePath), (object) parentResourcePath);
        tracingParameters.Add(nameof (resourceType), (object) resourceType);
        tracingParameters.Add(nameof (resourceName), (object) resourceName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdateAtResourceLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{parentResourcePath}/{resourceType}/{resourceName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{resourceProviderNamespace}", Uri.EscapeDataString(resourceProviderNamespace));
      _url = _url.Replace("{parentResourcePath}", parentResourcePath);
      _url = _url.Replace("{resourceType}", resourceType);
      _url = _url.Replace("{resourceName}", Uri.EscapeDataString(resourceName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
      if (parameters != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) parameters, this.Client.SerializationSettings);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (resourceProviderNamespace == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceProviderNamespace));
      if (parentResourcePath == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parentResourcePath));
      if (resourceType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceType));
      if (resourceName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceName));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (resourceProviderNamespace), (object) resourceProviderNamespace);
        tracingParameters.Add(nameof (parentResourcePath), (object) parentResourcePath);
        tracingParameters.Add(nameof (resourceType), (object) resourceType);
        tracingParameters.Add(nameof (resourceName), (object) resourceName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "DeleteAtResourceLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{parentResourcePath}/{resourceType}/{resourceName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{resourceProviderNamespace}", Uri.EscapeDataString(resourceProviderNamespace));
      _url = _url.Replace("{parentResourcePath}", parentResourcePath);
      _url = _url.Replace("{resourceType}", resourceType);
      _url = _url.Replace("{resourceName}", Uri.EscapeDataString(resourceName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
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

    public async Task<AzureOperationResponse<ManagementLockObject>> GetAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (resourceProviderNamespace == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceProviderNamespace));
      if (parentResourcePath == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parentResourcePath));
      if (resourceType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceType));
      if (resourceName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceName));
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (resourceProviderNamespace), (object) resourceProviderNamespace);
        tracingParameters.Add(nameof (parentResourcePath), (object) parentResourcePath);
        tracingParameters.Add(nameof (resourceType), (object) resourceType);
        tracingParameters.Add(nameof (resourceName), (object) resourceName);
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetAtResourceLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{parentResourcePath}/{resourceType}/{resourceName}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{resourceProviderNamespace}", Uri.EscapeDataString(resourceProviderNamespace));
      _url = _url.Replace("{parentResourcePath}", parentResourcePath);
      _url = _url.Replace("{resourceType}", resourceType);
      _url = _url.Replace("{resourceName}", Uri.EscapeDataString(resourceName));
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if ((object) parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      parameters?.Validate();
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdateAtSubscriptionLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
      if (parameters != null)
      {
        _requestContent = SafeJsonConvert.SerializeObject((object) parameters, this.Client.SerializationSettings);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "DeleteAtSubscriptionLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
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

    public async Task<AzureOperationResponse<ManagementLockObject>> GetAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (lockName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (lockName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (lockName), (object) lockName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetAtSubscriptionLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/locks/{lockName}").ToString();
      _url = _url.Replace("{lockName}", Uri.EscapeDataString(lockName));
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<ManagementLockObject> _result = new AzureOperationResponse<ManagementLockObject>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ManagementLockObject>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (odataQuery), (object) odataQuery);
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtResourceGroupLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/locks").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      List<string> _queryParameters = new List<string>();
      if (odataQuery != null)
      {
        string _odataFilter = odataQuery.ToString();
        if (!string.IsNullOrEmpty(_odataFilter))
          _queryParameters.Add(_odataFilter);
        _odataFilter = (string) null;
      }
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
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
      if (resourceProviderNamespace == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceProviderNamespace));
      if (parentResourcePath == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parentResourcePath));
      if (resourceType == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceType));
      if (resourceName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (resourceName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (odataQuery), (object) odataQuery);
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (resourceProviderNamespace), (object) resourceProviderNamespace);
        tracingParameters.Add(nameof (parentResourcePath), (object) parentResourcePath);
        tracingParameters.Add(nameof (resourceType), (object) resourceType);
        tracingParameters.Add(nameof (resourceName), (object) resourceName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtResourceLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{parentResourcePath}/{resourceType}/{resourceName}/providers/Microsoft.Authorization/locks").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{resourceProviderNamespace}", Uri.EscapeDataString(resourceProviderNamespace));
      _url = _url.Replace("{parentResourcePath}", parentResourcePath);
      _url = _url.Replace("{resourceType}", resourceType);
      _url = _url.Replace("{resourceName}", Uri.EscapeDataString(resourceName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      List<string> _queryParameters = new List<string>();
      if (odataQuery != null)
      {
        string _odataFilter = odataQuery.ToString();
        if (!string.IsNullOrEmpty(_odataFilter))
          _queryParameters.Add(_odataFilter);
        _odataFilter = (string) null;
      }
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtSubscriptionLevelWithHttpMessagesAsync(
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (this.Client.SubscriptionId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.SubscriptionId");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (odataQuery), (object) odataQuery);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtSubscriptionLevel", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/locks").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      List<string> _queryParameters = new List<string>();
      if (odataQuery != null)
      {
        string _odataFilter = odataQuery.ToString();
        if (!string.IsNullOrEmpty(_odataFilter))
          _queryParameters.Add(_odataFilter);
        _odataFilter = (string) null;
      }
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceGroupLevelNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtResourceGroupLevelNext", (IDictionary<string, object>) tracingParameters);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceLevelNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtResourceLevelNext", (IDictionary<string, object>) tracingParameters);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtSubscriptionLevelNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListAtSubscriptionLevelNext", (IDictionary<string, object>) tracingParameters);
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
        CloudException ex = new CloudException(string.Format("Operation returned an invalid status code '{0}'", (object) _statusCode));
        try
        {
          _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
          CloudError _errorBody = SafeJsonConvert.DeserializeObject<CloudError>(_responseContent, this.Client.DeserializationSettings);
          if (_errorBody != null)
          {
            ex = new CloudException(_errorBody.Message);
            ex.Body = _errorBody;
          }
          _errorBody = (CloudError) null;
        }
        catch (JsonException ex1)
        {
        }
        ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
        ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
        if (_httpResponse.Headers.Contains("x-ms-request-id"))
          ex.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
        if (_shouldTrace)
          ServiceClientTracing.Error(_invocationId, (Exception) ex);
        _httpRequest.Dispose();
        _httpResponse?.Dispose();
        throw ex;
      }
      AzureOperationResponse<IPage<ManagementLockObject>> _result = new AzureOperationResponse<IPage<ManagementLockObject>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ManagementLockObject>) SafeJsonConvert.DeserializeObject<Page<ManagementLockObject>>(_responseContent, this.Client.DeserializationSettings);
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
