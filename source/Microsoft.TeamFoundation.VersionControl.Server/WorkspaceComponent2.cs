// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceComponent2
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceComponent2 : WorkspaceComponent
  {
    public override void UpdateWorkspace(
      Guid ownerId,
      string workspaceName,
      Guid updatedOwnerId,
      string updatedName,
      string updatedComment,
      string updatedComputerName,
      List<WorkingFolder> workingFolders,
      string originalSecurityToken,
      string newSecurityToken,
      WorkspaceOptions options,
      bool isLocalWorkspace)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkspace");
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindString("@updatedName", updatedName, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@updatedIdentity", updatedOwnerId);
      this.BindString("@updatedComment", updatedComment, -1, false, SqlDbType.NVarChar);
      this.BindString("@updatedComputerName", updatedComputerName, 64, true, SqlDbType.NVarChar);
      this.BindMappingTable("@folderList", (IEnumerable<Mapping>) workingFolders);
      this.BindGuid("@author", this.Author);
      this.BindGuid("@workspaceNamespaceGuid", SecurityConstants.WorkspaceSecurityNamespaceGuid);
      this.BindString("@originalSecurityToken", originalSecurityToken, 578, false, SqlDbType.NVarChar);
      this.BindString("@newSecurityToken", newSecurityToken, 578, false, SqlDbType.NVarChar);
      this.BindBoolean("@fileTime", (options & WorkspaceOptions.SetFileTimeToCheckin) != 0);
      this.ExecuteNonQuery();
    }
  }
}
