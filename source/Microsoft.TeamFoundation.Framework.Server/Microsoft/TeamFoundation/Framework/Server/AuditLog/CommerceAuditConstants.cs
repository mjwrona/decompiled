// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.CommerceAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class CommerceAuditConstants
  {
    private static readonly string CommerceArea = "Billing.";
    public static readonly string SubscriptionLink = CommerceAuditConstants.CommerceArea + nameof (SubscriptionLink);
    public static readonly string SubscriptionUnlink = CommerceAuditConstants.CommerceArea + nameof (SubscriptionUnlink);
    public static readonly string SubscriptionUpdate = CommerceAuditConstants.CommerceArea + nameof (SubscriptionUpdate);
    public static readonly string LimitUpdate = CommerceAuditConstants.CommerceArea + nameof (LimitUpdate);
    public static readonly string PreviousSubscriptionGuid = nameof (PreviousSubscriptionGuid);
    public static readonly string NewSubscriptionGuid = nameof (NewSubscriptionGuid);
    public static readonly string MeterName = nameof (MeterName);
    public static readonly string PreviousLimitNumber = nameof (PreviousLimitNumber);
    public static readonly string LimitNumber = nameof (LimitNumber);
  }
}
