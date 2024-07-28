// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionDailyStatsConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public static class ExtensionDailyStatsConstants
  {
    public const int MaxNDays = 360;
    public static DateTime AverageRatingLoggingStartDate = new DateTime(2017, 3, 20);
    public const int DefaultUninstallEventCount = 10;
    public const string Latest = "latest";
    public const string Buy = "buy";
    public const string ConnectedBuy = "connectedBuy";
    public const string ConnectedInstall = "connectedInstall";
    public const string Download = "download";
    public const string Install = "install";
    public const string Try = "try";
    public const string Uninstall = "uninstall";
    public const string WebPageView = "pageview";
    public const string InstallCount = "installcount";
    public const string ColumnWidth = "columnWidth";
    public const string TrialEndCount = "trialEndCount";
    public const string TrialExtendCount = "trialExtendCount";
    public const string NewPurchaseQuantityHosted = "newPurchaseQuantityHosted";
    public const string NewPurchaseQuantityConnected = "newPurchaseQuantityConnected";
    public const string UpgradeQuantityHosted = "upgradeQuantityHosted";
    public const string UpgradeQuantityConnected = "upgradeQuantityConnected";
    public const string DowngradeQuantityHosted = "downgradeQuantityHosted";
    public const string DowngradeQuantityConnected = "downgradeQuantityConnected";
    public const string RenewalQuantityHosted = "renewalQuantityHosted";
    public const string RenewalQuantityConnected = "renewalQuantityConnected";
    public const string CanceledQuantityHosted = "canceledQuantityHosted";
    public const string CanceledQuantityConnected = "canceledQuantityConnected";
    public const string CanceledAccountsCountHosted = "canceledAccountsCountHosted";
    public const string CanceledAccountsCountConnected = "canceledAccountsCountConnected";
    public const string NewPurchaseAccountsCountHosted = "newPurchaseAccountsCountHosted";
    public const string NewPurchaseAccountsCountConnected = "newPurchaseAccountsCountConnected";
    public const string RenewalAccountsCountHosted = "renewalAccountsCountHosted";
    public const string RenewalAccountsCountConnected = "renewalAccountsCountConnected";
    public const string UpgradeAccountsCountHosted = "upgradeAccountsCountHosted";
    public const string UpgradeAccountsCountConnected = "upgradeAccountsCountConnected";
    public const string DowngradeAccountsCountHosted = "downgradeAccountsCountHosted";
    public const string DowngradeAccountsCountConnected = "downgradeAccountsCountConnected";
    public const string EventType = "eventType";
  }
}
