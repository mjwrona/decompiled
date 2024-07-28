// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInfo
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureSubscriptionInfo
  {
    private string quotaId;
    private string locationPlacementId;

    public string Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public string DisplayName { get; set; }

    public SubscriptionStatus Status { get; set; }

    public bool IsAdministrator { get; set; }

    public Guid TenantId { get; set; }

    public AzureSpendingLimit SpendingLimit { get; set; }

    public AzureSubscriptionAttributes Attributes { get; private set; }

    public AzureOfferType OfferType { get; private set; }

    public AzureLocationPlacementSettings LocationPlacementSettings { get; private set; } = new AzureLocationPlacementSettings();

    public string LocationPlacementId
    {
      get => this.locationPlacementId;
      set
      {
        this.locationPlacementId = value;
        AzureLocationPlacementSettings settings;
        if (!AzureLocationPlacement.TryMap(this.locationPlacementId, out settings))
          return;
        this.LocationPlacementSettings = settings;
      }
    }

    public string QuotaId
    {
      get => this.quotaId;
      set
      {
        this.quotaId = value;
        AzureQuotaId.Mapping mapping;
        if (AzureQuotaId.TryMap(this.quotaId, out mapping))
        {
          this.Attributes = mapping.Attributes;
          this.OfferType = mapping.OfferType;
        }
        else
          this.OfferType = AzureOfferType.Unsupported;
      }
    }

    public override string ToString() => this.Serialize<AzureSubscriptionInfo>();
  }
}
