// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ResourceGroupsOperations
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  internal class ResourceGroupsOperations : 
    IServiceOperations<ResourceManagementClient>,
    IResourceGroupsOperations
  {
    internal ResourceGroupsOperations(ResourceManagementClient client) => this.Client = client != null ? client : throw new ArgumentNullException(nameof (client));

    public ResourceManagementClient Client { get; private set; }

    public async Task<AzureOperationResponse<bool>> CheckExistenceWithHttpMessagesAsync(
      string resourceGroupName,
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
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CheckExistence", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("HEAD");
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
      _httpResponse = await this.Client.HttpClient.SendAsync(_httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
      if (_shouldTrace)
        ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
      HttpStatusCode _statusCode = _httpResponse.StatusCode;
      cancellationToken.ThrowIfCancellationRequested();
      string _responseContent = (string) null;
      if (_statusCode != HttpStatusCode.NoContent && _statusCode != HttpStatusCode.NotFound)
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
      AzureOperationResponse<bool> _result = new AzureOperationResponse<bool>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      _result.Body = _statusCode == HttpStatusCode.NoContent;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_shouldTrace)
        ServiceClientTracing.Exit(_invocationId, (object) _result);
      return _result;
    }

    public async Task<AzureOperationResponse<ResourceGroup>> CreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      ResourceGroup parameters,
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
      if (parameters == null)
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
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdate", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
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
      AzureOperationResponse<ResourceGroup> _result = new AzureOperationResponse<ResourceGroup>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ResourceGroup>(_responseContent, this.Client.DeserializationSettings);
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
          _result.Body = SafeJsonConvert.DeserializeObject<ResourceGroup>(_responseContent, this.Client.DeserializationSettings);
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
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ConfiguredTaskAwaitable<AzureOperationResponse> configuredTaskAwaitable = this.BeginDeleteWithHttpMessagesAsync(resourceGroupName, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse _response = await configuredTaskAwaitable;
      configuredTaskAwaitable = this.Client.GetPostOrDeleteOperationResultAsync(_response, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse operationResponse = await configuredTaskAwaitable;
      return operationResponse;
    }

    public async Task<AzureOperationResponse<ResourceGroup>> GetWithHttpMessagesAsync(
      string resourceGroupName,
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
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Get", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
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
      AzureOperationResponse<ResourceGroup> _result = new AzureOperationResponse<ResourceGroup>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ResourceGroup>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ResourceGroup>> UpdateWithHttpMessagesAsync(
      string resourceGroupName,
      ResourceGroupPatchable parameters,
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
      if (parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
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
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Update", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
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
      AzureOperationResponse<ResourceGroup> _result = new AzureOperationResponse<ResourceGroup>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ResourceGroup>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<ResourceGroupExportResult>> ExportTemplateWithHttpMessagesAsync(
      string resourceGroupName,
      ExportTemplateRequest parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ConfiguredTaskAwaitable<AzureOperationResponse<ResourceGroupExportResult>> configuredTaskAwaitable = this.BeginExportTemplateWithHttpMessagesAsync(resourceGroupName, parameters, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse<ResourceGroupExportResult> _response = await configuredTaskAwaitable;
      configuredTaskAwaitable = this.Client.GetPostOrDeleteOperationResultAsync<ResourceGroupExportResult>(_response, customHeaders, cancellationToken).ConfigureAwait(false);
      AzureOperationResponse<ResourceGroupExportResult> operationResponse = await configuredTaskAwaitable;
      return operationResponse;
    }

    public async Task<AzureOperationResponse<IPage<ResourceGroup>>> ListWithHttpMessagesAsync(
      ODataQuery<ResourceGroupFilter> odataQuery = null,
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "List", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups").ToString();
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
      AzureOperationResponse<IPage<ResourceGroup>> _result = new AzureOperationResponse<IPage<ResourceGroup>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ResourceGroup>) SafeJsonConvert.DeserializeObject<Page<ResourceGroup>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> BeginDeleteWithHttpMessagesAsync(
      string resourceGroupName,
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
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "BeginDelete", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}").ToString();
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
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
      if (_statusCode != HttpStatusCode.OK && _statusCode != HttpStatusCode.Accepted)
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

    public async Task<AzureOperationResponse<ResourceGroupExportResult>> BeginExportTemplateWithHttpMessagesAsync(
      string resourceGroupName,
      ExportTemplateRequest parameters,
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
      if (parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (resourceGroupName), (object) resourceGroupName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "BeginExportTemplate", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/exportTemplate").ToString();
      _url = _url.Replace("{subscriptionId}", Uri.EscapeDataString(this.Client.SubscriptionId));
      _url = _url.Replace("{resourceGroupName}", Uri.EscapeDataString(resourceGroupName));
      List<string> _queryParameters = new List<string>();
      if (this.Client.ApiVersion != null)
        _queryParameters.Add(string.Format("api-version={0}", (object) Uri.EscapeDataString(this.Client.ApiVersion)));
      if (_queryParameters.Count > 0)
        _url = _url + (_url.Contains("?") ? "&" : "?") + string.Join("&", (IEnumerable<string>) _queryParameters);
      HttpRequestMessage _httpRequest = new HttpRequestMessage();
      HttpResponseMessage _httpResponse = (HttpResponseMessage) null;
      _httpRequest.Method = new HttpMethod("POST");
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
      if (_statusCode != HttpStatusCode.OK && _statusCode != HttpStatusCode.Accepted)
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
      AzureOperationResponse<ResourceGroupExportResult> _result = new AzureOperationResponse<ResourceGroupExportResult>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<ResourceGroupExportResult>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<ResourceGroup>>> ListNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListNext", (IDictionary<string, object>) tracingParameters);
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
      AzureOperationResponse<IPage<ResourceGroup>> _result = new AzureOperationResponse<IPage<ResourceGroup>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<ResourceGroup>) SafeJsonConvert.DeserializeObject<Page<ResourceGroup>>(_responseContent, this.Client.DeserializationSettings);
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
