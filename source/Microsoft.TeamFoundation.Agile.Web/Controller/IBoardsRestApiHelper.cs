// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.IBoardsRestApiHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  internal interface IBoardsRestApiHelper
  {
    IAgileSettings AgileSettings { get; }

    List<string> ApplicableBacklogLevelIds { get; }

    Dictionary<string, string> BacklogLevelId2BacklogLevelName { get; }

    CommonStructureProjectInfo CssProjectInfo { get; }

    Dictionary<string, string> LowerBacklogLevelName2BacklogLevelId { get; }

    string GetBoardBacklogLevelIdByNameOrId(BoardService boardService, string board);

    Guid GetBoardIdFromNameOrId(BoardService boardService, string board);

    BoardSettings GetBoardSettings(string backlogLevelId);

    BoardSettings GetOrCreateBoardSettings(string backlogLevelId);

    Dictionary<string, BoardCardRules> ConvertCardRulesToRowView(
      BoardCardRuleSettings boardCardRuleSettings);
  }
}
