// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardRowSuggestedValuesApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "boardrows")]
  public class BoardRowSuggestedValuesApiController : BoardSuggestedValuesApiControllerBase
  {
    [HttpGet]
    [ClientExample("GET__work_boardRows_ByProject.json", "Get available board rows in a project", null, null)]
    public IList<BoardSuggestedValue> GetRowSuggestedValues() => (IList<BoardSuggestedValue>) this.TfsRequestContext.GetService<BoardService>().GetRowSuggestedValues(this.TfsRequestContext, this.ProjectGuid).Select<string, BoardSuggestedValue>((Func<string, BoardSuggestedValue>) (value => new BoardSuggestedValue()
    {
      Name = value
    })).ToList<BoardSuggestedValue>();
  }
}
