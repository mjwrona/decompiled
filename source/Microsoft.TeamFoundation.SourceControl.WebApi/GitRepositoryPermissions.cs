// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRepositoryPermissions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Flags]
  [GenerateAllConstants(null)]
  public enum GitRepositoryPermissions
  {
    None = 0,
    [Obsolete("In M112, Administer was replaced with CreateRepository, DeleteRepository, RenameRepository, EditPolicies, RemoveOthersLocks, and ManagePermissions")] Administer = 1,
    GenericRead = 2,
    GenericContribute = 4,
    ForcePush = 8,
    CreateBranch = 16, // 0x00000010
    CreateTag = 32, // 0x00000020
    ManageNote = 64, // 0x00000040
    PolicyExempt = 128, // 0x00000080
    CreateRepository = 256, // 0x00000100
    DeleteRepository = 512, // 0x00000200
    RenameRepository = 1024, // 0x00000400
    EditPolicies = 2048, // 0x00000800
    RemoveOthersLocks = 4096, // 0x00001000
    ManagePermissions = 8192, // 0x00002000
    PullRequestContribute = 16384, // 0x00004000
    PullRequestBypassPolicy = 32768, // 0x00008000
    ViewAdvSecAlerts = 65536, // 0x00010000
    DismissAdvSecAlerts = 131072, // 0x00020000
    ManageAdvSecScanning = 262144, // 0x00040000
    ProjectLevelPermissions = ManageAdvSecScanning | DismissAdvSecAlerts | ViewAdvSecAlerts | PullRequestBypassPolicy | PullRequestContribute | ManagePermissions | RemoveOthersLocks | EditPolicies | RenameRepository | DeleteRepository | CreateRepository | PolicyExempt | ManageNote | CreateTag | CreateBranch | ForcePush | GenericContribute | GenericRead, // 0x0007FFFE
    RepositoryLevelPermissions = ManageAdvSecScanning | DismissAdvSecAlerts | ViewAdvSecAlerts | PullRequestBypassPolicy | PullRequestContribute | ManagePermissions | RemoveOthersLocks | EditPolicies | RenameRepository | DeleteRepository | PolicyExempt | ManageNote | CreateTag | CreateBranch | ForcePush | GenericContribute | GenericRead, // 0x0007FEFE
    BranchLevelPermissions = PullRequestBypassPolicy | ManagePermissions | RemoveOthersLocks | EditPolicies | PolicyExempt | ForcePush | GenericContribute, // 0x0000B88C
    CreateBranchPermissions = PullRequestBypassPolicy | ManagePermissions | RemoveOthersLocks | PolicyExempt | ForcePush | GenericContribute, // 0x0000B08C
    NonBranchRefLevelPermissions = ManagePermissions | ForcePush, // 0x00002008
    BranchesRootLevelPermissions = NonBranchRefLevelPermissions | PullRequestBypassPolicy | RemoveOthersLocks | EditPolicies | PolicyExempt | CreateBranch | GenericContribute, // 0x0000B89C
  }
}
