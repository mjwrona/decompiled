// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IBoardUserSettingsUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public interface IBoardUserSettingsUtils
  {
    void SetBoardUserSettings(
      IVssRequestContext requestContext,
      Guid projectGuid,
      WebApiTeam team,
      Guid boardId,
      Dictionary<string, string> userSettings);

    BoardUserSettings GetBoardUserSettings(
      IVssRequestContext requestContext,
      Guid projectGuid,
      WebApiTeam team,
      Guid boardId);
  }
}
