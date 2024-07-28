// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.SecurityAuditLogConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class SecurityAuditLogConstants
  {
    public const string EventSummary = "EventSummary";
    public const string ACEs = "ACEs";
    public const string AccessControlLists = "AccessControlLists";
    public const string Identities = "Identities";
    public const string NamespaceId = "NamespaceId";
    public const string NamespaceName = "NamespaceName";
    public const string Permissions = "Permissions";
    public const string Recurse = "Recurse";
    public const string Token = "Token";
    public const string Tokens = "Tokens";
    public const string SubjectDescriptor = "SubjectDescriptor";
    public const string SubjectDisplayName = "SubjectDisplayName";
    public const string ChangedPermission = "ChangedPermission";
    public const string PermissionModifiedTo = "PermissionModifiedTo";
    public const string EventSummaryType = "EventSummaryType";
    public const string IdentityDescriptors = "IdentityDescriptors";
    public const string IdentityIds = "IdentityIds";
    private static readonly string SecurityArea = "Security.";
    public static readonly string ModifyPermission = SecurityAuditLogConstants.SecurityArea + nameof (ModifyPermission);
    public static readonly string ResetPermission = SecurityAuditLogConstants.SecurityArea + nameof (ResetPermission);
    public static readonly string ModifyAccessControlLists = SecurityAuditLogConstants.SecurityArea + nameof (ModifyAccessControlLists);
    public static readonly string ResetAccessControlLists = SecurityAuditLogConstants.SecurityArea + nameof (ResetAccessControlLists);
    public static readonly string RemoveIdentityACEs = SecurityAuditLogConstants.SecurityArea + nameof (RemoveIdentityACEs);
    public static readonly string RemoveAccessControlLists = SecurityAuditLogConstants.SecurityArea + nameof (RemoveAccessControlLists);
    public static readonly string RemoveAllAccessControlLists = SecurityAuditLogConstants.SecurityArea + nameof (RemoveAllAccessControlLists);
    public static readonly string RemovePermission = SecurityAuditLogConstants.SecurityArea + nameof (RemovePermission);
  }
}
