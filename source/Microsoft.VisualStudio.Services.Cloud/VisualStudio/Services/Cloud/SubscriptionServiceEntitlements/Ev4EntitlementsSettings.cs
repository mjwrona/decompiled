// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.Ev4EntitlementsSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  public class Ev4EntitlementsSettings
  {
    public string ClientId { get; set; }

    public string TenantId { get; set; }

    public string AadInstance { get; set; }

    public string Uri { get; set; }

    public string Resource { get; set; }

    public bool UseSubjectNameAuthentication { get; set; }

    public string EntitlementClientCertificateLookupKey { get; set; }

    public string LogServiceName { get; set; }
  }
}
