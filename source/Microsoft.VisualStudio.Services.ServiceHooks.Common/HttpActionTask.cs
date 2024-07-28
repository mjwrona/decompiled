// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.HttpActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class HttpActionTask : ActionTask
  {
    private const string c_newLine = "\n";
    private static readonly string s_layer = typeof (HttpActionTask).Name;
    private static readonly string s_area = typeof (HttpActionTask).Namespace;
    private readonly byte[] m_httpRequestMessageContentBytes;
    private readonly string m_httpRequestStringRepresentation;
    private readonly bool m_acceptUntrustedCerts;

    public HttpActionTask(
      HttpRequestMessage httpRequestMessage,
      string httpRequestStringRepresentation,
      bool acceptUntrustedCerts = false)
    {
      ArgumentUtility.CheckForNull<HttpRequestMessage>(httpRequestMessage, nameof (httpRequestMessage));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(httpRequestStringRepresentation, nameof (httpRequestStringRepresentation));
      this.HttpRequestMessage = httpRequestMessage;
      this.m_httpRequestStringRepresentation = httpRequestStringRepresentation;
      this.m_acceptUntrustedCerts = acceptUntrustedCerts;
      if (httpRequestMessage.Content == null)
        return;
      Task<byte[]> task = httpRequestMessage.Content.ReadAsByteArrayAsync();
      task.Wait();
      this.m_httpRequestMessageContentBytes = task.Result;
    }

    public HttpRequestMessage HttpRequestMessage { get; set; }

    public override async Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      HttpActionTask httpActionTask = this;
      string asString = (string) null;
      HttpResponseMessage response = (HttpResponseMessage) null;
      Stopwatch sw = new Stopwatch();
      Exception exception;
      try
      {
        httpActionTask.UpdateNotificationForRequest(requestContext, httpActionTask.m_httpRequestStringRepresentation);
        requestContext.GetService<IUrlAddressIpValidatorService>().ApplyIPAddressAllowedRangeOnHttpRequest(requestContext, httpActionTask.HttpRequestMessage);
        HttpClientHandler clientHandler;
        using (clientHandler = httpActionTask.GetClientHandler(requestContext))
        {
          HttpClient httpClient1 = new HttpClient((HttpMessageHandler) clientHandler);
          httpClient1.Timeout = timeout;
          HttpClient httpClient2 = httpClient1;
          using (httpClient1)
          {
            sw.Start();
            try
            {
              response = await httpClient2.SendAsync(httpActionTask.HttpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
            }
            finally
            {
              sw.Stop();
            }
          }
        }
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
        {
          Method = httpActionTask.HttpRequestMessage.Method,
          RequestUri = httpActionTask.HttpRequestMessage.RequestUri,
          Version = httpActionTask.HttpRequestMessage.Version
        };
        if (httpActionTask.HttpRequestMessage.Content != null && httpActionTask.m_httpRequestMessageContentBytes != null)
          httpRequestMessage.Content = (HttpContent) new ByteArrayContent(httpActionTask.m_httpRequestMessageContentBytes);
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) httpActionTask.HttpRequestMessage.Headers)
          httpRequestMessage.Headers.Add(header.Key, header.Value);
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) httpActionTask.HttpRequestMessage.Properties)
          httpRequestMessage.Properties.Add(property.Key, property.Value);
        httpActionTask.HttpRequestMessage = httpRequestMessage;
        if (response.IsSuccessStatusCode)
        {
          asString = HttpActionTask.GetStringRepresentation(response);
          httpActionTask.UpdateNotificationForResponse(requestContext, asString, new double?(sw.Elapsed.TotalSeconds));
          return new ActionTaskResult(ActionTaskResultLevel.Success);
        }
        string errorMessage = string.Format("{0} ({1})", (object) response.ReasonPhrase, (object) (int) response.StatusCode);
        asString = HttpActionTask.GetStringRepresentation(response);
        httpActionTask.UpdateNotificationForResponse(requestContext, asString, new double?(sw.Elapsed.TotalSeconds), errorMessage);
        if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.BadGateway || response.StatusCode == HttpStatusCode.ServiceUnavailable || response.StatusCode == HttpStatusCode.GatewayTimeout)
          return new ActionTaskResult(ActionTaskResultLevel.TransientError, errorMessage: errorMessage);
        if (response.StatusCode == HttpStatusCode.Gone)
          return new ActionTaskResult(ActionTaskResultLevel.TerminalFailure, errorMessage: errorMessage);
        if (response.StatusCode == HttpStatusCode.MovedPermanently || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.SeeOther || response.StatusCode == HttpStatusCode.TemporaryRedirect)
          requestContext.TraceAlways(1051927, TraceLevel.Info, HttpActionTask.s_area, HttpActionTask.s_layer, string.Format("HTTP Action returned response code {0}", (object) response.StatusCode));
        return new ActionTaskResult(ActionTaskResultLevel.EnduringFailure, errorMessage: errorMessage);
      }
      catch (TaskCanceledException ex)
      {
        return new ActionTaskResult(ActionTaskResultLevel.TransientError, (Exception) ex, CommonResources.TaskCanceledTimeout);
      }
      catch (HttpRequestException ex)
      {
        exception = (Exception) ex;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (ie => ie is TaskCanceledException)) is TaskCanceledException canceledException)
          return new ActionTaskResult(ActionTaskResultLevel.TransientError, (Exception) canceledException, canceledException.Message);
        exception = (Exception) ex;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (response == null)
      {
        httpActionTask.UpdateNotificationForResponse(requestContext, httpActionTask.BuildExceptionMessageList(requestContext, exception), new double?(sw.Elapsed.TotalSeconds), exception.Message, exception.ToString());
      }
      else
      {
        if (asString == null)
          asString = HttpActionTask.GetStringRepresentation(response);
        httpActionTask.UpdateNotificationForResponse(requestContext, asString, new double?(sw.Elapsed.TotalSeconds), exception.Message, exception.ToString());
      }
      return new ActionTaskResult(ActionTaskResultLevel.EnduringFailure, exception, exception.Message);
    }

    protected virtual HttpClientHandler GetClientHandler(IVssRequestContext requestContext)
    {
      HttpClientHandler clientHandler = new HttpClientHandler()
      {
        AllowAutoRedirect = !requestContext.IsFeatureEnabled("ServiceHooks.Notification.DisableRedirects")
      };
      if (this.m_acceptUntrustedCerts)
        clientHandler.ServerCertificateCustomValidationCallback = (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>) ((requestMessage, certificate, chain, sslPolicyErrors) => true);
      return clientHandler;
    }

    private string BuildExceptionMessageList(IVssRequestContext requestContext, Exception ex)
    {
      int num = 5;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < num && ex != null; ++index)
      {
        stringBuilder.AppendFormat("    {0}{1}", (object) ex.Message, (object) Environment.NewLine);
        ex = ex.InnerException;
      }
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        stringBuilder.AppendLine();
        stringBuilder.Append(ServiceHooksWebApiResources.Response_OnPremFirewall());
      }
      return ServiceHooksWebApiResources.Response_ErrorNoResponse((object) stringBuilder.ToString());
    }

    private static string GetStringRepresentation(HttpResponseMessage response)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_StatusCodeTemplate((object) (int) response.StatusCode, (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_ReasonPhraseTemplate((object) response.ReasonPhrase, (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HttpVersionTemplate((object) response.Version, (object) "\n"));
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeadersStartTemplate((object) "\n"));
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Headers)
        HttpActionTask.AppendHeaderValues(sb, header);
      if (response.Content != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Content.Headers)
          HttpActionTask.AppendHeaderValues(sb, header);
      }
      sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeadersEndTemplate((object) "\n"));
      return sb.ToString();
    }

    private static void AppendHeaderValues(
      StringBuilder sb,
      KeyValuePair<string, IEnumerable<string>> header)
    {
      foreach (string str in header.Value)
        sb.Append(ServiceHooksWebApiResources.HttpActionTask_HeaderKeyValueTemplate((object) header.Key, (object) str, (object) "\n"));
    }
  }
}
