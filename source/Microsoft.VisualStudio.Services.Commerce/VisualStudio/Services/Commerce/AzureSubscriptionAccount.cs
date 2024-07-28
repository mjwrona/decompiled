// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionAccount
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureSubscriptionAccount
  {
    public Guid AccountId { get; set; }

    public Guid CollectionId { get; set; }

    public Guid? SubscriptionId { get; set; }

    public SubscriptionStatus SubscriptionStatusId { get; set; }

    public string ResourceGroupName { get; set; }

    public string GeoLocation { get; set; }

    public string ResourceName { get; set; }

    public OperationResult OperationResult { get; set; }

    public AzureOfferType? SubscriptionOfferType { get; set; }
  }
}
