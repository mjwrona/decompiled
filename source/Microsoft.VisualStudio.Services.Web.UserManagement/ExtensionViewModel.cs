// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.ExtensionViewModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using System;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class ExtensionViewModel
  {
    public string ExtensionId { get; set; }

    public string DisplayName { get; set; }

    public string ExtensionState { get; set; }

    public bool AllOrNothing { get; set; }

    public bool IsEarlyAdopter { get; set; }

    public DateTime? BillingStartDate { get; set; }

    public int GracePeriod { get; set; }

    public int TrialPeriod { get; set; }

    public bool IsPurchaseCanceled { get; set; }

    public bool IsTrialExpiredWithNoPurchase { get; set; }

    public bool IsFirstParty { get; set; }

    public int IncludedQuantity { get; set; }

    public GetExtensionUrlsViewModel ExtensionUrlsViewModel { get; set; }
  }
}
