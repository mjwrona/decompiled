// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.GroupAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class GroupAuditConstants
  {
    public static readonly string GroupArea = "Group.";
    public static readonly string Create = GroupAuditConstants.GroupArea + "CreateGroups";
    public static readonly string Update = GroupAuditConstants.GroupArea + "UpdateGroups";
    public static readonly string Modify = GroupAuditConstants.Update + ".Modify";
    public static readonly string Delete = GroupAuditConstants.Update + ".Delete";
    public static readonly string MembershipUpdate = GroupAuditConstants.GroupArea + "UpdateGroupMembership";
    public static readonly string MembershipUpdateAdd = GroupAuditConstants.MembershipUpdate + ".Add";
    public static readonly string MembershipUpdateRemove = GroupAuditConstants.MembershipUpdate + ".Remove";
    public const string Groups = "groups";
    public const string GroupDeletes = "groupDeletes";
    public const string GroupUpdates = "groupUpdates";
    public const string Updates = "updates";
    public const string GroupId = "GroupId";
    public const string GroupName = "GroupName";
    public const string MemberId = "MemberId";
    public const string MemberDisplayName = "MemberDisplayName";
    public const string GroupDescription = "GroupDescription";
  }
}
