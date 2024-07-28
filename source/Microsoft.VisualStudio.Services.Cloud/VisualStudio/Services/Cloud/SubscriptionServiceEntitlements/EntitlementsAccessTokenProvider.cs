// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.EntitlementsAccessTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  public class EntitlementsAccessTokenProvider : IEntitlementsAccessTokenProvider
  {
    private IConfidentialClientApplication confidentialClient;

    public AuthenticationResult AuthenticateApplication(
      IVssRequestContext requestContext,
      Ev4EntitlementsSettings settings)
    {
      return this.GetConfidentialClientApplication(requestContext, settings).AcquireTokenForClient((IEnumerable<string>) new string[1]
      {
        settings.Resource + "/.default"
      }).WithAuthority(settings.AadInstance + "/" + settings.TenantId).ExecuteAsync().SyncResultConfigured<AuthenticationResult>();
    }

    private IConfidentialClientApplication GetConfidentialClientApplication(
      IVssRequestContext requestContext,
      Ev4EntitlementsSettings settings)
    {
      if (this.confidentialClient == null)
      {
        X509Certificate2 entitlementCertificate = this.GetEv4EntitlementCertificate(requestContext, settings);
        this.confidentialClient = ConfidentialClientApplicationBuilder.Create(settings.ClientId).WithCertificate(entitlementCertificate, settings.UseSubjectNameAuthentication).Build();
      }
      return this.confidentialClient;
    }

    private X509Certificate2 GetEv4EntitlementCertificate(
      IVssRequestContext requestContext,
      Ev4EntitlementsSettings settings)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", settings.EntitlementClientCertificateLookupKey, true);
      return service.RetrieveFileAsCertificate(vssRequestContext, itemInfo.DrawerId, itemInfo.LookupKey);
    }
  }
}
