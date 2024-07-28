// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent10
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent10 : BoardSettingsComponent9
  {
    public override Dictionary<Guid, SortedSet<string>> GetBoardColumnSuggestedValues(
      Guid? projectId = null)
    {
      this.PrepareStoredProcedure("prc_GetBoardColumnSuggestedValues");
      if (projectId.HasValue)
        this.BindDataspace(projectId.Value);
      else
        this.BindNullValue("@dataspaceId", SqlDbType.Int);
      Dictionary<Guid, SortedSet<string>> columnSuggestedValues = new Dictionary<Guid, SortedSet<string>>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardSuggestedValueRecord>((ObjectBinder<BoardSuggestedValueRecord>) new BoardSuggestedValueRecordBinder());
        foreach (BoardSuggestedValueRecord suggestedValueRecord in resultCollection.GetCurrent<BoardSuggestedValueRecord>().Items)
        {
          SortedSet<string> sortedSet;
          if (!columnSuggestedValues.TryGetValue(suggestedValueRecord.ProjectId, out sortedSet))
          {
            sortedSet = new SortedSet<string>();
            columnSuggestedValues.Add(suggestedValueRecord.ProjectId, sortedSet);
          }
          sortedSet.Add(suggestedValueRecord.Value);
        }
      }
      return columnSuggestedValues;
    }

    public override Dictionary<Guid, SortedSet<string>> GetBoardRowSuggestedValues(Guid? projectId = null)
    {
      this.PrepareStoredProcedure("prc_GetBoardRowSuggestedValues");
      if (projectId.HasValue)
        this.BindDataspace(projectId.Value);
      else
        this.BindNullValue("@dataspaceId", SqlDbType.Int);
      Dictionary<Guid, SortedSet<string>> rowSuggestedValues = new Dictionary<Guid, SortedSet<string>>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BoardSuggestedValueRecord>((ObjectBinder<BoardSuggestedValueRecord>) new BoardSuggestedValueRecordBinder());
        foreach (BoardSuggestedValueRecord suggestedValueRecord in resultCollection.GetCurrent<BoardSuggestedValueRecord>().Items)
        {
          SortedSet<string> sortedSet;
          if (!rowSuggestedValues.TryGetValue(suggestedValueRecord.ProjectId, out sortedSet))
          {
            sortedSet = new SortedSet<string>();
            rowSuggestedValues.Add(suggestedValueRecord.ProjectId, sortedSet);
          }
          sortedSet.Add(suggestedValueRecord.Value);
        }
      }
      return rowSuggestedValues;
    }
  }
}
