// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementHttpClient
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [ResourceArea("F987B69A-D314-468F-AAF8-6137C847A8E0")]
  public class MachineManagementHttpClient : MachineManagementHttpClientBase
  {
    private readonly ApiResourceVersion m_currentApiVersion = new ApiResourceVersion(1.0);

    public MachineManagementHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MachineManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MachineManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MachineManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MachineManagementHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task DeleteMessageAsync(
      string poolName,
      string instanceName,
      string queueName,
      byte[] accessToken,
      long messageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      string base64StringNoPadding = accessToken.ToBase64StringNoPadding();
      return this.DeleteMessageAsync(poolName, instanceName, queueName, base64StringNoPadding, messageId, userState, cancellationToken);
    }

    public Task<MachineInstanceMessage> GetMessageAsync(
      string poolName,
      string instanceName,
      string queueName,
      byte[] accessToken,
      long? lastMessageId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      string base64StringNoPadding = accessToken.ToBase64StringNoPadding();
      return this.GetMessageAsync(poolName, instanceName, queueName, base64StringNoPadding, lastMessageId, userState, cancellationToken);
    }

    public Task<(MachinePool, MachineInstance)> GetRegistrationInfoAsync(
      string poolName,
      string machineName,
      byte[] registrationToken,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      string base64StringNoPadding = registrationToken.ToBase64StringNoPadding();
      return this.GetRegistrationInfoAsync(poolName, machineName, base64StringNoPadding, userState, cancellationToken);
    }

    public override Task RequestActionAsync(
      string poolName,
      long requestId,
      RequestStateData state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("dc8c72f1-74e2-4788-98b0-26117efc38eb");
      object obj1 = (object) new
      {
        poolName = poolName,
        requestId = requestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<RequestStateData>(state, (MediaTypeFormatter) new VssJsonMediaTypeFormatter());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("3.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
