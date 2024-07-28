// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent15
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent15 : BoardSettingsComponent14
  {
    public override BoardSettingsDTO RestoreBoard(
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      this.PrepareStoredProcedure("prc_RestoreBoard");
      this.BindDataspace(projectId);
      this.BindGuid("@teamId", teamId);
      this.BindString("@categoryReferenceName", backlogLevelId, 256, false, SqlDbType.NVarChar);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetBoardSettings(rc);
    }

    public override void SoftDeleteBoards(IEnumerable<Guid> boardIds)
    {
      this.PrepareStoredProcedure("prc_SoftDeleteBoards");
      this.BindGuidTable("@boardIds", boardIds);
      this.ExecuteNonQuery();
    }
  }
}
