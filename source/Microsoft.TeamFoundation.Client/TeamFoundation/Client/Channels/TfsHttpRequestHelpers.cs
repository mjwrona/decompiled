// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsHttpRequestHelpers
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TfsHttpRequestHelpers
  {
    public static XmlWriter AddValidatingWriter(XmlWriter writer, Encoding encoding) => XmlWriter.Create(writer, new XmlWriterSettings()
    {
      Encoding = encoding,
      Indent = false,
      NewLineHandling = NewLineHandling.None,
      CheckCharacters = true
    });

    public static UriBuilder CreateUri(string url, string queryString) => new UriBuilder(url)
    {
      Query = queryString
    };

    public static HttpWebRequest CreateArtifactRequest(Uri uri, TfsConnection connection)
    {
      HttpWebRequest soapRequest = TfsHttpRequestHelpers.CreateSoapRequest(uri, connection, string.Empty);
      soapRequest.Method = "GET";
      return soapRequest;
    }

    public static HttpWebRequest CreateSoapRequest(Uri requestUri, TfsConnection connection) => TfsHttpRequestHelpers.CreateSoapRequest(requestUri, connection, (string) null);

    public static HttpWebRequest CreateSoapRequest(
      Uri requestUri,
      TfsConnection connection,
      string operationName)
    {
      return TfsHttpRequestHelpers.CreateSoapRequest(requestUri, connection.SessionId, operationName, connection.Culture, TfsRequestSettings.Default, connection.ClientCredentials, connection.IdentityToImpersonate);
    }

    public static HttpWebRequest CreateSoapRequest(
      Uri requestUri,
      Guid sessionId,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate)
    {
      Microsoft.VisualStudio.Services.Common.IssuedToken currentToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
      Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider = (Microsoft.VisualStudio.Services.Common.IssuedTokenProvider) null;
      return TfsHttpRequestHelpers.CreateSoapRequest(requestUri, sessionId, operationName, cultureInfo, settings, credentials, impersonate, false, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest CreateSoapRequest(
      Uri requestUri,
      Guid sessionId,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate,
      bool applyICredentialsToWebProxy,
      ref Microsoft.VisualStudio.Services.Common.IssuedToken currentToken,
      ref Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider)
    {
      return TfsHttpRequestHelpers.CreateSoapRequest(requestUri, sessionId, string.Empty, operationName, cultureInfo, settings, credentials, impersonate, applyICredentialsToWebProxy, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest CreateSoapRequest(
      Uri requestUri,
      Guid sessionId,
      string soapAction,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate,
      bool applyICredentialsToWebProxy,
      ref Microsoft.VisualStudio.Services.Common.IssuedToken currentToken,
      ref Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider)
    {
      ArgumentUtility.CheckForNull<Uri>(requestUri, nameof (requestUri));
      HttpWebRequest soapRequest = TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(requestUri), sessionId, operationName, cultureInfo, settings, credentials, impersonate, applyICredentialsToWebProxy, ref currentToken, ref tokenProvider);
      soapRequest.ContentType = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "application/soap+xml; charset={0}", (object) TfsRequestSettings.RequestEncoding.WebName);
      soapRequest.Method = "POST";
      if (!string.IsNullOrEmpty(soapAction))
        soapRequest.Headers.Add("SOAPAction", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) soapAction));
      if (settings.CompressionEnabled)
        soapRequest.AutomaticDecompression = DecompressionMethods.GZip;
      return soapRequest;
    }

    public static HttpWebRequest CreateWebRequest(Uri requestUri, TfsConnection connection) => TfsHttpRequestHelpers.CreateWebRequest(requestUri, connection, (string) null);

    public static HttpWebRequest CreateWebRequest(
      Uri requestUri,
      TfsConnection connection,
      string operationName)
    {
      return TfsHttpRequestHelpers.CreateSoapRequest(requestUri, connection.SessionId, operationName, connection.Culture, TfsRequestSettings.Default, connection.ClientCredentials, connection.IdentityToImpersonate);
    }

    public static HttpWebRequest CreateWebRequest(
      Uri requestUri,
      Guid sessionId,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate,
      bool applyICredentialsToWebProxy,
      ref Microsoft.VisualStudio.Services.Common.IssuedToken currentToken,
      ref Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider)
    {
      return TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(requestUri), sessionId, operationName, cultureInfo, settings, credentials, impersonate, applyICredentialsToWebProxy, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest PrepareWebRequest(
      HttpWebRequest webRequest,
      TfsConnection connection)
    {
      Microsoft.VisualStudio.Services.Common.IssuedToken currentToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
      Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider = (Microsoft.VisualStudio.Services.Common.IssuedTokenProvider) null;
      return TfsHttpRequestHelpers.PrepareWebRequest(webRequest, connection, (string) null, TfsRequestSettings.Default, false, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest PrepareWebRequest(
      HttpWebRequest webRequest,
      TfsConnection connection,
      string operationName,
      TfsRequestSettings settings,
      bool applyICredentialsToWebProxy,
      ref Microsoft.VisualStudio.Services.Common.IssuedToken currentToken,
      ref Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider)
    {
      return TfsHttpRequestHelpers.PrepareWebRequest(webRequest, connection.SessionId, operationName, connection.Culture, settings, connection.ClientCredentials, connection.IdentityToImpersonate, applyICredentialsToWebProxy, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest PrepareWebRequest(
      HttpWebRequest webRequest,
      Guid sessionId,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate)
    {
      Microsoft.VisualStudio.Services.Common.IssuedToken currentToken = (Microsoft.VisualStudio.Services.Common.IssuedToken) null;
      Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider = (Microsoft.VisualStudio.Services.Common.IssuedTokenProvider) null;
      return TfsHttpRequestHelpers.PrepareWebRequest(webRequest, sessionId, operationName, cultureInfo, settings, credentials, impersonate, false, ref currentToken, ref tokenProvider);
    }

    public static HttpWebRequest PrepareWebRequest(
      HttpWebRequest webRequest,
      Guid sessionId,
      string operationName,
      CultureInfo cultureInfo,
      TfsRequestSettings settings,
      VssCredentials credentials,
      IdentityDescriptor impersonate,
      bool applyICredentialsToWebProxy,
      ref Microsoft.VisualStudio.Services.Common.IssuedToken currentToken,
      ref Microsoft.VisualStudio.Services.Common.IssuedTokenProvider tokenProvider)
    {
      ArgumentUtility.CheckForNull<HttpWebRequest>(webRequest, nameof (webRequest));
      webRequest.UseDefaultCredentials = false;
      if (settings.BypassProxyOnLocal && BypassProxyOnLocalHelper.IsHostLocal(webRequest.RequestUri.Host))
        webRequest.Proxy = (IWebProxy) null;
      else
        webRequest.Proxy = VssHttpMessageHandler.DefaultWebProxy;
      if (currentToken == null && (tokenProvider != null || credentials.TryGetTokenProvider(webRequest.RequestUri, out tokenProvider)))
        currentToken = tokenProvider.CurrentToken;
      if (currentToken != null)
      {
        if (currentToken is ICredentials credentials1)
        {
          if (applyICredentialsToWebProxy && webRequest.Proxy != null)
            webRequest.Proxy.Credentials = credentials1;
          webRequest.Credentials = credentials1;
          if (credentials1 is Microsoft.VisualStudio.Services.Common.WindowsToken windowsToken)
          {
            webRequest.ConnectionGroupName = TfsHttpRequestHelpers.GetConnectionGroupName(webRequest.RequestUri, windowsToken.Credentials);
            webRequest.UnsafeAuthenticatedConnectionSharing = true;
          }
        }
        else
          currentToken.ApplyTo((IHttpRequest) new HttpWebRequestWrapper(webRequest));
        if (!currentToken.IsAuthenticated && !currentToken.FromStorage)
          webRequest.Headers.Set("X-VSS-UserData", string.Empty);
      }
      webRequest.UserAgent = TfsConnection.ApplicationName;
      webRequest.Headers.Set("X-TFS-FedAuthRedirect", "Suppress");
      webRequest.Headers.Set("X-TFS-Version", TeamFoundationVersion.CurrentContractVersion.ToString());
      webRequest.Headers.Set(HttpRequestHeader.AcceptLanguage, cultureInfo.Name);
      if (!string.IsNullOrEmpty(operationName))
        webRequest.Headers.Set("X-TFS-Session", sessionId.ToString("D") + ", " + operationName);
      else
        webRequest.Headers.Set("X-TFS-Session", sessionId.ToString("D"));
      if (!string.IsNullOrEmpty(settings.AgentId))
        webRequest.Headers.Set("X-VSS-Agent", settings.AgentId);
      if (impersonate != null)
        webRequest.Headers.Set("X-TFS-Impersonate", TFCommonUtil.MakeDescriptorSearchFactor(impersonate.IdentityType, impersonate.Identifier));
      webRequest.KeepAlive = true;
      webRequest.Pipelined = false;
      webRequest.PreAuthenticate = false;
      int num = Math.Min((int) settings.SendTimeout.TotalMilliseconds, int.MaxValue);
      webRequest.Timeout = num;
      webRequest.ReadWriteTimeout = num;
      webRequest.AllowAutoRedirect = false;
      ClientCertificateManager.Instance.ApplyCertificatesToWebRequest(webRequest);
      webRequest.ServicePoint.UseNagleAlgorithm = false;
      webRequest.ServicePoint.SetTcpKeepAlive(true, 30000, 5000);
      return webRequest;
    }

    public static string GetConnectionGroupName(Uri uri, ICredentials credentials)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      IEnumerator registeredModules = AuthenticationManager.RegisteredModules;
      int hashCode;
      while (registeredModules.MoveNext())
      {
        IAuthenticationModule current = (IAuthenticationModule) registeredModules.Current;
        NetworkCredential credential = credentials.GetCredential(uri, current.AuthenticationType);
        if (credential != null)
        {
          StringBuilder stringBuilder2 = stringBuilder1;
          hashCode = credential.GetHashCode();
          string str = hashCode.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          stringBuilder2.Append(str);
        }
      }
      StringBuilder stringBuilder3 = stringBuilder1;
      hashCode = WindowsIdentity.GetCurrent().User.GetHashCode();
      string str1 = hashCode.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      stringBuilder3.Append(str1);
      return stringBuilder1.ToString();
    }

    public static void TraceHeaders(WebHeaderCollection headers, string[] keywords = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("HTTP headers:");
      for (int index = 0; index < headers.Count; ++index)
      {
        stringBuilder.Append(headers.Keys[index]);
        stringBuilder.Append(": ");
        foreach (string str in headers.GetValues(index))
          stringBuilder.AppendLine(str);
      }
      TeamFoundationTrace.Verbose(keywords ?? TraceKeywordSets.General, stringBuilder.ToString());
    }
  }
}
