// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results.MappingPanelQueryResults
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results
{
  [DataContract]
  public class MappingPanelQueryResults : QueryResultModel
  {
    internal MappingPanelQueryResults()
    {
    }

    public MappingPanelQueryResults(
      IVssRequestContext requestContext,
      string wiql,
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryInfo,
      IDictionary<string, int> fieldWidthMap,
      IEnumerable<MruTeamInfo> teamsMru)
      : base(requestContext, wiql, queryInfo, fieldWidthMap)
    {
      this.TeamsMru = teamsMru;
      foreach (QueryDisplayColumn column in this.Columns)
        column.CanSortBy = false;
    }

    [DataMember(Name = "teamsMru")]
    public IEnumerable<MruTeamInfo> TeamsMru { get; }
  }
}
