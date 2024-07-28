// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AzCommAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class AzCommAuditConstants
  {
    private static readonly string AzCommArea = "Billing.";
    public static readonly string PurchaseUpdate = AzCommAuditConstants.AzCommArea + nameof (PurchaseUpdate);
    public static readonly string LimitUpdate = AzCommAuditConstants.AzCommArea + nameof (LimitUpdate);
    public static readonly string BillingModeUpdate = AzCommAuditConstants.AzCommArea + nameof (BillingModeUpdate);
    public static readonly string MeterName = nameof (MeterName);
    public static readonly string PreviousPurchaseNumber = nameof (PreviousPurchaseNumber);
    public static readonly string PreviousLimitNumber = nameof (PreviousLimitNumber);
    public static readonly string PurchaseNumber = nameof (PurchaseNumber);
    public static readonly string LimitNumber = nameof (LimitNumber);
    public static readonly string BillingMode = nameof (BillingMode);
    public static readonly string SubscriptionGuid = nameof (SubscriptionGuid);
  }
}
