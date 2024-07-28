// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.EnvironmentProxyCredentialsProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class EnvironmentProxyCredentialsProvider : IProxyCredentialsProvider
  {
    private const string HttpProxyVariable = "HTTP_PROXY";
    private const string HttpsProxyVariable = "HTTPS_PROXY";
    private const char UserInfoSeparator = ':';
    private readonly ILogger _logger;

    public EnvironmentProxyCredentialsProvider(ILoggerProvider logger) => this._logger = logger.Get((object) this);

    public ICredentials Get(Uri proxy)
    {
      string proxyVariableName = this.GetProxyVariableName(proxy);
      if (string.IsNullOrEmpty(proxyVariableName))
      {
        this._logger.Info(string.Format("{0} does not contain supported http scheme.", (object) proxy));
        return (ICredentials) null;
      }
      string environmentVariable = Environment.GetEnvironmentVariable(proxyVariableName);
      if (string.IsNullOrEmpty(environmentVariable))
      {
        this._logger.Info("Environment variable " + proxyVariableName + " is not defined.");
        return (ICredentials) null;
      }
      if (!Uri.IsWellFormedUriString(environmentVariable, UriKind.Absolute))
      {
        string message = "Value " + environmentVariable + " of environment variable " + proxyVariableName + " is not well formed uri.";
        this._logger.Warning(message);
        throw new ArgumentException(message);
      }
      Uri uri1 = new Uri(environmentVariable);
      if (Uri.Compare(uri1, proxy, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0)
      {
        this._logger.Info(string.Format("{0} does not match with value ({1}) of environment variable {2}.", (object) proxy, (object) environmentVariable, (object) proxyVariableName));
        return (ICredentials) null;
      }
      if (string.IsNullOrEmpty(uri1.UserInfo))
      {
        this._logger.Info("Environment variable " + environmentVariable + " does not contain user info part.");
        return (ICredentials) null;
      }
      string[] strArray = uri1.UserInfo.Split(':');
      if (strArray.Length != 2)
      {
        this._logger.Warning("User info from " + environmentVariable + " is not in correct format.");
        return (ICredentials) null;
      }
      return (ICredentials) new NetworkCredential()
      {
        UserName = WebUtility.UrlDecode(strArray[0]),
        Password = WebUtility.UrlDecode(strArray[1])
      };
    }

    private string GetProxyVariableName(Uri proxy)
    {
      if (string.Equals(proxy.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        return "HTTP_PROXY";
      return string.Equals(proxy.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? "HTTPS_PROXY" : (string) null;
    }
  }
}
