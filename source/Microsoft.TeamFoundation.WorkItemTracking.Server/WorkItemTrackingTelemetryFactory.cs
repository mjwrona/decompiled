// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingTelemetryFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTrackingTelemetryFactory
  {
    public static WorkItemTrackingTelemetry GetTelemetryObject(
      IVssRequestContext requestContext,
      string feature)
    {
      if (feature == QueryTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new QueryTelemetry(requestContext, feature);
      if (feature == TrendQueryTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new TrendQueryTelemetry(requestContext, feature);
      if (feature == WorkItemUpdateTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemUpdateTelemetry(requestContext, feature);
      if (feature == WorkItemMoveTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemMoveTelemetry(requestContext, feature);
      if (feature == WorkItemChangeTypeTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemChangeTypeTelemetry(requestContext, feature);
      if (feature == WorkItemTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemTelemetry(requestContext, feature);
      if (feature == WorkItemPageTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemPageTelemetry(requestContext, feature);
      if (feature == AttachmentTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new AttachmentTelemetry(requestContext, feature);
      if (feature == WorkItemIdentityTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemIdentityTelemetry(requestContext, feature);
      if (feature == ImsSyncTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new ImsSyncTelemetry(requestContext, feature);
      if (feature == WorkItemRestoreTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemRestoreTelemetry(requestContext, feature);
      if (feature == WorkItemMetadataTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemMetadataTelemetry(requestContext, feature);
      if (feature == WorkItemTypeExtensionsReconciliationTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemTypeExtensionsReconciliationTelemetry(requestContext, feature);
      if (feature == WorkItemTypeExtensionsReconciliationJobTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new WorkItemTypeExtensionsReconciliationJobTelemetry(requestContext, feature);
      if (feature == RemoteLinkingTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new RemoteLinkingTelemetry(requestContext, feature);
      if (feature == ResourceLinkingTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new ResourceLinkingTelemetry(requestContext, feature);
      if (feature == CodeReviewTelemetry.Feature)
        return (WorkItemTrackingTelemetry) new CodeReviewTelemetry(requestContext, feature);
      return feature == WorkItemIdentityEagerSyncTelemetry.Feature ? (WorkItemTrackingTelemetry) new WorkItemIdentityEagerSyncTelemetry(requestContext, feature) : (WorkItemTrackingTelemetry) null;
    }
  }
}
