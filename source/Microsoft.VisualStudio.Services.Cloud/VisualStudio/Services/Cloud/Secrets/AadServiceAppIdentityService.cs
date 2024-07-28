// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Secrets.AadServiceAppIdentityService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.Secrets
{
  public class AadServiceAppIdentityService : IAadServiceAppIdentityService, IVssFrameworkService
  {
    internal const string ClientIdKey = "AadServiceAppClientId";
    internal const string CertName = "AadServiceAppCertName";
    internal const string TenantIdKey = "AadServiceAppTenantId";
    internal const string AadInstanceKey = "AadServiceAppAadInstance";
    private IConfidentialClientApplication m_confidentialClientApplication;
    private string m_clientId;
    private string m_certName;
    private string m_tenantId;
    private string m_aadInstance;
    private string m_authority;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckSystemRequestContext();
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        this.AppCertName
      });
      Interlocked.CompareExchange<IConfidentialClientApplication>(ref this.m_confidentialClientApplication, this.InitializeClientApplication(systemRequestContext), (IConfidentialClientApplication) null);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));

    private void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      Volatile.Write<IConfidentialClientApplication>(ref this.m_confidentialClientApplication, this.InitializeClientApplication(requestContext));
    }

    private IConfidentialClientApplication InitializeClientApplication(
      IVssRequestContext requestContext)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, "ConfigurationSecrets", this.AppCertName, true);
      X509Certificate2 certificate = service.RetrieveFileAsCertificate(requestContext, itemInfo.DrawerId, itemInfo.LookupKey);
      return certificate.HasPrivateKey ? ConfidentialClientApplicationBuilder.Create(this.AppClientId).WithCertificate(certificate, true).Build() : throw new ApplicationException("Cert found with name '" + this.AppCertName + "' does not have private key");
    }

    public string AppClientId => this.m_clientId ?? (this.m_clientId = AzureRoleUtil.GetOverridableConfigurationSetting("AadServiceAppClientId"));

    public string AppCertName => this.m_certName ?? (this.m_certName = AzureRoleUtil.GetOverridableConfigurationSetting("AadServiceAppCertName"));

    public string AppTenantId => this.m_tenantId ?? (this.m_tenantId = AzureRoleUtil.GetOverridableConfigurationSetting("AadServiceAppTenantId"));

    public string AppAadInstance => this.m_aadInstance ?? (this.m_aadInstance = AzureRoleUtil.GetOverridableConfigurationSetting("AadServiceAppAadInstance"));

    public async Task<AuthenticationResult> GetAuthResultAsync(string resource, string authority = null) => await this.m_confidentialClientApplication.AcquireTokenForClient((IEnumerable<string>) MsalUtility.GetScopes(resource)).WithAuthority(authority ?? this.AppAuthority).ExecuteAsync().ConfigureAwait(false);

    public AuthenticationResult GetAuthResult(string resource, string authority = null) => this.GetAuthResultAsync(resource, authority).SyncResult<AuthenticationResult>();

    private string AppAuthority => this.m_authority ?? (this.m_authority = this.AppAadInstance + "/" + this.AppTenantId);
  }
}
