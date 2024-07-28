// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CommerceDataProps
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class CommerceDataProps
  {
    public DateTime EventTime;
    public int TrialStartCount;
    public int TrialEndCount;
    public int TrialExtendCount;
    public int BuyCountConnected;
    public int BuyCountHosted;
    public int NewPurchaseQuantityHosted;
    public int NewPurchaseQuantityConnected;
    public int UpgradeQuantityHosted;
    public int UpgradeQuantityConnected;
    public int DowngradeQuantityHosted;
    public int DowngradeQuantityConnected;
    public int RenewalQuantityHosted;
    public int RenewalQuantityConnected;
    public int CanceledQuantityHosted;
    public int CanceledQuantityConnected;
    public int CanceledAccountsCountHosted;
    public int CanceledAccountsCountConnected;
    public int NewPurchaseAccountsCountHosted;
    public int NewPurchaseAccountsCountConnected;
    public int RenewalAccountsCountHosted;
    public int RenewalAccountsCountConnected;
    public int UpgradeAccountsCountHosted;
    public int UpgradeAccountsCountConnected;
    public int DowngradeAccountsCountHosted;
    public int DowngradeAccountsCountConnected;

    public CommerceDataProps()
    {
      this.EventTime = new DateTime();
      this.TrialStartCount = 0;
      this.TrialEndCount = 0;
      this.TrialExtendCount = 0;
      this.BuyCountConnected = 0;
      this.BuyCountHosted = 0;
      this.NewPurchaseQuantityHosted = 0;
      this.NewPurchaseQuantityConnected = 0;
      this.UpgradeQuantityHosted = 0;
      this.UpgradeQuantityConnected = 0;
      this.DowngradeQuantityHosted = 0;
      this.DowngradeQuantityConnected = 0;
      this.RenewalQuantityHosted = 0;
      this.RenewalQuantityConnected = 0;
      this.CanceledQuantityHosted = 0;
      this.CanceledQuantityConnected = 0;
      this.CanceledAccountsCountHosted = 0;
      this.CanceledAccountsCountConnected = 0;
      this.NewPurchaseAccountsCountHosted = 0;
      this.NewPurchaseAccountsCountConnected = 0;
      this.RenewalAccountsCountHosted = 0;
      this.RenewalAccountsCountConnected = 0;
      this.UpgradeAccountsCountHosted = 0;
      this.UpgradeAccountsCountConnected = 0;
      this.DowngradeAccountsCountHosted = 0;
      this.DowngradeAccountsCountConnected = 0;
    }
  }
}
