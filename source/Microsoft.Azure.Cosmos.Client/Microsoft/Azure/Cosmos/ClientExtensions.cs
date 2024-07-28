// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal static class ClientExtensions
  {
    public static async Task<HttpResponseMessage> GetAsync(
      this HttpClient client,
      Uri uri,
      INameValueCollection additionalHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      HttpResponseMessage async;
      using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
      {
        if (additionalHeaders != null)
        {
          foreach (string additionalHeader in (IEnumerable) additionalHeaders)
          {
            if (GatewayStoreClient.IsAllowedRequestHeader(additionalHeader))
              requestMessage.Headers.TryAddWithoutValidation(additionalHeader, additionalHeaders[additionalHeader]);
          }
        }
        async = await client.SendHttpAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
      }
      return async;
    }

    public static Task<DocumentServiceResponse> ParseResponseAsync(
      HttpResponseMessage responseMessage,
      JsonSerializerSettings serializerSettings = null,
      DocumentServiceRequest request = null)
    {
      return GatewayStoreClient.ParseResponseAsync(responseMessage, serializerSettings, request);
    }

    public static async Task<DocumentServiceResponse> ParseMediaResponseAsync(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      INameValueCollection headers = responseMessage.StatusCode < HttpStatusCode.BadRequest ? GatewayStoreClient.ExtractResponseHeaders(responseMessage) : throw await GatewayStoreClient.CreateDocumentClientExceptionAsync(responseMessage, (IClientSideRequestStatistics) null);
      HttpResponseMessage responseMessage1 = responseMessage;
      MediaStream body = new MediaStream(responseMessage1, await responseMessage.Content.ReadAsStreamAsync());
      responseMessage1 = (HttpResponseMessage) null;
      INameValueCollection headers1 = headers;
      int statusCode = (int) responseMessage.StatusCode;
      return new DocumentServiceResponse((Stream) body, headers1, (HttpStatusCode) statusCode);
    }
  }
}
