// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.MachineManagement.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [ResourceArea("F987B69A-D314-468F-AAF8-6137C847A8E0")]
  public abstract class MachineManagementHttpClientBase : VssHttpClientBase
  {
    public MachineManagementHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MachineManagementHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MachineManagementHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MachineManagementHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MachineManagementHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<AgentSpecHealth> QueryAgentSpecHealthAsync(
      AgentSpecHealthRequest healthRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bff16571-7972-40c8-83ab-966e4a9b622e");
      HttpContent httpContent = (HttpContent) new ObjectContent<AgentSpecHealthRequest>(healthRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<AgentSpecHealth>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<MachineImageLabel> GetMachineImageLabelAsync(
      string imageLabel,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MachineImageLabel>(new HttpMethod("GET"), new Guid("33e5d203-cc38-478b-a0a0-aad8d7cc04ed"), (object) new
      {
        imageLabel = imageLabel
      }, new ApiResourceVersion(7.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<MachineImageLabel>> GetMachineImageLabelsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<MachineImageLabel>>(new HttpMethod("GET"), new Guid("33e5d203-cc38-478b-a0a0-aad8d7cc04ed"), version: new ApiResourceVersion(7.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<(MachinePool, MachineInstance)> GetRegistrationInfoAsync(
      string poolName,
      string instanceName,
      string registrationToken,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("80ed82a1-c9f1-41bf-95b6-8becc1a72b89");
      object routeValues = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (registrationToken), registrationToken);
      return this.SendAsync<(MachinePool, MachineInstance)>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<MachineRegistrationResponseData> RegisterMachineInstanceAsync(
      string poolName,
      string instanceName,
      string imageName,
      string imageVersion,
      byte[] poolAuthToken,
      bool? provisioning = null,
      bool? startProvisioning = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("80ed82a1-c9f1-41bf-95b6-8becc1a72b89");
      object obj1 = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<byte[]>(poolAuthToken, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection1 = new List<KeyValuePair<string, string>>();
      collection1.Add(nameof (imageName), imageName);
      collection1.Add(nameof (imageVersion), imageVersion);
      bool flag;
      if (provisioning.HasValue)
      {
        List<KeyValuePair<string, string>> collection2 = collection1;
        flag = provisioning.Value;
        string str = flag.ToString();
        collection2.Add(nameof (provisioning), str);
      }
      if (startProvisioning.HasValue)
      {
        List<KeyValuePair<string, string>> collection3 = collection1;
        flag = startProvisioning.Value;
        string str = flag.ToString();
        collection3.Add(nameof (startProvisioning), str);
      }
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection1;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<MachineRegistrationResponseData>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public virtual Task<MachineInstance> UpdateMachineInstanceAsync(
      string poolName,
      string instanceName,
      byte[] accessToken,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a67cc23a-6234-4f05-a75b-f08892603e86");
      object obj1 = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<byte[]>(accessToken, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<MachineInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteMessageAsync(
      string poolName,
      string instanceName,
      string queueName,
      string accessToken,
      long messageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("b53c80e3-c3eb-4224-9294-82e773974c5a");
      object routeValues = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (queueName), queueName);
      keyValuePairList.Add(nameof (accessToken), accessToken);
      keyValuePairList.Add(nameof (messageId), messageId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await managementHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<MachineInstanceMessage> GetMessageAsync(
      string poolName,
      string instanceName,
      string queueName,
      string accessToken,
      long? lastMessageId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b53c80e3-c3eb-4224-9294-82e773974c5a");
      object routeValues = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (queueName), queueName);
      keyValuePairList.Add(nameof (accessToken), accessToken);
      if (lastMessageId.HasValue)
        keyValuePairList.Add(nameof (lastMessageId), lastMessageId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<MachineInstanceMessage>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<Stream> GetMachineImageLabelMetadataAsync(
      string imageLabel,
      MachineImageLabelVersion? machineImageLabelVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d1373938-8207-4336-a609-f01b4230c8d7");
      object routeValues = (object) new
      {
        imageLabel = imageLabel
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (machineImageLabelVersion.HasValue)
        keyValuePairList.Add(nameof (machineImageLabelVersion), machineImageLabelVersion.Value.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await managementHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await managementHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task PublishMetricsAsync(
      string poolName,
      string instanceName,
      MachineMetric[] metrics,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d506bde2-6521-455e-beff-ddf6157d8b95");
      object obj1 = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MachineMetric[]>(metrics, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MachineManagementHttpClientBase managementHttpClientBase2 = managementHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await managementHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<HostReputation>> GetLowReputationsAsync(
      int maxScore,
      DateTime sinceDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<HostReputation>>(new HttpMethod("GET"), new Guid("b4a6cfca-5e88-41ef-911a-48e1da721873"), (object) new
      {
        maxScore = maxScore,
        sinceDate = sinceDate
      }, new ApiResourceVersion(7.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task RequestActionAsync(
      string poolName,
      long requestId,
      RequestStateData state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dc8c72f1-74e2-4788-98b0-26117efc38eb");
      object obj1 = (object) new
      {
        poolName = poolName,
        requestId = requestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RequestStateData>(state, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MachineManagementHttpClientBase managementHttpClientBase2 = managementHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await managementHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task FinishRequestAsync(
      long requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("POST"), new Guid("658f89f4-f19c-464c-85a4-7ea6669d78e5"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.1, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<MachineRequest> GetRequestAsync(
      long requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<MachineRequest>(new HttpMethod("GET"), new Guid("658f89f4-f19c-464c-85a4-7ea6669d78e5"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion(7.1, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<MachineRequest> QueueRequestAsync(
      MachineRequest machineRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("658f89f4-f19c-464c-85a4-7ea6669d78e5");
      HttpContent httpContent = (HttpContent) new ObjectContent<MachineRequest>(machineRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<MachineRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<MachineInstance> UnassignRequestAsync(
      long requestId,
      string poolName,
      string machineName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("658f89f4-f19c-464c-85a4-7ea6669d78e5");
      object routeValues = (object) new
      {
        requestId = requestId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (poolName), poolName);
      keyValuePairList.Add(nameof (machineName), machineName);
      return this.SendAsync<MachineInstance>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task AcknowledgeMessageAsync(
      Guid messageId,
      string state,
      int completionWaitTimeMinutes,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("a35df791-a007-4d83-a935-cf134703a0ad");
      object routeValues = (object) new
      {
        messageId = messageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (state), state);
      keyValuePairList.Add(nameof (completionWaitTimeMinutes), completionWaitTimeMinutes.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await managementHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task CompleteMessageAsync(
      Guid messageId,
      string completionResult,
      string errorMessage,
      string errorDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("a35df791-a007-4d83-a935-cf134703a0ad");
      object routeValues = (object) new
      {
        messageId = messageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (completionResult), completionResult);
      keyValuePairList.Add(nameof (errorMessage), errorMessage);
      keyValuePairList.Add(nameof (errorDetails), errorDetails);
      using (await managementHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<MachineRequestResource> GetResourceAsync(
      string requestType,
      string version,
      string platform = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37016efb-bdea-4ddf-9684-19098d11ebd4");
      object routeValues = (object) new
      {
        requestType = requestType,
        version = version
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (platform != null)
        keyValuePairList.Add(nameof (platform), platform);
      return this.SendAsync<MachineRequestResource>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<MachineRequestResource>> GetResourcesAsync(
      string requestType,
      string platform = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37016efb-bdea-4ddf-9684-19098d11ebd4");
      object routeValues = (object) new
      {
        requestType = requestType
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (platform != null)
        keyValuePairList.Add(nameof (platform), platform);
      return this.SendAsync<List<MachineRequestResource>>(method, locationId, routeValues, new ApiResourceVersion(7.1, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ReportSuspiciousActivityAsync(
      long requestId,
      SuspiciousActivityReport report,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9e9c24c2-67c0-4f2d-ad33-2063e3ae102c");
      object obj1 = (object) new{ requestId = requestId };
      HttpContent httpContent = (HttpContent) new ObjectContent<SuspiciousActivityReport>(report, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MachineManagementHttpClientBase managementHttpClientBase2 = managementHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await managementHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task PublishTracesAsync(
      string poolName,
      string instanceName,
      MmsProvisionerTrace[] traces,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineManagementHttpClientBase managementHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6dc94c76-5395-4e5c-be27-e69634668cd0");
      object obj1 = (object) new
      {
        poolName = poolName,
        instanceName = instanceName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<MmsProvisionerTrace[]>(traces, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MachineManagementHttpClientBase managementHttpClientBase2 = managementHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await managementHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
