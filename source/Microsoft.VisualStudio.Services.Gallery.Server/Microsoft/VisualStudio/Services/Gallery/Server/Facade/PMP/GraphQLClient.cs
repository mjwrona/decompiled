// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.GraphQLClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  internal class GraphQLClient : IGraphQLClient
  {
    private readonly HttpClient _httpClient;
    private IPackageAggregateMutation _packageAggregateMutationBuilder;
    private IMsiAccessTokenProvider _managedIdentityAccessTokenProvider;
    private long _pmpRetryCount = 3;
    private long _pmpUploadRequestDelayTime = 2;

    public GraphQLClient(IVssRequestContext requestContext, HttpClient httpClient)
    {
      this._httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));
      if (this._packageAggregateMutationBuilder == null)
        this._packageAggregateMutationBuilder = (IPackageAggregateMutation) new PackageAggregateMutation();
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/**");
      this._pmpRetryCount = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMP/UploadService/Times/RetryCount", 3);
      this._pmpUploadRequestDelayTime = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMP/UploadService/Times/DelayTime", 2);
    }

    public GraphQLClient(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      IMsiAccessTokenProvider msiAccessTokenProvider)
    {
      this._httpClient = httpClient;
      this._managedIdentityAccessTokenProvider = msiAccessTokenProvider;
    }

    public async Task<HttpResponseMessage> CallMutationWithRetry(
      IVssRequestContext requestContext,
      string graphQLServiceUri,
      string mutationRequestString,
      Guid extensionId,
      string mutationName)
    {
      int currentRetryCount = 0;
      bool mutationSucceeded = false;
      while (!mutationSucceeded && (long) currentRetryCount < this._pmpRetryCount)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          HttpResponseMessage httpResponseMessage;
          using (httpResponseMessage = await this._httpClient.SendAsync(this.BuildGraphQLRequest(graphQLServiceUri, mutationRequestString, requestContext)).ConfigureAwait(false))
          {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
              mutationSucceeded = true;
              stopwatch.Stop();
              IVssRequestContext requestContext1 = requestContext;
              Guid extensionId1 = extensionId;
              int statusCode = (int) httpResponseMessage.StatusCode;
              string reasonPhrase = httpResponseMessage.ReasonPhrase;
              int retryCount = currentRetryCount;
              string str = mutationName;
              bool flag = mutationSucceeded;
              string timeSpanMS = stopwatch.ElapsedMilliseconds.ToString();
              int num = flag ? 1 : 0;
              string mutationName1 = str;
              GraphQLClient.PublishTelemetryForPMPService(requestContext1, "GraphQL", extensionId1, statusCode, reasonPhrase, retryCount, timeSpanMS, num != 0, mutationName1);
              return httpResponseMessage;
            }
            if (httpResponseMessage.StatusCode >= HttpStatusCode.InternalServerError)
            {
              stopwatch.Stop();
              ++currentRetryCount;
              requestContext.Trace(12062089, TraceLevel.Info, "gallery", nameof (CallMutationWithRetry), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to send request to GraphQL Service for extension {0} retry count {1} errorCode {2}", (object) extensionId, (object) currentRetryCount, (object) (int) httpResponseMessage.StatusCode));
              if ((long) currentRetryCount >= this._pmpRetryCount)
              {
                requestContext.TraceAlways(12062089, TraceLevel.Error, "gallery", nameof (CallMutationWithRetry), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to send request to GraphQL Service for extension {0} all retries exhaust errorCode {1}", (object) extensionId, (object) httpResponseMessage.StatusCode));
                IVssRequestContext requestContext2 = requestContext;
                Guid extensionId2 = extensionId;
                int statusCode = (int) httpResponseMessage.StatusCode;
                string reasonPhrase = httpResponseMessage.ReasonPhrase;
                int retryCount = currentRetryCount;
                string str = mutationName;
                bool flag = mutationSucceeded;
                string timeSpanMS = stopwatch.ElapsedMilliseconds.ToString();
                int num = flag ? 1 : 0;
                string mutationName2 = str;
                GraphQLClient.PublishTelemetryForPMPService(requestContext2, "GraphQL", extensionId2, statusCode, reasonPhrase, retryCount, timeSpanMS, num != 0, mutationName2);
                return httpResponseMessage;
              }
              await Task.Delay(TimeSpan.FromSeconds((double) this._pmpUploadRequestDelayTime));
            }
            else
            {
              stopwatch.Stop();
              IVssRequestContext requestContext3 = requestContext;
              Guid extensionId3 = extensionId;
              int statusCode = (int) httpResponseMessage.StatusCode;
              string reasonPhrase = httpResponseMessage.ReasonPhrase;
              int retryCount = currentRetryCount;
              string str = mutationName;
              bool flag = mutationSucceeded;
              string timeSpanMS = stopwatch.ElapsedMilliseconds.ToString();
              int num = flag ? 1 : 0;
              string mutationName3 = str;
              GraphQLClient.PublishTelemetryForPMPService(requestContext3, "GraphQL", extensionId3, statusCode, reasonPhrase, retryCount, timeSpanMS, num != 0, mutationName3);
              return httpResponseMessage;
            }
          }
        }
        catch (Exception ex)
        {
          stopwatch.Stop();
          IVssRequestContext requestContext4 = requestContext;
          Guid extensionId4 = extensionId;
          int retryCount = currentRetryCount;
          bool flag = mutationSucceeded;
          string timeSpanMS = stopwatch.ElapsedMilliseconds.ToString();
          int num = flag ? 1 : 0;
          Exception exception = ex;
          GraphQLClient.PublishTelemetryForPMPService(requestContext4, "GraphQL", extensionId4, 0, "Exception occurred while sending request", retryCount, timeSpanMS, num != 0, exception: exception);
          throw;
        }
      }
      return (HttpResponseMessage) null;
    }

    internal HttpRequestMessage BuildGraphQLRequest(
      string graphQLServiceUri,
      string content,
      IVssRequestContext requestContext)
    {
      JObject jobject = new JObject()
      {
        ["query"] = (JToken) content
      };
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
      {
        Method = HttpMethod.Post,
        RequestUri = new Uri(graphQLServiceUri),
        Content = (HttpContent) new StringContent(jobject.ToString(), Encoding.UTF8, "application/json")
      };
      httpRequestMessage.Headers.Add("X-PackageManagement-CorrelationId", string.Format("vscode{0}", (object) requestContext.ActivityId));
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableAuthTokenFetchForPMPWebApi"))
        this.AddManagedIdentityAccessTokenToHttpRequest(requestContext, httpRequestMessage);
      return httpRequestMessage;
    }

    private static void PublishTelemetryForPMPService(
      IVssRequestContext requestContext,
      string serviceName,
      Guid extensionId,
      int statusCode,
      string reason,
      int retryCount,
      string timeSpanMS,
      bool mutationSucceeded = false,
      string mutationName = "",
      Exception exception = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          nameof (serviceName),
          serviceName
        },
        {
          "ExtensionId",
          extensionId.ToString()
        },
        {
          "StatusCode",
          statusCode.ToString()
        },
        {
          "Reason",
          reason
        },
        {
          "RetryCount",
          retryCount.ToString()
        },
        {
          "isMutationSucceeded",
          mutationSucceeded.ToString()
        },
        {
          "MutationName",
          mutationName
        },
        {
          "TimeSpanMS",
          timeSpanMS
        }
      };
      if (exception != null)
        dictionary.Add("Exception", exception.Message);
      else
        dictionary.Add("Exception", string.Empty);
      string str = dictionary.Serialize<Dictionary<string, string>>();
      requestContext.TraceAlways(12062089, TraceLevel.Info, "gallery", nameof (PublishTelemetryForPMPService), "{0}", (object) str);
    }

    private void AddManagedIdentityAccessTokenToHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage httpRequestMessage)
    {
      if (this._managedIdentityAccessTokenProvider == null)
        this._managedIdentityAccessTokenProvider = (IMsiAccessTokenProvider) new MsiAccessTokenProvider(MsiTokenCache.SharedCache, (IAzureInstanceMetadataProvider) new AzureInstanceMetadataProvider(this._httpClient));
      string accessToken = this._managedIdentityAccessTokenProvider.GetAccessToken(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/WebApiResourceUri", false, (string) null));
      httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
  }
}
