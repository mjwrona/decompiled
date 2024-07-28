// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent17
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent17 : BoardSettingsComponent16
  {
    public override IEnumerable<BoardRecord> GetBoardsByCategoryReferenceNames(
      IEnumerable<string> categoryReferenceNames)
    {
      this.PrepareStoredProcedure("prc_GetBoardsByCategoryReferenceNames");
      this.BindStringTable("@categoryRefNames", categoryReferenceNames);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardRecord>((ObjectBinder<BoardRecord>) this.GetBoardRecordBinder());
        return (IEnumerable<BoardRecord>) resultCollection.GetCurrent<BoardRecord>().Items;
      }
    }
  }
}
