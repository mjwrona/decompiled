// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Authentication.ManagedIdentityTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Core;
using Azure.Identity;
using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.Authentication
{
  public class ManagedIdentityTokenProvider : ITokenProvider
  {
    private TokenCredential m_credential;
    private readonly string m_resource;
    private readonly string[] m_scopes;
    private readonly ITFLogger m_logger;
    private const string App = "App";
    private const string AppId = "AppId";
    private const string RunAs = "RunAs";
    private const string TenantId = "TenantId";
    private const string CertificateThumbprint = "CertificateThumbprint";
    private const string CertificateStoreLocation = "CertificateStoreLocation";
    private const string c_connectionStringPattern = "([\\s;]*(?![\\s;])(?<key>([^=\\s\\p{Cc}]|\\s+[^=\\s\\p{Cc}]|\\s+==|==)+)\\s*=(?!=)\\s*(?<value>(\"([^\"\0]|\"\")*\")|('([^'\0]|'')*')|((?![\"'\\s])([^;\\s\\p{Cc}]|\\s+[^;\\s\\p{Cc}])*(?<![\"'])))(\\s*)(;|[\0\\s]*$))*[\\s;]*[\0\\s]*";
    private static readonly Regex s_connectionStringRegex = new Regex("([\\s;]*(?![\\s;])(?<key>([^=\\s\\p{Cc}]|\\s+[^=\\s\\p{Cc}]|\\s+==|==)+)\\s*=(?!=)\\s*(?<value>(\"([^\"\0]|\"\")*\")|('([^'\0]|'')*')|((?![\"'\\s])([^;\\s\\p{Cc}]|\\s+[^;\\s\\p{Cc}])*(?<![\"'])))(\\s*)(;|[\0\\s]*$))*[\\s;]*[\0\\s]*", RegexOptions.ExplicitCapture);
    private const string c_connStrName = "DeploymentIdentityConnectionString";

    public ManagedIdentityTokenProvider(string resource, ITFLogger logger = null)
    {
      this.m_logger = logger ?? (ITFLogger) new NullLogger();
      this.CreateTokenCredential();
      this.m_resource = resource;
      this.m_scopes = MsalUtility.GetScopes(this.m_resource);
    }

    public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(
      CancellationToken cancellationToken)
    {
      return AuthenticationHeaderValueExtensions.ToBearerToken(await this.GetTokenAsync());
    }

    public async Task<string> GetTokenAsync()
    {
      this.m_logger.Info("ManagedIdentityTokenProvider - Using Managed Identity to get token for resource '" + this.m_resource + "'.");
      AccessToken tokenAsync = await this.m_credential.GetTokenAsync(new TokenRequestContext(this.m_scopes, (string) null, (string) null, (string) null, false), CancellationToken.None);
      TeamFoundationTracingService.TraceRaw(1234567, TraceLevel.Info, nameof (ManagedIdentityTokenProvider), nameof (ManagedIdentityTokenProvider), string.Format("GotToken. ExpiresOn: {0}.", (object) ((AccessToken) ref tokenAsync).ExpiresOn));
      return ((AccessToken) ref tokenAsync).Token;
    }

    private void CreateTokenCredential()
    {
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting("DeploymentIdentityConnectionString");
      Dictionary<string, string> connectionString = ManagedIdentityTokenProvider.ParseConnectionString(configurationSetting);
      ManagedIdentityTokenProvider.ValidateAttribute(connectionString, "RunAs", configurationSetting);
      if (!string.Equals(connectionString["RunAs"], "App", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("ManagedIdentityTokenProvider only support connection strings, with RunAs=App.");
      string findValue;
      if (connectionString.TryGetValue("CertificateThumbprint", out findValue))
      {
        ManagedIdentityTokenProvider.ValidateAttribute(connectionString, "AppId", configurationSetting);
        ManagedIdentityTokenProvider.ValidateAttribute(connectionString, "TenantId", configurationSetting);
        ManagedIdentityTokenProvider.ValidateAttribute(connectionString, "CertificateStoreLocation", configurationSetting);
        string str1 = connectionString["AppId"];
        string str2 = connectionString["TenantId"];
        X509Certificate2 certificate = new CertHandler(this.m_logger).FindCertificate(StoreName.My, (StoreLocation) Enum.Parse(typeof (StoreLocation), connectionString["CertificateStoreLocation"], true), X509FindType.FindByThumbprint, (object) findValue, false);
        this.m_credential = (TokenCredential) new ClientCertificateCredential(str2, str1, certificate);
      }
      else
      {
        string str;
        connectionString.TryGetValue("AppId", out str);
        this.m_credential = (TokenCredential) new ManagedIdentityCredential(str, (TokenCredentialOptions) null);
      }
    }

    private static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
      Dictionary<string, string> connectionString1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Match match = !string.IsNullOrWhiteSpace(connectionString) ? ManagedIdentityTokenProvider.s_connectionStringRegex.Match(connectionString) : throw new ArgumentException("Connection string is empty.");
      if (!match.Success || match.Length != connectionString.Length)
        throw new ArgumentException("Connection string " + connectionString + " is not in a proper format. Expected format is Key1=Value1;Key2=Value2;");
      int num = 0;
      CaptureCollection captures = match.Groups[2].Captures;
      foreach (Capture capture in match.Groups[1].Captures)
      {
        string key = capture.Value.Replace("==", "=");
        string str = captures[num++].Value;
        if (str.Length > 0)
        {
          switch (str[0])
          {
            case '"':
              str = str.Substring(1, str.Length - 2).Replace("\"\"", "\"");
              break;
            case '\'':
              str = str.Substring(1, str.Length - 2).Replace("''", "'");
              break;
          }
        }
        if (!string.IsNullOrWhiteSpace(key))
        {
          if (!connectionString1.ContainsKey(key))
            connectionString1[key] = str;
          else
            throw new ArgumentException("Connection string " + connectionString + " is not in a proper format. Key '" + key + "' is repeated.");
        }
      }
      return connectionString1;
    }

    private static void ValidateAttribute(
      Dictionary<string, string> connectionSettings,
      string attribute,
      string connectionString)
    {
      if (connectionSettings != null && (!connectionSettings.ContainsKey(attribute) || string.IsNullOrWhiteSpace(connectionSettings[attribute])))
        throw new ArgumentException("Connection string " + connectionString + " is not valid. Must contain '" + attribute + "' attribute and it must not be empty.");
    }
  }
}
