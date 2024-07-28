// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ProjectAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ProjectAuditConstants
  {
    public const string ProjectId = "ProjectId";
    public const string ProjectName = "ProjectName";
    public const string PreviousProjectName = "PreviousProjectName";
    public const string ProcessTemplate = "ProcessTemplate";
    public const string ProcessName = "ProcessName";
    public const string OldProcessName = "OldProcessName";
    public const string ProjectVisibility = "ProjectVisibility";
    public const string PreviousProjectVisibility = "PreviousProjectVisibility";
    public const string ProjectDeleteType = "ProjectDeleteType";
    public static readonly string AreaPathCreate = "AreaPath.Create";
    public static readonly string AreaPathUpdate = "AreaPath.Update";
    public static readonly string AreaPathDelete = "AreaPath.Delete";
    public static readonly string IterationPathCreate = "IterationPath.Create";
    public static readonly string IterationPathUpdate = "IterationPath.Update";
    public static readonly string IterationPathDelete = "IterationPath.Delete";
    private static readonly string Project = "Project.";
    public static readonly string Create = ProjectAuditConstants.Project + nameof (Create);
    public static readonly string Update = ProjectAuditConstants.Project + nameof (Update);
    public static readonly string Rename = ProjectAuditConstants.Update + nameof (Rename);
    public static readonly string Visibility = ProjectAuditConstants.Update + nameof (Visibility);
    [Obsolete("A breaking change needed to be made to the string resources for project delete messages so this ID is deprecated in favor of Delete2")]
    public static readonly string Delete = ProjectAuditConstants.Project + nameof (Delete);
    public static readonly string SoftDelete = ProjectAuditConstants.Project + nameof (SoftDelete);
    public static readonly string HardDelete = ProjectAuditConstants.Project + nameof (HardDelete);
    public static readonly string Restore = ProjectAuditConstants.Project + nameof (Restore);
    public static readonly string ProjectAreaPathCreate = ProjectAuditConstants.Project + ProjectAuditConstants.AreaPathCreate;
    public static readonly string ProjectAreaPathUpdate = ProjectAuditConstants.Project + ProjectAuditConstants.AreaPathUpdate;
    public static readonly string ProjectAreaPathDelete = ProjectAuditConstants.Project + ProjectAuditConstants.AreaPathDelete;
    public static readonly string ProjectIterationPathCreate = ProjectAuditConstants.Project + ProjectAuditConstants.IterationPathCreate;
    public static readonly string ProjectIterationPathUpdate = ProjectAuditConstants.Project + ProjectAuditConstants.IterationPathUpdate;
    public static readonly string ProjectIterationPathDelete = ProjectAuditConstants.Project + ProjectAuditConstants.IterationPathDelete;
    public static readonly string ProcessModified = "Process.Modify";
    public static readonly string ProjectProcessModified = ProjectAuditConstants.Project + ProjectAuditConstants.ProcessModified;
    public static readonly string ProjectProcessModifiedWithoutOldProcess = ProjectAuditConstants.Project + ProjectAuditConstants.ProcessModified + "WithoutOldProcess";
  }
}
