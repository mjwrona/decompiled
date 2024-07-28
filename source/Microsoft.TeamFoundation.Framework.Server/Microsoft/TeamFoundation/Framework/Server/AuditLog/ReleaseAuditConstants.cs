// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ReleaseAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ReleaseAuditConstants
  {
    private static readonly string ReleaseArea = "Release.";
    public static readonly string ReleasePipelineCreated = ReleaseAuditConstants.ReleaseArea + nameof (ReleasePipelineCreated);
    public static readonly string ReleasePipelineDeleted = ReleaseAuditConstants.ReleaseArea + nameof (ReleasePipelineDeleted);
    public static readonly string ReleasePipelineModified = ReleaseAuditConstants.ReleaseArea + nameof (ReleasePipelineModified);
    public static readonly string ReleaseCreated = ReleaseAuditConstants.ReleaseArea + nameof (ReleaseCreated);
    public static readonly string ReleaseDeleted = ReleaseAuditConstants.ReleaseArea + nameof (ReleaseDeleted);
    public static readonly string ApprovalCompleted = ReleaseAuditConstants.ReleaseArea + nameof (ApprovalCompleted);
    public static readonly string ApprovalsCompleted = ReleaseAuditConstants.ReleaseArea + nameof (ApprovalsCompleted);
    public static readonly string DeploymentCompleted = ReleaseAuditConstants.ReleaseArea + nameof (DeploymentCompleted);
    public static readonly string DeploymentsCompleted = ReleaseAuditConstants.ReleaseArea + nameof (DeploymentsCompleted);
  }
}
