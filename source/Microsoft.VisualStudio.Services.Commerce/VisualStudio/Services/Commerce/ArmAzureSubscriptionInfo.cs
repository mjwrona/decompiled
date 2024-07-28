// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmAzureSubscriptionInfo
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ArmAzureSubscriptionInfo
  {
    public string Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public string DisplayName { get; set; }

    public string State { get; set; }

    public ArmAzureSubscriptionPolicies SubscriptionPolicies { get; set; }

    public Guid TenantId { get; set; }
  }
}
