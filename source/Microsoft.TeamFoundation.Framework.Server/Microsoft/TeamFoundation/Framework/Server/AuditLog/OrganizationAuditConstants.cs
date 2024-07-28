// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.OrganizationAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class OrganizationAuditConstants
  {
    public static readonly string OrganizationArea = "Organization";
    public static readonly string Create = OrganizationAuditConstants.OrganizationArea + ".Create";
    public static readonly string Update = OrganizationAuditConstants.OrganizationArea + ".Update";
    public static readonly string Rename = OrganizationAuditConstants.Update + ".Rename";
    public static readonly string Restore = OrganizationAuditConstants.Update + ".Restore";
    public static readonly string UpdateOwner = OrganizationAuditConstants.Update + ".Owner";
    public static readonly string ForceUpdateOwner = OrganizationAuditConstants.Update + ".ForceUpdateOwner";
    public static readonly string Delete = OrganizationAuditConstants.Update + ".Delete";
    public static readonly string Connect = OrganizationAuditConstants.OrganizationArea + ".LinkToAAD";
    public static readonly string Disconnect = OrganizationAuditConstants.OrganizationArea + ".UnlinkFromAAD";
    public const string OrganizationId = "OrganizationId";
    public const string PreferredRegion = "PreferredRegion";
    public const string OrganizationName = "OrganizationName";
    public const string OriginalName = "OriginalName";
    public const string OldOwnerName = "OldOwnerName";
    public const string NewOwnerName = "NewOwnerName";
    public const string ForceUpdateReason = "ForceUpdateReason";
    public const string AADTenant = "AADTenant";
    public const string OldOrganizationName = "OldOrganizationName";
    public const string NewOrganizationName = "NewOrganizationName";
  }
}
