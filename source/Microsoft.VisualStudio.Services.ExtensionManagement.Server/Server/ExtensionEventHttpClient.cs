// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionEventHttpClient
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionEventHttpClient : VssHttpClientBase
  {
    public ExtensionEventHttpClient(Uri baseUrl, VssHttpRequestSettings settings)
      : base(baseUrl, new VssCredentials(), settings)
    {
    }

    public ExtensionEventHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<ExtensionEventCallbackResult> SendExtensionEventCallbackAsync(
      string installCallbackUrl,
      string applicationToken,
      ExtensionEventCallbackData data,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ExtensionEventHttpClient extensionEventHttpClient = this;
      ExtensionEventCallbackResult eventCallbackResult;
      using (HttpRequestMessage requestMessage = extensionEventHttpClient.CreateEventMessage(installCallbackUrl, data, applicationToken))
        eventCallbackResult = await extensionEventHttpClient.SendAsync<ExtensionEventCallbackResult>(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      return eventCallbackResult;
    }

    protected HttpRequestMessage CreateEventMessage(
      string url,
      ExtensionEventCallbackData data,
      string applicationToken)
    {
      HttpRequestMessage eventMessage = new HttpRequestMessage(HttpMethod.Post, url);
      if (!string.IsNullOrEmpty(applicationToken))
        eventMessage.Headers.Add("X-VSS-Extension-Token", applicationToken);
      eventMessage.Content = (HttpContent) new ObjectContent<ExtensionEventCallbackData>(data, (MediaTypeFormatter) new VssJsonMediaTypeFormatter());
      return eventMessage;
    }
  }
}
