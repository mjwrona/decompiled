// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract.RiskEvaluationEventDetails
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract
{
  public class RiskEvaluationEventDetails
  {
    public string order_id { get; set; }

    public RiskEvaluationAccountDetails account_details { get; set; }

    public RiskEvaluationBillingDetails billing_details { get; set; }

    public List<RiskEvaluationPaymentInstrument> payment_instruments { get; set; }

    public RiskEvaluationOfferDetails offer_details { get; set; }

    public string client { get; set; }

    public List<RiskEvaluationOrderItem> order_line_items { get; set; }

    public RiskEvaluationCatalogDetails catalog_details { get; set; }

    public RiskEvaluationDeviceDetails device_details { get; set; }

    public RiskEvaluationSubscriptionDetails subscription_details { get; set; }

    public string quota_id { get; set; }
  }
}
