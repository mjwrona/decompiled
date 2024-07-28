// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AfdValidationHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;

namespace Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor
{
  public static class AfdValidationHelper
  {
    private static readonly HashSet<HttpStatusCode> s_retryableStatusCodes = new HashSet<HttpStatusCode>()
    {
      HttpStatusCode.BadRequest,
      HttpStatusCode.Unauthorized
    };
    private const int c_maxRetryCount = 3;

    public static void ValidateConfiguration(AfdConfigData configuration, ITFLogger logger) => AfdValidationHelper.TryAllRoutes((IEnumerable<string>) configuration.Hosts, (IEnumerable<string>) configuration.Paths, logger, configuration.UseHttpsRedirectRoutes ? new bool?(false) : new bool?(), new bool?());

    public static void ValidateProduction(ITFLogger logger)
    {
      AfdValidationHelper.TryAllRoutes((IEnumerable<string>) new string[5]
      {
        "vsmps.visualstudio.com",
        "*.vsmps.visualstudio.com",
        "vssps.visualstudio.com",
        "*.vssps.visualstudio.com",
        "*.visualstudio.com"
      }, (IEnumerable<string>) new string[2]{ "/", "/*" }, logger, new bool?(false), new bool?(true));
      AfdValidationHelper.TestRequestRouting("https://app.vsmps.visualstudio.com/_apis/health");
      AfdValidationHelper.TestRequestRouting("https://app.vssps.visualstudio.com/_apis/health");
      AfdValidationHelper.TestRequestRouting("https://tfsprodsbr1.visualstudio.com/_apis/health");
      AfdValidationHelper.TestRequestRouting("https://tfsprodwcus0.visualstudio.com/_apis/health");
      AfdValidationHelper.TestRequestRouting("https://mseng.visualstudio.com");
      AfdValidationHelper.TestRequestRouting("https://AfdTestCollection.visualstudio.com");
      AfdValidationHelper.TestRequestRouting("https://mseng.vssps.visualstudio.com/_apis/connectionData");
      AfdValidationHelper.TestRequestRouting("https://AfdTestCollection.vssps.visualstudio.com/_apis/connectionData");
    }

    private static void TryAllRoutes(
      IEnumerable<string> hosts,
      IEnumerable<string> paths,
      ITFLogger logger,
      bool? expectProcessIdHeaderForHttp,
      bool? expectProcessIdHeaderForHttps)
    {
      foreach (string host1 in hosts)
      {
        string host2 = host1.Replace("*", "testdomain");
        foreach (string path1 in paths)
        {
          string path2 = path1.Replace("*", "testendpoint");
          logger.Info("Trying route " + host2 + " - " + path2 + " over HTTP.");
          AfdValidationHelper.TryRoute(host2, path2, false, expectProcessIdHeaderForHttp, logger);
          logger.Info("Trying route " + host2 + " - " + path2 + " over HTTPS.");
          AfdValidationHelper.TryRoute(host2, path2, true, expectProcessIdHeaderForHttps, logger);
        }
      }
    }

    private static void TryRoute(
      string host,
      string path,
      bool secured,
      bool? expectProcessIdHeader,
      ITFLogger logger)
    {
      Uri path1 = new Uri(new Uri(secured ? "https://" + host : "http://" + host, UriKind.Absolute), path);
      logger.Info("Testing request against " + path1.AbsoluteUri);
      HttpWebResponse response = AfdValidationHelper.SendPreviewRequest(path1);
      if (response.StatusCode != HttpStatusCode.OK)
      {
        if (response.StatusCode == HttpStatusCode.BadRequest)
          throw new HttpRequestException("Routing failed for request " + path1.AbsoluteUri + ".");
        logger.Info(string.Format("Request to {0} returned a non-200 response code: {1} {2}.", (object) path1.AbsoluteUri, (object) (int) response.StatusCode, (object) response.StatusCode));
      }
      AfdValidationHelper.CheckResponseHeaders(response, expectProcessIdHeader);
    }

    private static void TestRequestRouting(string endpoint)
    {
      Uri path = new Uri(endpoint);
      HttpWebResponse response1 = AfdValidationHelper.SendPreviewRequest(path);
      if (response1.StatusCode != HttpStatusCode.OK && response1.StatusCode != HttpStatusCode.Found && response1.StatusCode != HttpStatusCode.MovedPermanently)
        throw new HttpRequestException(string.Format("Request to {0} failed.", (object) path));
      AfdValidationHelper.CheckResponseHeaders(response1, new bool?(true));
      HttpWebResponse response2 = AfdValidationHelper.SendPreviewRequest(path);
      if (response2.StatusCode != HttpStatusCode.OK && response2.StatusCode != HttpStatusCode.Found && response2.StatusCode != HttpStatusCode.MovedPermanently)
        throw new HttpRequestException(string.Format("Request to {0} failed.", (object) path));
      AfdValidationHelper.CheckResponseHeaders(response2, new bool?(true), new bool?(false));
    }

    private static HttpWebResponse SendPreviewRequest(Uri path)
    {
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(new UriBuilder(path)
      {
        Host = "preview.afdcp.com"
      }.Uri);
      request.Host = path.Host;
      request.UserAgent = "AFD Validation Helper";
      request.AllowAutoRedirect = false;
      request.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((sender, certificate, chain, errors) => true);
      return AfdValidationHelper.GetResponseWithRetries(request);
    }

    private static HttpWebResponse GetResponseWithRetries(HttpWebRequest request)
    {
      int num = 0;
      HttpWebResponse response;
      do
      {
        try
        {
          response = (HttpWebResponse) request.GetResponse();
        }
        catch (WebException ex) when (ex.Response != null)
        {
          response = (HttpWebResponse) ex.Response;
        }
      }
      while (AfdValidationHelper.s_retryableStatusCodes.Contains(response.StatusCode) && num++ < 3);
      return response;
    }

    private static void CheckResponseHeaders(
      HttpWebResponse response,
      bool? expectProcessId = null,
      bool? expectRequestRouted = null)
    {
      if (string.IsNullOrEmpty(response.Headers["X-MSEdge-Ref"]))
        throw new HttpRequestException("Response from AFD did not contain an X-MSEdge-Ref header.");
      if (expectProcessId.HasValue && expectProcessId.Value == string.IsNullOrEmpty(response.Headers["X-TFS-ProcessId"]))
        throw new HttpRequestException("Unexpected ProcessID header value: " + (response.Headers["X-TFS-ProcessId"] ?? string.Empty) + ".");
      if (!expectRequestRouted.HasValue)
        return;
      bool? nullable = expectRequestRouted;
      bool flag = string.IsNullOrEmpty(response.Headers["X-TFS-RequestRouted"]);
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        throw new HttpRequestException("Unexpected RequestRouted header value: " + (response.Headers["X-TFS-RequestRouted"] ?? string.Empty) + ".");
    }
  }
}
