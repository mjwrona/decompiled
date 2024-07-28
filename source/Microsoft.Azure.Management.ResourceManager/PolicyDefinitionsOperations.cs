// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.PolicyDefinitionsOperations
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  internal class PolicyDefinitionsOperations : 
    IServiceOperations<PolicyClient>,
    IPolicyDefinitionsOperations
  {
    internal PolicyDefinitionsOperations(PolicyClient client) => this.Client = client != null ? client : throw new ArgumentNullException(nameof (client));

    public PolicyClient Client { get; private set; }

    public async Task<AzureOperationResponse<PolicyDefinition>> CreateOrUpdateWithHttpMessagesAsync(
      string policyDefinitionName,
      PolicyDefinition parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
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
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdate", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
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
      if (_statusCode != HttpStatusCode.Created)
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
      AzureOperationResponse<PolicyDefinition> _result = new AzureOperationResponse<PolicyDefinition>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.Created)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<PolicyDefinition>(_responseContent, this.Client.DeserializationSettings);
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
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
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
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Delete", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
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

    public async Task<AzureOperationResponse<PolicyDefinition>> GetWithHttpMessagesAsync(
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
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
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "Get", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
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
      AzureOperationResponse<PolicyDefinition> _result = new AzureOperationResponse<PolicyDefinition>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<PolicyDefinition>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<PolicyDefinition>> GetBuiltInWithHttpMessagesAsync(
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetBuiltIn", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
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
      AzureOperationResponse<PolicyDefinition> _result = new AzureOperationResponse<PolicyDefinition>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<PolicyDefinition>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<PolicyDefinition>> CreateOrUpdateAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      PolicyDefinition parameters,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
      if (parameters == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (parameters));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (managementGroupId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (managementGroupId));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (parameters), (object) parameters);
        tracingParameters.Add(nameof (managementGroupId), (object) managementGroupId);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "CreateOrUpdateAtManagementGroup", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Management/managementgroups/{managementGroupId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
      _url = _url.Replace("{managementGroupId}", Uri.EscapeDataString(managementGroupId));
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
      if (_statusCode != HttpStatusCode.Created)
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
      AzureOperationResponse<PolicyDefinition> _result = new AzureOperationResponse<PolicyDefinition>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.Created)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<PolicyDefinition>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse> DeleteAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (managementGroupId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (managementGroupId));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (managementGroupId), (object) managementGroupId);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "DeleteAtManagementGroup", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Management/managementgroups/{managementGroupId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
      _url = _url.Replace("{managementGroupId}", Uri.EscapeDataString(managementGroupId));
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

    public async Task<AzureOperationResponse<PolicyDefinition>> GetAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (policyDefinitionName == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (policyDefinitionName));
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (managementGroupId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (managementGroupId));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (policyDefinitionName), (object) policyDefinitionName);
        tracingParameters.Add(nameof (managementGroupId), (object) managementGroupId);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "GetAtManagementGroup", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Management/managementgroups/{managementGroupId}/providers/Microsoft.Authorization/policyDefinitions/{policyDefinitionName}").ToString();
      _url = _url.Replace("{policyDefinitionName}", Uri.EscapeDataString(policyDefinitionName));
      _url = _url.Replace("{managementGroupId}", Uri.EscapeDataString(managementGroupId));
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
      AzureOperationResponse<PolicyDefinition> _result = new AzureOperationResponse<PolicyDefinition>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = SafeJsonConvert.DeserializeObject<PolicyDefinition>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListWithHttpMessagesAsync(
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
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "List", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "subscriptions/{subscriptionId}/providers/Microsoft.Authorization/policyDefinitions").ToString();
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListBuiltInWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListBuiltIn", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Authorization/policyDefinitions").ToString();
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListByManagementGroupWithHttpMessagesAsync(
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.Client.ApiVersion == null)
        throw new ValidationException(ValidationRules.CannotBeNull, "this.Client.ApiVersion");
      if (managementGroupId == null)
        throw new ValidationException(ValidationRules.CannotBeNull, nameof (managementGroupId));
      bool _shouldTrace = ServiceClientTracing.IsEnabled;
      string _invocationId = (string) null;
      if (_shouldTrace)
      {
        _invocationId = ServiceClientTracing.NextInvocationId.ToString();
        Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
        tracingParameters.Add(nameof (managementGroupId), (object) managementGroupId);
        tracingParameters.Add(nameof (cancellationToken), (object) cancellationToken);
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListByManagementGroup", (IDictionary<string, object>) tracingParameters);
        tracingParameters = (Dictionary<string, object>) null;
      }
      string _baseUrl = this.Client.BaseUri.AbsoluteUri;
      string _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "providers/Microsoft.Management/managementgroups/{managementGroupId}/providers/Microsoft.Authorization/policyDefinitions").ToString();
      _url = _url.Replace("{managementGroupId}", Uri.EscapeDataString(managementGroupId));
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListNextWithHttpMessagesAsync(
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListBuiltInNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListBuiltInNext", (IDictionary<string, object>) tracingParameters);
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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

    public async Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListByManagementGroupNextWithHttpMessagesAsync(
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
        ServiceClientTracing.Enter(_invocationId, (object) this, "ListByManagementGroupNext", (IDictionary<string, object>) tracingParameters);
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
      AzureOperationResponse<IPage<PolicyDefinition>> _result = new AzureOperationResponse<IPage<PolicyDefinition>>();
      _result.Request = _httpRequest;
      _result.Response = _httpResponse;
      if (_httpResponse.Headers.Contains("x-ms-request-id"))
        _result.RequestId = _httpResponse.Headers.GetValues("x-ms-request-id").FirstOrDefault<string>();
      if (_statusCode == HttpStatusCode.OK)
      {
        _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        try
        {
          _result.Body = (IPage<PolicyDefinition>) SafeJsonConvert.DeserializeObject<Page<PolicyDefinition>>(_responseContent, this.Client.DeserializationSettings);
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
