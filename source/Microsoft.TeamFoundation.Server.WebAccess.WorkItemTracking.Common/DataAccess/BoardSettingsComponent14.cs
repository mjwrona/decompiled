// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent14
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent14 : BoardSettingsComponent13
  {
    public override IEnumerable<BoardRecord> GetBoardsByIds(IEnumerable<Guid> boardIds)
    {
      this.PrepareStoredProcedure("prc_GetBoardsByIds");
      this.BindGuidTable("@boardIds", boardIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardRecord>((ObjectBinder<BoardRecord>) this.GetBoardRecordBinder());
        return (IEnumerable<BoardRecord>) resultCollection.GetCurrent<BoardRecord>().Items;
      }
    }
  }
}
