// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureSubscriptionInternal
  {
    public Guid AzureSubscriptionId { get; set; }

    public Guid AzureSubscriptionTenantId { get; set; }

    public SubscriptionStatus AzureSubscriptionStatusId { get; set; }

    public SubscriptionSource AzureSubscriptionSource { get; set; }

    public string AzureOfferCode { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    public AzureSubscriptionInternal()
    {
    }

    public AzureSubscriptionInternal(
      Guid azureSubscriptionId,
      Guid azureSubscriptionTenantId,
      SubscriptionStatus azureSubscriptionStatusId,
      SubscriptionSource azureSubscriptionSource = SubscriptionSource.Normal)
    {
      this.AzureSubscriptionId = azureSubscriptionId;
      this.AzureSubscriptionTenantId = azureSubscriptionTenantId;
      this.AzureSubscriptionStatusId = azureSubscriptionStatusId;
      this.AzureSubscriptionSource = azureSubscriptionSource;
    }

    internal IAzureSubscription ToAzureSubscription() => (IAzureSubscription) new AzureSubscription()
    {
      Id = this.AzureSubscriptionId,
      Status = this.AzureSubscriptionStatusId,
      TenantId = this.AzureSubscriptionTenantId,
      Source = this.AzureSubscriptionSource,
      OfferType = AzureQuotaId.ToOfferType(this.AzureOfferCode)
    };
  }
}
