// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.Ev4EntitlementSettingsManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  public class Ev4EntitlementSettingsManager : IEv4EntitlementSettingsManager
  {
    private const string c_Ev4EntitlementRegistryRoot = "/Service/SubscriptionServiceEntitlements/";
    private const string c_Ev4EntitlementClientId = "/Service/SubscriptionServiceEntitlements/ClientId";
    private const string c_Ev4EntitlementAadInstance = "/Service/SubscriptionServiceEntitlements/AadInstance";
    private const string c_Ev4EntitlementUri = "/Service/SubscriptionServiceEntitlements/Uri";
    private const string c_Ev4EntitlementTenantId = "/Service/SubscriptionServiceEntitlements/TenantId";
    private const string c_Ev4EntitlementResource = "/Service/SubscriptionServiceEntitlements/Resource";
    private const string c_Ev4EntitlementUseSubjectNameAuthentication = "/Service/SubscriptionServiceEntitlements/UseSubjectNameAuthentication";
    private const string c_Ev4EntitlementClientCertificate = "/Service/SubscriptionServiceEntitlements/CertName";
    private const string c_Ev4EntitlementLogServiceName = "/Service/SubscriptionServiceEntitlements/LogServiceName";

    public Ev4EntitlementsSettings GetEv4Settings(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/ClientId", (string) null);
      string str2 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/AadInstance", (string) null);
      string str3 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/Uri", (string) null);
      string str4 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/TenantId", (string) null);
      string str5 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/Resource", (string) null);
      bool flag = service.GetValue<bool>(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/UseSubjectNameAuthentication", false);
      string str6 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/CertName", (string) null);
      string str7 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/SubscriptionServiceEntitlements/LogServiceName", (string) null);
      return new Ev4EntitlementsSettings()
      {
        ClientId = str1,
        AadInstance = str2,
        Uri = str3,
        TenantId = str4,
        Resource = str5,
        UseSubjectNameAuthentication = flag,
        EntitlementClientCertificateLookupKey = str6,
        LogServiceName = str7
      };
    }
  }
}
