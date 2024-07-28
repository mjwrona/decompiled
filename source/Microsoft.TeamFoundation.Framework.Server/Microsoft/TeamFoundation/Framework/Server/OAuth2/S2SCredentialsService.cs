// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.S2SCredentialsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class S2SCredentialsService : IS2SCredentialsService, IVssFrameworkService
  {
    private const string TraceArea = "Authentication";
    private const string TraceLayer = "Service";
    internal const string S2SAuthSecretsDrawer = "S2SAuthSecrets";
    private long m_oauthS2SCredentialsVersion;

    internal ConcurrentDictionary<string, VssCredentials> m_credentialsCache { get; private set; }

    public S2SCredentialsService()
    {
      this.m_credentialsCache = new ConcurrentDictionary<string, VssCredentials>();
      this.m_oauthS2SCredentialsVersion = -1L;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public VssCredentials GetS2SCredentials(
      IVssRequestContext requestContext,
      Guid servicePrincipal)
    {
      requestContext.CheckDeploymentRequestContext();
      if (!requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) OAuth2RegistryConstants.S2SClientEnabled, false))
        throw new NotSupportedException("All SDK Services were required to on-board AAD S2S OAuth by M85. Please see https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/2552/S2S-Authentication.");
      string servicePrincipalString = servicePrincipal.ToString("D");
      requestContext.Trace(5510600, TraceLevel.Verbose, "Authentication", "Service", "GetS2SCredentials servicePrincipalString:{0}", (object) servicePrincipalString);
      IOAuth2SettingsService oauthSettingsService = requestContext.GetService<IOAuth2SettingsService>();
      long version = oauthSettingsService.GetVersion(requestContext);
      if (servicePrincipal == Guid.Empty)
        return this.GetS2SOAuthCredentials(requestContext, servicePrincipalString, oauthSettingsService);
      if (this.m_oauthS2SCredentialsVersion != version)
      {
        this.ClearCache();
        this.m_oauthS2SCredentialsVersion = version;
      }
      return this.m_credentialsCache.GetOrAdd(string.Format("{0}:{1}", (object) servicePrincipalString, (object) this.m_oauthS2SCredentialsVersion), (Func<string, VssCredentials>) (key => this.GetS2SOAuthCredentials(requestContext, servicePrincipalString, oauthSettingsService, true)));
    }

    internal virtual VssCredentials GetS2SOAuthCredentials(
      IVssRequestContext deploymentContext,
      string servicePrincipalString,
      IOAuth2SettingsService oauthSettingsService,
      bool preAuthenticate = false)
    {
      IS2SAuthSettings s2SauthSettings = oauthSettingsService.GetS2SAuthSettings(deploymentContext);
      return S2SCredentialsService.GetS2SOAuthCredentials(deploymentContext, servicePrincipalString, s2SauthSettings.GetSigningCertificate(deploymentContext), s2SauthSettings.IssuanceEndpoint, s2SauthSettings.TenantDomain, s2SauthSettings.PrimaryServicePrincipal, s2SauthSettings.Issuer, preAuthenticate);
    }

    internal static VssCredentials GetS2SOAuthCredentials(
      IVssRequestContext deploymentContext,
      string servicePrincipalString,
      X509Certificate2 signingCertificate,
      Uri issuanceEndpoint,
      string tenantDomain,
      Guid primaryServicePrincipal,
      string issuer,
      bool preAuthenticate)
    {
      string targetServicePrincipalName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/visualstudio.com", (object) servicePrincipalString);
      VssSigningCredentials signingCredentials = VssSigningCredentials.Create(signingCertificate);
      return new VssCredentials(new WindowsCredential(false), (FederatedCredential) new VssOAuth2ServicePrincipalCredential(issuanceEndpoint, tenantDomain, primaryServicePrincipal, targetServicePrincipalName, issuer, signingCredentials, preAuthenticate: preAuthenticate));
    }

    private void ClearCache() => this.m_credentialsCache.Clear();
  }
}
