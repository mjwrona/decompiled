// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.MarketplacePurchase
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class MarketplacePurchase
  {
    public string PlanName { get; set; }

    public string BillingCycle { get; set; }

    public string UnitName { get; set; }

    public int UnitCount { get; set; }

    public string NextBillingDate { get; set; }
  }
}
