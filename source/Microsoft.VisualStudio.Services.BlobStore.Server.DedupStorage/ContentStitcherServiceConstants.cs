// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher.ContentStitcherServiceConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9521FAE3-5DB1-49D0-98DB-6A544E3AB730
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.DedupStorage.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher
{
  public static class ContentStitcherServiceConstants
  {
    public const string HostedServiceNameRegistry = "/Configuration/Settings/HostedServiceName";
    public const string ServicePrincipal = "00000070-0000-8888-8000-000000000000";
    public const string LoginAuthority = "https://login.windows.net/";
    public const int DefaultRetryCount = 3;
    public const int DefaultRetryBackoffExponentBase = 5;
    public const string BlobstoreHostedServiceShortNamePrefix = "vsblob";
    public const string DevDomainName = "dedupstorage.k8s";
    public const string ProdDomainName = "dedup.microsoft.com";
    public const string PpeDomainName = "dedup.microsoft-ppe.com";
    public const string PfDomainName = "dedup.microsoft-int.com";

    public static string GetAppIdWithScope(bool isProduction) => "api://" + ContentStitcherServiceConstants.GetAppId(isProduction) + "/.default";

    private static string GetAppId(bool isProduction) => !isProduction ? "040cdc62-00ba-41b5-8dc5-95331f90d29b" : "0c71d3b1-e814-4cc0-ab02-40ecd2b3d4fe";

    public static string GetHostNameByTenantName(bool isProduction, string tenantName)
    {
      if (!isProduction)
        return "dedupstorage.k8s";
      string nameByTenantName;
      switch (tenantName)
      {
        case "prodeus23":
          nameByTenantName = "ppeeus23.dedup.microsoft-ppe.com";
          break;
        case "ppeeus23":
          nameByTenantName = tenantName + ".dedup.microsoft-ppe.com";
          break;
        case "pfeus2c":
          nameByTenantName = tenantName + ".dedup.microsoft-int.com";
          break;
        case "prodsu6weu":
          nameByTenantName = "prodweusu6.dedup.microsoft.com";
          break;
        default:
          nameByTenantName = tenantName + ".dedup.microsoft.com";
          break;
      }
      return nameByTenantName;
    }
  }
}
