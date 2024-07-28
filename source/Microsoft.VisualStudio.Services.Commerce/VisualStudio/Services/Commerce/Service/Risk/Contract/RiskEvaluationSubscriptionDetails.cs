// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract.RiskEvaluationSubscriptionDetails
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract
{
  public class RiskEvaluationSubscriptionDetails
  {
    public Guid subscription_id { get; set; } = Guid.Empty;

    public string subscription_status { get; set; }
  }
}
