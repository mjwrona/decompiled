// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Remote.GalleryKPIHelpers
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Remote
{
  public class GalleryKPIHelpers
  {
    public const string s_area = "GalleryKPIHelpers";
    public const string s_layer = "Service";

    public static void EnsureKPIsAreRegistered(IVssRequestContext requestContext)
    {
      KpiService service = requestContext.GetService<KpiService>();
      service.EnsureKpiIsRegistered(requestContext, "MarketService", "S2SSuccessEMS", "Service", "S2SSuccess in EMS", "This KPI indicates successful S2S calls from Market service to EMS.");
      service.EnsureKpiIsRegistered(requestContext, "MarketService", "S2SFailureEMS", "Service", "S2SFailure in EMS", "This KPI indicates failures in S2S calls from Market service to EMS.");
      service.EnsureKpiIsRegistered(requestContext, "MarketService", "S2SSuccessBilling", "Service", "S2SSuccess in Billing", "This KPI indicates successful S2S calls from Market service to Billing.");
      service.EnsureKpiIsRegistered(requestContext, "MarketService", "S2SFailureBilling", "Service", "S2SFailure in Billing", "This KPI indicates failures in S2S calls from Market service to Billing.");
    }

    public void LogS2SKPI(IVssRequestContext requestContext, string kpiName) => requestContext.GetService<KpiService>().Publish(requestContext, "MarketService", "Service", kpiName, 1.0);
  }
}
