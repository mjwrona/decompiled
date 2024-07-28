// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent18
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent18 : BoardSettingsComponent17
  {
    public override void UpdateBoardOptions(
      Guid projectId,
      Guid boardId,
      int cardReordering,
      bool statusBadgeIsPublic = false)
    {
      this.PrepareStoredProcedure("prc_SetBoardOptions");
      this.BindDataspace(projectId);
      this.BindGuid("@boardId", boardId);
      this.BindInt("@cardReordering", cardReordering);
      this.BindBoolean("@statusBadgeIsPublic", statusBadgeIsPublic);
      this.ExecuteNonQuery();
    }
  }
}
