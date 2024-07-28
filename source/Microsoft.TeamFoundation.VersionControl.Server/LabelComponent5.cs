// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelComponent5
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelComponent5 : LabelComponent4
  {
    public override void CreateWorkspaceLabelLocal(
      string labelName,
      ItemPathPair labelScope,
      Guid ownerId,
      string comment,
      Guid workspaceOwnerId,
      string workspaceName,
      string localItem,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_CreateWorkspaceLabelLocal");
      this.BindString("@labelName", labelName, 64, true, SqlDbType.NVarChar);
      this.BindPreDataspaceServerItemPathPair("@labelScope", labelScope, true);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@comment", comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.ExecuteNonQuery();
    }
  }
}
