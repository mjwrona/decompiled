// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ArtifactsAuditingConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ArtifactsAuditingConstants
  {
    private static readonly string prefix = "Artifacts.";
    private static readonly string feedPath = ArtifactsAuditingConstants.prefix + "Feed.";
    private static readonly string feedOrgPath = ArtifactsAuditingConstants.feedPath + "Org.";
    private static readonly string dropPath = ArtifactsAuditingConstants.prefix + "Drop.";
    private static readonly string dropOrgPath = ArtifactsAuditingConstants.dropPath + "Org.";
    private static readonly string feedProjectPath = ArtifactsAuditingConstants.feedPath + "Project.";
    private static readonly string feedOrgViewPath = ArtifactsAuditingConstants.feedOrgPath + "FeedView.";
    private static readonly string feedProjectViewPath = ArtifactsAuditingConstants.feedProjectPath + "FeedView.";
    public static readonly string FeedOrgCreate = ArtifactsAuditingConstants.feedOrgPath + "Create";
    public static readonly string FeedProjectCreate = ArtifactsAuditingConstants.feedProjectPath + "Create";
    public static readonly string FeedOrgCreateWithUpstreams = ArtifactsAuditingConstants.feedOrgPath + "Create.Upstreams";
    public static readonly string FeedProjectCreateWithUpstreams = ArtifactsAuditingConstants.feedProjectPath + "Create.Upstreams";
    public static readonly string FeedOrgViewCreate = ArtifactsAuditingConstants.feedOrgViewPath + "Create";
    public static readonly string FeedProjectViewCreate = ArtifactsAuditingConstants.feedProjectViewPath + "Create";
    public static readonly string FeedOrgSoftDelete = ArtifactsAuditingConstants.feedOrgPath + "SoftDelete";
    public static readonly string FeedProjectSoftDelete = ArtifactsAuditingConstants.feedProjectPath + "SoftDelete";
    public static readonly string FeedOrgHardDelete = ArtifactsAuditingConstants.feedOrgPath + "HardDelete";
    public static readonly string FeedProjectHardDelete = ArtifactsAuditingConstants.feedProjectPath + "HardDelete";
    public static readonly string FeedOrgViewDelete = ArtifactsAuditingConstants.feedOrgViewPath + "HardDelete";
    public static readonly string FeedProjectViewDelete = ArtifactsAuditingConstants.feedProjectViewPath + "HardDelete";
    public static readonly string FeedOrgModify = ArtifactsAuditingConstants.feedOrgPath + "Modify";
    public static readonly string FeedProjectModify = ArtifactsAuditingConstants.feedProjectPath + "Modify";
    public static readonly string FeedOrgViewModify = ArtifactsAuditingConstants.feedOrgViewPath + "Modify";
    public static readonly string FeedProjectViewModify = ArtifactsAuditingConstants.feedProjectViewPath + "Modify";
    public static readonly string FeedOrgPermissionModified = ArtifactsAuditingConstants.FeedOrgModify + ".Permissions";
    public static readonly string FeedProjectPermissionModified = ArtifactsAuditingConstants.FeedProjectModify + ".Permissions";
    public static readonly string FeedOrgPermissionDeleted = ArtifactsAuditingConstants.FeedOrgModify + ".Permissions.Deletion";
    public static readonly string FeedProjectPermissionDeleted = ArtifactsAuditingConstants.FeedProjectModify + ".Permissions.Deletion";
    public static readonly string DropOrgDelete = ArtifactsAuditingConstants.dropOrgPath + "Delete";
    public static readonly string DropOrgModify = ArtifactsAuditingConstants.dropOrgPath + "Modify";
  }
}
