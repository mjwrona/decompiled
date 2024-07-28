// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureOfferPlanDefinition
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureOfferPlanDefinition : IEquatable<AzureOfferPlanDefinition>
  {
    public int MeterId { get; set; }

    public string PlanId { get; set; }

    public string Publisher { get; set; }

    public string OfferName { get; set; }

    public string OfferId { get; set; }

    public string PlanName { get; set; }

    public string PlanVersion { get; set; }

    public int Quantity { get; set; }

    public bool IsPublic { get; set; }

    public string PublisherName { get; set; }

    public bool Equals(AzureOfferPlanDefinition plan) => this.Equals(plan, false);

    public bool Equals(AzureOfferPlanDefinition plan, bool compareForUpdate)
    {
      if (!(plan != (AzureOfferPlanDefinition) null))
        return false;
      return compareForUpdate ? string.Equals(this.PlanId, plan.PlanId, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Publisher, plan.Publisher, StringComparison.OrdinalIgnoreCase) && string.Equals(this.OfferName, plan.OfferName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.PlanName, plan.PlanName, StringComparison.OrdinalIgnoreCase) && this.Quantity == plan.Quantity && string.Equals(this.PublisherName, plan.PublisherName, StringComparison.OrdinalIgnoreCase) : this.MeterId == plan.MeterId && this.PlanVersion == plan.PlanVersion && this.IsPublic == plan.IsPublic && this.PlanId == plan.PlanId && this.Publisher == plan.Publisher && this.OfferName == plan.OfferName && this.PlanName == plan.PlanName && this.Quantity == plan.Quantity && this.OfferId == plan.OfferId && this.PublisherName == plan.PublisherName;
    }

    public override int GetHashCode() => this.PlanId == null ? this.MeterId.GetHashCode() : this.PlanId.GetHashCode();

    public override bool Equals(object obj) => this.Equals(obj as AzureOfferPlanDefinition);

    public AzureOfferPlanDefinition Clone() => new AzureOfferPlanDefinition()
    {
      MeterId = this.MeterId,
      PlanId = this.PlanId,
      Publisher = this.Publisher,
      OfferName = this.OfferName,
      PlanName = this.PlanName,
      PlanVersion = this.PlanVersion,
      Quantity = this.Quantity,
      IsPublic = this.IsPublic,
      OfferId = this.OfferId,
      PublisherName = this.PublisherName
    };

    public static bool operator ==(AzureOfferPlanDefinition left, AzureOfferPlanDefinition right) => (object) left == null || (object) right == null ? object.Equals((object) left, (object) right) : left.Equals(right);

    public static bool operator !=(AzureOfferPlanDefinition left, AzureOfferPlanDefinition right) => !(left == right);
  }
}
