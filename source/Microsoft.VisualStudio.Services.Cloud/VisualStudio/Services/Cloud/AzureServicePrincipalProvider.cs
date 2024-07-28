// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureServicePrincipalProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.KeyVault;
using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureServicePrincipalProvider
  {
    internal const string ClientIdKey = "RuntimeServicePrincipalClientId";
    internal const string ThumbprintKey = "RuntimeServicePrincipalCertThumbprint";
    internal const string TenantKey = "RuntimeServicePrincipalApplicationTenantId";
    private IConfidentialClientApplication m_confidentialClientApplication;
    private string m_clientId;
    private string m_certThumbprint;
    private string m_tenantId;
    private string m_authority;
    private X509Certificate2 m_cert;
    private readonly ITFLogger m_logger;

    public AzureServicePrincipalProvider()
      : this((ITFLogger) null)
    {
    }

    public AzureServicePrincipalProvider(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new NullLogger();

    public KeyVaultClient.AuthenticationCallback GetKeyVaultAuthCallback() => new KeyVaultClient.AuthenticationCallback(this.GetTokenAsync);

    public string RuntimeServicePrincipalClientId => this.m_clientId ?? (this.m_clientId = AzureRoleUtil.GetOverridableConfigurationSetting(nameof (RuntimeServicePrincipalClientId)));

    public string RuntimeServicePrincipalCertificateThumbprint => this.m_certThumbprint ?? (this.m_certThumbprint = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint"));

    public string RuntimeServicePrincipalApplicationTenantId => this.m_tenantId ?? (this.m_tenantId = AzureRoleUtil.GetOverridableConfigurationSetting(nameof (RuntimeServicePrincipalApplicationTenantId)));

    public X509Certificate2 RuntimeServicePrincipalCertificate
    {
      get
      {
        if (this.m_cert == null)
        {
          X509Certificate2 certificateByThumbprint = this.FindCertificateByThumbprint(this.RuntimeServicePrincipalCertificateThumbprint);
          if (certificateByThumbprint == null)
            throw new ApplicationException("Can't find the cert with thumbprint '" + this.RuntimeServicePrincipalCertificateThumbprint + "'");
          this.m_cert = certificateByThumbprint.HasPrivateKey ? certificateByThumbprint : throw new ApplicationException("Cert found with thumbprint '" + this.RuntimeServicePrincipalCertificateThumbprint + "' does not have private key");
        }
        return this.m_cert;
      }
    }

    internal virtual X509Certificate2 FindCertificateByThumbprint(string thumbprint) => new CertHandler().FindCertificateByThumbprint(thumbprint);

    public AuthenticationResult GetAuthResult(string authority, string resource) => this.ConfidentialClientApplication.AcquireTokenForClient((IEnumerable<string>) MsalUtility.GetScopes(resource)).WithAuthority(authority).ExecuteAsync().SyncResultConfigured<AuthenticationResult>();

    private async Task<string> GetTokenAsync(string authority, string resource, string scope)
    {
      this.m_logger.Info("RuntimeKeyVaultProvider - Using clientId " + this.RuntimeServicePrincipalClientId + " and cert with thumbprint " + this.RuntimeServicePrincipalCertificateThumbprint);
      ITFLogger logger = this.m_logger;
      DateTime dateTime = this.RuntimeServicePrincipalCertificate.NotBefore;
      // ISSUE: variable of a boxed type
      __Boxed<DateTime> universalTime1 = (ValueType) dateTime.ToUniversalTime();
      dateTime = this.RuntimeServicePrincipalCertificate.NotAfter;
      // ISSUE: variable of a boxed type
      __Boxed<DateTime> universalTime2 = (ValueType) dateTime.ToUniversalTime();
      string message = string.Format("{0} - Certificate lifetime: {1} - {2}", (object) "RuntimeKeyVaultProvider", (object) universalTime1, (object) universalTime2);
      logger.Info(message);
      return (await this.AcquireTokenForClientAsync(MsalUtility.GetScopes(resource), CancellationToken.None).ConfigureAwait(false)).AccessToken;
    }

    internal virtual Task<AuthenticationResult> AcquireTokenForClientAsync(
      string[] scopes,
      CancellationToken cancellationToken)
    {
      return this.ConfidentialClientApplication.AcquireTokenForClient((IEnumerable<string>) scopes).ExecuteAsync(cancellationToken);
    }

    internal IConfidentialClientApplication ConfidentialClientApplication
    {
      get
      {
        if (this.m_confidentialClientApplication == null)
          Interlocked.CompareExchange<IConfidentialClientApplication>(ref this.m_confidentialClientApplication, ConfidentialClientApplicationBuilder.Create(this.RuntimeServicePrincipalClientId).WithAuthority(this.Authority).WithCertificate(this.RuntimeServicePrincipalCertificate).Build(), (IConfidentialClientApplication) null);
        return this.m_confidentialClientApplication;
      }
    }

    private string Authority => this.m_authority ?? (this.m_authority = "https://login.microsoftonline.com/" + this.RuntimeServicePrincipalApplicationTenantId);
  }
}
