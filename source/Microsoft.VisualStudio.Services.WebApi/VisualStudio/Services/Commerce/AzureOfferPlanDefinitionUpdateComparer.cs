// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureOfferPlanDefinitionUpdateComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureOfferPlanDefinitionUpdateComparer : IEqualityComparer<AzureOfferPlanDefinition>
  {
    public bool Equals(AzureOfferPlanDefinition first, AzureOfferPlanDefinition second)
    {
      if ((object) first == (object) second)
        return true;
      return first != (AzureOfferPlanDefinition) null && second != (AzureOfferPlanDefinition) null && first.Equals(second, true);
    }

    public int GetHashCode(AzureOfferPlanDefinition obj) => ((((((obj.PlanId != null ? obj.PlanId.GetHashCode() : 0) * 17 ^ (obj.Publisher != null ? obj.Publisher.GetHashCode() : 0)) * 17 ^ (obj.OfferName != null ? obj.OfferName.GetHashCode() : 0)) * 17 ^ (obj.OfferId != null ? obj.OfferId.GetHashCode() : 0)) * 17 ^ (obj.PlanName != null ? obj.PlanName.GetHashCode() : 0)) * 17 ^ obj.Quantity) * 17 ^ (obj.PublisherName != null ? obj.PublisherName.GetHashCode() : 0);
  }
}
