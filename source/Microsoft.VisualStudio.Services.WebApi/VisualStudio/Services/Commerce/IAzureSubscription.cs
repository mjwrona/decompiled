// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IAzureSubscription
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface IAzureSubscription
  {
    Guid Id { get; set; }

    SubscriptionStatus Status { get; set; }

    AccountProviderNamespace Namespace { get; set; }

    AzureOfferType? OfferType { get; set; }

    SubscriptionSource Source { get; set; }

    int AnniversaryDay { get; set; }

    DateTime Created { get; set; }

    DateTime LastUpdated { get; set; }
  }
}
